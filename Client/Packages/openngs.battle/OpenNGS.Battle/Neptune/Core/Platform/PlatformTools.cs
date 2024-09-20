using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

public class PlatformTools
{
    public static bool LogEnabled = false;
#if UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern void sendMailReportIssue(string issueType, string clientVersion, string playerId, string serverID, string playerName);

	[DllImport("__Internal")]
	private static extern void gameCenterLogin(string gameCenterLoginCheckUrl, string gameCenterBinNewUrl);

	[DllImport("__Internal")]
    private static extern void initPaymentUrls(string szRequestUrl, string szFinishUrl);

    [DllImport("__Internal")]
	private static extern void doRecharge(string title, string itemId, string cost, string gameServerIP, string uin);

    [DllImport("__Internal")]
    private static extern void dologin(string szLoginUrl, string szGamecenterCheckUrl, string szGameCenterBindNewUrl, string szVersion);

    [DllImport("__Internal")]
    private static extern void addLocalNotification(string key, string text, string date, string repeatInteval);

    [DllImport("__Internal")]
    private static extern void cancelLocalNotification(string key);


    [DllImport("__Internal")]
    public static extern string AssetsManagerReadClientVersion();
	[DllImport("__Internal")]
	public static extern void AssetsManagerCreate(string checkVersionUrl, string urlParam);
	[DllImport("__Internal")]
	public static extern void AssetsManagerStartUpdate(string userId);
	[DllImport("__Internal")]
	public static extern int AssetsManagerGetCode();
	[DllImport("__Internal")]
	public static extern string AssetsManagerGetDesc();
	[DllImport("__Internal")]
	public static extern float AssetsManagerGetProgress();
	[DllImport("__Internal")]
	public static extern int AssetsManagerBinaryUpdated();

	[DllImport("__Internal")]
	public static extern int AssetsManagerIsEndOf();
    [DllImport("__Internal")]
    public static extern int AssetsManagerHasMissingFiles();

    [DllImport("__Internal")]
    public static extern int HgIsOptionalFileMissing(string inputFileName);

    [DllImport("__Internal")]
    public static extern void       AssetsManagerSetDownloadInBackground(int val);

    [DllImport("__Internal")]
    public static extern void        AssetsManagerSetUsingOptionalList(int val);
    
    [DllImport("__Internal")]
    public static extern int         AssetsManagerAddOptionListToPartialDownload();

     [DllImport("__Internal")]
    public static extern int        AssetsManagerPrepareForPartialDownload(string checkVersionUrl, string urlParam);
     [DllImport("__Internal")]
    public static extern int        AssetsManagerAddPartialDownloadFile(string lpszFile);
     [DllImport("__Internal")]
    public static extern int        AssetsManagerStartPartialDownload(string userID);

   [DllImport("__Internal")]
    public static extern int        AssetsManagerGetActualBytesDownloaded();
    [DllImport("__Internal")]
    public static extern int        AssetsManagerGetActualBytesTotalNeedDownload();

    [DllImport("__Internal")]
    public static extern void       AssetsManagerStartChecker(string user_id, string checkVerFileUrl);

    [DllImport("__Internal")]
    public static extern int        AssetsManagerGetCheckerProgress();

    [DllImport("__Internal")]
    public static extern string     AssetsManagerGetCheckerResult();

    [DllImport("__Internal")]
    public static extern void       AssetsManagerDestroyChecker();

    [DllImport("__Internal")]
    private static extern void HGInit ();

	[DllImport("__Internal")]
	private static extern IntPtr HGGetFileContents (string szfn, out IntPtr lpsize);
    [DllImport("__Internal")]
    public static extern void HGSetDebugState(bool logEnabled);
    [DllImport("__Internal")]
    private static extern void HGDeleteJsonFile(int deleteJsonType);
	[DllImport("__Internal")]
	public static extern int HGGetIsAmazonVersion();
	
