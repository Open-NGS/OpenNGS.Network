using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class UFProfiler
{
    string Key = null;
    public class UIProfilerData
    {
        float start = 0;
        public float Time = 0;
        public int Calls = 0;
        public int Indent = 0;
        public UIProfilerData()
        {
            this.Indent = UFProfiler.indent;
            this.Start();
        }

        public void Start()
        {
            this.Calls++;
            this.start = SafeTime.realtimeSinceStartup;
        }

        public void End()
        {
            this.Time += SafeTime.realtimeSinceStartup - this.start;
        }
    }

    static string logfile = string.Empty;
    public static string LogFile
    {
        get { return logfile; }
        set
        {
            logfile = value;
#if !UNITY_EDITOR && !UNITY_STANDALONE
            logfile = Application.persistentDataPath + "/" + logfile;
#endif
            File.WriteAllText(logfile, "Name,Time,Calls\r\n");
        }
    }

    public static int indent = 0;
    static Dictionary<string, UIProfilerData> profilers = new Dictionary<string, UIProfilerData>();

    public static UIProfilerData Start(string key)
    {
        //key = key.PadLeft(key.Length + indent * 2, ' ');
        if (profilers.ContainsKey(key))
        {
            profilers[key].Start();

        }
        else
        {
            UIProfilerData data = new UIProfilerData();
            profilers[key] = data;
        }
        indent++;
        return profilers[key];
    }

    public static void End(string key)
    {
        indent--;
        //key = key.PadLeft(key.Length + indent * 2, ' ');
        if (profilers.ContainsKey(key))
        {
            profilers[key].End();
        }
        if (indent <= 0)
        {
            indent = 0;
            Save();
        }
    }

    public static void End(UIProfilerData data)
    {
        if (data == null)
            return;
        indent--;
        //key = key.PadLeft(key.Length + indent * 2, ' ');
        data.End();
        if (indent <= 0)
        {
            indent = 0;
            Save();
        }
    }

    public static void Reset()
    {
        profilers.Clear();
    }

    public static void Save()
    {
#if !UNITY_EDITOR && !UNITY_STANDALONE
        StringBuilder sb = new StringBuilder();
        foreach (KeyValuePair<string, UIProfilerData> data in profilers)
        {
            sb.AppendLine(string.Format("{0},{1:f8},{2}",
                data.Key.PadLeft(data.Key.Length + Mathf.Max(0, data.Value.Indent) * 4, ' '),
                data.Value.Time,
                data.Value.Calls));
        }
        profilers.Clear();
        if (!string.IsNullOrEmpty(logfile))
        {
            File.AppendAllText(logfile, sb.ToString());
        }
#endif
    }

    public UFProfiler(string key)
    {
#if DEVELOPMENT
        this.Key = key;
        UFProfiler.Start(this.Key);
#endif
    }

    ~UFProfiler()
    {
#if DEVELOPMENT
        UFProfiler.End(this.Key);
        this.Key = null;
#endif
    }
}