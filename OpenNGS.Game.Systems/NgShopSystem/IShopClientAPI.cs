using OpenNGS.Shop.Service;

namespace OpenNGS.Systems
{
    public interface IShopClientAPI
    {
        public void Init();
        public BuyRsp BuyItem(BuyReq request);
        public ShopRsp GetShopState(ShopReq request);
    }
}
