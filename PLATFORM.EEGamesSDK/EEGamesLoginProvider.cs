using OpenNGS.Platform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.Platform.EEGames
{
    public class EEGamesSDKProvider : ISDKProvider
    {
        public IModuleProvider CreateProvider(PLATFORM_MODULE module)
        {
            if(module == PLATFORM_MODULE.LOGIN)
            {
                return new EEGamesLoginProvider();
            }
            return null;
        }
    }
    public class EEGamesLoginProvider : ILoginProvider
    {
        public PLATFORM_MODULE Module => PLATFORM_MODULE.LOGIN;

        public void AutoLogin()
        {
            throw new System.NotImplementedException();
        }

        public PlatformLoginRet GetLoginRet()
        {
            throw new System.NotImplementedException();
        }

        public void Login(string channel, string permissions = "", string subChannel = "", string extraJson = "")
        {
            //throw new System.NotImplementedException();
        }

        public void Logout(string channel = "")
        {
            throw new System.NotImplementedException();
        }

        public void SwitchUser(bool useLaunchUser)
        {
            throw new System.NotImplementedException();
        }
    }
}

