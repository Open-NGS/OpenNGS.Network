using OpenNGS.Shop.Data;
using OpenNGS.Shop.Service;

namespace OpenNGS.Systems
{
    public interface IShopClientAPI
    {
        public void Init();
        public BuyRsp BuyItem(BuyReq request);
        SellRsp SellItem(SellReq _req);
        public ShopRsp GetShopState(ShopReq request);

        long GetFinalBuyPrice(uint shopId, uint goodId);
        (long, Good) GetFinalSellPrice(uint shopId, uint nItemID);
    }
}
