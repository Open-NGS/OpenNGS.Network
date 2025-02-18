using Newtonsoft.Json;
using OpenNGS.Platform;
using OpenNGS.SDK.Core.Initiallization;
using OpenNGS.SDK.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using OpenNGS.SDK.Auth;
using OpenNGS.SDK.Auth.Models;
using OpenNGS.SDK.Log;
using System.Net.Mail;
using static UnityEngine.Networking.UnityWebRequest;

namespace OpenNGS.Platform.EEGames
{
    public class EEGamesLoginData
    {
        public string UserName;
        public string Password;
        public string Account;
        public string VerifyCode;
        public VerificationType VerifyTyp;
        public bool RequestVerifyCode;
        public EEGamesLoginData()
        {

        }
    }
    public class EEGamesSDKProvider : ISDKProvider
    {
        private EEGamesLoginProvider m_loginProvider = null;
        public IModuleProvider CreateProvider(PLATFORM_MODULE module)
        {
            if (module == PLATFORM_MODULE.LOGIN)
            {
                TextAsset textAsset = Resources.Load<TextAsset>("EEGames");

                string appid = "";
                string secret = "";
                if (textAsset != null)
                {
                    // 将文本内容按行分割
                    string[] lines = textAsset.text.Split('\n');

                    // 创建一个字典来存储键值对
                    Dictionary<string, string> configDict = new Dictionary<string, string>();

                    foreach (string line in lines)
                    {
                        // 按等号分割每行
                        string[] parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            // 去除空白字符并添加到字典中
                            configDict[parts[0].Trim()] = parts[1].Trim();
                        }
                    }

                    // 从字典中获取appid和secret
                    if (configDict.ContainsKey("appid"))
                    {
                        appid = configDict["appid"];
                    }

                    if (configDict.ContainsKey("secret"))
                    {
                        secret = configDict["secret"];
                    }

                    // 打印读取到的值
                    Debug.Log("AppID: " + appid);
                    Debug.Log("Secret: " + secret);
                }
                else
                {
                    Debug.LogError("Config file not found!");
                }

                m_loginProvider = new EEGamesLoginProvider();
                m_loginProvider.InitLoginProvider(appid, secret);
                return m_loginProvider;
            }
            else if (module == PLATFORM_MODULE.REPORT)
            {
                return new EEGamesReportProvider();
            }
            else if (module == PLATFORM_MODULE.REPORT)
            {
                return new EEGamesReportProvider();
            }
            else if(module == PLATFORM_MODULE.NOTICE)
            {
                return new EEGamesNoticeProvider();
            }
            else
            {
                Debug.LogError("Not supported provider " + module);
            }
            return null;
        }

        public bool Initialize()
        {
            return true;
        }

        public void Terminate()
        {
        }

