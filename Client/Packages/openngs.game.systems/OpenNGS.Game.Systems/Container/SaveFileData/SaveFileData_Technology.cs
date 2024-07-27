using OpenNGS.Technology.Data;
using System.Collections.Generic;


[global::ProtoBuf.ProtoContract()]
public class SaveFileData_Technology : ISaveInfo
{
    [global::ProtoBuf.ProtoMember(1)]
    public Dictionary<uint, TechNodeSaveData> nodesSaveData;

    public SaveFileData_Technology()
    {
        nodesSaveData = new Dictionary<uint, TechNodeSaveData>();
    }
}
