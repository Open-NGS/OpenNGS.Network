using OpenNGS;
using OpenNGS.Item.Data;
using OpenNGS.Rank.Common;
using OpenNGS.Reward.Data;
using OpenNGS.Systems;
using System.Collections.Generic;
using Systems;

public class RewardSystem : GameSubSystem<RewardSystem>, IRewardSystem
{
    IItemSystem m_itemSys;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_itemSys = App.GetService<IItemSystem>();
    }

    RewardContainer rewardContainer;

    public override string GetSystemName()
    {
        return "openngs.system.rewardsystem";
    }

    public void AddRewardContainer(RewardContainer container)
    {
        if(container != null)
        {
            rewardContainer = container;
        }
        else
        {
            rewardContainer = new RewardContainer();
        }

        foreach(Reward reward in NGSStaticData.reward.Items)
        {
            RewardSaveData item = rewardContainer.GetRewardById(reward.Id);
            if (item == null)
            {
                item = new RewardSaveData();
                item.Id = reward.Id;
                item.ReceiveCount = 0;
                rewardContainer.AddRewards(item);
            }
        }

    }

    public List<RewardData> GetReward(uint rewardId)
    {
        if (rewardContainer == null) return null;

        Reward reward = NGSStaticData.reward.GetItem(rewardId);
        List<RewardContent> rewardList = NGSStaticData.rewardContent.GetItems(rewardId);
        List<RewardData> reslist = new List<RewardData>();

        if(reward != null &&  rewardList != null )
        {
            foreach (RewardContent item in rewardList)
            {
                RewardData itemData = new RewardData();
                itemData.Id = rewardId;
                itemData.ItemID = item.ItemID;
                itemData.ItemCount = item.ItemCount;
                if (CheckCondition(rewardId, reward.Condition))
                {
                    itemData.Status = OpenNGS.Reward.Common.REWARDSTAT_TYPE.REWARDSTAT_TYPE_AVAILABLE;
                }
                else
                {
                    itemData.Status = OpenNGS.Reward.Common.REWARDSTAT_TYPE.REWARDSTAT_TYPE_UNAVAILABLE;
                }
                reslist.Add(itemData);
            }
            
        }
        return reslist;
    }

    public RESULT_TYPE ReceiveReward(uint rewardId)
    {
        if (rewardContainer == null) return RESULT_TYPE.RESULT_TYPE_NONE;

        RESULT_TYPE resultType = RESULT_TYPE.RESULT_TYPE_FAILED;
        Reward rewardItem = NGSStaticData.reward.GetItem(rewardId);
        if (rewardItem != null)
        {
            if (CheckCondition(rewardId, rewardItem.Condition))
            {
                resultType = RESULT_TYPE.RESULT_TYPE_SUCCESS;
                List<RewardContent> rewardList = NGSStaticData.rewardContent.GetItems(rewardId);
                foreach (RewardContent item in rewardList)
                {
                    m_itemSys.AddItemsByID(item.ItemID, item.ItemCount);
                }
                RewardSaveData data = rewardContainer.GetRewardById(rewardId);
                if (data != null)
                {
                    rewardContainer.UpdateReward(data, ++data.ReceiveCount);
                }
            }
        }

        return resultType;
    }

    private bool CheckCondition(uint rewardId, uint conditionId)
    {
        bool result = false;
        RewardCondition condition = NGSStaticData.rewardCondition.GetItem(conditionId);
        if (condition != null)
        {
            RewardSaveData data = rewardContainer.GetRewardById(rewardId);
            if (data != null)
            {
                if (data.ReceiveCount < condition.ObtainCountLimit)
                {
                    result = true;
                }
            }
        }
        return result;
    }

    protected override void OnClear()
    {
        m_itemSys = null;
        rewardContainer = null;
        base.OnClear();
    }
}

