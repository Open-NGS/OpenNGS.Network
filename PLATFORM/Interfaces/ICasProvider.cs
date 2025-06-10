namespace OpenNGS.Platform
{
    public interface ICasProvider : IModuleProvider
    {
        void Initialize(string appKey, string strGameID, bool bTestMode = false);
        void SetMetaData(string strMetaCategory, string strMetaKey, string strMetaValue);
        void LoadBanner(string strAdUnitId, uint nBannerPosition);
        void ShowBannerAd(string strAdUnitId);
        void HideBannerAd(string strAdUnitId);
        void LoadAd(string strAdUnitId, PlatformAdsType _typ);
        void ShowAd(string strAdUnitId, PlatformAdsType _typ);
    }
}

