using UnityEngine;
using static OpenNGS.Platform.Platform;
namespace OpenNGS.Platform
{
    public class PlatformCas
    {
        public static event OnPlatformRetEventHandler<PlatformCasRet> CasRetEvent;
        public static void Initialize(string strAppKey, string strGameID, bool bTestMode = false)
        {
            if (!Platform.IsSupported(PLATFORM_MODULE.CAS))
                return;
            ICasProvider _casProvider = Platform.GetCas();
            if (_casProvider != null)
            {
                _casProvider.Initialize(strAppKey, strGameID, bTestMode);
            }
        }
        public static void SetMetaData(string strMetaCategory, string strMetaKey, string strMetaValue)
        {
            if (!Platform.IsSupported(PLATFORM_MODULE.CAS))
                return;
            ICasProvider _casProvider = Platform.GetCas();
            if (_casProvider != null)
            {
                _casProvider.SetMetaData(strMetaCategory, strMetaKey, strMetaValue);
            }
        }
        public static void LoadBanner(string strAdUnitId, uint nBannerPosition)
        {
            if (!Platform.IsSupported(PLATFORM_MODULE.CAS))
                return;
            ICasProvider _casProvider = Platform.GetCas();
            if (_casProvider != null)
            {
                _casProvider.LoadBanner(strAdUnitId, nBannerPosition);
            }
        }
        public static void ShowBannerAd(string strAdUnitId)
        {
            if (!Platform.IsSupported(PLATFORM_MODULE.CAS))
                return;
            ICasProvider _casProvider = Platform.GetCas();
            if (_casProvider != null)
            {
                _casProvider.ShowBannerAd(strAdUnitId);
            }
        }
        public static void HideBannerAd(string strAdUnitID)
        {
            if (!Platform.IsSupported(PLATFORM_MODULE.CAS))
                return;
            ICasProvider _casProvider = Platform.GetCas();
            if (_casProvider != null)
            {
                _casProvider.HideBannerAd(strAdUnitID);
            }
        }
        public static void LoadAd(string strAdUnitId, PlatformAdsType _typ)
        {
            if (!Platform.IsSupported(PLATFORM_MODULE.CAS))
                return;
            ICasProvider _casProvider = Platform.GetCas();
            if (_casProvider != null)
            {
                _casProvider.LoadAd(strAdUnitId, _typ);
            }
        }
        public static void ShowAd(string strAdUnitId, PlatformAdsType _typ)
        {
            if (!Platform.IsSupported(PLATFORM_MODULE.CAS))
                return;
            ICasProvider _casProvider = Platform.GetCas();
            if (_casProvider != null)
            {
                _casProvider.ShowAd(strAdUnitId, _typ);
            }
        }
        internal static void OnCasRet(PlatformCasRet ret)
        {
            Debug.Log("[Platform]PlatformCasRet:" + ret.ToJsonString());
            if (CasRetEvent != null)
                CasRetEvent(ret);
        }
    }
    public enum PlatFormCasResult
    {

        InitCompleted = 0,  // 初始化完成
        InitFail,           // 初始化失败
        BannerLoaded,       // 横幅广告加载成功
        BannerError,        // 横幅广告加载失败
        BannerClicked,      // 横幅广告被点击
        BannerShown,        // 横幅广告显示成功
        BannerHidden,       // 横幅广告隐藏成功
        OnAdsAdLoaded,      // 加载成功
        OnAdsFailedToLoad,  // 加载失败
        OnAdsShowFailure,   // 显示失败
        OnAdsShowStart,     // 显示成功
        OnAdsShowClick,     // 显示中被点击
        OnAdsShowSkip,      // 播放中被跳过
        OnAdsShowComplete,  // 完整播放完毕
        BannerCollapsed,    // 横幅广告被Collapsed
        BannerLeftApp,      // 横幅广告离开应用
        BannerExpanded,     // 横幅广告被Expanded
        OnAdsClosed,        // 广告关闭
        OnAdsInfoChanged,   // 广告信息变更
    }
    public enum PlatformAdsType
    {
        Banner = 0,         // 横幅广告
        Interstitial = 1,   // 插屏广告
        Rewarded = 2,       // 激励广告
    }
    public class PlatformCasRet : PlatformBaseRet
    {
        private string bannerAdUnitID;
        private string adUnitID;
        private string gameID;
        private string appKey;
        private bool testMode;
        private uint casResultTyp;
        private uint casErrorTyp;

        [JsonProp("gameID")]
        public string GameID
        {
            get { return gameID; }
            set { gameID = value; }
        }
        [JsonProp("appKey")]
        public string AppKey
        {
            get { return appKey; }
            set { appKey = value; }
        }
        [JsonProp("testMode")]
        public bool TestMode
        {
            get { return testMode; }
            set { testMode = value; }
        }
        [JsonProp("bannerAdUnitID")]
        public string BannerAdUnitID
        {
            get { return bannerAdUnitID; }
            set { bannerAdUnitID = value; }
        }
        [JsonProp("adUnitID")]
        public string AdUnitID
        {
            get { return adUnitID; }
            set { adUnitID = value; }
        }
        [JsonProp("casResultTyp")]
        public uint CasResultTyp
        {
            get { return casResultTyp; }
            set { casResultTyp = value; }
        }
        [JsonProp("casErrorTyp")]
        public uint CasErrorTyp
        {
            get { return casErrorTyp; }
            set { casErrorTyp = value; }
        }
        public void Init()
        {
            adUnitID = "";
            casResultTyp = 0;
            casErrorTyp = 0;
        }
        public PlatformCasRet()
        {
        }

        public PlatformCasRet(string param) : base(param)
        {
        }

        public PlatformCasRet(object json) : base(json)
        {
        }
    }

}
