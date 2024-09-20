/**
 * Debug Controle, if unity editor or alpha, don't define DISABLE_DEBUG_LOG, if release just define DISABLE_DEBUG_LOG
 */
using System;
using UnityEngine;
using System.Diagnostics;
using System.Runtime.InteropServices;


public class Debug
{
    public static void Break()
    {
        UnityEngine.Debug.Break();
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void Assert(bool condition)
    {
        UnityEngine.Debug.Assert(condition);
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void Assert(bool condition, object message)
    {
        UnityEngine.Debug.Assert(condition, message);
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void Assert(bool condition, object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.Assert(condition, message,  context);
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void Assert(bool condition, string message)
    {
        UnityEngine.Debug.Assert(condition, message);
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void Assert(bool condition, UnityEngine.Object context)
    {
        UnityEngine.Debug.Assert(condition, context);
    }

    public static void Log(object message, UnityEngine.Object obj = null)
    {
        if (Logger.LogLevel >= LogLevel.Log)
        {
            Logger.Log(message.ToString(), obj);
        }
    }
	public static void LogWarning(object message, UnityEngine.Object obj = null)
    {
	    if (Logger.LogLevel >= LogLevel.Warning)
	    {
	        Logger.LogWarning(message.ToString(), obj);
	    }
    }
	public static void LogError(object message, UnityEngine.Object obj = null)
    {
	    if (Logger.LogLevel >= LogLevel.Error)
	    {
	        Logger.LogError(message.ToString(), obj);
	    }
    }

    public static void LogException(System.Exception e, UnityEngine.Object obj = null)
    {
        if (Logger.LogLevel >= LogLevel.Exception)
        {
            Logger.LogError(e.Message);
        }
    }

	public static void LogFormat(string format,params object[] args)
	{
	    if (Logger.LogLevel >= LogLevel.Log)
	    {
	        Logger.Log(string.Format(format, args));
	    }
	}

	public static void LogWarningFormat(string format,params object[] args)
	{
	    if (Logger.LogLevel >= LogLevel.Warning)
	    {
	        Logger.LogWarning(string.Format(format, args));
	    }
	}

	public static void LogErrorFormat(string format,params object[] args)
	{
	    if (Logger.LogLevel >= LogLevel.Error)
	    {
	        Logger.LogError(string.Format(format, args));
	    }
	}

    public static void Dump(object obj)
    {
        Debug.Log("----------------------------------------- < " + obj.GetType().ToString() + " > -----------------------------------------");
        System.Reflection.FieldInfo[] tempFieldInfoArray = obj.GetType().GetFields();
        for (int i = 0; i < tempFieldInfoArray.Length; ++i)
        {
            Debug.Log("Field : " + tempFieldInfoArray[i].Name + "\t\t\t| Type : " + tempFieldInfoArray[i].FieldType.ToString() + "\t\t\t| Value : " + tempFieldInfoArray[i].GetValue(obj).ToString());
        }
        System.Reflection.PropertyInfo[] tempPropertyInfoArray = obj.GetType().GetProperties();
        object[] tempObjectArray = null;
        for (int i = 0; i < tempPropertyInfoArray.Length; ++i)
        {
            Debug.Log("Property : " + tempPropertyInfoArray[i].Name + "\t\t\t| Type : " + tempPropertyInfoArray[i].PropertyType.ToString() + "\t\t\t| Value : " + tempPropertyInfoArray[i].GetValue(obj, tempObjectArray).ToString());
        }
        Debug.Log("----------------------------------------- < " + obj.GetType().ToString() + " > -----------------------------------------");
    }
}
