using Networks.NetWorkModule;
using System;
using System.Collections.Generic;
//using GCloud.MSDK;
using UnityEngine;

public struct SNSInfo
{
    public string nickName;
}

public class NoticeInfo
{
    public NoticeInfo()
    {
        noticeRets = new List<NoticeRet>();
    }
    public List<NoticeRet> noticeRets;
}

public class NoticeRet
{
    public NoticeRet()
    {
        textInfo = new NoticeTextInfo();
        picUrlList = new List<NoticePictureInfo>();
    }
    public int noticeId;
    public int beginTime;
    public int endTime;
    public int order;
    public int contentType;
    public NoticeTextInfo textInfo;
    public List<NoticePictureInfo> picUrlList;
    public string webUrl; //网页公告链接
    public string extraJson;
    public string groupType;
    public string tab;
}

public class NoticeTextInfo
{
    public string noticeTitle;
    public string noticeContent;
    public string noticeRedirectUrl;
}

public class NoticePictureInfo
{
    public string noticePicUrl;
    public string noticePicSize;
}

public enum BeaconEventName
{
    Start,
    Login,
}

public class NetworkAdapterModule
{
    #region NetworkAdapterModule
    public static readonly NetworkAdapterModule mInstance = new NetworkAdapterModule();

    // 适配层接口
    private INetworkAdapter networkAdapter;


    public INetworkAdapter NetworkAdapter
    {
        get
        {
            return networkAdapter;
        }

        set
        {
            networkAdapter = value;
        }
    }



    public void Initialize(System.Int64 gameId, string gameKey)
    {
		Uninitialize ();

        //TODO 根据宏定义，初始化不同网络适配器
        if(NetworkAdapter == null)
        {
            NetworkAdapter = new NetworkAdapterOpenNGS();
            NetworkAdapter.Init(gameId, gameKey);
        }
        else
        {
            Debug.LogError("NetworkAdapter is not null!");
        }
    }
    public void Uninitialize()
    {
        if (NetworkAdapter != null)
        {
            NetworkAdapter.UnInit();
            NetworkAdapter = null;
        }
    }
    #endregion

    #region 业务层事件注册
    public void RegisterFeedbackEvent(INetworkAdapter.INetworkFeedbackEvent feedbackEvent)
    {
        NetworkAdapter.feedbackEvent += feedbackEvent;
    }

    public void UnRegisterFeedbackEvent(INetworkAdapter.INetworkFeedbackEvent feedbackEvent)
    {
        NetworkAdapter.feedbackEvent -= feedbackEvent;
    }


    public void RegisterUpdateStageEvent(INetworkAdapter.IUpdateStageEvent updateStageEvent)
    {
        NetworkAdapter.updateStageEvent += updateStageEvent;
    }

    public void UnRegisterUpdateStageEvent(INetworkAdapter.IUpdateStageEvent updateStageEvent)
    {
        NetworkAdapter.updateStageEvent -= updateStageEvent;
    }
    public void RegisterUpdateProgressEvent(INetworkAdapter.IUpdateProgressEvent updateProgressEvent)
    {
        NetworkAdapter.updateProgressEvent += updateProgressEvent;
    }

    public void UnRegisterUpdateProgressEvent(INetworkAdapter.IUpdateProgressEvent updateProgressEvent)
    {
        NetworkAdapter.updateProgressEvent -= updateProgressEvent;
    }
    #endregion

    #region 信息上报

    public void ReportLoadLevel(string sceneName)
    {
        if (NetworkAdapter != null)
        {
            NetworkAdapter.ReportLoadLevel(sceneName);
        }

    }

    public void ReportLoadLevelCompleted()
    {
        if (NetworkAdapter != null)
        {
            NetworkAdapter.ReportLoadLevelCompleted();
        }
    }

