#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define DISABLESTEAMWORKS
#endif

#if !DISABLESTEAMWORKS
using Steamworks;

using OpenNGS.Platform;

public class SteamProvider : OpenNGS.Platform.ISDKProvider
{
    public IModuleProvider CreateProvider(PLATFORM_MODULE module)
    {
        switch (module)
        {
            case PLATFORM_MODULE.LOGIN: return new SteamLogin();
            case PLATFORM_MODULE.ACHIEVEMENT: return new SteamAchievement();
            case PLATFORM_MODULE.REMOTE_STORAGE: return new SteamRemoteStorage();
            default: return null;
        }
    }

    public bool Initialize()
    {
        if (!SteamAPI.Init())
        {
            return false;
        }
        return true;
    }

    public void Terminate()
    {
       
    }

    public void Update()
    {
    }
}
#endif