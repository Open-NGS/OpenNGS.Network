using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Platform
{
    public abstract class PlatformBaseProvider: IPlatformProvider
    {
        IPlatfromModule[] Modules = new IPlatfromModule[(int)OPENNGS_PLATFORM_MODULE.MUDULE_COUNT];


        public abstract IPlatfromModule OnCreateMocule(OPENNGS_PLATFORM_MODULE module);

        public void CreateMocule(OPENNGS_PLATFORM_MODULE module)
        {
            var obj = OnCreateMocule(module);
            if (obj != null)
            {
                Modules[(int)module] = obj;
            }
        }

        public IPlatfromModule GetModule(OPENNGS_PLATFORM_MODULE module)
        {
            return Modules[(int)module];
        }

        public abstract bool Init();

        public bool IsSupported(OPENNGS_PLATFORM_MODULE module)
        {
            return Modules[(int)module] != null;
        }

        
    }
}
