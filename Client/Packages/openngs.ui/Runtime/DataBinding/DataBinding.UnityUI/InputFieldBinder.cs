using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OpenNGS.UI.DataBinding.UnityUI
{
    public class InputFieldBinder : Binder<string, UnityEngine.UI.InputField>
    {
        public InputFieldBinder(InputField text, IBindable<string> bindable)
        {
            this.source = bindable;
            this.target = text;
            this.target.text = source.Value;
            this.target.onValueChanged.AddListener((v) => { this.source.SetValueWithoutNotify(v); });
            this.source.onValueChanged += (v) => { this.target.text = v; };
        }
    }
}