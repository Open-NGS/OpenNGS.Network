using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace OpenNGS.UI.DataBinding
{
    public class BindableEnumerable<T> : Bindable<IEnumerable<T>>
    {
        public BindableEnumerable(IEnumerable<T> val) : base(val) { }

        private int m_ElementCount = 0;
        

        public override IBindable BindSource(object source, string bindingPath)
        {
            m_Source = source;
            m_SourceType = source.GetType();
            
            CreatePropertyWrapper(bindingPath);
            
            m_Value = m_Wrapper?.GetGenericValue(m_Source);
            m_ElementCount = GetCount();
            return this;
        }

        private void CreatePropertyWrapper(string propertyName)
        {
            var property = m_SourceType.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (property == null) throw new Exception(m_SourceType.Name + " type no property:" + propertyName);
            var wrapperType = typeof(PropertyWrapper<,>).MakeGenericType(m_SourceType, typeof(IEnumerable<T>));
            m_Wrapper = Activator.CreateInstance(wrapperType, property) as IPropertyWrapper<IEnumerable<T>>;
        }

        private int GetCount()
        {
            if (m_Value == null)
                return 0;
            
            // if bindable value is ICollection<T> return count, otherwise it iterates.
            if (m_Value is ICollection<T> collectionT)
            {
                return collectionT.Count;
            }
            
            if (m_Value is ICollection collection)
            {
                return collection.Count;
            }
            
            var result = 0;
            
            using var enumerator = m_Value.GetEnumerator();
            while (enumerator.MoveNext())
                result++;

            return result;
        }

        protected override void CheckSourceValueChanged()
        {
            // check list reference changed
            var t = m_Wrapper?.GetGenericValue(m_Source);
            if (t != null && !ReferenceEquals(m_Value, t))
            {
                Value = t;
            }
            
            // check elements count changed or not
            if (m_ElementCount != GetCount())
            {
                m_ElementCount = GetCount();
                OnValueChanged();
            }
        }
    }
}
