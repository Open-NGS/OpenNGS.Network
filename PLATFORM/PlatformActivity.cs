using OpenNGS.Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformActivity
{
    public static void ActivityStart(string actitityId)
    {
#if DEBUG
        Debug.LogFormat("ActivityStart.actitityId:{0}", actitityId);
#endif
#if !UNITY_EDITOR
        var m = Platform.GetActivity();
        if (m != null) m.ActivityStart(actitityId);
#endif
    }

    public static void ActivityEnd(string actitityId, string outcome)
    {
#if DEBUG
        Debug.LogFormat("ActivityEnd.actitityId:{0}", actitityId);
#endif
#if !UNITY_EDITOR
        var m = Platform.GetActivity();
        if (m != null) m.ActivityEnd(actitityId, outcome);
#endif
    }


}
