
/// <summary>
/// 
/// </summary>
public enum PlatformReportEvent
{
    /// <summary>
    /// 占位 标识从这个后面都是在UNITY代码中调用数据上报
    /// </summary>
    StartUnityEvent,

    /// <summary>
    /// 游戏启动
    /// </summary>
    Start,

    /// <summary>
    /// SDK初始化成功
    /// </summary>
    SDKInitSuccess,

    /// <summary>
    /// SDK初始化失败
    /// </summary>
    SDKInitFailue,
    
    //DolphinMgr
    /// <summary>
    /// 开始更新APK
    /// </summary>
    StartUpdateApk,
    /// <summary>
    /// 没有可用APK更新
    /// </summary>
    WithoutUpdateApk,
    /// <summary>
    /// 开始更新APK-下载APK资源
    /// </summary>
    StartDownloadUpdateApk,
    /// <summary>
    /// 完成更新APK-下载APK资源
    /// </summary>
    CompleteDownloadUpdateApk,
    /// <summary>
    /// 通知可安装APK
    /// </summary>
    NoticeInstanceApk,
    /// <summary>
    /// 开始更新资源
    /// </summary>
    StartUpdateSource,
    /// <summary>
    /// 没有可用资源更新
    /// </summary>
    WithoutUpdateSource,
    /// <summary>
    /// 开始首包解压
    /// </summary>
    FirstExtractStart,
    /// <summary>
    /// 首包解压完成
    /// </summary>
    FirstExtractComplete,
    /// <summary>
    /// 开始下载更新资源
    /// </summary>
    StartDownloadUpdateSource,
    /// <summary>
    /// 完成下载更新资源
    /// </summary>
    CompleteDownloadUpdateSource,
    
    /// <summary>
    /// 进入登录场景，打开登录UI
    /// </summary>
    EnterLoginScreen,

    /// <summary>
    /// SDK登录成功
    /// </summary>
    SDKLoginSuccess,

    /// <summary>
    /// SDK登录失败
    /// </summary>
    SDKLoginFailue,
    
    /// <summary>
    /// 使用任意渠道登录游戏 --- 在LUA代码中调用
    /// </summary>
    LoginGame,
    
    /// <summary>
    /// 获取服务器列表成功  --- 在LUA代码中调用
    /// </summary>
    QueryServerListSuccess,
    
    /// <summary>
    /// 获取服务器列表失败   --- 在LUA代码中调用
    /// </summary>
    QueryServerListFailue,
    
    /// <summary>
    /// 选服后点击登录进入游戏 
    /// </summary>
    EnterGame,
    
    /// <summary>
    /// 完成一个引导 extra_1标识引导名字 对应asset文件名字
    /// </summary>
    EndOneTutorial,
    /// <summary>
    /// 完成新手引导
    /// </summary>
    EndInitialTutorial,
    /// <summary>
    /// 创建角色
    /// </summary>
    CreateRole,
}
