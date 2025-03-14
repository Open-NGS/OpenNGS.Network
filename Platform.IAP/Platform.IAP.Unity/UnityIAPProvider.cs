using System.Collections.Generic;
using OpenNGS.Platform;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;

namespace OpenNGS.IAP.Unity
{
#if OpenNgsIAP

    public class UnityIAPProvider : IIAPProvider, IIAPExtension, IDetailedStoreListener
    {
        public PLATFORM_MODULE Module => PLATFORM_MODULE.IAP;
        protected PlatformIAPRet m_ret = new PlatformIAPRet();
        private IStoreController m_StoreController;

        CrossPlatformValidator m_Validator = null;
        public virtual void InitializePurchasing(Dictionary<string, uint> _dictProducts)
        {
            m_ret = new PlatformIAPRet();

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            foreach (var product in _dictProducts)
            {
                builder.AddProduct(product.Key, (ProductType)product.Value);
            }

            UnityPurchasing.Initialize(this, builder);
        }

        public virtual void Purchase(string productID)
        {
            m_ret = new PlatformIAPRet();
            m_ret.ProductID = productID;
            m_StoreController.InitiatePurchase(productID);
        }

        public virtual void Restore()
        {
            m_ret = new PlatformIAPRet();
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            m_ret.ResultType = (uint)PlatFormIAPResult.PurchaseFail;
            m_ret.RetCode = (int)failureDescription.reason;
            _callBackIAP(m_ret);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            m_ret.ResultType = (uint)PlatFormIAPResult.InitFail;
            m_ret.RetCode = (int)error;
            _callBackIAP(m_ret);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            m_ret.ResultType = (uint)PlatFormIAPResult.InitFail;
            m_ret.RetCode = (int)error;
            m_ret.RetMsg = message;
            _callBackIAP(m_ret);
        }

        bool IsPurchaseValid(Product product)
        {
            bool bRes = false;
            //If we the validator doesn't support the current store, we assume the purchase is valid
            if (IsCurrentStoreSupportedByValidator())
            {
                try
                {
                    var result = m_Validator.Validate(product.receipt);

                    //The validator returns parsed receipts.
                    //LogReceipts(result);
                    bRes = true;
                }

                //If the purchase is deemed invalid, the validator throws an IAPSecurityException.
                catch (IAPSecurityException reason)
                {
                    //Debug.Log($"Invalid receipt: {reason}");
                    return false;
                }
            }
            return true;
        }

        static void LogReceipts(IEnumerable<UnityEngine.Purchasing.Security.IPurchaseReceipt> receipts)
        {
            foreach (var receipt in receipts)
            {
                LogReceipt(receipt);
            }
        }


        static void LogReceipt(IPurchaseReceipt receipt)
        {
            //Debug.Log($"Product ID: {receipt.productID}\n" +
            //    $"Purchase Date: {receipt.purchaseDate}\n" +
            //    $"Transaction ID: {receipt.transactionID}");

            if (receipt is GooglePlayReceipt googleReceipt)
            {
                //Debug.Log($"Purchase State: {googleReceipt.purchaseState}\n" +
                //    $"Purchase Token: {googleReceipt.purchaseToken}");
            }

            if (receipt is AppleInAppPurchaseReceipt appleReceipt)
            {
                //Debug.Log($"Original Transaction ID: {appleReceipt.originalTransactionIdentifier}\n" +
                //    $"Subscription Expiration Date: {appleReceipt.subscriptionExpirationDate}\n" +
                //    $"Cancellation Date: {appleReceipt.cancellationDate}\n" +
                //    $"Quantity: {appleReceipt.quantity}");
            }
        }


        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            //Retrieve the purchased product
            var product = args.purchasedProduct;

            var isPurchaseValid = IsPurchaseValid(product);

            if (isPurchaseValid)
            {
                //Add the purchased product to the players inventory

                m_ret.ResultType = (uint)PlatFormIAPResult.PurchaseProcess;
                m_ret.RetCode = (int)PlatFormIAPPurchaseProcessRet.Success;

                //Debug.Log("Valid receipt, unlocking content.");
            }
            else
            {
                //Debug.Log("Invalid receipt, not unlocking content.");
                m_ret.ResultType = (uint)PlatFormIAPResult.PurchaseProcess;
                m_ret.RetCode = (int)PlatFormIAPPurchaseProcessRet.Failed;
            }
            _callBackIAP(m_ret);

            //We return Complete, informing Unity IAP that the processing on our side is done and the transaction can be closed.
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            m_ret.ResultType = (uint)PlatFormIAPResult.PurchaseFail;
            m_ret.RetCode = (int)failureReason;
            _callBackIAP(m_ret);
        }

        public virtual void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            m_StoreController = controller;
            bool bRes = InitializeValidator();
            if (bRes == true)
            {
                m_ret.ResultType = (uint)PlatFormIAPResult.Init;
            }
            else
            {
                m_ret.ResultType = (uint)PlatFormIAPResult.InvalidStore;
            }
            _callBackIAP(m_ret);
        }

        protected virtual void _PostInitialized(IStoreController controller, IExtensionProvider extensions)
        {
        }

        static bool IsAppleAppStoreSelected()
        {
            var currentAppStore = StandardPurchasingModule.Instance().appStore;
            return currentAppStore == AppStore.AppleAppStore ||
                currentAppStore == AppStore.MacAppStore;
        }
        static bool IsGooglePlayStoreSelected()
        {
            var currentAppStore = StandardPurchasingModule.Instance().appStore;
            return currentAppStore == AppStore.GooglePlay;
        }
        static bool IsCurrentStoreSupportedByValidator()
        {
            //The CrossPlatform validator only supports the GooglePlayStore and Apple's App Stores.
            return IsGooglePlayStoreSelected() || IsAppleAppStoreSelected();
        }
        bool InitializeValidator()
        {
            if (IsCurrentStoreSupportedByValidator())
            {
#if !UNITY_EDITOR
                var appleTangleData = m_UseAppleStoreKitTestCertificate ? AppleStoreKitTestTangle.Data() : AppleTangle.Data();
                m_Validator = new CrossPlatformValidator(GooglePlayTangle.Data(), appleTangleData, Application.identifier);
#endif
                return true;
            }
            else
            {
                //todo log
                //userWarning.WarnInvalidStore(StandardPurchasingModule.Instance().appStore);
            }
            return false;
        }

        private void _callBackIAP(PlatformIAPRet _ret)
        {
            PlatformCallback.Instance.OnIAPCallBack(_ret);
        }
        public virtual void OnRestore(bool success, string error)
        {
            m_ret.ResultType = (uint)PlatFormIAPResult.Restore;
            if (success == true)
            {
                m_ret.RetCode = (int)PlatFormIAPPurchaseProcessRet.Success;
            }
            else
            {
                m_ret.RetCode = (int)PlatFormIAPPurchaseProcessRet.Failed;
                m_ret.RetMsg = error;
            }
            _callBackIAP(m_ret);
        }
        public void Stop()
        {
        }
        public void Start()
        {

        }
        public void Update()
        {
        }
    }

#endif
}