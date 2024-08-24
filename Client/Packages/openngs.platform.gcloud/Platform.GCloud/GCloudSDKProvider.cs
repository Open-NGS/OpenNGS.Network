using OpenNGS.Platform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GCloudSDKProvider : ISDKProvider
{
    public IModuleProvider CreateProvider(Platform_MODULE module)
    {
        switch(module)
        {
            case Platform_MODULE.BASE: return new GCloudBaseProvider();
            case Platform_MODULE.LOGIN: return new GCloudLoginProvider();
            case Platform_MODULE.PAY:return GetPayService();
            case Platform_MODULE.DIR: return new GCloudDirProvider();
            case Platform_MODULE.PUSH: break;
            case Platform_MODULE.REPORT: break;
            case Platform_MODULE.SHARE: break;
        }
        return null;
    }


    public OpenNGS.Platform.Pay.IPayServiceProvider GetPayService()
    {
        //PAY_CHANNEL channel = PAY_CHANNEL.Midas;
#if SUPERSDK
            return new Pay.MidasYouzu.MidasOverseaYouzuProvider();


                case PAY_CHANNEL.MidasOverseaYouzu:
                    this.provider = new Pay.MidasYouzu.MidasOverseaYouzuProvider();
                    break;
                case PAY_CHANNEL.SuperSDK:
                    this.provider = new SuperSDKPayProvider();
                    break;
#else
        return null;//  new Pay.Midas.MidasOverseaProvider();
#endif
    }

}
