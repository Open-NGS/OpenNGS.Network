using System;

public class SafeTime
{
    static long startupTick = DateTime.Now.Ticks;
    const float TicksPerSecond = 10000000f;

    public static float realtimeSinceStartup { get
        {
            return (DateTime.Now.Ticks - startupTick) / TicksPerSecond;
        }
    }
}