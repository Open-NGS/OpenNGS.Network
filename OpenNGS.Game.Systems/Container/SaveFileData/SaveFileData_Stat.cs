using Dynamic.Data;
using OpenNGS.Statistic.Common;
using System.Collections;
using System.Collections.Generic;


[global::ProtoBuf.ProtoContract()]
public class SaveFileData_Stat : ISaveInfo
{
    [global::ProtoBuf.ProtoMember(1)]
    public Dictionary<STAT_EVENT, List<OpenNGS.Statistic.Common.StatValue>> DicStatValue;

    public SaveFileData_Stat()
    {
        DicStatValue = new Dictionary<STAT_EVENT, List<StatValue>>();
    }
}
