using OpenNGS.Exchange.Common;
using OpenNGS.Exchange.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Systems
{
    public interface IExchangeSystem
    {
        public EXCHANGE_RESULT_TYPE ExchangeItem(List<SourceItem> src, List<TargetItem> target);
    }
}
