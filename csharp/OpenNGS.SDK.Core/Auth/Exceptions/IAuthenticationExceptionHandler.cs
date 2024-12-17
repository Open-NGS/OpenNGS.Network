using OpenNGS.SDK.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNGS.SDK.Auth.Exceptions
{
    public interface IAuthenticationExceptionHandler
    {
        RequestFailedException BuildInvalidCredentialsException();
        RequestFailedException BuildVerificationCodeException();
    }
}
