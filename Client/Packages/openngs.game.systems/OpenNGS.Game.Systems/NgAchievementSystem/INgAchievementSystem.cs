using OpenNGS.Achievement.Common;
using OpenNGS.Achievement.Data;
using OpenNGS.Achievement.Service;
using OpenNGS.ERPC;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenNGS.Systems
{
    public interface INgAchievementSystem
    {
        public void AddAchievementStates(List<AchievementState> AchievementStates = null);
        public Task<GetAchievementRewardRsp> GetAchievementReward(GetAchievementRewardReq value, ClientContext context = null);
        public Task<UpdateAchievementRsp> UpdateAchievement(UpdateAchievementReq value, ClientContext context = null);
        public Task<GetAchievementsRsp> GetAchievements(GetAchievementsReq value, ClientContext context = null);
    }
}
