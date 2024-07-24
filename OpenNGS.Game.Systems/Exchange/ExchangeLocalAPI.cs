using OpenNGS;
using OpenNGS.Exchange.Data;
using OpenNGS.Systems;

public class ExchangeLocalAPI : Singleton<ExchangeLocalAPI>, IExchangeClientAPI
{
    INgExchangeSystem m_exchangeSys;

    public void Init()
    {
        m_exchangeSys = App.GetService<INgExchangeSystem>();
    }

    public ExchangeRsp ExchangeItem(ExchangeReq request)
    {
        return m_exchangeSys.ExchangeItem(request);
    }
}
