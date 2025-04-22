using OpenNGS.Ads.Unity;
using OpenNGS.Share.Unity;
using OpenNGS.IAP.Unity;
namespace OpenNGS.Platform.EEGames
{
    public class EEGamesSDKProvider : ISDKProvider
    {
        public IModuleProvider CreateProvider(PLATFORM_MODULE module)
        {
            if (module == PLATFORM_MODULE.LOGIN)
            {
                return new EEGamesLoginProvider();
            }
            else if (module == PLATFORM_MODULE.REPORT)
            {
                return new EEGamesReportProvider(Platform.InitOption);
            }
            else if (module == PLATFORM_MODULE.NOTICE)
            {
                return new EEGamesNoticeProvider();
            }
            else if (module == PLATFORM_MODULE.CAS)
            {
                return new UnityCasProvider();
            }
            else if (module == PLATFORM_MODULE.IAP)
            {
                return new UnityIAPProvider();
            }
            else if (module == PLATFORM_MODULE.SHARE)
            {
                return new UnityShareProvider();
            }
            else
            {
                //Debug.LogError("Not supported provider " + module);
            }
            return null;
        }

        public bool Initialize()
        {
            return true;
        }

        public void Terminate()
        {
        }

        public void Update()
        {
        }
    }
}