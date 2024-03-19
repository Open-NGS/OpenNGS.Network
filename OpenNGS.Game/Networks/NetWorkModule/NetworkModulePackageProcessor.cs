using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using OpenNGS.Network;
using OpenNGS.Extension;
using protocol;

public class NetworkModulePackageProcessor
{
    // Gamesvr网络连接接口
    private INetworkConnector Connector;

    INetworkModuleExtension networkExtension;

    public delegate void SendRequestFailHandler();
    public SendRequestFailHandler onSendRequestFail;

    public delegate void HeartBeatTimeoutHandler();
    public HeartBeatTimeoutHandler onHeartBeatTimeOut;

    public NetworkModule.NetworkStatusHandler onNetworkStatus;
    public NetworkModule.MessageHeadStatusHandler onMessageHeadStatus;

    /// <summary>
    /// 上一次请求是否超时，用于减少超时上报，因为第一个请求超时后，后面所有的请求都会超时，后面的超时请求并不重要。
    /// </summary>
    private bool lastRequestTimeout = false;
    /// <summary>
    /// 请求静默超时标志，用于减少重复上报次数
    /// </summary>
    private bool lastSilentRequestTimeout = false;

    private MemoryStream requestStream;
    private MemoryStream responseStream;


    // 协议数据最大长度
    private const int _cmMsgMaxLen = 64512;

    // 静默请求超时时间
    private const float _cmSilentRequestTimeout = 1.0f;

    // 请求超时时间， 不要设置5，避免和心跳超时一样
    private const int _cmRequestTimeout = 4;

    Dictionary<int, Type> _requestMsgDict = new Dictionary<int, Type>();

    public NetworkModulePackageProcessor()
    {

    }

    public void Initialize(INetworkConnector conn, INetworkModuleExtension extension)
    {
        Connector = conn;
        Connector.onRecvedData = RecvedDataHandler;

        requestStream = new MemoryStream(_cmMsgMaxLen);
        responseStream = new MemoryStream(_cmMsgMaxLen);

        networkExtension = extension; //new InnovFramework.NetworkModuleExtension();
        networkExtension.Init();
    }

    public void Uninitialize()
    {
        if (networkExtension != null)
        {
            networkExtension.UnInit();
            networkExtension = null;
        }
        mNetworkNotifyMsg.Clear();
    }

    /// <summary>
    /// 上报超时请求，帮助服务器优化
    /// </summary>
    /// <param name="req"></param>
    /// <param name="timeoutType"></param>
    private void ReportTimeOutRequest(SCSPkg req, int timeoutType)
    {

        //timeoutType为0代表静默超时（1s），可能是接口处理时间过长造成
        if (timeoutType == 0)
        {
            //如果上次静默请求超时，那么就不在继续上报
            if (lastSilentRequestTimeout)
            {
                return;
            }
            //否则，设置静默请求超时，下次不在上报同样操作
            lastSilentRequestTimeout = true;
        }
        else
        {
            //如果上次请求超时，那么就不在继续上报
            if (lastRequestTimeout)
            {
                return;
            }
            //否则，设置本次请求超时，下次不在上报
            lastRequestTimeout = true;
        }

        string info = string.Format("[RequestTimeout][t:{0}]op:{1},s:{2},l:{3}", timeoutType, req.body.op,
            req.body.isSend, req.body.reqLvl);

        //bugly上报日志
        Debug.LogError(info);
    }

    public void SendRequest(SCSPkg request)
    {
        if (!_requestMsgDict.ContainsKey((int)request.body.op))
        {
            _requestMsgDict[(int)request.body.rsp_op] = request.body.rsp_type;
        }
        DoRequestQueue(request);
    }

    /// <summary>
    /// 处理请求队列
    /// </summary>
    private void DoRequestQueue(SCSPkg request)
    {
        if (DoSendRequest(request))
        {
            StartWaitingResponse();
        }
        else
        {
            onSendRequestFail?.Invoke();
        }
        //立即回收
        ProtoPool.Instance.Release(request);
    }
    bool DoSendRequest(SCSPkg pkg)
    {
        if (!Connector.IsValid())
        {
            Debug.LogWarning("Send request gs conn is not valid");
            return false;
        }
        Type pb_type = pkg.body.req.GetType();
        //Debug.Log("Send request type: " + pb_type);


        pkg.head.msgid = (protocol.Opcode)pkg.body.op;
        SerializeRequest(ref requestStream, pkg.head, pkg.body.req);

        byte[] buff = requestStream.GetBuffer();
        return Connector.SendData(buff, (int)requestStream.Length);
    }

