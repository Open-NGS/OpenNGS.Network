using OpenNGS.Platform;
using OpenNGS.Platform.PS5;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_PS5
public class PS5SDKProvider : OpenNGS.Platform.ISDKProvider
{

    public PS5SDKProvider(bool initialUserAlwaysLoggedIn)
    {
        PS5SDK.InitialUserAlwaysLoggedIn = initialUserAlwaysLoggedIn;
    }
    public IModuleProvider CreateProvider(PLATFORM_MODULE module)
    {
        switch (module)
        {
            case PLATFORM_MODULE.APP:
                return new PS5App();
            case PLATFORM_MODULE.LOGIN:
                return new PS5Auth();
            case PLATFORM_MODULE.USER:
                return new PS5User();
            case PLATFORM_MODULE.UDS:
                return new PS5UDS();
            case PLATFORM_MODULE.ACHIEVEMENT:
                return new PS5Trophies();
            case PLATFORM_MODULE.ACTIVITY:
                return new PS5Activity();
            default:
                return null;
        }
    }

    public bool Initialize()
    {
        PS5SDK.Initialize();
        return true;
    }


    public void Update()
    {
        PS5SDK.Update();
    }
    public void Terminate()
    {
        PS5SDK.Terminate();
    }

}
#endif