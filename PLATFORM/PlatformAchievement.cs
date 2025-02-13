using OpenNGS.Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAchievement
{
    public static void Unlock(int id)
    {
#if DEBUG
        Debug.LogFormat("PlatformAchievement.Unlock:{0}", id);
#endif
        Platform.GetAchievement().Unlock(id);
    }

    public static void Unlock(string key)
    {
#if DEBUG
        Debug.LogFormat("PlatformAchievement.Unlock:{0}", key);
#endif
        Platform.GetAchievement().Unlock(key);
    }

    public static void ResetAllAchievements()
    {
#if DEBUG
        Debug.Log("PlatformAchievement.ResetAllAchievements");
#endif
        Platform.GetAchievement().ResetAllAchievements();
    }

}
