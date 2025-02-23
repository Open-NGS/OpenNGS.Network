using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS
{
    public interface IContextContainer
    {
        public GameContext GetGameContext(GameContextType contextID);
    }
}
