using OpenNGS.Shop.Service;

public interface INgShopSystem
{
    public BuyRsp BuyItem(BuyReq request);
    public ShopRsp GetShopState(ShopReq request);
    int GetGoodRemainingLimit(uint goodId, uint shelfId);
}
