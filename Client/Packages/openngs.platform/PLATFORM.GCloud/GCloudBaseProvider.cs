using OpenNGS.Platform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GCloudBaseProvider : IBaseProvider
{
    public bool IsDebug { get => GCloud.MSDK.MSDK.isDebug; set => GCloud.MSDK.MSDK.isDebug = value; }

    public Platform_MODULE Module =>  Platform_MODULE.BASE;

    public bool IsAppInstalled(string appStr)
    {
#if GCLOUD_MSDK_WINDOWS || GCLOUD_MSDK_MAC
        return false;
#else
        return GCloud.MSDK.MSDKTools.IsAppInstalled(appStr);
#endif
    }
}
