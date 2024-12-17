using OpenNGS.SDK.Auth.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.SDK.Auth
{
    public interface IAuthenticationService
    {
        event Action<bool> AutoLoginCallback;

        event Action<EegamesUserInfo> LoginCallback;

        event Action VerificationCodeCallback;

        string Token { get; }

        EegamesUserInfo User { get; }

        Task<bool> AutoLogin();

        Task LoginByUsernamePassword(string username, string password);

        Task LoginOrRegisterByVerificationCode(VerificationType verificationType, string dest, string code);

        Task SendVerificationCode(VerificationType verificationType, string dest);

        void Logout();
    }
}
