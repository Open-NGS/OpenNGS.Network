using OpenNGS.Exchange.Data;

namespace OpenNGS.Systems
{
    public interface INgExchangeSystem
    {
        public ExchangeRsp ExchangeItemByGrid(ExchangeByGridIDReq request);
        public ExchangeRsp ExchangeItemByID(ExchangeByItemIDReq request);
    }
}

