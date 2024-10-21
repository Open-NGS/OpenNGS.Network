using Newtonsoft.Json;
using OpenNGS.Extension;
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
            return null;
        }
    }
    public class EEGamesLoginProvider : ILoginProvider
    {
        public PLATFORM_MODULE Module => PLATFORM_MODULE.LOGIN;

        public void AutoLogin()
        {
            throw new System.NotImplementedException();
        }

        public PlatformLoginRet GetLoginRet()
        {
            throw new System.NotImplementedException();
        }

        private void _callBack(PlatformLoginRet _ret)
        {

        }

        private const string api_login_path = "/api/login";
        private const string url_login = "http://192.168.10.101:7228";
        //private const string url_login = "https://git.eegames.net/";
        private const string tokein_sample = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJBcHBJZCI6IjliZDM4MzVjLWEwMzUtNGYxMy05NDRkLTg0Y2Q5OTIyNjdkNCIsIlVzZXJJZCI6IjMiLCJuYmYiOjE3MjkyMzA2OTcsImV4cCI6MTcyOTIzMjQ5NywiaXNzIjoiRWVnYW1lcyIsImF1ZCI6IkVlZ2FtZXMifQ.wjExy6bcrZzs9_z_c7siIfvHHICBVZwUm1iFd6cW1Bo";
        public void Login(string channel, string permissions = "", string subChannel = "", string extraJson = "")
        {
            //string strUrl = "https://auth.eegames.com/login/oauth/authorize?response_type=code&scope=openid%20profile%20email%20phone&redirect_uri=http:%2F%2Fplatform-api.openngs.com%2Fapi%2Feauth%2Fcallback&client_id=1c35349a39b34aa7b144&state=%2F";
            //Application.OpenURL(strUrl);
            //PlatformLoginRet _ret = new PlatformLoginRet();
            //PlatformCallback.Instance.StartCoroutine(WebRequest(strUrl,
            //    "", _ret, _callBack));
            string strUrl = url_login;
            //string url = url_login + api_login_path + "&channel=" + 0 + "&verification=" + 0 + "&data="+ tokein_sample;
            string url = url_login + api_login_path;
            PlatformLoginRet _ret = new PlatformLoginRet();
            PlatformCallback.Instance.StartCoroutine(WebPost(url,
                "", _ret, _callBack));
        }

        private IEnumerator WebPost(string uri, string data, PlatformLoginRet ret, UnityAction<PlatformLoginRet> callback)
        {
            string jsonString = "{\"channel\":0,\"verification\":0,\"data\":\"" + tokein_sample + "\"}";

            // 创建 UnityWebRequest 对象
            UnityWebRequest webRequest = new UnityWebRequest(uri, "POST");

            // 将 JSON 字符串转换为 byte[]
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);

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
                // 域名没有定，现在用本地
                //get api/user 
                Debug.Log("WebRequest Ret:" + returnJson);
                //object _obj = JsonConvert.DeserializeObject(returnJson);
                Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(returnJson);
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

