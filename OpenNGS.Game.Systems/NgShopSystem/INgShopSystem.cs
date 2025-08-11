using OpenNGS.Shop.Service;
using System;

public interface INgShopSystem
{
    public BuyRsp BuyItem(BuyReq request);
    public ShopRsp GetShopState(ShopReq request);
    int GetGoodRemainingLimit(uint goodId, uint shelfId,
            int? customDays = null, int? customHours = null,
            int? customMinutes = null, int? customSeconds = null);
    void SetCurrentTime(int days, int hours, int minutes, int seconds);
    DateTimeOffset GetCurrentTime();
}
