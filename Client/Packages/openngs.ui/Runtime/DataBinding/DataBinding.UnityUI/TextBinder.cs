using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OpenNGS.UI.DataBinding.UnityUI
{
    public class TextBinder : Binder<string, UnityEngine.UI.Text>
    {
        public TextBinder(Text text, IBindable<string> bindable)
        {
            this.source = bindable;
            this.target = text;
            this.source.onValueChanged += (v) => { this.target.text = this.source.Value; };
        }
    }
}