
using System;

namespace OpenNGS.Platform
{
    public class PlatformUserData : PlatformData
    {
        public string ID { get; set; }
        

        public string DisplayName { get; set; }
        public string ImageURL { get; set; }
        public string SmallImageUrl { get; set; }
        public string InviteToken { get; set; }

        public string Token { get; set; }
    }
    public interface IUsersModule : IPlatfromModule
    {
        public NRequest<PlatformUserData> GetUserTicket();

        public NRequest<PlatformUserData> GetUserInfo(ulong id);

        public NRequest<PlatformUserData> GetLoggedInUser();
    }
}
