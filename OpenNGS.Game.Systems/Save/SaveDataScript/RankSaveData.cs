using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[global::ProtoBuf.ProtoContract()]
public class RankSaveData
{
    [global::ProtoBuf.ProtoMember(1)]
    public int index;
    [global::ProtoBuf.ProtoMember(2)]
    public CharacterInfo character;
    [global::ProtoBuf.ProtoMember(3)]
    public uint cleartime;
}
