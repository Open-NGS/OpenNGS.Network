using OpenNGS.SDK.Auth;
using OpenNGS.SDK.Avatar.Models;
using OpenNGS.SDK.Avatar.Network;
using OpenNGS.SDK.Core.Network;
using System;
using System.Threading.Tasks;

namespace OpenNGS.SDK.Avatar
{
    public class AvatarServiceInternal : IAvatarService
    {
        public event Action<AppPlayerVo> PlayerAvatarCallback;

        public event Action<bool> DownloadVRMCallback;

        internal IAvatarNetworkClient NetworkClient { get; set; }
        public AvatarServiceInternal()
        {
            NetworkClient = new AvatarNetworkClient("http://api.pre.eegames.net/services/platform/avatar");
        }

        public Task GetPlayerAvatar()
        {
            return HandlePlayerAvatar(() => NetworkClient.GetPlayerAvatar(AuthcationService.Instance.Token));
        }

        public Task DownloadVRM(string url, string path = "")
        {
            return HandleDownloadVRM(() => NetworkClient.DownloadVRM(url, path));
        }

        internal async Task HandlePlayerAvatar(Func<Task<NetworkResponse<AppPlayerVo>>> playerAvatarRequest)
        {
            CompletePlayerAvatar(await playerAvatarRequest());
        }

        internal async Task HandleDownloadVRM(Func<Task<bool>> downloadRequest)
        {
            CompleteDownloadVRM(await downloadRequest());
        }

        private void CompleteDownloadVRM(bool result)
        {
            DownloadVRMCallback?.Invoke(result);
        }

        private void CompletePlayerAvatar(NetworkResponse<AppPlayerVo> response)
        {
            if (CompleteSuccess(response))
            {
                PlayerAvatarCallback?.Invoke(response.Data);
            }
        }

        internal bool CompleteSuccess(NetworkResponse response)
        {
            if (response.Success)
            {
                return true;
            }
            else
            {
                Log.Error($"[AvatarService] {response.Message}");
                return false;
            }
        }
    }
}
