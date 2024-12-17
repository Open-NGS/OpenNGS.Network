using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace OpenNGS.SDK.Core.Utils
{
    public class SignUtil
    {
        /// <summary>
        /// 生成签名
        /// 加密前：appid=wx123456789&nonce=155121212121&timestamp=1684565287668&key=35AB7ECF665EF5EF44CF8640EC136300
        /// 加密后：4CD98E261F46AA75E8935695C864A26D
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string CreateSign(SortedDictionary<string, string> parameters, string key)
        {
            var sortedParams = new SortedDictionary<string, string>(parameters);
            var sb = new StringBuilder();

            foreach (var entry in sortedParams)
            {
                string k = entry.Key;
                string v = entry.Value;

                if (!string.IsNullOrEmpty(v) && k != "signature" && k != "key")
                {
                    sb.Append(k + "=" + v + "&");
                }
            }

            sb.Append("key=" + key);
            Log.Info("生成密钥前 " + sb.ToString());

            string sign = GetMD5(sb.ToString()).ToUpper();
            return sign;
        }

        /// <summary>
        /// 校验签名
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsCorrectSign(SortedDictionary<string, string> parameters, string key)
        {
            string sign = CreateSign(parameters, key);
            // 前端传来的签名
            string requestSign = parameters["signature"].ToUpper();
            Log.Info("通过用户发送数据获取新签名：" + sign);
            return requestSign.Equals(sign);
        }

        /// <summary>
        /// md5常用工具类
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5(string input)
        {
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }

        }

        /// <summary>
        /// 生成uuid
        /// </summary>
        /// <returns></returns>
        public static string GenerateUUID()
        {
            string uuid = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 32);
            return uuid;
        }
    }
}
