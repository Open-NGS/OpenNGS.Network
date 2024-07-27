using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Platform.Pay
{

    public interface INPayCallbackInit
    {
        /// <summary>
        /// 初始化的回调。如果涉及到补发货，会通过此回调通知app补发货的信息
        /// </summary>
        void OnInitFinished(Dictionary<string, object> result);

        ///<summary>
        ///补发货的回调。业务主动调用补发货接口的情况下，会通过此回调返回结果。
        ///</summary>
        void OnReprovideFinished(Dictionary<string, string> result);
    }

    ///<summary>
    ///callback for payment
    ///</summary>
    public interface INPayCallbackPay
    {
        void OnPayLoginExpired();
        void OnPayFinished(NPayResponse result);
    }

    public interface INPayGetInfoCallback
    {
        //param type:"mp","short_openid"
        //param retCode: 0 is success; other is failed
        //param json: the market and product info
        void GetInfoFinished(string type, int retCode, string json);
    }


    public interface INGetLocalPriceCallback
    {
        void OnGetProdcut(int retCode, List<NProductInfo> result);
    }
}
