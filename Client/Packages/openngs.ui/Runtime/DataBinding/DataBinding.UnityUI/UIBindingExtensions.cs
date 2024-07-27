using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OpenNGS.UI.DataBinding.UnityUI
{

    public static class UIBindingExtensions
    {
        public static void Bind(this GameObject element, IDataSource data)
        {
            var bindings = element.GetComponentsInChildren<UIPropertyBinding>(true);
            foreach (var binding in bindings)
            {
                if (string.IsNullOrEmpty(binding.BindingProperty) || string.IsNullOrEmpty(binding.BindingPath))
                    continue;
                
                if (binding.Source == element)
                    data.Bind(binding.Target, binding.BindingProperty, binding.BindingPath);
            }
        }

        public static void Bind(this InputField behaviour, IBindable bindable)
        {
            var binder = BindingUtil.CreateGenericBinder(behaviour, nameof(behaviour.text), bindable);
            bindable.SetBinder(binder);
        }
        
        public static void Bind(this Text behaviour, IDataSource dataSource, string bindingPath)
        {
            var bindable = dataSource.GetBindable(bindingPath);
            if (bindable == null) return;
            var binder = BindingUtil.CreateGenericBinder(behaviour, nameof(behaviour.text), bindable);
            bindable.SetBinder(binder);
        }
        
        public static void Bind(this InputField behaviour, IDataSource dataSource, string bindingPath)
        {
            var bindable = dataSource.GetBindable(bindingPath);
            if (bindable == null) return;
            var binder = BindingUtil.CreateGenericBinder(behaviour, nameof(behaviour.text), bindable);
            bindable.SetBinder(binder);
        }
        
        public static void Bind(this Text behaviour, IBindable bindable)
        {
            var binder = BindingUtil.CreateGenericBinder(behaviour, nameof(behaviour.text), bindable);
            bindable.SetBinder(binder);
        }
        
        public static void Bind<T>(this T behaviour, string propertyName, IBindable bindable)
        {
            var binder = BindingUtil.CreateGenericBinder(behaviour, propertyName, bindable);
            bindable.SetBinder(binder);
        }
        
        public static void Bind<T>(this T behaviour, string propertyName, IDataSource dataSource, string bindingPath)
        {
            var bindable = dataSource.GetBindable(bindingPath);
            if (bindable == null) return;
            var binder = BindingUtil.CreateGenericBinder(behaviour, propertyName, bindable);
            bindable.SetBinder(binder);
        }
    }
}