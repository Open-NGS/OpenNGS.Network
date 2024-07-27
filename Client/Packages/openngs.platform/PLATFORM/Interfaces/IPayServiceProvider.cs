using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Platform.Pay
{
    public interface IPayServiceProvider : IModuleProvider
    {
        void Initialize(string idc, string env, string idcInfo, NPayBaseRequest req, INPayCallbackInit callback);
        void SetProcess(string procressName);
        void SetLogEnable(bool enable);
        
        string GetSDKVersion();
        bool IsPayEnable();
        void Pay(NPayBaseRequest req, INPayCallbackPay callback);
        void GetLocalPrice(string channel, List<string> productIds, INGetLocalPriceCallback callback);
        void GetInfo(string reqType, NPayBaseRequest req, INPayGetInfoCallback callback);
    }
}
