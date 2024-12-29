using Newtonsoft.Json;
using OpenNGS.Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace OpenNGS.Platform.EEGames
{
    public class EEGamesSDKProvider : ISDKProvider
    {
        public IModuleProvider CreateProvider(PLATFORM_MODULE module)
        {
            if(module == PLATFORM_MODULE.LOGIN)
            {
                return new EEGamesLoginProvider();
            }
            else if( module == PLATFORM_MODULE.REPORT)
            {
                return new EEGamesReportProvider();
            }
            return null;
        }
    }
    public class EEGamesCallBack : IThirdpartyCallBack
    {
        public void OnCallBack(string moduleName, string funcName, string result)
        {
        }

        public void OnException(int code, string msg)
        {
        }
    }
    public class EEGamesLoginProvider : ILoginProvider
    {
        public PLATFORM_MODULE Module => PLATFORM_MODULE.LOGIN;
        private EEGamesCallBack m_callBack;
        public EEGamesLoginProvider()
        {
            m_callBack = new EEGamesCallBack();
            PlatformCallback.Instance.Init(m_callBack);
        }
        public void AutoLogin()
        {
            throw new System.NotImplementedException();
        }

        public PlatformLoginRet GetLoginRet()
        {
            throw new System.NotImplementedException();
        }

        private void _callBackLogin(PlatformLoginRet _ret)
        {
            PlatformCallback.Instance.OnCallBack(_ret);
        }

        private const string api_login_path = "/api/auth/login";
        private const string url_login = "http://api.eegames.com/services/platform/auth";
        public void Login(string channel, string permissions = "", string subChannel = "", string extraJson = "")
        {
            string url = url_login + api_login_path;
            PlatformLoginRet _ret = new PlatformLoginRet();
            PlatformCallback.Instance.StartCoroutine(WebPost(url,
                extraJson, _ret, _callBackLogin));

        }

        private IEnumerator WebPost(string uri, string data, PlatformLoginRet ret, UnityAction<PlatformLoginRet> callback)
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
                || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(webRequest.error);
                ret.RetCode = PlatformError.NETWORK_ERROR;
                ret.RetMsg = webRequest.error;
            }
            else
            {
                string returnJson = webRequest.downloadHandler.text;
                ret.ThirdMsg = returnJson;
                ret.RetCode = PlatformError.SUCCESS;
                // 域名没有定，现在用本地
                //get api/user 
                Debug.Log("WebRequest Ret:" + returnJson);
                //object _obj = JsonConvert.DeserializeObject(returnJson);
                Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(returnJson);
                if(dictionary.TryGetValue("data", out string strVal))
                {
                    ret.Token = strVal;
                }
                //Dictionary<string, string> resultData = JsonUtil.LoadJson<Dictionary<string, string>>(returnJson);
                //ret.RetCode = (int)resultData["ret"];
                //ret.RetMsg = resultData["msg"].ToString();
                //if (ret.RetCode == PlatformError.SUCCESS)
                //{
                //    ret.OpenId = resultData["openid"].ToString();
                //}
                int a = 0;
            }
            if (callback != null)
                callback(ret);
        }

        private IEnumerator WebRequest(string url, string data, PlatformLoginRet ret, UnityAction<PlatformLoginRet> callback)
        {
            Debug.LogFormat("[WebRequest]url:{0} data:[{1}]", url, data);
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("Accept", "application/json");
                www.method = UnityWebRequest.kHttpVerbPOST;
                yield return www.SendWebRequest();
                Debug.Log("[WebRequest]responseCode:" + www.responseCode);
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                    ret.RetCode = PlatformError.NETWORK_ERROR;
                    ret.RetMsg = www.error;
                }
                else
                {
                    string returnJson = www.downloadHandler.text;
                    // 域名没有定，现在用本地
                    //get api/user 
                    Debug.Log("WebRequest Ret:" + returnJson);
                    //var resultData = JsonMapper.ToObject(returnJson);
                    //ret.RetCode = (int)resultData["ret"];
                    //ret.RetMsg = resultData["msg"].ToString();
                    //if (ret.RetCode == PlatformError.SUCCESS)
                    //{
                    //    ret.OpenId = resultData["openid"].ToString();
                    //}
                }
                if (callback != null)
                    callback(ret);
            }
        }

        public void Logout(string channel = "")
        {
            throw new System.NotImplementedException();
        }

        public void SwitchUser(bool useLaunchUser)
        {
            throw new System.NotImplementedException();
        }
    }
}

