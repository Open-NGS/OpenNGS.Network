

namespace OpenNGS
{

    public enum BaseXCodec
    {
        Base64 = 64,
        Base58 = 58,
        Base32 = 32,
        Base16 = 16,
    }


    public class BaseX
    {

        private class BaseXCharset
        {
            public const string base64charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz+/"; //Base-64
            public const string base58charset = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz"; //Base-58.

            public static string GetCharset(BaseXCodec codec)
            {
                switch(codec)
                {
                    case BaseXCodec.Base58:return base58charset;
                    default:
                        return base64charset.Substring(0, (int)codec);
                }
            }
        }

        /// <summary>
        /// decimal value type to base X string
        /// </summary>
        /// <param name="value">The max value can not more decimal.MaxValue<</param>
        /// <returns>Return a specified base X encode string</returns>
        public static string Encode(decimal value, BaseXCodec codec)//17223472558080896352ul
        {
            string charset = BaseXCharset.GetCharset(codec);
            string result = string.Empty;
            do
            {
                decimal index = value % charset.Length;
                result = charset[(int)index] + result;
                value = (value - index) / charset.Length;
            }
            while (value > 0);

            return result;
        }


        /// <summary>
        /// base X encode string to decimal
        /// </summary>
        /// <param name="value">62 encode string</param>
        /// <returns>Return a specified decimal number that decode by base X string</returns>
        public static decimal Decode(string value, BaseXCodec codec)
        {
            string charset = BaseXCharset.GetCharset(codec);
            decimal result = 0;
            for (int i = 0; i < value.Length; i++)
            {
                int x = value.Length - i - 1;
                result += charset.IndexOf(value[i]) * Pow(charset.Length, x);// Math.Pow(exponent, x);
            }
            return result;
        }

        /// <summary>
        /// Power(n,x)
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static decimal Pow(decimal n, decimal x)
        {
            decimal value = 1;////1 will be the result for any number's power 0.
            while (x > 0)
            {
                value = value * n;
                x--;
            }
            return value;
        }

    }

}