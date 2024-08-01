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

    public ExchangeRsp ExchangeItemByGrid(ExchangeByGridIDReq request)
    {
        if(m_exchangeApi != null)
        {
            return m_exchangeApi.ExchangeItemByGrid(request);
        }
        else
        {
            NgDebug.LogError("ExchangeService not Initiate");
        }
        return null;
    }
    public ExchangeRsp ExchangeItemByID(ExchangeByItemIDReq request)
    {
        if (m_exchangeApi != null)
        {
            return m_exchangeApi.ExchangeItemByID(request);
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
