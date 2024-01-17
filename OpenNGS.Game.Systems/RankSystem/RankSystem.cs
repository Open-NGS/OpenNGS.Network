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
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        public override string GetSystemName()
        {
            return "com.openngs.system.rank";
        }

        #region C2S
        public async void RequestRank(uint nLevelID, OpenNGS.Rank.Common.RANK_DIFFICULT_TYPE _typ)
        {
            //var rsp = await RankService.Instance.RankRequest(rankId);
            var rsp = new GetRankRsq();
            OnRankRsp(rsp);
        }

        #endregion

        #region S2C
        private void OnRankRsp(GetRankRsq rsp)
        {
            OnGetRank?.Invoke(rsp);
        }
        #endregion
    }

}