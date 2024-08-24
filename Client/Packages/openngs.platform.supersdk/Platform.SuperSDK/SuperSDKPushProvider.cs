#if SUPERSDK

using OpenNGS.Platform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperSDKPushProvider : IPushProvider
{
    public Platform_MODULE Module => Platform_MODULE.PUSH;

    public void RegisterPush()
    {
        Dictionary<string, object> paramsDic = new Dictionary<string, object>() { };
        paramsDic.Add("lang", LocalizationManager.CurrentLanguageCode.ToLower()); //海外必传，语言，可传入：zh-cn（简体中文）/zh-tw（繁体中文）/en（英文）/de（德语）/ru（俄语）/fr（法语）/es（西班牙语）/pt（葡萄牙语）/tr（土耳其语）/pl（波兰语）/ko（韩语）/ja（日语）
        //也可通过方法获取常量：BCoreParams.Language.EN  英文
        paramsDic.Add("showType","0");//显示类型，0 表示应用在前台也需要弹出通知 
//1 表示应用在前台不需要弹出通知
        SuperSDK.getInstance().Invoke("push", "registerPush", paramsDic);
    }

    public void SetAccount(string accountId)
    {
        Dictionary<string, object> paramsDic = new Dictionary<string, object>() { };
        paramsDic.Add("osdk_user_id", accountId); //必传，用户ID，必须是带前缀的用户ID，如000016_123456
        SuperSDK.getInstance().Invoke("push", "bindUser", paramsDic);
    }

    void DeleteAccount(string accountId)
    {
        //设置信鸽关联帐号
        GCloud.MSDK.MSDKPush.DeleteAccount("XG", accountId);
        Debug.Log("MSDKPush.DeleteAccount XG  : " + accountId);
    }
}
#endif
