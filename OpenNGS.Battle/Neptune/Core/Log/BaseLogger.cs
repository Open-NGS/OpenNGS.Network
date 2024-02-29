using UnityEngine;
using System.Collections;
using System;

public class BaseLogger
{
    private int logFilter;
    private LogLevel logLevel;
    public BaseLogger(LogLevel level, int filter)
    {
        this.logLevel = level;
        this.logFilter = filter;
    }

    public bool IsLogTypeAllowed(LogType logType)
    {
        if (logType == LogType.Exception && (logFilter & (int)LogFilter.Exception) == (int)LogFilter.Exception)
            return true;
        if ((int)logLevel < (int)logType)
            return false;
        if (logFilter == 0)
            return true;
        if ((logFilter & (int)LogFilter.Log) == (int)LogFilter.Log && logType == LogType.Log)
            return true;
        if ((logFilter & (int)LogFilter.Warning) == (int)LogFilter.Warning && logType == LogType.Warning)
            return true;
        if ((logFilter & (int)LogFilter.Error) == (int)LogFilter.Error && logType == LogType.Error)
            return true;
        if ((logFilter & (int)LogFilter.Assert) == (int)LogFilter.Assert && logType == LogType.Assert)
            return true;
        return false;
    }

    public bool IsCombatLogAllowed()
    {
        return (logFilter & (int)LogFilter.Combat) == (int)LogFilter.Combat;
    }
}
