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
        throw new System.NotImplementedException();
    }

    public void Initialize()
    {
        if (!SteamAPI.Init())
        {
            return;
        }
    }
}
#endif