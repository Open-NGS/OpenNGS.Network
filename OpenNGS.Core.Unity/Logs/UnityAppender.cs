using System;
using UnityEngine;

namespace OpenNGS.Logs.Appenders
{
    /// <summary>
    /// Unity internal appender
    /// </summary>
   public class UnityAppender : BaseAppender
    {
        public override string TypeIdentify => "Unity";

        public static ILogHandler logger;

        public UnityAppender() : base("Unity")
        {
            logger = UnityEngine.Debug.unityLogger.logHandler;
            UnityEngine.Debug.unityLogger.logHandler = new UnityLogger();
        }

        ~UnityAppender()
        {
            if (logger != null && UnityEngine.Debug.unityLogger.logHandler == this)
                UnityEngine.Debug.unityLogger.logHandler = logger;
        }

        public override void Init(AppenderConfig config)
        {
            base.Init(config);
        }

        public override void AppendFormat(string tag, LogType logType, object context, string format, params object[] args)
        {
            logger.LogFormat((UnityEngine.LogType)logType,context as UnityEngine.Object, LogSystem.Time + "[" + tag + "][" + logType.ToString() + "]" + format, args);
        }

        public override void AppendException(string tag, Exception exception, object context)
        {
            //logger.LogException(exception, context as UnityEngine.Object);
        }
    }
}
