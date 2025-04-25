using OpenNGS.Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformUser
{
    public static long GetUserID()
    {
        var m = Platform.GetUser();
        if (m == null) return -1;
        return m.GetUserID();
    }
    public static ulong GetAccountID()
    {
        var m = Platform.GetUser();
        if (m == null) return 0;
        return m.GetAccountID();
    }
}
