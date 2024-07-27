using Dynamic.Data;
using OpenNGS.Character.Common;
using System.Collections.Generic;

[global::ProtoBuf.ProtoContract()]

public class SaveFileData_Dialog : ISaveInfo
{
    [global::ProtoBuf.ProtoMember(1)]
    public DialogSaveData _Dialog;

    public SaveFileData_Dialog()
    {
        _Dialog = new DialogSaveData();
    }
}

