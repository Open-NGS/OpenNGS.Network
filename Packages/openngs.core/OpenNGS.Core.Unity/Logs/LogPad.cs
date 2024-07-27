using OpenNGS.Logs;
using OpenNGS.Logs.Appenders;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogPad : MonoBehaviour
{
    public Transform logPad;
    public Transform logButton;

    public static LogPad Instance;

    public Text logText;
    public Scrollbar scroll;


    LogPadAppender appender;

    /// <summary>
    /// Battle Log Appender
    /// </summary>
    class LogPadAppender : BaseAppender
    {
        public override string TypeIdentify => "LogPadAppender";

        private Text output;
        public List<string> lines = new List<string>();

        public LogPadAppender(string name, Text text) : base(name)
        {
            output = text;
        }

        public override void Init(AppenderConfig config)
        {
            base.Init(new AppenderConfig()
            {
                Enable = true,
                FilterTags = LogSystem.Tag,
                FilterType = LogFilter.FilterType.Log,
                Format = AppenderConfig.LogFormat.Full,
                Type = "LogPadAppender"
            });
        }

        public override void AppendFormat(string tag, OpenNGS.Logs.LogType logType, object context, string format, params object[] args)
        {
            lines.Add(LogSystem.Time + "[" + tag + "][" + logType.ToString() + "]" + string.Format(format, args));
        }

        public override void AppendException(string tag, System.Exception exception, object context)
        {

        }
    }

    internal void Init()
    {
        appender = new LogPadAppender("LogPad",this.logText);
        LogSystem.AddLogAppender(appender);
    }

    void Awake()
    {
#if (UNITY_SWITCH || UNITY_ANDROID) && (DEBUG_LOG)
        DontDestroyOnLoad(this);
        Instance = this;
        logButton.gameObject.SetActive(true);
        this.Init();
#else
        Destroy(this);
#endif
    }

    public void OnLogPad()
    {
        bool show = !logPad.gameObject.activeSelf;
        logPad.gameObject.SetActive(show);
        ShowLog(appender.lines.Count);
    }

    public void OnScroll(float val)
    {
        if (appender == null) return;
        int line = (int)(appender.lines.Count * val);
        ShowLog(line);
    }

    void ShowLog(int i)
    {
        int start = Math.Max(0, i - 60);
        int end = i;

        var list = appender.lines.GetRange(start, end - start);
        logText.text = string.Join("\n", list);
    }
}
