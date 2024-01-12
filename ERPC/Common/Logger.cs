namespace OpenNGS.ERPC
{
    public delegate void Logger(string message);

    public class Log
    {
        public const int TRACE = 0;
        public const int DEBUG = 1;
        public const int INFO = 2;
        public const int ERROR = 3;

        public static Logger Logger
        {
            set
            {
                s_logger = value;
            }
        }

        public static int Priority
        {
            get { return s_priority; }
            set { s_priority = value; }
        }

        internal static Logger s_logger = null;
        internal static int s_priority = INFO;

        internal static void Trace(string msg)
        {
            if (s_logger != null && s_priority <= TRACE)
            {
                s_logger(msg);
            }
        }
        internal static void Debug(string msg)
        {
            if (s_logger != null && s_priority <= DEBUG)
            {
                s_logger(msg);
            }
        }
        internal static void Info(string msg)
        {
            if (s_logger != null && s_priority <= INFO)
            {
                s_logger(msg);
            }
        }
        internal static void Error(string msg)
        {
            if (s_logger != null && s_priority <= ERROR)
            {
                s_logger(msg);
            }
        }
    }
}
