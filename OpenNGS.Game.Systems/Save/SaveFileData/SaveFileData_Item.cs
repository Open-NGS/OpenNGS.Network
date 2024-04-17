using OpenNGS.Item.Data;
using System.Collections.Generic;


[global::ProtoBuf.ProtoContract()]
public class SaveFileData_Item : ISaveInfo
{
    [global::ProtoBuf.ProtoMember(1)]
    public Dictionary<long, ItemSaveData> _items;
    [global::ProtoBuf.ProtoMember(2)]
    public Dictionary<long, ItemSaveData> _equips;

    public SaveFileData_Item() 
    { 
        _items = new Dictionary<long, ItemSaveData>();
        _equips = new Dictionary<long, ItemSaveData>();
    }
}
