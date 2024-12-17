using OpenNGS.SDK.Core.Initiallization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.SDK.Core
{
    public interface IOpenNGSPlatformServices
    {
        InitializationOptions Options { get; }
        void Initialize(InitializationOptions options);

        string GenerateSignature(string nonce = "");
    }
}