	[DllImport("__Internal")]
	private static extern bool isFacebookConnected();
	[DllImport("__Internal")]
	private static extern void connectFacebook();
	[DllImport("__Internal")]
	private static extern void disConnectFacebook();
	[DllImport("__Internal")]
	private static extern void adJustCallEvent(string key);
	[DllImport("__Internal")]
	private static extern void adJustCallRevenueEvent(string key, double amountInCents);
	[DllImport("__Internal")]
	private static extern void openURL(string url);
    [DllImport("__Internal")]
    private static extern void setAppiraterString(string alertTitle, string alertMessage, string alertCancelTitle, string alertRateTitle, string alertRateLaterTitle);
    [DllImport("__Internal")]
    public static extern string getIpv4overIpv6(string address, string port);
    [DllImport("__Internal")]
    public static extern string getAppParam();


    [DllImport("__Internal")]
    public static extern int hg_reader_can_handle(string inputFileName);
    
    [DllImport("__Internal")]
    private static extern int  HgAddMissingFiles(string inputFileName);
	[DllImport("__Internal")]
	private static extern void CopyTextToClipboard(string text);
	[DllImport("__Internal")]
	private static extern void SavePhoto(string text);
    [DllImport("__Internal")]
    private static extern void IosFacebookLike(bool show);
    [DllImport("__Internal")]
    private static extern void ShareMessage(string msgTitle, string msgContent, float posx, float posy);

    [DllImport("__Internal")]
    private static extern long getSystemFreeSpace();
    [DllImport( "__Internal" )]
    private static extern float getiOSBatteryLevel();

    [DllImport("__Internal")]
    private static extern int GetNetworkType();

#elif UNITY_ANDROID
    [DllImport("hcu3dplugin")]
    public static extern string AssetsManagerReadClientVersion();
	[DllImport("hcu3dplugin")]
	public static extern void AssetsManagerCreate( string checkVersionUrl, string urlParam);
	[DllImport("hcu3dplugin")]
	public static extern void AssetsManagerStartUpdate(string userId);
	[DllImport("hcu3dplugin")]
	public static extern int AssetsManagerGetCode();
	[DllImport("hcu3dplugin")]
	public static extern string AssetsManagerGetDesc();
	[DllImport("hcu3dplugin")]
	public static extern float AssetsManagerGetProgress();
	[DllImport("hcu3dplugin")]
	public static extern int AssetsManagerBinaryUpdated();
	
	[DllImport("hcu3dplugin")]
	public static extern int AssetsManagerIsEndOf();
    
    [DllImport("hcu3dplugin")]
    public static extern int AssetsManagerHasMissingFiles();

    [DllImport("hcu3dplugin")]
    public static extern void       AssetsManagerSetDownloadInBackground(int val);

    [DllImport("hcu3dplugin")]
    public static extern void        AssetsManagerSetUsingOptionalList(int val);
    
    [DllImport("hcu3dplugin")]
    public static extern int         AssetsManagerAddOptionListToPartialDownload();

    [DllImport("hcu3dplugin")]
    public static extern int        AssetsManagerPrepareForPartialDownload(string checkVersionUrl, string urlParam);
    
    [DllImport("hcu3dplugin")]
    public static extern int        AssetsManagerAddPartialDownloadFile(string lpszFile);
    
    [DllImport("hcu3dplugin")]
    public static extern int        AssetsManagerStartPartialDownload(string userID);

    [DllImport("hcu3dplugin")]
    public static extern int        AssetsManagerGetActualBytesDownloaded();
    [DllImport("hcu3dplugin")]
    public static extern int        AssetsManagerGetActualBytesTotalNeedDownload();

    [DllImport("hcu3dplugin")]
    public static extern void       AssetsManagerStartChecker(string user_id, string checkVerFileUrl);

    [DllImport("hcu3dplugin")]
    public static extern int        AssetsManagerGetCheckerProgress();
    
    [DllImport("hcu3dplugin")]
    public static extern string     AssetsManagerGetCheckerResult();

    [DllImport("hcu3dplugin")]
    public static extern void       AssetsManagerDestroyChecker();

    [DllImport("hcu3dplugin")]
    public static extern int HgIsOptionalFileMissing(string inputFileName);

    [DllImport("hcu3dplugin")]
    private static extern void HGInit();

    [DllImport("hcu3dplugin")]
	private static extern IntPtr HGGetFileContents (string szfn, out IntPtr lpsize);
    
