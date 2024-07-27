using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NotificationCenter Class like iOS NSNotificationCenter 
/// </summary>

public class NotificationCenter : NotificationObject<object, string>
{
    private static NotificationCenter _default;
    public static NotificationCenter Default => _default ??= new NotificationCenter();

    public new void PostNotification(string notification, params object[] args)
    {
        base.PostNotification(notification, args);
    }
}
