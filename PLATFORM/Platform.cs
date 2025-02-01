using System.Runtime.CompilerServices;

namespace OpenNGS.Platform
{
    public class Platform
    {

        static ISDKProvider SDKProvider;
        static IModuleProvider[] Modules = new IModuleProvider[(int)PLATFORM_MODULE.MUDULE_COUNT];

        public static bool Initialized { get; set; }

        public static bool Init(ISDKProvider provider)
        {
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

        internal static bool IsSupported(PLATFORM_MODULE module)
        {
            return Modules[(int)module] != null;
        }

        internal static IAppProvider GetApp()
        {
            return (IAppProvider)Modules[(int)PLATFORM_MODULE.APP];
        }

        internal static ILoginProvider GetLogin()
        {
            return (ILoginProvider)Modules[(int)PLATFORM_MODULE.LOGIN];
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
