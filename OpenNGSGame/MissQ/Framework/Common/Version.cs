/*
 * Copyright (c) 2016 
 *
 * time:   2016-12-09
 * file:   Version.cs
 * desc:   版本号
 *
 */
namespace MissQ
{
	public class Version
    {
        // 大版本发布必须修改此变量
		public static string VersionNumber = "0.1.10.0";
        public static string ResVersionNumber = "0.1.10.1";

        // 从字符串版本号转换到UInt64整数版本号
		public static ulong GetVersionNumUInt64(string version)
        {
            var numArray = version.Split('.');
            if (numArray.Length != 4)
            {
                return 0;
            }

            return (ulong.Parse(numArray[0]) << 48) + (ulong.Parse(numArray[1]) << 32) +
                    (ulong.Parse(numArray[2]) << 16) + (ulong.Parse(numArray[3]));
        }

        // 从整数版本号换到字符串版本号
		public static string GetVersionString(ulong version)
        {
            return string.Format("{0}.{1}.{2}.{3}", version >> 48, (version << 16) >> 48, (version << 32) >> 48, (version << 48) >> 48);
        }
    }
}
