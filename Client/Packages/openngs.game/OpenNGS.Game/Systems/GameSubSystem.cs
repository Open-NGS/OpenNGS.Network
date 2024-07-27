using System;
using System.Collections.Generic;

namespace Systems
{
    public abstract class GameSubSystem<T> : GameSystemBase<T> where T : GameSubSystem<T>, new()
    {
        public override GameContextType GetGameContextType() { return GameContextType.World; }
    }
}

