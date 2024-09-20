using System.Collections.Generic;
using System;
using OpenNGS.Logs;
using System.Diagnostics;
using UnityEngine;

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

    }

	//public static void LogError(string log,UnityEngine.Object obj = null)
 //   {
	//	if (Enable)
 //       {
 //           string stack = "";
 //           stack = String.Format("\nStackTrace:\n\t{0}\n", UnityEngine.StackTraceUtility.ExtractStackTrace().Replace("\n","\n\t"));
 //           for (int i = 0; i < loggers.Count; i++)
 //           {
 //               if (loggers[i] is FileLogger)
 //                   log += stack;
 //               loggers[i].LogFormat(LogType.Error, null, "{0}", log);
 //           }
 //       }
 //   }


    public static void LogCombat(string log)
    {
        if (Enable && EnableBattleLog)
        {
            NgDebug.LogFormat("Battle", "{0}", log);
        }
    }

    public static void LogError(string message)
    {
        if (Enable)
            LogErrorFormat("{0}", message);
    }

    public static void LogErrorFormat(string format, params object[] args)
    {
        if (Enable)
            NgDebug.LogErrorFormat("Neptune", format, args);
    }

    public static void Log(string message)
    {
        if (Enable)
            NgDebug.LogFormat("Neptune", "{0}", message);
    }

    public static void LogWarning(string message)
    {
        if (Enable)
            NgDebug.LogWarningFormat("Neptune", "{0}", message);
    }

    public static void Roll()
    {
       
    }
}