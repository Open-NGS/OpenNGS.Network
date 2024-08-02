using OpenNGS.Exchange.Service;
using OpenNGS.Shop.Data;
using OpenNGS.Shop.Service;
using System.Collections.Generic;
using Systems;

namespace OpenNGS.Systems
{
    public class NgShopSystem : GameSubSystem<NgShopSystem>, INgShopSystem
    {
        INgExchangeSystem m_exchangeSys;
        INgItemSystem m_itemSys;

        //商店ID，货架动态信息
        private Dictionary<uint, List<ShelfState>> shopMap;

        protected override void OnCreate()
        {
            m_exchangeSys = App.GetService<INgExchangeSystem>();
            m_itemSys = App.GetService<INgItemSystem>();
            shopMap = new Dictionary<uint, List<ShelfState>>();
            InitShopSystem();
            base.OnCreate();
        }

        private void InitShopSystem()
        {
            foreach(Good good in NGSStaticData.goodDatas.Items)
            {
                GoodState _goodState = new GoodState();
                _goodState.GoodID = good.ID;
                _goodState.Left = -1;                           //剩余购买次数 -1 = 无限次

                uint _shelfID = good.ShelfId;
                Shelf _shelf = NGSStaticData.shelfDatas.GetItem(_shelfID);
                if(_shelf == null)
                {
                    NgDebug.LogErrorFormat("Good Belong Shelf is not Defined, GoodID : [{0}], ShelfID : [{1}]", good.ID, _shelfID);
                }
                OpenNGS.Shop.Data.Shop _shop = NGSStaticData.shops.GetItem(_shelf.ShopId);
                if(_shop == null)
                {
                    NgDebug.LogErrorFormat("Good Belong Shelf is not Defined, ShelfID : [{0}], ShopID : [{1}]", _shelfID, _shop.ID);
                }

                if(shopMap.ContainsKey(_shop.ID) == false)
                {
                    ShelfState _shelfState = new ShelfState();
                    _shelfState.ShelfId = _shelfID;             //货架Id
                    _shelfState.RefreshTime = 0;                //下次刷新时间 0 = 不刷新
                    _shelfState.Left = -1;                      //剩余刷新次数 -1 = 无限次
                    shopMap[_shop.ID] = new List<ShelfState>() { _shelfState };
                    _shelfState.Goods.Add(_goodState);
                }
                else
                {
                    ShelfState _shelfState = shopMap[_shop.ID].Find(item => item.ShelfId == _shelfID);
                    if(_shelfState == null)
                    {
                        _shelfState = new ShelfState();
                        _shelfState.ShelfId = _shelfID;
                        _shelfState.RefreshTime = 0;
                        _shelfState.Left = -1;
                        shopMap[_shop.ID].Add(_shelfState);
                    }
                    _shelfState.Goods.Add(_goodState);
                }
            }
        }

        public override string GetSystemName()
        {
            return "NgShop";
        }

        public BuyRsp BuyItem(BuyReq request)
        {
            BuyRsp response = new BuyRsp();
            response.result = Shop.Common.ShopResultType.Success;

            if(shopMap.TryGetValue(request.ShopId, out List<ShelfState> shelfs))
            {
                //容错判断
                ShelfState _shelf = shelfs.Find(item => item.ShelfId == request.ShelfId);
                if( _shelf == null)
                {
                    response.result = Shop.Common.ShopResultType.Error_DataInfo;
                    return response;
                }
                GoodState _good = _shelf.Goods.Find(item => item.GoodID == request.GoodId);
                if(_good == null)
                {
                    response.result = Shop.Common.ShopResultType.Error_DataInfo;
                    return response;
                }

                //判断货物的的剩余书否满足购买要求
                if( _good.Left >= 0 )
                {
                    if(_good.Left < request.GoodCounts)
                    {
                        response.result = Shop.Common.ShopResultType.Failed_NotEnough_Good;
                        return response;
                    }
                    else
                    {
                        DoExchange(response, request);
                        if(response.result == Shop.Common.ShopResultType.Success)
                        {
                            _good.Left -= (int)request.GoodCounts;
                        }
                    }
                }
                else
                {
                    DoExchange(response, request);
                }
            }
            else
            {
                response.result = Shop.Common.ShopResultType.Error_DataInfo;
                return response;
            }
            return response;
        }

        /// <summary>
        /// 执行购买逻辑
        /// </summary>
        private void DoExchange(BuyRsp response, BuyReq request)
        {
            Good _good = NGSStaticData.goodDatas.GetItem(request.GoodId);
            if( _good == null )
            {
                response.result = Shop.Common.ShopResultType.Error_DataInfo;
                return;
            }

            ExchangeByItemIDReq _exchangeReq = new ExchangeByItemIDReq();
            ItemSrcState src = new ItemSrcState();
            src.Col = m_itemSys.GetCurrencyColById(_good.CurrencyId);
            src.ItemID = _good.CurrencyId;
            src.Counts = _good.CurrencyCounts * request.GoodCounts;
            _exchangeReq.Source.Add(src);

            TargetState trg = new TargetState();
            trg.Col = request.ColIdex;
            trg.ItemID = _good.ItemId;
            trg.Counts = request.GoodCounts;
            _exchangeReq.Target.Add(trg);

            m_exchangeSys.ExchangeItemByID(_exchangeReq);
        }

        public ShopRsp GetShopState(ShopReq request)
        {
            ShopRsp _response = new ShopRsp();
            OpenNGS.Shop.Data.Shop _shop = NGSStaticData.shops.GetItem(request.ShopId);
            if(_shop == null)
            {
                _response.result = Shop.Common.ShopResultType.Error_DataInfo;
                return _response;
            }

            if(shopMap.TryGetValue(request.ShopId, out List<ShelfState> _shelfs))
            {
                _response.Shelfs.AddRange(_shelfs);
            }
            else
            {
                _response.result = Shop.Common.ShopResultType.Error_DataInfo;
                return _response;
            }

            _response.EndTime = 0;                          //开放结束时间， 0 = 不关闭
            return _response;
        }

        protected override void OnClear()
        {
            shopMap = null;
            m_exchangeSys = null;
            base.OnClear();
        }
    }
}
