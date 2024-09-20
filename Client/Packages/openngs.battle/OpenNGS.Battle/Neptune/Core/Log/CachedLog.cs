using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CachedLog
{
    const int CacheSize = 55000;
    const int FlushThreshold = 15000;
    public static StringBuilder logCache = new StringBuilder("", CacheSize);
    public static void Log(string txt)
    {
        logCache.AppendLine(txt);
        if (logCache.Length > FlushThreshold)
        {
            FlushLog();
        }

    }

    public static void FlushLog()
    {
        if (logCache.Length > 0)
            Debug.Log(logCache.ToString());
        logCache.Length = 0;//清空sb
    }
}
