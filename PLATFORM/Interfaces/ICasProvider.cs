namespace OpenNGS.Platform
{
    public interface ICasProvider : IModuleProvider
    {
        void Initialize(string strGameID, bool bTestMode = false);
        void SetMetaData(string strMetaCategory, string strMetaKey, string strMetaValue);
        void LoadBanner(string strAdUnitId, uint nBannerPosition);
        void ShowBannerAd(string strAdUnitId);
        void HideBannerAd();
        void LoadAd(string strAdUnitId);
        void ShowAd(string strAdUnitId);
    }
}

