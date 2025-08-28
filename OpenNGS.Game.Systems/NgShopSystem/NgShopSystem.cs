using OpenNGS.Common;
using OpenNGS.Exchange.Common;
using OpenNGS.Exchange.Service;
using OpenNGS.Shop.Common;
using OpenNGS.Shop.Data;
using OpenNGS.Shop.Service;
using System;
using System.Collections.Generic;
using System.Xml;
using Systems;

namespace OpenNGS.Systems
{
    public class NgShopSystem : GameSubSystem<NgShopSystem>, INgShopSystem
    {
        INgExchangeSystem m_exchangeSys;
        INgItemSystem m_itemSys;

        private Dictionary<uint, List<ShelfState>> shopMap;
        // 添加外部时间控制字段
        private DateTimeOffset? _currentTime;
        public override string GetSystemName()
        {
            return "NgShop";
        }

        // 允许外部设置当前时间（天数、小时、分钟、秒）
        public void SetCurrentTime(int days, int hours, int minutes, int seconds)
        {
            // 假设从某个固定起点开始计算（如游戏开服时间）
            var baseDate = new DateTime(2023, 1, 1);
            _currentTime = baseDate.AddDays(days)
                                 .AddHours(hours)
                                 .AddMinutes(minutes)
                                 .AddSeconds(seconds);
        }

        // 获取当前时间（如果未设置外部时间则使用系统时间）
        private DateTimeOffset GetCurrentTime()
        {
            return _currentTime ?? DateTimeOffset.UtcNow;
        }
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
            shopMap.Clear();
            foreach (Good good in ShopStaticData.goodDatas.Items)
            {
                GoodState goodState = new GoodState { GoodID = good.ID, Left = good.Limit > 0 ? (int)good.Limit : -1 };

                uint shelfID = good.ShelfId;
                Shelf shelf = ShopStaticData.shelfDatas.GetItem(shelfID);
                if (shelf == null)
                {
                    NgDebug.LogErrorFormat("Good Belong Shelf is not Defined, GoodID: {0}, ShelfID: {1}", good.ID, shelfID);
                    continue;
                }

                OpenNGS.Shop.Data.Shop shop = ShopStaticData.shops.GetItem(shelf.ShopId);
                if (shop == null)
                {
                    NgDebug.LogErrorFormat("Shop not Defined, ShelfID: {0}, ShopID: {1}", shelfID, shelf.ShopId);
                    continue;
                }

                if (!shopMap.TryGetValue(shop.ID, out List<ShelfState> shelfList))
                {
                    shelfList = new List<ShelfState>();
                    shopMap[shop.ID] = shelfList;
                }

                ShelfState shelfState = shelfList.Find(item => item.ShelfId == shelfID);
                if (shelfState == null)
                {
                    shelfState = new ShelfState
                    {
                        ShelfId = shelfID,
                        RefreshPeriod = shelf.RefreshTime, // 存储ISO 8601字符串（如 "P1D"）
                        Left = -1
                    };
                    UpdateShelfRefreshTime(shelfState); // 初始化刷新时间
                    shelfList.Add(shelfState);
                }
                shelfState.Goods.Add(goodState);
            }
        }

        /// <summary>
        /// 检查并更新货架刷新状态
        /// </summary>
        private bool CheckAndUpdateShelfRefresh(ShelfState shelf)
        {
            if (string.IsNullOrEmpty(shelf.RefreshPeriod))
                return false;

            // 使用注入的时间而非系统时间
            DateTimeOffset now = GetCurrentTime();
            if (now.ToUnixTimeSeconds() < shelf.RefreshTime)
                return false;

            // 需要刷新
            UpdateShelfRefreshTime(shelf);
            return true;
        }

        /// <summary>
        /// 解析ISO 8601时间段并计算下次刷新时间（基于XmlConvert）
        /// </summary>
        private void UpdateShelfRefreshTime(ShelfState shelf)
        {
            if (string.IsNullOrEmpty(shelf.RefreshPeriod))
                return;

            try
            {
                TimeSpan refreshInterval = XmlConvert.ToTimeSpan(shelf.RefreshPeriod);

                // 基于当前注入时间计算下次刷新
                DateTimeOffset nextRefresh = GetCurrentTime() + refreshInterval;
                shelf.RefreshTime = nextRefresh.ToUnixTimeSeconds();
            }
            catch
            {
                shelf.RefreshTime = 0;
            }
        }