    [DllImport("hcu3dplugin")]
    public static extern void HGSetDebugState(bool logEnabled);

    [DllImport("hcu3dplugin")]
    private static extern void HGDeleteJsonFile(int deleteJsonType);
    [DllImport("hcu3dplugin")]
    public static extern int HGGetIsAmazonVersion();

    [DllImport("hcu3dplugin")]
    public static extern int hg_reader_can_handle(string inputFileName);

    [DllImport("hcu3dplugin")]
    private static extern int  HgAddMissingFiles(string inputFileName);

#endif


    /**
 * 
 * Read file data by c/cpp/oc, it is IOBlocking. device only
 */
    public static void Init()
    {
#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
        HGInit();
#endif
    }

    /**
     * 
     * Read file data by c/cpp/oc, it is IOBlocking. device only
     */
    public static byte[] GetFileContents(string szfilename)
    {
#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)

        IntPtr parasize = new IntPtr(0);
        IntPtr unmanaged_ptr = HGGetFileContents(szfilename, out parasize);
        int lpsize = parasize.ToInt32();
        //no file large than 2G bytes
        if (lpsize > 0 && unmanaged_ptr.ToInt64() != 0)
        {
            byte[] managed_data = new byte[lpsize];
            Marshal.Copy(unmanaged_ptr, managed_data, 0, lpsize);
            Marshal.FreeHGlobal(unmanaged_ptr);
            return managed_data;
        }
        if (LogEnabled) Debug.Log("GetFileContents is null lpsize " + lpsize + " unmanaged_ptr : " + unmanaged_ptr.ToInt64());
        return null;
#else
        return System.IO.File.ReadAllBytes(szfilename);
#endif
    }

    public static int AddMissingFiles(string inputFileName)
    {
#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
        return HgAddMissingFiles(inputFileName);
#else
        return 0;
#endif
    }

    public static void DeleteJsonFile(int deleteJsonType)
    {
#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
        HGDeleteJsonFile(deleteJsonType);
#endif
    }

    /**
     * Send email bug report by user
     *
     */
    public static void HGSendMailIssue(string issueType, string clientVersion, string playerID, string serverID, string playerName)
    {
        if (LogEnabled) Debug.Log("HGSendMailIssue  " + issueType + " ," + clientVersion + " ," + playerID + " ," + serverID + " ," + playerName);
#if !UNITY_EDITOR
#if UNITY_IPHONE
		sendMailReportIssue(issueType,clientVersion,playerID,serverID,playerName);
#elif UNITY_ANDROID
		AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge"); 
		jc.CallStatic("sendMailReportIssue",issueType,clientVersion,playerID,serverID,playerName);
#else

#endif
#endif
    }







    /**
 * add local notification
 */
    public static void HGAddLocalNotification(string key, string nId, string szTitle, string text, long triggerTime, int repeatInteval)
    {
        Debug.Log("Local Notification Added: " + key + ", nID: " + nId);
#if !UNITY_EDITOR
#if UNITY_IPHONE
		//Debug.Log("HGAddLocalNotification:" + key + ","  + nId +  ","  +szTitle+ ","+text + "," + triggerTime + ","+repeatInteval);
		addLocalNotification(key,text,""+triggerTime,""+repeatInteval);
#elif UNITY_ANDROID
		triggerTime  = triggerTime*1000;
		AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge"); 
		//addLocalNotification(String xszTitle, String xtext,
		//                    String xnId, String xiType, String xtriggerTime, String xrepeatInteval)
		jc.CallStatic("addLocalNotification",szTitle,text,nId,""+triggerTime,""+repeatInteval);
		
#else
		
#endif
#endif
    }

    /**
     * cancel local notification by key
     */
    public static void HGCancelLocalNotification(string key, string nId)
    {
        Debug.Log("Local Notification Cancel: " + key + ", nID: " + nId);
#if !UNITY_EDITOR
#if UNITY_IPHONE
		cancelLocalNotification(key);
#elif UNITY_ANDROID
		AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge"); 
		jc.CallStatic("cancelLocalNotification",nId);
#else
		
#endif
#endif
    }


