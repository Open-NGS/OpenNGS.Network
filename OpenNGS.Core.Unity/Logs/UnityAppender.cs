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

        //new Logger(new DebugLogHandler());

        public static ILogHandler logger;

        public UnityAppender() : base("Unity")
        {
            InitUnityLoggerHandler();
        }

        static void InitUnityLoggerHandler()
        {
            if (logger == null)
            {
                logger = UnityEngine.Debug.unityLogger.logHandler;
            }
            if (UnityEngine.Debug.unityLogger.logHandler is not UnityLoggerHandler)
            {
                UnityEngine.Debug.unityLogger.logHandler = new UnityLoggerHandler();
            }
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
            logger.LogException(exception, context as UnityEngine.Object);
        }
    }
}
