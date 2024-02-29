using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRecordSystem
{
    // 单局
    public Dictionary<uint, Dictionary<string, ulong>> SingleRecor();
    // 全局
    public Dictionary<string, ulong> OverallRecor();
}
