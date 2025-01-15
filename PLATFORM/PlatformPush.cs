using OpenNGS.Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPush
{
    
    public static void RegisterPush()
    {
        Platform.GetPush().RegisterPush();
    }
   
    public static void SetAccount(string accountId)
    {
        Platform.GetPush().SetAccount(accountId);
    }

    public static void DeleteAccount(string accountId)
    {
        Platform.GetPush().DeleteAccount(accountId);
    }

}
