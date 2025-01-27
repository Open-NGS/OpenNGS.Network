#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define DISABLESTEAMWORKS
#endif

#if !DISABLESTEAMWORKS
using Steamworks;

using OpenNGS.Platform;
using UnityEngine;

public class SteamRemoteStorage : IRemoteStorageProvider
{
    public PLATFORM_MODULE Module => PLATFORM_MODULE.REMOTE_STORAGE;

    public bool FileDelete(string fileName)
    {
        throw new System.NotImplementedException();
    }

    public int GetFileCount()
    {
        throw new System.NotImplementedException();
    }

    public string GetFileNameAndSize(int i, out object _)
    {
        throw new System.NotImplementedException();
    }

    public bool IsEnabledForApp()
    {
        throw new System.NotImplementedException();
    }

    public void Start()
    {
        throw new System.NotImplementedException();
    }

    public void Stop()
    {
        throw new System.NotImplementedException();
    }
}

#endif