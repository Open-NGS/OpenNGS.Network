
namespace OpenNGS.Platform
{
    public enum OPENNGS_PLATFORM_MODULE
    {
        Base,
        Users,
        DeepLinking,
        IAP,
        Leaderboards,
        Achievement,
        Sharing,
        Room,
        Social,
        Friends,
        MUDULE_COUNT,
    }

    public enum OPENNGS_PLAT_RESULT
    {
        Success = 0,
        Unknown = -1,
    }

    public enum OPENNGS_SAVE_DATA_PATH_TYPE
    {
        Saved_Game,
        Persistent,
        Install
    }
}