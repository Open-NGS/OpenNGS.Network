using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public delegate void Action<in T1, in T2, in T3, in T4, in T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

/// <summary>
/// 事件机制
/// fangyiliu(刘方毅)
/// </summary>
public static class ExtendEvents
{

    /// <summary>
    /// Subscribe-Publish model implements from SendMessage
    /// 仿Unity　SendMessage 实现 NGUI内部用的事件机制 就是这种方式
    /// </summary>
    public class PublisherSendMessage
    {
        public ForeachMutableList<System.Object> Subscribes = new ForeachMutableList<object>();

        public void Subscribe(object subscribe)
        {
            Subscribes.Add(subscribe);
        }

        public void UnSubscribe(object subscribe)
        {
            Subscribes.Remove(subscribe);
        }

        public void Notify(string name, params object[] args)
        {
            SendMessage(Subscribes, name, args);
        }

        public void Notify(string name)
        {
            SendMessage(Subscribes, name);
        }

        //public void Notify<T>(string name,T arg)
        //{
        //    SendMessage(Subscribes, name,arg);
        //}
        //public void Notify<T1,T2>(string name,T1 arg1,T2 arg2)
        //{
        //    SendMessage(Subscribes, name,arg1,arg2);
        //}
        //public void Notify<T1,T2,T3>(string name,T1 arg1,T2 arg2,T3 arg3)
        //{
        //    SendMessage(Subscribes, name,arg1,arg2,arg3);
        //}
        //public void Notify<T1, T2, T3,T4>(string name, T1 arg1, T2 arg2, T3 arg3,T4 arg4)
        //{
        //    SendMessage(Subscribes, name, arg1, arg2, arg3,arg4);
        //}
        //public void Notify<T1, T2, T3, T4,T5>(string name, T1 arg1, T2 arg2, T3 arg3, T4 arg4,T5 arg5)
        //{
        //    SendMessage(Subscribes, name, arg1, arg2, arg3, arg4,arg5);
        //}
        public void UnSubscribeAll()
        {
            Subscribes.Clear();
        }
    }

    /// <summary>
    /// Subscribe-Publish model implements from Action callback 
    /// 推荐
    /// </summary>

    public class Publisher<K> where K : IConvertible
    {
        Dictionary<K, ForeachMutableList<Delegate>> EventMap = new Dictionary<K, ForeachMutableList<Delegate>>();

        private ForeachMutableList<Delegate> _GetOrAddCallList(K name)
        {
            ForeachMutableList<Delegate> cbList;
            if (!EventMap.TryGetValue(name, out cbList))
            {
                cbList = new ForeachMutableList<Delegate>();
                EventMap.Add(name, cbList);
            }

            return cbList;
        }

        private bool _TryGetCallList(K name, out ForeachMutableList<Delegate> cbList)
        {
            return EventMap.TryGetValue(name, out cbList);
        }

        public void UnSubscribeAll()
        {
            EventMap.Clear();
        }

        public void Subscribe(K name, Action cb)
        {
            _GetOrAddCallList(name).Add(cb);
        }

        public void UnSubscribe(K name, Action cb)
        {
            _GetOrAddCallList(name).Remove(cb);
        }

        public void Notify(K name)
        {
            ForeachMutableList<Delegate> cbList;
            if (_TryGetCallList(name, out cbList))
            {
                Action callback;
                foreach (var cb in cbList)
                {
                    callback = cb as Action;
                    if (callback != null)
                    {
                        try
                        {
                            callback();
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex);
                        }
                    }
                    else
                    {
                        Debug.LogError(name);
                    }
                }
            }
        }

        public void Subscribe<T>(K name, Action<T> cb)
        {
            _GetOrAddCallList(name).Add(cb);
        }

        public void UnSubscribe<T>(K name, Action<T> cb)
        {
            _GetOrAddCallList(name).Remove(cb);
        }

