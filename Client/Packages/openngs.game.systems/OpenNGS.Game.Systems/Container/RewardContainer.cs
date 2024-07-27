using System.Linq;

namespace OpenNGS.Reward.Data
{
    public partial class RewardContainer
    {

        public void AddRewards(RewardSaveData reward)
        {
            rewardDict.Add(reward);
        }
        public void RemoveRewards(RewardSaveData reward)
        {
            rewardDict.Remove(reward);
        }
        public void UpdateReward(RewardSaveData reward, uint num)
        {
            reward.ReceiveCount = num;
        }
        public RewardSaveData GetRewardById(uint rewardId)
        {
            return rewardDict.FirstOrDefault(i => i.Id == rewardId);
        }

    }
}