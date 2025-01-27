using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Platform
{
    public class PlatformApp
    {
        public static bool IsAppInstalled(string appStr)
        {
            return Platform.GetBase().IsAppInstalled(appStr);
        }

        public static bool IsAppSubscribed(string appId)
        {
            return Platform.GetBase().IsAppSubscribed(appId);
        }
    }
}
