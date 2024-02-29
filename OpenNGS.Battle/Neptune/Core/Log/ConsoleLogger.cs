using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.IO;


#if UNITY_STANDALONE_WIN || UNITY_EDITOR
public class WinConsole: BaseLogger, ILogger
{
    private const int STD_OUTPUT_HANDLE = -11;

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool AttachConsole(int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool AllocConsole();

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool FreeConsole();

    [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    static extern bool SetConsoleTitle(string lpConsoleTitle);


    TextWriter oldOutput;
    static FileStream fileStream;
    static StreamWriter standardOutput;


    public WinConsole(LogLevel level, int filter) : base(level, filter)
    {
        Initialize();
    }

    ~WinConsole()
    {
        Shutdown();
    }


    public void Initialize()
    {
        if (!AttachConsole(-1))
        {
            AllocConsole();
            Console.WriteLine("Console Alloced");
        }
        else
        {
            Console.WriteLine("Console Initialized");
        }

        oldOutput = Console.Out;

        try
        {
            IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            fileStream = new FileStream(stdHandle, FileAccess.Write);
            standardOutput = new StreamWriter(fileStream, System.Text.Encoding.ASCII);
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
        }
        catch (System.Exception e)
        {
            Debug.Log("Couldn't redirect output: " + e.Message);
        }
    }

    public void Shutdown()
    {
        Console.SetOut(oldOutput);
        FreeConsole();
    }

    public void LogException(Exception exception, UnityEngine.Object context)
    {
        if (this.IsLogTypeAllowed(LogType.Exception))
            Console.WriteLine(string.Format("[Exception]{0})", exception.ToString()));
    }

    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        if (this.IsLogTypeAllowed(logType))
            Console.WriteLine(Logger.Time + "[" + logType.ToString() + "]" + string.Format(format, args));
    }

    public void LogCombat(string log)
    {
        if (this.IsCombatLogAllowed())
            Console.WriteLine(Logger.Time + string.Format("[Battle]{0}", log));
    }


    public void SetTitle(string strName)
    {
        SetConsoleTitle(strName);
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
#endif

#if !UNITY_EDITOR && UNITY_ANDROID
public class AndroidConsole: BaseLogger, ILogger
{
    public AndroidConsole(LogLevel level, int filter) : base(level, filter)
    {
    }

    ~AndroidConsole()
    {
    }

    public void Close()
    {
    }

    public void Flush()
    {
    }

    public static void LogToConsole(string content)
    {
        try
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
            jc.CallStatic("LogToConsole", content);
        }
        catch (Exception e)
        {
            return;
        }
    }

    public void LogException(Exception exception, UnityEngine.Object context)
    {
        if (this.IsLogTypeAllowed(LogType.Exception))
            LogToConsole(string.Format("[Exception]{0})", exception.ToString()));
    }

    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        if (this.IsLogTypeAllowed(logType))
            LogToConsole(Logger.Time + "[" + logType.ToString() + "]" + string.Format(format, args));
    }

    public void LogCombat(string log)
    {
        if (this.IsCombatLogAllowed())
            LogToConsole(Logger.Time + string.Format("[Battle]{0}", log));
    }

    public void Roll()
    {
    }
}
#endif

#if !UNITY_EDITOR && UNITY_IPHONE
public class IOSConsole: BaseLogger, ILogger
{
    public IOSConsole(LogLevel level, int filter) : base(level, filter)
    {
    }

    ~IOSConsole()
    {
    }

    public void Close()
    {
    }

    public void Flush()
    {
    }

    [DllImport("__Internal")]
    public static extern void LogToConsole(string content);

    public void LogException(Exception exception, UnityEngine.Object context)
    {
        if (this.IsLogTypeAllowed(LogType.Exception))
            LogToConsole(string.Format("[Exception]{0})", exception.ToString()));
    }

    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        if (this.IsLogTypeAllowed(logType))
            LogToConsole(Logger.Time + "[" + logType.ToString() + "]" + string.Format(format, args));
    }

    public void LogCombat(string log)
    {
        if (this.IsCombatLogAllowed())
            LogToConsole(Logger.Time + string.Format("[Battle]{0}", log));
    }

    public void Roll()
    {

    }
}
#endif