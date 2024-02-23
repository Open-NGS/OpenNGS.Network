using OpenNGS;
using OpenNGS.Systems;
using System;
using Systems;

public class TimeSystem : GameSubSystem<TimeSystem>,ITimeSystem
{
    public long GetCurTime()
    {
        DateTime currentTime = DateTime.Now;
        long timeStamp = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();
        return timeStamp;
    }
    protected override void OnCreate()
    {
        base.OnCreate();
    }


    public override string GetSystemName()
    {
        return "com.openngs.system.TimeSystem";
    }
}
