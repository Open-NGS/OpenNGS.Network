using System;
using System.Reflection;

namespace OpenNGS.UI
{
    public interface IPropertyWrapper
    {
        object GetValue(object obj);
        void SetValue(object obj, object value);
        Type GetPropertyType();
        string GetPropertyName();
    }

    public interface IPropertyWrapper<T> : IPropertyWrapper
    {
        public T GetGenericValue(object obj);
    }

    public class PropertyWrapper<T, V> : IPropertyWrapper<V>
    {
        private readonly Func<T, V> m_Getter;
        private readonly Action<T, V> m_Setter;
        private string m_Name;

        public PropertyWrapper(PropertyInfo property)
        {
            var getter = property.GetGetMethod() ?? property.GetGetMethod(true);
            var setter = property.GetSetMethod() ?? property.GetSetMethod(true);
            
            if (getter != null) 
                m_Getter = Delegate.CreateDelegate(typeof(Func<T, V>), getter) as Func<T, V>;
            
            if (setter != null)
                m_Setter = Delegate.CreateDelegate(typeof(Action<T, V>), setter) as Action<T, V>;
            
            m_Name = property.Name;
        }

        public V GenericValue(T obj)
        {
            return obj == null || m_Getter == null ? default(V) : m_Getter(obj);
        }

        private void SetValue(T obj, V value)
        {
            if (obj == null)
                return;

            m_Setter?.Invoke(obj, value);
        }

        public V GetGenericValue(object obj)
        {
            return GenericValue((T)obj);
        }

        public object GetValue(object obj)
        {
            return GenericValue((T) obj);
        }
        
        public void SetValue(object obj, object value)
        {
            SetValue((T) obj, (V) value);
        }

        public Type GetPropertyType()
        {
            return typeof(V);
        }

        public string GetPropertyName()
        {
            return m_Name;
        }
    }
}