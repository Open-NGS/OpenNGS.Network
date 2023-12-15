using UnityEngine;
using Object = UnityEngine.Object;

namespace OpenNGS.UI.DataBinding.UnityUI
{
    public class UIPropertyBinding : MonoBehaviour
    {
        // public BindingMode Mode = BindingMode.OneWay;
        public string TargetType;

        public Object Target;
        public GameObject Source;
        
        public string BindingProperty;
        public string BindingPath;
        private void Start() { }
    }
}
