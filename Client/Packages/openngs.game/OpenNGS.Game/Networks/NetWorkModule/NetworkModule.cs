using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using OpenNGS;
using OpenNGSCommon;
using protocol;
using Networks.NetWorkModule;
//using GameGuide;

public sealed class NetworkModule
{
    public static readonly NetworkModule Instance = new NetworkModule();

    private NetworkModule() { }

    /// <summary>
    /// 网络模块状态
    /// </summary>
    private enum NetworkFsmState
    {
        Init,                       // 初始化/中止状态
        Connecting,                 // 连接状态
        Authing,                    // 鉴权状态
        Working,                    // 工作状态
        Reconnecting,               // 重连状态
        //ReAuthing,                  // 重新鉴权状态
        Disconnect,                 // 断开连接状态
        //Waiting,                    // 等待状态
    }

    /// <summary>
    /// 网络模块事件
    /// </summary>
    private enum NetworkFsmEvent
    {
        Connect,                    // 连接服务器无登录态
        ConnectWithAuth,            // 连接服务器有登录态
        ConnectSuccess,             // 连接成功
        ConnectFailure,             // 连接失败
        ReconnectSuccess,           // 重连成功
        ReconnectFailure,           // 重连失败
        Disconnect,                 // 连接断开
        Error,                      // 连接异常
        CancelWaitingQueue,         // 取消等待排队

        Authorized,                 // 鉴权成功
        AuthFailure,                // 鉴权失败
        SendRequestFailure,         // 发送请求失败或超时
        HeartbeatTimeout,           // 心跳超时
        Kick,                       // 服务器踢号
        LocalKick,                  // 本地踢号

        UserReconnect,              // 玩家主动重连
        UserDisconnect,             // 玩家主动断连
    }

    /// <summary>
    /// 通知上层当前网络状态
    /// </summary>
    public enum NetworkStatus
    {
        SilentConnecting,           // 静默连接中
        Connecting,                 // 连接中
        Reconnecting,               // 重连中
        ConnectFinished,            // 连接完成
        StayInQueue,                // 连接排队中

        SilentRequesting,           // 静默请求中
        Requesting,                 // 请求中
        RequestFinished,            // 请求完成

        Disconnected,               // 未连接
    }

    /// <summary>
    /// 网络状态通知接口
    /// </summary>
    /// <param name="status"></param>
    public delegate void NetworkStatusHandler(NetworkStatus status);
    public NetworkStatusHandler onNetworkStatus;

    /// <summary>
    /// 授权状态通知接口
    /// </summary>
    /// <param name="status"></param>
    public delegate void NetworkAuthHandler(NetworkResult connectResult);
    public NetworkAuthHandler onNetworkAuth;

    /// <summary>
    /// 错误处理接口
    /// </summary>
    /// <param name="errmsg">错误信息</param>
    public delegate void ErrorHandler(NetworkResult connectResult);
    public ErrorHandler onError;

    /// <summary>
    /// 登录请求结果接口
    /// </summary>
    /// <param name="success"></param>
    /// <param name="connectResult"></param>
    /// <param name="loginErrno"></param>
    /// <param name="loginResult"></param>
    public delegate void LoginHandler(bool success, NetworkResult connectResult, ResultCode loginErrno, LoginRsp loginResult);
    public LoginHandler onLogin;
    public LoginHandler onReLogin;

    /// <summary>
    /// 登录排队回调接口
    /// </summary>
    /// <param name="position">当前位置</param>
    /// <param name="queueLen">队伍长度</param>
    /// <param name="estimateTime">预估时间秒</param>
    public delegate void LoginStayInQueueHandler(int position, int queueLen, int estimateTime);
    public LoginStayInQueueHandler onStayInQueueEvent;

    /// <summary>
    /// 踢号通知接口
    /// </summary>
    /// <param name="reason"></param>
    public delegate void KickHandler(KickReason reason);
    public KickHandler onKick;

    /// <summary>
    /// 网络环境变化接口
    /// </summary>
    public delegate void NetworkChangedHandler();
    public NetworkChangedHandler onNetworkChanged;

    /// <summary>
    /// 重连超过一定次数未成功
    /// </summary>
    public delegate void ReconnectFailedHandler(NetworkResult connectResult);
    public ReconnectFailedHandler onReconnectFailed;

    /// <summary>
    /// 外部唤醒回调
    /// </summary>
    public delegate void NetworkWakeUpHandler(NetworkWakeUp wakeUp);
    public NetworkWakeUpHandler onNetworkWakeUp;

    public delegate void MessageHeadStatusHandler(byte[]status);
    public MessageHeadStatusHandler onMessageHeadStatus;

    // 连接超时时间
    private const int connectTimeout = 15;

    // 重新尝试登录时间间隔
    private const int retryLoginTime = 1;

    // 自动网络连接状态检测时间间隔
    private const int detectConnectionTime = 3;