    /// <summary>
    /// 请求序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="head"></param>
    /// <param name="body"></param>
    private void SerializeRequest<T>(ref MemoryStream stream, MsgHead head, T body) where T : OpenNGS.IProtoExtension
    {
        stream.SetLength(0);
        stream.Position = 0;

        if (head != null)
        {
            stream.Position = sizeof(int);
            MissQ.Tools.ProtoHelper.Serialize(stream, head);

            stream.Position = 0;
            stream.Write(BitConverter.GetBytes((int)(stream.Length - 4)), 0, 4);
            stream.Position = stream.Length;
        }

        if (body != null)
        {
            MissQ.Tools.ProtoHelper.Serialize(stream, body);
        }
    }

    /// <summary>
    /// 等待服务器应答超时
    /// 请求超时可能引起客户端与服务器数据不一致，需要走重连流程
    /// </summary>
    private void HandleResponseTimeout()
    {
        // 请求超时走重连流程
        onSendRequestFail?.Invoke();
    }

    /// <summary>
    /// 服务器应答
    /// </summary>
    /// <param name="reqseq"></param>
    /// <param name="errcode"></param>
    /// <param name="stream"></param>
    ///
    private void HandleResponse(int msgid, uint reqseq, int errcode, MemoryStream stream)
    {
        // 需要放在Process前面，因为Process中可能再次发送协议
        StopWaitingResponse();

        if (networkExtension != null && _requestMsgDict.TryGetValue(msgid, out var rspType))
            networkExtension.MsgStreamProcess(msgid, rspType, null, errcode, stream);

        HandleGSNotify(msgid, stream);
    }

    /// <summary>
    /// 接收数据回调接口
    /// </summary>
    /// <param name="buff">接收到的数据缓冲区</param>
    /// <param name="iOffset">有效数据开始位置偏移</param>
    /// <param name="iLength">数据长度</param>
    ///
    private void RecvedDataHandler(byte[] buff, int iOffset, int iLength)
    {
        //收到数据，标志网络连通了。所以清除最近一次请求超时标志
        lastRequestTimeout = false;
        lastSilentRequestTimeout = false;
        // 这里假设收到的包都是一个完整的协议包
        int iHDLen = System.BitConverter.ToInt32(buff, iOffset);
        if (iLength < iHDLen + 1)
        {
            return;
        }

        this.responseStream.SetLength(0);
        this.responseStream.Write(buff, iOffset + 4, iHDLen);
        this.responseStream.Position = 0;
        MsgHead head = MissQ.Tools.ProtoHelper.Deserialize<MsgHead>(responseStream);
        if (head == null)
        {
            Debug.LogError("Deserialize protocol head fail");
            return;
        }
        responseStream.SetLength(0);
        responseStream.Write(buff, iOffset + 4 + iHDLen, iLength - 4 - iHDLen);
        this.responseStream.Position = 0;

        if (head.status_data != null)
        {
            onMessageHeadStatus?.Invoke(head.status_data);
        }

        if (head.msgid == Opcode.OPCODE_HEARTBEAT_RSP)   // 心跳应答包
        {
            HeartbeatRsp rsp = MissQ.Tools.ProtoHelper.Deserialize<HeartbeatRsp>(responseStream);
            if (rsp == null)
            {
                Debug.LogError("Deserialize protocol heartbeat body fail");

            }
            else
            {
                HandleHeartbeatResponse(rsp);
                ProtoPool.Instance.Release(rsp);

            }
        }
        else
        {
            // 请求应答包 / GS应答
            HandleResponse((int)head.msgid, 0, head.errcode, responseStream);
        }

        //pb-net Deserialize 时现在会自动从ProtoObjectPool中spawn对象，用完释放回池中
        ProtoPool.Instance.Release(head);
    }

    /// <summary>
    /// 等待请求应答，包含静默等待
    /// </summary>
    private void StartWaitingResponse()
    {
        onNetworkStatus?.Invoke(NetworkModule.NetworkStatus.SilentRequesting);
    }

    /// <summary>
    /// 取消等待请求应答
    /// </summary>
    private void StopWaitingResponse()
    {
        onNetworkStatus?.Invoke(NetworkModule.NetworkStatus.RequestFinished);
    }

    /// <summary>
    /// 静默等待超时
    /// </summary>
    private void SilentWaitingResponseTimeout()
    {
        onNetworkStatus?.Invoke(NetworkModule.NetworkStatus.Requesting);
    }

    /// <summary>
    /// 请求超时
    /// </summary>
    private void WaitingResponseTimeout()
    {
        onNetworkStatus?.Invoke(NetworkModule.NetworkStatus.RequestFinished);
        HandleResponseTimeout();
    }

    /// <summary>
    /// 重置请求状态
    /// </summary>
    public void ResetRequestStatus()
    {
        // 如果有请求未完成则停止等待请求
        StopWaitingResponse();
    }

    //#endregion


