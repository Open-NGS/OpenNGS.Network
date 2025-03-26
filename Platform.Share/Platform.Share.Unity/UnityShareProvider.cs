using OpenNGS.Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NativeShare;

namespace OpenNGS.Share.Unity
{
#if OpenNgsShare
    public class UnityShareProvider : IShareProvider
    {
        public PLATFORM_MODULE Module => PLATFORM_MODULE.SHARE;

        private PlatformShareRet ret;

        private NativeShare nativeShare;

        public void Initialize()
        {
            ret = new PlatformShareRet();
            ret.Init();

            nativeShare = new NativeShare();
        }

        public void ShowSharePlatformList(PlatformShareInfo platformShareInfo)
        {
            if (nativeShare == null)
            {
                nativeShare = new NativeShare();
            }

            nativeShare.Clear();
            if (!String.IsNullOrEmpty(platformShareInfo.title))
            {
                nativeShare.SetTitle(platformShareInfo.title);
            }

            if (!String.IsNullOrEmpty(platformShareInfo.text))
            {
                nativeShare.SetText(platformShareInfo.text);
            }

            if (!String.IsNullOrEmpty(platformShareInfo.url))
            {
                nativeShare.SetUrl(platformShareInfo.url);
            }

            if (!String.IsNullOrEmpty(platformShareInfo.filePath))
            {
                nativeShare.AddFile(platformShareInfo.filePath);
            }

            if (platformShareInfo.texture != null && !String.IsNullOrEmpty(platformShareInfo.createdFileName))
            {
                nativeShare.AddFile(platformShareInfo.texture, platformShareInfo.createdFileName);
            }

            nativeShare.SetCallback(_nativeShareResultCallBack);

            nativeShare.Share();
        }

        private void _nativeShareResultCallBack(ShareResult result, string shareTarget)
        {
            ret.ShareResultType = (uint)result;
            ret.ShareTargt = shareTarget;
            _callBackShare(ret);
        }

        /// <summary>
        /// MSDK 功能接口
        /// 发送消息功能，消息类型包括好友邀请、文字、链接、图片、音乐、视频、小程序、视频号、状态等。
        /// 微信渠道下部分类型可以给指定好友发送。
        /// </summary>
        /// <param name="platformShareInfo"></param>
        /// <param name="channel"></param>
        public void SendMessage(PlatformShareInfo platformShareInfo, string channel = "")
        {

        }

        /// <summary>
        /// MSDK 功能接口
        /// 分享功能（微信朋友圈、微信游戏圈、QQ 空间、Facebook TimeLine、Twitter），
        /// 分享类型包括文字、图片、链接、邀请、音乐、视频。
        /// </summary>
        /// <param name="platformShareInfo"></param>
        /// <param name="channel"></param>
        public void Share(PlatformShareInfo platformShareInfo, string channel = "")
        {

        }

        private void _callBackShare(PlatformShareRet _ret)
        {
            PlatformCallback.Instance.OnShareCallBack(_ret);
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
#else
    public class UnityShareProvider : IShareProvider
    {
        PLATFORM_MODULE IModuleProvider.Module => throw new NotImplementedException();
        private PlatformShareRet ret;

        void IShareProvider.Initialize()
        {
            ret.ShareResultType = (uint)ShareResult.Unknown;
            ret.RetCode = 0;
            ret.RetMsg = "未启用 OpenNgsShare 宏定义！";
            _callBackShare(ret);
        }

        void IShareProvider.SendMessage(PlatformShareInfo platformShareInfo, string channel)
        {

        }

        void IShareProvider.Share(PlatformShareInfo platformShareInfo, string channel)
        {

        }

        void IShareProvider.ShowSharePlatformList(PlatformShareInfo platformShareInfo)
        {

        }

        private void _callBackShare(PlatformShareRet _ret)
        {
            PlatformCallback.Instance.OnShareCallBack(_ret);
        }

        void IModuleProvider.Start()
        {

        }

        void IModuleProvider.Stop()
        {

        }

        void IModuleProvider.Update()
        {

        }
    }
#endif
}
