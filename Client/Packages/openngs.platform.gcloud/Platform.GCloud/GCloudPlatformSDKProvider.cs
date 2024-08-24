using OpenNGS.Platform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OpenNGS.Platform.GCloud
{
    public class GCloudPlatformSDKProvider : PlatformBaseProvider
    {
        public override bool Init()
        {
            return true;
        }

        public override IPlatfromModule OnCreateMocule(Platform_PLATFORM_MODULE module)
        {
            switch (module)
            {
                case Platform_PLATFORM_MODULE.Users: return new GCloudModuleUsers();
            }
            return null;
        }
    }
}