using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Platform
{
    public class ReportUtil
    {
        public const string EventOnlineDuration     = "OnlineDuration";
        public const string EventUserWatchAds       = "UserWatchAds";
        public const string EventUserTryWatchAds    = "UserTryWatchAds";
        public const string EventUserIAP            = "UserIAP";
        public const string EventUserIAPResult      = "UserIAPResult";
        public const string EventGameStart          = "GameStart";
        public const string EventLevelStart         = "LevelStart";
        public const string EventLevelEnd           = "LevelEnd";
        public const string EventReserve            = "Reserve";

        private object m_reportObj = null;
        private BaseReportData m_BaseReportData = null;
        public virtual string GetSerialize(object obj)
        {
            return string.Empty;
        }
        public void ReportGameStart()
        {
            SetExtra(m_reportObj);
            PlatformReport.Report(EventGameStart, PlatformReport.GetExtraInfo());
        }
        public void SetReportObj(object _obj)
        {
            m_reportObj = _obj;
            m_BaseReportData = _obj as BaseReportData;
        }
        public void Report(string eventId)
        {
            SetExtra(m_reportObj);
            PlatformReport.Report(eventId, PlatformReport.GetExtraInfo());
        }
        protected void SetExtra(object obj)
        {
            PlatformReport.GetExtraInfo().extra_1 = GetSerialize(obj);
        }
        public void ReportOnlineDuration(float onlineDuration)
        {
            m_BaseReportData.InitData();
            m_BaseReportData.online_duration = onlineDuration;
            Report(EventOnlineDuration);
        }
        public void ReportCas(string adID, uint adType, uint nRes, uint nCasTyp, bool bTry)
        {
            m_BaseReportData.InitData();
            m_BaseReportData.ad_id = adID;
            m_BaseReportData.ad_typ = adType;
            m_BaseReportData.res = nRes;
            m_BaseReportData.ad_casTyp = nCasTyp;
            m_BaseReportData.ad_try = bTry;
            Report(EventUserWatchAds);
        }
        public void ReportLevelStart(int levelID)
        {
            m_BaseReportData.InitData();
            m_BaseReportData.level_id = levelID;
            Report(EventLevelStart);
        }

        public void ReportGameEnd(int levelID, uint res)
        {
            m_BaseReportData.InitData();
            m_BaseReportData.level_id = levelID;
            m_BaseReportData.level_success = res;
            Report(EventLevelEnd);
        }
        public void ReportShare(int index)
        {
            m_BaseReportData.InitData();
            m_BaseReportData.share_id = index;
            Report(EventReserve);
        }
        public void ReportUserIAP(string strProductID, decimal dPrice, uint nRes, uint nSuccess)
        {
            m_BaseReportData.InitData();
            m_BaseReportData.Revenue = dPrice;
            m_BaseReportData.ProductID = strProductID;
            m_BaseReportData.res = nRes;
            m_BaseReportData.iap_success = nSuccess;
            Report(EventUserIAP);
        }
    }


    public class BaseReportData
    {
        private int m_level_id;
        private int m_share_id;
        private uint m_level_success;
        private string m_ad_id;
        private uint m_ad_typ;
        private uint m_res;
        private uint m_ad_casTyp;
        private bool m_ad_try;
        private uint m_iap_success;
        private string m_iap_product_id;
        private decimal m_iap_price;
        private float m_online_duration;
        public BaseReportData()
        {
            InitData();
        }
        public virtual void InitData()
        {
            m_ad_id = "";
            m_ad_try = false;
            m_ad_typ = 0;
            m_ad_casTyp = 0;
            m_level_success = 0;
            m_share_id = 0;
            m_level_id = 0;
            m_res = 0;
            m_iap_success = 0;
            m_online_duration = 0.0f;
        }
        public string ProductID
        {
            get { return m_iap_product_id; }
            set { m_iap_product_id = value; }
        }
        public decimal Revenue
        {
            get { return m_iap_price; }
            set { m_iap_price = value; }
        }
        public string ad_id
        {
            get { return m_ad_id; }
            set { m_ad_id = value; }
        }

        public bool ad_try
        {
            get { return m_ad_try; }
            set { m_ad_try = value; }
        }
        public uint ad_typ
        {
            get { return m_ad_typ; }
            set { m_ad_typ = value; }
        }
        public uint ad_casTyp
        {
            get { return m_ad_casTyp; }
            set { m_ad_casTyp = value; }
        }        
        public uint res
        {
            get { return m_res; }
            set { m_res = value; }
        }
        public uint iap_success
        {
            get { return m_iap_success; }
            set { m_iap_success = value; }
        }
        public float online_duration
        {
            get { return m_online_duration; }
            set { m_online_duration = value; }
        }
        public uint level_success
        {
            get { return m_level_success; }
            set { m_level_success = value; }
        }
        public int level_id
        {
            get { return m_level_id; }
            set { m_level_id = value; }
        }
        public int share_id
        {
            get { return m_share_id; }
            set { m_share_id = value; }
        }
    }
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
                version_number = UnityEngine.Application.version;
                platform_id = UnityEngine.Application.platform.ToString();
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
                uniqueid = UnityEngine.SystemInfo.deviceUniqueIdentifier;
            }

            [JsonIgnore]
            public string version_number
            {
                get { return extraMap["version_number"].ToString(); }
                set { extraMap["version_number"] = value; }
            }

            [JsonIgnore]
            public string platform_id
            {
                get { return extraMap["platform_id"].ToString(); }
                set { extraMap["platform_id"] = value; }
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
