using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 日志记录器接口
/// </summary>
public interface ILogger
{
    void LogException(Exception exception, UnityEngine.Object context);
    void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args);
    void LogCombat(string log);

    void Flush();
	void Roll();
    void Close();
}
