
using System.Collections.Generic;
using OpenNGS.Platform;
using UnityEngine;
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

#if UNITY_ANDROID
        private IGooglePlayStoreExtensions m_StoreExtensions;
#elif UNITY_IOS
        private IAppleExtensions m_StoreExtensions;
#endif

        private PlatformIAPConfig m_Config;

        CrossPlatformValidator m_Validator = null;
        public virtual void InitializePurchasing(Dictionary<string, uint> _dictProducts, PlatformIAPConfig _config)
        {
            m_ret = new PlatformIAPRet();

            m_Config = _config;
#if UNITY_EDITOR
            StandardPurchasingModule.Instance().useFakeStoreAlways = true;
#endif
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

#if UNITY_ANDROID || UNITY_IOS
            if (m_StoreExtensions != null)
            {
                m_StoreExtensions.RestoreTransactions(OnRestore);
            }
            else
            {
                m_ret.ResultType = (uint)PlatFormIAPResult.RestoreFail;
                m_ret.RetMsg = "StoreExtensions Is Null!";
                _callBackIAP(m_ret);
            }
#endif
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
#if !UNITY_EDITOR
                    var result = m_Validator.Validate(product.receipt);
                    //The validator returns parsed receipts.
                    LogReceipts(result);
#endif
                    bRes = true;
                }

                //If the purchase is deemed invalid, the validator throws an IAPSecurityException.
                catch (IAPSecurityException reason)
                {
                    //Debug.Log($"Invalid receipt: {reason}");
                    m_ret.RetMsg = reason.Message;
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

            m_ret.ProductID = product.definition.id;
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
#if UNITY_EDITOR
            // Editor环境下不是Android也不是iOS，所以就不需要判断Validator是否合法
            bRes = true;
#endif
            if (bRes == true)
            {
                m_ret.ResultType = (uint)PlatFormIAPResult.Init;
            }
            else
            {
                m_ret.ResultType = (uint)PlatFormIAPResult.InvalidStore;
            }

#if UNITY_ANDROID
            m_StoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
#elif UNITY_IOS
            m_StoreExtensions = extensions.GetExtension<IAppleExtensions>();
#endif
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
                var appleTangleData = m_Config.UseAppleStoreKitTestCertificate ? m_Config.AppleStoreKitTestTangleData : m_Config.AppleTangleData;
                m_Validator = new CrossPlatformValidator(m_Config.GooglePlayTangleData, appleTangleData, Application.identifier);
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

        public void GetPriceByID(string productID)
        {
            if (m_StoreController == null || m_StoreController.products == null)
            {
                m_ret.ProductID = productID;
                m_ret.ResultType = (uint)PlatFormIAPResult.GetPriceFail;
                m_ret.RetMsg = "StoreController or products is Null! Check Init!";
                _callBackIAP(m_ret);
                return;
            }

            m_ret.ProductID = productID;
            string priceStr = "";
            decimal price = 0;
            foreach (var product in m_StoreController.products.all)
            {
                if (productID == product.definition.id)
                {
                    priceStr = product.metadata.localizedPriceString;
                    price = product.metadata.localizedPrice;
                    break;
                }
            }
            // 未查询到相关ID的价格信息
            if (string.IsNullOrEmpty(priceStr))
            {
                m_ret.ResultType = (uint)PlatFormIAPResult.GetPriceFail;
                m_ret.RetMsg = $"Can't Find Product ID {productID}!";
            }
            else
            {
                m_ret.ResultType = (uint)PlatFormIAPResult.GetPrice;
                m_ret.ProductPrice = priceStr;
                m_ret.Price = price;
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
#else
    public class UnityIAPProvider : IIAPProvider
    {
        public PLATFORM_MODULE Module => PLATFORM_MODULE.IAP;

        public void GetPriceByID(string productID)
        {

        }

        public void InitializePurchasing(Dictionary<string, uint> _dictProducts, PlatformIAPConfig _config)
        {

        }

        public void Purchase(string productID)
        {

        }

        public void Restore()
        {

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
    }
#endif
}