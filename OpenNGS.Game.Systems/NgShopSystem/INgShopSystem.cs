using OpenNGS.Shop.Data;

public interface INgShopSystem
{
    public BuyRsp BugItem(BuyReq request);
    public ShopRsp GetShopState(ShopReq request);
}
