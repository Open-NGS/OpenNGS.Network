using OpenNGS.Exchange.Data;

namespace OpenNGS.Systems
{
    public interface IExchangeClientAPI
    {
        public void Init();
        public ExchangeRsp ExchangeItem(ExchangeReq request);
    }
}
