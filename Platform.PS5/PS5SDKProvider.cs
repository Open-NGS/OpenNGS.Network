using OpenNGS.Platform;
using OpenNGS.Platform.PS5;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS5SDKProvider : OpenNGS.Platform.ISDKProvider
{
    public IModuleProvider CreateProvider(PLATFORM_MODULE module)
    {
        switch (module)
        {
            case PLATFORM_MODULE.LOGIN:
                return new PS5Auth();
            case PLATFORM_MODULE.UDS:
                return new PS5UDS();
            case PLATFORM_MODULE.ACHIEVEMENT:
                return new PS5Trophies();
            default:
                return null;
        }
    }

    public void Initialize()
    {
        PS5SDK.Initialize();
    }
}