    // Gamesvr网络连接接口
    private INetworkConnector connector;


    // 网络连接状态机
    private OpenNGS.FSM fsm = new OpenNGS.FSM();

    // 服务器IP
    private string targetIp;

    // 服务器realm端口
    private string targetRealmPort;

    // 服务器gate端口
    private string targetGatePort;
    // 登陆账号平台
    private Platform platform = Platform.None;

    // 用户名
    private string username;

    // 密码
    private string password;

    // FirebaseToken
    private string firebaseToken;

    // 登陆模式
    private LoginReason loginReason;

    // 登录排队
    private bool stayInQueue;

    // 是否处理连接会话中
    private bool isInSession = false;

    // 缓存登陆结果信息
    private NetworkResult connectResult = NetworkResult.FAIL;
    private ResultCode loginErrno = default(ResultCode);
    private LoginRsp loginRsp = default(LoginRsp);

    private NetworkModulePackageProcessor packageProcessor;
    /// <summary>
    /// 初始化网络模块
    /// </summary>
    public void Initialize(INetworkConnector connector, INetworkModuleExtension networkModuleExtension)
    {
        Uninitialize();

        if (connector == null)
        {
            Debug.LogError("Network connector is null");
            return;
        }

        Debug.Log("NetworkModule Initialize");

        this.connector = connector;
        this.connector.onConnect = ConnectEventHandler;
        this.connector.onReconnect = ReconnectEventHandler;
        this.connector.onStayInQueue = StayInQueueEventhandler;
        this.connector.onDisconnect = DisconnectEventHandler;
        this.connector.onError = ErrorEventHandler;
        this.connector.onNetworkChanged = NotifyNetworkChanged;
        this.connector.onNetworkWakeUp = NotifyNetworkWakeUp;
        this.connector.Initialize();

        fsm = new FSM();
        BindFSM();
        fsm.Init(Scheduler.GetInstance());
        fsm.Start((int)NetworkFsmState.Init);

        packageProcessor = new NetworkModulePackageProcessor();
        packageProcessor.Initialize(this.connector, networkModuleExtension);
        packageProcessor.onSendRequestFail = () => fsm.OnEvent((int)NetworkFsmEvent.SendRequestFailure);
        packageProcessor.onHeartBeatTimeOut = () => fsm.OnEvent((int)NetworkFsmEvent.HeartbeatTimeout);
        packageProcessor.onNetworkStatus = (status) => NotifyNetworkStatus(status);
        packageProcessor.onMessageHeadStatus = onMessageHeadStatus;

        ResetReconnectCounter();

        Scheduler.GetInstance().AddUpdate(Update);

        NetworkErrMsg.Init();
    }

    /// <summary>
    /// 销毁网络模块
    /// </summary>
    public void Uninitialize()
    {
        if (connector != null)
        {
            connector.onConnect = null;
            connector.onReconnect = null;
            connector.onStayInQueue = null;
            connector.onDisconnect = null;
            connector.onError = null;
            connector.onRecvedData = null;
            connector.onError = null;
            connector.onNetworkChanged = null;
            connector.Uninitialize();
            connector = null;
        }

        Scheduler.GetInstance().RemoveUpdate(Update);
        
        fsm.Destroy();

        packageProcessor?.Uninitialize();

        ResetReconnectCounter();
        
        Debug.Log("NetworkModule Uninitialize");
    }

    /// <summary>
    /// 网络更新
    /// </summary>
    private void Update()
    {
        fsm.Update();
    }

