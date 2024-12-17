using OpenNGS.SDK.Avatar.Models;
using OpenNGS.SDK.Core.Network;
using System.Threading.Tasks;

namespace OpenNGS.SDK.Avatar.Network
{
    public interface IAvatarNetworkClient
    {
        Task<NetworkResponse<AppPlayerVo>> GetPlayerAvatar(string accessToken);

        Task<bool> DownloadVRM(string url, string path = "");
    }
}
