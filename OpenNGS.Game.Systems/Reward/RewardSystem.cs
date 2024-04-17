using OpenNGS;
using OpenNGS.Rank.Common;
using OpenNGS.Reward.Data;
using OpenNGS.Systems;
using System.Collections.Generic;
using Systems;

public class RewardSystem : GameSubSystem<RewardSystem>, IRewardSystem
{
    IItemSystem m_itemSys;
    //ISaveSystem m_saveSys;
    private SaveFileData_Reward m_reward;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_itemSys = App.GetService<IItemSystem>();
        //m_saveSys = App.GetService<ISaveSystem>();

        //ISaveInfo saveInfo = m_saveSys.GetFileData("REWARD");
        //if (saveInfo != null && saveInfo is SaveFileData_Reward)
        //{
        //    m_reward = (SaveFileData_Reward)saveInfo;
        //}
        //else
        //{
        //    m_reward = new SaveFileData_Reward();
        //}

        //foreach (Reward reward in NGSStaticData.reward.Items)
        //{
        //    if (m_reward.DicReward.TryGetValue(reward.Id, out RewardSaveData data) == false)
        //    {
        //        m_reward.DicReward[reward.Id] = new RewardSaveData();
        //        m_reward.DicReward[reward.Id].Id = reward.Id;
        //        m_reward.DicReward[reward.Id].ReceiveCount = 0;
        //    }
        //}
    }

    public override string GetSystemName()
    {
        return "openngs.system.rewardsystem";
    }

    public List<RewardData> GetReward(uint rewardId)
    {
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
                //SaveFileData_Reward saveItem = m_saveSys.GetFileData("REWARD") as SaveFileData_Reward;
                if (m_reward.DicReward.TryGetValue(rewardId, out RewardSaveData data))
                {
                    data.ReceiveCount++;
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
            //SaveFileData_Reward rewardItem = m_saveSys.GetFileData("REWARD") as SaveFileData_Reward;
            if (m_reward.DicReward.TryGetValue(rewardId, out RewardSaveData data))
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
        base.OnClear();
    }
}