    /**
     * 
     * do purchase
     * 
     */
    public static void HGPurchase(string title, string cost, string itemID, string serverId, string userId, string uin)
    {
        if (LogEnabled) Debug.Log("User " + userId + "(uin:" + uin + ") Purchase item [" + title + "(" + itemID + ")] with " + cost + " on server " + serverId);
#if !UNITY_EDITOR
#if UNITY_IPHONE
		doRecharge(title,itemID,cost,serverId,uin);
#elif UNITY_ANDROID
		string jsonStr= "{"
            + "\"title\":\"" + title + "\","
                + "\"cost\":\"" + cost + "\","
                + "\"transactionType\":\"" + itemID + "\","
                + "\"serverId\":\"" + serverId + "\","
                + "\"userId\":\"" + userId + "\","
                + "\"uin\":\"" + uin + "\""
				+"}";

        Debug.Log("Jni Json str: " + jsonStr);
		AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge"); 
		jc.CallStatic("doPayment",jsonStr);
#else
		
#endif
#endif
    }

    /**
     * 
     * need to call at the begining
     */
    public static void HGInitPaymentManager(string request_url, string getkey_url, string deliver_url)
    {


#if UNITY_IPHONE
        initPaymentUrls(request_url, deliver_url);
#elif UNITY_ANDROID
			string urls="";
			urls  += request_url;
			urls  +="\n";
			urls  += getkey_url;
			urls  +="\n";
			urls  +=deliver_url;
			
			AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge"); 
			jc.CallStatic("initPayment",urls);
#else

#endif
    }

    public static void HGUpdateLocalPrices(string payIds)
    {
#if UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
        jc.CallStatic("UpdateSkuPrices", payIds);
#endif
    }

    /**
     * device id login 
     */
    public static void HGDoLogin(string loginUrl, string gameCenterLoginCheckUrl, string gameCenterBinNewUrl, string googleLoginURL, string clientVersion)
    {
        if (LogEnabled)
        {
            Debug.Log("PlatformTools.cs HGDoLogin, loginurl: " + loginUrl + "\n");
            Debug.Log("PlatformTools.cs HGDoLogin, gameCenterLoginCheckUrl: " + gameCenterLoginCheckUrl + "\n");
            Debug.Log("PlatformTools.cs HGDoLogin, gameCenterBinNewUrl: " + gameCenterBinNewUrl + "\n");
            Debug.Log("PlatformTools.cs HGDoLogin, googleLoginURL: " + googleLoginURL + "\n");
            Debug.Log("PlatformTools.cs HGDoLogin, clientVersion: " + clientVersion + "\n");
        }
            
#if !UNITY_EDITOR
#if UNITY_IPHONE
		dologin(loginUrl,gameCenterLoginCheckUrl,gameCenterBinNewUrl, clientVersion);
#elif UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
        jc.CallStatic("doLogin", loginUrl, gameCenterLoginCheckUrl, gameCenterBinNewUrl, googleLoginURL, clientVersion);
#endif
#endif
    }

    /**
     * game center init
     */
    public static void HGDoGameCenterInit(string loginUrl, string gameCenterLoginCheckUrl, string gameCenterBinNewUrl, string googleLoginURL, string clientVersion)
    {
        if (LogEnabled)
        {
            Debug.Log("PlatformTools.cs HGDoGameCenterInit, loginurl: " + loginUrl + "\n");
            Debug.Log("PlatformTools.cs HGDoGameCenterInit, gameCenterLoginCheckUrl: " + gameCenterLoginCheckUrl + "\n");
            Debug.Log("PlatformTools.cs HGDoGameCenterInit, gameCenterBinNewUrl: " + gameCenterBinNewUrl + "\n");
            Debug.Log("PlatformTools.cs HGDoGameCenterInit, googleLoginURL: " + googleLoginURL + "\n");
            Debug.Log("PlatformTools.cs HGDoGameCenterInit, clientVersion: " + clientVersion + "\n");
        }
            
#if !UNITY_EDITOR
#if UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
        jc.CallStatic("doInitGooglePlay", loginUrl, gameCenterLoginCheckUrl, gameCenterBinNewUrl, googleLoginURL, clientVersion);
#endif
#endif
    }

