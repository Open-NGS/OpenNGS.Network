using OpenNGS.Platform;
#if SUPERSDK
using LitJson;
using SuperSDKV4;
using SuperSDKV4.Classes;
#else
//using GCloud.MSDK;
#endif
using UnityEngine;

// 用于接收来自Java / Object C层SendMessage时传过来的消息

public interface IThirdpartyProvider
{
    void Init(IThirdpartyCallBack callback);
}
public interface IThirdpartyCallBack
{
    void OnCallBack(string moduleName, string funcName, string result);

    void OnException(int code, string msg);
}
#if SUPERSDK
public class PlatformCallback : MonoSingleton<PlatformCallback>,  ICallBackManager, IThirdpartyProvider, ISingleton
#else
public class PlatformCallback : MonoSingleton<PlatformCallback>, IThirdpartyProvider, ISingleton
#endif
{
    private IThirdpartyCallBack thirdCallBack;
    public void OnCreate()
    {
        Debug.Log("OpenNGSPlatform::OnCreate");
#if SUPERSDK
        //初始化
        SuperSDK.getInstance().Init(this.gameObject.name, this);
        Midas.UnityPay.MidasPayService.Instance.InitializeThirtpartyProvider(this);
#endif
    }
    private void Awake()
    {
        Debug.Log("OpenNGSPlatform::Awake");
    }

    public void CallBack(string msg)
	{
        Debug.Log("OpenNGSPlatform::CallBack:" + msg);
#if SUPERSDK && UNITY_ANDROID

        AndroidCallSuperSDK.GetInstance().CallBack(msg);
#endif
    }

    private void Start()
    {
        Debug.Log("OpenNGSPlatform::ActivityStart:");
#if !SUPERSDK
        //MSDKLogin.LoginRetEvent += OnMSDKLoginRet;
        //MSDKLogin.LoginBaseRetEvent += OnMSDKBaseRet;
#endif
    }
#if !SUPERSDK
    //private void OnMSDKBaseRet(GCloud.MSDK.MSDKBaseRet ret)
    //{
    //    PlatformLogin.OnBastRet(new PlatformBaseRet(ret.ToString()));
    //}

    //private void OnMSDKLoginRet(GCloud.MSDK.MSDKBaseRet ret)
    //{
    //    PlatformLogin.OnLoginRet(new PlatformLoginRet(ret.ToString()));
    //}
#endif
    private void OnSuperSDKLoginRet(PlatformLoginRet ret)
    {
#if SUPERSDK
        INTLLogin.GetOpenId(ret, PlatformLogin.OnLoginRet);
#else
        PlatformLogin.OnLoginRet(ret);
#endif
    }

    public void Init(IThirdpartyCallBack callback)
	{
        Debug.Log("OpenNGSPlatform::Init ThirdpartyCallBack");
        thirdCallBack = callback;
	}

#if SUPERSDK

    public void OnCallBack(string moduleName, string funcName, string result)
    {
        Debug.Log("OpenNGSPlatform::OnCallBack");
        Debug.Log("---------------SuperSDK callback ---------------------");
        Debug.Log("moduleName:" + moduleName);
        Debug.Log("funcName:" + funcName);
        Debug.Log("result:" + result);
        var resultData = JsonMapper.ToObject(result);
        //初始化回调
        if (moduleName.Equals(SuperSDKPlatform.MODULE_NAME))
        {
            if (funcName.Equals(SuperSDKPlatform.FUNC_INIT))
            {
                Debug.Log("---------------SuperSDK initCallBack ---------------------");
                PlatformReport.Report(PlatformReportEvent.Start,true);
                if ((int)resultData["code"] == SuperSDKConstants.SUCCESS)
                {
                    Debug.Log("init success");
                    SuperSDKEnv.Init();
                    NetworkHandlerHub.analytics.OnPlatformInit(true);
                }
                else
                {
                    NetworkHandlerHub.analytics.OnPlatformInit(false);
                    Debug.Log("init error");
                    PlatformBaseRet baseRet = new PlatformBaseRet();
                    baseRet.MethodNameId = (int)MSDKMethodNameID.MSDK_LOGIN_LOGOUT;
                    baseRet.RetCode = PlatformError.INITIALIZE_FAILED;
                    baseRet.ThirdCode = (int)resultData["code"];
                    baseRet.RetMsg = resultData["msg"].ToString();
                    PlatformLogin.OnBastRet(baseRet);
                }
            }
            //登录回调
            else if (funcName.Equals(SuperSDKPlatform.FUNC_LOGIN))
            {
                Debug.Log("---------------SuperSDK loginCallBack ---------------------");

                var code = (int)resultData["code"];
                PlatformLoginRet loginRet = new PlatformLoginRet();
                loginRet.Channel = MSDKChannel.Youzu;
                loginRet.MethodNameId = (int)MSDKMethodNameID.MSDK_LOGIN_LOGIN;
                loginRet.ThirdCode = code;
                if (code == SuperSDKConstants.SUCCESS)
                {
                    /*
                     * //"{\"osdk_game_id\":\"196377985\",\"user_id\":\"57\",\"login_sdk_name\":\"Demo\",\"channel_id\":\"0\",\"extend\":\"2150|2016302|opgameid\",\"account_system_id\":\"0060000\",\"osdk_user_id\":\"0060000_57\",\"ip\":\"103.7.29.9\",\"country\":\"HK\",\"time\":1614665207,\"sign\":\"d3a363f888173acd854421cf1f58f2c5\"}"    string
                     * */
                    var msg = resultData["data"];
                    //保存Ticket，需要发送游戏服务端进行验证
                    string ticket = msg["osdk_ticket"].ToString();
                    string ticketJson = StringUtil.Base64ToCommonString(ticket);
                    var userData = JsonMapper.ToObject(ticketJson);

                    Debug.Log("Login Success : token = " + ticketJson);
                    loginRet.RetCode = PlatformError.SUCCESS;
                    loginRet.RetMsg = msg["msg"].ToString();
                    loginRet.UserName = msg["userinfo"]["login_sdk_name"].ToString();
                    loginRet.OpenId = userData["osdk_user_id"].ToString();
                    loginRet.Token = ticket;
                    SuperSDKEnv.account = loginRet.OpenId;
                    SuperSDKEnv.channel_id = userData["channel_id"].ToString();
                    loginRet.ChannelId = int.Parse(SuperSDKEnv.channel_id);
                    loginRet.ExtraJson = "";
                    loginRet.Pf = $@"youzu_m_youzu-2001-{(Application.platform == RuntimePlatform.Android ? "android" : "ios")}";
                    loginRet.PfKey = "pfKey";
                    loginRet.PictureUrl = "";
                    loginRet.RealNameAuth = false;
                    
                    NetworkHandlerHub.analytics.OnPlatformLogin(true);
                }
                else
                {
                    //SDK登录失败
                    loginRet.RetCode = code;
                    NetworkHandlerHub.analytics.OnPlatformLogin(false);
                    Debug.Log("Login Error");
                }
                OnSuperSDKLoginRet(loginRet);
            }
            //注销回调
            else if (funcName.Equals(SuperSDKPlatform.FUNC_LOGOUT))
            {
                Debug.Log("---------------SuperSDK logoutCallBack ---------------------");
                var code = (int)resultData["code"];
                var msg = resultData["msg"].ToString();
                PlatformBaseRet baseRet = new PlatformBaseRet();
                baseRet.MethodNameId = (int)MSDKMethodNameID.MSDK_LOGIN_LOGOUT;
                baseRet.RetCode = code;
                baseRet.ThirdCode = code;
                baseRet.RetMsg = msg;
                PlatformLogin.OnBastRet(baseRet);
            }
            //获取订单回调
            else if (funcName.Equals(SuperSDKPlatform.FUNC_PAY_ORDER_ID))
            {
                Debug.Log("---------------SuperSDK getOrderIdCallBack ---------------------");

                if ((int)resultData["code"] == SuperSDKConstants.SUCCESS)
                {
                    //获取订单成功
                    var orderId = JsonMapper.ToJson(resultData["data"]);

                    Debug.Log("orderId :" + orderId);
                }
                else
                {
                    //获取订单失败
                    Debug.Log("get orderId failed :" + resultData["msg"]);
                }
            }
            // 获取货币本地化接口
            else if (funcName.Equals("getProductsInfoJson"))
            {
                Debug.Log("---------------SuperSDK getProductsInfo CallBack ---------------------");
                var ret = (int)resultData["code"];
                if (ret == SuperSDKConstants.SUCCESS)
                {
                    //获取货币本地化单成功
                    SuperSDKPay.GetLocalPriceCallback(PlatformError.SUCCESS, resultData["data"]);
                }
                else
                {
                    SuperSDKPay.GetLocalPriceCallback(PlatformError.INVALID, null);
                    //获取货币本地化单失败
                    Debug.Log("getProductsInfo failed :" + ret + " msg:" + resultData["msg"]);
                }
            }
            //支付回调
            else if (funcName.Equals(SuperSDKPlatform.FUNC_PAY))
            {
                Debug.Log("---------------SuperSDK payCallBack ---------------------");

                if (midasCallback != null)
                    midasCallback.OnCallBack(moduleName, funcName, result);

                if ((int)resultData["code"] == SuperSDKConstants.SUCCESS)
                    Debug.Log("Pay Succ");
                else
                    Debug.Log("Pay Failed");
            }
            //退出回调
            else if (funcName.Equals(SuperSDKPlatform.FUNC_EXIT))
            {
                Debug.Log("---------------SuperSDK exitCallBack ---------------------");
                if ((int)resultData["code"] == SuperSDKConstants.SUCCESS) Application.Quit();
            }
            //other回调
            else if (funcName.Equals(SuperSDKPlatform.FUNC_OTHER_FUNCTION))
            {
                Debug.Log("---------------SuperSDK callSpecialCallBack ---------------------");
            }
        }
        else if (moduleName.Equals(SuperSDKTools.MODULE_NAME))
        {
            // 获取客户端IP信息的回调
            if (funcName.Equals(SuperSDKTools.FUNC_GET_IPINFO))
            {
                //"{\"code\":1,\"data\":\"{\\\"status\\\":1,\\\"msg\\\":\\\"成功\\\",\\\"data\\\":{\\\"ip\\\":\\\"103.7.29.8\\\",\\\"country\\\":\\\"HK\\\",\\\"continent\\\":\\\"AP\\\"}}\",\"msg\":\"success\"}"
                if ((int)resultData["code"] == SuperSDKConstants.SUCCESS)
                {
                    Debug.Log("GetIPInfo.data:" + resultData["data"].ToString());
                    var msg = JsonMapper.ToObject(resultData["data"].ToString());
                    SuperSDKEnv.client_ip = msg["data"]["ip"].ToString();
                    SuperSDKEnv.ipcountry = msg["data"]["country"].ToString();
                    SuperSDKEnv.ipcontinent = msg["data"]["continent"].ToString();
                }
                else
                    Debug.Log("FUNC_GET_IPINFO Failed");
            }

        }
        else if (moduleName.Equals("xsdk"))
        {
            if (funcName.Equals("queryServers"))
                Debug.Log("queryServers result:" + result);
            else if (funcName.Equals("queryRoles"))
                Debug.Log("queryRoles result:" + result);
            else if (funcName.Equals("deleteRole")) Debug.Log("deleteRole result:" + result);
        }
        else if (moduleName.Equals("push"))
        {
            if (funcName.Equals("receiveRemoteNotification"))
            {
                Debug.Log("---------------SuperSDK receivePushCallBack ---------------------");
                Debug.Log("收到远程推送消息:" + JsonMapper.ToJson(resultData["data"]));
            }
            else if (funcName.Equals("addLocalNotifcation"))
            {
                var myData = JsonMapper.ToObject(result);
                var code = (int)myData["code"];
                if (code == SuperSDKConstants.SUCCESS)
                {
                    //添加本地推送成功"
                }
                else if (code == -2)
                {
                    //本地已存在该identifier的任务，请根据id调用删除本地接口后，再添加
                }

                var data = JsonMapper.ToObject(JsonMapper.ToJson(myData["data"]));
                //推送的消息放在data里面，为json格式的字符串
            }
            else if (funcName.Equals("fetchLocalNotification"))
            {
                var myData = JsonMapper.ToObject(result);
                var code = (int)myData["code"];
                if (code == SuperSDKConstants.SUCCESS)
                {
                    var data = JsonMapper.ToObject(JsonMapper.ToJson(myData["data"]));
                    //该本地推送存在,推送的详细信息为data数据
                }
            }
        }
        else if (moduleName.Equals("h5sdk"))
        {
            if (funcName.Equals("callNativeAPIForPay"))
            {
                Debug.Log("---------------SuperSDK callNativeAPIForPay ---------------------");

                var productId = resultData["data"].ToString();
                //游戏匹配productId 进行支付处理
                DealForH5Pay(productId);
            }
            else if (funcName.Equals("callNativeAPIForCommon"))
            {
                Debug.Log("---------------SuperSDK callNativeAPIForCommon ---------------------");
                //通过此回调，H5SDK传参数给原生端，原生端实现某种协议功能，所以此处最好能够热更实现功能。
            }
            else if (funcName.Equals("H5ViewDidClose"))
            {
                Debug.Log("---------------SuperSDK H5ViewDidClose ---------------------");
                //h5页面关闭，如果在打开h5页面时，关闭了游戏声音，请在收到此回调后恢复游戏声音。
            }
        }
    }

    public void OnException(int code, string msg)
    {
		Debug.Log("---------------SuperSDK execptionCallBack ---------------------");
		Debug.Log("execption code:" + code);
		Debug.Log("execption msg:" + msg);

	}

    private void DealForH5Pay(string productId)
    {
        switch (productId)
        {
            case "xxx":
                break;
        }
    }
#else
    public void OnCallBack(PlatformLoginRet _ret)
    {
        OnSuperSDKLoginRet(_ret);
    }
    public void OnCasCallBack(PlatformCasRet _ret)
    {
        PlatformCas.OnCasRet(_ret);
    }
    public void OnReportCallBack(PlatformReportRet _ret)
    {
        PlatformReport.OnReportRet(_ret);
    }
    public void OnNoticeCallBack(PlatformNoticeRet _ret)
    {
        PlatformNotice.OnNoticeRet(_ret);
    }

#endif
}
