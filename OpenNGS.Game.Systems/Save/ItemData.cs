using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

[global::ProtoBuf.ProtoContract()]
public class ItemData : ISaveInfo
{
    [global::ProtoBuf.ProtoMember(1)]
    public Dictionary<long, ItemSaveData> _items;

    public ItemData() 
    { 
        _items = new Dictionary<long, ItemSaveData>();
    }
}
