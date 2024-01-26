using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS
{
    public interface ISystem
    {
        void Init();

        string GetSystemName();
    }
}
