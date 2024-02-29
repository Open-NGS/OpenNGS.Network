using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public static Table<OpenNGS.Dialog.Data.DialogTalk, uint> DialogTalk = new Table<OpenNGS.Dialog.Data.DialogTalk, uint>((item) => { return item.DialogueDataID;}, false);
        public static Table<OpenNGS.Dialog.Data.Dialogue, uint> Dialogue = new Table<OpenNGS.Dialog.Data.Dialogue, uint>((item) => { return item.DialogueID; }, false);
        public static Table<OpenNGS.Levels.Data.NGSLevelInfo,uint> levelInfo=new Table<Levels.Data.NGSLevelInfo, uint>((item) => { return item.ID; }, false);
        public static Table<OpenNGS.Levels.Data.LevelStageData, uint> levelData = new Table<Levels.Data.LevelStageData, uint>((item) => { return item.LevelID; }, false);
        public static Table<OpenNGS.Levels.Data.ConditionData, uint> conditionData = new Table<Levels.Data.ConditionData, uint>((item) => { return item.ConditionID; }, false);
        public static Table<OpenNGS.Suit.Data.SuitData,uint> suitInfo=new Table<Suit.Data.SuitData, uint>((item) => { return item.ID; }, false);
        public static Table<OpenNGS.UI.Data.UIConfig,uint>  uiConfig=new Table<UI.Data.UIConfig, uint>((item) => { return item.Id; }, false);
        public static Table<OpenNGS.Enemy.Data.EnemyInfo,uint> enemyInfo=new Table<Enemy.Data.EnemyInfo, uint>((item) => { return item.ID; }, false);
        //public static Table<OpenNGS.Make.Data.ItemInfo, uint> items = new Table<OpenNGS.Make.Data.ItemInfo, uint>((item) => { return item.ID; }, false);
        //public static Table<OpenNGS.Make.Data.MakeInfo, uint> makes = new Table<OpenNGS.Make.Data.MakeInfo, uint>((item) => { return item.ID; }, false);
        public static Table<OpenNGS.Shop.Data.ShopSell, uint, uint> sells = new Table<OpenNGS.Shop.Data.ShopSell, uint, uint>((item) => { return item.ShopID; }, (item) => { return item.ItemID; }, false);
        public static Table<OpenNGS.Statistic.Data.StatData,uint> s_statDatas = new Table<Statistic.Data.StatData, uint>((item) => { return item.Id; }, false);
        public static Table<OpenNGS.Achievement.Data.Achievement, uint> s_achiDatas = new Table<Achievement.Data.Achievement, uint>((item) => { return item.ID; }, false);
        public static Table<OpenNGS.Item.Data.MakeDesign, uint> MakeItems = new Table<Item.Data.MakeDesign, uint>((item) => { return item.ID; }, false);
        public static ListTableBase<OpenNGS.Achievement.Data.AchievementAward, uint> s_achiAward = new ListTableBase<Achievement.Data.AchievementAward, uint>((item) => { return item.ID; }, false);
        public static Table<OpenNGS.HandBook.Data.HandBook, uint> s_handBook = new Table<HandBook.Data.HandBook, uint> ((item) => { return item.GroupID; }, false);
        
        public static void Init() { }
    }
}