    /**
     * game center init
     */
    public static void HGDoGameCenterLogin(string loginUrl, string gameCenterLoginCheckUrl, string gameCenterBinNewUrl, string googleLoginURL, string clientVersion)
    {
        if (LogEnabled)
        {
            Debug.Log("PlatformTools.cs HGDoGameCenterLogin, loginurl: " + loginUrl + "\n");
            Debug.Log("PlatformTools.cs HGDoGameCenterLogin, gameCenterLoginCheckUrl: " + gameCenterLoginCheckUrl + "\n");
            Debug.Log("PlatformTools.cs HGDoGameCenterLogin, gameCenterBinNewUrl: " + gameCenterBinNewUrl + "\n");
            Debug.Log("PlatformTools.cs HGDoGameCenterLogin, googleLoginURL: " + googleLoginURL + "\n");
            Debug.Log("PlatformTools.cs HGDoGameCenterLogin, clientVersion: " + clientVersion + "\n");
        }
#if !UNITY_EDITOR
#if UNITY_IPHONE
		gameCenterLogin(gameCenterLoginCheckUrl,gameCenterBinNewUrl);
#elif UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
        jc.CallStatic("doGooglePlayLogin", loginUrl, gameCenterLoginCheckUrl, gameCenterBinNewUrl, googleLoginURL, clientVersion);
#endif
#endif
    }

    public static void HGOpenUrl(string url)
    {
        if (LogEnabled) Debug.Log("PlatformTools.cs Open url: " + url + "\n");
#if !UNITY_EDITOR
#if UNITY_IPHONE
        openURL(url);
#elif UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
        jc.CallStatic("doOpenURL", url);
#endif
#endif
    }

    public static void HGRestartGame()
    {
        if (LogEnabled) Debug.Log("PlatformTools.cs HGRestartGame.");
#if (!UNITY_EDITOR && UNITY_ANDROID)
        AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
        jc.CallStatic("restartGame");
#endif
    }

    public static void HGPopupRestartGameMessage(string msg, string okText)
    {
        if (LogEnabled) Debug.Log("PlatformTools.cs HGPopupRestartGameMessage.");
#if (!UNITY_EDITOR && UNITY_ANDROID)
        AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
        jc.CallStatic("popupRestartGameMessage",msg,okText);
#endif
    }
    public static void HGSetAppiraterString(string alertTitle, string alertMessage, string alertCancelTitle, string alertRateTitle, string alertRateLaterTitle, string setStringStr="")
    {
        if (LogEnabled) Debug.Log("PlatformTools.cs HGSetAppiraterString, alertTitle: " + alertTitle);

        alertMessage = alertMessage.StartsWith("KEY.") ? "" : alertMessage;
        alertCancelTitle = alertCancelTitle.StartsWith("KEY.") ? "" : alertCancelTitle;
        alertRateTitle = alertRateTitle.StartsWith("KEY.") ? "" : alertRateTitle;
        alertRateLaterTitle = alertRateLaterTitle.StartsWith("KEY.") ? "" : alertRateLaterTitle;

#if !UNITY_EDITOR
#if UNITY_IPHONE
		setAppiraterString(alertTitle, alertMessage, alertCancelTitle, alertRateTitle, alertRateLaterTitle);
#elif UNITY_ANDROID
        try
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
            jc.CallStatic("setAppiraterString", setStringStr, alertMessage, alertCancelTitle, alertRateTitle,
                alertRateLaterTitle);
        }
        catch (Exception e)
        {
            // ignored

        }

#endif
#endif
    }

    // 不要直接使用这个函数， 通过GameUtil.GetSystemTotalMem()调用
    public static long HGGetSystemTotalMem()
    {
        long totalMem = 0;
        if (LogEnabled) Debug.Log("PlatformTools.cs HGGetSystemTotalMem.");
#if (!UNITY_EDITOR && UNITY_ANDROID)
        try
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
            totalMem = jc.CallStatic<long>("getTotalMem");
        }
        catch (Exception e)
        {
            // ignored

        }
