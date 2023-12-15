using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

/// <summary>
/// Model class inherit this class can send change Notification for it's observers
/// </summary>
/// <typeparam name="T1"></typeparam>
/// observer type can be view model class type
/// <typeparam name="T2"></typeparam>
/// notification type can be enum or string
public class NotificationObject<T1, T2> where T1 : class
{
    private readonly Hashtable m_Notifications;
    protected NotificationObject()
    {
        m_Notifications = new Hashtable();
    }
    
    public delegate void NotificationHandler0();

    public delegate void NotificationHandlerN(params object[] args);
    
    public void AddObserver(T1 observer, T2 notification, NotificationHandler0 handler)
    {
        AddObserverFunc(observer, notification, handler);
    }

    public void AddObserver(T1 observer, T2 notification, NotificationHandlerN handler)
    {
        AddObserverFunc(observer, notification, handler);
    }

    private void AddObserverFunc(T1 observer, T2 notification, object handler)
    {
        if (observer == null)
        {
            Debug.Log("Null observer specified for notification in AddObserver.");
            return;
        }

        if (m_Notifications[notification] == null)
        {
            m_Notifications[notification] = new Dictionary<T1, object>();
        }

        var handlers = (Dictionary<T1, object>)m_Notifications[notification];
        if (!handlers.ContainsKey(observer))
        {
            handlers.Add(observer, handler);
        }
    }

    private readonly List<T2> m_TmpKeys = new List<T2>();
    
    public void RemoveObserver(T1 observer)
    {
        foreach (DictionaryEntry kv in m_Notifications)
        {
            var key = (T2)kv.Key;
            var v = (Dictionary<T1, object>)kv.Value;
            if (v.ContainsKey(observer))
            {
                m_TmpKeys.Add(key);
            }
        }

        foreach(var key in m_TmpKeys)
        {
            RemoveObserver(observer, key);
        }
        m_TmpKeys.Clear();
    }
    private void RemoveObserver(T1 observer, T2 notification)
    {
        if (m_Notifications[notification] == null)
        {
            Debug.LogWarning("No need to remove notification not exist");
            return;
        }

        var handlers = (Dictionary<T1, object>)m_Notifications[notification];

        if (handlers != null)
        {
            if (handlers.ContainsKey(observer))
            {
                handlers.Remove(observer);
            }
        }

        if (handlers.Count == 0)
        {
            m_Notifications.Remove(notification);
        }
    }

    private readonly List<T1> m_ObserversToRemove = new List<T1>();

    protected void PostNotification(T2 notification, params object[] args)
    {
        var handlers = (Dictionary<T1, object>)m_Notifications[notification];

        m_ObserversToRemove.Clear();

        if (handlers == null)
        {
            return;
        }
        
        foreach (var handler in handlers)
        {
            if (handler.Key != null)
            {
                if (handler.Value != null)
                {
                    if (handler.Value is NotificationHandler0)
                    {
                        (handler.Value as NotificationHandler0)?.Invoke();

                        if (args.Length != 0)
                        {
                            Debug.LogWarning($"handler '{handler.Value}' receive useless parameter. notification: {notification}");
                        }
                    }
                    else
                    {
                        (handler.Value as NotificationHandlerN)?.Invoke(args);
                    }
                }
                else
                {
                    Debug.LogError($"Ops! Receive notification '{notification}', but handler has been destroyed ");
                }
            }
            else
            {
                Debug.LogError($"Ops! Receive notification '{notification}', but observer has been destroyed ");
                m_ObserversToRemove.Add(handler.Key);
            }
        }

        if (m_ObserversToRemove.Count > 0)
        {
            foreach (var o in m_ObserversToRemove)
            {
                if (handlers != null) 
                    handlers.Remove(o);
            }
        }
    }
}