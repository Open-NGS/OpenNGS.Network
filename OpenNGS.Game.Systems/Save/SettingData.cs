using OpenNGS.SaveData;
using OpenNGS.Setting.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[global::ProtoBuf.ProtoContract()]

public class SettingData : ISaveEntity
{
    [global::ProtoBuf.ProtoMember(1)]
    public SettingSaveData settingSaveData;
  
    public void Init()
    {
        settingSaveData = new SettingSaveData();
    }

    public void MigrateToVersion(int i)
    {

    }
}

