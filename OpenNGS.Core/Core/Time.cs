using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNGS
{
    public class Time
    {
        static float gameStartTime = 0;
        static System.Diagnostics.Stopwatch systemWatch = new System.Diagnostics.Stopwatch();

        public static int frameCount { get; private set; }

        static Time()
        {
            frameCount = 0;
#if UNITY_EDITOR
            UnityEngine.Application.onBeforeRender += OnFrame;
#endif
            systemWatch.Start();
            gameStartTime = (float)systemWatch.Elapsed.TotalSeconds;
        }

        private static void OnFrame()
        {
            frameCount++;
        }

        ~Time()
        {
            systemWatch.Stop();
        }

        public static void ResetGameTime(int historyTime)
        {
            historyGameTime = historyTime;
            gameStartTime = (float)systemWatch.Elapsed.TotalSeconds;
        }

        public static int GameTime
        {
            get
            {
                return (int)(systemWatch.Elapsed.TotalSeconds - gameStartTime);
            }
        }

        private static int historyGameTime;

        public static int TotalGameTime
        {
            get
            {
                return (historyGameTime + GameTime);
            }
        }

        public static float RealtimeSinceStartup
        {
            get
            {
                return (float)systemWatch.Elapsed.TotalSeconds;
            }
        }

        public static int Timestamp
        {
            get
            {
                TimeSpan ts = System.DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1);
                return (int)ts.TotalSeconds;
            }
        }

        public static long TimestampMs
        {
            get
            {
                TimeSpan ts = System.DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1);
                return (long)ts.TotalMilliseconds;
            }
        }

        public static DateTime GetTime(int timestamp)
        {
            DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp);
            return time;
        }

        public static int GetTimestamp(DateTime time)
        {
            TimeSpan ts = time - new DateTime(1970, 1, 1);
            return (int)ts.TotalSeconds;
        }
    }
}
