using OpenNGS.Exchange.Common;
using OpenNGS.Exchange.Service;
using OpenNGS.Item.Data;
using OpenNGS.Shop.Common;
using OpenNGS.Shop.Data;
using OpenNGS.Shop.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Systems;

namespace OpenNGS.Systems
{
    public class NgShopSystem : GameSubSystem<NgShopSystem>, INgShopSystem
    {
        private const long CONST_RATE = 1000L;
        INgExchangeSystem m_exchangeSys;
        INgItemSystem m_itemSys;
        private Dictionary<uint, ShopState> shopMap = new Dictionary<uint, ShopState>();
        // 添加外部时间控制字段
        private DateTimeOffset? _currentTime;
        private long m_nExteralDiscount = 1000L;
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
            InitShopSystem();
            base.OnCreate();
        }
        private Dictionary<uint, long> m_ShopBuyDiscount = new Dictionary<uint, long>();
        private Dictionary<uint, long> m_ShopSellDiscount = new Dictionary<uint, long>();
        private void InitShopSystem()
        {
            shopMap.Clear();

            // 1. 最外层循环：遍历所有商店配置
            foreach (Shop.Data.Shop shopCfg in ShopStaticData.shops.Items)
            {
                // --- 预处理商店层级的规则和折扣 ---
                List<ShopRule> shopAllRules = null;
                if (shopCfg.Rules != null)
                {
                    shopAllRules = shopCfg.Rules
                    .Select(id => ShopStaticData.shopRules.GetItem(id))
                    .Where(r => r != null)
                    .ToList();
                }
                else
                {
                    shopAllRules = new List<ShopRule>();
                }


                var shopBuyRules = shopAllRules.Where(r => r.RuleTyp == SHOP_RULE_TYPE.Buy).ToList();
                var shopSellRules = shopAllRules.Where(r => r.RuleTyp == SHOP_RULE_TYPE.Sell).ToList();

                // 创建ShopState实例
                ShopState shopState = new ShopState
                {
                    ShopID = shopCfg.ID,
                };

                // 2. 第二层循环：遍历所有货架配置
                foreach (Shelf shelfCfg in ShopStaticData.shelfs.Items)
                {
                    // --- 预处理货架层级的规则 ---
                    List<ShopRule> shelfAllRules = null;
                    if (shelfCfg.Rules != null)
                    {
                        shelfAllRules = shelfCfg.Rules
                        .Select(id => ShopStaticData.shopRules.GetItem(id))
                        .Where(r => r != null)
                        .ToList();
                    }
                    else
                    {
                        shelfAllRules = new List<ShopRule>();
                    }

                    var shelfBuyRules = shelfAllRules.Where(r => r.RuleTyp == SHOP_RULE_TYPE.Buy).ToList();
                    var shelfSellRules = shelfAllRules.Where(r => r.RuleTyp == SHOP_RULE_TYPE.Sell).ToList();

                    ShelfState shelfState = null;
                    if (shelfCfg.ShopId == shopCfg.ID)
                    {
                        // 创建ShelfState实例
                        shelfState = new ShelfState
                        {
                            ShelfId = shelfCfg.ID,
                            RefreshPeriod = shelfCfg.RefreshTime, // 复制刷新周期字符串
                            Left = -1 // 货架层级限制，可根据需求扩展
                        };
                        UpdateShelfRefreshTime(shelfState); // 初始化首次刷新时间

                    }
                    // 3. 第三层循环：遍历所有商品配置
                    foreach (Good goodCfg in ShopStaticData.goodDatas.Items)
                    {
                        //if (goodCfg.ShelfId != shelfCfg.ID) continue;

                        OpenNGS.Item.Data.Item itemInfo = ItemStaticData.items.GetItem(goodCfg.ItemId);
                        if (itemInfo == null) continue;

                        // --- 核心规则判断 ---
                        if(shelfState != null)
                        {
                            long shopDiscount = CONST_RATE;
                            long shelfDiscount = CONST_RATE;
                            if (shopBuyRules != null && shopBuyRules.Count > 0 )
                            {
                                ShopRule _shopBuyRule = CheckGoodEligibility(goodCfg, shopBuyRules);
                                if (_shopBuyRule != null)
                                {
                                    shopDiscount = _shopBuyRule.Discount;
                                }
                                else
                                {
                                }
                            }
                            if (shelfBuyRules != null && shelfBuyRules.Count > 0)
                            {
                                ShopRule _shelfSellRule = CheckGoodEligibility(goodCfg, shelfBuyRules);
                                if (_shelfSellRule != null)
                                {
                                    shelfDiscount = _shelfSellRule.Discount;
                                }
                                else
                                {
                                }
                            }
                            if (goodCfg.ShelfId == shelfCfg.ID)
                            {
                                // 如果可购买，则创建GoodState并加入货架
                                GoodState goodState = new GoodState
                                {
                                    GoodID = goodCfg.ID,
                                    Left = goodCfg.Limit > 0 ? (int)goodCfg.Limit : -1,
                                    Price = (uint)((goodCfg.Price * shopDiscount) / CONST_RATE * shelfDiscount / CONST_RATE * (m_nExteralDiscount / CONST_RATE))
                                };
                                shelfState.Goods.Add(goodState);
                            }
                        }
                        // b) 在可购买的基础上，判断商品是否可贩卖 (必须同时满足商店和货架的Sell规则)
                        ShopRule _shopSellRule = CheckGoodEligibility(goodCfg, shopSellRules);

                        if (_shopSellRule != null)
                        {
                            ShopSellItem _sellItem = new ShopSellItem();
                            _sellItem.ItemID = goodCfg.ItemId;
                            _sellItem.GoodID = goodCfg.ID;
                            _sellItem.Price = (uint)((goodCfg.Price * _shopSellRule.Discount) / CONST_RATE ) ;
                            // 如果也可贩卖，将其ID加入商店的可贩卖列表
                            shopState.SellItems.Add(_sellItem);
                        }
                    }

                    // 如果该货架上有商品，则将该货架状态加入商店状态
                    if(shelfState != null)
                    {
                        if (shelfState.Goods.Any())
                        {
                            shopState.Shelves.Add(shelfState);
                        }
                    }
                }

                // 如果该商店下有任何货架（且货架上有商品），则将该商店的最终状态存入map
                if (shopState.Shelves.Any())
                {
                    shopMap[shopCfg.ID] = shopState;
                }
            }
        }

