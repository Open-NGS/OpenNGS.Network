using OpenNGS.Platform;
using System.Collections;
using System.Collections.Generic;
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

        public void Login(string channel, string permissions = "", string subChannel = "", string extraJson = "")
        {
            //Application.OpenURL("https://www.eegames.com/");
            //PlatformLoginRet _ret = new PlatformLoginRet();
            //PlatformCallback.Instance.StartCoroutine(WebRequest("",
            //    "", _ret, _callBack));
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

