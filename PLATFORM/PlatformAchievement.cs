using OpenNGS.Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAchievement
{
    public static void Unlock(int id)
    {
        Platform.GetAchievement().Unlock(id);
    }

    public static void Unlock(string key)
    {
        Platform.GetAchievement().Unlock(key);
    }

    public static void ResetAllAchievements()
    {
        Platform.GetAchievement().ResetAllAchievements();
    }

}
