using OpenNGS.Shop.Data;

namespace OpenNGS.Systems
{
    public interface IShopClientAPI
    {
        public void Init();
        public BuyRsp BugItem(BuyReq request);
        public ShopRsp GetShopState(ShopReq request);
    }
}