#endif
        return totalMem;
    }

    // 不要直接使用这个函数， 通过GameUtil.GetSystemAvailableMem()调用
    public static long HGGetSystemAvailableMem()
    {
        long availabeMem = 0;
        if (LogEnabled) Debug.Log("PlatformTools.cs HGGetSystemAvailableMem.");
#if (!UNITY_EDITOR && UNITY_ANDROID)
        try
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
            availabeMem = jc.CallStatic<long>("getAvailableMem");
        }
        catch (Exception e)
        {
            // ignored

        }
#endif
        return availabeMem;
    }


    public static string HGConvertIpv4overIpv6(string address, string port)
    {
        string realAddress = address;
        if (LogEnabled) Debug.Log("PlatformTools.cs HGConvertIpv4overIpv6.");
#if (!UNITY_EDITOR && UNITY_IPHONE)
        realAddress = getIpv4overIpv6(address, port);
#endif
        return realAddress;
    }

    // -- get app param, just link app version
    public static string HGGetAppParam()
    {
        string appParam = "";
        if (LogEnabled) Debug.Log("PlatformTools.cs HGGetAppParam.");
#if !UNITY_EDITOR
#if UNITY_ANDROID
        try
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
            appParam = jc.CallStatic<string>("getAppParam");
        }
        catch (Exception e)
        {
            // ignored
            if (LogEnabled) Debug.Log("PlatformTools.cs HGGetAppParam exception: " + e.ToString());
        }
#elif UNITY_IPHONE
        appParam = getAppParam();
        Debug.Log("Got AppParamData: " + appParam);
#endif
#endif
        return appParam;
    }

    // -- get app param, just link app version
    public static void FacebookLike(bool show = true)
    {
        if (LogEnabled) Debug.Log("PlatformTools.cs FacebookLike.");
#if (!UNITY_EDITOR && UNITY_ANDROID)
        try
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
            jc.CallStatic<string>("facebookLike");
        }
        catch (Exception e)
        {
            // ignored
            if (LogEnabled) Debug.Log("PlatformTools.cs facebookLike exception: " + e.ToString());
        }
#elif !UNITY_EDITOR && UNITY_IPHONE
        if (LogEnabled) Debug.Log("PlatformTools.cs FacebookLike IosFacebookLike.");
        IosFacebookLike(show);
#endif
    }


    public static void HGGCCollect()
    {
        if (LogEnabled) Debug.Log("PlatformTools.cs HGGCCollect.");
#if (!UNITY_EDITOR && UNITY_ANDROID)
        try
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
            jc.CallStatic("GCCollect");
        }
        catch (Exception e)
        {
            Debug.Log("U3DBridge GCCollect Got Exception.");
            return;
        }
        Debug.Log("HGGCCollect Done!");
#endif
    }

    public static void HGClearHidenProcess()
    {
        if (LogEnabled) Debug.Log("PlatformTools.cs HGClearHidenProcess");
#if (!UNITY_EDITOR && UNITY_ANDROID)
        try
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
            jc.CallStatic("ClearHidenProcess");
        }
        catch (Exception e)
        {
            // Ignore
        }
#endif
    }

    public static void shareMsg(String activityTitle, String msgTitle, String msgText,String imgPath, float posx = 0, float posy = 0)
    {
        if (LogEnabled) Debug.LogFormat("shareMsg :{0} {1} {2} {3} \n", activityTitle, msgTitle, msgText, imgPath, imgPath);
#if !UNITY_EDITOR
#if UNITY_IPHONE
        ShareMessage(msgTitle, msgText, posx, posy);
#elif UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
        jc.CallStatic("shareMsg", activityTitle,msgTitle,msgText,imgPath);
#endif
#endif
    }

