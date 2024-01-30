using OpenNGS.Rank.Data;
using OpenNGS.SaveData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[global::ProtoBuf.ProtoContract()]
public class SaveData : ISaveEntity
{
    [global::ProtoBuf.ProtoMember(1)]
    public ItemData saveItems;
    [global::ProtoBuf.ProtoMember(2)]
    public RankData saveRanks;

    public void Init()
    {
        saveItems = new ItemData();
        saveRanks = new RankData();
    }

    public void MigrateToVersion(int i)
    {
        throw new System.NotImplementedException();
    }
}

