using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNGS.SDK.Core.Utils
{
    public static class RandomUtil
    {
        private static Random random = new Random();

        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+";
            var result = new StringBuilder(length);
            for (var i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }
            return result.ToString();
        }
    }
}
