
#if SUPERSDK
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midas.UnityPay;
using MidasUnityPay;

namespace OpenNGS.Platform.Pay.MidasYouzu
{


    public class NPayCallbackReprovide : MidasReprovideCallback
    {
        public INPayCallbackInit callback;

        public NPayCallbackReprovide(INPayCallbackInit callback)
        {
            this.callback = callback;
        }
        public void OnMidasReprovideFinished(Dictionary<string, string> result)
        {
            this.callback.OnReprovideFinished(result);
        }
    }

    public class NPayCallbackPay : MidasPayCallback
    {
        public INPayCallbackPay callback;

        public NPayCallbackPay(INPayCallbackPay callback)
        {
            this.callback = callback;
        }

        public void OnMidasLoginExpired()
        {
            this.callback.OnPayLoginExpired();
        }

        public void OnMidasPayFinished(APMidasResponse result)
        {
            NPayResponse response = new NPayResponse(result.ToString());
            this.callback.OnPayFinished(response);
        }
    }

    public class NPayCallbackGetInfo : MidasGetInfoCallback
    {
        public INPayGetInfoCallback callback;

        public NPayCallbackGetInfo(INPayGetInfoCallback callback)
        {
            this.callback = callback;
        }

        public void GetInfoFinished(string type, int retCode, string json)
        {

        }

        public void OnMidasGetInfoFinished(Dictionary<string, string> result)
        {
            //this.callback.GetInfoFinished(type, retCode, json);
        }
    }

    public class NGetLocalPriceCallback
    {
        public INGetLocalPriceCallback Callback;

        public NGetLocalPriceCallback(INGetLocalPriceCallback callback)
        {
            this.Callback = callback;
        }


        /*
* 返回当前支付商品信息，用于不同国家显示金额，msg格式：
* [{"productId":"diamond100","type":"inapp","price":"HK$8.00","price_amount_micros":8000000,
*"price_currency_code":"HKD","title":"100 Diamonds (Legacy of Discord-FuriousWings)",
*"description":"Recharge 0.99$ for 100 Diamonds"},
*{"productId":"diamond1000","type":"inapp","price":"HK$78.00","price_amount_micros":78000000,
*"price_currency_code":"HKD","title":"1000 Diamonds (Legacy of Discord-FuriousWings)",
*"description":"Recharge 9.99$ for 1000 Diamonds"}, 
*{"productId":"diamond500","type":"inapp","price":"HK$38.00","price_amount_micros":38000000,
*"price_currency_code":"HKD","title":"500 Diamonds (Legacy of Discord-FuriousWings)",
*"description":"Recharge 4.99$ for 500 Diamonds"}]   完整的是这样的格式。没有就是null
*  商品id为错误时code同样是成功，result为null   get_currency_type为固定值，productid为谷歌商品id
*/
        public void OnGetProdcut(int ret, LitJson.JsonData json)
        {
            var res = new List<NProductInfo>();
            if (ret == 0)
            {
                foreach (LitJson.JsonData data in json)
                {
                    var mlp = new NProductInfo();
                    mlp.productId = data["productId"].ToString();
                    mlp.price = data["price"].ToString();
                    mlp.currency = data["price_currency_code"].ToString();
                    mlp.microprice = data["price_amount_micros"].ToString();
                    mlp.title = data["title"].ToString();
                    mlp.description = data["description"].ToString();
                    res.Add(mlp);
                }
            }
            this.Callback.OnGetProdcut(ret, res);
        }
    }


    class MidasOverseaYouzuProvider : IPayServiceProvider
    {
        public void Initialize(string idc, string env, string idcInfo, NPayBaseRequest req, INPayCallbackInit callback)
        {
            MidasPayService.Instance.Initialize(idc, env, idcInfo, req.RequestMidasYouzuInit, new NPayCallbackReprovide(callback));
        }
        public void GetInfo(string reqType, APMidasGameRequest req, MidasGetInfoCallback callback)
        {
            MidasPayService.Instance.GetInfo(reqType, req, callback);
        }

        public string GetSDKVersion()
        {
            return "1.0.0";
        }
        public bool IsPayEnable()
        {
            return true;
        }

        public void Pay(NPayBaseRequest req, INPayCallbackPay callback)
        {
            MidasPayService.Instance.Pay(req.RequestMidasYouzuPay, new NPayCallbackPay(callback));
        }

        public void SetLogEnable(bool enable)
        {
            MidasPayService.Instance.SetLogEnable(enable);
        }

        public void SetProcess(string procressName)
        {
            //MidasPayService.Instance.SetProcess(procressName);
        }

        public void GetInfo(string reqType, NPayBaseRequest req, INPayGetInfoCallback callback)
        {
            MidasPayService.Instance.GetInfo(reqType, req.RequestMidasYouzuPay, new NPayCallbackGetInfo(callback));
        }

        public void GetLocalPrice(string channel, List<string> productIds, INGetLocalPriceCallback callback)
        {
            SuperSDKPay.GetLocalPrice(channel, productIds, new NGetLocalPriceCallback(callback));
        }
    }
}
#endif
