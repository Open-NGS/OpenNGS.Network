using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !SUPERSDK
//using GCloud.MSDK;
#endif

namespace OpenNGS.Platform
{
    public enum MSDKWebViewOrientation
    {
        Auto = 1,
        Portrait = 2,
        Landscape = 3,
    }

    public class PlatformWebView
    {
        public static void OpenUrl(string url, MSDKWebViewOrientation screenType = MSDKWebViewOrientation.Auto, bool isFullScreen = false,
                                    bool isUseURLEcode = true, string extraJson = "", bool isBrowser = false)
        {
#if !SUPERSDK
            //GCloud.MSDK.MSDKWebView.OpenUrl(url, (GCloud.MSDK.MSDKWebViewOrientation)screenType,isFullScreen,isUseURLEcode,extraJson,isBrowser);
#else
            UnityEngine.Application.OpenURL(url);
            UnityEngine.Debug.Log("Super sdk webview 还没接入完成，以默认浏览器打开。");
#endif
        }
    }
}
