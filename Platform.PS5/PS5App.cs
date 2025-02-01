using OpenNGS.Platform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.Platform.PS5
{

    public class PS5App : IAppProvider
    {
        public PLATFORM_MODULE Module => PLATFORM_MODULE.APP;

        public bool IsDebug
        {
            get
            {
#if DEBUG || DEVELOPMENT
                return true;
#else
                return false;
#endif
            }
            set { }
        }



        public bool IsAppInstalled(string appStr)
        {
            return true;
        }

        public bool IsAppSubscribed(string appId)
        {
            return true;
        }

        public void Start()
        {
            
        }

        public void Stop()
        {
            
        }

        public void Update()
        {
            
        }
    }
}