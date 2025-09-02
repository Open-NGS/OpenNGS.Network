using Steamworks;
namespace OpenNGS.Platform.Steam
{
    public class SteamApp : IAppProvider
    {
        public bool IsDebug { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public PLATFORM_MODULE Module => PLATFORM_MODULE.APP;

        public bool IsAppInstalled(string appStr)
        {
            if(uint.TryParse(appStr, out uint nAppID))
            {
                return SteamApps.BIsAppInstalled((AppId_t)nAppID);
            }
            else
            {
                return false;
            }
        }

        public bool IsAppSubscribed(string appId)
        {
            if (uint.TryParse(appId, out uint nAppID))
            {
                return SteamApps.BIsSubscribedApp((AppId_t)nAppID);
            }
            else
            {
                return false;
            }
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Update()
        {
        }
    }
}