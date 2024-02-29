using Dynamic.Data;
using OpenNGS.Systems;
using OpenNGSCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
