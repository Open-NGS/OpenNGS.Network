using ProtoBuf.Meta;
using OpenNGS;
namespace OpenNGSCommon
{
	public partial class OwnerInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			Type = OBJECT_TYPE.OBJECT_TYPE_NONE;
			Id = 0;
		}
		public void OnRelease()
		{
			Type = OBJECT_TYPE.OBJECT_TYPE_NONE;
			Id = 0;
		}
		public void OnSpawn()
		{
		}
		public static OwnerInfo SpawnFromPool()
		{
			return (OwnerInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(OwnerInfo));
		}
	}
	public partial class ItemData : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			Guid = 0;
			ItemID = 0;
			Count = 0;
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGS.NGSAttributes>(Attributes);
		}
		public void OnRelease()
		{
			Guid = 0;
			ItemID = 0;
			Count = 0;
			OpenNGS.Net.ProtoPool.Instance.Release(Attributes);
		}
		public void OnSpawn()
		{
			 Attributes = (NGSAttributes)OpenNGS.Net.ProtoPool.Instance.Get(typeof(NGSAttributes));
		}
		public static ItemData SpawnFromPool()
		{
			return (ItemData)OpenNGS.Net.ProtoPool.Instance.Get(typeof(ItemData));
		}
	}
	public partial class ItemList : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			for(int ItemsCount = 0; ItemsCount < Items.Count; ItemsCount++)
			{
				OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.ItemData>(Items[ItemsCount]);
			}
			Items.Clear();
		}
		public void OnRelease()
		{
			for(int ItemsCount = 0; ItemsCount < Items.Count; ItemsCount++)
			{
				OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.ItemData>(Items[ItemsCount]);
			}
			Items.Clear();
		}
		public void OnSpawn()
		{
			for(int ItemsCount = 0; ItemsCount < Items.Count; ItemsCount++)
			{
				 Items[ItemsCount] = (ItemData)OpenNGS.Net.ProtoPool.Instance.Get(typeof(ItemData));
			}
		}
		public static ItemList SpawnFromPool()
		{
			return (ItemList)OpenNGS.Net.ProtoPool.Instance.Get(typeof(ItemList));
		}
	}
	public partial class ItemSet : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
		}
		public void OnRelease()
		{
		}
		public void OnSpawn()
		{
		}
		public static ItemSet SpawnFromPool()
		{
			return (ItemSet)OpenNGS.Net.ProtoPool.Instance.Get(typeof(ItemSet));
		}
	}
	public partial class Queue : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			QueueType = QUEUE_TYPE.QUEUE_TYPE_NONE;
			QueueID = 0;
			Current = 0;
			Target = 0;
			Interval = 0;
			StartTime = 0;
			RemainTime = 0;
			Status = QUEUE_STATUS.QUEUE_STATUS_IDLE;
			UpdateType = QUEUE_UPDATE_TYPE.QUEUE_UPDATE_TYPE_TIME;
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.OwnerInfo>(Owner);
			Param = 0;
		}
		public void OnRelease()
		{
			QueueType = QUEUE_TYPE.QUEUE_TYPE_NONE;
			QueueID = 0;
			Current = 0;
			Target = 0;
			Interval = 0;
			StartTime = 0;
			RemainTime = 0;
			Status = QUEUE_STATUS.QUEUE_STATUS_IDLE;
			UpdateType = QUEUE_UPDATE_TYPE.QUEUE_UPDATE_TYPE_TIME;
			OpenNGS.Net.ProtoPool.Instance.Release(Owner);
			Param = 0;
		}
		public void OnSpawn()
		{
			 Owner = (OwnerInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(OwnerInfo));
		}
		public static Queue SpawnFromPool()
		{
			return (Queue)OpenNGS.Net.ProtoPool.Instance.Get(typeof(Queue));
		}
	}
	public partial class QueueSet : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
		}
		public void OnRelease()
		{
		}
		public void OnSpawn()
		{
		}
		public static QueueSet SpawnFromPool()
		{
			return (QueueSet)OpenNGS.Net.ProtoPool.Instance.Get(typeof(QueueSet));
		}
	}
	public partial class QuestNode : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			uid = 0;
			groupid = 0;
			questid = 0;
			status = QUEST_STATE_TYPE.QUEST_STATE_TYPE_NONE;
			statid = 0;
			curval = 0;
			accepttime = 0;
		}
		public void OnRelease()
		{
			uid = 0;
			groupid = 0;
			questid = 0;
			status = QUEST_STATE_TYPE.QUEST_STATE_TYPE_NONE;
			statid = 0;
			curval = 0;
			accepttime = 0;
		}
		public void OnSpawn()
		{
		}
		public static QuestNode SpawnFromPool()
		{
			return (QuestNode)OpenNGS.Net.ProtoPool.Instance.Get(typeof(QuestNode));
		}
	}
	public partial class QuestNodeGroup : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			groupid = 0;
			status = QUEST_STATE_TYPE.QUEST_STATE_TYPE_NONE;
			curval = 0;
		}
		public void OnRelease()
		{
			groupid = 0;
			status = QUEST_STATE_TYPE.QUEST_STATE_TYPE_NONE;
			curval = 0;
		}
		public void OnSpawn()
		{
		}
		public static QuestNodeGroup SpawnFromPool()
		{
			return (QuestNodeGroup)OpenNGS.Net.ProtoPool.Instance.Get(typeof(QuestNodeGroup));
		}
	}
	public partial class TechTree : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			TreeId = 0;
		}
		public void OnRelease()
		{
			TreeId = 0;
		}
		public void OnSpawn()
		{
		}
		public static TechTree SpawnFromPool()
		{
			return (TechTree)OpenNGS.Net.ProtoPool.Instance.Get(typeof(TechTree));
		}
	}
	public partial class Tech : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			Id = 0;
			Level = 0;
			Queue = 0;
		}
		public void OnRelease()
		{
			Id = 0;
			Level = 0;
			Queue = 0;
		}
		public void OnSpawn()
		{
		}
		public static Tech SpawnFromPool()
		{
			return (Tech)OpenNGS.Net.ProtoPool.Instance.Get(typeof(Tech));
		}
	}
	public partial class SeasonInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			total_match = 0;
			win_match = 0;
		}
		public void OnRelease()
		{
			total_match = 0;
			win_match = 0;
		}
		public void OnSpawn()
		{
		}
		public static SeasonInfo SpawnFromPool()
		{
			return (SeasonInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(SeasonInfo));
		}
	}
	public partial class RankRow : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			player_id = 0;
			score = 0;
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.SeasonInfo>(season_info);
		}
		public void OnRelease()
		{
			player_id = 0;
			score = 0;
			OpenNGS.Net.ProtoPool.Instance.Release(season_info);
		}
		public void OnSpawn()
		{
			 season_info = (SeasonInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(SeasonInfo));
		}
		public static RankRow SpawnFromPool()
		{
			return (RankRow)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RankRow));
		}
	}
	public partial class RankPage : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			for(int rank_rowsCount = 0; rank_rowsCount < rank_rows.Count; rank_rowsCount++)
			{
				OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.RankRow>(rank_rows[rank_rowsCount]);
			}
			rank_rows.Clear();
		}
		public void OnRelease()
		{
			for(int rank_rowsCount = 0; rank_rowsCount < rank_rows.Count; rank_rowsCount++)
			{
				OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.RankRow>(rank_rows[rank_rowsCount]);
			}
			rank_rows.Clear();
		}
		public void OnSpawn()
		{
			for(int rank_rowsCount = 0; rank_rowsCount < rank_rows.Count; rank_rowsCount++)
			{
				 rank_rows[rank_rowsCount] = (RankRow)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RankRow));
			}
		}
		public static RankPage SpawnFromPool()
		{
			return (RankPage)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RankPage));
		}
	}
	public partial class RankType : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			life_type = RANKING_LIFE_TYPE.RANKING_LIFE_ETERNAL;
			scope_type = RANKING_SCOPE_TYPE.RANKING_SCOPE_SVR;
			value_type = RANK_VALUE_TYPE.RANK_VALUE_TYPE_NONE;
			zone_range = RANK_ZONE_RANGE_TYPE.RANK_ZONE_RANGE_NONE;
			zone_id = 0;
			object_type = RANK_OBJECT_TYPE.RANK_OBJECT_TYPE_NONE;
		}
		public void OnRelease()
		{
			life_type = RANKING_LIFE_TYPE.RANKING_LIFE_ETERNAL;
			scope_type = RANKING_SCOPE_TYPE.RANKING_SCOPE_SVR;
			value_type = RANK_VALUE_TYPE.RANK_VALUE_TYPE_NONE;
			zone_range = RANK_ZONE_RANGE_TYPE.RANK_ZONE_RANGE_NONE;
			zone_id = 0;
			object_type = RANK_OBJECT_TYPE.RANK_OBJECT_TYPE_NONE;
		}
		public void OnSpawn()
		{
		}
		public static RankType SpawnFromPool()
		{
			return (RankType)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RankType));
		}
	}
	public partial class ServiceInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			service_name = null;
		}
		public void OnRelease()
		{
			service_name = null;
		}
		public void OnSpawn()
		{
		}
		public static ServiceInfo SpawnFromPool()
		{
			return (ServiceInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(ServiceInfo));
		}
	}
	public partial class GetRequest : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			uin = 0;
			count = 0;
			page = 0;
		}
		public void OnRelease()
		{
			uin = 0;
			count = 0;
			page = 0;
		}
		public void OnSpawn()
		{
		}
		public static GetRequest SpawnFromPool()
		{
			return (GetRequest)OpenNGS.Net.ProtoPool.Instance.Get(typeof(GetRequest));
		}
	}
	public partial class PlayerIdentifier : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			id = 0;
		}
		public void OnRelease()
		{
			id = 0;
		}
		public void OnSpawn()
		{
		}
		public static PlayerIdentifier SpawnFromPool()
		{
			return (PlayerIdentifier)OpenNGS.Net.ProtoPool.Instance.Get(typeof(PlayerIdentifier));
		}
	}
	public partial class ListRequest : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			uin = 0;
			count = 0;
			page = 0;
		}
		public void OnRelease()
		{
			uin = 0;
			count = 0;
			page = 0;
		}
		public void OnSpawn()
		{
		}
		public static ListRequest SpawnFromPool()
		{
			return (ListRequest)OpenNGS.Net.ProtoPool.Instance.Get(typeof(ListRequest));
		}
	}
	public partial class IdRequest : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			uin = 0;
		}
		public void OnRelease()
		{
			uin = 0;
		}
		public void OnSpawn()
		{
		}
		public static IdRequest SpawnFromPool()
		{
			return (IdRequest)OpenNGS.Net.ProtoPool.Instance.Get(typeof(IdRequest));
		}
	}
	public partial class ResponseResult : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			errcode = 0;
			errmsg = 0;
		}
		public void OnRelease()
		{
			errcode = 0;
			errmsg = 0;
		}
		public void OnSpawn()
		{
		}
		public static ResponseResult SpawnFromPool()
		{
			return (ResponseResult)OpenNGS.Net.ProtoPool.Instance.Get(typeof(ResponseResult));
		}
	}
	public partial class MailSenderInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			sender_type = MAIL_SENDER_TYPE.SENDER_TYPE_USER;
			sender_uin = 0;
			sender_guild_position = 0;
			sender_guild_id = 0;
		}
		public void OnRelease()
		{
			sender_type = MAIL_SENDER_TYPE.SENDER_TYPE_USER;
			sender_uin = 0;
			sender_guild_position = 0;
			sender_guild_id = 0;
		}
		public void OnSpawn()
		{
		}
		public static MailSenderInfo SpawnFromPool()
		{
			return (MailSenderInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(MailSenderInfo));
		}
	}
	public partial class MailReceiverInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			receiver_type = MAIL_RECEIVER_TYPE.RECEIVER_TYPE_USER;
			receiver_uin = 0;
		}
		public void OnRelease()
		{
			receiver_type = MAIL_RECEIVER_TYPE.RECEIVER_TYPE_USER;
			receiver_uin = 0;
		}
		public void OnSpawn()
		{
		}
		public static MailReceiverInfo SpawnFromPool()
		{
			return (MailReceiverInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(MailReceiverInfo));
		}
	}
	public partial class PlayerBaseInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			uin = 0;
			nickname = null;
			guildid = 0;
			guild_name = null;
		}
		public void OnRelease()
		{
			uin = 0;
			nickname = null;
			guildid = 0;
			guild_name = null;
		}
		public void OnSpawn()
		{
		}
		public static PlayerBaseInfo SpawnFromPool()
		{
			return (PlayerBaseInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(PlayerBaseInfo));
		}
	}
	public partial class PlayerStatusInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			uin = 0;
			online_status = PLAYER_ONLINE_STATUS_TYPE.OFFLINE;
		}
		public void OnRelease()
		{
			uin = 0;
			online_status = PLAYER_ONLINE_STATUS_TYPE.OFFLINE;
		}
		public void OnSpawn()
		{
		}
		public static PlayerStatusInfo SpawnFromPool()
		{
			return (PlayerStatusInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(PlayerStatusInfo));
		}
	}
	public partial class ChannelInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			ctype = CHAT_CHANNEL_TYPE.CHAT_TYPE_WORLD;
			uid = 0;
			uid1 = 0;
		}
		public void OnRelease()
		{
			ctype = CHAT_CHANNEL_TYPE.CHAT_TYPE_WORLD;
			uid = 0;
			uid1 = 0;
		}
		public void OnSpawn()
		{
		}
		public static ChannelInfo SpawnFromPool()
		{
			return (ChannelInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(ChannelInfo));
		}
	}
	public partial class ChatInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.ChannelInfo>(channel);
			content = null;
			argsparams = null;
			time = 0;
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.PlayerBaseInfo>(sender);
			seq = 0;
		}
		public void OnRelease()
		{
			OpenNGS.Net.ProtoPool.Instance.Release(channel);
			content = null;
			argsparams = null;
			time = 0;
			OpenNGS.Net.ProtoPool.Instance.Release(sender);
			seq = 0;
		}
		public void OnSpawn()
		{
			 channel = (ChannelInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(ChannelInfo));
			 sender = (PlayerBaseInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(PlayerBaseInfo));
		}
		public static ChatInfo SpawnFromPool()
		{
			return (ChatInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(ChatInfo));
		}
	}
	public partial class MatchPlayerAttribute : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			name = null;
			int_val = 0;
		}
		public void OnRelease()
		{
			name = null;
			int_val = 0;
		}
		public void OnSpawn()
		{
		}
		public static MatchPlayerAttribute SpawnFromPool()
		{
			return (MatchPlayerAttribute)OpenNGS.Net.ProtoPool.Instance.Get(typeof(MatchPlayerAttribute));
		}
	}
	public partial class MatchPlayer : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			uin = 0;
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.MatchPlayerAttribute>(attributes);
		}
		public void OnRelease()
		{
			uin = 0;
			OpenNGS.Net.ProtoPool.Instance.Release(attributes);
		}
		public void OnSpawn()
		{
			 attributes = (MatchPlayerAttribute)OpenNGS.Net.ProtoPool.Instance.Get(typeof(MatchPlayerAttribute));
		}
		public static MatchPlayer SpawnFromPool()
		{
			return (MatchPlayer)OpenNGS.Net.ProtoPool.Instance.Get(typeof(MatchPlayer));
		}
	}
	public partial class MatchTicket : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			ticket_id = null;
			ticket_uid = 0;
			match_code = null;
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.MatchPlayer>(players);
			begin_time = 0;
			status = 0;
		}
		public void OnRelease()
		{
			ticket_id = null;
			ticket_uid = 0;
			match_code = null;
			OpenNGS.Net.ProtoPool.Instance.Release(players);
			begin_time = 0;
			status = 0;
		}
		public void OnSpawn()
		{
			 players = (MatchPlayer)OpenNGS.Net.ProtoPool.Instance.Get(typeof(MatchPlayer));
		}
		public static MatchTicket SpawnFromPool()
		{
			return (MatchTicket)OpenNGS.Net.ProtoPool.Instance.Get(typeof(MatchTicket));
		}
	}
	public partial class MatchResultPlayer : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			uin = 0;
		}
		public void OnRelease()
		{
			uin = 0;
		}
		public void OnSpawn()
		{
		}
		public static MatchResultPlayer SpawnFromPool()
		{
			return (MatchResultPlayer)OpenNGS.Net.ProtoPool.Instance.Get(typeof(MatchResultPlayer));
		}
	}
	public partial class MatchCamp : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			score = 0;
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.MatchResultPlayer>(players);
		}
		public void OnRelease()
		{
			score = 0;
			OpenNGS.Net.ProtoPool.Instance.Release(players);
		}
		public void OnSpawn()
		{
			 players = (MatchResultPlayer)OpenNGS.Net.ProtoPool.Instance.Get(typeof(MatchResultPlayer));
		}
		public static MatchCamp SpawnFromPool()
		{
			return (MatchCamp)OpenNGS.Net.ProtoPool.Instance.Get(typeof(MatchCamp));
		}
	}
	public partial class MatchResult : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			result_id = 0;
			result_uid = null;
			match_code = null;
			create_time = 0;
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.MatchCamp>(camps);
		}
		public void OnRelease()
		{
			result_id = 0;
			result_uid = null;
			match_code = null;
			create_time = 0;
			OpenNGS.Net.ProtoPool.Instance.Release(camps);
		}
		public void OnSpawn()
		{
			 camps = (MatchCamp)OpenNGS.Net.ProtoPool.Instance.Get(typeof(MatchCamp));
		}
		public static MatchResult SpawnFromPool()
		{
			return (MatchResult)OpenNGS.Net.ProtoPool.Instance.Get(typeof(MatchResult));
		}
	}
	public partial class RoomStatus : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			roomTid = 0;
			roomUid = 0;
			token = null;
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.RoomUrl>(roomurl);
			status = 0;
			maxPlayerLimit = 0;
			uin = 0;
			roomOwnerType = ROOM_OWNER_TYPE.ROOM_OWNER_TYPE_SYSTEM;
			roomOpenLevel = ROOM_OPEN_LEVEL.ROOM_OPEN_LEVEL_NIL;
		}
		public void OnRelease()
		{
			roomTid = 0;
			roomUid = 0;
			token = null;
			OpenNGS.Net.ProtoPool.Instance.Release(roomurl);
			status = 0;
			maxPlayerLimit = 0;
			uin = 0;
			roomOwnerType = ROOM_OWNER_TYPE.ROOM_OWNER_TYPE_SYSTEM;
			roomOpenLevel = ROOM_OPEN_LEVEL.ROOM_OPEN_LEVEL_NIL;
		}
		public void OnSpawn()
		{
			 roomurl = (RoomUrl)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RoomUrl));
		}
		public static RoomStatus SpawnFromPool()
		{
			return (RoomStatus)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RoomStatus));
		}
	}
	public partial class RoomUrl : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			svrService = null;
			svrInstanceId = 0;
			roomTid = 0;
			roomUid = 0;
			token = null;
		}
		public void OnRelease()
		{
			svrService = null;
			svrInstanceId = 0;
			roomTid = 0;
			roomUid = 0;
			token = null;
		}
		public void OnSpawn()
		{
		}
		public static RoomUrl SpawnFromPool()
		{
			return (RoomUrl)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RoomUrl));
		}
	}
	public partial class RoomSetting : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			roomName = null;
			maxPlayerLimit = 0;
			roomOpenLevel = ROOM_OPEN_LEVEL.ROOM_OPEN_LEVEL_NIL;
			metaData = null;
		}
		public void OnRelease()
		{
			roomName = null;
			maxPlayerLimit = 0;
			roomOpenLevel = ROOM_OPEN_LEVEL.ROOM_OPEN_LEVEL_NIL;
			metaData = null;
		}
		public void OnSpawn()
		{
		}
		public static RoomSetting SpawnFromPool()
		{
			return (RoomSetting)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RoomSetting));
		}
	}
	public partial class Vector3 : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			x = 0;
			y = 0;
			z = 0;
		}
		public void OnRelease()
		{
			x = 0;
			y = 0;
			z = 0;
		}
		public void OnSpawn()
		{
		}
		public static Vector3 SpawnFromPool()
		{
			return (Vector3)OpenNGS.Net.ProtoPool.Instance.Get(typeof(Vector3));
		}
	}
	public partial class EntityKey : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			eType = 0;
			uid = 0;
			tid = 0;
			fatherUid = 0;
			opendId = 0;
		}
		public void OnRelease()
		{
			eType = 0;
			uid = 0;
			tid = 0;
			fatherUid = 0;
			opendId = 0;
		}
		public void OnSpawn()
		{
		}
		public static EntityKey SpawnFromPool()
		{
			return (EntityKey)OpenNGS.Net.ProtoPool.Instance.Get(typeof(EntityKey));
		}
	}
	public partial class EntityPos : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.Vector3>(pos);
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.Vector3>(direction);
			speed = 0;
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.Vector3>(target);
		}
		public void OnRelease()
		{
			OpenNGS.Net.ProtoPool.Instance.Release(pos);
			OpenNGS.Net.ProtoPool.Instance.Release(direction);
			speed = 0;
			OpenNGS.Net.ProtoPool.Instance.Release(target);
		}
		public void OnSpawn()
		{
			 pos = (Vector3)OpenNGS.Net.ProtoPool.Instance.Get(typeof(Vector3));
			 direction = (Vector3)OpenNGS.Net.ProtoPool.Instance.Get(typeof(Vector3));
			 target = (Vector3)OpenNGS.Net.ProtoPool.Instance.Get(typeof(Vector3));
		}
		public static EntityPos SpawnFromPool()
		{
			return (EntityPos)OpenNGS.Net.ProtoPool.Instance.Get(typeof(EntityPos));
		}
	}
	public partial class EntityAttr : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			attrType = 0;
			attrId = 0;
			value = 0;
			attrBuf = null;
		}
		public void OnRelease()
		{
			attrType = 0;
			attrId = 0;
			value = 0;
			attrBuf = null;
		}
		public void OnSpawn()
		{
		}
		public static EntityAttr SpawnFromPool()
		{
			return (EntityAttr)OpenNGS.Net.ProtoPool.Instance.Get(typeof(EntityAttr));
		}
	}
	public partial class Entity : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.EntityKey>(id);
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.EntityPos>(pos);
			for(int attrsCount = 0; attrsCount < attrs.Count; attrsCount++)
			{
				OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.EntityAttr>(attrs[attrsCount]);
			}
			attrs.Clear();
			avatar = null;
			attributes = null;
			localid = 0;
			uin = 0;
			hostuin = 0;
			parentuid = 0;
			version = 0;
		}
		public void OnRelease()
		{
			OpenNGS.Net.ProtoPool.Instance.Release(id);
			OpenNGS.Net.ProtoPool.Instance.Release(pos);
			for(int attrsCount = 0; attrsCount < attrs.Count; attrsCount++)
			{
				OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.EntityAttr>(attrs[attrsCount]);
			}
			attrs.Clear();
			avatar = null;
			attributes = null;
			localid = 0;
			uin = 0;
			hostuin = 0;
			parentuid = 0;
			version = 0;
		}
		public void OnSpawn()
		{
			 id = (EntityKey)OpenNGS.Net.ProtoPool.Instance.Get(typeof(EntityKey));
			 pos = (EntityPos)OpenNGS.Net.ProtoPool.Instance.Get(typeof(EntityPos));
			for(int attrsCount = 0; attrsCount < attrs.Count; attrsCount++)
			{
				 attrs[attrsCount] = (EntityAttr)OpenNGS.Net.ProtoPool.Instance.Get(typeof(EntityAttr));
			}
		}
		public static Entity SpawnFromPool()
		{
			return (Entity)OpenNGS.Net.ProtoPool.Instance.Get(typeof(Entity));
		}
	}
	public partial class PlayerDesc : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			uin = 0;
			nickName = null;
		}
		public void OnRelease()
		{
			uin = 0;
			nickName = null;
		}
		public void OnSpawn()
		{
		}
		public static PlayerDesc SpawnFromPool()
		{
			return (PlayerDesc)OpenNGS.Net.ProtoPool.Instance.Get(typeof(PlayerDesc));
		}
	}
	public partial class StatusData : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			SystemName = null;
			OpCode = StatusOpCode.Status_None;
			for(int DatasCount = 0; DatasCount < Datas.Count; DatasCount++)
			{
				Datas[DatasCount] = null;
			}
			Datas.Clear();
		}
		public void OnRelease()
		{
			SystemName = null;
			OpCode = StatusOpCode.Status_None;
			for(int DatasCount = 0; DatasCount < Datas.Count; DatasCount++)
			{
				Datas[DatasCount] = null;
			}
			Datas.Clear();
		}
		public void OnSpawn()
		{
			for(int DatasCount = 0; DatasCount < Datas.Count; DatasCount++)
			{
			}
		}
		public static StatusData SpawnFromPool()
		{
			return (StatusData)OpenNGS.Net.ProtoPool.Instance.Get(typeof(StatusData));
		}
	}
	public partial class StatusDataList : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			timestamp = 0;
			for(int status_datasCount = 0; status_datasCount < status_datas.Count; status_datasCount++)
			{
				OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.StatusData>(status_datas[status_datasCount]);
			}
			status_datas.Clear();
		}
		public void OnRelease()
		{
			timestamp = 0;
			for(int status_datasCount = 0; status_datasCount < status_datas.Count; status_datasCount++)
			{
				OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.StatusData>(status_datas[status_datasCount]);
			}
			status_datas.Clear();
		}
		public void OnSpawn()
		{
			for(int status_datasCount = 0; status_datasCount < status_datas.Count; status_datasCount++)
			{
				 status_datas[status_datasCount] = (StatusData)OpenNGS.Net.ProtoPool.Instance.Get(typeof(StatusData));
			}
		}
		public static StatusDataList SpawnFromPool()
		{
			return (StatusDataList)OpenNGS.Net.ProtoPool.Instance.Get(typeof(StatusDataList));
		}
	}
	public partial class DirtyMarkInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			system_name = null;
			mark = 0;
		}
		public void OnRelease()
		{
			system_name = null;
			mark = 0;
		}
		public void OnSpawn()
		{
		}
		public static DirtyMarkInfo SpawnFromPool()
		{
			return (DirtyMarkInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(DirtyMarkInfo));
		}
	}
	public partial class RoomData : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			owner = 0;
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.RoomUrl>(room_url);
		}
		public void OnRelease()
		{
			owner = 0;
			OpenNGS.Net.ProtoPool.Instance.Release(room_url);
		}
		public void OnSpawn()
		{
			 room_url = (RoomUrl)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RoomUrl));
		}
		public static RoomData SpawnFromPool()
		{
			return (RoomData)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RoomData));
		}
	}
	public partial class InviteInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			from_uin = 0;
			to_uin = 0;
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.RoomData>(room_info);
			result = ReplyResultCode.RET_NONE;
			timestamp = 0;
		}
		public void OnRelease()
		{
			from_uin = 0;
			to_uin = 0;
			OpenNGS.Net.ProtoPool.Instance.Release(room_info);
			result = ReplyResultCode.RET_NONE;
			timestamp = 0;
		}
		public void OnSpawn()
		{
			 room_info = (RoomData)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RoomData));
		}
		public static InviteInfo SpawnFromPool()
		{
			return (InviteInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(InviteInfo));
		}
	}
	public partial class RequestEnterInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			request_uin = 0;
			be_requested_uin = 0;
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.RoomData>(room_info);
			result = ReplyResultCode.RET_NONE;
			timestamp = 0;
		}
		public void OnRelease()
		{
			request_uin = 0;
			be_requested_uin = 0;
			OpenNGS.Net.ProtoPool.Instance.Release(room_info);
			result = ReplyResultCode.RET_NONE;
			timestamp = 0;
		}
		public void OnSpawn()
		{
			 room_info = (RoomData)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RoomData));
		}
		public static RequestEnterInfo SpawnFromPool()
		{
			return (RequestEnterInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RequestEnterInfo));
		}
	}
	public partial class PlayerSocialStatusData : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			uin = 0;
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.PlayerStatusInfo>(status_info);
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.RoomData>(room_info);
		}
		public void OnRelease()
		{
			uin = 0;
			OpenNGS.Net.ProtoPool.Instance.Release(status_info);
			OpenNGS.Net.ProtoPool.Instance.Release(room_info);
		}
		public void OnSpawn()
		{
			 status_info = (PlayerStatusInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(PlayerStatusInfo));
			 room_info = (RoomData)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RoomData));
		}
		public static PlayerSocialStatusData SpawnFromPool()
		{
			return (PlayerSocialStatusData)OpenNGS.Net.ProtoPool.Instance.Get(typeof(PlayerSocialStatusData));
		}
	}
	public partial class UserSettingValueItem : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			value = 0;
			values = null;
		}
		public void OnRelease()
		{
			value = 0;
			values = null;
		}
		public void OnSpawn()
		{
		}
		public static UserSettingValueItem SpawnFromPool()
		{
			return (UserSettingValueItem)OpenNGS.Net.ProtoPool.Instance.Get(typeof(UserSettingValueItem));
		}
	}
	public partial class ObjectiveNode : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			groupid = 0;
			objid = 0;
			status = OBJECTIVE_STATE_TYPE.OBJECTIVE_STATE_TYPE_NONE;
			curval = 0;
			seasonid = 0;
			accepttime = 0;
			endtime = 0;
		}
		public void OnRelease()
		{
			groupid = 0;
			objid = 0;
			status = OBJECTIVE_STATE_TYPE.OBJECTIVE_STATE_TYPE_NONE;
			curval = 0;
			seasonid = 0;
			accepttime = 0;
			endtime = 0;
		}
		public void OnSpawn()
		{
		}
		public static ObjectiveNode SpawnFromPool()
		{
			return (ObjectiveNode)OpenNGS.Net.ProtoPool.Instance.Get(typeof(ObjectiveNode));
		}
	}
	public partial class RankingKey : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.RankingType>(ObjType);
			ObjectId = 0;
		}
		public void OnRelease()
		{
			OpenNGS.Net.ProtoPool.Instance.Release(ObjType);
			ObjectId = 0;
		}
		public void OnSpawn()
		{
			 ObjType = (RankingType)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RankingType));
		}
		public static RankingKey SpawnFromPool()
		{
			return (RankingKey)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RankingKey));
		}
	}
	public partial class RankingType : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			ZoneRange = RANK_ZONE_RANGE_TYPE.RANK_ZONE_RANGE_NONE;
			ZoneId = 0;
			ObjectType = RANK_OBJECT_TYPE.RANK_OBJECT_TYPE_NONE;
			ValueType = RANK_VALUE_TYPE.RANK_VALUE_TYPE_NONE;
		}
		public void OnRelease()
		{
			ZoneRange = RANK_ZONE_RANGE_TYPE.RANK_ZONE_RANGE_NONE;
			ZoneId = 0;
			ObjectType = RANK_OBJECT_TYPE.RANK_OBJECT_TYPE_NONE;
			ValueType = RANK_VALUE_TYPE.RANK_VALUE_TYPE_NONE;
		}
		public void OnSpawn()
		{
		}
		public static RankingType SpawnFromPool()
		{
			return (RankingType)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RankingType));
		}
	}
	public partial class RankingNode : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.RankingKey>(RankKey);
			Value = 0;
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.RankBaseInfo>(BaseInfo);
		}
		public void OnRelease()
		{
			OpenNGS.Net.ProtoPool.Instance.Release(RankKey);
			Value = 0;
			OpenNGS.Net.ProtoPool.Instance.Release(BaseInfo);
		}
		public void OnSpawn()
		{
			 RankKey = (RankingKey)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RankingKey));
			 BaseInfo = (RankBaseInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RankBaseInfo));
		}
		public static RankingNode SpawnFromPool()
		{
			return (RankingNode)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RankingNode));
		}
	}
	public partial class RankPlayerBaseInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			uin = 0;
			nickname = null;
			guildid = 0;
			guildname = null;
		}
		public void OnRelease()
		{
			uin = 0;
			nickname = null;
			guildid = 0;
			guildname = null;
		}
		public void OnSpawn()
		{
		}
		public static RankPlayerBaseInfo SpawnFromPool()
		{
			return (RankPlayerBaseInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RankPlayerBaseInfo));
		}
	}
	public partial class RankGuildBaseInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			guildid = 0;
			guildname = null;
			membernum = 0;
		}
		public void OnRelease()
		{
			guildid = 0;
			guildname = null;
			membernum = 0;
		}
		public void OnSpawn()
		{
		}
		public static RankGuildBaseInfo SpawnFromPool()
		{
			return (RankGuildBaseInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RankGuildBaseInfo));
		}
	}
	public partial class RankBaseInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.RankPlayerBaseInfo>(player);
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.RankGuildBaseInfo>(guild);
		}
		public void OnRelease()
		{
			OpenNGS.Net.ProtoPool.Instance.Release(player);
			OpenNGS.Net.ProtoPool.Instance.Release(guild);
		}
		public void OnSpawn()
		{
			 player = (RankPlayerBaseInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RankPlayerBaseInfo));
			 guild = (RankGuildBaseInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RankGuildBaseInfo));
		}
		public static RankBaseInfo SpawnFromPool()
		{
			return (RankBaseInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RankBaseInfo));
		}
	}
	public partial class LimitInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			type = 0;
			endtime = 0;
		}
		public void OnRelease()
		{
			type = 0;
			endtime = 0;
		}
		public void OnSpawn()
		{
		}
		public static LimitInfo SpawnFromPool()
		{
			return (LimitInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(LimitInfo));
		}
	}
	public partial class AccountIdentity : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			type = 0;
			identity = null;
		}
		public void OnRelease()
		{
			type = 0;
			identity = null;
		}
		public void OnSpawn()
		{
		}
		public static AccountIdentity SpawnFromPool()
		{
			return (AccountIdentity)OpenNGS.Net.ProtoPool.Instance.Get(typeof(AccountIdentity));
		}
	}
	public partial class AccountInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			gameid = 0;
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.AccountIdentity>(identity);
			openid = 0;
			permission = 0;
			createtime = 0;
			lastlogintime = 0;
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.LimitInfo>(limit);
		}
		public void OnRelease()
		{
			gameid = 0;
			OpenNGS.Net.ProtoPool.Instance.Release(identity);
			openid = 0;
			permission = 0;
			createtime = 0;
			lastlogintime = 0;
			OpenNGS.Net.ProtoPool.Instance.Release(limit);
		}
		public void OnSpawn()
		{
			 identity = (AccountIdentity)OpenNGS.Net.ProtoPool.Instance.Get(typeof(AccountIdentity));
			 limit = (LimitInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(LimitInfo));
		}
		public static AccountInfo SpawnFromPool()
		{
			return (AccountInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(AccountInfo));
		}
	}
	public partial class UserInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			userid = 0;
			openid = 0;
			serverid = 0;
			gentime = 0;
		}
		public void OnRelease()
		{
			userid = 0;
			openid = 0;
			serverid = 0;
			gentime = 0;
		}
		public void OnSpawn()
		{
		}
		public static UserInfo SpawnFromPool()
		{
			return (UserInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(UserInfo));
		}
	}
	public partial class CharacterInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			uin = 0;
			nickname = null;
			createtime = 0;
			avatar = null;
			userid = 0;
			openid = 0;
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.LimitInfo>(limit);
			imageurl = null;
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.AccountIdentity>(identity);
		}
		public void OnRelease()
		{
			uin = 0;
			nickname = null;
			createtime = 0;
			avatar = null;
			userid = 0;
			openid = 0;
			OpenNGS.Net.ProtoPool.Instance.Release(limit);
			imageurl = null;
			OpenNGS.Net.ProtoPool.Instance.Release(identity);
		}
		public void OnSpawn()
		{
			 limit = (LimitInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(LimitInfo));
			 identity = (AccountIdentity)OpenNGS.Net.ProtoPool.Instance.Get(typeof(AccountIdentity));
		}
		public static CharacterInfo SpawnFromPool()
		{
			return (CharacterInfo)OpenNGS.Net.ProtoPool.Instance.Get(typeof(CharacterInfo));
		}
	}
	public partial class MiniGameUrl : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			service_name = null;
			service_id = 0;
			game_tid = 0;
			game_uid = 0;
			game_mode = 0;
			game_level = 0;
		}
		public void OnRelease()
		{
			service_name = null;
			service_id = 0;
			game_tid = 0;
			game_uid = 0;
			game_mode = 0;
			game_level = 0;
		}
		public void OnSpawn()
		{
		}
		public static MiniGameUrl SpawnFromPool()
		{
			return (MiniGameUrl)OpenNGS.Net.ProtoPool.Instance.Get(typeof(MiniGameUrl));
		}
	}
	public partial class MiniGameLevelData : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			level = 0;
			mode = 0;
			timestamp = 0;
			status = 0;
			score = 0;
			time_cost = 0;
			progress = 0;
			grade = 0;
		}
		public void OnRelease()
		{
			level = 0;
			mode = 0;
			timestamp = 0;
			status = 0;
			score = 0;
			time_cost = 0;
			progress = 0;
			grade = 0;
		}
		public void OnSpawn()
		{
		}
		public static MiniGameLevelData SpawnFromPool()
		{
			return (MiniGameLevelData)OpenNGS.Net.ProtoPool.Instance.Get(typeof(MiniGameLevelData));
		}
	}
	public partial class MiniGamePlayerData : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			uin = 0;
			openid = 0;
		}
		public void OnRelease()
		{
			uin = 0;
			openid = 0;
		}
		public void OnSpawn()
		{
		}
		public static MiniGamePlayerData SpawnFromPool()
		{
			return (MiniGamePlayerData)OpenNGS.Net.ProtoPool.Instance.Get(typeof(MiniGamePlayerData));
		}
	}
	public partial class MiniGameData : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			appid = 0;
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.MiniGameUrl>(game_url);
			OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.RoomUrl>(room_url);
			roomtid = 0;
			create_time = 0;
			for(int playersCount = 0; playersCount < players.Count; playersCount++)
			{
				OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.MiniGamePlayerData>(players[playersCount]);
			}
			players.Clear();
		}
		public void OnRelease()
		{
			appid = 0;
			OpenNGS.Net.ProtoPool.Instance.Release(game_url);
			OpenNGS.Net.ProtoPool.Instance.Release(room_url);
			roomtid = 0;
			create_time = 0;
			for(int playersCount = 0; playersCount < players.Count; playersCount++)
			{
				OpenNGS.Net.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.MiniGamePlayerData>(players[playersCount]);
			}
			players.Clear();
		}
		public void OnSpawn()
		{
			 game_url = (MiniGameUrl)OpenNGS.Net.ProtoPool.Instance.Get(typeof(MiniGameUrl));
			 room_url = (RoomUrl)OpenNGS.Net.ProtoPool.Instance.Get(typeof(RoomUrl));
			for(int playersCount = 0; playersCount < players.Count; playersCount++)
			{
				 players[playersCount] = (MiniGamePlayerData)OpenNGS.Net.ProtoPool.Instance.Get(typeof(MiniGamePlayerData));
			}
		}
		public static MiniGameData SpawnFromPool()
		{
			return (MiniGameData)OpenNGS.Net.ProtoPool.Instance.Get(typeof(MiniGameData));
		}
	}
	public partial class MiniGameMsgHead : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			appid = 0;
			gametid = 0;
			gameuid = 0;
			token = null;
		}
		public void OnRelease()
		{
			appid = 0;
			gametid = 0;
			gameuid = 0;
			token = null;
		}
		public void OnSpawn()
		{
		}
		public static MiniGameMsgHead SpawnFromPool()
		{
			return (MiniGameMsgHead)OpenNGS.Net.ProtoPool.Instance.Get(typeof(MiniGameMsgHead));
		}
	}
	public class NGSCommonPBFactory
	{
		public static void  AutoRegistePBFactory()
		{
			 RuntimeTypeModel.Default.Add(typeof(OwnerInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(ItemData), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(ItemList), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(ItemSet), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(Queue), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(QueueSet), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(QuestNode), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(QuestNodeGroup), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(TechTree), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(Tech), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(SeasonInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(RankRow), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(RankPage), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(RankType), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(ServiceInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(GetRequest), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(PlayerIdentifier), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(ListRequest), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(IdRequest), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(ResponseResult), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(MailSenderInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(MailReceiverInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(PlayerBaseInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(PlayerStatusInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(ChannelInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(ChatInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(MatchPlayerAttribute), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(MatchPlayer), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(MatchTicket), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(MatchResultPlayer), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(MatchCamp), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(MatchResult), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(RoomStatus), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(RoomUrl), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(RoomSetting), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(Vector3), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(EntityKey), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(EntityPos), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(EntityAttr), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(Entity), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(PlayerDesc), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(StatusData), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(StatusDataList), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(DirtyMarkInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(RoomData), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(InviteInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(RequestEnterInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(PlayerSocialStatusData), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(UserSettingValueItem), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(ObjectiveNode), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(RankingKey), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(RankingType), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(RankingNode), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(RankPlayerBaseInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(RankGuildBaseInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(RankBaseInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(LimitInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(AccountIdentity), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(AccountInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(UserInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(CharacterInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(MiniGameUrl), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(MiniGameLevelData), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(MiniGamePlayerData), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(MiniGameData), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(MiniGameMsgHead), true).SetFactory("SpawnFromPool");
		}
	}
}
