using OpenNGS.SDK.Avatar.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.SDK.Avatar
{
    public interface IAvatarService
    {
        event Action<AppPlayerVo> PlayerAvatarCallback;

        event Action<bool> DownloadVRMCallback;

        Task GetPlayerAvatar();

        Task DownloadVRM(string url, string path = "");
    }
}
