using OpenNGS;
using OpenNGS.Exchange.Data;
using OpenNGS.Systems;

/// <summary>
/// 放在游戏测的内容
/// </summary>
public class ExchangeSystem : Singleton<ExchangeSystem>, INgExchangeSystem
{
    public void Init() { }

    public ExchangeRsp ExchangeItemByGrid(ExchangeByGridIDReq request)
    {
        return ExchangeService.Instance.ExchangeItemByGrid(request);
    }

    public ExchangeRsp ExchangeItemByID(ExchangeByItemIDReq request)
    {
        return ExchangeService.Instance.ExchangeItemByID(request);
    }
}
