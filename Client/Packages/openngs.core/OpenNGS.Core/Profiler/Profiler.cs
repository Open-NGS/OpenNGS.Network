using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNGS.Profiling
{
    public class Profiler
    {
        public static void BeginSample(string name)
        {
#if PROFILER
            UnityEngine.Profiling.Profiler.BeginSample(name);
#endif
        }

        public static void EndSample()
        {
#if PROFILER
            UnityEngine.Profiling.Profiler.EndSample();
#endif
        }
    }
}
