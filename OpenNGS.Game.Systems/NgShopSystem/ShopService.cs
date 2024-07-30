using OpenNGS;
using OpenNGS.Shop.Service;
using OpenNGS.Systems;

public class ShopService : Singleton<ShopService>
{
    IShopClientAPI m_shopApi;
    public void Init(IShopClientAPI clientApi)
    {
        m_shopApi = clientApi;
    }

    public BuyRsp BugItem(BuyReq request)
    {
        return m_shopApi.BugItem(request);
    }

    public ShopRsp GetShopState(ShopReq request)
    {
        return m_shopApi.GetShopState(request);
    }

    ~ShopService()
    {
        m_shopApi = null;
    }
}
