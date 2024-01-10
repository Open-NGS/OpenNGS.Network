

/// <summary>
/// 账号平台类型
/// </summary>
public enum Platform
{
    None,
    Guest,
    QQ,
    Wechat,
    Google,
    Facebook,
    GameCenter,
    //AutoLogin,
}

/// <summary>
/// 网络环境
/// </summary>
public enum NetworkEnv
{
    Unknown,
    NotReachable,
    Cable,
    ViaWifi,
    Via2G,
    Via3G,
    Via4G,
}

/// <summary>
/// 网络运营商
/// </summary>
public enum NetworkCarrier
{
    Unknown,
    ChinaMobile,
    ChinaUnicom,
    ChinaTelecom,
    ChinaSpacecom,
}

public enum NetworkWakeUp
{
    Success,// 本地原有票据有效，使用原有票据登录
    AccountRefresh,// 新旧 openid 相同，票据不同。刷新登录票据
    UrlLogin,// 本地无openid，拉起有票据，使用新票据登录
    NeedSelect,//"票据均有效，提示玩家选择账号"
    NeedLogin,//"票据均无效，进入登录页面"
}

public abstract class INetworkConnector
{
    public delegate void NetworkEventHandler(NetworkResult result);

    /// <summary>
    /// 连接完成回调接口
    /// </summary>
    /// <param name="result"></param>
    public NetworkEventHandler onConnect;

    /// <summary>
    /// 重连完成回调接口
    /// </summary>
    /// <param name="result">错误码</param>
    public NetworkEventHandler onReconnect;

    /// <summary>
    /// 连接排队回调接口
    /// </summary>
    /// <param name="position">当前位置</param>
    /// <param name="queueLen">队伍长度</param>
    /// <param name="estimateTime">预估时间秒</param>
    public delegate void NetworkStayInQueueEventHandler(int position, int queueLen, int estimateTime);
    public NetworkStayInQueueEventHandler onStayInQueue;

    /// <summary>
    /// 断开连接回调接口
    /// </summary>
    /// <param name="result">错误码</param>
    public delegate void NetworkDisconnectEventHandler(NetworkResult result);
    public NetworkDisconnectEventHandler onDisconnect;

    /// <summary>
    /// 发生错误回调接口
    /// </summary>
    /// <param name="result">错误码</param>
    public delegate void NetworkErrorEventHandler(NetworkResult result);
    public NetworkErrorEventHandler onError;

    /// <summary>
    /// 接收数据回调接口
    /// </summary>
    /// <param name="buff">接收到的数据缓冲区</param>
    /// <param name="iOffset">有效数据开始位置偏移</param>
    /// <param name="iLength">数据长度</param>
    public delegate void NetworkRecvedDataHandler(byte[] buff, int iOffset, int iLength);
    public NetworkRecvedDataHandler onRecvedData;

    /// <summary>
    /// 网络环境变化回调接口
    /// </summary>
    public delegate void NetworkChangedEventHandler();
    public NetworkChangedEventHandler onNetworkChanged;


    /// <summary>
    /// 外部唤醒回调接口
    /// </summary>
    public delegate void NetworkWakeUpEventHandler(NetworkWakeUp wakeUp);
    public NetworkWakeUpEventHandler onNetworkWakeUp;

    /// <summary>
    /// 初始化网络连接对象
    /// </summary>
    /// <param name="pf">平台</param>
    /// <param name="ip">服务器IP</param>
    /// <param name="port">服务器端口</param>
    /// <returns>初始化网络连接对象是否成功</returns>
    public abstract void Initialize();

    /// <summary>
    /// 销毁网络连接对象
    /// </summary>
    public abstract void Uninitialize();

    /// <summary>
    /// 判断网络连接是否可用
    /// </summary>
    /// <returns>网络连接是否可用</returns>
    public abstract bool IsValid();

    /// <summary>
    /// 判断物理网络是否连通
    /// </summary>
    /// <returns>网络连接是否可用</returns>
    public abstract bool IsNetworkReachable();

    /// <summary>
    /// 网络环境
    /// </summary>
    /// <returns></returns>
    public abstract NetworkEnv GetNetworkEnv();

    /// <summary>
    /// 网络运营商
    /// </summary>
    /// <returns></returns>
    public abstract NetworkCarrier GetNetworkCarrier();

    /// <summary>
    /// 连接服务器
    /// </summary>
    /// <param name="uiTimeout">超时时间</param>
    /// <param name="bLogin">是否登陆鉴权服务器</param>
    /// <returns>连接服务器是否成功</returns>
    public abstract bool Connect(bool bLogin, uint uiTimeout, string ip, string port, string gatePort, Platform platform, string userName = "", string password = "");

    /// <summary>
    /// 重连服务器
    /// </summary>
    /// <param name="uiTimeout">超时时间</param>
    /// <returns>重连服务器是否成功</returns>
    public abstract bool Reconnect(uint uiTimeout);

    /// <summary>
    /// 断开服务器
    /// </summary>
    /// <returns>断开服务器是否成功</returns>
    public abstract bool Disconnect();

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="buff">发送数据buff</param>
    /// <param name="iLen">发送数据长度</param>
    /// <returns>发送数据是否成功</returns>
    public abstract bool SendData(byte[] buff, int iLen);

    /// <summary>
    /// 获取UID
    /// </summary>
    /// <returns></returns>
    public virtual string GetUID() { return string.Empty; }

    /// <summary>
    /// 获取openId
    /// </summary>
    /// <returns></returns>
    public virtual string GetOpenId() { return string.Empty; }

    /// <summary>
    /// 获取保存的平台信息。用于已经登录过得玩家自动登录
    /// </summary>
    /// <param name="platform"></param>
    /// <returns></returns>
    public abstract Platform GetSavedPlatform();

    /// <summary>
    /// 是否有本地登录态
    /// </summary>
    /// <returns></returns>
    public abstract bool HasLoginRet();

    /// <summary>
    /// 是否为外部唤醒
    /// </summary>
    /// <returns></returns>
    public abstract bool IsWakeUp();

    /// <summary>
    /// 获取外部拉起中的信息
    /// </summary>
    /// <returns></returns>
    public abstract string GetWakeUpData();

    /// <summary>
    /// 清除外部拉起中的信息
    /// </summary>
    public abstract void ClearWakeUpData();

    /// <summary>
    /// 获取头像地址
    /// </summary>
    /// <returns></returns>
    public abstract string GetPictureUrl();

    /// <summary>
    /// 设置IP和Port
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    public abstract void SetIpAndPort(string ip, string port);

    /// <summary>
    /// 切换账号
    /// </summary>
    /// <param name="bSwitch"></param>
    public abstract void SwitchUser(bool bSwitch);

    public abstract OpenNGS.ERPC.RPCClient GetRpcClient();
}
