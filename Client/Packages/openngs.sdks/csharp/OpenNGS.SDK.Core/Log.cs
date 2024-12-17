using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNGS.SDK
{
    public static class Log
    {
        public static void Info(string msg)
        {
            Console.WriteLine($"【{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}】Info：{msg}");
        }

        public static void Error(string msg)
        {
            Console.Error.WriteLine($"【{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}】Error：{msg}");
        }
    }
}
