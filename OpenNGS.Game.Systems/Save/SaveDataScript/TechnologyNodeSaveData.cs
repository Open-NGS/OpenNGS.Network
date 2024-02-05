using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[global::ProtoBuf.ProtoContract()]
public class TechnologyNodeSaveData
{
    [global::ProtoBuf.ProtoMember(1)]
    public uint id;
    [global::ProtoBuf.ProtoMember(2)]
    public uint level;
    [global::ProtoBuf.ProtoMember(3)]
    public bool locked;
    [global::ProtoBuf.ProtoMember(4)]
    public bool activated;
}
