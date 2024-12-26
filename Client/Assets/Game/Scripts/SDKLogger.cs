
using UnityEngine;

public class SDKLogger : OpenNGS.SDK.Log.ILogger
{
    public void Log(object message)
    {
        Debug.Log(message);
    }

    public void LogFormat(string message, params object[] args)
    {
        Debug.LogFormat(message, args);
    }

    public void Error(object message)
    {
        Debug.LogError(message);
    }

    public void ErrorFormat(string message, params object[] args)
    {
        Debug.LogErrorFormat(message, args);
    }

    public void Warning(object message)
    {
        Debug.LogWarning(message);
    }

    public void WarningFormat(string message, params object[] args)
    {
        Debug.LogWarningFormat(message, args);
    }
}
