using OpenNGS.SaveData;
using OpenNGS.Setting.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[global::ProtoBuf.ProtoContract()]

public class SaveSettingData : ISaveEntity
{
    [global::ProtoBuf.ProtoMember(1)]
    public SaveFileData_Setting settingSaveData;
  
    public void Init()
    {
        settingSaveData = new SaveFileData_Setting();
    }

    public void MigrateToVersion(int i)
    {

    }
}

