using OpenNGS.Platform;

namespace OpenNGS.Platform.Steam
{
    public class SteamLogin : ILoginProvider
    {
        public PLATFORM_MODULE Module => PLATFORM_MODULE.LOGIN;

        public void AutoLogin()
        {

        }

        public PlatformLoginRet GetLoginRet()
        {
            return null;
        }

        public void Login(string channel, string permissions = "", string subChannel = "", string extraJson = "")
        {

        }

        public void Logout(string channel = "")
        {
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void SwitchUser(bool useLaunchUser)
        {
        }

        public void Update()
        {
        }
    }

}