        public BuyRsp BuyItem(BuyReq request)
        {
            BuyRsp response = new BuyRsp { result = ShopResultType.Success };

            if (!shopMap.TryGetValue(request.ShopId, out List<ShelfState> shelfs))
            {
                response.result = ShopResultType.Error_DataInfo;
                return response;
            }

            ShelfState shelf = shelfs.Find(item => item.ShelfId == request.ShelfId);
            if (shelf == null)
            {
                response.result = ShopResultType.Error_DataInfo;
                return response;
            }

            // 检查是否需要刷新
            if (CheckAndUpdateShelfRefresh(shelf))
            {
                // 重置商品状态
                foreach (var good in shelf.Goods)
                {
                    Good originalGood = ShopStaticData.goodDatas.GetItem(good.GoodID);
                    good.Left = originalGood?.Limit > 0 ? (int)originalGood.Limit : -1;
                }
            }

            // 原有购买逻辑...
            GoodState goodState = shelf.Goods.Find(item => item.GoodID == request.GoodId);
            if (goodState == null)
            {
                response.result = ShopResultType.Error_DataInfo;
                return response;
            }

            if (goodState.Left >= 0 && goodState.Left < request.GoodCounts)
            {
                response.result = ShopResultType.Failed_NotEnough_Good;
                return response;
            }

            DoExchange(response, request);
            if (response.result == ShopResultType.Success && goodState.Left > 0)
            {
                goodState.Left -= (int)request.GoodCounts;
            }

            return response;
        }
        /// <summary>
        /// 执行购买逻辑
        /// </summary>
        private void DoExchange(BuyRsp response, BuyReq request)
        {
            Good _good = ShopStaticData.goodDatas.GetItem(request.GoodId);
            if (_good == null)
            {
                response.result = Shop.Common.ShopResultType.Error_DataInfo;
                return;
            }

            ExchangeByItemIDReq _exchangeReq = new ExchangeByItemIDReq();
            ItemSrcState src = new ItemSrcState();
            src.Col = m_itemSys.GetCurrencyColById(_good.BuyID);
            src.ItemID = _good.BuyID;
            src.Counts = _good.Price * request.GoodCounts;
            _exchangeReq.Source.Add(src);

            TargetState trg = new TargetState();
            trg.Col = request.ColIdex;
            trg.ItemID = _good.ItemId;
            trg.Counts = request.GoodCounts;
            _exchangeReq.Target.Add(trg);
            ExchangeRsp exchangeRsp = m_exchangeSys.ExchangeItemByID(_exchangeReq);
            if (exchangeRsp.result == ExchangeResultType.Error_NotDefine_Target || exchangeRsp.result == ExchangeResultType.Error_NotExist_Source)
            {
                response.result = ShopResultType.Error_DataInfo;
            }
            else if (exchangeRsp.result == ExchangeResultType.Failed_OverLimitNum)
            {
                response.result = ShopResultType.Failed_ItemOverLimit;
            }
            else if (exchangeRsp.result == ExchangeResultType.Failed_NotEnough)
            {
                response.result = ShopResultType.Failed_NotEnough_Gold;
            }
        }
        public ShopRsp GetShopState(ShopReq request)
        {
            ShopRsp _response = new ShopRsp();
            OpenNGS.Shop.Data.Shop _shop = ShopStaticData.shops.GetItem(request.ShopId);
            if (_shop == null)
            {
                _response.result = Shop.Common.ShopResultType.Error_DataInfo;
                return _response;
            }

            if (shopMap.TryGetValue(request.ShopId, out List<ShelfState> _shelfs))
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

        /// <summary>
        /// 获取指定商品的剩余限购数量
        /// </summary>
        /// <param name="goodId">商品ID</param>
        /// <param name="shelfId">货架ID</param>
        /// <returns>剩余数量（-1表示无限）</returns>
        public int GetGoodRemainingLimit(uint goodId, uint shelfId,
            int? customDays = null, int? customHours = null,
            int? customMinutes = null, int? customSeconds = null)
        {
            // 如果传入了自定义时间，则更新系统时间
            if (customDays.HasValue || customHours.HasValue ||
                customMinutes.HasValue || customSeconds.HasValue)
            {
                SetCurrentTime(
                    customDays ?? 0,
                    customHours ?? 0,
                    customMinutes ?? 0,
                    customSeconds ?? 0
                );
            }

            // 检查所有货架是否需要刷新
            foreach (var shop in shopMap.Values)
            {
                foreach (var shelf in shop)
                {
                    CheckAndUpdateShelfRefresh(shelf);
                }
            }

            // 查找商品逻辑（保持不变）
            foreach (var shopPair in shopMap)
            {
                var shelfState = shopPair.Value.Find(s => s.ShelfId == shelfId);
                if (shelfState == null) continue;

                var goodState = shelfState.Goods.Find(g => g.GoodID == goodId);
                if (goodState != null) return goodState.Left;
            }

            return -1;
        }

        DateTimeOffset INgShopSystem.GetCurrentTime()
        {
            return GetCurrentTime();
        }

        public void ResetCreateTime(int nYear, int nMonth, int nDay)
        {
            var baseDate = new DateTime(nYear, nMonth, nDay);
            _currentTime = baseDate.AddHours(0)
                                 .AddMinutes(0)
                                 .AddSeconds(0);
            InitShopSystem();
        }
    }
}