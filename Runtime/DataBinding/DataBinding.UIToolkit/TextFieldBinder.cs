using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace OpenNGS.UI.DataBinding.UIElements
{
    public class TextFieldBinder : Binder<string, TextField>
    {
        public TextFieldBinder(TextField text, IBindable<string> bindable)
        {
            this.source = bindable;
            this.target = text;
            this.target.value = source.Value;
            this.target.RegisterCallback<ChangeEvent<string>>((v) => { this.source.SetValueWithoutNotify(v.newValue); });
            this.source.onValueChanged += (v) => { this.target.value = v; };
        }
    }
}