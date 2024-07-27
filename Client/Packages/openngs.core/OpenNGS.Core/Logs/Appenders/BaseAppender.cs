using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNGS.Logs.Appenders
{
    public abstract class BaseAppender
    {
        public string Name { get; set; }
        public BaseAppender(string name)
        {
            this.Name = name;
        }
        protected LogFilter Filter;
        protected AppenderConfig Config;
        public abstract string TypeIdentify { get; }

        public bool Enable = false;

        public virtual void Init(AppenderConfig config)
        {
            this.Config = config;
            this.Filter = new LogFilter(config.FilterType, config.FilterTags);
            this.Enable = config.Enable;
        }

        public void LogException(string tag, Exception exception, object context)
        {
            if (this.Enable && this.Filter.IsFiltered(LogType.Exception, tag))
                this.AppendException(tag, exception, context);
        }

        public void LogFormat(string tag, LogType logType, object context, string format, params object[] args)
        {
            if (this.Enable && this.Filter.IsFiltered(logType, tag))
                this.AppendFormat(tag, logType, context, format, args);
        }

        public abstract void AppendFormat(string tag, LogType logType, object context, string format, params object[] args);
        public abstract void AppendException(string tag, Exception exception, object context);
    }
}
