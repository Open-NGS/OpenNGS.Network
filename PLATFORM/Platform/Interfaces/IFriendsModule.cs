using System.Collections;
using System.Collections.Generic;
using OpenNGS.Platform;
using UnityEngine;

namespace OpenNGS.Platform
{
    public class PlatformUserList : PlatformData
    {
        public List<PlatformUser> Users { get; set; }
    }
    
    public class FriendData : PlatformData
    {
        public string JsonString { get; set; }
    }

    public interface IFriendsModule : IPlatfromModule
    {
        public OPENNGS_PLATFORM_MODULE Module => OPENNGS_PLATFORM_MODULE.Friends;

        public void SetIDCEnv(string url);
        public void InitSDK(string appId, string appKey);
        public void SetOpenId(string openId);
        public void SetServer(int platId, int serverId);
        public NRequest<FriendData> Request(string category, string command, Dictionary<string, object> data = null);
        public NRequest<FriendData> Apply(string toAccount, string applyMsg = "", string remark = "", string groupId = "", List<string> labelIds = null, string scene = "");
        public NRequest<FriendData> Reply(string fromAccount, int action, string rejectReason = "", string remark = "");
        public NRequest<FriendData> GetApplyList(int type);
        public NRequest<FriendData> GetFriends(int pageSize, string gid = "", int pageIndex = -1, byte[] context = null);
        public NRequest<FriendData> Search(string key);
        public NRequest<FriendData> DeleteFriend(string toAccount);
        public NRequest<FriendData> AddBlacklist(string toAccount);
        public NRequest<FriendData> DelBlacklist(string toAccount);
        
        public NRequest<PlatformUserList> GetLoggedInUserFriends();

    }
}
