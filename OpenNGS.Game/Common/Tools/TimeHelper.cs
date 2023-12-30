using System;
using UnityEngine;

public static class TimeHelper
{
    public const int Day = 24 * 3600;
    public const int Hour = 3600;
    public const int Min = 60;

    private static uint _svrTime;
    private static long _last;

    public static long ServerTime => _svrTime + GetUtcNowTimeStamp() - _last;

    private static readonly DateTime StartDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    
    public static void RefreshServerTime(uint svrTime)
    {
        _svrTime = svrTime;
        _last = GetUtcNowTimeStamp();
    }

    public static long GetUtcNowTimeStamp()
    {
        var ts = DateTime.UtcNow - StartDate;
        return Convert.ToInt64(ts.TotalSeconds);
    }
    
    public static long GetUtcNowTimeStampMs()
    {
        var ts = DateTime.UtcNow - StartDate;
        return Convert.ToInt64(ts.Ticks / 10000);
    }

    public static DateTime GetDateTime(long timeStamp)
    {
        return StartDate.AddSeconds(timeStamp).ToLocalTime();
    }

    public static string FormatSeconds(long seconds)
    {
        var day = seconds / Day;
        seconds -= day * Day;
        var hour = seconds / Hour;
        seconds -= hour * Hour;
        var min = seconds / Min;
        seconds %= Min;

        var dayStr = day > 0 ? GetFormatTime(day) : "00";
        var hourStr = hour > 0 ? GetFormatTime(hour) : "00";
        var minStr = min > 0 ? GetFormatTime(min) : "00";
        var sStr = seconds > 0 ? GetFormatTime(seconds) : "00";

        return $"{dayStr}:{hourStr}:{minStr}:{sStr}";
    }
    
    private static string GetFormatTime(long time)
    {
        return time < 10 ? $"0{time}" : time.ToString();
    }

    public static long GetTimeLeft(ulong serverEndTime)
    {
        return (long) Mathf.Max(0, (long) serverEndTime - ServerTime);
    }
    
    public static long GetTimeLeftOnServerStart(ulong serverStartTime)
    {
        return (long) Mathf.Max(0, ServerTime - (long) serverStartTime);
    }
}
