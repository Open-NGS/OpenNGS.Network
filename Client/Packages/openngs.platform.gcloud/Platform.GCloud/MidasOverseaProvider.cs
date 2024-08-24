#if !SUPERSDK
using MidasPay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Platform.Pay.Midas
{
    public class NPayCallbackInit : MidasInitCallback
    {
        public INPayCallbackInit callback;

        public NPayCallbackInit(INPayCallbackInit callback)
        {
            this.callback = callback;
        }
        public void OnMidasInitFinished(Dictionary<string, object> result)
        {
            this.callback.OnInitFinished(result);
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
            this.callback.GetInfoFinished(type, retCode, json);
        }
    }



    public class NGetLocalPriceCallback : MidasGetLocalPriceCallback
    {
        public INGetLocalPriceCallback Callback;

        public NGetLocalPriceCallback(INGetLocalPriceCallback callback)
        {
            this.Callback = callback;
        }

        public void OnMidasGetProdcut(Dictionary<string, object> result)
        {
            var res = new List<NProductInfo>();
            var ret = (int)result["ret"];
            if (ret == 0)
            {
                List<object> arr = (List<object>)result["productInfo"];
                foreach (object o in arr)
                {
                    Dictionary<string, object> dic = (Dictionary<string, object>)o;
                    var mlp = new NProductInfo();
                    mlp.productId = dic["productId"].ToString();
                    mlp.price = dic["price"].ToString();
                    mlp.currency = dic["currency"].ToString();
                    mlp.microprice = dic["microprice"].ToString();
                    res.Add(mlp);
                }
            }
            this.Callback.OnGetProdcut(ret, res);
        }
    }


    class MidasOverseaProvider : IPayServiceProvider
    {
        public Platform_MODULE Module => Platform_MODULE.PAY;

        public void Initialize(string idc, string env, string idcInfo, NPayBaseRequest req, INPayCallbackInit callback)
        {
            MidasPayService.Instance.Initialize(idc, env, idcInfo, req.RequestMidas, new NPayCallbackInit(callback));
        }


        public void GetInfo(string reqType, NPayBaseRequest req, INPayGetInfoCallback callback)
        {
            MidasPayService.Instance.GetInfo(reqType, req.RequestMidas, new NPayCallbackGetInfo(callback));
        }
        public void GetLocalPrice(string channel, List<string> productIds, INGetLocalPriceCallback callback)
        {
            MidasPayService.Instance.GetLocalPrice(channel, productIds, new NGetLocalPriceCallback(callback));
        }

        public string GetSDKVersion()
        {
            return MidasPayService.Instance.GetMidasSDKVersion();
        }
        public bool IsPayEnable()
        {
            return MidasPayService.Instance.IsPayEnable();
        }

        public void Pay(NPayBaseRequest req, INPayCallbackPay callback)
        {
            MidasPayService.Instance.Pay(req.RequestMidas, new NPayCallbackPay(callback));
        }

        public void SetLogEnable(bool enable)
        {
            MidasPayService.Instance.SetLogEnable(enable);
        }

        public void SetProcess(string procressName)
        {
            MidasPayService.Instance.SetProcess(procressName);
        }
    }
}

#endif
