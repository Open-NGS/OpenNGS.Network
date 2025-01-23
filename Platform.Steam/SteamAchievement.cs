#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define DISABLESTEAMWORKS
#endif

#if !DISABLESTEAMWORKS
using Steamworks;

using OpenNGS.Platform;
using UnityEngine;

public class SteamAchievement : IAchievementProvider
{
    public PLATFORM_MODULE Module => PLATFORM_MODULE.ACHIEVEMENT;

    public void Unlock(int id)
    {
        throw new System.NotSupportedException();
        Debug.Log("Trophy Unlocking");
    }

    public void UnlockProgress(int id, long value)
    {
        throw new System.NotSupportedException();
        Debug.Log("Progress Trophy Updating");
    }

    public void Unlock(string key)
    {
        if (!SteamAPI.Init())
        {
            return;
        }
        if (!SteamManager.Initialized)
        {
            Debug.LogError("SteamManager instance is not available.");
            return;
        }

        bool isAchieved;
        SteamUserStats.GetAchievement(key, out isAchieved);

        if (!isAchieved)
        {
            SteamUserStats.SetAchievement(key);
            SteamUserStats.StoreStats();
        }
    }

    public void UnlockProgress(string key, long value)
    {
        throw new System.NotSupportedException();
    }

    public void Start()
    {
        throw new System.NotImplementedException();
    }

    public void Stop()
    {
        throw new System.NotImplementedException();
    }

    public void ResetAllAchievements()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("SteamManager instance is not available.");
            return;
        }

        uint achievementCount = SteamUserStats.GetNumAchievements();
        for (uint i = 0; i < achievementCount; i++)
        {
            string achievementName = SteamUserStats.GetAchievementName(i);
            bool result = SteamUserStats.ClearAchievement(achievementName);
        }

        bool storeResult = SteamUserStats.StoreStats();
        Debug.Log("清除所有Steam成就");
    }
}

#endif