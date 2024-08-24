using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNGS.SaveData
{
    public interface ISaveData
    {
        bool Migrate(int version);
    }

    public interface ISaveEntity
    {
        void Init();
        void MigrateToVersion(int i);
    }

    [global::ProtoBuf.ProtoContract()]
    public partial class SaveData<T> : ISaveData, global::ProtoBuf.IExtensible where T : ISaveEntity, new()
    {
        public const int MAGIC = 0x4944534E;
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public SaveData()
        {
            OnConstructor();
        }

        partial void OnConstructor();


        [global::ProtoBuf.ProtoMember(1)]
        public int Index { get; set; }
        [global::ProtoBuf.ProtoMember(2)]
        public int Version { get; private set; }
        [global::ProtoBuf.ProtoMember(3)]
        public int Magic { get; set; }
        [global::ProtoBuf.ProtoMember(4)]
        public int CreateTime { get; private set; }

        [global::ProtoBuf.ProtoMember(5)]
        public int Time { get; internal set; }
        [global::ProtoBuf.ProtoMember(6)]
        public int Totaltime { get; internal set; }
        [global::ProtoBuf.ProtoMember(7)]
        public Dictionary<string, string> MetaData { get; internal set; }

        [global::ProtoBuf.ProtoMember(8)]
        public T Data { get; private set; }



        public void Init(int version)
        {
            this.Version = version;
            this.Magic = MAGIC;
            this.CreateTime = OpenNGS.Time.Timestamp;
            this.Data = new T();
            this.Data.Init();
        }

        public bool Migrate(int version)
        {
            if(this.Version != version)
            {
                for(int i=this.Version +1;i<= version;i++)
                {
                    this.Data.MigrateToVersion(i);
                    //MigrateToVersion((GameDataVersion)i);
                }
                this.Version = version;
                return true;
            }
            return false;
        }
    }
}
