
public enum NetworkResult
{
    SUCCESS = 0,
    CONNECT_FAIL = -1,
    NOCONNECT = -2,
    NETWORK_EXCEPTION = -3,
    FAIL = -4,
    FAIL_SERVER_FULL = -5,
    FAIL_INVALID_PASSWORD = -6,
    FAIL_AUTOLOGIN = -7,
    AUTH_FAIL = -8,
    FAIL_SERVER_STOP_CONNECTION = -9,
};


/// <summary>
/// 请求发送级别
/// </summary>
public enum RequestLevel
{
    NotResend,                  // 只尝试发送一次
    Resend,                     // 联网情况下发送失败或未收到回包，重连后重发
    MustReach,                  // 断网情况下发送请求，联网后也要重发
}
