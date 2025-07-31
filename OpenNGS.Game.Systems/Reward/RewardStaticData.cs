using System.Collections;
using System.Collections.Generic;

namespace OpenNGS.Systems
{
    public static class RewardStaticData
    {
        public static Table<OpenNGS.Reward.Data.Reward, uint> reward = new Table<Reward.Data.Reward, uint>((item) => { return item.Id; }, false);
        public static ListTableBase<OpenNGS.Reward.Data.RewardContent, uint> rewardContent = new ListTableBase<Reward.Data.RewardContent, uint>((item) => { return item.Id; }, false);
        public static Table<OpenNGS.Reward.Data.RewardCondition, uint> rewardCondition = new Table<Reward.Data.RewardCondition, uint>((item) => { return item.Id; }, false);

        public static void Init() { }
    }
}