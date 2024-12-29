using OpenNGS.SDK.Auth;
using OpenNGS.SDK.Core.Initiallization;
using OpenNGS.SDK.Core;
using UnityEngine;
using OpenNGS.SDK.Log;

public class AuthcationDemo : MonoBehaviour
{
    public string UserName;
    public string Password;
    public string EmailAddress = "kanegu@eegames.com";
    public string PhoneNumber = "13311788813";
    public string VerifyCode;

    [ContextMenu("LoginAccount")]
    private void LoginAccount()
    {
        Debug.Log("Test Auth ------------------------------- !");

        OpenNGSPlatformServices.Initialize(new InitializationOptions()
        {
            AppId = "iboN4V3anKRnsKwgudonW0ESxGwJLNUz2rhN",
            AppSecret = "YDwqSQ5Be0oGIpQJ6sPBlHLHveRTfC5p"
        }, new SDKLogger());

        AuthcationService.Instance.LoginCallback += (result) =>
        {
            SDKLog.Info($"[ÕËºÅÃÜÂëµÇÂ¼³É¹¦] ÓÃ»§:[{result.Nickname}]");
        };
        
        AuthcationService.Instance.LoginResultCallback += (result) =>
        {
            SDKResultCode _res = (SDKResultCode)result;
            if (_res != SDKResultCode.RESULT_OK)
            {
                Debug.Log($"µÇÂ¼´íÎó {_res}");
            }
        };

        AuthcationService.Instance.LoginByUsernamePassword(UserName, Password);
    }

    [ContextMenu("TestAutoLogin")]
    private void TestAutoLogin()
    {
        Debug.Log("Test Auth ------------------------------- !");

        OpenNGSPlatformServices.Initialize(new InitializationOptions()
        {
            AppId = "iboN4V3anKRnsKwgudonW0ESxGwJLNUz2rhN",
            AppSecret = "YDwqSQ5Be0oGIpQJ6sPBlHLHveRTfC5p"
        }, new SDKLogger());

        AuthcationService.Instance.AutoLoginCallback += (result) =>
        {
            if (result)
            {
                Debug.Log($"ÒÑ×Ô¶¯µÇÂ¼ {AuthcationService.Instance.User.Nickname}");
            }
            else
            {
                Debug.Log("Î´µÇÂ¼£¬ÕýÔÚµÇÂ½....");
                AuthcationService.Instance.LoginByUsernamePassword(UserName, Password);
            }
        };

        AuthcationService.Instance.AutoLogin();

        Debug.Log("Test Auth End ---------------------------- !");
    }

    [ContextMenu("RequestVerifyCodeEmail")]
    private void RequestVerifyCodeEmail()
    {
        OpenNGSPlatformServices.Initialize(new InitializationOptions()
        {
            AppId = "iboN4V3anKRnsKwgudonW0ESxGwJLNUz2rhN",
            AppSecret = "YDwqSQ5Be0oGIpQJ6sPBlHLHveRTfC5p"
        }, new SDKLogger());

        AuthcationService.Instance.VerificationCodeCallback += (result) =>
        {
            SDKLog.Info($"[RequestVerifyCodeEmail] code:[{result}]");
        };
        AuthcationService.Instance.SendVerificationCode( OpenNGS.SDK.Auth.Models.VerificationType.Email,
            EmailAddress);
    }


    [ContextMenu("RequestVerifyCodePhone")]
    private void RequestVerifyCodePhone()
    {
        OpenNGSPlatformServices.Initialize(new InitializationOptions()
        {
            AppId = "iboN4V3anKRnsKwgudonW0ESxGwJLNUz2rhN",
            AppSecret = "YDwqSQ5Be0oGIpQJ6sPBlHLHveRTfC5p"
        }, new SDKLogger());

        AuthcationService.Instance.VerificationCodeCallback += (result) =>
        {
            SDKLog.Info($"[RequestVerifyCodePhone] code:[{result}]");
        };
        AuthcationService.Instance.SendVerificationCode(OpenNGS.SDK.Auth.Models.VerificationType.Phone,
            PhoneNumber);
    }

    [ContextMenu("TryLoginPhone")]
    private void TryLoginPhone()
    {
        OpenNGSPlatformServices.Initialize(new InitializationOptions()
        {
            AppId = "iboN4V3anKRnsKwgudonW0ESxGwJLNUz2rhN",
            AppSecret = "YDwqSQ5Be0oGIpQJ6sPBlHLHveRTfC5p"
        }, new SDKLogger());

        AuthcationService.Instance.LoginCallback += (result) =>
        {
            SDKLog.Info($"[LoginCallback] ÓÃ»§[{result.Nickname}] µÇÂ¼³É¹¦ code:[{result}]");
        };

        AuthcationService.Instance.LoginResultCallback += (result) =>
        {
            SDKLog.Error($"[LoginCallback] µÇÂ¼´íÎó code:[{result}]");
        };


        AuthcationService.Instance.LoginOrRegisterByVerificationCode(
            OpenNGS.SDK.Auth.Models.VerificationType.Phone,
            PhoneNumber,
            VerifyCode);
    }

    [ContextMenu("TryLoginEmail")]
    private void TryLoginEmail()
    {
        OpenNGSPlatformServices.Initialize(new InitializationOptions()
        {
            AppId = "iboN4V3anKRnsKwgudonW0ESxGwJLNUz2rhN",
            AppSecret = "YDwqSQ5Be0oGIpQJ6sPBlHLHveRTfC5p"
        }, new SDKLogger());

        AuthcationService.Instance.LoginCallback += (result) =>
        {
            SDKLog.Info($"[LoginCallback] ÓÃ»§[{result.Nickname}] µÇÂ¼³É¹¦ code:[{result}]");
        };

        AuthcationService.Instance.LoginResultCallback += (result) =>
        {
            SDKLog.Error($"[LoginCallback] µÇÂ¼´íÎó code:[{result}]");
        };


        AuthcationService.Instance.LoginOrRegisterByVerificationCode( 
            OpenNGS.SDK.Auth.Models.VerificationType.Email,
            EmailAddress,
            VerifyCode);
    }
    [ContextMenu("TryLogout")]
    private void TryLogout()
    {
        OpenNGSPlatformServices.Initialize(new InitializationOptions()
        {
            AppId = "iboN4V3anKRnsKwgudonW0ESxGwJLNUz2rhN",
            AppSecret = "YDwqSQ5Be0oGIpQJ6sPBlHLHveRTfC5p"
        }, new SDKLogger());

        SDKLog.Info($"[Logout]");

        AuthcationService.Instance.Logout();
    }
    // Start is called before the first frame update
    void Start()
    {
        //TestLogin();
    }
}
