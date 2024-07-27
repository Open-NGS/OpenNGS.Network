using OpenNGS.Levels.Common;
using System.Collections;
using System.Collections.Generic;


namespace OpenNGS.Systems
{
    public static class NGSStaticData
    {
        public static Table<OpenNGS.Shop.Data.Shop, uint> shops = new Table<OpenNGS.Shop.Data.Shop, uint>((item) => { return item.ID; }, false);
        public static Table<OpenNGS.Shop.Data.Shelf,uint, uint> shelfs = new Table<OpenNGS.Shop.Data.Shelf, uint, uint>((item) => { return item.ShopId; }, (item) => { return item.ID; }, false);
        public static Table<OpenNGS.Shop.Data.Good, uint, uint> goods = new Table<OpenNGS.Shop.Data.Good, uint, uint>((item) => { return item.ShelfId; }, (item) => { return item.ID; }, false);
        public static Table<OpenNGS.Technology.Data.NodeData, uint> technologyNodes = new Table<Technology.Data.NodeData, uint>((item) => { return item.ID; }, false);
        public static Table<OpenNGS.Item.Data.Item,uint> items = new Table<Item.Data.Item, uint>((item) => { return item.Id; }, false);
        public static Table<OpenNGS.Item.Data.EquipStats, uint> equipStats = new Table<Item.Data.EquipStats, uint>((item) => { return item.ID; }, false);
        public static Table<OpenNGS.Dialog.Data.DialogTalk, uint> DialogTalk = new Table<OpenNGS.Dialog.Data.DialogTalk, uint>((item) => { return item.DialogTalkID;}, false);
        public static Table<OpenNGS.Dialog.Data.DialogList, uint> Dialogue = new Table<OpenNGS.Dialog.Data.DialogList, uint>((item) => { return item.DialogID; }, false);
        public static Table<OpenNGS.Dialog.Data.DialogChoice, uint> Choice = new Table<OpenNGS.Dialog.Data.DialogChoice, uint>((item) => { return item.DialogChoiceID; }, false);
        public static Table<OpenNGS.Quest.Data.QuestGroup, uint> QuestGroup = new Table<OpenNGS.Quest.Data.QuestGroup, uint>((item) => { return item.QuestGroupID; }, false);
        public static Table<OpenNGS.Quest.Data.Quest, uint> Quest = new Table<OpenNGS.Quest.Data.Quest, uint>((item) => { return item.QuestID; }, false);
        public static Table<OpenNGS.Levels.Data.NGSLevelInfo,uint> levelInfo=new Table<Levels.Data.NGSLevelInfo, uint>((item) => { return item.ID; }, false);
       
        public static Table<OpenNGS.Suit.Data.SuitData,uint> suitInfo=new Table<Suit.Data.SuitData, uint>((item) => { return item.ID; }, false);
        public static Table<OpenNGS.UI.Data.UIConfig,uint>  uiConfig=new Table<UI.Data.UIConfig, uint>((item) => { return item.Id; }, false);
        public static Table<OpenNGS.Enemy.Data.EnemyInfo,uint> enemyInfo=new Table<Enemy.Data.EnemyInfo, uint>((item) => { return item.ID; }, false);
        //public static Table<OpenNGS.Make.Data.ItemInfo, uint> items = new Table<OpenNGS.Make.Data.ItemInfo, uint>((item) => { return item.ID; }, false);
        //public static Table<OpenNGS.Make.Data.MakeInfo, uint> makes = new Table<OpenNGS.Make.Data.MakeInfo, uint>((item) => { return item.ID; }, false);
        public static Table<OpenNGS.Shop.Data.ShopSell, uint, uint> sells = new Table<OpenNGS.Shop.Data.ShopSell, uint, uint>((item) => { return item.ShopID; }, (item) => { return item.ItemID; }, false);
        public static Table<OpenNGS.Statistic.Data.StatData,uint> s_statDatas = new Table<Statistic.Data.StatData, uint>((item) => { return item.Id; }, false);
        public static Table<OpenNGS.Achievement.Data.Achievement, uint> s_achiDatas = new Table<Achievement.Data.Achievement, uint>((item) => { return item.ID; }, false);
        public static Table<OpenNGS.Item.Data.MakeDesign, uint> MakeItems = new Table<Item.Data.MakeDesign, uint>((item) => { return item.ID; }, false);
        public static Table<OpenNGS.Item.Data.ComposeCostInfo, uint> ComposeInfo = new Table<Item.Data.ComposeCostInfo, uint>((item) => { return item.ID; }, false);
        public static Dictionary<uint,OpenNGS.Item.Data.ComposeCostInfo> ComposeInfos = SettingTable<OpenNGS.Item.Data.ComposeCostInfo,uint>.map;
        public static Table<OpenNGS.Item.Data.ComposeCostInfo, uint, uint> composeCostInfo = new Table<Item.Data.ComposeCostInfo, uint, uint>((item) => { return (uint)item.Kind; }, (item) => { return (uint)item.Quality; }, false);
        public static ListTableBase<OpenNGS.Achievement.Data.AchievementAward, uint> s_achiAward = new ListTableBase<Achievement.Data.AchievementAward, uint>((item) => { return item.ID; }, false);
        public static Table<OpenNGS.Levels.Data.LevelEnemyInfo, uint, uint> levelEnemyInfo = new Table<Levels.Data.LevelEnemyInfo, uint, uint>((item) => { return item.ID; }, (item) => { return item.RuleID; }, false);
        public static ListTableBase<OpenNGS.Levels.Data.LevelEnemyInfo,uint> levelEnemyInfos=new ListTableBase<Levels.Data.LevelEnemyInfo, uint>((item) => { return item.ID; }, false);
        public static Table<OpenNGS.Reward.Data.Reward, uint> reward = new Table<Reward.Data.Reward, uint> ((item) => { return item.Id; }, false);
        public static ListTableBase<OpenNGS.Reward.Data.RewardContent, uint> rewardContent = new ListTableBase<Reward.Data.RewardContent, uint> ((item) => { return item.Id; }, false);
        public static Table<OpenNGS.Reward.Data.RewardCondition, uint> rewardCondition = new Table<Reward.Data.RewardCondition, uint> ((item) => { return item.Id; }, false);
        public static Table<OpenNGS.Statistic.Data.StatData, uint> statisticItems = new Table<OpenNGS.Statistic.Data.StatData, uint>((item) => { return item.Id; }, false);

        public static void Init() { }
    }
}
