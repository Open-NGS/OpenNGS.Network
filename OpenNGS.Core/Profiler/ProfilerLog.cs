using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNGS;
using Debug = UnityEngine.Debug;

namespace OpenNGS.Profiling
{
    public class ProfilerLog
    {
        const string Tag = "Profiler";
        private static Dictionary<string, ProfilerData> records = new Dictionary<string, ProfilerData>();
        private static int indent = 0;

        private struct ProfilerData
        {
            public float time;
            public int indent;
        }
        public ProfilerLog()
        {
        }
        public static void Start(string key, string objectName)
        {
            Start(string.Format("{0}({1})", key, objectName));
        }
        public static void End(string key, string objectName)
        {
            End(string.Format("{0}({1})", key, objectName));
        }
        public static void Start(string key)
        {
#if PROFILER
            
            float time = Time.RealtimeSinceStartup;
            records[key] = new ProfilerData() { time = time, indent = indent };
            OpenNGSDebug.LogFormat(Tag, "{0," + indent + "}@[{1}]Begin :{2}", "", key, time);
            indent += 4;
#endif
        }
        public static void End(string key)
        {
#if PROFILER
            indent -= 4;
            if (records.ContainsKey(key))
            {
                float elapsed = Time.RealtimeSinceStartup - records[key].time;
                OpenNGSDebug.LogFormat(Tag, "{0," + records[key].indent + "}@[{1}]End :{2} - Elapsed :{3:f3}s", "", key, Time.RealtimeSinceStartup, elapsed);
            }
#endif
        }

        private float start;
        private string text;

        public ProfilerLog(string text)
        {
            this.text = text;
            Start(text);
        }

        public void Dispose()
        {
            if (this.text != null)
                End(this.text);
        }
    }
}
