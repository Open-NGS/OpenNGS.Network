using System.Collections.Generic;
using OpenNGS.Exchange.Data;
using OpenNGS.Exchange.Common;
using Systems;
using OpenNGS.Item.Service;
using OpenNGS.Item.Data;

namespace OpenNGS.Systems
{ 
    public class NgExchangeSystem : GameSubSystem<NgExchangeSystem>, INgExchangeSystem
    {
        private INgItemSystem m_NgItemSys = null;

        protected override void OnCreate()
        {
            m_NgItemSys = App.GetService<INgItemSystem>();
            base.OnCreate();
        }

        public override string GetSystemName()
        {
            return "NgExchange";
        }

        public ExchangeRsp ExchangeItem(ExchangeReq request)
        {
            ExchangeResultType _resultType = ExchangeResultType.Success;
            ExchangeRsp response = new ExchangeRsp();

            //检查来源物体是否满足条件
            RemoveReq _removeReq = new RemoveReq();
            foreach(SourceState src in request.src)
            {
                RemoveItemReq _req = new RemoveItemReq();
                _req.ColIdx = src.Col;
                _req.Grid = src.Grid;
                _req.Counts = src.Counts;
                _removeReq.RemoveList.Add(_req);
            }

            if(_removeReq.RemoveList.Count > 0)
            {
                switch (m_NgItemSys.CanRemoveItem(_removeReq))
                {
                    case Item.Common.ItemResultType.ItemResultType_RemoveItemFail_GridNotExist:
                        _resultType = ExchangeResultType.Error_NotExist_Source;
                        break;
                    case Item.Common.ItemResultType.ItemResultType_SortItemFail_NotExist:
                        _resultType = ExchangeResultType.Error_NotDefine_Target;
                        break;
                    case Item.Common.ItemResultType.ItemResultType_RemoveItemFail_NotEnoughNum:
                        _resultType = ExchangeResultType.Failed_NotEnough;
                        break;
                }
            }

            if(_resultType != ExchangeResultType.Success)
            {
                response.result = _resultType;
                return response;
            }

            //检查目标物体是否可以添加

            AddReq _addReq = new AddReq();
            foreach (TargetState trg in request.target)
            {
                AddItemReq _req = new AddItemReq();
                _req.ColIdx = trg.Col;
                _req.ItemID = trg.ItemID;
                _req.Counts = trg.Counts;
                _addReq.AddList.Add(_req);
            }

            if(_addReq.AddList.Count > 0)
            {
                switch (m_NgItemSys.CanAddItem(_addReq))
                {
                    case Item.Common.ItemResultType.ItemResultType_AddItemFail_NotEnoughGrid:
                        _resultType = ExchangeResultType.Failed_OverLimitNum;
                        break;
                    case Item.Common.ItemResultType.ItemResultType_AddItemFail_NotExist:
                        _resultType = ExchangeResultType.Error_NotDefine_Target;
                        break;
                }
            }

            if (_resultType != ExchangeResultType.Success)
            {
                response.result = _resultType;
                return response;
            }

            //执行交换逻辑
            response.result = _resultType;
            AddItemRsp _addRsp = null;
            if (_removeReq.RemoveList.Count > 0)
            {
                m_NgItemSys.RemoveItems(_removeReq);
            }
            if(_addReq.AddList.Count > 0)
            {
                _addRsp = m_NgItemSys.AddItems(_addReq);
            }

            if(_addRsp != null)
            {
                if(_addRsp.Result != null)
                {
                    foreach(ItemSaveState item in _addRsp.Result.ItemList)
                    {
                        response.LstItemData.Add(item);
                    }
                }
            }

            return response;
        }
    }
}

