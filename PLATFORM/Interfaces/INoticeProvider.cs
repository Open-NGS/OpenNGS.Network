namespace OpenNGS.Platform
{
    public interface INoticeProvider : IModuleProvider
    {
        void GetNotices(string noticeGroup, string language, int region, string partition, string extra);
    }
}