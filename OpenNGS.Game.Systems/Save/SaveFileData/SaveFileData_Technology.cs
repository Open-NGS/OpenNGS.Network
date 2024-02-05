using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[global::ProtoBuf.ProtoContract()]
public class SaveFileData_Technology : ISaveInfo
{
    [global::ProtoBuf.ProtoMember(1)]
    public Dictionary<uint, TechnologyNodeSaveData> nodesSaveData;

    public SaveFileData_Technology()
    {
        nodesSaveData = new Dictionary<uint, TechnologyNodeSaveData>();
    }
}
