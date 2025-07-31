using OpenNGS;
using OpenNGS.Achievement.Common;
using OpenNGS.Achievement.Data;
using OpenNGS.Achievement.Service;
using OpenNGS.Collection.Service;
using OpenNGS.Core;
using OpenNGS.ERPC;
using OpenNGS.ERPC.Configuration;
using OpenNGS.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

public class AchievementAPIService : Singleton<AchievementAPIService>, INiAchievementService
{
    public async Task<GetAchievementRewardRsp> GetAchievementReward(GetAchievementRewardReq value, ClientContext context = null)
    {
        return await AchievementAPIController.Instance.GetAchievementReward(value, context);
    }

    public async Task<GetAchievementsRsp> GetAchievements(GetAchievementsReq value, ClientContext context = null)
    {
        await Task.Delay(100);

        return await AchievementAPIController.Instance.GetAchievements(value, context);
    }

    public async Task<UpdateAchievementRsp> UpdateAchievement(UpdateAchievementReq value, ClientContext context = null)
    {
        await Task.Delay(100);

        return await AchievementAPIController.Instance.UpdateAchievement(value, context);
    }
}

