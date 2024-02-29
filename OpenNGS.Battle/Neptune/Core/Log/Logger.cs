using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

public enum LogLevel
{
    Exception = 0,
    Error = 1,
    Assert = 2,
    Warning = 3,
    Log = 4,
    Combat = 5,
}

public enum LogFilter
{
    Exception = 1,
    Error = 2,
    Assert = 4,
    Warning = 8,
    Log = 16,
    Combat = 32 
}

class LogHandler : ILogHandler
{

    public void LogException(Exception exception, UnityEngine.Object context)
    {
        Logger.LogException(exception, context);
    }

    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        Logger.LogFormat(logType, context, format, args);
    }
}

public class LogConfig
{
    public string Name;
    public string Type;
    public bool Enable;
    public LogLevel LogLevel;
    public int LogFilter;
    public string LogFile;
    public bool Roll;
}

public class Logger
{
    static int frame = 0;
    private static List<ILogger> loggers = new List<ILogger>();

    static ILogHandler internalHandler;

    static int indent = 0;
    public static string Indent
    {
        get { return new string(' ', indent); }
    }

    public static void AddIndent() { indent += 4; }
    public static void DecIndent() { indent -= 4;if (indent < 0) indent = 0; }

    public static bool Enable = true;
    public static bool EnableBattleLog = false;
    public static LogLevel LogLevel = LogLevel.Log;

    static Logger()
    {
#if !UNITY_EDITOR && UNITY_IPHONE
        Logger.AppendLogger(new IOSConsole(LogLevel.Log, 31));
#endif
#if !UNITY_EDITOR && UNITY_ANDROID
        Logger.AppendLogger(new AndroidConsole(LogLevel.Log, 31));
#endif
    if (internalHandler == null)
        internalHandler = UnityEngine.Debug.unityLogger.logHandler;
#if UNITY_EDITOR
        Logger.AppendLogger(new UnityLogger(LogLevel.Log, 31));
#endif
    }

    public static void Init(List<LogConfig> configs)
    {
        UnityEngine.Debug.unityLogger.logHandler = new LogHandler();

        Application.logMessageReceivedThreaded += Application_logMessageReceivedThreaded;
        loggers.Clear();
        foreach (LogConfig config in configs)
        {
            if (!config.Enable)
                continue;
            switch (config.Type)
            {
                case "Unity":
                    Logger.AppendLogger(new UnityLogger(config.LogLevel, config.LogFilter));
                    break;
                case "File":
#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
                    if (PlatformTools.HGGetSystemFreeSpace() < 5)
                        continue;
#endif
                    FileLogger tempFileObject = new FileLogger(config.LogFile, config.LogLevel, config.LogFilter, config.Roll);
                    Logger.AppendLogger(tempFileObject);
                    break;
                case "Console":
#if UNITY_STANDALONE_WIN
                    Logger.AppendLogger(new WinConsole(config.LogLevel, config.LogFilter));
#endif
#if !UNITY_EDITOR && UNITY_IPHONE
                    Logger.AppendLogger(new IOSConsole(config.LogLevel, config.LogFilter));
#endif
#if !UNITY_EDITOR && UNITY_ANDROID
                    Logger.AppendLogger(new AndroidConsole(config.LogLevel, config.LogFilter));
#endif
                    break;
            }
        }
    }

    public static void Close()
    {
        foreach (ILogger logger in loggers)
        {
            logger.Close();
        }
    }

    public static string Time
    {
        get
        {
            //if (System.Threading.Thread.CurrentThread.ManagedThreadId == 1)
            //{
            //    frame = UnityEngine.Time.frameCount;
            //}
            //return DateTime.Now.ToString("[" + System.Threading.Thread.CurrentThread.ManagedThreadId + ":" + frame + ":" + SafeTime.realtimeSinceStartup.ToString("f6") + "][HH:mm:ss.ffffff zz] " + Logger.Indent);
            return DateTime.Now.ToString("[HH:mm:ss.ffffff zz] " + Logger.Indent);
        }
    }

    private static void Application_logMessageReceivedThreaded(string condition, string stackTrace, LogType type)
    {
//        if (!string.IsNullOrEmpty(condition) && condition.IndexOf("[UnityLogger]") >= 0)
//            return;
        if (Enable)
        {
            for (int i = 0; i < loggers.Count; i++)
            {
                if (type == LogType.Exception)
                    loggers[i].LogFormat(LogType.Error, null, "{0}\r\nstackTrace:\r\n{1}", condition, stackTrace);
            }
        }
    }

    public static void AppendLogger(ILogger logger)
    {
        if(logger is UnityLogger)
        {
            ((UnityLogger)logger).LogHandler = internalHandler;
        }
        loggers.Add(logger);
    }

    public static void Flush()
    {
        for (int i = 0; i < loggers.Count; i++)
        {
            loggers[i].Flush();
        }
    }


    public static void Roll()
    {
        for (int i = 0; i < loggers.Count; i++)
        {
            loggers[i].Roll();
        }
    }


    public static void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        if (Enable)
        {
            for (int i = 0; i < loggers.Count; i++)
            {
                loggers[i].LogFormat(logType, context, format, args);
            }
        }
    }

    public static void LogException(Exception exception, UnityEngine.Object context)
    {
        if (Enable)
        {
            for (int i = 0; i < loggers.Count; i++)
            {
                loggers[i].LogException(exception, context);
            }
        }
    }

    public static void Log(string log,UnityEngine.Object obj = null)
    {

		if (Enable)
        {
            for (int i = 0; i < loggers.Count; i++)
            {
                loggers[i].LogFormat(LogType.Log, null, "{0}", log);
            }
        }
    }

	public static void LogWarning(string log,UnityEngine.Object obj = null)
    {
		if (Enable)
        {
            for (int i = 0; i < loggers.Count; i++)
            {
                loggers[i].LogFormat(LogType.Warning, null, "{0}", log);
            }
        }
    }

	public static void LogError(string log,UnityEngine.Object obj = null)
    {
		if (Enable)
        {
            string stack = "";
            stack = String.Format("\nStackTrace:\n\t{0}\n", UnityEngine.StackTraceUtility.ExtractStackTrace().Replace("\n","\n\t"));
            for (int i = 0; i < loggers.Count; i++)
            {
                if (loggers[i] is FileLogger)
                    log += stack;
                loggers[i].LogFormat(LogType.Error, null, "{0}", log);
            }
        }
    }


    public static void LogCombat(string log)
    {
        if (Enable && EnableBattleLog)
        {
            for (int i = 0; i < loggers.Count; i++)
            {
                loggers[i].LogCombat(log);
            }
        }
    }

    public static string GetLogs(bool combat = false)
    {
        return "";
    }
}