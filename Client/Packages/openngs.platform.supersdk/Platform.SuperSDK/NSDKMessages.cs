#if SUPERSDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenNGSMessages
{
    public MidasUnityPay.APMidasInitRequest RequestMidasYouzuInit
    {
        get
        {
            MidasUnityPay.APMidasInitRequest req = new MidasUnityPay.APMidasInitRequest()
            {
                offerId = this.offerId,
                openId = this.openId,
                zoneId = this.zoneId,
                goodsZoneId = this.goodsZoneId,
                pf = this.pf,
                appExtends = this.appExtends,
                channelExtras = this.channelExtras,
                openKey = this.openKey,
                sessionId = this.sessionId,
                sessionType = this.sessionType,
                pfKey = this.pfKey,
                currencyType = this.currencyType,
                country = this.country
            };
            return req;
        }
    }

    public MidasUnityPay.APMidasBasePayRequest RequestMidasYouzuPay
    {
        get
        {
            MidasUnityPay.APMidasBasePayRequest req = null;
            switch (this.Type)
            {
                case NPayRequestType.GameRequest:
                    {
                        req = new MidasUnityPay.APMidasGameRequest();
                        this.InitBaseRequest(req);
                        //req.saveValue = this.count;
                        req.productId = this.productId;
                        //req.payItem = this.count;
                        //在上面的 InitBaseRequest() 中填充正确
                        //req.appExtends = this.appExtends;
                    }
                    break;
                case NPayRequestType.GoodsRequest:
                    {
                        req = new MidasUnityPay.APMidasGoodsRequest();
                        this.InitBaseRequest(req);
                        //req.saveValue = this.count;
                        req.productId = this.productId;
                        //req.payItem = this.payItem;
                        //在上面的 InitBaseRequest() 中填充正确
                        //req.appExtends = this.appExtends;
                        req.pf = this.pf;
                    }
                    break;
                default:
                    req = new MidasUnityPay.APMidasGameRequest();
                    this.InitBaseRequest(req);
                    break;
            }

            return req;
        }
    }

    

        private void InitBaseRequest(MidasUnityPay.APMidasBasePayRequest req)
        {
            req.offerId = this.offerId;
            req.openId = this.openId;
            req.zoneId = this.zoneId;
            req.goodsZoneId = this.goodsZoneId;
            req.pf = this.pf;
            req.appExtends = this.appExtends;
            req.channelExtras = this.channelExtras;
            req.openKey = this.openKey;
            req.sessionId = this.sessionId;
            req.sessionType = this.sessionType;
            req.pfKey = this.pfKey;
            req.currencyType = this.currencyType;
            req.country = this.country;
            req.payChannel = "os_youzu";

            // 传递游族支付相关的参数
            var appExtends = new Dictionary<string, string>();
            appExtends.Add(SuperSDKPlatform.KEY_PRICE, this.price);
            appExtends.Add(SuperSDKPlatform.KEY_PRODUCT_NAME, this.productName); //购买的商品名字
            appExtends.Add(SuperSDKPlatform.KEY_PRODUCT_DESC, this.productDesc); //购买的商品的描述
            appExtends.Add(SuperSDKPlatform.KEY_POINT_RATE, this.pointRate); //比如萌江湖中是1元换10个元宝,那么pointRate填10, pointName填 元宝
            appExtends.Add(SuperSDKPlatform.KEY_POINT_NAME, this.pointName); //比如萌江湖中是1元换10个元宝,那么pointRate填10, pointName填 元宝
            appExtends.Add(SuperSDKPlatform.KEY_ORDER_TITLE, this.orderTitle); //订单标题，部分平台的订单标题有要求
            req.appExtends = global::Midas.Comm.CommTools.Map2String(appExtends);
        }
}
#endif
