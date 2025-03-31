using System.Collections.Generic;
using UnityEngine;
using static OpenNGS.Platform.Platform;
namespace OpenNGS.Platform
{
    public class PlatformIAP
    {
        public static event OnPlatformRetEventHandler<PlatformIAPRet> IAPRetEvent;
        public static void Initialize(Dictionary<string, uint> _dictProducts)
        {
            if (!Platform.IsSupported(PLATFORM_MODULE.IAP))
                return;
            IIAPProvider _iapProvider = Platform.GetIAP();
            if (_iapProvider != null)
            {
                _iapProvider.InitializePurchasing(_dictProducts);
            }
        }
        public static void Restore()
        {
            if (!Platform.IsSupported(PLATFORM_MODULE.IAP))
                return;
            IIAPProvider _iapProvider = Platform.GetIAP();
            if (_iapProvider != null)
            {
                _iapProvider.Restore();
            }
        }
        public static void Purchase(string strProductID)
        {
            if (!Platform.IsSupported(PLATFORM_MODULE.IAP))
                return;
            IIAPProvider _iapProvider = Platform.GetIAP();
            if (_iapProvider != null)
            {
                _iapProvider.Purchase(strProductID);
            }
        }
        internal static void OnIAPRet(PlatformIAPRet ret)
        {
            Debug.Log("[Platform]PlatformIAPRet:" + ret.ToJsonString());
            if (IAPRetEvent != null)
                IAPRetEvent(ret);
        }
    }
    public enum PlatFormIAPResult
    {
        Init = 0,
        InitFail,
        PurchaseFail,
        PurchaseProcess,
        InvalidStore,
        Restore
    }
    public enum PlatFormIAPPurchaseProcessRet
    {
        Success = 0,
        Failed
    }
    public class PlatformIAPRet : PlatformBaseRet
    {
        private string productID;
        private uint resultType;
        [JsonProp("productID")]
        public string ProductID
        {
            get { return productID; }
            set { productID = value; }
        }
        [JsonProp("resultType")]
        public uint ResultType
        {
            get { return resultType; }
            set { resultType = value; }
        }
        public void Init()
        {
            productID = "";
            resultType = 0;
        }
        public PlatformIAPRet()
        {
        }

        public PlatformIAPRet(string param) : base(param)
        {
        }

        public PlatformIAPRet(object json) : base(json)
        {
        }
    }

}
