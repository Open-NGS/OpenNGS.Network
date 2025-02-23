
using System.Collections.Generic;

public class NetworkAdapterOpenNGS : INetworkAdapter
{
    public override void ReportLoadLevel(string sceneName)
    {

    }
    public override void ReportLoadLevelCompleted()
    {

    }
    public override float GetDownloadSizeKB()
    {
        return 0;
    }
    public override void ReportStageChange(StageEnum stage)
    {

    }
    public override void FirstReportGameInfo()
    {

    }
    public override void ReportBeaconData(string eventName, Dictionary<string, string> paramsDic,
        string spChannels = "", bool isRealTime = true)
    {

    }
    public override string GetAddrByName(string domain)
    {
        return string.Empty;
    }
    public override bool IsAppUpdate()
    {
        return false;
    }
}

