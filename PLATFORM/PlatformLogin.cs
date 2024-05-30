using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.Platform
{
    // SDK 桥接接口

    public class PlatformLogin
    {
        public delegate void OnPlatformRetEventHandler<T>(T ret);

        /// <summary>
        /// 登出回调、应用唤醒回调
        /// </summary>
        public static event OnPlatformRetEventHandler<PlatformBaseRet> BaseRetEvent;

        /// <summary>
        //// 登录回调，包括 login、bind、autologin、switchuser 等
        /// </summary>
        public static event OnPlatformRetEventHandler<PlatformLoginRet> LoginRetEvent;


        /// <summary>
        /// 登录指定渠道
        /// </summary>
        /// <param name="channel">渠道信息，比如“WeChat”、“QQ”、“Facebook”.</param>
        /// <param name="permissions">登录授权权限列表，多个权限用逗号分隔，比如 user_info,inapp_friends.</param>
        /// <param name="subChannel">子渠道.</param>
        /// <param name="extraJson">Extra json.</param>
        public static void Login(string channel, string permissions = "", string subChannel = "", string extraJson = "")
        {
            Debug.Log("[Platform]Login:" + channel);

            if (!Platform.IsSupported(PLATFORM_MODULE.LOGIN))
                return;
            ILoginProvider _loginProvider = Platform.GetLogin();
            if(_loginProvider != null)
            {
                _loginProvider.Login(channel, permissions, subChannel, extraJson);
            }
        }

        public static void RegisteTable(string protocol, string applicationPath)
        {
            //string keyPath = "HKEY_CLASSES_ROOT\\" + protocol;
            //System.Diagnostics.Process.Start("reg", "add \"" + keyPath + "\"");

            //// 设置默认值为URL:自定义协议
            //System.Diagnostics.Process.Start("reg", "add \"" + keyPath + "\" /v \"URL Protocol\" /d \"\"");

            //// 设置URL协议的命令行
            //string defaultIconKeyPath = keyPath + "\\DefaultIcon";
            //System.Diagnostics.Process.Start("reg", "add \"" + defaultIconKeyPath + "\" /ve /t REG_SZ /d \"" + applicationPath + ",1\" /f");

            //// 设置打开命令
            //string commandKeyPath = keyPath + "\\shell\\open\\command";
            //System.Diagnostics.Process.Start("reg", "add \"" + commandKeyPath + "\" /ve /t REG_SZ /d \"" + applicationPath + "\\\"%1\"");
        }

        public static void SwitchUser(bool useLaunchUser)
        {
            if (!Platform.IsSupported(PLATFORM_MODULE.LOGIN))
                return;
            Platform.GetLogin().SwitchUser(true);
        }
        public static void Logout(string channel="")
        {
            Debug.Log("[Platform]Logout:" + channel);
            if (!Platform.IsSupported(PLATFORM_MODULE.LOGIN))
                return;
            Platform.GetLogin().Logout(channel);
        }

        // MSDK 基础接口
        internal static void OnBastRet(PlatformBaseRet ret)
        {
            Debug.Log("[Platform]OnBastRet:" + ret.ToJsonString());
            if (BaseRetEvent != null)
                BaseRetEvent(ret);
        }

        internal static void OnLoginRet(PlatformLoginRet ret)
        {
            Debug.Log("[Platform]OnLoginRet:" + ret.ToJsonString());
            if (LoginRetEvent != null)
                LoginRetEvent(ret);
        }

        public static PlatformLoginRet GetLoginRet()
        {
            return Platform.GetLogin().GetLoginRet();
        }

        public static void AutoLogin()
        {
            Platform.GetLogin().AutoLogin();
        }

    }


    public class PlatformLoginRet : PlatformBaseRet
    {
        // 用户 ID
        private string openID;

        private string token;

        private long tokenExpire;

        private int firstLogin;

        private string regChannelDis;

        private string userName;

        private int gender;

        private string birthdate;

        private string pictureUrl;

        private string pf;

        private string pfKey;

        private bool realNameAuth;

        private int channelID;

        private string channel;

        private string channelInfo;

        private string confirmCode;

        private long confirmCodeExpireTime;

        private string bindList;

        /// <summary>
        /// 用户 ID
        /// </summary>
        /// <value>The open identifier.</value>
        [JsonProp("openid")]
        public string OpenId
        {
            get { return openID; }
            set { openID = value; }
        }

        /// <summary>
        /// 用户 凭证
        /// </summary>
        /// <value>The token.</value>
        [JsonProp("token")]
        public string Token
        {
            get { return token; }
            set { token = value; }
        }

        /// <summary>
        /// 过期时间
        /// </summary>
        /// <value>The token expire.</value>
        [JsonProp("token_expire_time")]
        public long TokenExpire
        {
            get { return tokenExpire; }
            set { tokenExpire = value; }
        }

        /// <summary>
        /// 是否首次登陆，未知-1，否0，是1
        /// </summary>
        /// <value>The first login.</value>
        [JsonProp("first")]
        public int FirstLogin
        {
            get { return firstLogin; }
            set { firstLogin = value; }
        }

        /// <summary>
        /// 首次注册的分发渠道
        /// </summary>
        /// <value>The reg channel dis.</value>
        [JsonProp("reg_channel_dis")]
        public string RegChannelDis
        {
            get { return regChannelDis; }
            set { regChannelDis = value; }
        }

        /// <summary>
        ///  昵称
        /// </summary>
        /// <value>The name of the user.</value>
        [JsonProp("user_name")]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// 性别(int) 0未定义,1男, 2女
        /// </summary>
        /// <value>The gender.</value>
        [JsonProp("gender")]
        public int Gender
        {
            get { return gender; }
            set { gender = value; }
        }

        /// <summary>
        /// 出生日期(1987-2-23 11:33:33)
        /// </summary>
        /// <value>The birthdate.</value>
        [JsonProp("birthdate")]
        public string Birthdate
        {
            get { return birthdate; }
            set { birthdate = value; }
        }


        /// <summary>
        //// 头像链接
        /// </summary>
        /// <value>The picture URL.</value>
        [JsonProp("picture_url")]
        public string PictureUrl
        {
            get { return pictureUrl; }
            set { pictureUrl = value; }
        }

        /// <summary>
        /// pf 值
        /// </summary>
        /// <value>The pf.</value>
        [JsonProp("pf")]
        public string Pf
        {
            get { return pf; }
            set { pf = value; }
        }

        // pf key
        [JsonProp("pfKey")]
        public string PfKey
        {
            get { return pfKey; }
            set { pfKey = value; }
        }

        // 是否需要实名认证
        [JsonProp("need_name_auth")]
        public bool RealNameAuth
        {
            get { return realNameAuth; }
            set { realNameAuth = value; }
        }

        // 渠道ID
        [JsonProp("channelid")]
        public int ChannelId
        {
            get { return channelID; }
            set { channelID = value; }
        }

        // 渠道名
        [JsonProp("channel")]
        public string Channel
        {
            get { return channel; }
            set { channel = value; }
        }

        // 第三方渠道登录信息
        [JsonProp("channel_info")]
        public string ChannelInfo
        {
            get { return channelInfo; }
            set { channelInfo = value; }
        }

        /// <summary>
        /// 确认码，绑定失败后返回
        /// </summary>
        /// <value>The Confirm Code.</value>
        [JsonProp("confirm_code")]
        public string ConfirmCode
        {
            get { return confirmCode; }
            set { confirmCode = value; }
        }

        /// <summary>
        /// 确认码过期时间戳
        /// </summary>
        /// <value>The Confirm Code Expire Time.</value>
        [JsonProp("confirm_code_expire_time")]
        public long ConfirmCodeExpireTime
        {
            get { return confirmCodeExpireTime; }
            set { confirmCodeExpireTime = value; }
        }

        /// <summary>
        /// 绑定信息(JSON 数据，数组类型)
        /// </summary>
        /// <value>The Bind List.</value>
        [JsonProp("bind_list")]
        public string BindList
        {
            get { return bindList; }
            set { bindList = value; }
        }

        public PlatformLoginRet()
        {
        }

        public PlatformLoginRet(string param) : base(param)
        {
        }

        public PlatformLoginRet(object json) : base(json)
        {
        }
    }

}