        public void Update()
        {
        }
    }
    public class EEGamesCallBack : IThirdpartyCallBack
    {
        public void OnCallBack(string moduleName, string funcName, string result)
        {
        }

        public void OnException(int code, string msg)
        {
        }
    }
    public class EEGamesLoginProvider : ILoginProvider
    {
        public PLATFORM_MODULE Module => PLATFORM_MODULE.LOGIN;

        PLATFORM_MODULE IModuleProvider.Module => throw new NotImplementedException();

        private EEGamesCallBack m_callBack;
        private PlatformLoginRet m_LoginResult = null;
        private InitializationOptions m_initOption;
        public EEGamesLoginProvider()
        {
            m_LoginResult = new PlatformLoginRet();
            m_callBack = new EEGamesCallBack();
            PlatformCallback.Instance.Init(m_callBack);
        }
        public void InitLoginProvider(string strAppId, string AppSecret)
        {
            m_initOption = new InitializationOptions();
            m_initOption.AppId = strAppId;
            m_initOption.AppSecret = AppSecret;
            //OpenNGSPlatformServices.Initialize(new InitializationOptions()
            //{
            //    //AppId = "iboN4V3anKRnsKwgudonW0ESxGwJLNUz2rhN",
            //    //AppSecret = "YDwqSQ5Be0oGIpQJ6sPBlHLHveRTfC5p"
            //    AppId = strAppId,
            //    AppSecret = AppSecret
            //}, new SDKLogger());
            SDKLogger _log = new SDKLogger();
            OpenNGSPlatformServices.Initialize(m_initOption, _log);
            // todo这个要改回上面的
            //OpenNGSPlatformServices.Initialize(m_initOption);
        }
        public void AutoLogin()
        {
            AuthcationService.Instance.AutoLoginCallback += (result) =>
            {
                if (result)
                {
                    //Debug.Log($"已自动登录 {AuthcationService.Instance.User.Nickname}");
                    m_LoginResult.MethodNameId = (int)MSDKMethodNameID.MSDK_LOGIN_AUTOLOGIN;
                    m_LoginResult.Token = AuthcationService.Instance.Token;
                    m_LoginResult.RetCode = (int)SDKResultCode.RESULT_OK;
                    m_LoginResult.UserName = AuthcationService.Instance.User.Nickname;
                    _callBackLogin(m_LoginResult);
                }
                else
                {
                    Debug.Log("未登录，正在登陆....");
                    //AuthcationService.Instance.LoginByUsernamePassword(UserName, Password);
                }
            };

            AuthcationService.Instance.AutoLogin();
        }

        public PlatformLoginRet GetLoginRet()
        {
            return m_LoginResult;
        }

        private void _callBackLogin(PlatformLoginRet _ret)
        {
            PlatformCallback.Instance.OnCallBack(_ret);
        }

        private void _getVerifyCode(int nResultCode)
        {
            SDKLog.Info($"[RequestVerifyCodeEmail] code:[{nResultCode}]");
            m_LoginResult.MethodNameId = (int)MSDKMethodNameID.MSDK_ACCOUNT_VERIFY_CODE;
        }
        public void Login(string channel, string permissions = "", string subChannel = "", string extraJson = "")
        {
            EEGamesLoginData _loginData = JsonConvert.DeserializeObject<EEGamesLoginData>(extraJson);
            if (string.IsNullOrEmpty(_loginData.VerifyCode) == true)
            {
                // 请求验证码
                if (_loginData.RequestVerifyCode == true)
                {
                    if (_loginData.VerifyTyp == VerificationType.Phone)
                    {
                        AuthcationService.Instance.VerificationCodeCallback += (result) =>
                        {
                            _getVerifyCode(result);
                        };
                        AuthcationService.Instance.SendVerificationCode(VerificationType.Phone, _loginData.Account);
                    }
                    else if (_loginData.VerifyTyp == VerificationType.Email)
                    {
                        AuthcationService.Instance.VerificationCodeCallback += (result) =>
                        {
                            _getVerifyCode(result);
                        };
                        AuthcationService.Instance.SendVerificationCode(VerificationType.Email, _loginData.Account);
                    }
                }
                else
                {
                    AuthcationService.Instance.LoginCallback += (result) =>
                    {
                        SDKLog.Info($"[账号密码登录成功] 用户:[{result.Nickname}]");
                        m_LoginResult.MethodNameId = (int)MSDKMethodNameID.MSDK_LOGIN_LOGIN;
                        m_LoginResult.Token = AuthcationService.Instance.Token;
                        m_LoginResult.UserName = result.Nickname;
                        m_LoginResult.RetCode = (int)SDKResultCode.RESULT_OK;
                        _callBackLogin(m_LoginResult);
                    };

                    AuthcationService.Instance.LoginResultCallback += (result) =>
                    {
                        if (result != (int)SDKResultCode.RESULT_OK)
                        {
                            m_LoginResult.MethodNameId = (int)MSDKMethodNameID.MSDK_LOGIN_LOGIN;
                            m_LoginResult.RetCode = result;
                            _callBackLogin(m_LoginResult);
                        }
                    };

                    AuthcationService.Instance.LoginByUsernamePassword(_loginData.UserName, _loginData.Password);
                }
            }
            else
            {
                if (_loginData.VerifyTyp == VerificationType.Phone || _loginData.VerifyTyp == VerificationType.Email)
                {
                    AuthcationService.Instance.LoginCallback += (result) =>
                    {
                        SDKLog.Info($"[LoginCallback] 用户[{result.Nickname}] 登录成功 code:[{result}]");

                        m_LoginResult.MethodNameId = (int)MSDKMethodNameID.MSDK_ACCOUNT_LOGIN_WITH_CODE;
                        m_LoginResult.Token = AuthcationService.Instance.Token;
                        m_LoginResult.UserName = result.Nickname;
                        m_LoginResult.RetCode = (int)SDKResultCode.RESULT_OK;
                        if (AuthcationService.Instance.User.NewUser == true)
                        {
                            m_LoginResult.FirstLogin = 1;
                        }
                        _callBackLogin(m_LoginResult);
                    };

                    AuthcationService.Instance.LoginResultCallback += (result) =>
                    {
                        if (result != 0)
                        {
                            SDKLog.Info($"[LoginCallback] 登录错误 code:[{result}]");

                            m_LoginResult.MethodNameId = (int)MSDKMethodNameID.MSDK_ACCOUNT_LOGIN_WITH_CODE;
                            m_LoginResult.RetCode = result;
                            _callBackLogin(m_LoginResult);
                        }
                    };

                    AuthcationService.Instance.LoginOrRegisterByVerificationCode(
                        _loginData.VerifyTyp,
                        _loginData.Account,
                        _loginData.VerifyCode);
                }
            }
        }

        public void Logout(string channel = "")
        {
            AuthcationService.Instance.LogoutCallback += (result) =>
            {
                m_LoginResult.MethodNameId = (int)MSDKMethodNameID.MSDK_LOGIN_LOGOUT;
                m_LoginResult.RetCode = result;
                _callBackLogin(m_LoginResult);
            };
            AuthcationService.Instance.Logout();
        }

        public void SwitchUser(bool useLaunchUser)
        {
        }

        void ILoginProvider.AutoLogin()
        {
            //throw new NotImplementedException();
        }

        void IModuleProvider.Start()
        {
            //throw new NotImplementedException();
        }

        void IModuleProvider.Stop()
        {
            //throw new NotImplementedException();
        }

        void IModuleProvider.Update()
        {
            //throw new NotImplementedException();
        }
    }

    public class SDKLogger : OpenNGS.SDK.Log.ILogger
    {
        public void Log(object message)
        {
            Debug.Log(message);
        }

        public void LogFormat(string message, params object[] args)
        {
            Debug.LogFormat(message, args);
        }

        public void Error(object message)
        {
            Debug.LogError(message);
        }

        public void ErrorFormat(string message, params object[] args)
        {
            Debug.LogErrorFormat(message, args);
        }

        public void Warning(object message)
        {
            Debug.LogWarning(message);
        }

        public void WarningFormat(string message, params object[] args)
        {
            Debug.LogWarningFormat(message, args);
        }
    }

}

