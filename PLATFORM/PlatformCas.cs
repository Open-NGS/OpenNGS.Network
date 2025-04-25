using UnityEngine;
using static OpenNGS.Platform.Platform;
namespace OpenNGS.Platform
{
    public class PlatformCas
    {
        public static event OnPlatformRetEventHandler<PlatformCasRet> CasRetEvent;
        public static void Initialize(string strGameID, bool bTestMode = false)
        {
            if (!Platform.IsSupported(PLATFORM_MODULE.CAS))
                return;
            ICasProvider _casProvider = Platform.GetCas();
            if (_casProvider != null)
            {
                _casProvider.Initialize(strGameID, bTestMode);
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
        public static void HideBannerAd()
        {
            if (!Platform.IsSupported(PLATFORM_MODULE.CAS))
                return;
            ICasProvider _casProvider = Platform.GetCas();
            if (_casProvider != null)
            {
                _casProvider.HideBannerAd();
            }
        }
        public static void LoadAd(string strAdUnitId)
        {
            if (!Platform.IsSupported(PLATFORM_MODULE.CAS))
                return;
            ICasProvider _casProvider = Platform.GetCas();
            if (_casProvider != null)
            {
                _casProvider.LoadAd(strAdUnitId);
            }
        }
        public static void ShowAd(string strAdUnitId)
        {
            if (!Platform.IsSupported(PLATFORM_MODULE.CAS))
                return;
            ICasProvider _casProvider = Platform.GetCas();
            if (_casProvider != null)
            {
                _casProvider.ShowAd(strAdUnitId);
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
        InitCompleted = 0,
        InitFail,
        BannerLoaded,
        BannerError,
        BannerClicked,
        BannerShown,
        BannerHidden,
        OnAdsAdLoaded,
        OnAdsFailedToLoad,
        OnAdsShowFailure,
        OnAdsShowStart,
        OnAdsShowClick,
        OnAdsShowSkip,
        OnAdsShowComplete
    }
    public class PlatformCasRet : PlatformBaseRet
    {
        private string bannerAdUnitID;
        private string adUnitID;
        private string gameID;
        private bool testMode;
        private uint casResultTyp;
        private uint casErrorTyp;

        [JsonProp("gameID")]
        public string GameID
        {
            get { return gameID; }
            set { gameID = value; }
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
