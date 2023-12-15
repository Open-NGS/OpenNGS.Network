using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using OpenNGS.UI.DataBinding.UnityUI;
using UnityEngine;

namespace OpenNGS.UI.DataBinding
{
    public static class BindingUtil
    {
        public static IBinder CreateGenericBinder(object control, string propertyName, IBindable bindable)
        {
            var wrapper = GetCachedPropertyWrapper(control.GetType(), propertyName);
            var binderType = typeof(UIGenericBinder<,>).MakeGenericType(wrapper.GetPropertyType(), control.GetType());
            return Activator.CreateInstance(binderType, control, bindable, propertyName) as IBinder;
        }
        
        public static bool IsGenericList(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);
        }

        public static bool IsGenericEnumerableCollection(Type t)
        {
            // var interfaceType = typeof(IEnumerable);
            // return t.IsGenericType && interfaceType.IsAssignableFrom(t);

            if (t == typeof(string))
                return false;
            
            var array = t.GetInterfaces();
            foreach (var iType in array)
            {
                if (iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return true;
                }
            }

            return false;
        }

        public static Type GetEnumerableGenericType(Type t)
        {
            Type ret = null;
            var array = t.GetInterfaces();
            foreach (var iType in array)
            {
                if (iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    ret = iType.GetGenericArguments()[0];
                    break;
                }

            }
            return ret;
        }

        public static IPropertyWrapper CreatePropertyWrapper(Type type, string propertyName)
        {
            IPropertyWrapper wrapper = null;
            PropertyInfo property = type.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (property == null) throw new Exception(type.Name + " type no property:" + propertyName);
            Type wrapperType = typeof(PropertyWrapper<,>).MakeGenericType(type, property.PropertyType);
            wrapper = Activator.CreateInstance(wrapperType, property) as IPropertyWrapper;
            return wrapper;
        }
        
        private static readonly Dictionary<Type, Dictionary<string, IPropertyWrapper>> PropertyCache = new ();
        public static IPropertyWrapper GetCachedPropertyWrapper(Type type, string propertyName) 
        {
            if (string.IsNullOrEmpty(propertyName)) throw new Exception("propertyName is null");

            if (!PropertyCache.TryGetValue(type, out var proCache)) 
            {
                proCache = new Dictionary<string, IPropertyWrapper>();
                PropertyCache.Add(type, proCache);
            }

            if (!proCache.TryGetValue(propertyName, out var wrapper))
            {
                wrapper = CreatePropertyWrapper(type, propertyName);
                proCache.Add(propertyName, wrapper);
            }
            return wrapper;
        }
        
        private static readonly Dictionary<Type, Dictionary<string, IUnityEventWrapper>> EventWrapperCache = new ();
        public static IUnityEventWrapper GetCachedUnityEventWrapper(Type controlType, Type dataType, string propertyName) 
        {
            if (string.IsNullOrEmpty(propertyName)) throw new Exception("propertyName is null");

            if (!EventWrapperCache.TryGetValue(controlType, out var proCache)) 
            {
                proCache = new Dictionary<string, IUnityEventWrapper>();
                EventWrapperCache.Add(controlType, proCache);
            }

            if (!proCache.TryGetValue(propertyName, out var wrapper))
            {
                PropertyInfo property = controlType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (property == null) throw new Exception(controlType.Name + " type no property:" + propertyName);
                Type wrapperType = typeof(UnityEventWrapper<,>).MakeGenericType(controlType, dataType);
                
                wrapper = Activator.CreateInstance(wrapperType, property) as IUnityEventWrapper;

                proCache.Add(propertyName, wrapper);
            }
            return wrapper;
        }
        
        public static string GetPropertyName<TType, TProperty>(Expression<Func<TType, TProperty>> propertyLambda)
        {
            var me = propertyLambda.Body as MemberExpression;

            if (me == null)
            {
                throw new ArgumentException("You must pass a lambda of the form: 'v => Class.Property' or 'v => object.Property'");
            }

            return me.Member.Name;
        }
        
        public static string GetPropertyName<T>(Expression<Func<T>> propertyLambda)
        {
            var me = propertyLambda.Body as MemberExpression;

            if (me == null)
            {
                throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
            }

            return me.Member.Name;
        }
        
        /// <summary>
        /// Don't call this method, just prevent engine strip GameObject's active property
        /// </summary>
        private static void PreventEngineStripObsoleteProperties()
        {
            var gameObject = new GameObject("");
#pragma warning disable CS0618
            gameObject.active = true;
#pragma warning restore CS0618
        }
    }
}
