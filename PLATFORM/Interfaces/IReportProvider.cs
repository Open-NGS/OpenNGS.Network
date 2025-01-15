using static OpenNGS.Platform.PlatformReport;

namespace OpenNGS.Platform
{
    public interface IReportProvider : IModuleProvider
    {
        void Report(string eventId, ExtraInfo extraInfo);
    }
}