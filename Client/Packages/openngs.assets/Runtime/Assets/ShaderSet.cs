using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenNGS.Assets
{
    class ShaderSet : AssetSet<Shader>
    {
#if UNITY_PS4 || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
        public new static Shader Get(string name)
        {
            return Shader.Find(name);
        }
#endif
    }
}
