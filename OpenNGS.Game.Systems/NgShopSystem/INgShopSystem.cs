using OpenNGS.Shop.Service;

public interface INgShopSystem
{
    public BuyRsp BugItem(BuyReq request);
    public ShopRsp GetShopState(ShopReq request);
}
