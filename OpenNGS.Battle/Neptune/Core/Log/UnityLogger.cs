using UnityEngine;
using System;

public class UnityLogger : BaseLogger, ILogger
{


    public ILogHandler LogHandler;

    public UnityLogger(LogLevel level, int filter) : base(level, filter)
    {
    }

    public void LogException(Exception exception, UnityEngine.Object context)
    {
        if (this.IsLogTypeAllowed(LogType.Exception))
        {
            LogHandler.LogException(exception, context);
        }
    }

    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        if (this.IsLogTypeAllowed(logType))
            LogHandler.LogFormat(logType, context, format, args);
    }

    public void LogCombat(string log)
    {
        if (this.IsCombatLogAllowed())
            LogHandler.LogFormat(LogType.Log, null, "[Battle]{0}", log);
    }

    public void Close()
    {
        
    }

    public void Flush()
    {
        
    }

    public void Roll()
    {

    }
}

