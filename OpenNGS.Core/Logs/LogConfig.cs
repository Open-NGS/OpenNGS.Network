using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNGS.Logs
{
    /// <summary>
    /// Appender config
    /// </summary>
    public struct AppenderConfig
    {
        public enum LogFormat
        {
            Full,
            Compact,
        }
        public string Name;
        public string Type;
        public bool Enable;
        public string LogFile;
        public LogFilter.FilterType FilterType;
        public string FilterTags;
        public LogFormat Format;
        public bool Roll;
    }

    public interface ILogConfig
    {
        bool LogEnable { get;}
        List<AppenderConfig> LogAppenders { get; }
    }
}
