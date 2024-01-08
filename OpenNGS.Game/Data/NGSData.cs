#region Designer generated code
#pragma warning disable 0612, 0618, 1591, 3021
namespace OpenNGS.Data
{

    [global::ProtoBuf.ProtoContract()]
    public enum EXT_CONDITION_TYPE
    {
        EXT_CONDITION_TYPE_NONE = 0,
        EXT_CONDITION_TYPE_STATISTICS = 1,
    }

    [global::ProtoBuf.ProtoContract()]
    public enum CONDITION_PARAM_TYPE
    {
        CONDITION_PARAMTYPE_VALUE = 0,
        CONDITION_PARAMTYPE_RANGE = 1,
        CONDITION_PARAMTYPE_NITEXT = 2,
    }

    [global::ProtoBuf.ProtoContract()]
    public enum CONDITION_COMPARE_TYPE
    {
        CONDITION_COMPARETYPE_NONE = 0,
        CONDITION_COMPARETYPE_EQUAL = 1,
        CONDITION_COMPARETYPE_LESS = 2,
        CONDITION_COMPARETYPE_GREATER = 3,
        CONDITION_COMPARETYPE_LESS_OR_EUQAL = 4,
        CONDITION_COMPARETYPE_GREATER_OR_EQUAL = 5,
        CONDITION_COMPARETYPE_RANGE = 6,
        CONDITION_COMPARETYPE_RANGEL = 7,
        CONDITION_COMPARETYPE_RANGER = 8,
        CONDITION_COMPARETYPE_RANGEINC = 9,
    }

    [global::ProtoBuf.ProtoContract()]
    public enum FUNCTION_PARAM_TYPE
    {
        FUNCTION_PARAM_TYPE_NONE = 0,
        FUNCTION_PARAM_TYPE_VALUE = 1,
        FUNCTION_PARAM_TYPE_RANGE = 2,
        FUNCTION_PARAM_TYPE_LISTADD = 3,
    }

    [global::ProtoBuf.ProtoContract()]
    public enum FUNCTION_NAME
    {
        FUNCTION_NAME_NONE = 0,
        FUNCTION_NAME_OPEN_UI = 1,
        FUNCTION_NAME_SET_PARAM = 2,
        FUNCTION_NAME_SET_PROCESS = 3,
    }

