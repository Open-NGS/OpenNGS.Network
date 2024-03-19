using OpenNGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNGS.SaveData
{
    [global::ProtoBuf.ProtoContract()]
    public partial class IndexiesData<T> : global::ProtoBuf.IExtensible where T : ISaveEntity,new()
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public IndexiesData()
        {
            OnConstructor();
        }

        partial void OnConstructor();
        public const int MAGIC = 0x5844494E;

        [global::ProtoBuf.ProtoMember(1)]
        public int Magic { get; private set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int Version { get; private set; }

        [global::ProtoBuf.ProtoMember(3)]
        public int Current { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public Dictionary<int, SaveSlot<T>> Slots { get; private set; }

        public static IndexiesData<T> Create(int capacity,int version)
        {
            IndexiesData<T> index = new IndexiesData<T>()
            {
                Magic = MAGIC,
                Version = version,
                Slots = new Dictionary<int, SaveSlot<T>>(capacity),
                Current = 0,
            };
            return index;
        }

        public void New(int index)
        {
            SaveSlot<T> slot;
            if (!this.Slots.TryGetValue(index, out slot))
            {
                slot = new SaveSlot<T>();
                this.Slots.Add(index, slot);
            }
            slot.Index = index;
            slot.Time = Time.Timestamp;
            slot.SaveData = new SaveData<T>();
            slot.SaveData.Init(this.Version);
            slot.SaveData.Index = index;
            
        }

        public void Delete(int index)
        {
            this.Slots.Remove(index);
        }

        public bool Migrate(int version)
        {
            if(this.Version < version)
            {
                foreach(var slot in this.Slots)
                {
                    if (slot.Value.SaveData != null)
                        slot.Value.SaveData.Migrate(version);
                }
                this.Version = version;
                return true;
            }
            return false;
        }
    }
}
