using OpenNGS.Rank.Data;
using OpenNGS.SaveData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Systems
{
    [global::ProtoBuf.ProtoContract()]
    public class SaveData : ISaveEntity
    {
        [global::ProtoBuf.ProtoMember(1)]
        public ItemData saveItems;
        [global::ProtoBuf.ProtoMember(2)]
        public RankData saveRanks;
        [global::ProtoBuf.ProtoMember(3)]
        public CharacterSaveData charaInfos;
        public void Init()
        {
            saveItems = new ItemData();
            saveRanks = new RankData();
            charaInfos = new CharacterSaveData();
        }

        public void MigrateToVersion(int i)
        {
        }
    }


}