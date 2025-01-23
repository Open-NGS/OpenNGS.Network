namespace OpenNGS.Platform
{
    public class Platform
    {

        static ISDKProvider SDKProvider;
        static IModuleProvider[] Modules = new IModuleProvider[(int)PLATFORM_MODULE.MUDULE_COUNT];

        public static bool Initialized { get; set; }

        public static void Init(ISDKProvider provider)
        {
            SDKProvider = provider;
            for (int i = 0; i < (int)PLATFORM_MODULE.MUDULE_COUNT; i++)
            {
                Modules[i] = provider.CreateProvider((PLATFORM_MODULE)i);
            }
            SDKProvider.Initialize();
        }

        internal static bool IsSupported(PLATFORM_MODULE module)
        {
            return Modules[(int)module] != null;
        }

        internal static IBaseProvider GetBase()
        {
            return (IBaseProvider)Modules[(int)PLATFORM_MODULE.BASE];
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
    }
}