    public void ReportBeaconData(BeaconEventName eventName, Dictionary<string, string> paramsDic, string spChannels = "", bool isRealTime = true)
    {
        if (NetworkAdapter != null)
        {
            NetworkAdapter.ReportBeaconData(eventName.ToString(), paramsDic, spChannels, isRealTime);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="stage"></param>
    public void ReportStageChange(StageEnum stage)
    {
        if (NetworkAdapter != null)
        {
            NetworkAdapter.ReportStageChange(stage);
        }
    }
    public void FirstReportGameInfo()
    {
        if (NetworkAdapter != null)
        {
            NetworkAdapter.FirstReportGameInfo();
        }
    }


    /// <summary>
    /// 域名转IP
    /// </summary>
    /// <param name="domain">域名</param>
    /// <returns>IP</returns>
    public string GetAddrByName(string domain)
    {
        if (NetworkAdapter != null)
        {
            return NetworkAdapter.GetAddrByName(domain);
        }
        return domain;
    }

    #endregion

    #region 分享适配层
    //public void SendMessage(Action<int> cb, FriendReqType type, string channel, string title, string desc, string imagePath, string iconPath, string link, string extra)
    //{
    //    if (networkAdapter != null)
    //    {
    //        networkAdapter.SendMessage(cb, type, channel, title, desc, imagePath, iconPath, link, extra);
    //    }
    //}

    //public void Share(Action<int> cb, FriendReqType type, string channel, string title, string desc, string imagePath, string iconPath, string link, string extra)
    //{
    //    if (networkAdapter != null)
    //    {
    //        networkAdapter.Share(cb, type, channel, title, desc, imagePath, iconPath, link, extra);
    //    }
    //}

    public void HandleWakeUpData()
    {
        if (networkAdapter != null)
        {
            networkAdapter.HandleWakeUpData();
        }
    }

    public string GetWakeUpData(ulong uin, uint zone, string nickName)
    {
        if (networkAdapter != null)
        {
            return networkAdapter.GetWakeUpData(uin, zone, nickName);
        }
        return "";
    }

    public string GetWakeUpData(ulong roomID)
    {
        if (networkAdapter != null)
        {
            return networkAdapter.GetWakeUpData(roomID);
        }
        return "";
    }

    //public string GetWakeUpData(SCSFightReportInfo info)
    //{
    //    if (networkAdapter != null)
    //    {
    //        return networkAdapter.GetWakeUpData(info);
    //    }
    //    return "";
    //}

    #endregion

    #region SNS适配层

    public Action<SNSInfo> GetPlayerSNSInfoCB
    {
        get { return NetworkAdapter.GetPlayerSNSInfoCB; }
    }

    public void GetPlayerSNSInfo(Platform platform, Action<SNSInfo> rsp)
    {
        NetworkAdapter.GetPlayerSNSInfo(platform, rsp);
    }
#endregion

    #region 通告适配层
    public void ShowNotice(NoticeType noticeType, Action<NoticeInfo> rsp)
    {
        NetworkAdapter.ShowNotice(noticeType, rsp);
    }
    #endregion

    #region 判断登录平台
    public bool IsPlatformInstalled(Platform platform)
    {
        return NetworkAdapter.IsPlatformInstalled(platform);
    }
    #endregion

    #region 打开浏览器

    /// <summary>
    /// 打开浏览器
    /// </summary>
    /// <param name="url">网址</param>
    public virtual void OpenURL(string url, Action action = null)
    {
        NetworkAdapter.OpenURL(url, action);
    }
    #endregion

    #region 自动更新

    public float SizeNow       //当前进度
    {
        get { return NetworkAdapter.sizeNow; }
    }
    public float SizeTotal  //总进度
    {
        get { return NetworkAdapter.sizeTotal; }
    }
    public string AppVersion     //程序版本号
    {
        get
        {
            return NetworkAdapter.AppVersion;
        }
    }

    public string ResVersion
    {
        get
        {
            return NetworkAdapter.ResVersion;
        }
    }

    public void StartUpdate()
    {
        if (NetworkAdapter != null)
        {
            NetworkAdapter.StartUpdate();
        }
    }

    public void ConfirmUpdate()
    {
        NetworkAdapter.ConfirmUpdate();
    }

    public void CancelUpdate()
    {
        NetworkAdapter.CancelUpdate();
    }



    #endregion
}
