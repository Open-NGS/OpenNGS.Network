using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using OpenNGS.UI.DataBinding;
using UnityEngine;

namespace OpenNGS.UI.DataBinding.UnityUI
{
    public class RecyclableScrollRectBinder<T> : Binder<IEnumerable<T>, RecyclableScrollRect>, IRecyclableScrollRectDataSource
    {
        public RecyclableScrollRectBinder(RecyclableScrollRect rect, IBindable<IEnumerable<T>> bindable)
        {
            m_IsTKeyValuePair = IsTKeyValuePair();
            source = bindable;
            target = rect;
            target.Initialize(this);
            source.onValueChanged += args => rect.ReloadData();
        }

        /// <summary>
        /// Type Dictionary、SortedDictionary、SortedList
        /// IEnumerable T is KeyValuePair
        /// KeyValuePair is struct not class
        /// It's tricky
        /// </summary>
        /// <returns></returns>

        private readonly bool m_IsTKeyValuePair = false;
        private static bool IsTKeyValuePair() 
        {
            var type = typeof(T);
            if (type.IsGenericType)
            {
                return type.GetGenericTypeDefinition() != null && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
            }
            return false;
        }

        private IPropertyWrapper m_ValuePropertyWrapper;
        private PropertyInfo m_ValuePropertyInfo;
        private object GetKeyValuePairValue(T kv)
        {
            // Use KeyValuePair reflection PropertyInfo.GetValue is too slow
            // Use Delegate.CreateDelegate will cause ArgumentException : method arguments are incompatible
            // m_ValuePropertyWrapper ??= CreateKeyValuePairValuePropertyWrapper();
            // return m_ValuePropertyWrapper.GetValue(kv);

            m_ValuePropertyInfo ??= typeof(T).GetProperty("Value");
            var ret = m_ValuePropertyInfo?.GetValue(kv);
            return ret;
        }

        private IPropertyWrapper CreateKeyValuePairValuePropertyWrapper()
        {
            // This will cause ArgumentException : method arguments are incompatible
            // Don't know why..
            
            var valueType = typeof(T);
            var wrapperType = typeof(PropertyWrapper<,>).MakeGenericType(valueType, valueType.GetGenericArguments()[1]);
            var wrapper = Activator.CreateInstance(wrapperType, valueType.GetProperty("Value")) as IPropertyWrapper;
            return wrapper;
        }
        
        private T GetItemByIndex(int index)
        {
            // if bindable value is IList<T> or IReadOnlyList<T> return value by index, otherwise it iterates.
            var v = source.Value;
            if (source.Value is IList<T> list)
            {
                return list[index];
            }
            
            if (source.Value is IReadOnlyList<T> rList)
            {
                return rList[index];
            }
            
            var curIndex = 0;
            
            using var enumerator = source.Value.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (curIndex == index)
                    return enumerator.Current;
                
                curIndex++;
            }

            return default(T);
        }

        private object GetKeyValuePairItemByIndex(int index)
        {
            var curIndex = 0;
            
            // Collection Dictionary<TKey,TValue>、SortedList<TKey, TValue>、SortedDictionary<TKey, TValue>
            if (source.Value is IDictionary dictionary)
            {
                var e = dictionary.GetEnumerator();
                while (e.MoveNext())
                {
                    if (curIndex == index)
                        return e.Value;
                
                    curIndex++;
                }
            }
            
            using var enumerator = source.Value.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (curIndex == index)
                    return GetKeyValuePairValue(enumerator.Current);
                
                curIndex++;
            }

            return null;
        }

        #region IRecyclableScrollRectDataSource
        public int GetItemCount()
        {
            if (source.Value == null) 
                return 0;
            
            // if bindable value is ICollection<T> return count, otherwise it iterates.
            if (source.Value is ICollection<T> collectionT)
            {
                return collectionT.Count;
            }
            
            if (source.Value is ICollection collection)
            {
                return collection.Count;
            }

            var result = 0;
            
            using var enumerator = source.Value.GetEnumerator();
            while (enumerator.MoveNext())
                result++;

            return result;
        }

        public void SetCell(ICell cell, int index)
        {
            if (source.Value == null)
                return;

            if (m_IsTKeyValuePair)
            {
                var v = GetKeyValuePairItemByIndex(index);
                if (v != null) cell.ConfigureCell(index, v);
            }
            else
            {
                var data = GetItemByIndex(index);
                if (data != null) cell.ConfigureCell(index, data);
            }
        }
        #endregion
    }
}
