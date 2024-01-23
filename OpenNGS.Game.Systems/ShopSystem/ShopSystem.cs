using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNGS.Rank.Data;
using OpenNGS.Shop.Data;
using UnityEngine.Events;

namespace OpenNGS.Systems
{
    public class ShopSystem : EntitySystem
    {
        public UnityAction<BuyItemRsq> OnBuyItem;
        public UnityAction<SellItemRsq> OnSellItem;
        public UnityAction<GetShopInfoRsq> OnGetShopInfo;
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        public override string GetSystemName()
        {
            return "com.openngs.system.rank";
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
