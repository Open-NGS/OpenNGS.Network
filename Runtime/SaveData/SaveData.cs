using OpenNGS.SaveData.File;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    public partial class SaveData : ISaveData, global::ProtoBuf.IExtensible
    {
        public const int MAGIC = 0x4453474E;
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public SaveData()
        {
            this.Magic = MAGIC;
            this.CreateTime = OpenNGS.Time.Timestamp;
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public int Magic { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int Version { get; private set; }

        /// <summary>
        /// Title
        /// Maximum length 127
        /// </summary>
        [global::ProtoBuf.ProtoMember(3)]
        public string Title { get; set; }

        /// <summary>
        /// Sub Title
        /// Maximum length 127
        /// </summary>
        [global::ProtoBuf.ProtoMember(4)]
        public string SubTitle { get; set; }

        /// <summary>
        /// Detail
        /// Maximum length 1023
        /// </summary>
        [global::ProtoBuf.ProtoMember(5)]
        public string Detail { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [global::ProtoBuf.ProtoMember(6)]
        public int CreateTime { get; private set; }
        /// <summary>
        /// 当前写入时间
        /// </summary>
        [global::ProtoBuf.ProtoMember(7)]
        public int Time { get; set; }

        [global::ProtoBuf.ProtoMember(8)]
        public uint Totaltime { get; set; }

        [global::ProtoBuf.ProtoMember(9)]
        public Dictionary<string, string> MetaData { get; internal set; }

        /// <summary>
        /// UserID
        /// by passing a userId of 0 we use the default user that started the title
        /// </summary>
        public long UserID { get; set; }

        public string DirName { get; set; }

        public ulong TotalSize { get; set; }
        public ulong FreeSize { get; set; }

        public SaveDataResult Status { get; set; }

        public bool Loaded { get; private set; }

        internal List<SaveDataFile> Files = new List<SaveDataFile>();

        public DateTime SaveTime { get { return OpenNGS.Time.GetTime(this.Time); } }


        public SaveData(string name)
        {
            this.Magic = MAGIC;
            this.CreateTime = OpenNGS.Time.Timestamp;
            this.DirName = name;
        }


        public void Init(int version)
        {
            this.Version = version;
            this.Magic = MAGIC;
            this.CreateTime = OpenNGS.Time.Timestamp;
            //this.Data = new T();
            //this.Data.Initialize();
        }

        internal void AddSaveData(SaveDataFile file)
        {
            this.Files.Add(file);
        }

        public bool Migrate(int version)
        {
            if(this.Version != version)
            {
                for(int i=this.Version +1;i<= version;i++)
                {
                    //this.Data.MigrateToVersion(i);
                    //MigrateToVersion((GameDataVersion)i);
                }
                this.Version = version;
                return true;
            }
            return false;
        }

        internal void Write(MemoryStream ms)
        {
            ms.WriteInt(this.Magic);
            ms.WriteInt(this.Version);
            ms.WriteInt(this.CreateTime);
            ms.WriteInt(this.Time);
            ms.WriteString(this.Title);
            ms.WriteString(this.SubTitle??"");
            ms.WriteString(this.Detail ?? "");
        }

        internal void Read(MemoryStream ms)
        {
            this.Magic = ms.ReadInt();
            this.Version = ms.ReadInt();
            this.CreateTime = ms.ReadInt();
            this.Time = ms.ReadInt();
            this.Title = ms.ReadString();
            this.SubTitle = ms.ReadString();
            this.Detail = ms.ReadString();
        }

    }
}
