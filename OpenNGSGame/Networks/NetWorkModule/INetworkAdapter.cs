using UnityEngine;
using System.Collections.Generic;
using MissQ;
using MissQ.Common;
//using GCloud.MSDK;

public enum NoticeType
{
    NoticeType_ServerClose = 0,    //停服公告
    NoticeType_LogIn = 1,          //登陆前公告
    NoticeType_LogInOver = 2,      //登陆后公告
    NoticeType_Activity = 3,       //活动公告
    NoticeType_Scroll = 4,         //滚动通知公告
};

/// <summary>
/// 虚拟场景和TGPA 定义相同
/// </summary>
public enum StageEnum : int
{
    DefaultStage = 0,
    BootStage = 1,
    UpdateStage = 2,
    LoginStage = 3,
    MainStage = 4,
    LoadingStage = 5,
    GamingStage = 6,
}

public enum UpdateStage
{
    UpdateStage_Init = 0,          //初始状态
    UpdateStage_CheckAPP = 1,      //检查APP更新
    UpdateStage_DownloadAPP = 2,   //正在下载APP更新
    UpdateStage_CheckRES = 3,      //检查资源更新
    UpdateStage_DownloadRES = 4,   //正在下载资源更新
    UpdateStage_Success = 5,       //更新成功
    UpdateStage_Fail = 6,          //更新失败
    UpdateStage_NeedNoUpdate = 7,  //无需更新
    UpdateStage_Confirm = 8,       //等待确认
    UpdateStage_InstallAPP = 9,    //安装APP
    UpdateStage_SourceExtract = 10 //资源解压，在资源更新现在完成之后
};

public abstract class INetworkAdapter
{
    #region 业务层接口定义
    public delegate void INetworkFeedbackEvent(int flag, string desc);
    public INetworkFeedbackEvent feedbackEvent;


    //自动更新回调
    public delegate void IUpdateStageEvent(UpdateStage stage, string errorCode);
    public IUpdateStageEvent updateStageEvent;

    public delegate void IUpdateProgressEvent(float process, UpdateStage stage);
    public IUpdateProgressEvent updateProgressEvent;
    #endregion

    public virtual void Init(System.Int64 gameId, string gameKey) { }

    public virtual void UnInit() { }

    /// <summary>
    /// 获取个人信息回调
    /// </summary>
    public System.Action<SNSInfo> GetPlayerSNSInfoCB;

    ///// <summary>
    ///// 好友分享
    ///// </summary>
    ///// <param name="title"></param>
    ///// <param name="desc"></param>
    ///// <param name="imagePath"></param>
    ///// <param name="iconPath"></param>
    ///// <param name="link"></param>
    ///// <param name="extra"></param>
    //public virtual void SendMessage(System.Action<int> cb, FriendReqType type, string channel, string title, string desc, string imagePath, string iconPath, string link, string extra) { }

    ///// <summary>
    ///// 朋友分享
    ///// </summary>
    ///// <param name="title"></param>
    ///// <param name="desc"></param>
    ///// <param name="imagePath"></param>
    ///// <param name="iconPath"></param>
    ///// <param name="link"></param>
    ///// <param name="extra"></param>
    //public virtual void Share(System.Action<int> cb, FriendReqType type, string channel, string title, string desc, string imagePath, string iconPath, string link, string extra) { }

    /// <summary>
    /// 处理启动信息
    /// </summary>
    public virtual void HandleWakeUpData() { }

    /// <summary>
    /// 获取邀战
    /// </summary>
    /// <param name="uin"></param>
    /// <param name="zone"></param>
    /// <param name="nickName"></param>
    /// <returns></returns>
    public virtual string GetWakeUpData(ulong uin, uint zone, string nickName) { return ""; }

    /// <summary>
    /// 获取聚会信息
    /// </summary>
    /// <param name="roomID"></param>
    /// <returns></returns>
    public virtual string GetWakeUpData(ulong roomID) { return ""; }

    /// <summary>
    /// 获取回放信息
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    //public virtual string GetWakeUpData(SCSFightReportInfo info) { return ""; }

    /// <summary>
    /// 获取个人信息
    /// </summary>
    /// <returns></returns>
    public virtual void GetPlayerSNSInfo(Platform pt, System.Action<SNSInfo> rsp) { }


    /// <summary>
    /// 显示游戏公告
    /// </summary>
    /// <returns></returns>
    public virtual void ShowNotice(NoticeType noticeType, System.Action<NoticeInfo> rsp) { }

    /// <summary>
    /// 判断平台是否安装
    /// </summary>
    /// <param name="platform"></param>
    /// <returns></returns>
    public virtual bool IsPlatformInstalled(Platform platform) { return false; }

    /// <summary>
    /// 打开浏览器
    /// </summary>
    /// <param name="url">网址</param>
    public virtual void OpenURL(string url, System.Action action) { }



    /// <summary>
    /// 上报场景加载
    /// </summary>
    /// <param name="sceneName">场景名字</param>
    public abstract void ReportLoadLevel(string sceneName);
    /// <summary>
    /// 上报场景加载完毕
    /// </summary>
    public abstract void ReportLoadLevelCompleted();


    /// <summary>
    /// 上报Stage修改
    /// </summary>
    /// <param name="stage">TGPA</param>
    public abstract void ReportStageChange(StageEnum stage);
    /// <summary>
    /// 第一次上报信息
    /// </summary>
    public abstract void FirstReportGameInfo();

    /// <summary>
    /// 上报灯塔数据
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="paramsDic"></param>
    /// <param name="spChannels"></param>
    /// <param name="isRealTime"></param>
    public abstract void ReportBeaconData(string eventName, Dictionary<string, string> paramsDic, string spChannels = "", bool isRealTime = true);

    /// <summary>
    /// 域名转IP（HTTPDNS接入）
    /// </summary>
    /// <param name="domain">域名</param>
    /// <returns>IP</returns>
    public abstract string GetAddrByName(string domain);

    #region 自动更新
    public UpdateStage _stage = UpdateStage.UpdateStage_Init;     //更新状态

    public float sizeNow;           //当前进度
    public float sizeTotal;         //总进度

    public UpdateStage Stage
    {
        get
        {
            return _stage;
        }
    }

    public abstract bool IsAppUpdate();
    public abstract float GetDownloadSizeKB();

    //程序版本号 GetDefayltAPPVersion
    public string AppVersion
    {
        get
        {
            return MissQBoot.mInstance.GetMissQVersion();

        }
    }

    public string ResVersion
    {
        get
        {
            string version = PlayerPrefs.GetString(MissQBaseConst.CONSTResVersionKey, "");
            if (string.IsNullOrEmpty(version))
            {
                version = MissQBoot.mInstance.GetMissQResVersion();
            }
            return version;
        }
        set
        {
            string version = PlayerPrefs.GetString(MissQBaseConst.CONSTResVersionKey, "");
            if (!version.Equals(value))
            {
                PlayerPrefs.SetString(MissQBaseConst.CONSTResVersionKey, value);
            }
        }
    }
    public virtual void StartUpdate() { }

    public virtual void ConfirmUpdate() { }

    public virtual void CancelUpdate() { }
    #endregion
}
