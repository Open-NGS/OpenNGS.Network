using OpenNGS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.UI
{
    public class UISystem
    {
        static Dictionary<UISystemType, IUISystem> Systems = new Dictionary<UISystemType, IUISystem>();
        public static IUISystem Get(UISystemType type)
        {
            IUISystem system = null;
            if(!Systems.TryGetValue(type, out system))
            {
                throw new System.Exception("UI System not existed. Type:" + type.ToString());
            }
            return system;
        }

        public static void Register(UISystemType type, IUISystem system)
        {
            Systems[type] = system;
        }

        public static void Init()
        {
            foreach(var sys in Systems)
            {
                sys.Value.InitSystem();
            }
        }
    }
}
