using OpenNGS.SDK.Auth;
using OpenNGS.SDK.Core.Initiallization;
using OpenNGS.SDK.Core.Internal;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.SDK.Core
{
    public static class OpenNGSPlatformServices
    {

        public static IOpenNGSPlatformServices Instance { get; set; }

        public static void Initialize (InitializationOptions options)
        {
            if (Instance == null)
            {
                Instance = new OpenNGSPlatformServicesInternal();
            }

            Instance.Initialize(options);

            AuthcationService.Instance = new AuthcationServiceInternal();
        }

    }
}
