using Newtonsoft.Json;
using OpenNGS.SDK.Auth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.SDK.Core.Network
{
    public class NetworkHandler : INetworkHandler
    {
        private readonly HttpClient _httpClient;

        public NetworkHandler()
        {
            _httpClient = new HttpClient();
        }


        public async Task<T> SendAsync<T>(HttpMethod method, string url, Dictionary<string, string> headers = null,
                                            object content = null, string contentType = "application/json",
                                            Dictionary<string, string> queryParameters = null)
        {
            if (queryParameters != null)
            {
                var query = new FormUrlEncodedContent(queryParameters);
                url += "?" + query.ReadAsStringAsync().Result;
            }

            // 添加Headers
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            try
            {
                // 处理请求内容
                HttpContent httpContent = null;
                if (content != null)
                {
                    if (contentType == "application/x-www-form-urlencoded")
                    {
                        var formContent = new FormUrlEncodedContent(content as Dictionary<string, string>);
                        httpContent = formContent;
                    }
                    else if (contentType == "multipart/form-data")
                    {
                        httpContent = new MultipartFormDataContent();
                        var contentDictionary = ObjectToDictionary(content);
                        if (contentDictionary != null)
                        {
                            foreach (var item in contentDictionary)
                            {
                                if (item.Value is Stream stream)
                                {
                                    // 如果是文件流，创建一个ByteArrayContent或StreamContent
                                    var fileContent = new StreamContent(stream);
                                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                                    {
                                        Name = "filename"
                                    };
                                    (httpContent as MultipartFormDataContent).Add(fileContent, item.Key);
                                }
                                else
                                {
                                    // 其他字段，创建一个StringContent
                                    (httpContent as MultipartFormDataContent).Add(new StringContent(item.Value.ToString()), item.Key);
                                }
                            }
                        }
                    }
                    else
                    {
                        httpContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, contentType);
                    }
                }

                // 发送请求
                HttpResponseMessage response = null;
                switch (method.Method)
                {
                    case "GET":
                        response = await _httpClient.GetAsync(url);
                        break;
                    case "POST":
                        response = await _httpClient.PostAsync(url, httpContent);
                        break;
                    case "PUT":
                        response = await _httpClient.PutAsync(url, httpContent);
                        break;
                    case "DELETE":
                        response = await _httpClient.DeleteAsync(url);
                        break;
                    default:
                        Console.Error.WriteLine($"The HTTP method {method.Method} is not supported.");
                        break;
                }

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(result);
                }
                else
                {
                    // 处理非成功状态码，例如400
                    Console.Error.WriteLine($"Error: {response.StatusCode}");
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.Error.WriteLine($"Error details: {errorContent}");
                    return default;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine(ex.Message);
                return default;
            }
        }

        public static Dictionary<string, object> ObjectToDictionary(object obj)
        {
            var dictionary = new Dictionary<string, object>();
            if (obj == null)
                return dictionary;

            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var value = property.GetValue(obj);
                dictionary.Add(property.Name, value);
            }

            return dictionary;
        }

        // 发送GET请求
        public async Task<T> GetAsync<T>(string url, Dictionary<string, string> queryParameters = null, Dictionary<string, string> headers = null)
        {
            return await SendAsync<T>(HttpMethod.Get, url, headers, null, "application/json", queryParameters);
        }

        // 发送POST请求
        public async Task<T> PostAsync<T>(string url, object content, string contentType = "application/json", Dictionary<string, string> headers = null)
        {
            return await SendAsync<T>(HttpMethod.Post, url, headers, content, contentType, null);
        }

        // 发送POST FormData 请求
        public async Task<T> PostFormAsync<T>(string url, object content, string contentType = "multipart/form-data", Dictionary<string, string> headers = null)
        {
            return await SendAsync<T>(HttpMethod.Post, url, headers, content, contentType, null);
        }

        // 发送PUT请求
        public async Task<T> PutAsync<T>(string url, object content, string contentType = "application/json", Dictionary<string, string> headers = null)
        {
            return await SendAsync<T>(HttpMethod.Put, url, headers, content, contentType, null);
        }

        // 发送DELETE请求
        public async Task<T> DeleteAsync<T>(string url, Dictionary<string, string> headers = null)
        {
            return await SendAsync<T>(HttpMethod.Delete, url, headers, null, null, null);
        }

        public Task<bool> DownloadFile(string fileUrl, string filePath)
        {
            var tcs = new TaskCompletionSource<bool>();

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // 发送GET请求
                    HttpResponseMessage response = client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead).Result;
                    response.EnsureSuccessStatusCode(); // 确保响应状态码为200-299

                    // 创建文件流以写入文件
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    using (HttpContent content = response.Content)
                    {
                        // 将响应内容复制到文件流
                        content.CopyToAsync(fileStream).ContinueWith(t =>
                        {
                            if (t.IsFaulted)
                            {
                                // 如果复制失败，设置任务为失败状态
                                tcs.SetException(t.Exception);
                                tcs.SetResult(false);
                            }
                            else if (t.IsCanceled)
                            {
                                // 如果复制被取消，设置任务为取消状态
                                tcs.SetCanceled();
                                tcs.SetResult(false);
                            }
                            else
                            {
                                // 如果复制成功，设置任务为成功状态
                                tcs.SetResult(true);
                                Console.WriteLine("文件下载完成！");
                            }
                        }, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.OnlyOnCanceled);
                    }
                }
            }
            catch (HttpRequestException e)
            {
                // 如果捕获到异常，设置任务为失败状态
                tcs.SetException(e);
                tcs.SetResult(false);
                Log.Error($"\nException Caught! {e.Message}");
            }

            return tcs.Task;
        }
    }
}