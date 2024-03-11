using OpenNGS.HandBook.Common;
using System.Collections;
using System.Collections.Generic;


[global::ProtoBuf.ProtoContract()]
public class SaveFileData_HandBook : ISaveInfo
{
    [global::ProtoBuf.ProtoMember(1)]
    public Dictionary<uint, HandBookInfo> DicHandBook;

    public SaveFileData_HandBook()
    {
        DicHandBook = new Dictionary<uint, HandBookInfo>();
    }
}
