using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Platform
{
    public interface ILoginProvider: IModuleProvider
    {
        void Login(string channel, string permissions = "", string subChannel = "", string extraJson = "");
        void SwitchUser(bool useLaunchUser);
        void Logout(string channel = "");
        PlatformLoginRet GetLoginRet();
        void AutoLogin();

    }
}
