using System;
using System.Collections.Generic;
using OpenNGS.Exchange.Data;
using OpenNGS.Shop.Common;
using OpenNGS.Shop.Data;
using UnityEngine.Events;

namespace OpenNGS.Systems
{
    public class ShopSystem : EntitySystem
    {
        public UnityAction<BuyItemRsq> OnBuyItem;
        public UnityAction<SellItemRsq> OnSellItem;
        public UnityAction<GetShopInfoRsq> OnGetShopInfo;

        private IExchangeSystem m_exchangeSys = null;
        private IItemSystem m_itemSys = null;
        protected override void OnCreate()
        {
            base.OnCreate();
        }
        public void RegisteItemSystem(IExchangeSystem _exchangeSys)
        {
            m_exchangeSys = _exchangeSys;
        }

        public void RegisteItemSystem(IItemSystem _itemSys)
        {
            m_itemSys = _itemSys;
        }

        public override string GetSystemName()
        {
            return "com.openngs.system.rank";
        }

        /// <summary>
        /// 获取当前开放的货架id
        /// </summary>
        private uint GetShelfID(uint shopID)
        {
            uint shelfID = 0;
            return shelfID;
        }

        /// <summary>
        /// 检查商店内是否有该商品
        /// </summary>
        private bool CheckShopItem(uint shopId, uint shopItemId)
        {
            Good shopItemInfo = NGSStaticData.goods.GetItem(shopItemId);
            uint shelfId = shopItemInfo.ShelfId;
            //验证shelf是否是开启状态
            if(shelfId != GetShelfID(shopId)) 
            { 
                return false; 
            }

            Shelf shelf = NGSStaticData.shelfs.GetItem(GetShelfID(shopId));
            OpenNGS.Shop.Data.Shop shop = NGSStaticData.shops.GetItem(shelf.ShopId);

            if(shopId != shop.ID)
            {  
                return false; 
            }

            return true;
        }

        public SHOP_RESULT_TYPE BuyItem(BuyItemInfo item)
        {
            if (item == null) return SHOP_RESULT_TYPE.SHOP_RESULT_TYPE_NO_ITEM;
            //根据shopItemID获取商品信息
            if (!CheckShopItem(item.ShopId, item.ShopItemId)) return SHOP_RESULT_TYPE.SHOP_RESULT_TYPE_ERROR_ITEM;

            Good good = NGSStaticData.goods.GetItem(item.ShopItemId);

            List<SourceItem> sourceItems = new List<SourceItem>();
            List<TargetItem> targetItems = new List<TargetItem>();
            
            uint id = m_itemSys.GetGuidByItemID(good.PriceItemID);
            SourceItem sourceItem = new SourceItem();
            sourceItem.GUID = id;
            sourceItem.Count = good.PriceItemCount * item.ShopItemCount;
            sourceItems.Add(sourceItem);

            TargetItem targetItem = new TargetItem();
            targetItem.ItemID = good.ItemId;
            targetItem.Count = item.ShopItemCount;
            targetItems.Add(targetItem);

            m_exchangeSys.ExchangeItem(sourceItems, targetItems);

            return SHOP_RESULT_TYPE.SHOP_RESULT_TYPE_SUCCESS;
        }


        public SHOP_RESULT_TYPE SellItem(SellItemInfo item)
        {
            if (item == null) return SHOP_RESULT_TYPE.SHOP_RESULT_TYPE_NO_ITEM;

            if (!CheckSellInfo(item)) return SHOP_RESULT_TYPE.SHOP_RESULT_TYPE_NO_SELL;

            Good good = NGSStaticData.goods.GetItem(item.ItemId);

            List<SourceItem> sourceItems = new List<SourceItem>();
            List<TargetItem> targetItems = new List<TargetItem>();

            SourceItem sourceItem = new SourceItem();
            sourceItem.GUID = item.GUID;
            sourceItem.Count = item.ShopItemCount;
            sourceItems.Add(sourceItem);

            uint id = m_itemSys.GetGuidByItemID(good.PriceItemID);
            TargetItem targetItem = new TargetItem();
            targetItem.ItemID = id;
            targetItem.Count = good.PriceItemCount * item.ShopItemCount;
            targetItems.Add(targetItem);

            m_exchangeSys.ExchangeItem(sourceItems, targetItems);

            return SHOP_RESULT_TYPE.SHOP_RESULT_TYPE_SUCCESS;
        }

        private bool CheckSellInfo(SellItemInfo item)
        {
            List<ShopSell> sellitems = NGSStaticData.sells.GetItems(item.ShopId);

            foreach(ShopSell sell in sellitems)
            {
                if(sell.ItemID == item.ItemId) return true;
            }
            return false;
        }

        public List<Good> GetShopInfo(uint ShopId)
        {
            uint shelfID = GetShelfID(ShopId);
            if(shelfID == 0) return null;
            List<Good> result = NGSStaticData.shelfGoods.GetItems(shelfID);
            return result;
        }

        #region C2S
        //public async void RequestRank(uint nLevelID, OpenNGS.Rank.Common.RANK_DIFFICULT_TYPE _typ)
        public void RequestBuyItem(BuyItemInfo item)
        {
            /*
             *var Item = ItemSystem.Instance.GetItem(item.id);
             *var Gold = BagSystem.Instance.GetItem(Item.Gold)
             *if(Gold.Count > Item.Gold * item.Count){
             *  SendMessage(item);
             *}
            */

            //之后会用异步方法获取数据
            //var rsp = await RankService.Instance.RankRequest(rankId);
            //OnRankRsp(rsp);
        }

        public void RequestSellItem(SellItemInfo item)
        {
            /*
             *var Item = ItemSystem.Instance.GetItem(item.id);
             *var ItemCount = BagSystem.Instance.GetItem(item.id)
             *if(item.Count < ItemCount){
             *  SendMessage(item);
             *}
            */

            //之后会用异步方法获取数据
            //var rsp = await RankService.Instance.RankRequest(rankId);
            //OnRankRsp(rsp);
        }

        public void RequestGetShopInfo(int ShopId)
        {
            /*
            Shop.Data.GetShopInfoReq req = new GetShopInfoReq();
            req.ShopId = (uint)ShopId;
            SendMessage(req);
            */

            //之后会用异步方法获取数据
            //var rsp = await RankService.Instance.RankRequest(rankId);
            //OnRankRsp(rsp);
        }

        #endregion

        #region S2C
        // 之后这个函数要更改为private
        public void OnBuyItemRsp(BuyItemRsq rsp)
        {
            OnBuyItem?.Invoke(rsp);
            //模拟服务器数据，并调用UI函数
            
        }

        public void OnSellItemRsp(SellItemRsq rsp)
        {
            OnSellItem?.Invoke(rsp);
            //模拟服务器数据，并调用UI函数
            
        }

        public void OnGetShopInfoRsp(GetShopInfoRsq rsp)
        {
            OnGetShopInfo?.Invoke(rsp);
            //模拟服务器数据，并调用UI函数
            
        }
        #endregion
    }
}
