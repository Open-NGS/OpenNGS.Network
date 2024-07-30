using OpenNGS.Exchange.Service;

namespace OpenNGS.Systems
{
    public interface IExchangeClientAPI
    {
        public void Init();
        public ExchangeRsp ExchangeItemByGrid(ExchangeByGridIDReq request);
        public ExchangeRsp ExchangeItemByID(ExchangeByItemIDReq request);
    }
}
