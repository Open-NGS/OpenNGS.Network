using OpenNGS.SDK.Auth.Models;
using OpenNGS.SDK.Auth.Models.Requests;
using OpenNGS.SDK.Core;
using OpenNGS.SDK.Core.Network;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenNGS.SDK.Auth.Network
{
    public class AuthenticationNetworkClient : IAuthenticationNetworkClient
    {
        const string p_UsernamePasswordLoginUrlStem = "/api/vl/auth/login-username";
        const string p_VerificationCodeLoginUrlStem = "/api/v1/auth/login-sms-code";
        const string p_VerificationCodeSendUrlStem = "/api/v1/auth/send-sms-code";
        const string p_UserInfoUrlStem = "/api/v1/user";

        readonly string m_UsernamePasswordLoginUrl;
        readonly string m_VerificationCodeLoginUrl;
        readonly string m_VerificationCodeSendUrl;
        readonly string m_UserInfoUrl;


        internal INetworkHandler NetworkHandler { get; }

        internal AuthenticationNetworkClient(string host)
        {
            NetworkHandler = new NetworkHandler();

            m_UsernamePasswordLoginUrl = host + p_UsernamePasswordLoginUrlStem;
            m_VerificationCodeLoginUrl = host + p_VerificationCodeLoginUrlStem;
            m_VerificationCodeSendUrl = host + p_VerificationCodeSendUrlStem;
            m_UserInfoUrl = host + p_UserInfoUrlStem;
        }

        public Task<NetworkResponse<string>> LoginByUsernamePasswordAsync(UsernamePasswordRequest request)
        {
            return NetworkHandler.PostAsync<NetworkResponse<string>>(m_UsernamePasswordLoginUrl, request);
        }

        public Task<NetworkResponse<string>> LoginOrRegisterByVerificationCode(VerificationCodeRequest request)
        {
            return NetworkHandler.PostAsync<NetworkResponse<string>>(m_VerificationCodeLoginUrl, request);
        }

        public Task<NetworkResponse> SendVerificationCode(VerificationCodeRequest request)
        {
            return NetworkHandler.PostAsync<NetworkResponse>(m_VerificationCodeSendUrl, request);
        }

        public Task<NetworkResponse<EegamesUserInfo>> GetUserInfo(string accessToken)
        {
            return NetworkHandler.GetAsync<NetworkResponse<EegamesUserInfo>>(m_UserInfoUrl, null, WithEnvironment(WithAccessToken(GetCommonHeaders(), accessToken)));
        }

        Dictionary<string, string> WithEnvironment(Dictionary<string, string> headers)
        {
            headers["AppId"] = OpenNGSPlatformServices.Instance.Options.AppId;
            headers["Signature"] = OpenNGSPlatformServices.Instance.GenerateSignature();
            return headers;
        }

        Dictionary<string, string> WithAccessToken(Dictionary<string, string> headers, string accessToken)
        {
            headers["Authorization"] = $"Bearer {accessToken}";
            return headers;
        }

        Dictionary<string, string> GetCommonHeaders()
        {
            return new Dictionary<string, string>();
        }
    }
}
