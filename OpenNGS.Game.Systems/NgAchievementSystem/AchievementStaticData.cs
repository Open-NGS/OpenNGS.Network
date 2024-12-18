
namespace OpenNGS.Systems
{
    public static class AchievementStaticData
    {
        public static Table<OpenNGS.Achievement.Data.Achievement, uint> achievement = new Table<OpenNGS.Achievement.Data.Achievement, uint>((item) => { return item.ID; }, false);
        public static Table<OpenNGS.Achievement.Data.AchievementAward, uint> achievementAward = new Table<OpenNGS.Achievement.Data.AchievementAward, uint>((item) => { return item.ID; }, false);

        public static void Init() { }
    }
}