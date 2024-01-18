using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNGS.Exchange.Data;

namespace OpenNGS.Systems
{
    public class ExchangeSystem : EntitySystem
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        public override string GetSystemName()
        {
            return "com.openngs.system.rank";
        }

        public bool Exchange(List<ExchangeItem> src, List<ExchangeItem> target)
        {
            return false;
        }

    }
}
