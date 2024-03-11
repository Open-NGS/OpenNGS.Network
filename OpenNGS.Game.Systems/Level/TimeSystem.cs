using OpenNGS;
using OpenNGS.Systems;
using System;
using Systems;

public class TimeSystem : GameSubSystem<TimeSystem>,ITimeSystem
{
    //因为考虑到游戏会暂停，所以不能够通过当前值来取时间
    private double m_dCurrentTime; 
    public long GetCurTime()
    {
        //DateTime currentTime = DateTime.Now;
        //long timeStamp = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();
        //return timeStamp;
        return (long)m_dCurrentTime;
    }
    protected override void OnCreate()
    {
        base.OnCreate();
    }


    public override string GetSystemName()
    {
        return "com.openngs.system.TimeSystem";
    }

    public void OnEnterFrame(float dt)
    {
        m_dCurrentTime += (double)dt;
    }
    public void SetStartTime(double time)
    {
        m_dCurrentTime = time;
    }
}
