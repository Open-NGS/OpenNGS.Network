using OpenNGS.Common;
using OpenNGS.Reward.Data;
using System.Collections.Generic;


public interface IRewardSystem
{
    public List<RewardData> GetReward(uint rewardId);
    public RESULT_TYPE ReceiveReward(uint rewardId);
    public void AddRewardContainer(RewardContainer container);
}

