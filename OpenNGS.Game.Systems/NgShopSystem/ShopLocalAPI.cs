using OpenNGS;
using OpenNGS.Shop.Data;
using OpenNGS.Systems;

public class ShopLocalAPI : Singleton<ShopLocalAPI>, IShopClientAPI
{
    INgShopSystem m_shopSys;

    public void Init()
    {
        m_shopSys = App.GetService<INgShopSystem>();
    }

    public BuyRsp BugItem(BuyReq request)
    {
        if (m_shopSys != null)
        {
            return m_shopSys.BugItem(request);
        }
        else
        {
            NgDebug.LogError("IShopClientAPI not get INgShopSystem");
        }
        return null;
    }

    public ShopRsp GetShopState(ShopReq request)
    {
        if (m_shopSys != null)
        {
            return m_shopSys.GetShopState(request);
        }
        else
        {
            NgDebug.LogError("IShopClientAPI not get INgShopSystem");
        }
        return null;
    }

    ~ShopLocalAPI()
    {
        m_shopSys = null;
    }
}


