using OpenNGS;
using OpenNGS.Statistic.Data;
using OpenNGS.Systems;
using System.Collections;
using System.Collections.Generic;
using Systems;

public class RecordSystem : GameSubSystem<RecordSystem>, IRecordSystem
{
    List<OpenNGS.Levels.Data.NGSLevelInfo> levelStages = LevelStaticData.levelInfo.Items;
    //List<StatData> stats = NGSStaticData.s_statDatas.Items;

    //Dictionary<StatData, ulong> SingleData = new Dictionary<StatData, ulong>();

    //Dictionary<StatData, ulong> OverallData = new Dictionary<StatData, ulong>();

    protected override void OnCreate()
    {
        base.OnCreate();
    }

    // 单局
    //public Dictionary<StatData, ulong> SingleRecor()
    //{
    //    SingleData.Clear();
    //    for (int i = 0; i < levelStages.Count; i++)
    //    {
    //        for (int j = 0; j < stats.Count; j++)
    //        {
    //            if (levelStages[i].ID == stats[j].ObjCategory && stats[j].Id > 2)
    //            {
    //                ulong data;
    //            }
    //        }
    //    }
    //    return SingleData;
    //}

    // 全局
    //public Dictionary<StatData, ulong> OverallRecor()
    //{
    //    OverallData.Clear();
    //    for (int i = 0; i < stats.Count; i++)
    //    {
    //        if (stats[i].ObjCategory == 0)
    //        {
    //            ulong data;
    //        }
    //    }
    //    return OverallData;
    //}
    public override string GetSystemName()
    {
        return "com.openngs.system.record";
    }
}
