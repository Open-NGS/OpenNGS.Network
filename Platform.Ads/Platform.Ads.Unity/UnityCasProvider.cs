using OpenNGS.Platform;
using UnityEngine.Advertisements;
namespace OpenNGS.Ads.Unity
{
    public class UnityCasProvider : ICasProvider, IUnityAdsInitializationListener, IUnityAdsShowListener, IUnityAdsLoadListener
    {
        public PLATFORM_MODULE Module => PLATFORM_MODULE.CAS;

        private string _adUnitId;
        private PlatformCasRet m_ret;
        public void Initialize(string strGameID, bool bTestMode = false)
        {
            if (!Advertisement.isInitialized && Advertisement.isSupported)
            {
                m_ret = new PlatformCasRet();
                m_ret.Init();
                m_ret.GameID = strGameID;
                m_ret.TestMode = bTestMode;
                Advertisement.Initialize(strGameID, bTestMode, this);
            }
        }
        public void LoadBanner(string strAdUnitId, uint nBannerPosition)
        {
            _adUnitId = strAdUnitId;
            // Set up options to notify the SDK of load events:
            BannerLoadOptions options = new BannerLoadOptions
            {
                loadCallback = OnBannerLoaded,
                errorCallback = OnBannerError
            };

            m_ret.Init();
            m_ret.AdUnitID = strAdUnitId;
            Advertisement.Banner.SetPosition((BannerPosition)nBannerPosition);
            // Load the Ad Unit with banner content:
            Advertisement.Banner.Load(strAdUnitId, options);
        }
        void OnBannerLoaded()
        {
            m_ret.CasResultTyp = (uint)PlatFormCasResult.BannerLoaded;
            _callBackCas(m_ret);
        }
        void OnBannerError(string message)
        {
            m_ret.CasResultTyp = (uint)PlatFormCasResult.BannerError;
            _callBackCas(m_ret);
        }
        public void ShowBannerAd(string strAdUnitId)
        {
            _adUnitId = strAdUnitId;
            // Set up options to notify the SDK of show events:
            BannerOptions options = new BannerOptions
            {
                clickCallback = OnBannerClicked,
                hideCallback = OnBannerHidden,
                showCallback = OnBannerShown
            };

            m_ret.Init();
            m_ret.AdUnitID = strAdUnitId;
            // Show the loaded Banner Ad Unit:
            Advertisement.Banner.Show(strAdUnitId, options);
        }
        void OnBannerClicked()
        {
            m_ret.CasResultTyp = (uint)PlatFormCasResult.BannerClicked;
            _callBackCas(m_ret);
        }
        void OnBannerShown()
        {
            m_ret.CasResultTyp = (uint)PlatFormCasResult.BannerShown;
            _callBackCas(m_ret);
        }
        void OnBannerHidden()
        {
            m_ret.CasResultTyp = (uint)PlatFormCasResult.BannerHidden;
            _callBackCas(m_ret);
        }
        public void HideBannerAd()
        {
            Advertisement.Banner.Hide();
        }
        public void LoadAd(string strAdUnitId)
        {
            m_ret.Init();
            _adUnitId = strAdUnitId;
            Advertisement.Load(strAdUnitId, this);
        }

        public void ShowAd(string strAdUnitId)
        {
            m_ret.Init();
            _adUnitId = strAdUnitId;
            Advertisement.Show(strAdUnitId, this);
        }

        // call back 
        public void OnInitializationComplete()
        {
            m_ret.CasResultTyp = (uint)PlatFormCasResult.InitCompleted;
            _callBackCas(m_ret);
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            m_ret.CasResultTyp = (uint)PlatFormCasResult.InitFail;
            m_ret.RetCode = (int)error;
            m_ret.RetMsg = message;
            _callBackCas(m_ret);
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            m_ret.CasResultTyp = (uint)PlatFormCasResult.OnAdsShowFailure;
            m_ret.RetCode = (int)error;
            m_ret.RetMsg = message;
            _callBackCas(m_ret);
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            m_ret.CasResultTyp = (uint)PlatFormCasResult.OnAdsShowStart;
            m_ret.AdUnitID = placementId;
            _callBackCas(m_ret);
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            m_ret.CasResultTyp = (uint)PlatFormCasResult.OnAdsShowClick;
            m_ret.AdUnitID = placementId;
            _callBackCas(m_ret);
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            if (placementId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
            {
                m_ret.CasResultTyp = (uint)PlatFormCasResult.OnAdsShowComplete;
                m_ret.AdUnitID = placementId;
                _callBackCas(m_ret);
            }
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            if (placementId.Equals(_adUnitId))
            {
                m_ret.CasResultTyp = (uint)PlatFormCasResult.OnAdsAdLoaded;
                m_ret.AdUnitID = placementId;
                _callBackCas(m_ret);
            }
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            m_ret.CasResultTyp = (uint)PlatFormCasResult.OnAdsFailedToLoad;
            m_ret.AdUnitID = placementId;
            m_ret.RetCode = (int)error;
            m_ret.RetMsg = message;
            _callBackCas(m_ret);
        }
        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Update()
        {
        }

        private void _callBackCas(PlatformCasRet _ret)
        {
            PlatformCallback.Instance.OnCasCallBack(_ret);
        }
    }

}