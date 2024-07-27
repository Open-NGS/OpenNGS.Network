using System;
using System.Collections;
using System.Collections.Generic;
using OpenNGS.UI.DataBinding.UnityUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace OpenNGS.UI.DataBinding
{
    public class UIGenericBinder<TSource, TTarget> : Binder<TSource, TTarget>
    {
        public UIGenericBinder(TTarget control, IBindable<TSource> bindable, string targetProperty)
        {
            source = bindable;
            target = control;
            
            source.onValueChanged += arg0 =>
            {
                var property = BindingUtil.GetCachedPropertyWrapper(control.GetType(), targetProperty);
                property.SetValue(target, source.Value);
            };
            
            // ugui on control value changed
            var uiEventName = UIEventRegister.GetRegisteredEvent(typeof(TTarget), targetProperty);
            if (!string.IsNullOrEmpty(uiEventName))
            {
                // toggle onValueChanged is not property
                // this is tricky
                if (target is UnityEngine.UI.Toggle toggle)
                {
                    toggle.onValueChanged.AddListener(((Bindable<bool>) bindable).SetValueWithoutNotify);
                    return;
                }
                
                // get event wrapper
                var unityEventWrapper = BindingUtil.GetCachedUnityEventWrapper(typeof(TTarget), typeof(TSource), uiEventName);
                var propertyValue = unityEventWrapper.GetValue(control);
                
                // add event listener
                unityEventWrapper.AddListener(propertyValue, new UnityAction<TSource>(bindable.SetValueWithoutNotify));
            }
            
            // ui tool kit binding callback
            if (target is CallbackEventHandler eventHandler)
            {
                try
                {
                    eventHandler.RegisterCallback<ChangeEvent<TSource>>(v => source.SetValueWithoutNotify(v.newValue));
                }
                catch(Exception e)
                {
                    Debug.Log($"Exception {e.StackTrace} {e}");
                }
            }
        }
    }
}
