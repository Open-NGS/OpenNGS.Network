using OpenNGS;
using System;
using System.Collections.Generic;

namespace Systems
{
    public abstract class BootstrapSubSystem<T> : GameSystemBase<T> where T : BootstrapSubSystem<T>, new()
    {
        public override GameContextType GetGameContextType() { return GameContextType.BootStrap; }
    }
}

