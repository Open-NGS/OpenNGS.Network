
namespace OpenNGS.Platform.Pay
{

    public enum NPayRequestType
    {
        InitRequest,
        GameRequest,
        GoodsRequest,
    }
    public class NPayBaseRequest
    {
        public string offerId;
        public string openId;
        public string zoneId;
        public string goodsZoneId;
        public string pf;
        public string appExtends;
        public string channelExtras;
        public string openKey;
        public string sessionId;
        public string sessionType;
        public string pfKey;
        public string currencyType;
        public string country;

        public NPayRequestType Type;
        public string extras;
        public string productId;
        public string payItem;
        public string count;
        public int tokenType;
        public string goodsTokenUrl;
        public string price;
        public string productName;
        public string productDesc;
        public string pointRate;
        public string pointName;
        public string orderTitle;

        public NPayBaseRequest(NPayRequestType type)
        {
            this.Type = type;
        }

       

//        public MidasPay.APMidasBaseRequest RequestMidas
//        {
//            get
//            {
//                MidasPay.APMidasBaseRequest req = null;
//                switch (this.Type)
//                {
//                    case NPayRequestType.InitRequest:
//                        req = new MidasPay.APMidasBaseRequest();
//                        this.InitBaseRequest(req);

//                        break;
//                    case NPayRequestType.GameRequest:
//                        {
//                            req = new MidasPay.APMidasGameRequest();
//                            this.InitBaseRequest(req);
//                            req.saveValue = this.count;
//#if UNITY_ANDROID

//#elif UNITY_IOS
//			                req.productId = this.productId;
//			                req.payItem = this.count;
//			                req.appExtends = this.appExtends;
//#endif
//                        }
//                        break;
//                    case NPayRequestType.GoodsRequest:
//                        {
//                            req = new MidasPay.APMidasGoodsRequest();
//                            ((MidasPay.APMidasGoodsRequest)req).tokenType = this.tokenType;
//                            ((MidasPay.APMidasGoodsRequest)req).goodsTokenUrl = this.goodsTokenUrl;
//                            req.saveValue = this.count;
//#if UNITY_ANDROID

//#elif UNITY_IOS
//			                req.productId = this.productId;
//			                req.payItem = this.payItem;
//			                req.appExtends = this.appExtends;
//                            req.pf = this.pf;
//#endif
//                        }
//                        break;
//                    default:
//                        req = new MidasPay.APMidasBaseRequest();
//                        break;
//                }

//                return req;
//            }
//        }



//        private void InitBaseRequest(MidasPay.APMidasBaseRequest req)
//        {
//            req.offerId = this.offerId;
//            req.openId = this.openId;
//            req.zoneId = this.zoneId;
//            req.goodsZoneId = this.goodsZoneId;
//            req.pf = this.pf;
//            req.appExtends = this.appExtends;
//            req.channelExtras = this.channelExtras;
//            req.openKey = this.openKey;
//            req.sessionId = this.sessionId;
//            req.sessionType = this.sessionType;
//            req.pfKey = this.pfKey;
//            req.currencyType = this.currencyType;
//            req.country = this.country;
//        }


    }
}
