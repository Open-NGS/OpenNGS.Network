using OpenNGS.Achievement.Common;
using OpenNGS.Exchange.Common;
using System.Collections;
using System.Collections.Generic;

namespace OpenNGS.Systems
{
    // todo，是否可以放入静态数据表中？
    public enum StatEventNotify
    {
        StatEventNotify_None = 0,
        StatEventNotify_Update = 1,
    }
    public interface IAchievementSystem
    {
        public OpenNGS.Achievement.Common.ACHIEVEMENT_STATUS GetAchievementStatus(uint nAchieID);
        public ACHIEVEMENT_RESULT GetAchievementReward(uint nAchieID);
        public void UpdateAchievementIfNeed();
        public Dictionary<uint, AchievementInfo> GetAchievementData();
    }
}