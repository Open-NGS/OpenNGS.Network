using OpenNGS.Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformUser
{
    public static long GetUserID()
    {
        return Platform.GetUser().GetUserID();
    }
}
