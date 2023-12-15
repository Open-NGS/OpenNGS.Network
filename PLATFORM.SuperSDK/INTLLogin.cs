#if SUPERSDK
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace OpenNGS.Platform
{
    public class INTLLogin
    {
        private const string api_openid_path = "/v2/profile/openid";
        private const string channelid = "100"; // 固定100

        private static string OS
        {
            get
            {
#if UNITY_ANDROID
                return "1";
#elif UNITY_IOS
                return "2";
#else
                return "5";
#endif
            }
        }

        public static void GetOpenId(PlatformLoginRet ret, UnityAction<PlatformLoginRet> callback)
        {
            Debug.LogFormat("GetOpenId:uid:{0}", ret.OpenId);
            string api_path = api_openid_path + string.Format("?channelid={0}&gameid={1}&os={2}&sdk_version=2.0&seq=&source=0&ts={3}", channelid, PlatformConsts.INTL_GAME_ID, OS, TimeUtil.Timestamp);
            string reqString = "{\"uid\":\"" + ret.OpenId + "\"}";
            string verifyString = api_path + reqString + PlatformConsts.INTL_SIGKEY;
            string sig = Hash.ComputeMd5HashString(verifyString);
            Debug.LogFormat("verifyString{0}", verifyString);
            string url = "https://" + PlatformConsts.INTL_HOST + api_path + "&sig=" + sig;
            PlatformCallback.Instance.StartCoroutine(WebRequest(url, reqString, ret, callback));
        }

        static IEnumerator WebRequest(string url, string data, PlatformLoginRet ret, UnityAction<PlatformLoginRet> callback)
        {
            Debug.LogFormat("[WebRequest]url:{0} data:[{1}]", url, data);
            using (UnityWebRequest www = UnityWebRequest.Put(url, data))
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
                    Debug.Log("WebRequest Ret:" + returnJson);
                    var resultData = JsonMapper.ToObject(returnJson);
                    ret.RetCode = (int)resultData["ret"];
                    ret.RetMsg = resultData["msg"].ToString();
                    if (ret.RetCode == PlatformError.SUCCESS)
                    {
                        ret.OpenId = resultData["openid"].ToString();
                    }
                }
                if (callback != null)
                    callback(ret);
            }
        }

    }
}
#endif