    #region 处理GS通知协议
    /// <summary>
    /// 处理NTF到达协议回包
    /// </summary>
    //public void HandleNtfReachRsp(int errcode, ConfirmNtfRsp rsp)
    //{

    //}

    //服务器主动下发协议现在只需要注册关联一次msgid和responseclass type，由反射系统自动处理回调

    Dictionary<int, Type> mNetworkNotifyMsg = new Dictionary<int, Type>();

    public void RegGSNotifyMsg(int msgid, Type rsp_type)
    {
        if (mNetworkNotifyMsg.ContainsKey(msgid))
            UnityEngine.Debug.LogErrorFormat("duplicate msgid:{0}", msgid);
        else
            mNetworkNotifyMsg[msgid] = rsp_type;
    }

    /// <summary>
    ///     处理GS通知协议
    /// </summary>
    /// <param name="msgid">协议号</param>
    /// <param name="stream">协议数据流</param>
    private void HandleGSNotify(int msgid, MemoryStream stream)
    {
        if (mNetworkNotifyMsg.TryGetValue(msgid, out Type value))
        {
            if (networkExtension != null)
                networkExtension.MsgStreamProcess(msgid, value, null, 0, stream);
        }
    }

    /// <summary>
    ///     注销GS通知协议处理器
    /// </summary>
    /// <param name="msgid">协议号</param>
    /// <returns>注销成功返回true；否则，返回false</returns>
    public void UnregGSNotifyHandler(int msgid)
    {
        if (mNetworkNotifyMsg.TryGetValue(msgid, out Type value))
        {
            mNetworkNotifyMsg.Remove(msgid);
        }
    }
    #endregion

    #region 处理心跳逻辑

    /// <summary>
    /// 心跳包相应回调
    /// </summary>
    /// <param name="svrTime"></param>
    public delegate void HeartbeatResponseHandler(uint svrTime);
    public HeartbeatResponseHandler mHeartbeatResponseHandler;

    // 发送心跳包时间间隔 单位：秒
    private const int _cmHeartBeatInterval = 5;

    // 上一次发送心跳包时间
    private DateTime _mHeartbeatSendTime;

    // 心跳超时标记
    private bool _mIsHeartbeatTimeout = false;

    //记录心跳包延迟
    private float _LagTime = 0;

    private float _AvgLagTime = 0;

    public float LagTime
    {
        get { return _AvgLagTime; }
    }

    /// <summary>
    /// 重置心跳信息
    /// </summary>
    public void ResetHeartbeat()
    {
        _mHeartbeatSendTime = DateTime.Now;
        _mIsHeartbeatTimeout = false;
        _LagTime = 0;
    }

    /// <summary>
    /// 判断是否需要发送心跳包，需要则发送
    /// </summary>
    public void HeartbeatRequest()
    {
        //防止低帧率导致数据偏差
        _LagTime += Math.Min(Time.unscaledDeltaTime, 0.1f);

        if (DateTime.Now > _mHeartbeatSendTime.AddSeconds(_cmHeartBeatInterval))
        {
            if (_mIsHeartbeatTimeout)
            {
                Debug.LogWarning("Heartbeat timeout");
                onHeartBeatTimeOut?.Invoke();
                return;
            }
            //UnityEngine.Profiling.Profiler.BeginSample("HeartbeatRequest");

            MsgHead head = (MsgHead)ProtoPool.Instance.Get(typeof(MsgHead));

            head.msgid = Opcode.OPCODE_HEARTBEAT_REQ;
            HeartbeatReq req = (HeartbeatReq)ProtoPool.Instance.Get(typeof(HeartbeatReq));

            req.status = 0;
            // 测试tss
            // req.anti_data = System.Text.Encoding.Default.GetBytes("hello world");



            SerializeRequest<HeartbeatReq>(ref this.requestStream, head, req);

            byte[] buff = this.requestStream.GetBuffer();
            Connector.SendData(buff, (int)this.requestStream.Length);

            ProtoPool.Instance.Release(head);
            ProtoPool.Instance.Release(req);

            _mHeartbeatSendTime = DateTime.Now;
            _mIsHeartbeatTimeout = true;
            _LagTime = 0;
        }
    }

    /// <summary>
    /// 处理心跳应答
    /// </summary>
    /// <param name="rsp">心跳应答包</param>
    private void HandleHeartbeatResponse(HeartbeatRsp rsp)
    {
        _mIsHeartbeatTimeout = false;

        float Lag = _AvgLagTime == 0 ? _LagTime : _AvgLagTime;
        _AvgLagTime = 0.3f * Lag + 0.7f * _LagTime;

        _LagTime = 0;
        if (mHeartbeatResponseHandler != null)
        {
            mHeartbeatResponseHandler(rsp.svr_time);
        }
    }
    #endregion
}
