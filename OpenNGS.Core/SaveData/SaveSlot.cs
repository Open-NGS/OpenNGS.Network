using Codice.CM.SEIDInfo;
using OpenNGS.SaveData.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNGS.SaveData
{
    [global::ProtoBuf.ProtoContract()]
    public partial class SaveSlot<T> : global::ProtoBuf.IExtensible where T : ISaveEntity,new()
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public SaveSlot()
        {
            OnConstructor();
        }

        partial void OnConstructor();

        public bool Active { get { return this.Index == SaveDataManager<T>.Instance.ActiveIndex; } }

        public DateTime SaveTime { get { return OpenNGS.Time.GetTime(this.Time); } }

        [global::ProtoBuf.ProtoMember(1)]
        public int Index { get; internal set; }

        [global::ProtoBuf.ProtoMember(2)]
        public SaveDataResult Status { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public int Time { get; internal set; }

        [global::ProtoBuf.ProtoMember(4)]
        public SaveData<T> SaveData { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        public Dictionary<string,string> Detail { get; internal set; }
    }
}
