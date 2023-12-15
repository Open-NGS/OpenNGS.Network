using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Platform
{
    class PlatformTools
    {
        internal static bool IsAppInstalled(string appStr)
        {
            return Platform.GetBase().IsAppInstalled(appStr);
        }
    }
}