        public void Notify<T>(K name, T arg)
        {
            ForeachMutableList<Delegate> cbList;
            if (_TryGetCallList(name, out cbList))
            {
                Action<T> callback;
                foreach (var cb in cbList)
                {
                    callback = cb as Action<T>;
                    if (callback != null)
                    {
                        try
                        {
                            callback(arg);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex);
                        }
                    }
                    else
                    {
                        Debug.LogError(arg);
                    }
                }
            }
        }

        public void Subscribe<T1, T2>(K name, Action<T1, T2> cb)
        {
            _GetOrAddCallList(name).Add(cb);
        }

        public void UnSubscribe<T1, T2>(K name, Action<T1, T2> cb)
        {
            _GetOrAddCallList(name).Remove(cb);
        }

        public void Notify<T1, T2>(K name, T1 arg1, T2 arg2)
        {
            ForeachMutableList<Delegate> cbList;
            if (_TryGetCallList(name, out cbList))
            {
                Action<T1, T2> callback;
                foreach (var cb in cbList)
                {
                    callback = cb as Action<T1, T2>;
                    if (callback != null)
                    {
                        try
                        {
                            callback(arg1, arg2);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex);
                        }
                    }
                    else
                    {
                        Debug.LogErrorFormat("{0},{1}", arg1, arg2);
                    }
                }
            }
        }

        public void Subscribe<T1, T2, T3>(K name, Action<T1, T2, T3> cb)
        {
            _GetOrAddCallList(name).Add(cb);
        }

        public void UnSubscribe<T1, T2, T3>(K name, Action<T1, T2, T3> cb)
        {
            _GetOrAddCallList(name).Remove(cb);
        }

        public void Notify<T1, T2, T3>(K name, T1 arg1, T2 arg2, T3 arg3)
        {
            ForeachMutableList<Delegate> cbList;
            if (_TryGetCallList(name, out cbList))
            {
                Action<T1, T2, T3> callback;
                foreach (var cb in cbList)
                {
                    callback = cb as Action<T1, T2, T3>;
                    if (callback != null)
                    {
                        try
                        {
                            callback(arg1, arg2, arg3);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex);
                        }
                    }
                    else
                    {
                        Debug.LogErrorFormat("{0},{1},{2}", arg1, arg2, arg3);
                    }
                }
            }
        }

        public void Subscribe<T1, T2, T3, T4>(K name, Action<T1, T2, T3, T4> cb)
        {
            _GetOrAddCallList(name).Add(cb);
        }

        public void UnSubscribe<T1, T2, T3, T4>(K name, Action<T1, T2, T3, T4> cb)
        {
            _GetOrAddCallList(name).Remove(cb);
        }

        public void Notify<T1, T2, T3, T4>(K name, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ForeachMutableList<Delegate> cbList;
            if (_TryGetCallList(name, out cbList))
            {
                Action<T1, T2, T3, T4> callback;
                foreach (var cb in cbList)
                {
                    callback = cb as Action<T1, T2, T3, T4>;
                    if (callback != null)
                    {
                        try
                        {
                            callback(arg1, arg2, arg3, arg4);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex);
                        }
                    }
                    else
                    {
                        Debug.LogErrorFormat("{0},{1},{2},{3}", arg1, arg2, arg3, arg4);
                    }
                }
            }
        }

        public void Subscribe<T1, T2, T3, T4, T5>(K name, Action<T1, T2, T3, T4, T5> cb)
        {
            _GetOrAddCallList(name).Add(cb);
        }

        public void UnSubscribe<T1, T2, T3, T4, T5>(K name, Action<T1, T2, T3, T4, T5> cb)
        {
            _GetOrAddCallList(name).Remove(cb);
        }

        public void Notify<T1, T2, T3, T4, T5>(K name, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ForeachMutableList<Delegate> cbList;
            if (_TryGetCallList(name, out cbList))
            {
                Action<T1, T2, T3, T4, T5> callback;
                foreach (var cb in cbList)
                {
                    callback = cb as Action<T1, T2, T3, T4, T5>;
                    if (callback != null)
                    {
                        try
                        {
                            callback(arg1, arg2, arg3, arg4, arg5);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex);
                        }
                    }
                    else
                    {
                        Debug.LogErrorFormat("{0},{1},{2},{3},{4｝", arg1, arg2, arg3, arg4, arg5);
                    }
                }
            }
        }

        public void Subscribe<T1, T2, T3, T4, T5, T6>(K name, Action<T1, T2, T3, T4, T5, T6> cb)
        {
            _GetOrAddCallList(name).Add(cb);
        }

        public void UnSubscribe<T1, T2, T3, T4, T5, T6>(K name, Action<T1, T2, T3, T4, T5, T6> cb)
        {
            _GetOrAddCallList(name).Remove(cb);
        }

        public void Notify<T1, T2, T3, T4, T5, T6>(K name, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ForeachMutableList<Delegate> cbList;
            if (_TryGetCallList(name, out cbList))
            {
                Action<T1, T2, T3, T4, T5, T6> callback;
                foreach (var cb in cbList)
                {
                    callback = cb as Action<T1, T2, T3, T4, T5, T6>;
                    if (callback != null)
                    {
                        try
                        {
                            callback(arg1, arg2, arg3, arg4, arg5, arg6);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex);
                        }
                    }
                    else
                    {
                        Debug.LogError($"{arg1}, {arg2}, {arg3}, {arg4}, {arg5}, {arg6}");
                    }
                }
            }
        }
    }

    public class GlobalPublisher : SPublisher<string>
    {
    }

    /// <summary>
    /// 全局 Subscribe-Publish model implements from Action callback 
    /// 推荐
    /// </summary>
    public class SPublisher<K> : OpenNGS.Singleton<SPublisher<K>> where K : IConvertible
    {

        private Publisher<K> Publisher = new Publisher<K>();


        public void Subscribe(K name, Action cb)
        {
            Publisher.Subscribe(name, cb);
        }

        public void UnSubscribe(K name, Action cb)
        {
            Publisher.UnSubscribe(name, cb);
        }

        public void Notify(K name)
        {
            Publisher.Notify(name);
        }

        public void Subscribe<T1>(K name, Action<T1> cb)
        {
            Publisher.Subscribe(name, cb);
        }

        public void UnSubscribe<T1>(K name, Action<T1> cb)
        {
            Publisher.UnSubscribe(name, cb);
        }

        public void Notify<T1>(K name, T1 arg1)
        {
            Publisher.Notify(name, arg1);
        }

        public void Subscribe<T1, T2>(K name, Action<T1, T2> cb)
        {
            Publisher.Subscribe(name, cb);
        }

        public void UnSubscribe<T1, T2>(K name, Action<T1, T2> cb)
        {
            Publisher.UnSubscribe(name, cb);
        }

        public void Notify<T1, T2>(K name, T1 arg1, T2 arg2)
        {
            Publisher.Notify(name, arg1, arg2);
        }

        public void Subscribe<T1, T2, T3>(K name, Action<T1, T2, T3> cb)
        {
            Publisher.Subscribe(name, cb);
        }

        public void UnSubscribe<T1, T2, T3>(K name, Action<T1, T2, T3> cb)
        {
            Publisher.UnSubscribe(name, cb);
        }

        public void Notify<T1, T2, T3>(K name, T1 arg1, T2 arg2, T3 arg3)
        {
            Publisher.Notify(name, arg1, arg2, arg3);
        }

        public void Subscribe<T1, T2, T3, T4>(K name, Action<T1, T2, T3, T4> cb)
        {
            Publisher.Subscribe(name, cb);
        }

        public void UnSubscribe<T1, T2, T3, T4>(K name, Action<T1, T2, T3, T4> cb)
        {
            Publisher.UnSubscribe(name, cb);
        }

        public void Notify<T1, T2, T3, T4>(K name, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Publisher.Notify(name, arg1, arg2, arg3, arg4);
        }

        public void Subscribe<T1, T2, T3, T4, T5>(K name, Action<T1, T2, T3, T4, T5> cb)
        {
            Publisher.Subscribe(name, cb);
        }

        public void UnSubscribe<T1, T2, T3, T4, T5>(K name, Action<T1, T2, T3, T4, T5> cb)
        {
            Publisher.UnSubscribe(name, cb);
        }

        public void Notify<T1, T2, T3, T4, T5>(K name, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Publisher.Notify(name, arg1, arg2, arg3, arg4, arg5);
        }

        public void Subscribe<T1, T2, T3, T4, T5, T6>(K name, Action<T1, T2, T3, T4, T5, T6> cb)
        {
            Publisher.Subscribe(name, cb);
        }

        public void UnSubscribe<T1, T2, T3, T4, T5, T6>(K name, Action<T1, T2, T3, T4, T5, T6> cb)
        {
            Publisher.UnSubscribe(name, cb);
        }

        public void Notify<T1, T2, T3, T4, T5, T6>(K name, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Publisher.Notify(name, arg1, arg2, arg3, arg4, arg5, arg6);
        }


        public void UnSubscribeAll()
        {
            Publisher.UnSubscribeAll();
        }
        protected void OnDispose()
        {
            Publisher.UnSubscribeAll();
        }
    }

    #region 模拟Unity SendMessage

    public static void SendMessage(System.Collections.IEnumerable subscribes, string messgae, object[] args)
    {
        foreach (var it in subscribes)
        {
            SendMessage(it, messgae, args);
        }
    }

    public static void SendMessage(this object subscribe, string messgae, params object[] args)
    {
        var fun = _GetMethod(subscribe.GetType(), messgae);
        if (fun == null)
        {
            Debug.LogWarning(" not found method" + subscribe.GetType() + " -> " + messgae);
        }
        else
        {
            _Invoke(subscribe, messgae, fun, args);
        }
    }

    static MethodInfo _GetMethod(Type type, string messgae)
    {
        //TODO::缓存对象获取的方法 进行反射优化
        var method = type.GetMethod(messgae,
          System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic |
          System.Reflection.BindingFlags.Instance);
        if (method == null && type.BaseType != typeof(object))
        {
            return _GetMethod(type.BaseType, messgae);
        }

        return method;
    }

    static void _Invoke(object obj, string messgae, MethodInfo fun, params object[] args)
    {
        var parameters = fun.GetParameters();
        if (parameters.Length == args.Length)
        {
            for (int i = 0; i < parameters.Length; ++i)
            {
                if (parameters[i].ParameterType != args[i].GetType())
                {
                    Debug.LogWarning("" + obj.GetType() + "->" + messgae + ":the " + i + " argument type not match!!");
                    return;
                }
            }

            if (fun != null) fun.Invoke(obj, args);
        }
        else
        {
            Debug.LogWarning("" + obj.GetType() + "->" + messgae + ":argument length not match!!");
        }
    }

    #endregion // 模拟Unity SendMessage
}
