using Newtonsoft.Json;
using OpenNGS.SDK.Core.Initiallization;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using static OpenNGS.Platform.PlatformReport;

namespace OpenNGS.Platform.EEGames
{
    public class EEGamesReportProvider : IReportProvider
    {
        private const string api_path = "/event";
        //private const string urlpath = "https://s.eegames.com";
        private string urlpath = "http://s.pre.eegames.net";
        public PLATFORM_MODULE Module => PLATFORM_MODULE.REPORT;
        public EEGamesReportProvider(InitializationOptions options)
        {
            urlpath = options.UrlReport;
        }
        ~EEGamesReportProvider()
        {
        }
        public void Report(string eventId, ExtraInfo extraInfo)
        {
#if !UNITY_EDITOR
            extraInfo.extraMap["eventname"] = eventId;
            string url = urlpath + api_path;
            string strExtraInfo = JsonConvert.SerializeObject(extraInfo);
            PlatformReportRet _ret = new PlatformReportRet();
            PlatformCallback.Instance.StartCoroutine(WebPost(url,
                strExtraInfo, _ret, _callBackReport));
#endif
        }

        private void _callBackReport(PlatformReportRet _ret)
        {
            PlatformCallback.Instance.OnReportCallBack(_ret);
        }
        private IEnumerator WebPost(string uri, string data, PlatformReportRet ret, UnityAction<PlatformReportRet> callback)
        {
            // 创建 UnityWebRequest 对象
            UnityWebRequest webRequest = new UnityWebRequest(uri, "POST");

            // 将 JSON 字符串转换为 byte[]
            byte[] bodyRaw = Encoding.UTF8.GetBytes(data);

            // 设置请求体
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            // 设置请求头
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            // 发送请求
            yield return webRequest.SendWebRequest();

            Debug.Log("[WebRequest]responseCode:" + webRequest.responseCode);
            if (webRequest.result == UnityWebRequest.Result.ConnectionError
                || webRequest.result == UnityWebRequest.Result.ProtocolError
                || webRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.Log(webRequest.error);
                NgDebug.LogException(new System.Exception(string.Format("[WebRequest]Result {0} Error {1}", webRequest.result, webRequest.error)));
                ret.RetCode = PlatformError.NETWORK_ERROR;
                ret.RetMsg = webRequest.error;
            }
            else
            {
                string returnJson = webRequest.downloadHandler.text;
                ret.ThirdMsg = returnJson;
                ret.RetCode = PlatformError.SUCCESS;
                Debug.Log("WebRequest Ret:" + returnJson);
                //Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(returnJson);
                //if (dictionary.TryGetValue("data", out string strVal))
                //{
                //    ret.Token = strVal;
                //}
            }
            if (callback != null)
                callback(ret);
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Update()
        {
        }
    }
}