    [global::ProtoBuf.ProtoContract()]
    public partial class Attribute : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public Attribute()
        {
            Name = "";
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint Id { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        [global::System.ComponentModel.DefaultValue("")]
        public string Name { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public global::OpenNGS.NGSText Title { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public global::OpenNGS.NGSText Description { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class AttributeList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public AttributeList()
        {
            items = new global::System.Collections.Generic.List<Attribute>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<Attribute> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class Message : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public Message()
        {
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint Id { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public global::OpenNGS.NGSText Title { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public global::OpenNGS.NGSText Text { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public global::OpenNGS.NGSText SubText { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        public global::OpenNGS.NGSText Option { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class MessageList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public MessageList()
        {
            items = new global::System.Collections.Generic.List<Message>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<Message> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class Notification : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public Notification()
        {
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint Id { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public global::OpenNGSCommon.NOTIFY_TYPE Type { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public uint MessageID { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public int Pos { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NotificationList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public NotificationList()
        {
            items = new global::System.Collections.Generic.List<Notification>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<Notification> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class QueueDefine : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public QueueDefine()
        {
            Desc = "";
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint Id { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        [global::System.ComponentModel.DefaultValue("")]
        public string Desc { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public global::OpenNGSCommon.QUEUE_TYPE Type { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public uint TargetId { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        public uint ConsumeId { get; set; }

        [global::ProtoBuf.ProtoMember(6)]
        public global::OpenNGSCommon.QUEUE_UPDATE_TYPE UpdateType { get; set; }

        [global::ProtoBuf.ProtoMember(7)]
        public uint Peroid { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class QueueDefineList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public QueueDefineList()
        {
            items = new global::System.Collections.Generic.List<QueueDefine>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<QueueDefine> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class TechData : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public TechData()
        {
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint TreeId { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public uint TechId { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public uint TechType { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public global::OpenNGS.NGSText Name { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        public uint Level { get; set; }

        [global::ProtoBuf.ProtoMember(6)]
        public uint LevelMax { get; set; }

        [global::ProtoBuf.ProtoMember(7)]
        public uint GlobalQueue { get; set; }

        [global::ProtoBuf.ProtoMember(8)]
        public uint Pos { get; set; }

        [global::ProtoBuf.ProtoMember(9)]
        public uint PreTech1 { get; set; }

        [global::ProtoBuf.ProtoMember(10)]
        public uint PreLevel1 { get; set; }

        [global::ProtoBuf.ProtoMember(11)]
        public uint PreTech2 { get; set; }

        [global::ProtoBuf.ProtoMember(12)]
        public uint PreLevel2 { get; set; }

        [global::ProtoBuf.ProtoMember(13)]
        public uint PreTech3 { get; set; }

        [global::ProtoBuf.ProtoMember(14)]
        public uint PreLevel3 { get; set; }

        [global::ProtoBuf.ProtoMember(15)]
        public uint PreTech4 { get; set; }

        [global::ProtoBuf.ProtoMember(16)]
        public uint PreLevel4 { get; set; }

        [global::ProtoBuf.ProtoMember(17)]
        public uint PreTech5 { get; set; }

        [global::ProtoBuf.ProtoMember(18)]
        public uint PreLevel5 { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class TechDataList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public TechDataList()
        {
            items = new global::System.Collections.Generic.List<TechData>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<TechData> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class TechUpgrade : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public TechUpgrade()
        {
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(4)]
        public uint TreeId { get; set; }

        [global::ProtoBuf.ProtoMember(1)]
        public uint TechId { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public global::OpenNGS.NGSText Name { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public global::OpenNGS.NGSText Description { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        public uint Level { get; set; }

        [global::ProtoBuf.ProtoMember(6)]
        public uint Time { get; set; }

        [global::ProtoBuf.ProtoMember(7)]
        public uint Wood { get; set; }

        [global::ProtoBuf.ProtoMember(8)]
        public uint Iron { get; set; }

        [global::ProtoBuf.ProtoMember(9)]
        public uint Stone { get; set; }

        [global::ProtoBuf.ProtoMember(10)]
        public uint Food { get; set; }

        [global::ProtoBuf.ProtoMember(11)]
        public uint AttributeId { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class TechUpgradeList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public TechUpgradeList()
        {
            items = new global::System.Collections.Generic.List<TechUpgrade>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<TechUpgrade> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class PrivilegeGroup : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public PrivilegeGroup()
        {
            Name = "";
            Description = "";
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint Id { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        [global::System.ComponentModel.DefaultValue("")]
        public string Name { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        [global::System.ComponentModel.DefaultValue("")]
        public string Description { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public uint Type { get; set; }

        [global::ProtoBuf.ProtoMember(5, IsPacked = true)]
        public uint[] Privileges { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class PrivilegeGroupList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public PrivilegeGroupList()
        {
            items = new global::System.Collections.Generic.List<PrivilegeGroup>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<PrivilegeGroup> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class Privilege : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public Privilege()
        {
            Name = "";
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint Id { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        [global::System.ComponentModel.DefaultValue("")]
        public string Name { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public uint Value { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class PrivilegeList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public PrivilegeList()
        {
            items = new global::System.Collections.Generic.List<Privilege>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<Privilege> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class Quest : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public Quest()
        {
            Tile = "";
            Description = "";
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint GroupID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public uint QuestID { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        [global::System.ComponentModel.DefaultValue("")]
        public string Tile { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        [global::System.ComponentModel.DefaultValue("")]
        public string Description { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        public uint Weights { get; set; }

        [global::ProtoBuf.ProtoMember(6)]
        public uint StatID { get; set; }

        [global::ProtoBuf.ProtoMember(7)]
        public uint Value { get; set; }

        [global::ProtoBuf.ProtoMember(8)]
        public uint RewardID { get; set; }

        [global::ProtoBuf.ProtoMember(9)]
        public uint IsHide { get; set; }

        [global::ProtoBuf.ProtoMember(10)]
        public uint DialogId { get; set; }

        [global::ProtoBuf.ProtoMember(11)]
        public uint IsBan { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class QuestList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public QuestList()
        {
            items = new global::System.Collections.Generic.List<Quest>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<Quest> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class QuestGroup : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public QuestGroup()
        {
            Tile = "";
            Description = "";
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint GroupID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        [global::System.ComponentModel.DefaultValue("")]
        public string Tile { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        [global::System.ComponentModel.DefaultValue("")]
        public string Description { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public global::OpenNGSCommon.QUEST_GROUP_TYPE GroupType { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        public uint IsQuestHead { get; set; }

        [global::ProtoBuf.ProtoMember(6)]
        public uint RelyOnGroupID { get; set; }

        [global::ProtoBuf.ProtoMember(7)]
        public uint NextQuestGroupID { get; set; }

        [global::ProtoBuf.ProtoMember(8)]
        public global::OpenNGSCommon.QUEST_PICK_TYPE PickQuestType { get; set; }

        [global::ProtoBuf.ProtoMember(9)]
        public uint PickNum { get; set; }

        [global::ProtoBuf.ProtoMember(10)]
        public uint RewardID { get; set; }

        [global::ProtoBuf.ProtoMember(11)]
        public uint IsBan { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class QuestGroupList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public QuestGroupList()
        {
            items = new global::System.Collections.Generic.List<QuestGroup>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<QuestGroup> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class Notice : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public Notice()
        {
            CondiionParam = "";
            ClickParam = "";
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint Id { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public uint Weight { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public global::OpenNGSCommon.ANNOUNCEMENT_CONNDITION_TYPE ConditionType { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        [global::System.ComponentModel.DefaultValue("")]
        public string CondiionParam { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        public global::OpenNGS.NGSText Title { get; set; }

        [global::ProtoBuf.ProtoMember(6)]
        public global::OpenNGS.NGSText Content { get; set; }

        [global::ProtoBuf.ProtoMember(7)]
        public global::OpenNGSCommon.ANNOUNCEMENT_CLICK_TYPE ClickType { get; set; }

        [global::ProtoBuf.ProtoMember(8)]
        [global::System.ComponentModel.DefaultValue("")]
        public string ClickParam { get; set; }

        [global::ProtoBuf.ProtoMember(9)]
        public uint IsCanIgnore { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NoticeList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public NoticeList()
        {
            items = new global::System.Collections.Generic.List<Notice>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<Notice> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class ObjectiveGroup : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public ObjectiveGroup()
        {
            Description = "";
            Title = "";
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint GroupId { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        [global::System.ComponentModel.DefaultValue("")]
        public string Description { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        [global::System.ComponentModel.DefaultValue("")]
        public string Title { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public global::OpenNGSCommon.OBJECTIVE_SYSTEM_TYPE SystemType { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        public global::OpenNGSCommon.OBJECTIVEGROUP_PICK_TYPE PickType { get; set; }

        [global::ProtoBuf.ProtoMember(6)]
        public global::OpenNGSCommon.OBJECTIVEGROUP_OWNER_TYPE OwnerType { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class ObjectiveGroupList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public ObjectiveGroupList()
        {
            items = new global::System.Collections.Generic.List<ObjectiveGroup>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<ObjectiveGroup> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class Objective : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public Objective()
        {
            Description = "";
            Title = "";
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint GroupId { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public uint ObjectiveId { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        [global::System.ComponentModel.DefaultValue("")]
        public string Description { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        [global::System.ComponentModel.DefaultValue("")]
        public string Title { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        public uint AchievementId { get; set; }

        [global::ProtoBuf.ProtoMember(6)]
        public uint RewardId { get; set; }

        [global::ProtoBuf.ProtoMember(7)]
        public global::OpenNGSCommon.OBJECTIVE_FINISH_RELATION_TYPE FinishRelationType { get; set; }

        [global::ProtoBuf.ProtoMember(8)]
        public global::OpenNGSCommon.OBJECTIVE_FINISH_TYPE FinishType { get; set; }

        [global::ProtoBuf.ProtoMember(9)]
        public uint FinishParam { get; set; }

        [global::ProtoBuf.ProtoMember(10, IsPacked = true)]
        public uint[] EffectId { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class ObjectiveList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public ObjectiveList()
        {
            items = new global::System.Collections.Generic.List<Objective>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<Objective> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class ObjectiveEffect : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public ObjectiveEffect()
        {
            Description = "";
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint EffectId { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        [global::System.ComponentModel.DefaultValue("")]
        public string Description { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public global::OpenNGSCommon.OBJECTIVE_EFFECT_FUNCTION_TYPE FunctionType { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public uint FunctionParam { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class ObjectiveEffectList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public ObjectiveEffectList()
        {
            items = new global::System.Collections.Generic.List<ObjectiveEffect>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<ObjectiveEffect> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class Ranking : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public Ranking()
        {
            name = "";
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint id { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        [global::System.ComponentModel.DefaultValue("")]
        public string name { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public uint size_limit { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public global::OpenNGSCommon.RANKING_LIFE_TYPE life_type { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        public global::OpenNGSCommon.RANKING_SCOPE_TYPE scope_type { get; set; }

        [global::ProtoBuf.ProtoMember(6)]
        public global::OpenNGSCommon.RANK_VALUE_TYPE value_type { get; set; }

        [global::ProtoBuf.ProtoMember(7)]
        public global::OpenNGSCommon.RANK_ZONE_RANGE_TYPE zone_range { get; set; }

        [global::ProtoBuf.ProtoMember(8)]
        public global::OpenNGSCommon.RANK_OBJECT_TYPE object_type { get; set; }

        [global::ProtoBuf.ProtoMember(9)]
        public global::OpenNGSCommon.RANK_EVENT_TYPE AddEventType { get; set; }

        [global::ProtoBuf.ProtoMember(10)]
        public global::OpenNGSCommon.RANK_EVENT_TYPE CleanEventType { get; set; }

        [global::ProtoBuf.ProtoMember(11)]
        public global::OpenNGSCommon.RANK_EVENT_TYPE DelEventType { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class RankingList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public RankingList()
        {
            items = new global::System.Collections.Generic.List<Ranking>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<Ranking> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class RedPoint : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public RedPoint()
        {
            Path = "";
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint ID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        [global::System.ComponentModel.DefaultValue("")]
        public string Path { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public global::OpenNGSCommon.REDPOINT_TYPE Type { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class RedPointList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public RedPointList()
        {
            items = new global::System.Collections.Generic.List<RedPoint>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<RedPoint> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class Shop : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public Shop()
        {
            OpenTime = "";
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint ID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public global::OpenNGS.NGSText Name { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        [global::System.ComponentModel.DefaultValue("")]
        public string OpenTime { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public uint Duration { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class ShopList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public ShopList()
        {
            items = new global::System.Collections.Generic.List<Shop>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<Shop> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NamingRule : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public NamingRule()
        {
            Language = "";
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint ID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        [global::System.ComponentModel.DefaultValue("")]
        public string Language { get; set; }

        [global::ProtoBuf.ProtoMember(3, IsPacked = true)]
        public uint[] NameGroups { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NamingRuleList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public NamingRuleList()
        {
            items = new global::System.Collections.Generic.List<NamingRule>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<NamingRule> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class Names : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public Names()
        {
            Name = "";
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint ID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public uint GroupId { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        [global::System.ComponentModel.DefaultValue("")]
        public string Name { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NamesList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public NamesList()
        {
            items = new global::System.Collections.Generic.List<Names>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<Names> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class FunctionCondition : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public FunctionCondition()
        {
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint ID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public uint ConsumeItem { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public int ConsumeValue { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public uint AttrID { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        public int AttrValue { get; set; }

        [global::ProtoBuf.ProtoMember(6)]
        public EXT_CONDITION_TYPE ExConditionType { get; set; }

        [global::ProtoBuf.ProtoMember(7)]
        public CONDITION_COMPARE_TYPE CompareType { get; set; }

        [global::ProtoBuf.ProtoMember(8)]
        public CONDITION_PARAM_TYPE ConditionParamType { get; set; }

        [global::ProtoBuf.ProtoMember(9)]
        public uint Param1 { get; set; }

        [global::ProtoBuf.ProtoMember(10)]
        public uint Param1b { get; set; }

        [global::ProtoBuf.ProtoMember(11)]
        public CONDITION_PARAM_TYPE ValueType { get; set; }

        [global::ProtoBuf.ProtoMember(12)]
        public uint Value1 { get; set; }

        [global::ProtoBuf.ProtoMember(13)]
        public uint Value2 { get; set; }

        [global::ProtoBuf.ProtoMember(14)]
        public int LockShowType { get; set; }

        [global::ProtoBuf.ProtoMember(15)]
        public int LockClickType { get; set; }

        [global::ProtoBuf.ProtoMember(16)]
        public global::OpenNGS.NGSText DesID { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class FunctionConditionList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public FunctionConditionList()
        {
            items = new global::System.Collections.Generic.List<FunctionCondition>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<FunctionCondition> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class FunctionCfg : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public FunctionCfg()
        {
            Des = "";
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint ID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public FUNCTION_NAME Name { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public uint ConditionID { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public uint PrivilegeID { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        [global::System.ComponentModel.DefaultValue("")]
        public string Des { get; set; }

        [global::ProtoBuf.ProtoMember(6)]
        public FUNCTION_PARAM_TYPE ParamType1 { get; set; }

        [global::ProtoBuf.ProtoMember(7)]
        public uint Param1 { get; set; }

        [global::ProtoBuf.ProtoMember(8)]
        public uint Param1b { get; set; }

        [global::ProtoBuf.ProtoMember(9)]
        public FUNCTION_PARAM_TYPE ParamType2 { get; set; }

        [global::ProtoBuf.ProtoMember(10)]
        public uint Param2 { get; set; }

        [global::ProtoBuf.ProtoMember(11)]
        public uint Param2b { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class FunctionCfgList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public FunctionCfgList()
        {
            items = new global::System.Collections.Generic.List<FunctionCfg>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<FunctionCfg> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class TechAttributes : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public TechAttributes()
        {
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint Id { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public global::OpenNGSCommon.NI_ATTR_TYPE Type { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public global::OpenNGS.NGSAttributes Attributes { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class TechAttributesList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public TechAttributesList()
        {
            items = new global::System.Collections.Generic.List<TechAttributes>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<TechAttributes> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class GM : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public GM()
        {
            cmd = "";
            handle = "";
            param = new global::System.Collections.Generic.List<string>();
            desc = "";
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint Id { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        [global::System.ComponentModel.DefaultValue("")]
        public string cmd { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        [global::System.ComponentModel.DefaultValue("")]
        public string handle { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public global::System.Collections.Generic.List<string> param { get; private set; }

        [global::ProtoBuf.ProtoMember(5)]
        [global::System.ComponentModel.DefaultValue("")]
        public string desc { get; set; }

        [global::ProtoBuf.ProtoMember(6)]
        public uint is_only_client { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class GMList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public GMList()
        {
            items = new global::System.Collections.Generic.List<GM>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<GM> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class UserSetting : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public UserSetting()
        {
            ComponentType = "";
            Component = "";
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint ID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public uint UserSettingType { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public uint Value { get; set; }

        [global::ProtoBuf.ProtoMember(4, IsPacked = true)]
        public uint[] Values { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        public global::OpenNGSCommon.STORAGE_TYPE StorageType { get; set; }

        [global::ProtoBuf.ProtoMember(6)]
        public uint DefaultValue { get; set; }

        [global::ProtoBuf.ProtoMember(7)]
        public uint MinValue { get; set; }

        [global::ProtoBuf.ProtoMember(8)]
        public uint MaxValue { get; set; }

        [global::ProtoBuf.ProtoMember(9)]
        public uint Multiple { get; set; }

        [global::ProtoBuf.ProtoMember(10)]
        [global::System.ComponentModel.DefaultValue("")]
        public string ComponentType { get; set; }

        [global::ProtoBuf.ProtoMember(11)]
        public uint OptionId { get; set; }

        [global::ProtoBuf.ProtoMember(12)]
        public global::OpenNGS.NGSText Title { get; set; }

        [global::ProtoBuf.ProtoMember(13)]
        public global::OpenNGS.NGSText Description { get; set; }

        [global::ProtoBuf.ProtoMember(14)]
        public uint SectionId { get; set; }

        [global::ProtoBuf.ProtoMember(15)]
        [global::System.ComponentModel.DefaultValue("")]
        public string Component { get; set; }

        [global::ProtoBuf.ProtoMember(16)]
        public global::OpenNGS.NGSText Tips { get; set; }

        [global::ProtoBuf.ProtoMember(17)]
        public uint ShowLabel { get; set; }

        [global::ProtoBuf.ProtoMember(18)]
        public uint Width { get; set; }

        [global::ProtoBuf.ProtoMember(19)]
        public uint RequireRestart { get; set; }

        [global::ProtoBuf.ProtoMember(20)]
        public uint Show { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class UserSettingList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public UserSettingList()
        {
            items = new global::System.Collections.Generic.List<UserSetting>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<UserSetting> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class UserSettingOption : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public UserSettingOption()
        {
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint ID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public uint IDX { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public global::OpenNGS.NGSText Title { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public uint Value { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        public global::OpenNGS.NGSText Description { get; set; }

        [global::ProtoBuf.ProtoMember(6)]
        public global::OpenNGS.NGSText Tips { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class UserSettingOptionList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public UserSettingOptionList()
        {
            items = new global::System.Collections.Generic.List<UserSettingOption>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<UserSettingOption> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class UserSettingGroup : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public UserSettingGroup()
        {
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint ID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public global::OpenNGS.NGSText Title { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public global::OpenNGS.NGSText Description { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public global::OpenNGS.NGSText Tips { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        public uint Show { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class UserSettingGroupList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public UserSettingGroupList()
        {
            items = new global::System.Collections.Generic.List<UserSettingGroup>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<UserSettingGroup> items { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class UserSettingSection : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public UserSettingSection()
        {
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public uint ID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public uint GroupID { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public uint Show { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public global::OpenNGS.NGSText Title { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        public global::OpenNGS.NGSText Description { get; set; }

        [global::ProtoBuf.ProtoMember(6)]
        public global::OpenNGS.NGSText Tips { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class UserSettingSectionList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public UserSettingSectionList()
        {
            items = new global::System.Collections.Generic.List<UserSettingSection>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<UserSettingSection> items { get; private set; }

    }

}

#pragma warning restore 0612, 0618, 1591, 3021
#endregion
