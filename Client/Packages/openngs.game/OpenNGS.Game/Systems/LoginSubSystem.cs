using System;
using System.Collections.Generic;

namespace Systems
{

    public abstract class LoginSubSystem<T> : GameSystemBase<T> where T : LoginSubSystem<T>, new()
    {
        public override GameContextType GetGameContextType() { return GameContextType.Login; }
    }
}