/// <summary>
/// 拷贝指定的文本内容到剪切板
/// </summary>
/// <param name="text">复制的文本内容</param>
public static void copyTextToClipboard(String text)
    {
        if (LogEnabled) Debug.LogFormat("CopyTextToClipboard :{0} \n", text);
#if !UNITY_EDITOR
#if UNITY_IPHONE
        CopyTextToClipboard(text);
#elif UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
        jc.CallStatic("CopyTextToClipboard", text);
#endif
#endif
    }

    /// <summary>
    /// 调用设备API执行APK安装
    /// </summary>
    /// <param name="apkFilePath">要安装的APK文件路径</param>
    public static void InstallAPK(String apkFilePath)
    {
        if (LogEnabled) Debug.LogFormat("InstallAPK :{0} \n", apkFilePath);
#if (!UNITY_EDITOR && UNITY_ANDROID)
        AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
        jc.CallStatic("InstallAPK", apkFilePath);
#endif
    }

    /// <summary>
    /// 向FCM注册Topic， 只在安卓平台起作用！
    /// </summary>
    /// <param name="topic"></param>
    public static void SubscribePushTopic(String topic) {
        if (LogEnabled) Debug.LogFormat("SubscribePushTopic :{0} \n", topic);
#if (!UNITY_EDITOR && UNITY_ANDROID)
        AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
        jc.CallStatic("SubscribePushTopic", topic);
#endif
    }


    /// <summary>
    /// 保存指定的图片文件到外部存储器
    /// </summary>
    /// <param name="text"></param>
    public static void SaveToDCIM(string text)
    {
        if (LogEnabled) Debug.LogFormat("SaveToDCIM ");
#if !UNITY_EDITOR
#if UNITY_IPHONE
        SavePhoto(text);
#elif UNITY_ANDROID

#endif
#endif
    }

    /// <summary>
    /// 删除当前设备上存储的device ID
    /// </summary>
    public static void DeleteDeviceId()
    {
        if (LogEnabled) Debug.LogFormat("DeleteDeviceId");
#if (!UNITY_EDITOR && UNITY_ANDROID)
        AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
        jc.CallStatic("DeleteDeviceId");
#endif
    }

    /// <summary>
    /// 获得设备外部存储的剩余可用空间
    /// </summary>
    /// <returns>
    /// 单位 MB
    /// </returns>
    public static long HGGetSystemFreeSpace()
    {
        long availabeSpace = 0;
        if (LogEnabled) Debug.Log("PlatformTools.cs HGGetSystemFreeSpace");
#if !UNITY_EDITOR
#if UNITY_IPHONE
        availabeSpace = getSystemFreeSpace();
#elif UNITY_ANDROID
        try
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
            availabeSpace = jc.CallStatic<long>("getSystemFreeSpace", Application.persistentDataPath);
        }
        catch (Exception e)
        {
            // ignored
            Debug.LogError("PlatformTools.cs HGGetSystemFreeSpace Failed");
        }
#endif
#endif
        return availabeSpace;
    }

    /// <summary>
    /// 获得当前设备的剩余电量
    /// </summary>
    /// <returns>
    /// 剩余电量的百分比 0 - 100
    /// </returns>
    public static float HGGetBatteryLevel()
    {
        int battery = 0;
#if !UNITY_EDITOR
#if UNITY_IPHONE
        battery = (int)(getiOSBatteryLevel()*100);
#elif UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
        battery = jc.CallStatic<int>("getBatteryLevel");
#endif
#endif
        if (LogEnabled) Debug.LogFormat("PlatformTools.cs HGGetBatteryLevel {0}",battery);
        return battery;
    }

    /// <summary>
    /// 获取当前活动网络的类型
    /// </summary>
    /// <returns>“WIFI” or “3G-XXXX”</returns>
    public static string HGGetNetworkType()
    {
        string networkType = "";
#if !UNITY_EDITOR
#if UNITY_IPHONE
        int netstatus = GetNetworkType();
        switch(netstatus) 
        {
            case 0:
                networkType = "NoNetwork";
                break;
            case 1:
                networkType = "Mobile";
                break;
            case 2:
                networkType = "WIFI";
                break;
            default:
                networkType = "Unkonwn";
                break;
        }
#elif UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.ucool.u3dplugin.U3DBridge");
        networkType = jc.CallStatic<string>("getNetworkType");
#endif
#endif
        if (LogEnabled) Debug.LogFormat("PlatformTools HGGetNetworkType: {0}", networkType);
        return networkType;
    }
}
