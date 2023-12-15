using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEngine.Events;

namespace OpenNGS.UI.DataBinding
{
    public interface IUnityEventWrapper
    {
        public void AddListener(object obj, object value);

        public void RemoveListener(object obj, object value);
        object GetValue(object obj);
    }
    
    public class UnityEventWrapper<TControl, T>: IUnityEventWrapper
    {
        private readonly Action<UnityEvent<T>, UnityAction<T>> m_AddListenerCall;
        private readonly Action<UnityEvent<T>, UnityAction<T>> m_RemoveListenerCall;
        private readonly Func<TControl, UnityEvent<T>> m_Getter;

        public UnityEventWrapper(PropertyInfo property)
        {
            var type = typeof(UnityEvent<T>);
            var aMethod = type.GetMethod("AddListener");
            var rMethod = type.GetMethod("RemoveListener");

            if (aMethod == null || rMethod == null) return;
            
            m_AddListenerCall = Delegate.CreateDelegate(typeof(Action<UnityEvent<T>, UnityAction<T>>), aMethod) as Action<UnityEvent<T>, UnityAction<T>>;
            m_RemoveListenerCall = Delegate.CreateDelegate(typeof(Action<UnityEvent<T>, UnityAction<T>>), rMethod) as Action<UnityEvent<T>, UnityAction<T>>;
            m_Getter = Delegate.CreateDelegate(typeof(Func<TControl, UnityEvent<T>>), property.GetGetMethod()) as Func<TControl, UnityEvent<T>>;
        }

        private void AddListener(UnityEvent<T> obj, UnityAction<T> value)
        {
            m_AddListenerCall?.Invoke(obj, value);
        }

        private void RemoveListener(UnityEvent<T> obj, UnityAction<T> value)
        {
            m_RemoveListenerCall?.Invoke(obj, value);
        }

        public void AddListener(object obj, object value)
        {
            try
            {
                // may raise invalid cast exception
                AddListener((UnityEvent<T>) obj, (UnityAction<T>) value);
            }
            catch (Exception e)
            {
                Debug.Log($"Data binding add listener exception, check the type u passed {e} stack trace: {e.StackTrace}");
            }
        }

        public void RemoveListener(object obj, object value)
        {
            try
            {
                // may raise invalid cast exception
                RemoveListener((UnityEvent<T>) obj, (UnityAction<T>) value);
            }
            catch (Exception e)
            {
                Debug.Log($"Data binding remove listener exception: {e} stack trace: {e.StackTrace}");       
            }
        }

        public object GetValue(object obj)
        {
            return m_Getter((TControl)obj);
        }
    }
}
