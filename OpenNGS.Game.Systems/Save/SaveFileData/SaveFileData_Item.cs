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

    public SaveFileData_Item() 
    { 
        _items = new Dictionary<long, ItemSaveData>();
    }
}
