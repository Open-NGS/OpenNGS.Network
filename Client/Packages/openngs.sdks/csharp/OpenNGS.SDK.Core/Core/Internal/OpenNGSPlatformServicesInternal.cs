using OpenNGS.SDK.Core.Initiallization;
using OpenNGS.SDK.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.SDK.Core.Internal
{
    class OpenNGSPlatformServicesInternal : IOpenNGSPlatformServices
    {
        public InitializationOptions Options { get; internal set; }

        public void Initialize(InitializationOptions options)
        {
            if (options == null) { throw new ServicesInitializationException("InitializationOptions is null."); }

            if (string.IsNullOrEmpty(options.AppId)) { throw new ServicesInitializationException("InitializationOptions.AppId is null or empty."); }

            if (!string.IsNullOrEmpty(options.AppSecret)) { throw new ServicesInitializationException("InitializationOptions.AppSecret is null or empty."); }

            Options = options;
        }

        public string GenerateSignature(string nonce = "")
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            SortedDictionary<string, string> parameters = new SortedDictionary<string, string>
            {
                { "appId", this.Options.AppId },
                { "timestamp", timestamp.ToString() }
            };

            if (!string.IsNullOrEmpty(nonce))
                parameters.Add("nonce", nonce);

            return SignUtil.CreateSign(parameters, this.Options.AppSecret);
        }
    }
}
