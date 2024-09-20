using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public interface ISingleton
{
    bool delayDestroy { get; set; }
    void Destroy();
}

public sealed class Singleton
{
    public static List<ISingleton> singletons = new List<ISingleton>();

    public static void AddSingleton(ISingleton singleton)
    {
        if (singletons.IndexOf(singleton) == -1)
            singletons.Add(singleton);
    }

    public static void Destroy(bool delay = false)
    {
        //string debugstr = "";
        //foreach(ISingleton singleton in singletons) {
        //    debugstr += singleton.ToString() + ", ";
        //    if(singleton.delayDestroy == delay) {
        //        singleton.Destory();
        //    }
        //}

        string debugstr = "";
        int count = singletons.Count;
        for (int index = count - 1; index >= 0; index--)
        {
            debugstr += singletons[index].ToString() + ", ";
            if (singletons[index].delayDestroy == delay)
            {
                singletons[index].Destroy();
                singletons.RemoveAt(index);
            }
        }
          
        Debug.Log("Destory Singleton list is: " + debugstr);
    }
}

public abstract class Singleton<T> : ISingleton where T : ISingleton, new()
{
    public bool delayDestroy { get; set; }
    private static T instance;
    public static T Instance
    {
        get
        {
            if (Equals(instance, default(T)))
            {
                instance = new T();
                Singleton.AddSingleton(instance);
            }
            return instance;
        }
    }

    public virtual void Destroy()
    {
        instance = default(T);
    }
}


public abstract class MonoBehaviourSingletonBase :MonoBehaviour
{
    
}

/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// 
/// As a note, this is made as MonoBehaviour because we need Coroutines.
/// </summary>
public abstract class BehaviourSingleton<T> : MonoBehaviourSingletonBase, ISingleton where T : MonoBehaviour , ISingleton
{
    public bool delayDestroy { get; set; }
    private static T _instance;
    public static bool NeedDestroyOnRestart = false ;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null || _instance.gameObject == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[Singleton] Something went really wrong " +
                                       " - there should never be more than 1 singleton! [" + typeof(T).ToString() +
                                       "] Reopenning the scene might fix it." + "\n");
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();
                        DontDestroyOnLoad(_instance);
                        //Debug.Log("[Singleton] An instance of " + typeof (T).ToString() + " is needed in the scene, so '" + singleton.name + "' was created with DontDestroyOnLoad.\n");
                    }
                    else
                    {
                        Debug.Log("[Singleton] Using instance already created: " + _instance.gameObject.name + "\n");

                    }
                    Singleton.AddSingleton(_instance);
                }
                return _instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;
    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    public virtual void OnDestroy()
    {
        applicationIsQuitting = true;
    }

    /**
     * used when restart game
     */
    public virtual void Destroy()
    {
        if (_instance != null)
        {
            Debug.Log("BehaviourSingleton.DestorySingleton Instance: " + _instance.GetType());
            if (NeedDestroyOnRestart)
            {
                if (_instance.gameObject != null)
                {
                    Debug.Log("BehaviourSingleton.DestorySingleton destroy gameobject Instance: " + _instance.GetType());
                    DestroyImmediate(_instance.gameObject);
                }
               
            }
        }
        _instance = null;
    }
}

public abstract class RefSingleton<T> : MonoBehaviour, ISingleton where T : MonoBehaviour, ISingleton
{
    private static T _instance;

    private static object _lock = new object();

    public bool delayDestroy { get; set; }

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null || _instance.gameObject == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[Singleton] Something went really wrong " +
                            " - there should never be more than 1 singleton!" +
                                       " Reopenning the scene might fix it." + "\n");
                        return _instance;
                    }

                    if (_instance != null)
                    {
                        // -- Add to stack for destory when game restart
                        Singleton.AddSingleton(_instance);

                        Debug.Log("[Singleton] Using instance already created: " +
                                  _instance.gameObject.name + "\n");
                    }
                }

                return _instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;
    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    public void OnDestroy()
    {
        applicationIsQuitting = true;
    }

    /**
     * used when restart game
     */
    public virtual void Destroy()
    {
//        if (_instance != null)
//        {
//            Debug.Log("RefSingleton.DestorySingleton Instance: " + _instance.GetType());
//        }
        
        _instance = null;
    }
}
