using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNGS.SDK.Core
{
    public class ServicesInitializationException : Exception
    {
        public ServicesInitializationException() { }

        public ServicesInitializationException(string message) : base(message) {}

        public ServicesInitializationException(string message, Exception innerException) : base(message, innerException) {}
    }
}
