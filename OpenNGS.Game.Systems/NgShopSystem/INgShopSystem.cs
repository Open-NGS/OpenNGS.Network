using OpenNGS.Shop.Data;
using OpenNGS.Shop.Service;
using System;

public interface INgShopSystem
{
    public BuyRsp BuyItem(BuyReq request);
    SellRsp SellItem(SellReq _req);
    public ShopRsp GetShopState(ShopReq request);
    int GetGoodRemainingLimit(uint goodId, uint shelfId,
            int? customDays = null, int? customHours = null,
            int? customMinutes = null, int? customSeconds = null);
    void SetCurrentTime(int days, int hours, int minutes, int seconds);
    DateTimeOffset GetCurrentTime();
    void ResetCreateTime(int nYear, int nMonth, int nDay);
    long GetFinalBuyPrice(uint shopId, uint goodId);
    (long, Good) GetFinalSellPrice(uint shopId, uint nItemID);
    void SetExteralDiscount(uint nExternal);
}
