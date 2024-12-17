using OpenNGS.SDK.Core;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;

namespace OpenNGS.SDK.Auth.Exceptions
{
    internal class AuthenticationExceptionHandler : IAuthenticationExceptionHandler
    {

        public RequestFailedException BuildInvalidCredentialsException()
        {
            return AuthenticationException.Create(AuthenticationErrorCodes.InvalidParameters, "Username and/or Password are not in the correct format");
        }

        public RequestFailedException BuildVerificationCodeException()
        {
            return AuthenticationException.Create(AuthenticationErrorCodes.InvalidParameters, "Dest and/or Code are not in the correct format");
        }
    }
}
