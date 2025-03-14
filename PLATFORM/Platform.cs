using OpenNGS.SDK.Core;
using OpenNGS.SDK.Core.Initiallization;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.Platform
{
    public class Platform
    {

        public delegate void OnPlatformRetEventHandler<T>(T ret);
        static ISDKProvider SDKProvider;
        static IModuleProvider[] Modules = new IModuleProvider[(int)PLATFORM_MODULE.MUDULE_COUNT];

        public static bool Initialized { get; set; }

        public static bool Init(ISDKProvider provider, OpenNGS.SDK.Log.ILogger logger)
        {
            InitOptions(logger);
            SDKProvider = provider;
            for (int i = 0; i < (int)PLATFORM_MODULE.MUDULE_COUNT; i++)
            {
                Modules[i] = provider.CreateProvider((PLATFORM_MODULE)i);
            }
            if (!SDKProvider.Initialize())
                return false;
            Initialized = true;

            Start();
            return true;
        }
        private static void InitOptions(OpenNGS.SDK.Log.ILogger logger)
        {
            PlatformSettingsManager.Initialize();
            InitializationOptions options = new InitializationOptions();
            string notice = PlatformSettingsManager.GetPlatformNoticeUrl();
            string avator = PlatformSettingsManager.GetPlatformAvatarUrl();
            string auth = PlatformSettingsManager.GetPlatformAuthUrl();
            string report = PlatformSettingsManager.GetPlatformReportUrl();
            options.UrlNotice = notice;
            options.UrlAvator = avator;
            options.UrlAuth = auth;
            options.UrlReport = report;

            TextAsset textAsset = Resources.Load<TextAsset>("EEGames");
            string appid = "";
            string secret = "";
            if (textAsset != null)
            {
                // 将文本内容按行分割
                string[] lines = textAsset.text.Split('\n');

                // 创建一个字典来存储键值对
                Dictionary<string, string> configDict = new Dictionary<string, string>();

                foreach (string line in lines)
                {
                    // 按等号分割每行
                    string[] parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        // 去除空白字符并添加到字典中
                        configDict[parts[0].Trim()] = parts[1].Trim();
                    }
                }

                // 从字典中获取appid和secret
                if (configDict.ContainsKey("appid"))
                {
                    appid = configDict["appid"];
                }

                if (configDict.ContainsKey("secret"))
                {
                    secret = configDict["secret"];
                }

                // 打印读取到的值
                Debug.Log("AppID: " + appid);
                Debug.Log("Secret: " + secret);
            }
            else
            {
                Debug.LogError("Config file not found!");
            }
            options.AppId = appid;
            options.AppSecret = secret;
            OpenNGSPlatformServices.Initialize(options, logger);
        }
        internal static bool IsSupported(PLATFORM_MODULE module)
        {
            return Modules[(int)module] != null;
        }

        internal static IAppProvider GetApp()
        {
            return (IAppProvider)Modules[(int)PLATFORM_MODULE.APP];
        }

        internal static ICasProvider GetCas()
        {
            return (ICasProvider)Modules[(int)PLATFORM_MODULE.CAS];
        }
        internal static IIAPProvider GetIAP()
        {
            return (IIAPProvider)Modules[(int)PLATFORM_MODULE.IAP];
        }
        internal static ILoginProvider GetLogin()
        {
            return (ILoginProvider)Modules[(int)PLATFORM_MODULE.LOGIN];
        }
        internal static INoticeProvider GetNotice()
        {
            return (INoticeProvider)Modules[(int)PLATFORM_MODULE.NOTICE];
        }

        internal static Pay.IPayServiceProvider GetPay()
        {
            return (Pay.IPayServiceProvider)Modules[(int)PLATFORM_MODULE.PAY];
        }

        internal static IDirProvider GetDir()
        {
            return (IDirProvider)Modules[(int)PLATFORM_MODULE.DIR];
        }

        internal static IReportProvider GetReport()
        {
            return (IReportProvider)Modules[(int)PLATFORM_MODULE.REPORT];
        }
        internal static IPushProvider GetPush()
        {
            return (IPushProvider)Modules[(int)PLATFORM_MODULE.PUSH];
        }

        internal static IActivityProvider GetActivity()
        {
            return (IActivityProvider)Modules[(int)PLATFORM_MODULE.ACTIVITY];

        }
        internal static IAchievementProvider GetAchievement()
        {
            return (IAchievementProvider)Modules[(int)PLATFORM_MODULE.ACHIEVEMENT];

        }
        internal static IRemoteStorageProvider GetRemoteStorage()
        {
            return (IRemoteStorageProvider)Modules[(int)PLATFORM_MODULE.REMOTE_STORAGE];
        }
        internal static IUserProvider GetUser()
        {
            return (IUserProvider)Modules[(int)PLATFORM_MODULE.USER];
        }
        public static void Start()
        {
            for (int i = 0; i < (int)PLATFORM_MODULE.MUDULE_COUNT; i++)
            {
                Modules[i]?.Start();
            }
        }

        public static void Update()
        {
            SDKProvider.Update();
            for (int i = 0; i < (int)PLATFORM_MODULE.MUDULE_COUNT; i++)
            {
                Modules[i]?.Update();
            }
        }

        public static void Terminate()
        {
            SDKProvider.Terminate();
        }
    }
}
