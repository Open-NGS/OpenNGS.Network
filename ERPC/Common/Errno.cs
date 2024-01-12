using System;

namespace OpenNGS.ERPC
{

    public class ERRNO
    {
        // 参数错误
        public const int INVALID_PARAM = -10;

        public const int SUCCESS = 0;

        // 服务端的错误码[1..99]
        // 服务端解码错误
        public const int SERVER_DECODE_ERR = 1;
        // 服务端编码错误
        public const int SERVER_ENCODE_ERR = 2;
        // 服务端没有调用相应的接口实现
        public const int SERVER_NOFUNC_ERR = 11;
        // 服务端没有调用相应的entity实现
        public const int SERVER_NOENTITY_ERR = 12;
        // 服务端参数错误
        public const int SERVER_INVALID_PARAM = 13;
        // 请求在服务端超时
        public const int SERVER_TIMEOUT_ERR = 21;
        // 请求在服务端过载
        public const int SERVER_OVERLOAD_ERR = 22;
        // 请求在服务端流量过大
        public const int SERVER_OVER_FLOW_CONTROL_ERR = 23;
        // 服务端系统错误
        public const int SERVER_SYSTEM_ERR = 31;
        // 服务端网络错误
        public const int SERVER_NETWORK_ERR = 141;

        // 客户端的错误码[100.199]
        // 请求在客户端调用超时
        public const int CLIENT_INVOKE_TIMEOUT_ERR = 101;
        // 请求在客户端网络层异步返回失败
        public const int CLIENT_INVOKE_ASYNC_NET_FAIL = 102;
        // 客户端系统错误
        public const int CLIENT_SYSTEM_ERR = 111;
        // 客户端编码错误
        public const int CLIENT_ENCODE_ERR = 121;
        // 客户端解码错误
        public const int CLIENT_DECODE_ERR = 122;
        // 客户端选ip路由错误
        public const int CLIENT_ROUTER_ERR = 131;
        // 客户端网络错误
        public const int CLINET_NETWORK_ERR = 141;
        // 客户没有调用相应的接口实现
        public const int CLINET_NOFUNC_ERR = 151;
        // 客户端请求流量过大
        public const int CLIENT_OVER_FLOW_CONTROL_ERR = 161;

        // 定时器错误码[200.299]
        // 无效参数
        public const int TIMER_INVALID_PARA = 200;
        // 定时器不存在
        public const int TIMER_INEXISTENT = 201;
    }

}
