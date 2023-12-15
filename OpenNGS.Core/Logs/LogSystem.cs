using OpenNGS.Logs.Appenders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace OpenNGS.Logs
{

    public class LogSystem
    {
        public const string Tag = "Normal";

        private static ILogConfig Config;
        private static List<BaseAppender> Appenders = new List<BaseAppender>();
        private static Dictionary<string, BaseAppender> AppenderMap = new Dictionary<string, BaseAppender>();

        public static string Time
        {
            get
            {
                return DateTime.Now.ToString("[HH:mm:ss.ffffff zz]", CultureInfo.InvariantCulture) + "[" + OpenNGS.Time.frameCount + "]";
            }
        }

        private static bool Inited = false;



        public static void Init(ILogConfig config, BaseAppender[] appenders)
        {
            if (Inited) return;
            Config = config;
            Appenders.Clear();
            foreach(var appender in appenders)
            {
                AddLogAppender(appender);
            }

            if (Config.LogEnable)
            {
                foreach (var appenderConfig in Config.LogAppenders)
                {
                    if (appenderConfig.Enable)
                    {
                        var appender = AppenderMap.GetValueOrDefault(appenderConfig.Name);
                        if (appender != null && appender.TypeIdentify == appenderConfig.Type)
                        {
                            appender.Init(appenderConfig);
                        }
                    }
                }
            }
            Inited = true;
            OpenNGSDebug.LogFormat("OpenNGS Log System Init: enable:{0} appenders:{1}", Config.LogEnable, Config.LogAppenders == null ? 0 : Config.LogAppenders.Count);
        }

        public static void LogMessage(string condition, string stackTrace, LogType type)
        {
            if (string.IsNullOrEmpty(condition))
            {
                condition = "<null-condition>";
            }
            if (string.IsNullOrEmpty(stackTrace))
            {
                stackTrace = "<null>";
            }
            else
            {
                stackTrace = stackTrace.Replace("\r", "\r\n");
            }
            LogFormat(null, type, null, "{0}\r\n{1}", condition, stackTrace);
        }

        public static void AddLogAppender(BaseAppender appender)
        {
            if (AppenderMap.ContainsKey(appender.Name)) 
                return;
            AppenderMap.Add(appender.Name, appender);
            Appenders.Add(appender);
        }

        public static void RemoveLogAppender(string name)
        {

            if (AppenderMap.ContainsKey(name))
            {
                Appenders.Remove(AppenderMap[name]);
                AppenderMap.Remove(name);
            }
        }

        public static void LogException(string tag, Exception exception, object context)
        {
            if (!Inited)
            {
                return;
            }
            if (Config.LogEnable)
            {
                for (int i = 0; i < Appenders.Count; i++)
                {
                    Appenders[i].LogException(Tag, exception, context);
                }
            }
        }

        public static void LogFormat(string tag, LogType logType, object context, string format, params object[] args)
        {
            if (!Inited)
            {
                return;
            }
            if (Config.LogEnable)
            {
                for (int i = 0; i < Appenders.Count; i++)
                {
                    Appenders[i].LogFormat(tag==null? Tag : tag, logType, context, format, args);
                }
            }
        }
    }
}