        private ShopRule CheckGoodEligibility(Good goodCfg, List<ShopRule> rules)
        {
            // 如果没有提供任何相关类型的规则，则视为不满足条件
            if (rules == null || !rules.Any())
            {
                return null;
            }
            OpenNGS.Item.Data.Item itemInfo = ItemStaticData.items.GetItem(goodCfg.ItemId);

            // 只要满足规则列表中的【任意一条】规则即可
            foreach (var rule in rules)
            {
                // 检查物品类型：-1代表通用，否则必须匹配
                bool typeMatch = rule.ItemType == -1 || rule.ItemType == (int)itemInfo.ItemType;
                // 检查商品ID：列表为空代表通用，否则必须包含
                bool goodIdMatch = rule.Goods == null || rule.Goods.Length == 0 || rule.Goods.Contains(goodCfg.ID);

                if (typeMatch && goodIdMatch)
                {
                    return rule; // 满足一条即可
                }
            }
            return null;
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

        public Shop.Service.BuyRsp BuyItem(Shop.Service.BuyReq request)
        {
            long nFinalPrice = GetFinalBuyPrice(request.ShopId, request.GoodId);
            BuyRsp _buyRsp = new BuyRsp { result = ShopResultType.Success };

            if (!shopMap.TryGetValue(request.ShopId, out ShopState _shopState))
            {
                _buyRsp.result = ShopResultType.Failed_InvalidShop;
                return _buyRsp;
            }

            ShelfState shelf = _shopState.Shelves.Find(item => item.ShelfId == request.ShelfId);
            if (shelf == null)
            {
                _buyRsp.result = ShopResultType.Failed_InvalidShelf;
                return _buyRsp;
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

            GoodState goodState = shelf.Goods.Find(item => item.GoodID == request.GoodId);
            if (goodState == null)
            {
                _buyRsp.result = ShopResultType.Failed_InvalidGood;
                return _buyRsp;
            }

            if (goodState.Left == 0 || (goodState.Left > 0 && request.GoodCounts > goodState.Left))
            {
                _buyRsp.result = ShopResultType.Failed_BuyOverLimit;
                return _buyRsp;
            }

            Good _good = ShopStaticData.goodDatas.GetItem(request.GoodId);
            if (_good == null)
            {
                _buyRsp.result = Shop.Common.ShopResultType.Failed_InvalidGood;
            }
            else
            {
                OpenNGS.Item.Data.Item _addItemInf = ItemStaticData.items.GetItem(_good.ItemId);
                OpenNGS.Item.Data.Item _removeItemInf = ItemStaticData.items.GetItem(_good.NeedItemID);
                if (_addItemInf == null || _removeItemInf == null)
                {
                    _buyRsp.result = ShopResultType.Failed_InvalidShopStatic;
                }
                else
                {
                    int nAddCol = m_itemSys.GetColByItemTyp((uint)_addItemInf.ItemType);
                    if(nAddCol >= 0)
                    {
                        int _colRemoved = m_itemSys.GetColByItemTyp((uint)_removeItemInf.ItemType);
                        if(_colRemoved >= 0)
                        {
                            _buyRsp.result = DoExchange((uint)_colRemoved, _good.NeedItemID, (uint)nFinalPrice * request.GoodCounts
                                , (uint)nAddCol, _good.ItemId, _good.ItemNum * request.GoodCounts);

                            if (_buyRsp.result == ShopResultType.Success)
                            {
                                if (goodState.Left > 0)
                                {
                                    goodState.Left -= 1;
                                }
                            }
                        }
                        else
                        {
                            _buyRsp.result = ShopResultType.Failed_InvalidShopStatic;
                        }
                    }
                    else
                    {
                        _buyRsp.result = ShopResultType.Failed_InvalidShopStatic;
                    }
                }
            }

            return _buyRsp;
        }
        /// <summary>
        /// 执行购买逻辑
        /// </summary>
        private ShopResultType DoExchange(uint nCol, uint nRemoveItemID, uint nRemoveCount, uint nAddCol, uint nAddItemID, uint nAddCounts)
        {
            ShopResultType _shopResult = ShopResultType.Success;
            // source是 移除
            ExchangeByItemIDReq _exchangeReq = new ExchangeByItemIDReq();
            ItemSrcState src = new ItemSrcState();
            src.Col = nCol;
            src.ItemID = nRemoveItemID;
            src.Counts = nRemoveCount;
            _exchangeReq.Source.Add(src);

            // target是 添加
            TargetState trg = new TargetState();
            trg.Col = nAddCol;
            trg.ItemID = nAddItemID;
            trg.Counts = nAddCounts;
            _exchangeReq.Target.Add(trg);
            ExchangeRsp exchangeRsp = null;
            (_shopResult, exchangeRsp) = _procExchange(_exchangeReq);
            return _shopResult;
        }

        public SellRsp SellItem(SellReq _req)
        {
            SellRsp _sellRsp = new SellRsp { Result = ShopResultType.Success };

            ItemSaveState _itemState = m_itemSys.GetItemStateByGUID(_req.GUID);
            if (_itemState == null)
            {
                _sellRsp.Result = ShopResultType.Failed_ItemNotFound;
            }

            if (!shopMap.TryGetValue(_req.ShopID, out ShopState _shopState))
            {
                _sellRsp.Result = ShopResultType.Failed_InvalidShop;
                return _sellRsp;
            }

            if (_shopState.SellItems == null)
            {
                _sellRsp.Result = ShopResultType.Failed_ShopNotSupportSell;
                return _sellRsp;
            }
            (long nFinalPrice, Good _goodSell) = GetFinalSellPrice(_req.ShopID, _itemState.ItemID);
            if (nFinalPrice > 0)
            {
                uint nCounts = _itemState.Count;
                if (_req.Counts > nCounts)
                {
                    _sellRsp.Result = ShopResultType.Failed_NotEnough_Good;
                }
                else
                {
                    ExchangeResultType _resExchange = _DoSellExchange(_itemState, _req.Counts, _goodSell.NeedItemID, (uint)nFinalPrice * _req.Counts);

                    _sellRsp.Result = _convertExchangeResultToShopResult(_resExchange);
                }
            }
            else
            {
                _sellRsp.Result = ShopResultType.Failed_ShopNotSupportSell;
            }
            return _sellRsp;
        }

        private ExchangeResultType _DoSellExchange(ItemSaveState _itemState, uint nCounts, uint nItemID, uint nItemCounts)
        {
            ExchangeByGridIDReq _exchangeReq = new ExchangeByGridIDReq();
            GridSrcState src = new GridSrcState();
            src.Col = _itemState.ColIdx;
            src.Grid = _itemState.Grid;
            src.Counts = nCounts;
            _exchangeReq.Source.Add(src);

            TargetState trg = new TargetState();
            Item.Data.Item _itemInf = ItemStaticData.items.GetItem(nItemID);
            int nCol = m_itemSys.GetColByItemTyp((uint)_itemInf.ItemType);
            if (nCol < 0)
            {
                return ExchangeResultType.Error_NotExist_Source;
            }
            else
            {
                trg.Col = (uint)nCol;
                trg.ItemID = nItemID;
                trg.Counts = nItemCounts;
                _exchangeReq.Target.Add(trg);
                ExchangeRsp exchangeRsp = m_exchangeSys.ExchangeItemByGrid(_exchangeReq);
                return exchangeRsp.result;
            }
        }
        private ShopResultType _convertExchangeResultToShopResult(ExchangeResultType _exRes)
        {
            ShopResultType _shopResult = ShopResultType.Success;
            if (_exRes == ExchangeResultType.Error_NotDefine_Target)
            {
                _shopResult = ShopResultType.Failed_InvalidAdded;
            }
            if (_exRes == ExchangeResultType.Error_NotExist_Source)
            {
                _shopResult = ShopResultType.Failed_InvalidRemoved;
            }
            else if (_exRes == ExchangeResultType.Failed_OverLimitNum)
            {
                _shopResult = ShopResultType.Failed_ItemOverLimit;
            }
            else if (_exRes == ExchangeResultType.Failed_NotEnough)
            {
                _shopResult = ShopResultType.Failed_NotEnough_Gold;
            }
            return _shopResult;
        }
        private (ShopResultType, ExchangeRsp) _procExchange(ExchangeByItemIDReq _exchangeReq)
        {
            ShopResultType _shopResult = ShopResultType.Success;
            ExchangeRsp exchangeRsp = m_exchangeSys.ExchangeItemByID(_exchangeReq);
            _shopResult = _convertExchangeResultToShopResult(exchangeRsp.result);
            return (_shopResult, exchangeRsp);
        }

        public ShopRsp GetShopState(ShopReq request)
        {
            ShopRsp _response = new ShopRsp();
            OpenNGS.Shop.Data.Shop _shop = ShopStaticData.shops.GetItem(request.ShopId);
            if (_shop == null)
            {
                _response.result = ShopResultType.Failed_InvalidShopStatic;
                return _response;
            }

            if (shopMap.TryGetValue(request.ShopId, out ShopState _shopState))
            {
                _response.Shelfs.AddRange(_shopState.Shelves);
                _response.result = ShopResultType.Success;
            }
            else
            {
                _response.result = Shop.Common.ShopResultType.Failed_InvalidShop;
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
                foreach (var shelf in shop.Shelves)
                {
                    CheckAndUpdateShelfRefresh(shelf);
                }
            }

            // 查找商品逻辑（保持不变）
            foreach (var shopPair in shopMap.Values)
            {
                var shelfState = shopPair.Shelves.Find(s => s.ShelfId == shelfId);
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
        public long GetFinalBuyPrice(uint shopId, uint goodId)
        {
            // 1. 查找商店状态
            if (!shopMap.TryGetValue(shopId, out ShopState shopState))
            {
                // NgDebug.LogError($"Shop not found: {shopId}");
                return -1; // 商店不存在
            }

            // 2. 查找商品的基准价格
            Shop.Data.Good good = ShopStaticData.goodDatas.GetItem(goodId);
            if (good == null)
            {
                // NgDebug.LogError($"Good not found: {goodId}");
                return -1; // 商品静态数据不存在
            }

            // 3. 校验该商品是否在该商店中可购买
            // 遍历商店的所有货架，看是否有任何一个货架包含此商品
            foreach(ShelfState _state in shopState.Shelves)
            {
                foreach(GoodState _goodState in _state.Goods)
                {
                    if(_goodState.GoodID == goodId)
                    {
                        return _goodState.Price;
                    }
                }
            }
            return 0;
        }

        public void SetExteralDiscount(uint nExternal)
        {
            m_nExteralDiscount = (long)nExternal;
        }

        /// <summary>
        /// 计算向指定商店出售一个商品的最终价格。
        /// </summary>
        /// <param name="shopId">商店的ID</param>
        /// <param name="nItemID">要出售的商品的ID</param>
        /// <returns>计算后的最终出售价格。如果商店不存在、商品不存在或该商品在此商店不可出售，则返回 -1。</returns>
        public (long, Good) GetFinalSellPrice(uint shopId, uint nItemID)
        {
            // 1. 查找商店状态
            if (!shopMap.TryGetValue(shopId, out ShopState _shopState))
            {
                // NgDebug.LogError($"Shop not found: {shopId}");
                return (-1, null); // 商店不存在
            }

            if (_shopState.SellItems == null)
            {
                return (-1, null);
            }

            foreach(ShopSellItem _SellItem in _shopState.SellItems)
            {
                if(_SellItem.ItemID == nItemID)
                {
                    Good _cfg = ShopStaticData.goodDatas.GetItem(_SellItem.GoodID);
                    if(_cfg != null)
                    {
                        return (_SellItem.Price, _cfg);
                    }
                }
            }
            return (-1, null);
        }
    }
}