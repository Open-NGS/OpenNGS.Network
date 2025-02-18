using OpenNGS.Platform;
using OpenNGS.SDK.Auth;
using OpenNGS.SDK.Notice;
using System.Collections;
using System.Collections.Generic;
using OpenNGS.SDK.Notice.Models;
using UnityEngine;
using OpenNGS.SDK.Auth.Models;
using OpenNGS.SDK.Core;

public class EEGamesNoticeProvider : INoticeProvider
{
    public PLATFORM_MODULE Module => PLATFORM_MODULE.NOTICE;
    private PlatformNoticeRet m_NoticeRet = new PlatformNoticeRet();
    public void GetNotices(string noticeGroup, string language, int region, string partition, string extra)
    {
        //NoticeServiceInternal
        //await AvatarService.Instance.GetPlayerAvatarList();

        NoticeService.Instance.PlayerNoticeErrorCallback += (result) =>
        {
            if( result == 0)
            {

            }
            else
            {
                int a = 0;
            }
        };

        NoticeService.Instance.PlayerNoticeListCallback += (result) =>
        {
            if (result != null)
            {
                _callBackNotices(m_NoticeRet);
            }
            else
            {
            }
        };

        NoticeService.Instance.GetNotices(noticeGroup, language, region, partition, extra);
    }

    public void Start()
    {
        int a = 0;
    }

    public void Stop()
    {
    }

    public void Update()
    {
    }

    private void _callBackNotices(PlatformNoticeRet _ret)
    {
        PlatformCallback.Instance.OnNoticeCallBack(_ret);
    }
}
