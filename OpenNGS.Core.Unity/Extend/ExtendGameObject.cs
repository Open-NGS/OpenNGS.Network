using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;



/// <summary>
/// GameObject  Extends
/// @fangyiliu(刘方毅)
/// </summary>
public static class ExtendGameObject
{
    internal static GameObject Singleton
    {
        get {
            if(sSingleton == null)
            {
                sSingleton = new GameObject("Singleton");
                GameObject.DontDestroyOnLoad(sSingleton);
            }
            return sSingleton;
        }
    }

    static GameObject sSingleton;
    static ExtendGameObject()
    {

    }

    public static void Destroy(this UnityEngine.Object obj)
    {
#if UNITY_EDITOR
        GameObject.DestroyImmediate(obj);
#else
        GameObject.Destroy(obj);
#endif
	}
    /// <summary>
    /// 递归搜索对象树里面为name的子对象，忽略层级 
    /// 兼容Unity的 Find("parent/cc/child")的方法
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <param name="needActive">是否是激活的对象</param>
    /// <returns>返回找到的第一个</returns>
    public static GameObject GetChild(this GameObject parent, string name, bool needActive = false)
    {
        var res = parent.transform.Find(name);
        if (res != null && (!needActive || res.gameObject.activeSelf))
        {
            return res.gameObject;
        }

        for (int i = 0; i < parent.transform.childCount; ++i)
        {
            var child = parent.transform.GetChild(i);
            if (needActive && !child.gameObject.activeSelf)
            {
                continue;
            }
            var ob = GetChild(child.gameObject, name, needActive);
            if (ob) return ob;
        }
        return null;
    }
    public static bool HasChild(this GameObject obj, GameObject child)
    {
        if (obj)
        {
            var myTrans = obj.transform;
            Transform parent = child.transform;
            while (parent)
            {
                if (parent == myTrans)
                {
                    return true;
                }
                parent = parent.parent;
            }
        }
        return false;
    }

