using OpenNGS;
using OpenNGS.Systems;
using System.Collections;
using System.Collections.Generic;
using Systems;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class RecordSystem : GameSubSystem<RecordSystem>, IRecordSystem
{
    IStatSystem stat = null;
    List<OpenNGS.Levels.Data.NGSLevelInfo> levelStages = NGSStaticData.levelInfo.Items;
    List<OpenNGS.Statistic.Data.StatData> stats = NGSStaticData.s_statDatas.Items;

    Dictionary<string, ulong> SingleData = new Dictionary<string, ulong>();
    Dictionary<uint, Dictionary<string, ulong>> LevelData = new Dictionary<uint, Dictionary<string, ulong>>();

    Dictionary<string, ulong> OverallData = new Dictionary<string, ulong>();

    protected override void OnCreate()
    {
        stat = App.GetService<IStatSystem>();
        base.OnCreate();
    }

    // 单局
    public Dictionary<uint, Dictionary<string, ulong>> SingleRecor()
    {
        for (int i = 0; i < levelStages.Count; i++)
        {
            for (int j = 0; j < stats.Count; j++)
            {
                if (levelStages[i].ID == stats[j].ObjCategory)
                {
                    ulong data;
                    stat.GetStatValueByID(stats[j].Id, out data);
                    SingleData.Add(stats[j].Description, data);
                    LevelData.Add(stats[j].Id, SingleData);
                }
            }
        }
        return LevelData;
    }

    // 全局
    public Dictionary<string, ulong> OverallRecor()
    {
        for (int i = 0; i < stats.Count; i++)
        {
            if (stats[i].ObjCategory == 0)
            {
                ulong data;
                stat.GetStatValueByID(stats[i].Id, out data);
                OverallData.Add(stats[i].Description,data);
            }
        }
        return OverallData;
    }
    public override string GetSystemName()
    {
        throw new System.NotImplementedException();
    }
}
