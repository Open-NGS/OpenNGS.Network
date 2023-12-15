using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace OpenNGS.UI.DataBinding
{

    public interface IBinder
    {
    }

    public interface IBindable
    {
        public void SetBinder(IBinder binder);
        public IBindable BindSource(object source, string bindingPath);
        public void Update();
        public Type GetBindableType();
    }

    public interface IBindable<T> : IBindable
    {
        public T Value { get; set; }
        public void SetValueWithoutNotify(T newValue);
        public UnityAction<T> onValueChanged { get; set; }
    }

    public class Bindable<T> : IBindable<T>
    {
        protected T m_Value;
        
        // 在 update 里值类型 boxing 会 gc
        // 相比之下 unboxing 只做一次 copy 动作，性能损耗可忽略不计

        private IBinder m_Binder;

        protected object m_Source;
        protected Type m_SourceType;

        protected IPropertyWrapper<T> m_Wrapper = null;
        private IEquatable<T> m_Equatable;
        
        private readonly bool m_IsValueType;
        private readonly bool m_IsEquatable;
        public Bindable(T val)
        {
            m_Value = val;
            m_IsValueType = typeof(T).IsValueType;
            
            if (m_IsValueType)
            {
                if (m_Value is IEquatable<T> equatable)
                {
                    m_IsEquatable = true;
                    m_Equatable = equatable;
                }
            }
        }
        
        public virtual T Value
        {
            get => m_Value;
            set
            {
                if (m_Value != null && m_Value.Equals(value)) 
                    return;
                
                m_Value = value;
                OnValueChanged();
            }
        }

        public UnityAction<T> onValueChanged { get; set; }

        public void SetBinder(IBinder binder)
        {
            m_Binder = binder;
            OnValueChanged();
        }

        public virtual IBindable BindSource(object source, string bindingPath)
        {
            m_Source = source;
            m_SourceType = source.GetType();
            m_Wrapper = (IPropertyWrapper<T>) BindingUtil.CreatePropertyWrapper(m_SourceType, bindingPath);
            return this;
        }

        protected void OnValueChanged()
        {
            onValueChanged?.Invoke(Value);
        }

        public void SetValueWithoutNotify(T newValue)
        {
            Debug.LogFormat("SetValueWithoutNotify:{0}", newValue);
            m_Value = newValue;

            m_Wrapper?.SetValue(m_Source, newValue);
        }

        public static implicit operator T(Bindable<T> v)
        {
            return v.Value;
        }

        public Type GetBindableType()
        {
            return typeof(T);
        }

        public void Update()
        {
            CheckSourceValueChanged();
        }

        protected virtual void CheckSourceValueChanged()
        {
            if (m_Wrapper == null)
                return;
                
            // update 里判断是否是值类型还是引用类型会有 gc
            // update 里值类型 T 的判空，会 boxing
            
            var v = m_Wrapper.GetGenericValue(m_Source);
            if (m_IsValueType)
            {
                if (m_IsEquatable)
                {
                    if (m_Equatable.Equals(v)) 
                        return;
                    
                    Value = v;
                    m_Equatable = (IEquatable<T>) v;
                }
                else
                {
                    if (!v.Equals(m_Value))
                    {
                        Value = v;
                    }
                }
            }
            else
            {
                if (v != null && !v.Equals(m_Value))
                {
                    Value = v;
                }
            }
        }
    }
}