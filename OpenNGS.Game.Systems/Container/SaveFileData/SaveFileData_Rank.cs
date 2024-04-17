using Dynamic.Data;
using OpenNGS.Character.Common;
using OpenNGS.Systems;
using System;
using System.Collections.Generic;


[global::ProtoBuf.ProtoContract()]
public class SaveFileData_Rank : ISaveInfo
{
    [global::ProtoBuf.ProtoMember(1)]
    public Dictionary<int, RankSaveData> _ranks;

    public SaveFileData_Rank() 
    { 
        _ranks = new Dictionary<int, RankSaveData>();
    }
}
