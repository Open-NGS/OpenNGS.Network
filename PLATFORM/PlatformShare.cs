using System.Collections;
using System.Collections.Generic;
using System.IO;
#if SUPERSDK
using I2.Loc;
using SuperSDKV4;
#endif
using UnityEngine;

namespace OpenNGS.Platform
{
    public enum SharePlatform
    {
        Default = 0,
        Facebook = 7,
        Instagram = 8,
        /// <summary>
        /// 暂时不需要使用
        /// </summary>
        KakaoTalk = 9,
        Twitter = 12,
    }
    
    public class PlatformShare
    {
        public static void Share(string title, string content, SharePlatform platform, string imageUrl,string templateid = null)
        {
            Dictionary<string, object> paramsDic = new Dictionary<string, object>() { };
            if (platform == SharePlatform.KakaoTalk)
            {
                paramsDic.Add("templateid",templateid);
                paramsDic.Add("templateargs","");
            }
            else
            {
                if( title != null )
                    paramsDic.Add("title", title);//图文分享可传，只分享图片
                if( content != null )
                    paramsDic.Add("content",content);//图文分享可传，只分享图片可不传
                if( imageUrl != null )
                    paramsDic.Add("images",imageUrl);//分享图片的网络地址或本地路径，参数必须为string类型 （如果只分享图片，可以只传此参数）
            }
            //paramsDic.Add("url","");//图文分享可传，只分享图片可不传
            if( platform != SharePlatform.Default )
                paramsDic.Add("platform",(int)platform + "");//该字段选传，不传会拉起ShareSDK默认分享UI，传了则不会拉起UI，直接调用对应平台分享接口,参数为string类型：
#if SUPERSDK
            SuperSDK.getInstance().Invoke("mobsharesdk", "mobShare", paramsDic);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="platform"></param>
        public static void ShareByCapture(SharePlatform platform)
        {
            //CoroutineManager.Start(ShareScreenshot(platform, "screenshot.png"));
        }

        static IEnumerator ShareScreenshot(SharePlatform platform,string imageFileName)
        {
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot(imageFileName);
            string imageFullPath = Path.Combine(Application.persistentDataPath, imageFileName);
            yield return new WaitForSeconds(0.5f);
            Share(null,null,platform,imageFullPath);

        }

    }
}
