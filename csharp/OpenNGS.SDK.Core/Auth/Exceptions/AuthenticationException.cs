using OpenNGS.SDK.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNGS.SDK.Auth.Exceptions
{
    public sealed class AuthenticationException : RequestFailedException
    {
        AuthenticationException(int errorCode, string message, Exception innerException = null)
            : base(errorCode, message, innerException)
        {
        }

        public static RequestFailedException Create(int errorCode, string message, Exception innerException = null)
        {
            if (errorCode < AuthenticationErrorCodes.MinValue)
            {
                return new RequestFailedException(errorCode, message, innerException);
            }
            else
            {
                return new AuthenticationException(errorCode, message, innerException);
            }
        }
    }
}
