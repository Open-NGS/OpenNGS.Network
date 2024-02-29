using OpenNGS.Statistic.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRecordSystem
{
    // 单局
    public Dictionary<StatData, ulong> SingleRecor();
    // 全局
    public Dictionary<StatData, ulong> OverallRecor();
}
