using System.Collections;
using System.Collections.Generic;

namespace OpenNGS.Systems
{
    public static class LevelStaticData
    {
        public static Table<OpenNGS.Levels.Data.NGSLevelInfo, uint> levelInfo = new Table<Levels.Data.NGSLevelInfo, uint>((item) => { return item.ID; }, false);
        public static Table<OpenNGS.Levels.Data.LevelEnemyInfo, uint, uint> levelEnemyInfo = new Table<Levels.Data.LevelEnemyInfo, uint, uint>((item) => { return item.ID; }, (item) => { return item.RuleID; }, false);
        public static ListTableBase<OpenNGS.Levels.Data.LevelEnemyInfo, uint> levelEnemyInfos = new ListTableBase<Levels.Data.LevelEnemyInfo, uint>((item) => { return item.ID; }, false);

        public static void Init() { }
    }
}