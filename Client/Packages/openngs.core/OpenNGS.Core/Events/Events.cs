using System;
using UnityEngine.Events;

namespace OpenNGS.Events
{
    [Serializable]
    public class OnIntChangeEvent : UnityEvent<int>{}

    [Serializable]
    public class OnFloatChangeEvent : UnityEvent<float> { }
}
