using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenNGS.Platform.GCloud
{
    public class GCloudModuleUsers : IUsersModule
    {
        public PlatformUser PlatformUser { get; private set; }

        NRequest<PlatformUser> accountRequest = new NRequest<PlatformUser>(0);

        public Platform_PLATFORM_MODULE Module => Platform_PLATFORM_MODULE.Users;

        public GCloudModuleUsers()
        {
            var msdl = PlatformCallback.Instance;
            PlatformLogin.LoginRetEvent += PlatformLogin_LoginRet;
        }


        void AccountLogined(PlatformLoginRet loginRet)
        {
            if (loginRet.RetCode == PlatformError.SUCCESS)
            {
                Debug.LogFormat("GCloudModuleUsers::PlatformLoginRet:[{0}] Msg;{1}", loginRet.RetCode, loginRet.RetMsg);
                Debug.Log("loginRet.OpenId:" + loginRet.OpenId);
                Debug.Log("loginRet.UserName:" + loginRet.UserName);
                Debug.Log("loginRet.PictureUrl:" + loginRet.PictureUrl);
                Debug.Log("loginRet.Token:" + loginRet.Token);
                Debug.Log("loginRet.Channel:" + loginRet.Channel);

                AccountLogined(loginRet.OpenId, loginRet.UserName, loginRet.PictureUrl, Platform_PLAT_RESULT.Success);
            }
            else
            {
                Debug.LogErrorFormat("GCloudModuleUsers::[MSDKError]Platform: {0}, OpenID: {1}, MSDKLoginFail: {2}:{3}", loginRet.Channel, loginRet.OpenId, loginRet.RetCode, loginRet.RetMsg);
                AccountLogined(loginRet.OpenId, loginRet.UserName, loginRet.PictureUrl, (Platform_PLAT_RESULT)loginRet.RetCode, loginRet.RetMsg);
            }
        }
        void AccountLogined(string openid,string username,string imageUrl, Platform_PLAT_RESULT result,string message = "")
        {
            this.PlatformUser.ID = openid;
            this.PlatformUser.ImageURL = imageUrl;
            this.PlatformUser.DisplayName = username;
            this.PlatformUser.Result = result;
            this.PlatformUser.Message = message;
            if (accountRequest != null)
                accountRequest.HandleMessage(PlatformUser);
        }

        private void PlatformLogin_LoginRet(PlatformLoginRet loginRet)
        {
            Debug.Log("PlatformLogin_LoginRetEvent : " + loginRet.ToJsonString());
            if(this.PlatformUser == null)
                this.PlatformUser = new PlatformUser();
            var _platform = loginRet.Channel;
            if (loginRet.MethodNameId == (int)MSDKMethodNameID.MSDK_LOGIN_LOGIN)
            {
                    AccountLogined(loginRet);
            }
            else if (loginRet.MethodNameId == (int)MSDKMethodNameID.MSDK_LOGIN_BIND)
            {
                Debug.Log("MSDK_LOGIN_BIND");
            }
            else if (loginRet.MethodNameId == (int)MSDKMethodNameID.MSDK_LOGIN_AUTOLOGIN)
            {
                if (loginRet.RetCode == PlatformError.SUCCESS)
                {
                    Debug.Log("MSDK_LOGIN_AUTOLOGIN_SUCCESS");
                    AccountLogined(loginRet);
                }
                else
                {
                    Debug.LogErrorFormat("[MSDKError]Platform: {0}, OpenID: {1}, MSDKAutoLoginFail: {2}", _platform, loginRet.OpenId, loginRet.RetCode);
                    PlatformLogin.Logout();
                }
            }
            else if (loginRet.MethodNameId == (int)MSDKMethodNameID.MSDK_LOGIN_QUERYUSERINFO)
            {
                if (loginRet.RetCode == PlatformError.SUCCESS)
                {
                    Debug.Log("MSDK_LOGIN_LOGIN_SUCCESS");
                    PlatformUser.DisplayName = loginRet.UserName;
                }
                else
                {
                    Debug.Log("MSDKLoginFail:" + loginRet.RetCode);
                }
            }
            else if (loginRet.MethodNameId == (int)MSDKMethodNameID.MSDK_LOGIN_LOGINWITHCONFIRMCODE)
            {
                Debug.Log("MSDK_LOGIN_LOGINWITHCONFIRMCODE");
            }
            accountRequest.HandleMessage(PlatformUser);
        }

        public NRequest<PlatformUser> GetLoggedInUser()
        {
            return accountRequest;
        }

        public NRequest<PlatformUser> GetUserInfo(ulong id)
        {
            throw new NotImplementedException();
        }

        public NRequest<PlatformUser> GetUserTicket()
        {
            throw new NotImplementedException();
        }
    }
}