    /// <summary>
    /// 初始化网络状态机
    /// </summary>
    private void BindFSM()
    {
        fsm.AddState((int)NetworkFsmState.Init, InitEnter, InitLeave);
        fsm.AddState((int)NetworkFsmState.Connecting, ConnectingEnter, ConnectingLeave);
        fsm.AddState((int)NetworkFsmState.Authing, AuthingEnter);
        fsm.AddState((int)NetworkFsmState.Working, WorkingEnter, WorkingLeave, WorkingUpdate);
        fsm.AddState((int)NetworkFsmState.Reconnecting, ReconnectingEnter, ReconnectingLeave);
        //fsm.AddState((int)NetworkFsmState.ReAuthing, ReAuthingEnter);
        fsm.AddState((int)NetworkFsmState.Disconnect, DisconnectEnter, DisconnectLeave);
        //fsm.AddState((int)NetworkFsmState.Waiting, WaitingEnter);

        // 可以使用 addEvent(int[] fromState, int toState ……)接口，在哪些状态下，收到XX事件，都跳入某个状态
        // 这样可以更方便理解设计，
        // 比如收到断线事件，在授权成功后，都跳入重连状态，走重连，授权如果未成功，则跳入初始状态，走重登入

        // 可以从2个角度考虑到所有情况
        // 从状态的角度，我有哪些状态，如何去下一个状态，正常处理的流程
        // 从事件的角度，如果发生了这个事件，我在不同状态下怎么处理，异常处理的流程

        fsm.addEvent((int)NetworkFsmState.Init, (int)NetworkFsmState.Connecting, (int)NetworkFsmEvent.Connect, OnConnect, 0);
        fsm.addEvent((int)NetworkFsmState.Init, (int)NetworkFsmState.Connecting, (int)NetworkFsmEvent.ConnectWithAuth, null, 0);

        fsm.addEvent((int)NetworkFsmState.Connecting, (int)NetworkFsmState.Authing, (int)NetworkFsmEvent.ConnectSuccess, null, 0);
        fsm.addEvent((int)NetworkFsmState.Connecting, (int)NetworkFsmState.Init, (int)NetworkFsmEvent.ConnectFailure, OnConnectFailure, 0);
        fsm.addEvent((int)NetworkFsmState.Connecting, (int)NetworkFsmState.Init, (int)NetworkFsmEvent.Disconnect, OnConnectFailure, 0);
        fsm.addEvent((int)NetworkFsmState.Connecting, (int)NetworkFsmState.Init, (int)NetworkFsmEvent.Error, OnConnectFailure, 0);
        fsm.addEvent((int)NetworkFsmState.Connecting, (int)NetworkFsmState.Init, (int)NetworkFsmEvent.CancelWaitingQueue, OnCancelWaitingQueue, 0);
        fsm.AddTimer((int)NetworkFsmState.Connecting, (int)NetworkFsmState.Init, connectTimeout, OnCheckTimeout, 0);

        fsm.addEvent((int)NetworkFsmState.Authing, (int)NetworkFsmState.Working, (int)NetworkFsmEvent.Authorized, OnAuthingSuccess, 0);
        fsm.addEvent((int)NetworkFsmState.Authing, (int)NetworkFsmState.Init, (int)NetworkFsmEvent.AuthFailure, OnAuthingFailure, 0);
        fsm.addEvent((int)NetworkFsmState.Authing, (int)NetworkFsmState.Init, (int)NetworkFsmEvent.Disconnect, OnAuthingFailure, 0);
        fsm.addEvent((int)NetworkFsmState.Authing, (int)NetworkFsmState.Init, (int)NetworkFsmEvent.Error, OnAuthingFailure, 0);
        fsm.addEvent((int)NetworkFsmState.Authing, (int)NetworkFsmState.Init, (int)NetworkFsmEvent.SendRequestFailure, OnAuthingFailure, 0);

        fsm.addEvent((int)NetworkFsmState.Working, (int)NetworkFsmState.Init, (int)NetworkFsmEvent.Kick, OnKick, 0);
        fsm.addEvent((int)NetworkFsmState.Working, (int)NetworkFsmState.Reconnecting, (int)NetworkFsmEvent.Disconnect, null, 0);
        fsm.addEvent((int)NetworkFsmState.Working, (int)NetworkFsmState.Reconnecting, (int)NetworkFsmEvent.Error, null, 0);
        fsm.addEvent((int)NetworkFsmState.Working, (int)NetworkFsmState.Reconnecting, (int)NetworkFsmEvent.SendRequestFailure, OnTimeoutFailure, 1);
        fsm.addEvent((int)NetworkFsmState.Working, (int)NetworkFsmState.Reconnecting, (int)NetworkFsmEvent.HeartbeatTimeout, OnTimeoutFailure, 1);

        fsm.addEvent((int)NetworkFsmState.Reconnecting, (int)NetworkFsmState.Working, (int)NetworkFsmEvent.ReconnectSuccess, null, 0);
        fsm.addEvent((int)NetworkFsmState.Reconnecting, (int)NetworkFsmState.Init, (int)NetworkFsmEvent.ReconnectFailure, OnReconnectFailure, 0);
        fsm.addEvent((int)NetworkFsmState.Reconnecting, (int)NetworkFsmState.Init, (int)NetworkFsmEvent.UserDisconnect, null, 0);


        int[] localKickToInitStates = {
            (int)NetworkFsmState.Connecting,
            (int)NetworkFsmState.Authing,
            (int)NetworkFsmState.Working,
            (int)NetworkFsmState.Reconnecting,
            //(int)NetworkFsmState.ReAuthing,
            (int)NetworkFsmState.Disconnect,
        };

        fsm.addEvent(localKickToInitStates, (int)NetworkFsmState.Init, (int)NetworkFsmEvent.LocalKick, null, 0);

        fsm.addEvent(localKickToInitStates, (int)NetworkFsmState.Init, (int)NetworkFsmEvent.Connect, OnConnectErrorState, 0);
    }

    int InitEnter()
    {
        Debug.LogWarning("InitEnter:");
        DisconnectToServer();
        Scheduler.GetInstance().AddTimer(detectConnectionTime, true, DetectConnectionValid);
        return 0;
    }

