using OpenNGS.Logs;
using OpenNGS.Logs.Appenders;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 拦截 Unity 的日志处理
/// </summary>
public class UnityLoggerHandler : ILogHandler
{
    public void LogFormat(UnityEngine.LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        LogSystem.LogFormat("Unity", (OpenNGS.Logs.LogType)logType, context, format, args);
    }

    public void LogException(Exception exception, UnityEngine.Object context)
    {
        //UnityAppender.logger.LogException(exception, context);
        LogSystem.LogException("Unity", exception, context);
    }
}
