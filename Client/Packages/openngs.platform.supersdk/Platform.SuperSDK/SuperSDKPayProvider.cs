using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Platform.Pay
{

    class SuperSDKPayProvider : IPayServiceProvider
    {
        public PLATFORM_MODULE Module => PLATFORM_MODULE.PAY;

        public void GetInfo(string reqType, NPayBaseRequest req, INPayGetInfoCallback callback)
        {
            
        }

        public void GetLocalPrice(string channel, List<string> productIds, INGetLocalPriceCallback callback)
        {
            
        }

        public string GetSDKVersion()
        {
            return "4.0.0";
        }

        public void Initialize(string idc, string env, string idcInfo, NPayBaseRequest req, INPayCallbackInit callback)
        {
           
        }

        public bool IsPayEnable()
        {
            return true;
            
        }

        public void Pay(NPayBaseRequest req, INPayCallbackPay callback)
        {
            
        }

        public void SetLogEnable(bool v)
        {
            
        }

        public void SetProcess(string marketStr)
        {
        }
    }
}
