
#if OpenNgsIAP
using UnityEngine.Purchasing;

namespace OpenNGS.IAP.Unity.Google
{
    public class UnityIAPPlatformProvider : UnityIAPProvider
    {
        private IGooglePlayStoreExtensions m_GooglePlayStoreExtensions;

        public override void Restore()
        {
            m_GooglePlayStoreExtensions.RestoreTransactions(OnRestore);
        }
        public override void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            m_GooglePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
            base.OnInitialized(controller, extensions);
        }
    }
#endif
}