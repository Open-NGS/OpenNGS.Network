using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Platform
{
	public class PlatformError
	{

		/* 已知错误，未定义错误码 */
		public const int INVALID = -1;
		/** 未知错误 */
		public const int UNKNOWN = -2;

		public const int SUCCESS = 0;
		public const int NO_ASSIGN = 1; /** 没有赋值 */
		public const int CANCEL = 2;
		public const int SYSTEM_ERROR = 3;
		public const int NETWORK_ERROR = 4;
		public const int MSDK_SERVER_ERROR = 5; // MSDK 后台返回错误，参考第三方错误码
		public const int TIMEOUT = 6;
		public const int NOT_SUPPORT = 7;
		public const int OPERATION_SYSTEM_ERROR = 8;
		public const int NEED_PLUGIN = 9;
		public const int NEED_LOGIN = 10;
		public const int INVALID_ARGUMENT = 11;
		public const int NEED_SYSTEM_PERMISSION = 12;
		public const int NEED_CONFIG = 13;
		public const int SERVICE_REFUSE = 14;
		public const int NEED_INSTALL_APP = 15;
		public const int APP_NEED_UPGRADE = 16;
		public const int INITIALIZE_FAILED = 17;
		public const int EMPTY_CHANNEL = 18;
		public const int FUNCTION_DISABLE = 19;
		public const int NEED_REALNAME = 20; // 需实名认证
		public const int REALNAME_FAIL = 21; // 实名认证失败
		public const int IN_PROGRESS = 22; // 上次操作尚未完成，稍微再试
		public const int API_DEPRECATED = 23;
		public const int LIBCURL_ERROR = 24;
		public const int FREQUENCY_LIMIT = 25; //频率限制

		/** 1000 ~ 1099 字段是 LOGIN 模块相关的错误码 */
		public const int LOGIN_UNKNOWN_ERROR = 1000;
		public const int LOGIN_NO_CACHED_DATA = 1001; // 本地没有登录缓存数据
		public const int LOGIN_CACHED_DATA_EXPIRED = 1002; //本地有缓存，但是该缓存已经失效
		public const int LOGIN_KEY_STORE_VERIFY_ERROR = 1004;
		public const int LOGIN_NEED_USER_DATA = 1005;
		/** 严格登录模式下，返回确认码时对应的返回值 */
		public const int LOGIN_CODE_FOR_CONNECT = 1006;

		public const int LOGIN_NEED_USER_DATA_SERVER = 1010;
		public const int LOGIN_URL_USER_LOGIN = 1011; // 异账号：使用URL登陆成功
		public const int LOGIN_NEED_LOGIN = 1012;     // 异账号：需要进入登陆页
		public const int LOGIN_NEED_SELECT_ACCOUNT = 1013; // 异账号：需要弹出异帐号提示
		public const int LOGIN_ACCOUNT_REFRESH = 1014; // 异账号：通过URL将票据刷新


		/** 1100 ~ 1199 字段是 FRIEND 模块相关的错误码 */
		public const int FRIEND_UNKNOWN_ERROR = 1100;

		/** 1200 ~ 1299 字段是 GROUP 模块相关的错误码 */
		public const int GROUP_UNKNOWN_ERROR = 1200;

		/** 1300 ~ 1399 字段是 NOTICE 模块相关的错误码 */
		public const int NOTICE_UNKNOWN_ERROR = 1300;

		/** 1400 ~ 1499 字段是 Push 模块相关的错误码 */
		public const int PUSH_RECEIVER_TEXT = 1400; // 收到推送消息
		public const int PUSH_NOTIFICATION_CLICK = 1401;    // 在通知栏点击收到的消息
		public const int PUSH_NOTIFICATION_SHOW = 1402;     // 收到通知之后，通知栏显示

		/** 1500 ~ 1599 字段是 WEBVIEW 模块相关的错误码 */
		public const int WEBVIEW_UNKNOWN_ERROR = 1500;

		public const int THIRD_ERROR = 9999;// 第三方错误情况，参考第三方错误码
	}
}
