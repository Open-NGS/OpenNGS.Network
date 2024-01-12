using System;

namespace OpenNGS.ERPC
{
    public struct Status
    {
        /// <summary>
        /// result for invoke action
        /// </summary>
        public Int32 Result;

        /// <summary>
        /// code return by server function
        /// </summary>
        public Int32 Code;

        /// <summary>
        /// detail info
        /// </summary>
        public string Message;

        public Status(Int32 holder)
        {
            Result = 0;
            Code = 0;
            Message = string.Empty;
        }

        public Status(Int32 result, Int32 code, string message)
        {
            Result = result;
            Code = code;
            Message = message;
        }

        public Status(Int32 code, string message)
        {
            Result = 0;
            Code = code;
            Message = message;
        }
    }
}
