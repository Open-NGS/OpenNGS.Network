using OpenNGS.Character.Common;
using OpenNGS.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

[global::ProtoBuf.ProtoContract()]
public class RankData : ISaveInfo
{
    [global::ProtoBuf.ProtoMember(1)]
    public Dictionary<int, RankSaveData> _ranks;

    public RankData() 
    { 
        _ranks = new Dictionary<int, RankSaveData>();
    }
}
