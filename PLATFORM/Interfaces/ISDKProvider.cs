using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Platform
{
    public interface IModuleProvider
    {
        PLATFORM_MODULE Module { get; }

        void Start();
        void Stop();
        void Update();
    }

    public interface ISDKProvider
    {
        IModuleProvider CreateProvider(PLATFORM_MODULE module);

        bool Initialize();
        void Terminate();
        void Update();
    }
}
