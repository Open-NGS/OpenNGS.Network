using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace OpenNGS.Platform
{
    public class Hash
    {
        public static string ComputeMd5HashFile(string filename)
        {
            return ComputeMd5Hash(File.ReadAllBytes(filename));
        }

        public static string ComputeMd5HashString(string str)
        {
            return ComputeMd5Hash(Encoding.ASCII.GetBytes(str));
        }

        public static string ComputeMd5Hash(byte[] buff)
        {
            MD5 md5 = MD5.Create();
            byte[] data = md5.ComputeHash(buff);
            StringBuilder sb = new StringBuilder(data.Length * 2);
            for (int i = 0; i < data.Length; ++i)
                sb.Append(data[i].ToString("x2"));
            return sb.ToString();
        }
    }


    public class TimeUtil
    {
        public static int Timestamp
        {
            get
            {
                return (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            }
        }
    }
}