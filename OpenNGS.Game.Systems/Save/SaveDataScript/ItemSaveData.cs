using System.Collections.Generic;
using UnityEngine;


[global::ProtoBuf.ProtoContract()]
public class ItemSaveData
{
    [global::ProtoBuf.ProtoMember(1)]
    public long GUID;
    [global::ProtoBuf.ProtoMember(2)]
    public int ItemID;
    [global::ProtoBuf.ProtoMember(3)]
    public int Durability;
    [global::ProtoBuf.ProtoMember(4)]
    public int Count;
    [global::ProtoBuf.ProtoMember(5)]
    public int Quality;
}