    public static GameObject AddChildEx(this GameObject parent,GameObject child)
    {
        child.transform.parent = parent.transform;
        child.transform.localPosition = Vector3.zero;
        child.transform.localRotation = Quaternion.identity;
        child.transform.localScale = Vector3.one;
        child.layer = parent.layer;

        return child;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool ReplaceChild(this GameObject parent, string name, GameObject obj)
    {
        GameObject old = parent.GetChild(name);

        if (old == null)
        {
            obj.Destroy();
            return false;
        }
        obj.transform.parent = old.transform.parent;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        obj.name = old.name;
        obj.layer = old.layer;
        old.transform.parent = null;
        old.Destroy();

        return true;
    }

    public static bool ReplaceSelf(this GameObject old, GameObject obj)
    {
        if (old == null)
        {
            obj.Destroy();
            return false;
        }
        obj.transform.parent = old.transform.parent;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        obj.name = old.name;
        obj.layer = old.layer;

        old.transform.parent = null;
        old.Destroy();
        return true;
    }
    /// <summary>
    /// 获取组件 如果没有则添加
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameobject"></param>
    /// <returns></returns>
    public static T GetOrAddComponent<T>(this GameObject gameobject) where T : Component
    {
        if (gameobject == null)
        {
            return null;
        }
        T component = gameobject.GetComponent<T>();
        if (component == null)
        {
            component = gameobject.AddComponent<T>();
        }
        return component;
    }


    /// <summary>
    /// 获取组件 如果没有则添加
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameobject"></param>
    /// <returns></returns>
    public static T GetOrAddComponent<T>(this Transform trs) where T : Component
    {
        if (trs == null)
        {
            return null;
        }
        T component = trs.gameObject.GetComponent<T>();
        if (component == null)
        {
            component = trs.gameObject.AddComponent<T>();
        }
        return component;
    }
    /// <summary>
    /// 获取组件 如果没有则添加
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="behaviour"></param>
    /// <returns></returns>
    public static T GetOrAddComponent<T>(this MonoBehaviour behaviour) where T : Component
    {
        if (behaviour == null)
        {
            return null;
        }
        T component = behaviour.GetComponent<T>();
        if (component == null)
        {
            component = behaviour.gameObject.AddComponent<T>();
        }
        return component;
    }

    public static T GetOrAddComponentAsChildren<T>(this GameObject gameObject, string name = "", bool includeInactive = false) where T : Component
    {
        if (gameObject == null)
        {
            return null;
        }

        T comp = gameObject.GetComponentInChildren<T>(includeInactive);
        if (comp == null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).FullName;
            }

            GameObject child = new GameObject(name);
            comp = child.AddComponent<T>();
            child.transform.SetParent(gameObject.transform);
        }
        return comp;
    }

    /// <summary>
    /// 打印对象树
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static string PrintObjectTree(this GameObject go)
    {
        var c = go.transform.parent;
        string r = go.name;
        while (c != null)
        {
            r = c.name + "->" + r;
            c = c.parent;
        }
        return r;
    }


	public static void SetLayerRecursively(this GameObject go, int layer)
	{
		if (go != null)
		{
			go.layer = layer;

			for (int i = 0; i < go.transform.childCount; ++i)
			{
				go.transform.GetChild(i).gameObject.SetLayerRecursively(layer);
			}
		}
	}

    public static void SetLayerExceptFixedLayer(this GameObject go, int layer)
    {
        if (go != null && !go.CompareTag("FixedLayer"))
        {
            go.layer = layer;

            for (int i = 0; i < go.transform.childCount; ++i)
            {
                Transform tran = go.transform.GetChild(i);
                if (tran != null && tran.gameObject!=null)
                {
                    tran.gameObject.SetLayerExceptFixedLayer(layer);
                }
            }
        }
    }
    
    public static void SetLayerExceptIgnoreLayer(this GameObject go, int layer)
    {
        if (go != null && go.layer != LayerMask.NameToLayer("Ignore"))
        {
            go.layer = layer;

            for (int i = 0; i < go.transform.childCount; ++i)
            {
                Transform tran = go.transform.GetChild(i);
                if (tran != null && tran.gameObject != null)
                {
                    tran.gameObject.SetLayerExceptIgnoreLayer(layer);
                }
            }
        }
    }

    /// <summary>
    /// 获取节点下所有的子节点
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static GameObject[] GetChildren(this GameObject go)
    {
        if (go == null)
        {
            return null;
        }

        if (go.transform.childCount <= 0)
        {
            return null;
        }

        GameObject[] children = new GameObject[go.transform.childCount];
        for (int i = 0; i < go.transform.childCount; i++)
        {
            GameObject gameObject = go.transform.GetChild(i).gameObject;
            children[i] = gameObject;
        }
        return children;
    }

	#region Collider
	public static float GetColliderHeight(this GameObject gameobject)
    {
        float height = 0f;
        if (gameobject)
        {
            Collider collider = gameobject.GetComponent<Collider>();
            if (collider != null)
            {
                if (collider is BoxCollider)
                {
                    height = ((BoxCollider)(collider)).size.y;
                }
                if (collider is CapsuleCollider)
                {
                    height = ((CapsuleCollider)(collider)).height;
                }
            }

        }

        return height;
    }

    public static Vector3 GetColliderCenter(this GameObject gameobject)
    {
        Vector3 center = Vector3.zero;

        Collider collider = gameobject.GetComponent<Collider>();
        if (collider != null)
        {
            if (collider is BoxCollider)
            {
                center = ((BoxCollider)(collider)).center;
            }
            if (collider is CapsuleCollider)
            {
                center = ((CapsuleCollider)(collider)).center;
            }
        }

        return center;
    }
    #endregion

    #region 事件机制 高效!!
    class GameObjectEventHolder : MonoBehaviour
    {
        void OnDestroy()
        {
            gameObject._UnSubscribeAll();
        }
    }
    
    static Dictionary<GameObject, ExtendEvents.Publisher<string>> sGameObjectEventMap = new Dictionary<GameObject, ExtendEvents.Publisher<string>>();

    private static ExtendEvents.Publisher<string> _GetOrAddPublisher(this GameObject obj)
    {
        ExtendEvents.Publisher<string> publisher;
        if(!sGameObjectEventMap.TryGetValue(obj, out publisher))
        {
            publisher = new ExtendEvents.Publisher<string>();
            obj.GetOrAddComponent<GameObjectEventHolder>();
            sGameObjectEventMap.Add(obj, publisher);
            
        }
        return publisher;
    }

    internal static void _UnSubscribeAll(this GameObject obj)
    {
        sGameObjectEventMap.Remove(obj);
    }

    public static void UnSubscribeAll()
    {
        sGameObjectEventMap.Clear();
    }
    public static void Subscribe<T1, T2, T3, T4, T5, T6>(this GameObject obj, string name, Action<T1, T2, T3, T4, T5, T6> cb)
    {
        obj._GetOrAddPublisher().Subscribe(name, cb);
    }
    public static void UnSubscribe<T1, T2, T3, T4, T5, T6>(this  GameObject obj,string name, Action<T1, T2, T3, T4, T5, T6> cb)
    {
        obj._GetOrAddPublisher().UnSubscribe(name, cb);
    }

    public static void Notify<T1, T2, T3, T4, T5, T6>(this GameObject obj,string name, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6)
    {
        obj._GetOrAddPublisher().Notify(name, a1, a2, a3, a4, a5, a6);
    }
    public static void Subscribe<T1, T2, T3, T4, T5>(this GameObject obj, string name, Action<T1, T2, T3, T4, T5> cb)
    {
        obj._GetOrAddPublisher().Subscribe(name, cb);
    }
    public static void UnSubscribe<T1, T2, T3, T4, T5>(this GameObject obj, string name, Action<T1, T2, T3, T4, T5> cb)
    {
        obj._GetOrAddPublisher().UnSubscribe(name, cb);
    }

    public static void Notify<T1, T2, T3, T4, T5>(this GameObject obj, string name, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5)
    {
        obj._GetOrAddPublisher().Notify(name, a1, a2, a3, a4, a5);
    }
    public static void Subscribe<T1, T2, T3, T4>(this GameObject obj, string name, Action<T1, T2, T3, T4> cb)
    {
        obj._GetOrAddPublisher().Subscribe(name, cb);
    }
    public static void UnSubscribe<T1, T2, T3, T4>(this GameObject obj, string name, Action<T1, T2, T3, T4> cb)
    {
        obj._GetOrAddPublisher().UnSubscribe(name, cb);
    }

    public static void Notify<T1, T2, T3, T4>(this GameObject obj, string name, T1 a1, T2 a2, T3 a3, T4 a4)
    {
        obj._GetOrAddPublisher().Notify(name, a1, a2, a3, a4);
    }
    public static void Subscribe<T1, T2, T3>(this GameObject obj, string name, Action<T1, T2, T3> cb)
    {
        obj._GetOrAddPublisher().Subscribe(name, cb);
    }
    public static void UnSubscribe<T1, T2, T3>(this GameObject obj, string name, Action<T1, T2, T3> cb)
    {
        obj._GetOrAddPublisher().UnSubscribe(name, cb);
    }

    public static void Notify<T1, T2, T3>(this GameObject obj, string name, T1 a1, T2 a2, T3 a3)
    {
        obj._GetOrAddPublisher().Notify(name, a1, a2, a3);
    }

    public static void Subscribe<T1, T2>(this GameObject obj, string name, Action<T1, T2> cb)
    {
        obj._GetOrAddPublisher().Subscribe(name, cb);
    }
    public static void UnSubscribe<T1, T2>(this GameObject obj, string name, Action<T1, T2> cb)
    {
        obj._GetOrAddPublisher().UnSubscribe(name, cb);
    }

    public static void Notify<T1, T2>(this GameObject obj, string name, T1 a1, T2 a2)
    {
        obj._GetOrAddPublisher().Notify(name, a1, a2);
    }
    public static void Subscribe<T1>(this GameObject obj, string name, Action<T1> cb)
    {
        obj._GetOrAddPublisher().Subscribe(name, cb);
    }
    public static void UnSubscribe<T1>(this GameObject obj, string name, Action<T1> cb)
    {
        obj._GetOrAddPublisher().UnSubscribe(name, cb);
    }

    public static void Notify<T1>(this GameObject obj, string name, T1 a1)
    {
        obj._GetOrAddPublisher().Notify(name, a1);
    }
    public static void Subscribe(this GameObject obj, string name, Action cb)
    {
        obj._GetOrAddPublisher().Subscribe(name, cb);
    }
    public static void UnSubscribe(this GameObject obj, string name, Action cb)
    {
        obj._GetOrAddPublisher().UnSubscribe(name, cb);
    }

    public static void Notify(this GameObject obj, string name)
    {
        obj._GetOrAddPublisher().Notify(name);
    }
    #endregion //事件机制 高效！！
}
