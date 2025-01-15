
using System;

namespace OpenNGS.Platform
{
    public class PlatformUser : PlatformData
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
        public NRequest<PlatformUser> GetUserTicket();

        public NRequest<PlatformUser> GetUserInfo(ulong id);

        public NRequest<PlatformUser> GetLoggedInUser();
    }
}
