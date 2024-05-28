using UnityEngine;
using System;
using Networks.NetWorkModule;

public class NetworkTool
{
    public static string PlatformConvertToMSDK(Platform platform)
    {
        switch (platform)
        {
            case Platform.None:
                return "None";
            case Platform.Guest:
                return "Guest";
            case Platform.QQ:
                return "QQ";
            case Platform.Wechat:
                return "WeChat";
            case Platform.Google:
                return "Google";
            case Platform.Facebook:
                return "Facebook";
            case Platform.GameCenter:
                return "GameCenter";
            default:
                Debug.LogWarning("ConverPlatform Unknown Platform " + platform.ToString());                
				return "";
        }
    }

    public static Platform MSDKConvertToPlatform(string platform)
    {
        switch (platform)
        {
            case "WeChat":
                return Platform.Wechat;
            case "QQ":
                return Platform.QQ;
            case "Guest":
                return Platform.Guest;
            case "Google":
                return Platform.Google;
            case "Facebook":
                return Platform.Facebook;
            case "GameCenter":
                return Platform.GameCenter;
            default:
                Debug.LogWarning("ConverApolloPlatform Unknown ApolloPlatform " + platform.ToString());                
				return Platform.None;
        }
    }

    //public static ChannelType PlatformConvertToGCloud(Platform platform)
    //{
    //    switch (platform)
    //    {
    //        case Platform.Guest:
    //            return ChannelType.Guest;
    //        case Platform.QQ:
    //            return ChannelType.QQ;
    //        case Platform.Wechat:
    //            return ChannelType.Wechat;
    //        case Platform.Google:
    //            return ChannelType.GooglePlay;
    //        case Platform.Facebook:
    //            return ChannelType.Facebook;
    //        case Platform.GameCenter:
    //            return ChannelType.GameCenter;
    //        default:
    //            Debug.LogWarning("ConverPlatform Unknown Platform " + platform.ToString());                
				//return ChannelType.None;
    //    }
    //}

    //public static Platform GCloudConvertToPlatform(ChannelType type)
    //{
    //    switch (type)
    //    {
    //        case ChannelType.Wechat:
    //            return Platform.Wechat;
    //        case ChannelType.QQ:
    //            return Platform.QQ;
    //        case ChannelType.Guest:
    //            return Platform.Guest;
    //        case ChannelType.GooglePlay:
    //            return Platform.Google;
    //        case ChannelType.Facebook:
    //            return Platform.Facebook;
    //        case ChannelType.GameCenter:
    //            return Platform.GameCenter;
    //        default:
    //            Debug.LogWarning("ConverApolloPlatform Unknown ApolloPlatform " + type.ToString());                
				//return Platform.None;
    //    }
    //}
	
	public static string ConvertNoticeType(NoticeType type)
    {
        switch (type)
        {
            case NoticeType.NoticeType_ServerClose:
                return "852";
            case NoticeType.NoticeType_LogIn:
                return "1000";
            case NoticeType.NoticeType_LogInOver:
                return "2000";
            case NoticeType.NoticeType_Activity:
                return "854";
            case NoticeType.NoticeType_Scroll:
                return "2001";
            default:
                Debug.LogWarning("ConvertNoticeType Unknown NoticeType " + type.ToString());
                return "851";
        }
    }

    public static int CompareVersion(string ver1, string ver2)
    {
        string[] ver1Arr = ver1.Split('.');
        string[] ver2Arr = ver2.Split('.');
        int len = Math.Min(ver1Arr.Length, ver2Arr.Length);

        for (int i = 0; i < len; i++)
        {
            ushort n1, n2;
            if (!ushort.TryParse(ver1Arr[i], out n1))
            {
                return -1;
            }
            if (!ushort.TryParse(ver2Arr[i], out n2))
            {
                return 1;
            }

            if (n1 != n2)
            {
                return n1 - n2;
            }
        }
        return ver1Arr.Length - ver2Arr.Length;
    }
}
