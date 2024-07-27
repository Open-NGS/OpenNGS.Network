using System.Collections;


namespace OpenNGS
{
    public static class ShortCodec
    {
        public static string ToShortCode64(decimal v)
        {
            return BaseX.Encode(v, BaseXCodec.Base64);
        }

        public static decimal FromShortCode64(string code)
        {
            return BaseX.Decode(code, BaseXCodec.Base64);
        }

        public static string ToShortCode58(decimal v)
        {
            return BaseX.Encode(v, BaseXCodec.Base58);
        }

        public static decimal FromShortCode58(string code)
        {
            return BaseX.Decode(code, BaseXCodec.Base58);
        }
    }
}