using OpenNGS.SDK.Auth;
using OpenNGS.SDK.Core.Initiallization;
using OpenNGS.SDK.Core;
using UnityEngine;
using OpenNGS.SDK.Log;
using OpenNGS.Platform;
using OpenNGS.Platform.EEGames;
using OpenNGS.SDK.Auth.Models;

public class AuthcationEEgamesLogin : MonoBehaviour
{
    public string UserName;
    public string Password;
    public string EmailAddress = "kanegu@eegames.com";
    public string PhoneNumber = "13311788813";
    public string VerifyCode;
    //public VerificationType VerifyType = VerificationType.Phone;

    [ContextMenu("LoginAccount")]
    private void LoginAccount()
    {
        EEGamesSDKProvider m_sdkProvider = new EEGamesSDKProvider();
        Platform.Init(m_sdkProvider);
        PlatformLogin.LoginRetEvent += OnLoginResult;

        EEGamesLoginData _val = new EEGamesLoginData();
        _val.UserName = UserName;
        _val.Password = Password;
        _TryLogin(_val);
    }

    private void _TryLogin(EEGamesLoginData _val)
    {
        fastJSON.JSONParameters jp = new fastJSON.JSONParameters();
        jp.EnableAnonymousTypes = true;
        string strResult = fastJSON.JSON.ToJSON(_val, jp);
        PlatformLogin.Login("Channel", "", "", strResult);
    }
    public void OnLoginResult(PlatformLoginRet _ret)
    {

    }

    [ContextMenu("TestAutoLogin")]
    private void TestAutoLogin()
    {
        //EEGamesSDKProvider m_sdkProvider = new EEGamesSDKProvider();
        //Platform.Init(m_sdkProvider);
        //PlatformLogin.LoginRetEvent += OnLoginResult;

        //EEGamesLoginData _val = new EEGamesLoginData();
        //_val.UserName = UserName;
        //_val.Password = Password;
        //_TryLogin(_val);
    }

    [ContextMenu("RequestVerifyCodeEmail")]
    private void RequestVerifyCodeEmail()
    {
        EEGamesSDKProvider m_sdkProvider = new EEGamesSDKProvider();
        Platform.Init(m_sdkProvider);
        PlatformLogin.LoginRetEvent += OnLoginResult;

        EEGamesLoginData _val = new EEGamesLoginData();
        _val.RequestVerifyCode = true;
        _val.Account = EmailAddress;
        _val.VerifyTyp = VerificationType.Email;
        _TryLogin(_val);
    }


    [ContextMenu("RequestVerifyCodePhone")]
    private void RequestVerifyCodePhone()
    {
        EEGamesSDKProvider m_sdkProvider = new EEGamesSDKProvider();
        Platform.Init(m_sdkProvider);
        PlatformLogin.LoginRetEvent += OnLoginResult;

        EEGamesLoginData _val = new EEGamesLoginData();
        _val.RequestVerifyCode = true;
        _val.Account = PhoneNumber;
        _val.VerifyTyp = VerificationType.Phone;
        _TryLogin(_val);
    }

    [ContextMenu("TryLoginPhone")]
    private void TryLoginPhone()
    {
        EEGamesSDKProvider m_sdkProvider = new EEGamesSDKProvider();
        Platform.Init(m_sdkProvider);
        PlatformLogin.LoginRetEvent += OnLoginResult;

        EEGamesLoginData _val = new EEGamesLoginData();
        _val.Account = PhoneNumber;
        _val.VerifyTyp = VerificationType.Phone;
        _val.VerifyCode = VerifyCode;
        _TryLogin(_val);
    }

    [ContextMenu("TryLoginEmail")]
    private void TryLoginEmail()
    {
        EEGamesSDKProvider m_sdkProvider = new EEGamesSDKProvider();
        Platform.Init(m_sdkProvider);
        PlatformLogin.LoginRetEvent += OnLoginResult;

        EEGamesLoginData _val = new EEGamesLoginData();
        _val.Account = EmailAddress;
        _val.VerifyTyp = VerificationType.Email;
        _val.VerifyCode = VerifyCode;
        _TryLogin(_val);
    }
    [ContextMenu("TryLogout")]
    private void TryLogout()
    {
        PlatformLogin.Logout();

    }
    // Start is called before the first frame update
    void Start()
    {
        //TestLogin();
    }
}
