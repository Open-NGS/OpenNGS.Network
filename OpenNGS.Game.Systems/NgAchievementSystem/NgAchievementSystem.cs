using OpenNGS;
using OpenNGS.Achievement.Common;
using OpenNGS.Achievement.Data;
using OpenNGS.Achievement.Service;
using OpenNGS.ERPC;
using OpenNGS.Systems;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;


public class NgAchievementSystem : Singleton<NgAchievementSystem>, INgAchievementSystem
{

    public List<AchievementState> achievementStates { get; set; }
    private Dictionary<uint, List<uint>> parentAchievementMap = new Dictionary<uint, List<uint>>();


    public void AddAchievementStates(List<AchievementState> AchievementStates = null)
    {
        if (AchievementStates != null)
        {
            achievementStates = AchievementStates;
        }
        else
        {
            achievementStates = new List<AchievementState>();
        }

        foreach (var achievement in AchievementStaticData.achievement.Items)
        {
            if (achievement.SubAchievement != null)
            {
                foreach (var subID in achievement.SubAchievement)
                {
                    if (!parentAchievementMap.ContainsKey(subID))
                    {
                        parentAchievementMap[subID] = new List<uint>();
                    }
                    parentAchievementMap[subID].Add(achievement.ID);
                }
            }
        }

    }
    public Task<GetAchievementRewardRsp> GetAchievementReward(GetAchievementRewardReq value, ClientContext context = null)
    {
        GetAchievementRewardRsp rsp = new GetAchievementRewardRsp();

        AchievementState achievementState = achievementStates
            .Find(a => a.ID == value.ID);
        if (achievementState != null)
        {
            if(achievementState.Status == Achievement_Status.Achievement_Status_Pending)
            {
                achievementState.Status = Achievement_Status.Achievement_Status_Done;
                Achievement parentAchievement = AchievementStaticData.achievement.GetItem(value.ID);
                foreach (var subID in parentAchievement.SubAchievement)
                {
                    var subState = achievementStates.Find(a => a.ID == subID);
                    if (subState == null || subState.Status == Achievement_Status.Achievement_Status_Stating)
                    {
                        subState.Status = Achievement_Status.Achievement_Status_Done;
                    }
                }
                rsp.result = Achievement_Result.AchievementResult_Success;
            }
            else if (achievementState.Status == Achievement_Status.Achievement_Status_Stating)
            {
                rsp.result = Achievement_Result.Achievement_Result_None;
            }
            else if (achievementState.Status == Achievement_Status.Achievement_Status_Done)
            {
                rsp.result = Achievement_Result.AchievementResult_Fail_HasGet;
            }
            else
            {
                rsp.result = Achievement_Result.Achievement_Result_None;
            }
        }
        else
        {
            rsp.result = Achievement_Result.AchievementResult_Fail_NotExist;
        }
        return Task.FromResult(rsp);
    }

    public Task<UpdateAchievementRsp> UpdateAchievement(UpdateAchievementReq value, ClientContext context = null)
    {
        UpdateAchievementRsp updateAchievementRsp = new UpdateAchievementRsp();
        updateAchievementRsp.result = Achievement_Result.Achievement_Result_None;

        AchievementState achievementState = achievementStates
            .Find(a => a.ID == value.ID);
        Achievement achievement = AchievementStaticData.achievement.GetItem(value.ID);
        if (achievementState != null)
        {
            achievementState.Progress = value.Progress;
        }
        else
        {
            // 没找到对应成就，添加新的成就状态
            achievementState = new AchievementState
            {
                PlayerID = value.PlayerID,
                ID = value.ID,
                Progress = value.Progress,
                Status = Achievement_Status.Achievement_Status_Stating,
            };
            achievementStates.Add(achievementState);
        }
        if (achievementState.Progress >= achievement.StatValue)
        {
            achievementState.Status = Achievement_Status.Achievement_Status_Pending;
            //时间

            CheckAndUpdateParentAchievements(value.ID, value.PlayerID);
        }
        updateAchievementRsp.result = Achievement_Result.AchievementResult_Success;
        return Task.FromResult(updateAchievementRsp);
    }
    private void CheckAndUpdateParentAchievements(uint subAchievementID,uint player)
    {
        if (!parentAchievementMap.ContainsKey(subAchievementID))
            return;

        List<uint> parentAchievements = parentAchievementMap[subAchievementID];
        foreach (var parentID in parentAchievements)
        {
            Achievement parentAchievement = AchievementStaticData.achievement.GetItem(parentID);
            AchievementState parentState = achievementStates.Find(a => a.ID == parentID);

            if (parentAchievement == null)
                continue;

            if (parentState == null)
            {
                parentState = new AchievementState
                {
                    PlayerID = player,
                    ID = parentID,
                    Progress = 0,
                    Status = Achievement_Status.Achievement_Status_Stating,
                };
                achievementStates.Add(parentState);
            }

            bool allSubAchievementsCompleted = true;
            foreach (var subID in parentAchievement.SubAchievement)
            {
                var subState = achievementStates.Find(a => a.ID == subID);
                if (subState == null || subState.Status == Achievement_Status.Achievement_Status_Stating)
                {
                    allSubAchievementsCompleted = false;
                    break;
                }
            }
            if (allSubAchievementsCompleted)
            {
                parentState.Status = Achievement_Status.Achievement_Status_Done;
                //时间

            }
        }
    }

    public Task<GetAchievementsRsp> GetAchievements(GetAchievementsReq value, ClientContext context = null)
    {
        var response = new GetAchievementsRsp
        {
            achievementStates = achievementStates
        };
        return Task.FromResult(response);
    }
}

