using OpenNGS.Rank.Data;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

namespace OpenNGS.Systems
{
    public class RankSystem : EntitySystem
    {
        public UnityAction<GetRankRsq> OnGetRank;
        public UnityAction<List<RankInfo>> OnGetRankList;
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
        public void RequestRank(uint nLevelID, OpenNGS.Rank.Common.RANK_DIFFICULT_TYPE _typ)
        {
            //之后会用异步方法获取数据
            //var rsp = await RankService.Instance.RankRequest(rankId);
            //OnRankRsp(rsp);
        }

        #endregion

        #region S2C
        // 之后这个函数要更改为private
        public void OnRankRsp(GetRankRsq rsp)
        {
            OnGetRank?.Invoke(rsp);
            //模拟服务器数据，并调用UI函数
            List<RankInfo> lst = new List<RankInfo>();
            OnGetRankList?.Invoke(lst);
        }
        #endregion
    }

}