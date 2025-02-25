using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Platform
{
	public class PlatformReport
	{
        internal static void OnReportRet(PlatformReportRet ret)
		{

		}

        static ExtraInfo s_extraInfo = new ExtraInfo();
		public class ExtraInfo
		{
            [JsonProperty]
            public Dictionary<String, Object> extraMap = new Dictionary<String, Object>();

			public ExtraInfo()
			{
				extraMap.Add("eventname", "clientreport");//当前的事件名称，当前支持clientreport
				channel_id = "";
				client_ip = "";
				opid = "";
				opgameid = "";
				uniqueid = "";
                server_id = "";
				server_type = "";
				account = "";
				account_register_time = "";
				account_first_ingame_time = "";
				role_id = "";
				role_name = "";
				level = "";
				vip_level = "";
				role_register_time = "";
				role_power = "";
				role_union_id = "";
				role_paid = "";
				role_type = "";
				ad_user = "";
				extra_event_id = "";
				loading_step = "";
				is_key_loading_step = "";
				extra_1 = "";
				extra_2 = "";
				extra_3 = "";
				extra_4 = "";
				extra_5 = "";
			}
            [JsonIgnore]
            public string channel_id
			{
				get { return extraMap["channel_id"].ToString(); }
				set { extraMap["channel_id"] = value; }
			}
            [JsonIgnore]
            public string client_ip
			{
				get { return extraMap["client_ip"].ToString(); }
				set { extraMap["client_ip"] = value; }
			}
            [JsonIgnore]
            public string opid
			{
				get { return extraMap["opid"].ToString(); }
				set { extraMap["opid"] = value; }
			}
            [JsonIgnore]
            public string opgameid
			{
				get { return extraMap["opgameid"].ToString(); }
				set { extraMap["opgameid"] = value; }
            }
            [JsonIgnore]
            public string uniqueid
            {
                get { return extraMap["uniqueid"].ToString(); }
                set { extraMap["uniqueid"] = value; }
            }
            [JsonIgnore]
            public string server_id
			{
				get { return extraMap["server_id"].ToString(); }
				set { extraMap["server_id"] = value; }
			}
            [JsonIgnore]
            public string server_type
			{
				get { return extraMap["server_type"].ToString(); }
				set { extraMap["server_type"] = value; }
			}
            [JsonIgnore]
            public string account
			{
				get { return extraMap["account"].ToString(); }
				set { extraMap["account"] = value; }
			}
            [JsonIgnore]
            public string account_register_time
			{
				get { return extraMap["account_register_time"].ToString(); }
				set { extraMap["account_register_time"] = value; }
			}
            [JsonIgnore]
            public string account_first_ingame_time
			{
				get { return extraMap["account_first_ingame_time"].ToString(); }
				set { extraMap["account_first_ingame_time"] = value; }
			}
            [JsonIgnore]
            public string role_id
			{
				get { return extraMap["role_id"].ToString(); }
				set { extraMap["role_id"] = value; }
			}
            [JsonIgnore]
            public string role_name
			{
				get { return extraMap["role_name"].ToString(); }
				set { extraMap["role_name"] = value; }
			}
            [JsonIgnore]
            public string level
			{
				get { return extraMap["level"].ToString(); }
				set { extraMap["level"] = value; }
			}
            [JsonIgnore]
            public string vip_level
			{
				get { return extraMap["vip_level"].ToString(); }
				set { extraMap["vip_level"] = value; }
			}
            [JsonIgnore]
            public string role_register_time
			{
				get { return extraMap["role_register_time"].ToString(); }
				set { extraMap["role_register_time"] = value; }
			}
            [JsonIgnore]
            public string role_power
			{
				get { return extraMap["role_power"].ToString(); }
				set { extraMap["role_power"] = value; }
			}
            [JsonIgnore]
            public string role_union_id
			{
				get { return extraMap["role_union_id"].ToString(); }
				set { extraMap["role_union_id"] = value; }
			}
            [JsonIgnore]
            public string role_paid
			{
				get { return extraMap["role_paid"].ToString(); }
				set { extraMap["role_paid"] = value; }
			}
            [JsonIgnore]
            public string role_type
			{
				get { return extraMap["role_type"].ToString(); }
				set { extraMap["role_type"] = value; }
			}
            [JsonIgnore]
            public string ad_user
			{
				get { return extraMap["ad_user"].ToString(); }
				set { extraMap["ad_user"] = value; }
			}
            [JsonIgnore]
            public string extra_event_id
			{
				get { return extraMap["extra_event_id"].ToString(); }
				set { extraMap["extra_event_id"] = value; }
			}
            [JsonIgnore]
            public string loading_step
			{
				get { return extraMap["loading_step"].ToString(); }
				set { extraMap["loading_step"] = value; }
			}
            [JsonIgnore]
            public string is_key_loading_step
			{
				get { return extraMap["is_key_loading_step"].ToString(); }
				set { extraMap["is_key_loading_step"] = value; }
			}
            [JsonIgnore]
            public string extra_1
			{
				get { return extraMap["extra_1"].ToString(); }
				set { extraMap["extra_1"] = value; }
			}
            [JsonIgnore]
            public string extra_2
			{
				get { return extraMap["extra_2"].ToString(); }
				set { extraMap["extra_2"] = value; }
			}
            [JsonIgnore]
            public string extra_3
			{
				get { return extraMap["extra_3"].ToString(); }
				set { extraMap["extra_3"] = value; }
			}
            [JsonIgnore]
            public string extra_4
			{
				get { return extraMap["extra_4"].ToString(); }
				set { extraMap["extra_4"] = value; }
			}
            [JsonIgnore]
            public string extra_5
			{
				get { return extraMap["extra_5"].ToString(); }
				set { extraMap["extra_5"] = value; }
			}

		}

		public static ExtraInfo GetExtraInfo()
		{
#if SUPERSDK
			s_extraInfo.client_ip = SuperSDKEnv.client_ip;
			s_extraInfo.channel_id = SuperSDKEnv.channel_id;
			s_extraInfo.account = SuperSDKEnv.account;
			s_extraInfo.opid = SuperSDKEnv.opid;
            s_extraInfo.opgameid = SuperSDKEnv.opgameid;
			string server_id = SuperSDKLogin.CurrentServer?.ServerID;
			if (!string.IsNullOrEmpty(server_id))
				s_extraInfo.server_id = server_id;
#endif
			return s_extraInfo;
		}

		public static void Report(string eventId, ExtraInfo extraInfo)
		{
            if (!Platform.IsSupported(PLATFORM_MODULE.REPORT))
                return;
            IReportProvider _reportProvider = Platform.GetReport();
            if (_reportProvider != null)
            {
				_reportProvider.Report(eventId, extraInfo);
            }
        }

		public static void Report(PlatformReportEvent eventType, ExtraInfo extraInfo, bool isKeyLoadingStep)
		{
			extraInfo.is_key_loading_step = isKeyLoadingStep == true ? "1" : "0";
			Report(eventType.ToString(), extraInfo);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventType"></param>
		/// <param name="isKeyLoadingStep">是否必经步骤</param>
		public static void Report(PlatformReportEvent eventType, bool isKeyLoadingStep)
		{
			Report(eventType, GetExtraInfo(), isKeyLoadingStep);
		}

		public static void Report(PlatformReportEvent eventType, bool isKeyLoadingStep, string extra_1)
		{
			var extraInfo = GetExtraInfo();
			extraInfo.extra_1 = extra_1;
			Report(eventType, extraInfo, isKeyLoadingStep);
		}

#if SUPERSDK
		public static void Track(SuperSDKAdTrackEvent reportEvent)
		{
			Track(reportEvent, null);
		}

		/// <summary>
		/// 广告投放埋点
		/// </summary>
		/// <param name="reportEvent"></param>
		/// <param name="eventValue"></param>
		public static void Track(SuperSDKAdTrackEvent reportEvent, string eventValue)
		{

			SuperSDKReport.Track(reportEvent, eventValue);
		}
#endif
    }
    public class PlatformReportRet : PlatformBaseRet
    {

    }
}
