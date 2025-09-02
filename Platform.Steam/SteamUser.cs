using OpenNGS.Platform;
using Steamworks;
namespace OpenNGS.Platform.Steam
{
    public class SteamUser : IUserProvider
    {
        public PLATFORM_MODULE Module => PLATFORM_MODULE.USER;

        public ulong GetAccountID()
        {
            if(Platform.Initialized == true)
            {
                CSteamID _steamID = Steamworks.SteamUser.GetSteamID();
                return _steamID.m_SteamID;
            }
            return 0;
        }

        public long GetUserID()
        {
            return 0;
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
