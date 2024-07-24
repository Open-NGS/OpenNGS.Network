using OpenNGS.Exchange.Data;

namespace OpenNGS.Systems
{
    public interface INgExchangeSystem
    {
        public ExchangeRsp ExchangeItem(ExchangeReq request);
    }
}

