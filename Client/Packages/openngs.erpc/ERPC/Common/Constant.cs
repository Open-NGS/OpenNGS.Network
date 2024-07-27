using System;

namespace OpenNGS.ERPC
{
    /// <summary>
    /// call type
    /// </summary>
    public class CALLTYPE
    {
        public const int UNARY = 0;
        public const int ONEWAY = 1;
    }

    public class MESSAGETYPE
    {
        public const int DYEING = 0x01;
        public const int TRACE = 0x02;
    }

    public class CONTENTTYPE
    {
        public const int PB = 0;
        public const int JSON = 1;
        public const int FLATBUFFER = 2;
    }

}
