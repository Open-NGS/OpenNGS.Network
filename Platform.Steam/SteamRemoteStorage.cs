#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define DISABLESTEAMWORKS
#endif

#if !DISABLESTEAMWORKS
using Steamworks;

using OpenNGS.Platform;
using UnityEngine;
namespace OpenNGS.Platform.Steam
{
    public class SteamRemoteStorage : IRemoteStorageProvider
    {
        public PLATFORM_MODULE Module => PLATFORM_MODULE.REMOTE_STORAGE;

        public bool FileDelete(string fileName)
        {
            return Steamworks.SteamRemoteStorage.FileDelete(fileName);
        }

        public int FileRead(string fileName, byte[] fileData, int fileSize)
        {
            return Steamworks.SteamRemoteStorage.FileRead(fileName, fileData, fileSize);
        }

        public bool FileWrite(string saveFileName, byte[] fileData, int length)
        {
            return Steamworks.SteamRemoteStorage.FileWrite(saveFileName, fileData, length);
        }

        public int GetFileCount()
        {
            return Steamworks.SteamRemoteStorage.GetFileCount();
        }

        public string GetFileNameAndSize(int i, out object _obj)
        {
            _obj = null;
            return Steamworks.SteamRemoteStorage.GetFileNameAndSize(i, out _);
        }

        public int GetFileSize(string fileName)
        {
            return Steamworks.SteamRemoteStorage.GetFileSize(fileName);
        }

        public bool IsEnabledForApp()
        {
            return Steamworks.SteamRemoteStorage.IsCloudEnabledForApp();
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

#endif