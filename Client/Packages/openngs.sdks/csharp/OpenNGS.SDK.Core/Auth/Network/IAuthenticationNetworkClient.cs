using OpenNGS.SDK.Auth.Models;
using OpenNGS.SDK.Auth.Models.Requests;
using OpenNGS.SDK.Core.Network;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.SDK.Auth.Network
{
    public interface IAuthenticationNetworkClient
    {
        Task<NetworkResponse<string>> LoginByUsernamePasswordAsync(UsernamePasswordRequest request);

        Task<NetworkResponse<string>> LoginOrRegisterByVerificationCode(VerificationCodeRequest verificationCodeRequest);

        Task<NetworkResponse> SendVerificationCode(VerificationCodeRequest verificationCodeRequest);

        Task<NetworkResponse<EegamesUserInfo>> GetUserInfo(string accessToken);

    }
}
