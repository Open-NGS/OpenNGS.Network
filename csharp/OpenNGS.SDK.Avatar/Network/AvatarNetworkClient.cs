using OpenNGS.SDK.Avatar.Models;
using OpenNGS.SDK.Core;
using OpenNGS.SDK.Core.Network;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.SDK.Avatar.Network
{
    public class AvatarNetworkClient : IAvatarNetworkClient
    {
        const string p_PlayerAvatarUrlStem = "/avatar/v1/player";

        readonly string m_PlayerAvatarUrl;

        internal INetworkHandler NetworkHandler { get; }

        internal AvatarNetworkClient(string host)
        {
            NetworkHandler = new NetworkHandler();

            m_PlayerAvatarUrl = host + p_PlayerAvatarUrlStem;
        }

        public Task<NetworkResponse<AppPlayerVo>> GetPlayerAvatar(string accessToken)
        {
            return NetworkHandler.GetAsync<NetworkResponse<AppPlayerVo>>(m_PlayerAvatarUrl, null, WithEnvironment(WithAccessToken(GetCommonHeaders(), accessToken)));
        }

        public Task<bool> DownloadVRM(string url, string path = "")
        {
            return NetworkHandler.DownloadFile(url, path);
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
