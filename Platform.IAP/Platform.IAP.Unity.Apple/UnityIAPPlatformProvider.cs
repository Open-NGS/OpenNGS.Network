
#if OpenNgsIAP
using UnityEngine.Purchasing;

namespace OpenNGS.IAP.Unity.Apple
{
    public class UnityIAPPlatformProvider : UnityIAPProvider
    {
        private IAppleExtensions m_AppleExtensions;

        public override void Restore()
        {
            m_AppleExtensions.RestoreTransactions(OnRestore);
        }
        public override void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
            base.OnInitialized(controller, extensions);
        }
    }
}
#endif