    int InitLeave()
    {
        Scheduler.GetInstance().RemoveTimer(DetectConnection);
        return 0;
    }
    int OnConnect()
    {
        stayInQueue = false;
        Debug.LogWarning("OnConnect:");

        NotifyNetworkStatus(NetworkStatus.Connecting);

#if UNITY_STANDALONE || UNITY_EDITOR || NoAuth
        ConnectToServer(false);
#else
        ConnectToServer(true);
#endif
        return 0;
    }
    
    int OnKick()
    {
        DisconnectToServer();
        return 0;
    }

    int OnConnectErrorState()
    {
        Debug.LogError("On connect in error state");

        return 0;
    }

    int ConnectingEnter()
    {
        Debug.LogWarning("ConnectingEnter:");
        return 0;
    }

    int ConnectingLeave()
    {
        NotifyNetworkStatus(NetworkStatus.ConnectFinished);
        if (onNetworkAuth != null)
        {
            onNetworkAuth(connectResult);
        }
        return 0;
    }

    int OnConnectFailure()
    {
        NotifyLogin(false, connectResult, loginErrno, loginRsp);
        return 0;
    }

    int OnCancelWaitingQueue()
    {
        return stayInQueue ? 0 : 1;
    }

    int OnCheckTimeout()
    {
        if (stayInQueue)
        {
            return 1;
        }
        else
        {
            loginErrno = ResultCode.RESULT_SERVER_TIMEOUT_ERR;
            connectResult = NetworkResult.CONNECT_FAIL;
            NotifyLogin(false, connectResult, loginErrno, loginRsp);
            return 0;
        }
    }

    int AuthingEnter()
    {
        Debug.LogWarning("AuthingEnter:");
        SetLoginReason(LoginReason.LOGINREASON_NORMAL);
        SendLoginRequest();
        return 0;
    }

    int OnAuthingSuccess()
    {
        NotifyLogin(true, connectResult, loginErrno, loginRsp);
        return 0;
    }

    int OnAuthingFailure()
    {
        StopRetryLogin();
        packageProcessor.ResetRequestStatus();
        NotifyLogin(false, connectResult, loginErrno, loginRsp);
        return 0;
    }

    int OnAuthingRedirect()
    {
        DisconnectToServer();
        return 0;
    }

    int WorkingEnter()
    {
        Debug.LogWarning("WorkingEnter:");
        RegGSNotifyMsg((int)Opcode.OPCODE_KICK_NTF, typeof(KickNtf));
        Scheduler.GetInstance().AddTimer(detectConnectionTime, true, DetectConnection);
        ResetReconnectCounter();
        packageProcessor.ResetHeartbeat();
        return 0;
    }

    int WorkingUpdate()
    {
        packageProcessor.HeartbeatRequest();
        return 0;
    }

    int WorkingLeave()
    {
        Scheduler.GetInstance().RemoveTimer(DetectConnection);
        packageProcessor.ResetRequestStatus();

        return 0;
    }

    int OnTimeoutFailure()
    {
        if (!IsValid())
        {
            return 1;
        }
        return 0;
    }

    int ReconnectingEnter()
    {
        Debug.LogWarning("ReconnectingEnter:");
        NotifyNetworkStatus(NetworkStatus.SilentConnecting);
        StartReconnectProcess();
        return 0;
    }

    int ReconnectingLeave()
    {
        NotifyNetworkStatus(NetworkStatus.ConnectFinished);
        StopReconnectProcess();
        return 0;
    }

    int OnReconnectFailure()
    {
        // 如果是授权过期或者服务器主动断开连接，则停止重连
        if (connectResult == NetworkResult.AUTH_FAIL || connectResult == NetworkResult.FAIL_SERVER_STOP_CONNECTION)
        {
            NotifyReconnectFailed(connectResult);
            return 0;
        }
        return 1;
    }

    //int ReAuthingEnter()
    //{
    //    SetLoginReason(LoginReason.LOGINREASON_RELOGIN);
    //    SendLoginRequest();
    //    return 0;
    //}

    int OnReAuthingSuccess()
    {
        NotifyReLogin(true, connectResult, loginErrno, loginRsp);
        return 0;
    }

    int OnReAuthingFailure()
    {
        StopRetryLogin();
        packageProcessor.ResetRequestStatus();
        NotifyReLogin(false, connectResult, loginErrno, loginRsp);
        return 0;
    }

    int DisconnectEnter()
    {
        Debug.LogWarning("DisconnectEnter:");
        // 也许链路已经断了，但是在执行一次也不会有问题
        DisconnectToServer();

        Scheduler.GetInstance().AddTimer(reconnectTimeout, false, DelayTry);
        
        return 0;
    }

    int DisconnectLeave()
    {
        Scheduler.GetInstance().RemoveTimer(DelayTry);

        return 0;
    }

