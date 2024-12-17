using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNGS.SDK.Core
{
    public class RequestFailedException : Exception
    {
        public int ErrorCode { get; }

        public RequestFailedException(int errorCode, string message) : this(errorCode, message, null)
        {
        }

        public RequestFailedException(int errorCode, string message, Exception innerException) : base(message, innerException)
        {
            this.ErrorCode = errorCode;
        }
    }
}
