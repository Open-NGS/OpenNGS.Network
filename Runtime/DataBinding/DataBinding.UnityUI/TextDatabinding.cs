using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OpenNGS.UI.DataBinding.UnityUI
{
    public class TextDatabinding : MonoBehaviour
    {
        public UnityEngine.UI.Text target;
        public string bindingPath;
        private string oldValue;
        public IBindable<string> source;

        private InputField input;

        private void Awake()
        {
            input = this.GetComponentInParent<InputField>();
        }

        private void LateUpdate()
        {
            if (target == null || source == null)
                return;
            if(target.text!= oldValue &&  target.text != source.Value)
            {
                oldValue = target.text;
                source.SetValueWithoutNotify(target.text);
                
            }
        }

        internal void SetValue(string v)
        {
            oldValue = v;
            if (input == null)
                target.text = v;
            else
                input.text = v;
        }
    }
}
