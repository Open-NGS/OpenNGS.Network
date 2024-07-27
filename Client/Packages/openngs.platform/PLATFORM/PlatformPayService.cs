using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using OpenNGS.Platform.Pay;

namespace OpenNGS.Platform
{
    // SDK 桥接接口
    public class PlatformPayService : Singleton<PlatformPayService>
    {
        IPayServiceProvider provider = null;
        public void Init(PAY_CHANNEL channel)
        {
            this.provider = Platform.GetPay();
        }
        internal void SetProcess(string marketStr)
        {
            provider.SetProcess(marketStr);
        }

        internal void SetLogEnable(bool v)
        {
            provider.SetLogEnable(v);
        }

        internal void Initialize(string idc, string env, string idcInfo, NPayBaseRequest req, INPayCallbackInit callback)
        {
            provider.Initialize(idc, env, idcInfo, req, callback);
        }

        internal string GetSDKVersion()
        {
            return provider.GetSDKVersion();
        }

        internal bool IsPayEnable()
        {
            return provider.IsPayEnable();
        }
        internal void Pay(NPayBaseRequest req, INPayCallbackPay callback)
        {
            provider.Pay(req, callback);
        }

        internal void GetInfo(string reqType, NPayBaseRequest req, INPayGetInfoCallback callback)
        {
            provider.GetInfo(reqType, req, callback);
        }
        internal void GetLocalPrice(string channel, List<string> productIds, INGetLocalPriceCallback callback)
        {
            provider.GetLocalPrice(channel, productIds, callback);
        }

    }

}
