//using GCloud.MSDK;
using OpenNGS.Platform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GCloudPushProvider : IPushProvider
{
    public Platform_MODULE Module => Platform_MODULE.PUSH;

    public void RegisterPush()
    {
#if GCLOUD_MSDK_WINDOWS || GCLOUD_MSDK_MAC
#else
        //GCloud.MSDK.MSDKPush.RegisterPush("XG");
#endif
        Debug.Log("GCloud.MSDK.MSDKPush.RegisterPush();");
    }

    public void SetAccount(string accountId)
    {
#if GCLOUD_MSDK_WINDOWS || GCLOUD_MSDK_MAC
#else
        //设置信鸽关联帐号
        GCloud.MSDK.MSDKPush.SetAccount("XG", accountId);
#endif
        Debug.Log("MSDKPush.SetAccount XG  : " + accountId);
    }

    public void DeleteAccount(string accountId)
    {
        #if GCLOUD_MSDK_WINDOWS || GCLOUD_MSDK_MAC
#else
        //设置信鸽关联帐号
        GCloud.MSDK.MSDKPush.DeleteAccount("XG", accountId);
#endif
        Debug.Log("MSDKPush.DeleteAccount XG  : " + accountId);
    }
}
