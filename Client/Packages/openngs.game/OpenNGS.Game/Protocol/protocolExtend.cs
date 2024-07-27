using ProtoBuf.Meta;
using OpenNGS;
using Common;
using OpenNGSCommon;
namespace protocol
{
	public partial class MsgHead : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			msgid = Opcode.OPCODE_BEGIN_NOTUSE;
			errcode = 0;
			status_data = null;
		}
		public void OnRelease()
		{
			msgid = Opcode.OPCODE_BEGIN_NOTUSE;
			errcode = 0;
			status_data = null;
		}
		public void OnSpawn()
		{
		}
		public static MsgHead SpawnFromPool()
		{
			return (MsgHead)OpenNGS.Network.ProtoPool.Instance.Get(typeof(MsgHead));
		}
	}
	public partial class HeartbeatReq : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			status = 0;
			anti_data = null;
		}
		public void OnRelease()
		{
			status = 0;
			anti_data = null;
		}
		public void OnSpawn()
		{
		}
		public static HeartbeatReq SpawnFromPool()
		{
			return (HeartbeatReq)OpenNGS.Network.ProtoPool.Instance.Get(typeof(HeartbeatReq));
		}
	}
	public partial class HeartbeatRsp : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			svr_time = 0;
		}
		public void OnRelease()
		{
			svr_time = 0;
		}
		public void OnSpawn()
		{
		}
		public static HeartbeatRsp SpawnFromPool()
		{
			return (HeartbeatRsp)OpenNGS.Network.ProtoPool.Instance.Get(typeof(HeartbeatRsp));
		}
	}
	public partial class LoginPlayerInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			uin = 0;
			nickname = null;
			avatar = null;
			limittype = 0;
			limittime = 0;
		}
		public void OnRelease()
		{
			uin = 0;
			nickname = null;
			avatar = null;
			limittype = 0;
			limittime = 0;
		}
		public void OnSpawn()
		{
		}
		public static LoginPlayerInfo SpawnFromPool()
		{
			return (LoginPlayerInfo)OpenNGS.Network.ProtoPool.Instance.Get(typeof(LoginPlayerInfo));
		}
	}
	public partial class LoginReq : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			imei = null;
			channel = 0;
			app_version = 0;
			res_version = 0;
			reason = LoginReason.LOGINREASON_NORMAL;
			language_id = 0;
			avatar_url = null;
			firebase_token = null;
		}
		public void OnRelease()
		{
			imei = null;
			channel = 0;
			app_version = 0;
			res_version = 0;
			reason = LoginReason.LOGINREASON_NORMAL;
			language_id = 0;
			avatar_url = null;
			firebase_token = null;
		}
		public void OnSpawn()
		{
		}
		public static LoginReq SpawnFromPool()
		{
			return (LoginReq)OpenNGS.Network.ProtoPool.Instance.Get(typeof(LoginReq));
		}
	}
	public partial class LoginRsp : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			result = 0;
			time = 0;
			svr_host = 0;
			svr_time = 0;
			version = null;
			for(int charactersCount = 0; charactersCount < characters.Count; charactersCount++)
			{
				OpenNGS.Network.ProtoFactory.Instance.RecycleProtocol<protocol.LoginPlayerInfo>(characters[charactersCount]);
			}
			characters.Clear();
			state = LoginState.LOGINSTATE_NONE;
		}
		public void OnRelease()
		{
			result = 0;
			time = 0;
			svr_host = 0;
			svr_time = 0;
			version = null;
			for(int charactersCount = 0; charactersCount < characters.Count; charactersCount++)
			{
				OpenNGS.Network.ProtoFactory.Instance.RecycleProtocol<protocol.LoginPlayerInfo>(characters[charactersCount]);
			}
			characters.Clear();
			state = LoginState.LOGINSTATE_NONE;
		}
		public void OnSpawn()
		{
			for(int charactersCount = 0; charactersCount < characters.Count; charactersCount++)
			{
				 characters[charactersCount] = (LoginPlayerInfo)OpenNGS.Network.ProtoPool.Instance.Get(typeof(LoginPlayerInfo));
			}
		}
		public static LoginRsp SpawnFromPool()
		{
			return (LoginRsp)OpenNGS.Network.ProtoPool.Instance.Get(typeof(LoginRsp));
		}
	}
	public partial class LogoutReq : OpenNGS.IProtoExtension
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
		public static LogoutReq SpawnFromPool()
		{
			return (LogoutReq)OpenNGS.Network.ProtoPool.Instance.Get(typeof(LogoutReq));
		}
	}
	public partial class LogoutRsp : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			nouse = 0;
		}
		public void OnRelease()
		{
			nouse = 0;
		}
		public void OnSpawn()
		{
		}
		public static LogoutRsp SpawnFromPool()
		{
			return (LogoutRsp)OpenNGS.Network.ProtoPool.Instance.Get(typeof(LogoutRsp));
		}
	}
	public partial class KickNtf : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			reason = KickReason.KICKREASON_REPEAT_LOGIN;
		}
		public void OnRelease()
		{
			reason = KickReason.KICKREASON_REPEAT_LOGIN;
		}
		public void OnSpawn()
		{
		}
		public static KickNtf SpawnFromPool()
		{
			return (KickNtf)OpenNGS.Network.ProtoPool.Instance.Get(typeof(KickNtf));
		}
	}
	public partial class CreatePlayerReq : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			nickname = null;
			language_id = 0;
		}
		public void OnRelease()
		{
			nickname = null;
			language_id = 0;
		}
		public void OnSpawn()
		{
		}
		public static CreatePlayerReq SpawnFromPool()
		{
			return (CreatePlayerReq)OpenNGS.Network.ProtoPool.Instance.Get(typeof(CreatePlayerReq));
		}
	}
	public partial class CreatePlayerRsp : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			result = 0;
			state = LoginState.LOGINSTATE_NONE;
			uin = 0;
		}
		public void OnRelease()
		{
			result = 0;
			state = LoginState.LOGINSTATE_NONE;
			uin = 0;
		}
		public void OnSpawn()
		{
		}
		public static CreatePlayerRsp SpawnFromPool()
		{
			return (CreatePlayerRsp)OpenNGS.Network.ProtoPool.Instance.Get(typeof(CreatePlayerRsp));
		}
	}
	public partial class PlayerInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			uin = 0;
			nickname = null;
			createtime = 0;
			guilduin = 0;
		}
		public void OnRelease()
		{
			uin = 0;
			nickname = null;
			createtime = 0;
			guilduin = 0;
		}
		public void OnSpawn()
		{
		}
		public static PlayerInfo SpawnFromPool()
		{
			return (PlayerInfo)OpenNGS.Network.ProtoPool.Instance.Get(typeof(PlayerInfo));
		}
	}
	public partial class EnterGameReq : OpenNGS.IProtoExtension
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
		public static EnterGameReq SpawnFromPool()
		{
			return (EnterGameReq)OpenNGS.Network.ProtoPool.Instance.Get(typeof(EnterGameReq));
		}
	}
	public partial class EnterGameRsp : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			result = 0;
			state = LoginState.LOGINSTATE_NONE;
			OpenNGS.Network.ProtoFactory.Instance.RecycleProtocol<protocol.PlayerInfo>(character);
		}
		public void OnRelease()
		{
			result = 0;
			state = LoginState.LOGINSTATE_NONE;
			OpenNGS.Network.ProtoPool.Instance.Release(character);
		}
		public void OnSpawn()
		{
			 character = (PlayerInfo)OpenNGS.Network.ProtoPool.Instance.Get(typeof(PlayerInfo));
		}
		public static EnterGameRsp SpawnFromPool()
		{
			return (EnterGameRsp)OpenNGS.Network.ProtoPool.Instance.Get(typeof(EnterGameRsp));
		}
	}
	public partial class GetPlayerInfoReq : OpenNGS.IProtoExtension
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
		public static GetPlayerInfoReq SpawnFromPool()
		{
			return (GetPlayerInfoReq)OpenNGS.Network.ProtoPool.Instance.Get(typeof(GetPlayerInfoReq));
		}
	}
	public partial class GetPlayerInfoRsp : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			result = 0;
			OpenNGS.Network.ProtoFactory.Instance.RecycleProtocol<protocol.PlayerInfo>(character);
		}
		public void OnRelease()
		{
			result = 0;
			OpenNGS.Network.ProtoPool.Instance.Release(character);
		}
		public void OnSpawn()
		{
			 character = (PlayerInfo)OpenNGS.Network.ProtoPool.Instance.Get(typeof(PlayerInfo));
		}
		public static GetPlayerInfoRsp SpawnFromPool()
		{
			return (GetPlayerInfoRsp)OpenNGS.Network.ProtoPool.Instance.Get(typeof(GetPlayerInfoRsp));
		}
	}
	public partial class Zone : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			zoneId = 0;
			version = 0;
		}
		public void OnRelease()
		{
			zoneId = 0;
			version = 0;
		}
		public void OnSpawn()
		{
		}
		public static Zone SpawnFromPool()
		{
			return (Zone)OpenNGS.Network.ProtoPool.Instance.Get(typeof(Zone));
		}
	}
	public partial class FriendBaseInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			uin = 0;
			version = 0;
			membernum = 0;
			applynum = 0;
			blacklistnum = 0;
		}
		public void OnRelease()
		{
			uin = 0;
			version = 0;
			membernum = 0;
			applynum = 0;
			blacklistnum = 0;
		}
		public void OnSpawn()
		{
		}
		public static FriendBaseInfo SpawnFromPool()
		{
			return (FriendBaseInfo)OpenNGS.Network.ProtoPool.Instance.Get(typeof(FriendBaseInfo));
		}
	}
	public partial class StatusReq : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			status = 0;
			map_ts = 0;
			pos = 0;
			for(int zonesCount = 0; zonesCount < zones.Count; zonesCount++)
			{
				OpenNGS.Network.ProtoFactory.Instance.RecycleProtocol<protocol.Zone>(zones[zonesCount]);
			}
			zones.Clear();
			first = false;
		}
		public void OnRelease()
		{
			status = 0;
			map_ts = 0;
			pos = 0;
			for(int zonesCount = 0; zonesCount < zones.Count; zonesCount++)
			{
				OpenNGS.Network.ProtoFactory.Instance.RecycleProtocol<protocol.Zone>(zones[zonesCount]);
			}
			zones.Clear();
			first = false;
		}
		public void OnSpawn()
		{
			for(int zonesCount = 0; zonesCount < zones.Count; zonesCount++)
			{
				 zones[zonesCount] = (Zone)OpenNGS.Network.ProtoPool.Instance.Get(typeof(Zone));
			}
		}
		public static StatusReq SpawnFromPool()
		{
			return (StatusReq)OpenNGS.Network.ProtoPool.Instance.Get(typeof(StatusReq));
		}
	}
	public partial class StatusRsp : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			svr_time = 0;
			for(int statusCount = 0; statusCount < status.Count; statusCount++)
			{
				OpenNGS.Network.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.StatusData>(status[statusCount]);
			}
			status.Clear();
		}
		public void OnRelease()
		{
			svr_time = 0;
			for(int statusCount = 0; statusCount < status.Count; statusCount++)
			{
				OpenNGS.Network.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.StatusData>(status[statusCount]);
			}
			status.Clear();
		}
		public void OnSpawn()
		{
			for(int statusCount = 0; statusCount < status.Count; statusCount++)
			{
				 status[statusCount] = (StatusData)OpenNGS.Network.ProtoPool.Instance.Get(typeof(StatusData));
			}
		}
		public static StatusRsp SpawnFromPool()
		{
			return (StatusRsp)OpenNGS.Network.ProtoPool.Instance.Get(typeof(StatusRsp));
		}
	}
	public partial class Attributes : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			value = null;
		}
		public void OnRelease()
		{
			value = null;
		}
		public void OnSpawn()
		{
		}
		public static Attributes SpawnFromPool()
		{
			return (Attributes)OpenNGS.Network.ProtoPool.Instance.Get(typeof(Attributes));
		}
	}
	public partial class NotifyInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			notify_tid = 0;
			for(int notify_paramsCount = 0; notify_paramsCount < notify_params.Count; notify_paramsCount++)
			{
				notify_params[notify_paramsCount] = null;
			}
			notify_params.Clear();
		}
		public void OnRelease()
		{
			notify_tid = 0;
			for(int notify_paramsCount = 0; notify_paramsCount < notify_params.Count; notify_paramsCount++)
			{
				notify_params[notify_paramsCount] = null;
			}
			notify_params.Clear();
		}
		public void OnSpawn()
		{
			for(int notify_paramsCount = 0; notify_paramsCount < notify_params.Count; notify_paramsCount++)
			{
			}
		}
		public static NotifyInfo SpawnFromPool()
		{
			return (NotifyInfo)OpenNGS.Network.ProtoPool.Instance.Get(typeof(NotifyInfo));
		}
	}
	public partial class ServiceTraceData : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			service = null;
			span = 0;
			status = 0;
		}
		public void OnRelease()
		{
			service = null;
			span = 0;
			status = 0;
		}
		public void OnSpawn()
		{
		}
		public static ServiceTraceData SpawnFromPool()
		{
			return (ServiceTraceData)OpenNGS.Network.ProtoPool.Instance.Get(typeof(ServiceTraceData));
		}
	}
	public partial class ServiceTraceDatas : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			for(int datasCount = 0; datasCount < datas.Count; datasCount++)
			{
				OpenNGS.Network.ProtoFactory.Instance.RecycleProtocol<protocol.ServiceTraceData>(datas[datasCount]);
			}
			datas.Clear();
		}
		public void OnRelease()
		{
			for(int datasCount = 0; datasCount < datas.Count; datasCount++)
			{
				OpenNGS.Network.ProtoFactory.Instance.RecycleProtocol<protocol.ServiceTraceData>(datas[datasCount]);
			}
			datas.Clear();
		}
		public void OnSpawn()
		{
			for(int datasCount = 0; datasCount < datas.Count; datasCount++)
			{
				 datas[datasCount] = (ServiceTraceData)OpenNGS.Network.ProtoPool.Instance.Get(typeof(ServiceTraceData));
			}
		}
		public static ServiceTraceDatas SpawnFromPool()
		{
			return (ServiceTraceDatas)OpenNGS.Network.ProtoPool.Instance.Get(typeof(ServiceTraceDatas));
		}
	}
	public partial class AnnouncementInfo : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			uid = 0;
			tid = 0;
			start_time_ms = 0;
			end_time_ms = 0;
			title_params = null;
			content_params = null;
		}
		public void OnRelease()
		{
			uid = 0;
			tid = 0;
			start_time_ms = 0;
			end_time_ms = 0;
			title_params = null;
			content_params = null;
		}
		public void OnSpawn()
		{
		}
		public static AnnouncementInfo SpawnFromPool()
		{
			return (AnnouncementInfo)OpenNGS.Network.ProtoPool.Instance.Get(typeof(AnnouncementInfo));
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
			return (PlayerIdentifier)OpenNGS.Network.ProtoPool.Instance.Get(typeof(PlayerIdentifier));
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
			return (GetRequest)OpenNGS.Network.ProtoPool.Instance.Get(typeof(GetRequest));
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
			return (ResponseResult)OpenNGS.Network.ProtoPool.Instance.Get(typeof(ResponseResult));
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
			return (ServiceInfo)OpenNGS.Network.ProtoPool.Instance.Get(typeof(ServiceInfo));
		}
	}
	public partial class SyncOperateNtf : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			ticketId = 0;
			uin = 0;
			operateType = 0;
			OpenNGS.Network.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.Entity>(entity);
		}
		public void OnRelease()
		{
			ticketId = 0;
			uin = 0;
			operateType = 0;
			OpenNGS.Network.ProtoPool.Instance.Release(entity);
		}
		public void OnSpawn()
		{
			 entity = (Entity)OpenNGS.Network.ProtoPool.Instance.Get(typeof(Entity));
		}
		public static SyncOperateNtf SpawnFromPool()
		{
			return (SyncOperateNtf)OpenNGS.Network.ProtoPool.Instance.Get(typeof(SyncOperateNtf));
		}
	}
	public partial class SyncEntityNtf : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			ticketId = 0;
			for(int entitysCount = 0; entitysCount < entitys.Count; entitysCount++)
			{
				OpenNGS.Network.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.Entity>(entitys[entitysCount]);
			}
			entitys.Clear();
			delEntityIds = null;
		}
		public void OnRelease()
		{
			ticketId = 0;
			for(int entitysCount = 0; entitysCount < entitys.Count; entitysCount++)
			{
				OpenNGS.Network.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.Entity>(entitys[entitysCount]);
			}
			entitys.Clear();
			delEntityIds = null;
		}
		public void OnSpawn()
		{
			for(int entitysCount = 0; entitysCount < entitys.Count; entitysCount++)
			{
				 entitys[entitysCount] = (Entity)OpenNGS.Network.ProtoPool.Instance.Get(typeof(Entity));
			}
		}
		public static SyncEntityNtf SpawnFromPool()
		{
			return (SyncEntityNtf)OpenNGS.Network.ProtoPool.Instance.Get(typeof(SyncEntityNtf));
		}
	}
	public partial class SyncEventNtf : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			eventid = 0;
			entityid = 0;
			entityparams = null;
		}
		public void OnRelease()
		{
			eventid = 0;
			entityid = 0;
			entityparams = null;
		}
		public void OnSpawn()
		{
		}
		public static SyncEventNtf SpawnFromPool()
		{
			return (SyncEventNtf)OpenNGS.Network.ProtoPool.Instance.Get(typeof(SyncEventNtf));
		}
	}
	public partial class RoomPrivilegeOperateNtf : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			uin = 0;
			privilegeType = ROOM_PRIVILEGE_TYPE.ROOM_PRIVILEGE_NIL;
		}
		public void OnRelease()
		{
			uin = 0;
			privilegeType = ROOM_PRIVILEGE_TYPE.ROOM_PRIVILEGE_NIL;
		}
		public void OnSpawn()
		{
		}
		public static RoomPrivilegeOperateNtf SpawnFromPool()
		{
			return (RoomPrivilegeOperateNtf)OpenNGS.Network.ProtoPool.Instance.Get(typeof(RoomPrivilegeOperateNtf));
		}
	}
	public partial class SyncHostNtf : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			uid = 0;
			hostuin = 0;
			version = 0;
		}
		public void OnRelease()
		{
			uid = 0;
			hostuin = 0;
			version = 0;
		}
		public void OnSpawn()
		{
		}
		public static SyncHostNtf SpawnFromPool()
		{
			return (SyncHostNtf)OpenNGS.Network.ProtoPool.Instance.Get(typeof(SyncHostNtf));
		}
	}
	public partial class SyncRoomSettingNtf : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			OpenNGS.Network.ProtoFactory.Instance.RecycleProtocol<OpenNGSCommon.RoomSetting>(setting);
		}
		public void OnRelease()
		{
			OpenNGS.Network.ProtoPool.Instance.Release(setting);
		}
		public void OnSpawn()
		{
			 setting = (RoomSetting)OpenNGS.Network.ProtoPool.Instance.Get(typeof(RoomSetting));
		}
		public static SyncRoomSettingNtf SpawnFromPool()
		{
			return (SyncRoomSettingNtf)OpenNGS.Network.ProtoPool.Instance.Get(typeof(SyncRoomSettingNtf));
		}
	}
	public partial class SyncPlayerStatusNtf : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			uin = 0;
			status = 0;
		}
		public void OnRelease()
		{
			uin = 0;
			status = 0;
		}
		public void OnSpawn()
		{
		}
		public static SyncPlayerStatusNtf SpawnFromPool()
		{
			return (SyncPlayerStatusNtf)OpenNGS.Network.ProtoPool.Instance.Get(typeof(SyncPlayerStatusNtf));
		}
	}
	public class protocolPBFactory
	{
		public static void  AutoRegistePBFactory()
		{
			 RuntimeTypeModel.Default.Add(typeof(MsgHead), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(HeartbeatReq), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(HeartbeatRsp), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(LoginPlayerInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(LoginReq), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(LoginRsp), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(LogoutReq), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(LogoutRsp), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(KickNtf), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(CreatePlayerReq), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(CreatePlayerRsp), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(PlayerInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(EnterGameReq), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(EnterGameRsp), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(GetPlayerInfoReq), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(GetPlayerInfoRsp), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(Zone), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(FriendBaseInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(StatusReq), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(StatusRsp), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(Attributes), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(CharacterInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(NotifyInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(ServiceTraceData), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(ServiceTraceDatas), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(AnnouncementInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(PlayerIdentifier), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(GetRequest), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(ResponseResult), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(ServiceInfo), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(SyncOperateNtf), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(SyncEntityNtf), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(SyncEventNtf), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(RoomPrivilegeOperateNtf), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(SyncHostNtf), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(SyncRoomSettingNtf), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(SyncPlayerStatusNtf), true).SetFactory("SpawnFromPool");
		}
	}
}
