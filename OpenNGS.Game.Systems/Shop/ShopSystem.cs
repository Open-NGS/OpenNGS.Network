using System;
using System.Collections.Generic;
using OpenNGS.Exchange.Common;
using OpenNGS.Exchange.Data;
using OpenNGS.Shop.Common;
using OpenNGS.Shop.Data;
using Systems;

namespace OpenNGS.Systems
{
    public class ShopSystem : GameSubSystem<ShopSystem>, IShopSystem
    {
        public Action<BuyItemRsq> OnBuyItem;
        public Action<SellItemRsq> OnSellItem;
        public Action<GetShopInfoRsq> OnGetShopInfo;

        private IExchangeSystem m_exchangeSys = null;
        private IItemSystem m_itemSys = null;
        private ISaveSystem m_saveSys = null;
        protected override void OnCreate()
        {
            m_exchangeSys = App.GetService<IExchangeSystem>();
            m_itemSys = App.GetService<IItemSystem>();
            m_saveSys = App.GetService<ISaveSystem>();
            base.OnCreate();
        }


        public override string GetSystemName()
        {
            return "com.openngs.system.shop";
        }

        /// <summary>
        /// 获取当前页面内的商品列表
        /// </summary>
        public Dictionary<uint, Good> GetGoodsInfo(uint currentShopId, uint shelfId)
        {
            Shelf shelf = NGSStaticData.shelfs.GetItem(currentShopId, shelfId);

            if (shelf == null)
                return null;

            Dictionary<uint, Good> result = NGSStaticData.goods.GetItems(shelfId);
            return result;
        }

        /// <summary>
        /// 检查商店内是否有该商品
        /// </summary>
        private bool CheckShopItem(uint shopId, uint shelfId, uint shopItemId)
        {
            Shelf shelf = NGSStaticData.shelfs.GetItem(shopId, shelfId);

            if(shelf == null)
                return false; 

            Good shopItemInfo = NGSStaticData.goods.GetItem(shelfId, shopItemId);

            if(shopItemInfo == null)
                return false;

            return true;
        }

        public SHOP_RESULT_TYPE BuyItem(BuyItemInfo item)
        {
            if (item == null) 
                return SHOP_RESULT_TYPE.SHOP_RESULT_TYPE_NO_ITEM;

            //根据shopItemID获取商品信息
            if (!CheckShopItem(item.ShopId, item.ShelfId, item.ShopItemId)) 
                return SHOP_RESULT_TYPE.SHOP_RESULT_TYPE_ERROR_ITEM;

            Good good = NGSStaticData.goods.GetItem(item.ShelfId, item.ShopItemId);

            List<SourceItem> sourceItems = new List<SourceItem>();
            List<TargetItem> targetItems = new List<TargetItem>();

            uint id = m_itemSys.GetGuidByItemID(good.CurrencyId);
            SourceItem sourceItem = new SourceItem();
            sourceItem.GUID = id;
            sourceItem.Count = good.CurrencyCounts * item.ShopItemCount;
            sourceItems.Add(sourceItem);

            TargetItem targetItem = new TargetItem();
            targetItem.ItemID = good.ItemId;
            targetItem.Count = item.ShopItemCount;
            targetItems.Add(targetItem);

            switch (m_exchangeSys.ExchangeItem(sourceItems, targetItems))
            {
                case EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOENOUGH:
                    return SHOP_RESULT_TYPE.SHOP_RESULT_TYPE_NO_ITEM;
                case EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOITEM:
                    return SHOP_RESULT_TYPE.SHOP_RESULT_TYPE_ERROR_ITEM;
            }

            m_saveSys.SaveFile();

            return SHOP_RESULT_TYPE.SHOP_RESULT_TYPE_SUCCESS;
        }

        public SHOP_RESULT_TYPE SellItem(SellItemInfo item)
        {
            if (item == null) 
                return SHOP_RESULT_TYPE.SHOP_RESULT_TYPE_NO_ITEM;

            ShopSell sellitem = NGSStaticData.sells.GetItem(item.ShopId, item.ItemId);

            if (sellitem == null)
                return SHOP_RESULT_TYPE.SHOP_RESULT_TYPE_NO_SELL;

            List<SourceItem> sourceItems = new List<SourceItem>();
            List<TargetItem> targetItems = new List<TargetItem>();

            SourceItem sourceItem = new SourceItem();
            sourceItem.GUID = item.GUID;
            sourceItem.Count = item.ShopItemCount;
            sourceItems.Add(sourceItem);

            uint id = m_itemSys.GetGuidByItemID(sellitem.SellPriceItem);
            TargetItem targetItem = new TargetItem();
            targetItem.ItemID = id;
            targetItem.Count = sellitem.SellPriceCount * item.ShopItemCount;
            targetItems.Add(targetItem);

            switch (m_exchangeSys.ExchangeItem(sourceItems, targetItems))
            {
                case EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOENOUGH:
                    return SHOP_RESULT_TYPE.SHOP_RESULT_TYPE_NO_ITEM;
                case EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOITEM:
                    return SHOP_RESULT_TYPE.SHOP_RESULT_TYPE_NO_SELL;
            }

            m_saveSys.SaveFile();

            return SHOP_RESULT_TYPE.SHOP_RESULT_TYPE_SUCCESS;
        }

        #region C2S
        //public async void RequestRank(uint nLevelID, OpenNGS.Rank.Common.RANK_DIFFICULT_TYPE _typ)
        //public void RequestBuyItem(BuyItemInfo item)
        //{
        //    /*
        //     *var Item = ItemSystem.Instance.GetItem(item.id);
        //     *var Gold = BagSystem.Instance.GetItem(Item.Gold)
        //     *if(Gold.Count > Item.Gold * item.Count){
        //     *  SendMessage(item);
        //     *}
        //    */

        //    之后会用异步方法获取数据
        //    var rsp = await RankService.Instance.RankRequest(rankId);
        //    OnRankRsp(rsp);
        //}

        //public void RequestSellItem(SellItemInfo item)
        //{
        //    /*
        //     *var Item = ItemSystem.Instance.GetItem(item.id);
        //     *var ItemCount = BagSystem.Instance.GetItem(item.id)
        //     *if(item.Count < ItemCount){
        //     *  SendMessage(item);
        //     *}
        //    */

        //    //之后会用异步方法获取数据
        //    //var rsp = await RankService.Instance.RankRequest(rankId);
        //    //OnRankRsp(rsp);
        //}

        //public void RequestGetShopInfo(int ShopId)
        //{
        //    /*
        //    Shop.Data.GetShopInfoReq req = new GetShopInfoReq();
        //    req.ShopId = (uint)ShopId;
        //    SendMessage(req);
        //    */

        //    //之后会用异步方法获取数据
        //    //var rsp = await RankService.Instance.RankRequest(rankId);
        //    //OnRankRsp(rsp);
        //}

        #endregion

        #region S2C
        // 之后这个函数要更改为private
        //public void OnBuyItemRsp(BuyItemRsq rsp)
        //{
        //    OnBuyItem?.Invoke(rsp);
        //    //模拟服务器数据，并调用UI函数

        //}

        //public void OnSellItemRsp(SellItemRsq rsp)
        //{
        //    OnSellItem?.Invoke(rsp);
        //    //模拟服务器数据，并调用UI函数

        //}

        //public void OnGetShopInfoRsp(GetShopInfoRsq rsp)
        //{
        //    OnGetShopInfo?.Invoke(rsp);
        //    //模拟服务器数据，并调用UI函数

        //}
        #endregion
    }
}
