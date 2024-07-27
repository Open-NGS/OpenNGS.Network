using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;

namespace OpenNGS.UI.DataBinding
{
    public class UIEventRegisterKey
    {
        public UIEventRegisterKey(Type controlType, string propertyName)
        {
            ControlType = controlType;
            PropertyName = propertyName;
        }
        
        /// <summary>
        /// implicit operator type cast
        /// </summary>
        public static implicit operator UIEventRegisterKey((Type, string) tuple) => new UIEventRegisterKey(tuple.Item1, tuple.Item2);
        private Type ControlType { get; }
        private string PropertyName { get; }
        
        /// <summary>
        /// override key equals function
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is UIEventRegisterKey other))
            {
                return false;
            }
            
            return PropertyName == other.PropertyName && ControlType == other.ControlType;
        }
    
        public override int GetHashCode()
        {
            return PropertyName.GetHashCode() ^ ControlType.GetHashCode();
        }
    }
    
    public class UIEventRegister
    {
        private static Dictionary<UIEventRegisterKey, string> _dic = new()
        {
            // ugui controls
            [(typeof(Slider), "value")] = "onValueChanged",
            [(typeof(InputField), "text")] = "onValueChanged",
            [(typeof(Toggle), "isOn")] = "onValueChanged",
            [(typeof(Scrollbar), "value")] = "onValueChanged",
            [(typeof(Dropdown), "value")] = "onValueChanged",
            
            // ui tool kit controls
            // [(typeof(TextField), "value")] ="onValueChanged"
        };

        public static string GetRegisteredEvent(Type type, string propertyName)
        {
            if (_dic.TryGetValue((type, propertyName), out var e))
            {
                return e;
            }
            return "";
        }
    }
}