    void DelayTry()
    {
        // 尝试重连
        if (OnDisconnectCheck() == 0)
        {
            fsm.Translate((int)NetworkFsmState.Reconnecting);
        }
        else
        {
            fsm.Translate((int)NetworkFsmState.Init);
        }
    }
	
    int OnDisconnectCheck()
    {
        if (!CheckReconnectCount())
        {
            NotifyReconnectFailed(connectResult);
            return 1;
        }
        return 0;
    }

    int WaitingEnter()
    {
        Debug.LogWarning("WaitingEnter:");
        return 0;
    }

    private void SetLoginReason(LoginReason loginReason)
    {
        this.loginReason = loginReason;
    }

    /// <summary>
    /// 连接是否正常
    /// </summary>
    /// <returns></returns>
    private bool IsValid()
    {
        return connector.IsValid();
    }

    /// <summary>
    /// 物理网络是否连通
    /// </summary>
    /// <returns></returns>
    public bool IsNetworkReachable()
    {
        return connector.IsNetworkReachable();
    }

    /// <summary>
    /// 获取网络环境
    /// </summary>
    /// <returns></returns>
    public NetworkEnv GetNetworkEnv()
    {
        return connector.GetNetworkEnv();
    }

    /// <summary>
    /// 获取网络运营商
    /// </summary>
    /// <returns></returns>
    public NetworkCarrier GetNetworkCarrier()
    {
        return connector.GetNetworkCarrier();
    }

    public OpenNGS.ERPC.RPCClient GetRPCClient()
    {
        return connector.GetRpcClient();
    }

    /// <summary>
    /// 启动登陆流程，没有登录态
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    /// <param name="platform"></param>
    /// <param name="userName"></param>
    /// <param name="passWord"></param>
    /// <param name="firebaseToken">当platform为None的时候用</param>
    public void StartLogin(string ip, string realmPort, string gatePort,  Platform platform, string userName, string passWord, string firebaseToken = "")
    {
        Debug.Log("NetworkModule StartLogin");

        targetIp = ip;
        targetRealmPort = realmPort;
        targetGatePort = gatePort;
        this.platform = platform;
        username = userName;
        password = passWord;
        this.firebaseToken = firebaseToken;

        fsm.OnEvent((int)NetworkFsmEvent.Connect);
    }

    /// <summary>
    /// 启动登录流程，已经有登录态
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    /// <param name="platform"></param>
    public void StartLoginWithAuth(string ip, string port, Platform platform)
    {
        Debug.Log("Start login with auth");

        targetIp = ip;
        targetRealmPort = port;
        this.platform = platform;

        if (connector != null)
        {
            connector.SetIpAndPort(ip, port);
        }

        fsm.OnEvent((int)NetworkFsmEvent.ConnectWithAuth);
    }

    /// <summary>
    /// 启动登出流程
    /// </summary>
    /// <returns></returns>
    public void StartLogout()
    {
        Debug.Log("Start logout");

        //PlayerInfo.Instance.channelModule.ResetLaunchFrom();

        SCSPkg pkg =  SCSPkg.New<LogoutReq>();
        pkg.body.rsp_op = Opcode.OPCODE_LOGOUT_RSP;
        pkg.body.op = Opcode.OPCODE_LOGOUT_REQ;
        ((LogoutReq)pkg.body.req).uin = 0;
        pkg.body.rsp_type = typeof(LogoutRsp);
        packageProcessor.SendRequest(pkg);
    }

    /// <summary>
    /// 本地登出流程
    /// </summary>
    public void StartLocalLogout()
    {
        Debug.Log("StartLocalLogout");

        //PlayerInfo.Instance.channelModule.ResetLaunchFrom();

        fsm.OnEvent((int) NetworkFsmEvent.LocalKick);

        NotifyKick(KickReason.KICKREASON_LOGOUT);
    }

    /// <summary>
    /// 退出登录排队
    /// </summary>
    public void LeaveLoginQueue()
    {
        fsm.OnEvent((int)NetworkFsmEvent.CancelWaitingQueue);
    }

    public void UserReconnect()
    {
        StopReconnectProcess();
        StartReconnectProcess();
        //fsm.OnEvent((int)NetworkFsmEvent.UserReconnect);
    }

    public void UserDisconnect()
    {
        fsm.OnEvent((int)NetworkFsmEvent.UserDisconnect);
    }

    public void GetLoginServerInfo(out string ip,out string port)
    {
        ip = targetIp;
        port = targetRealmPort;
    }

    public Platform GetSavedPlatform()
    {
        if (connector != null)
        {
            return connector.GetSavedPlatform();
        }
        return Platform.None;
    }

    public bool HasLoginRet()
    {
        if (connector != null)
        {
            return connector.HasLoginRet();
        }
        return false;
    }

    public bool IsWakeUp()
    {
        if (connector != null)
        {
            return connector.IsWakeUp();
        }
        return false;
    }

