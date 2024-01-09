using OpenNGSCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Systems
{
    public class EntitySystem
    {
        protected virtual void OnCreate() { }

        public virtual void OnStatus(StatusData status) { }

        public virtual string GetSystemName() { return ""; }
    }
}
