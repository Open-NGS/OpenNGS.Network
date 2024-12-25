//测试
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace OpenNGS.Platform.EEGames
{
    public class PlatformConsts
    {
        public const string INTL_HOST = "test.intlgame.com";    //TEST
        //public const string INTL_HOST = "sg.intlgame.com";  //Release(SG)
        //public const string INTL_HOST = "jp.intlgame.com";  //Release(JP)
        //public const string INTL_HOST = "us.intlgame.com";  //Release(US)

        public const string INTL_GAME_ID = "29023";   // INTL SDK 的GameID
        public const string INTL_SIGKEY = "f0ba9cb2bbb049d2d45818d6408fc93c";

    }
    public class EEGamesLogin
    {
        private const string api_openid_path = "/v2/profile/openid";
        private const string api_login_path = "/api/login";
        private const string url_login = "http://192.168.10.101:7228";
        private const string tokein_sample = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJBcHBJZCI6IjliZDM4MzVjLWEwMzUtNGYxMy05NDRkLTg0Y2Q5OTIyNjdkNCIsIlVzZXJJZCI6IjMiLCJuYmYiOjE3MjkyMzA2OTcsImV4cCI6MTcyOTIzMjQ5NywiaXNzIjoiRWVnYW1lcyIsImF1ZCI6IkVlZ2FtZXMifQ.wjExy6bcrZzs9_z_c7siIfvHHICBVZwUm1iFd6cW1Bo";


        public static void GetOpenId(PlatformLoginRet ret, UnityAction<PlatformLoginRet> callback)
        {
            Debug.LogFormat("GetOpenId:uid:{0}", ret.OpenId);
            //string api_path = api_openid_path + string.Format("?channelid={0}&gameid={1}&os={2}&sdk_version=2.0&seq=&source=0&ts={3}", channelid, PlatformConsts.INTL_GAME_ID, OS, TimeUtil.Timestamp);
            //string reqString = "{\"uid\":\"" + ret.OpenId + "\"}";
            //string verifyString = api_path + reqString + PlatformConsts.INTL_SIGKEY;
            //string sig = Hash.ComputeMd5HashString(verifyString);
            //Debug.LogFormat("verifyString{0}", verifyString);
            //string url = "https://" + PlatformConsts.INTL_HOST + api_path + "&sig=" + sig;
            //PlatformCallback.Instance.StartCoroutine(WebRequest(url, reqString, ret, callback));
        }

        // 1 打开网页
        // 2 输入登录信息
        // 3 client_id  应用id，后台建立
        // 4 redirect_uri 
        // 5 自定义协议，注册表，进行引用拉起 回传code code用来向平台api 获取授权 token

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
    }

}