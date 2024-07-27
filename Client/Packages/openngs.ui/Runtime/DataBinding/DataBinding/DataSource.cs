using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using OpenNGS.UI.DataBinding.UnityUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OpenNGS.UI.DataBinding
{
    public interface IDataSource
    {
        public void Update();
        public void Bind(object control, string bindingPropertyName, string bindingPath);
        public object GetViewModel();
        public IBindable GetBindable(string bindingPath);
    }
    
    public class DataSource<T> : IDataSource where T: class
    {
        private readonly T m_ViewModel;
        private readonly Dictionary<string, IBindable> m_Bindings = new ();
        public DataSource(T viewModel)
        {
            m_ViewModel = viewModel;
        }
        
        public void Bind(object control, string bindingPropertyName, string bindingPath)
        {
            if (!m_Bindings.TryGetValue(bindingPath, out var bindable))
            { 
                bindable = CreateBindable(bindingPath);
            }

            if (control.GetType() == typeof(RecyclableScrollRect) && bindingPropertyName.Equals("BindingDataSource"))
            {
                var b = CreateRecyclableScrollRectBinder(control as RecyclableScrollRect, bindable);
                bindable.SetBinder(b);
                return;
            }
            
            var binder = BindingUtil.CreateGenericBinder(control, bindingPropertyName, bindable);
            bindable.SetBinder(binder);
        }

        private IBinder CreateRecyclableScrollRectBinder(RecyclableScrollRect rect, IBindable bindable)
        {
            var binderType = typeof(RecyclableScrollRectBinder<>).MakeGenericType(bindable.GetBindableType().GetGenericArguments()[0]);
            return Activator.CreateInstance(binderType, rect, bindable) as IBinder;
        }

        private IBindable CreateBindable(string bindingPath)
        {
            var (wrapper, owner) = GetBindingProperty(bindingPath);
            var value = wrapper.GetValue(owner);
            var wrapperType = wrapper.GetPropertyType();

            IBindable bindable;
            if (BindingUtil.IsGenericEnumerableCollection(wrapperType))
            {
                var bindableListType = typeof(BindableEnumerable<>).MakeGenericType(BindingUtil.GetEnumerableGenericType(wrapperType));
                bindable = (IBindable) Activator.CreateInstance(bindableListType, value);
            }
            else
            {
                var bindableType = typeof(Bindable<>).MakeGenericType(wrapperType);
                bindable = (IBindable) Activator.CreateInstance(bindableType, value);    
            }

            m_Bindings[bindingPath] = bindable;
            bindable.BindSource(owner, wrapper.GetPropertyName());
            return bindable;
        }

        private (IPropertyWrapper, object) GetBindingProperty(string bindingPath)
        {
            var array = bindingPath.Split('/');
            if (array.Length == 1)
            {
                var wrapper = BindingUtil.CreatePropertyWrapper(typeof(T), bindingPath);
                return (wrapper, m_ViewModel);
            }

            var p = BindingUtil.CreatePropertyWrapper(typeof(T), array[0]);
            object owner = m_ViewModel;
            for (var i = 1; i < array.Length; ++i)
            {
                owner = p.GetValue(owner);
                p = BindingUtil.CreatePropertyWrapper(p.GetPropertyType(), array[i]);
            }

            return (p, owner);
        }

        public IBindable GetBindable(string bindingPath)
        {
            if (m_Bindings.TryGetValue(bindingPath, out var bindable))
            {
                return bindable;
            }

            var ret = CreateBindable(bindingPath);
            return ret;
        }

        public object GetViewModel()
        {
            return m_ViewModel;
        }

        public void Update()
        {
            foreach (var kv in m_Bindings)
            {
                kv.Value.Update();
            }
        }
    }
}