    public string GetWakeUpData()
    {
        if (connector != null)
        {
            return connector.GetWakeUpData();
        }
        return "";
    }

    public void ClearWakeUpData()
    {
        if (connector != null)
        {
            connector.ClearWakeUpData();
        }
    }

    public string GetUserOpenId()
    {
        if (connector != null)
        {
            return connector.GetOpenId();
        }
        return "";
    }

    public string GetUserAccount()
    {
        return username;
    }

    public string GetPictureUrl()
    {
        if (connector != null)
        {
            return connector.GetPictureUrl();
        }

        return "";
    }

    public void SwitchUser(bool bSwitch)
    {
        if (connector != null)
        {
            connector.SwitchUser(bSwitch);
        }
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    private bool ConnectToServer(bool bMobile)
    {
        Debug.LogFormat("NetworkModule.ConnectToServer(bMobile:{0}, _cmConnectTimeout:{1}, _mIp:{2}, _mPort:{3}, _mPlatform:{4}, _mUserName:{5}, _mPassword:{6})", bMobile, connectTimeout, targetIp, targetRealmPort, platform, username, password);

        bool bRet = connector.Connect(bMobile, connectTimeout, targetIp, targetRealmPort, targetGatePort, platform, username, password);

        Debug.LogFormat("NetworkModule.ConnectToServer ret:{0}", bRet);

        return bRet;
    }

    /// <summary>
    /// 连接服务器回调接口
    /// </summary>
    /// <param name="result">错误码</param>
    private void ConnectEventHandler(NetworkResult result)
    {
        connectResult = result;

        if (result == NetworkResult.SUCCESS)
        {
            Debug.LogWarning("On connect event handler success");
            fsm.LogCurState();
            fsm.OnEvent((int)NetworkFsmEvent.ConnectSuccess);
            isInSession = true;
        }
        else
        {
            Debug.LogWarningFormat("On connect event handler error {0}", result);
            ErrorEventHandler(result);
            fsm.OnEvent((int)NetworkFsmEvent.ConnectFailure);
            isInSession = false;
        }
    }

    /// <summary>
    /// 重连服务器
    /// </summary>
    private bool ReconnectToServer()
    {
        bool bRet = connector.Reconnect(connectTimeout);
        if (bRet)
        {
            Debug.LogWarning("Begin to reconnect to server");
        }
        else
        {
            Debug.LogWarning("Reconnect to server failure");
        }
        return bRet;
    }

    /// <summary>
    /// 重连服务器回调接口
    /// </summary>
    /// <param name="result">错误码</param>
    private void ReconnectEventHandler(NetworkResult result)
    {
        connectResult = result;

        if (result == NetworkResult.SUCCESS)
        {
            Debug.LogWarning("On reconnect event handler success");

            fsm.OnEvent((int)NetworkFsmEvent.ReconnectSuccess);
        }
        else
        {
            Debug.LogWarningFormat("On reconnect event handler error {0}", result);

            fsm.OnEvent((int)NetworkFsmEvent.ReconnectFailure);
        }
    }

    /// <summary>
    /// 连接排队回调接口
    /// </summary>
    /// <param name="position">当前位置</param>
    /// <param name="queueLen">队伍长度</param>
    /// <param name="estimateTime">预估时间秒</param>
    private void StayInQueueEventhandler(int position, int queueLen, int estimateTime)
    {
        if (!stayInQueue)
        {
            stayInQueue = true;
            NotifyNetworkStatus(NetworkStatus.StayInQueue);
        }

        NotifyLoginStayInQueue(position, queueLen, estimateTime);
    }

    /// <summary>
    /// 与服务器断开连接
    /// </summary>
    private void DisconnectToServer()
    {
        Debug.LogWarning("Disconnect to server");
        stayInQueue = false;
        connector.Disconnect();

    }

    /// <summary>
    /// 检测网络连接
    /// </summary>
    private void DetectConnection()
    {
        if (!IsNetworkReachable())
        {
            Debug.LogWarning("Detect connection network is not reachable");

            fsm.OnEvent((int)NetworkFsmEvent.Disconnect);
        }
        else if (!IsValid())
        {
            Debug.LogWarning("Detect connection is not valid");

            fsm.OnEvent((int)NetworkFsmEvent.Disconnect);
        }
    }

    private void DetectConnectionValid()
    {
        //if (IsNetworkReachable() && !IsValid())
        //{
        //    Debug.LogWarning("Detect connection is valid");
        //    // 非工作状态下，恢复网络后自动连接
        //    fsm.OnEvent((int)NetworkFsmEvent.Connect);
        //}
    }

    public Action OnConnectSuccess;
    
    /// <summary>
    /// 发起登陆请求
    /// </summary>
    private void SendLoginRequest()
    {
        OnConnectSuccess?.Invoke();
    }

    /// <summary>
    /// 登陆请求返回服务器正在踢号中时，等待一段时间重发登录请求
    /// </summary>
    private void RetryLoginWhenKicking()
    {
        Scheduler.GetInstance().AddTimer(retryLoginTime, false, SendLoginRequest);
    }

    /// <summary>
    /// 异常状态下离开鉴权状态时，停止重发登录请求
    /// </summary>
    private void StopRetryLogin()
    {
        Scheduler.GetInstance().RemoveTimer(SendLoginRequest);
    }

    /// <summary>
    /// 登陆请求回调接口
    /// </summary>
    /// <param name="errcode"></param>
    /// <param name="rsp"></param>
    public void HandleLoginResponse(int errcode, LoginRsp rsp)
    {
        loginErrno = (ResultCode)errcode;
        loginRsp = rsp;

        NgDebug.LogJson("NetworkModule.HandleLoginResponse", rsp);

        string msg = string.Empty;
        if (CheckSvrRetCode(errcode))
        {
            Debug.LogFormat("NetworkModule.HandleLoginResponse rsp.result:{0}", rsp.result);

            // 功能开关
            //PlayerInfo.Instance.channelModule.SetSwitchOn(rsp.switch_on);
            //PlayerInfo.Instance.Pay.SetZoneId(rsp.midas_zone);

            if (rsp.result == (uint)ResultCode.RESULT_OK)
            {
                fsm.OnEvent((int)NetworkFsmEvent.Authorized);
            }
            else if (rsp.result == (uint)ResultCode.RESULT_LOGIN_NOPLAYER)
            {
                fsm.OnEvent((int)NetworkFsmEvent.Authorized);
            }
            else if (rsp.result == (uint)ResultCode.RESULT_LOGIN_KICKING)
            {
                RetryLoginWhenKicking();
                return;
            }
            else
            {
                fsm.OnEvent((int)NetworkFsmEvent.AuthFailure);
            }
        }
        else
        {
            Debug.LogWarningFormat("NetworkModule.HandleLoginResponse errcode:{0}", errcode);
            fsm.OnEvent((int)NetworkFsmEvent.AuthFailure);
        }
    }

    /// <summary>
    /// 检查服务器应答返回码
    /// </summary>
    /// <param name="no"></param>
    /// <param name="notifyMsg"></param>
    /// <returns></returns>
    public bool CheckSvrRetCode(int no)
    {
        if (no == (int)ResultCode.RESULT_OK)
        {
            return true;
        }
        else
        {
            Debug.LogWarningFormat("CheckSvrRetCode errcode {0}", ((ResultCode)no).ToString());
            return false;
        }
    }

    /// <summary>
    /// 登出请求回调接口
    /// </summary>
    /// <param name="errcode"></param>
    /// <param name="rsp"></param>
    public void HandleLogoutResponse(int errcode, LogoutRsp rsp)
    {
        Debug.Log("Logout response");

        fsm.OnEvent((int)NetworkFsmEvent.Kick);

        NotifyKick(KickReason.KICKREASON_LOGOUT);
    }

    /// <summary>
    /// 服务器踢号通知接口
    /// </summary>
    /// <param name="ntf"></param>
    public void HandlerKickNotify(KickNtf ntf)
    {
        fsm.OnEvent((int)NetworkFsmEvent.Kick);

        if (ntf != null)
        {
            Debug.LogWarningFormat("Handle kick ntf reason {0}", ntf.reason);

            switch (ntf.reason)
            {
                case KickReason.KICKREASON_LOGOUT:
                    // 服务器现在的处理是玩家发登出请求，除了请求应答，还会发踢号通知，在请求应答里已经处理了，这里不用再处理
                    break;
                default:
                    NotifyKick(ntf.reason);
                    break;
            }
        }
        else
        {
            Debug.LogError("Handler kick ntf invalid data");
        }
    }

    /// <summary>
    /// 断开连接回调接口
    /// </summary>
    /// <param name="result">错误码</param>
    private void DisconnectEventHandler(NetworkResult result)
    {
        Debug.LogWarning("Network disconnect");

        isInSession = false;

        //fsm.OnEvent((int)NetworkFsmEvent.Disconnect);

        NotifyNetworkStatus(NetworkStatus.Disconnected);
    }

    /// <summary>
    /// 发生错误回调接口
    /// </summary>
    /// <param name="result">错误码</param>
    private void ErrorEventHandler(NetworkResult result)
    {
        Debug.LogWarning("Network error " + result);

        NotifyError(result);

        if (result == NetworkResult.NETWORK_EXCEPTION)
        {
            fsm.OnEvent((int)NetworkFsmEvent.Error);
        }
    }

    

#region 通知

    /// <summary>
    /// 通知上层网络状态
    /// </summary>
    /// <param name="status"></param>
    private void NotifyNetworkStatus(NetworkStatus status)
    {
#if DEBUG_LOG
        Debug.LogFormat("NetworkModule NotifyNetworkStatus {0}", status);
#endif
        if (onNetworkStatus != null)
        {
            try
            {
                onNetworkStatus(status);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("NetworkModule NotifyNetworkStatus Exception : {0}", e);
            }
        }
    }

    /// <summary>
    /// 通知上层网络错误
    /// </summary>
    private void NotifyError(NetworkResult connectResult)
    {
        Debug.LogFormat("NetworkModule NotifyError {0}", connectResult);
        if (onError != null)
        {
            onError(connectResult);
        }
    }

    /// <summary>
    /// 通知上层登陆结果
    /// </summary>
    private void NotifyLogin(bool success, NetworkResult connectResult, ResultCode loginErrno, LoginRsp loginRsp)
    {
        Debug.LogFormat("NetworkModule NotifyLogin {0}", connectResult);
        if (onLogin != null)
        {
            onLogin(success, connectResult, loginErrno, loginRsp);
        }
    }

    /// <summary>
    /// 通知上层重连登陆结果
    /// </summary>
    private void NotifyReLogin(bool success, NetworkResult connectResult, ResultCode loginErrno, LoginRsp loginRsp)
    {
        if (onReLogin != null)
        {
            onReLogin(success, connectResult, loginErrno, loginRsp);
        }
    }

    /// <summary>
    /// 通知上层登录排队
    /// </summary>
    /// <param name="position">当前位置</param>
    /// <param name="queueLen">队伍长度</param>
    /// <param name="estimateTime">预估时间秒</param>
    private void NotifyLoginStayInQueue(int position, int queueLen, int estimateTime)
    {
        if (onStayInQueueEvent != null)
        {
            onStayInQueueEvent(position, queueLen, estimateTime);
        }
    }

    /// <summary>
    /// 通知上层踢号
    /// </summary>
    private void NotifyKick(KickReason reason)
    {
        if (onKick != null)
        {
            onKick(reason);
        }
    }

    /// <summary>
    /// 通知上层网络环境变化
    /// </summary>
    private void NotifyNetworkChanged()
    {
        if (onNetworkChanged != null)
        {
            onNetworkChanged();
        }
    }

    private void NotifyNetworkWakeUp(NetworkWakeUp wakeUp)
    {
        if (onNetworkWakeUp != null)
        {
            onNetworkWakeUp(wakeUp);
        }
    }

    private void NotifyReconnectFailed(NetworkResult connectResult)
    {
        if (onReconnectFailed != null)
        {
            onReconnectFailed(connectResult);
        }
    }

#endregion



#region 断线重连

    // 断网状态下自动重连时间间隔
    private const int autoReconnectInterval = 5;

    // 重连延迟，隔一小段时间才重连，避免发生网络异常后同步重连
    private const float reconnectDelay = 0.1f;

    // 静默重连超时时间
    private const int slientReconnectTimeout = 5;

    // 重连超时时间
    private const int reconnectTimeout = 5;

    // 最大尝试重连次数
    private const int reconnectMaxCount = 10;

    // 尝试重连计数
    private int reconnectCounter;

    /// <summary>
    /// 重置重连计数器
    /// </summary>
    private void ResetReconnectCounter()
    {
        reconnectCounter = 0;
    }

    /// <summary>
    /// 检查重连次数
    /// </summary>
    /// <returns></returns>
    private bool CheckReconnectCount()
    {
        return ++reconnectCounter < reconnectMaxCount;
    }

    /// <summary>
    /// 开始重连流程
    /// </summary>
    private void StartReconnectProcess()
    {
        NotifyNetworkStatus(NetworkStatus.Reconnecting);
        Scheduler.GetInstance().AddTimer(slientReconnectTimeout, false, SlientReconnectTimeout);
        Scheduler.GetInstance().AddTimer(reconnectDelay, false, DelayReconnect);
    }

    /// <summary>
    /// 结束重连流程
    /// </summary>
    private void StopReconnectProcess()
    {
        Scheduler.GetInstance().RemoveTimer(SlientReconnectTimeout);
        Scheduler.GetInstance().RemoveTimer(DelayReconnect);
    }

    /// <summary>
    /// 开始重连
    /// </summary>
    private void DelayReconnect()
    {
        ReconnectToServer();
    }

    /// <summary>
    /// 静默重连超时
    /// </summary>
    private void SlientReconnectTimeout()
    {
        Debug.Log("Silent reconnect timeout");

        if (OnDisconnectCheck() == 0)
        {
            StopReconnectProcess();
            StartReconnectProcess();
        }
    }

    public void SendRequest(SCSPkg request)
    {
        packageProcessor.SendRequest(request);
    }

    public void RegGSNotifyMsg(int msgid, Type rsp_type)
    {
        packageProcessor.RegGSNotifyMsg(msgid, rsp_type);
    }

    public void UnregGSNotifyHandler(int msgid)
    {
        packageProcessor.UnregGSNotifyHandler(msgid);
    }

#endregion

}
