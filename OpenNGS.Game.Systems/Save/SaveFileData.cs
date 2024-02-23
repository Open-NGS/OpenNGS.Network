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
    public class SaveFileData : ISaveEntity
    {
        [global::ProtoBuf.ProtoMember(1)]
        public SaveFileData_Item saveItems;
        [global::ProtoBuf.ProtoMember(2)]
        public SaveFileData_Rank saveRanks;
        [global::ProtoBuf.ProtoMember(3)]
        public SaveFileData_Character charaInfos;
        [global::ProtoBuf.ProtoMember(4)]
        public SaveFileData_Dialog dialogData;
        [global::ProtoBuf.ProtoMember(5)]
        public SaveFileData_Technology technologyData;
        [global::ProtoBuf.ProtoMember(6)]
        public SaveFileData_Stat statData;
        [global::ProtoBuf.ProtoMember(7)]
        public SaveFileData_Achievement achiData;
        
        public void Init()
        {
            saveItems = new SaveFileData_Item();
            saveRanks = new SaveFileData_Rank();
            charaInfos = new SaveFileData_Character();
            dialogData = new SaveFileData_Dialog();
            technologyData = new SaveFileData_Technology();
            statData = new SaveFileData_Stat();
            achiData = new SaveFileData_Achievement();
        }

        public void MigrateToVersion(int i)
        {
        }
    }


}