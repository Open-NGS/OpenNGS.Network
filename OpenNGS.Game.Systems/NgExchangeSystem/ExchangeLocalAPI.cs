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
        if(m_exchangeSys != null)
        {
            return m_exchangeSys.ExchangeItem(request);
        }
        else
        {
            NgDebug.LogError("ExchangeLocalAPI not get INgExchangeSystem");
        }
        return null;
    }

    ~ExchangeLocalAPI()
    {
        m_exchangeSys = null;
    }
}
