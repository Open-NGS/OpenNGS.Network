using OpenNGS;
using OpenNGS.Exchange.Data;
using OpenNGS.Systems;

public class ExchangeService : Singleton<ExchangeService>
{
    IExchangeClientAPI m_exchangeApi;

    public void Init(IExchangeClientAPI clientApi)
    {
        m_exchangeApi = clientApi;
    }

    public ExchangeRsp ExchangeItem(ExchangeReq request)
    {
        if(m_exchangeApi != null)
        {
            return m_exchangeApi.ExchangeItem(request);
        }
        else
        {
            NgDebug.LogError("ExchangeService not Initiate");
        }
        return null;
    }

    ~ExchangeService()
    {
        m_exchangeApi = null;
    }
}
