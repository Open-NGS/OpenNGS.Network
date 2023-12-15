using Newtonsoft.Json;
using System;
using OpenNGS.Logs;

public class OpenNGSDebug
{

    const string DebugTag = "Debug";

    public static void LogJson(string message, object obj)
    {
        OpenNGSDebug.Log(message + ":" + JsonConvert.SerializeObject(obj));
    }
    
    public static void LogJsonError(string message, object obj)
    {
        OpenNGSDebug.LogError(message + ":" + JsonConvert.SerializeObject(obj));
    }
    
    public static void LogErrorJson(string message, object obj)
    {
        OpenNGSDebug.LogError(message + ":" + JsonConvert.SerializeObject(obj));
    }

    public static void Log(object message)
    {
        OpenNGSDebug.LogFormat("{0}", message);
    }

    public static void LogError(object message)
    {
        OpenNGSDebug.LogErrorFormat("{0}", message);
    }

    public static void LogFormat(string format, params object[] args)
    {
        LogSystem.LogFormat(null, LogType.Log, null, format, args);
    }
    public static void LogFormat(string tag, string format, params object[] args)
    {
        LogSystem.LogFormat(tag, LogType.Log, null, format, args);
    }

    public static void DebugFormat(string format, params object[] args)
    {
        LogFormat(DebugTag, format, args);
    }

    public static void LogErrorFormat(string format, params object[] args)
    {
        LogSystem.LogFormat(null, LogType.Error, null, format, args);
    }
    public static void LogErrorFormat(string tag, string format, params object[] args)
    {
        LogSystem.LogFormat(tag,LogType.Error, null, format, args);
    }

    public static void LogException(Exception ex)
    {
        LogSystem.LogException(null, ex, null);
    }
}
