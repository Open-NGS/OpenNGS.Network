using UnityEngine;

namespace OpenNGS.Platform
{
    public class NGSPlatform
    {

        static IPlatformProvider PlatformProvider;
        public static void Init(IPlatformProvider provider)
        {
#if DEBUG_LOG
            Debug.Log("NGSPlatform.Init()");
#endif
            PlatformProvider = provider;
            for (int i = 0; i < (int)OPENNGS_PLATFORM_MODULE.MUDULE_COUNT; i++)
            {
                provider.CreateMocule((OPENNGS_PLATFORM_MODULE)i);
            }
            PlatformProvider.Init();
        }

        public static bool IsSupported(OPENNGS_PLATFORM_MODULE module)
        {
            return PlatformProvider.IsSupported(module);
        }

        public static IAppModule App
        {
            get { return (IAppModule)PlatformProvider.GetModule(OPENNGS_PLATFORM_MODULE.Base); }
        }

        public static IUsersModule Users
        {
            get { return (IUsersModule)PlatformProvider.GetModule(OPENNGS_PLATFORM_MODULE.Users); }
        }

        public static IDeepLinkingModule DeepLinking
        {
            get { return (IDeepLinkingModule)PlatformProvider.GetModule(OPENNGS_PLATFORM_MODULE.DeepLinking); }
        }

        public static IIAPModule IAP
        {
            get { return (IIAPModule)PlatformProvider.GetModule(OPENNGS_PLATFORM_MODULE.IAP); }
        }

        public static ISharingModule Sharing
        {
            get { return (ISharingModule)PlatformProvider.GetModule(OPENNGS_PLATFORM_MODULE.Sharing); }
        }

        public static ILeaderboardsModule Leaderboards
        {
            get { return (ILeaderboardsModule)PlatformProvider.GetModule(OPENNGS_PLATFORM_MODULE.Leaderboards); }
        }

        public static IAchievementModule Achievements
        {
            get { return (IAchievementModule)PlatformProvider.GetModule(OPENNGS_PLATFORM_MODULE.Achievement); }
        }

        public static IRoomModule Rooms
        {
            get { return (IRoomModule)PlatformProvider.GetModule(OPENNGS_PLATFORM_MODULE.Room); }
        }
        public static IFriendsModule Friends
        {
            get { return (IFriendsModule)PlatformProvider.GetModule(OPENNGS_PLATFORM_MODULE.Friends); }
        }
    }
}
