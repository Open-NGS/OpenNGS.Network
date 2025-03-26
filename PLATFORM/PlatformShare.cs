using System.Collections;
using System.Collections.Generic;
using System.IO;
#if SUPERSDK
using I2.Loc;
using SuperSDKV4;
#endif
using UnityEngine;
using static OpenNGS.Platform.Platform;

namespace OpenNGS.Platform
{
    public class PlatformShare
    {
        public static event OnPlatformRetEventHandler<PlatformShareRet> ShareRetEvent;

        public static void Initialize()
        {
            if (!Platform.IsSupported(PLATFORM_MODULE.SHARE))
                return;
            IShareProvider _shareProvider = Platform.GetShare();
            if (_shareProvider != null)
            {
                _shareProvider.Initialize();
            }
        }

        /// <summary>
        /// 拉起分享列表
        /// </summary>
        /// <param name="platformShareInfo"></param>
        public static void ShowSharePlatformList(PlatformShareInfo platformShareInfo)
        {
            if (!Platform.IsSupported(PLATFORM_MODULE.SHARE))
                return;
            IShareProvider _shareProvider = Platform.GetShare();
            if (_shareProvider != null)
            {
                _shareProvider.ShowSharePlatformList(platformShareInfo);
            }
        }

        /// <summary>
        /// 向指定好友发送信息，当前为空实现
        /// </summary>
        /// <param name="platformShareInfo"></param>
        /// <param name="channel"></param>
        public static void SendMessage(PlatformShareInfo platformShareInfo, string channel = "")
        {
            if (!Platform.IsSupported(PLATFORM_MODULE.SHARE))
                return;
            IShareProvider _shareProvider = Platform.GetShare();
            if (_shareProvider != null)
            {
                _shareProvider.SendMessage(platformShareInfo, channel);
            }
        }

        /// <summary>
        /// 在朋友圈、游戏圈、QQ空间等分享消息，当前为空实现
        /// </summary>
        /// <param name="platformShareInfo"></param>
        /// <param name="channel"></param>
        public static void Share(PlatformShareInfo platformShareInfo, string channel = "")
        {
            if (!Platform.IsSupported(PLATFORM_MODULE.SHARE))
                return;
            IShareProvider _shareProvider = Platform.GetShare();
            if (_shareProvider != null)
            {
                _shareProvider.Share(platformShareInfo, channel);
            }
        }

        //        public static void Share(string title, string content, SharePlatform platform, string imageUrl, string templateid = null)
        //        {
        //            Dictionary<string, object> paramsDic = new Dictionary<string, object>() { };
        //            if (platform == SharePlatform.KakaoTalk)
        //            {
        //                paramsDic.Add("templateid", templateid);
        //                paramsDic.Add("templateargs", "");
        //            }
        //            else
        //            {
        //                if (title != null)
        //                    paramsDic.Add("title", title);//图文分享可传，只分享图片
        //                if (content != null)
        //                    paramsDic.Add("content", content);//图文分享可传，只分享图片可不传
        //                if (imageUrl != null)
        //                    paramsDic.Add("images", imageUrl);//分享图片的网络地址或本地路径，参数必须为string类型 （如果只分享图片，可以只传此参数）
        //            }
        //            //paramsDic.Add("url","");//图文分享可传，只分享图片可不传
        //            if (platform != SharePlatform.Default)
        //                paramsDic.Add("platform", (int)platform + "");//该字段选传，不传会拉起ShareSDK默认分享UI，传了则不会拉起UI，直接调用对应平台分享接口,参数为string类型：
        //#if SUPERSDK
        //            SuperSDK.getInstance().Invoke("mobsharesdk", "mobShare", paramsDic);
        //#endif
        //        }

        //        /// <summary>
        //        /// 
        //        /// </summary>
        //        /// <param name="platform"></param>
        //        public static void ShareByCapture(SharePlatform platform)
        //        {
        //            //CoroutineManager.Start(ShareScreenshot(platform, "screenshot.png"));
        //        }

        //        static IEnumerator ShareScreenshot(SharePlatform platform, string imageFileName)
        //        {
        //            yield return new WaitForEndOfFrame();
        //            ScreenCapture.CaptureScreenshot(imageFileName);
        //            string imageFullPath = Path.Combine(Application.persistentDataPath, imageFileName);
        //            yield return new WaitForSeconds(0.5f);
        //            Share(null, null, platform, imageFullPath);

        //        }
        internal static void OnShareRet(PlatformShareRet ret)
        {
            Debug.Log("[Platform]PlatformShareRet:" + ret.ToJsonString());
            if (ShareRetEvent != null)
                ShareRetEvent(ret);
        }

    }
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

    public enum ShareType
    {
        Share_TEXT = 0,
        Share_LINK = 1
    }

    public class PlatformShareInfo
    {
        /// <summary>
        /// 分享的类型，MobSDK可选填，Native未使用
        /// </summary>
        public ShareType shareType;

        /// <summary>
        /// 指定分享的平台，拉取全平台可默认
        /// </summary>
        public SharePlatform sharePlatform;

        /// <summary>
        /// 通用标题
        /// </summary>
        public string title;

        /// <summary>
        /// MobSDK可使用，标题的链接
        /// </summary>
        public string titleUrl;

        /// <summary>
        /// 通用文本
        /// </summary>
        public string text;

        /// <summary>
        /// NativeSDK使用的链接
        /// </summary>
        public string url;

        /// <summary>
        /// MobSDK使用，图片URL
        /// </summary>
        public string imageUrl;

        /// <summary>
        /// MobSDK和Native通用，本地图片地址
        /// </summary>
        public string filePath;

        /// <summary>
        /// Native使用，直接传递纹理，由SDK存储到Application.temporaryCachePath下并发送
        /// 需要搭配 createdFileName 字段使用！
        /// </summary>
        public Texture2D texture;

        /// <summary>
        /// Native使用，设置传递的纹理名称
        /// 需要搭配 texture 字段使用！
        /// </summary>
        public string createdFileName;
    }

    public class PlatformShareRet : PlatformBaseRet
    {
        private uint shareResultType;
        private string shareTargt;

        [JsonProp("shareResultType")]
        public uint ShareResultType
        {
            get { return shareResultType; }
            set { shareResultType = value; }
        }

        [JsonProp("shareTargt")]
        public string ShareTargt
        {
            get { return shareTargt; }
            set { shareTargt = value; }
        }
        public void Init()
        {
            shareResultType = 0;
            shareTargt = "";
        }

        public PlatformShareRet()
        {
        }

        public PlatformShareRet(string param) : base(param)
        {
        }

        public PlatformShareRet(object json) : base(json)
        {
        }
    }
}
