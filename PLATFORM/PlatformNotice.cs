using UnityEngine;
using OpenNGS.SDK.Notice.Models;
using System.Collections.Generic;
using static OpenNGS.Platform.Platform;
namespace OpenNGS.Platform
{
    public class PlatformNotice
    {
        public static event OnPlatformRetEventHandler<PlatformNoticeRet> NoticesRet;
        public static void GetNotices(string noticeGroup, string language, int region, string partition, string extra)
        {
            Debug.Log("[Platform]Notice");

            if (!Platform.IsSupported(PLATFORM_MODULE.NOTICE))
                return;
            INoticeProvider _noticeProvider = Platform.GetNotice();
            if (_noticeProvider != null)
            {
                _noticeProvider.GetNotices(noticeGroup, language, region, partition, extra);
            }
        }

        internal static void OnNoticeRet(PlatformNoticeRet ret)
        {
            Debug.Log("[Platform]OnLoginRet:" + ret.ToJsonString());
            if (NoticesRet != null)
                NoticesRet(ret);
        }
    }

    public class PlatformNoticeRet : PlatformBaseRet
    {
        public List<NoticeInfo> noticeInfos;
    }
}