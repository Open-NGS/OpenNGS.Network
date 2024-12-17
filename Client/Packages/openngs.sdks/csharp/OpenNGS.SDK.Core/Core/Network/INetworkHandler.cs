using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.SDK.Core.Network
{
    public interface INetworkHandler
    {
        Task<T> GetAsync<T>(string url, Dictionary<string, string> queryParameters = null, Dictionary<string, string> headers = null);

        Task<T> PostAsync<T>(string url, object content, string contentType = "application/json", Dictionary<string, string> headers = null);

        Task<T> PostFormAsync<T>(string url, object content, string contentType = "multipart/form-data", Dictionary<string, string> headers = null);

        Task<T> PutAsync<T>(string url, object content, string contentType = "application/json", Dictionary<string, string> headers = null);

        Task<T> DeleteAsync<T>(string url, Dictionary<string, string> headers = null);

        Task<bool> DownloadFile(string url, string path);
    }
}
