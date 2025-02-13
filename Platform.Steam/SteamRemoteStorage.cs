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
        return false;
    }

    public int FileRead(string fileName, byte[] fileData, int fileSize)
    {
        return 0;
    }

    public bool FileWrite(string saveFileName, byte[] fileData, int length)
    {
        return false;
    }

    public int GetFileCount()
    {
        return 0;
    }

    public string GetFileNameAndSize(int i, out object _obj)
    {
        _obj = null;
        return string.Empty;
    }

    public int GetFileSize(string fileName)
    {
        return 0;
    }

    public bool IsEnabledForApp()
    {
        return false;
    }

    public void Start()
    {
        throw new System.NotImplementedException();
    }

    public void Stop()
    {
    }

    public void Update()
    {
    }
}

#endif