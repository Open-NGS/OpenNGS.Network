using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class FileLogger: BaseLogger, ILogger
{
    private StreamWriter fileWriter;

    private int filter;
    private bool roll;
    private string filename;
    int rollIndex = 1;
    private string filePath = "";

    public FileLogger(string logfile, LogLevel level, int filter, bool roll) : base(level, filter)
    {
        if (fileWriter != null)
        {
            fileWriter.Close();
            fileWriter.Dispose();
        }
        fileWriter = null;
 		this.roll = roll;
        this.filename = logfile;
		
        if (Application.isMobilePlatform)
            filePath = Application.persistentDataPath + "/";

 		if (!this.roll)
        {
            try
            {
                File.Delete(filePath + this.filename);
            }catch
            {

            }
            fileWriter = File.AppendText(filePath + this.filename);
        }
        else
        {
            string name = filePath + this.filename.Replace(".", "_" + this.rollIndex + ".");
            File.Delete(name);
            fileWriter = File.AppendText(name);
        }

        if ((filter & (int)LogFilter.Combat) == (int)LogFilter.Combat)
        {
            fileWriter.AutoFlush = false;
        }
        else
        {
            fileWriter.AutoFlush = true;
        }
        this.filter = filter;

        this.Write("******" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "******");
    }

    void Write(string content)
    {
        if (fileWriter != null && fileWriter.BaseStream != null && fileWriter.BaseStream.CanWrite)
            fileWriter.WriteLine(content);
    }

    public void LogException(Exception exception, UnityEngine.Object context)
    {
        if (this.IsLogTypeAllowed(LogType.Exception))
            this.Write(Logger.Time + string.Format("[Exception]{0}", exception.ToString()));
    }

    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        if (this.IsLogTypeAllowed(logType))
            this.Write(Logger.Time + "[" + logType.ToString() + "]" + string.Format(format, args));
    }

    public void LogCombat(string log)
    {
        if (this.IsCombatLogAllowed())
            this.Write(log);
    }

    public void Close()
    {
        if (fileWriter == null)
            return;

        fileWriter.Flush();
        fileWriter.Close();
        fileWriter = null;
    }

    public void Flush()
    {
        if ((this.filter & (int)LogFilter.Combat) == (int)LogFilter.Combat)
            fileWriter.Flush();
    }

    public void Roll()
    {
        if (!this.roll)
            return;
        if (fileWriter != null)
        {
            fileWriter.Flush();
            fileWriter.Close();
            fileWriter.Dispose();
        }
        this.rollIndex++;
        fileWriter = null;
        string name = filePath + this.filename.Replace(".", "_" + this.rollIndex + ".");
        File.Delete(name);
        fileWriter = File.AppendText(name);

        if ((filter & (int)LogFilter.Combat) == (int)LogFilter.Combat)
        {
            fileWriter.AutoFlush = false;
        }
        else
        {
            fileWriter.AutoFlush = true;
        }

        this.Write("******" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "******");
    }
}