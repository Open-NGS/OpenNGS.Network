using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace OpenNGS.UI.DataBinding.UIElements
{

    public class UIBinding : IBinding
    {
        private TextField text;
        private IBindable<string> bindable;

        public UIBinding(TextField text, IBindable<string> bindable)
        {
            this.text = text;
            this.bindable = bindable;
            text.RegisterValueChangedCallback<string>((v) =>{ bindable.SetValueWithoutNotify(v.newValue); });
            bindable.onValueChanged += (v) => { text.SetValueWithoutNotify(v); };
        }

        public void PreUpdate()
        {
            Debug.LogFormat("{0}:PreUpdate", this);
        }

        public void Release()
        {
            Debug.LogFormat("{0}:Release", this);
        }

        public void Update()
        {
            Debug.LogFormat("{0}:Update", this);
        }
    }

    public static class UIBindingExtensions
    {
        public static void Bind(this TextField self, IBindable<string> bindable)
        {
            bindable.SetBinder(new TextFieldBinder(self, bindable));
        }

        public static void Bind(this VisualElement element, IDataSource data)
        {
            element.BindValues(data);
            if (element.childCount == 0)
            {
                return;
            }

            foreach (var child in element.Children())
            {
                child.Bind(data);
            }
        }

        private static void BindValues(this VisualElement element, IDataSource data)
        {
            if (element is TextField text && text.bindingPath != null)
            {
                foreach (var propertyInfo in data.GetViewModel().Values())
                {
                    if (text.bindingPath.Equals(propertyInfo.Name))
                    {
                        data.Bind(element, nameof(text.value), propertyInfo.Name);
                    }
                }
            }
        }

        private static IEnumerable<PropertyInfo> Values(this object data)
        {
            foreach (var propertyInfo in data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                yield return propertyInfo;
            }
        }
        
        public static void Bind<T>(this T behaviour, string propertyName, IBindable bindable)
        {
            var binder = BindingUtil.CreateGenericBinder(behaviour, propertyName, bindable);
            bindable.SetBinder(binder);
        }
    }
}