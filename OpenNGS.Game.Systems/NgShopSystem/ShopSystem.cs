using OpenNGS;
using OpenNGS.Shop.Service;
/// <summary>
/// 放在游戏测的内容
/// </summary>
public class ShopSystem : Singleton<ShopSystem>, INgShopSystem
{
    public void Init() { }

    public BuyRsp BugItem(BuyReq request)
    {
        return ShopService.Instance.BugItem(request);
    }

    public ShopRsp GetShopState(ShopReq request)
    {
        return ShopService.Instance.GetShopState(request);
    }
}
