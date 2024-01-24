using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNGS.Collections.Generic;
using OpenNGS.Exchange.Data;
using OpenNGS.Exchange.Common;
using static UnityEngine.GraphicsBuffer;

namespace OpenNGS.Systems
{
    public class ExchangeSystem : EntitySystem, IExchangeSystem
    {
        private IItemSystem m_itemSys = null;
        protected override void OnCreate()
        {
            base.OnCreate();
        }
        public void RegisteItemSystem(IItemSystem _itemSys)
        {
            m_itemSys = _itemSys;
        }

        public override string GetSystemName()
        {
            return "com.openngs.system.rank";
        }

        public EXCHANGE_RESULT_TYPE ExchangeItem(List<SourceItem> src, List<TargetItem> target)
        {
            EXCHANGE_RESULT_TYPE result = EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_SUCCESS;

            switch (CheckItemCondition(src))
            {
                case EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOCOUNT:
                    return EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOCOUNT;
                case EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_ERROR_ITEM:
                    return EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_ERROR_ITEM;
            }             
            SendRemovetem2Bag(src);
            SendAddItem2Bag(target);
            return result;
        }

        private EXCHANGE_RESULT_TYPE CheckItemCondition(List<SourceItem> items)
        {
            if(items.Count == 0 || items == null) return EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_SUCCESS;

            //去背包里查找src里面的道具是否满足条件
            foreach(SourceItem item in items)
            {
                if (item.ItemID == 0 && item.GUID == 0) return EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_ERROR_ITEM;
                if (item.ItemID == 0)
                {
                    if(!m_itemSys.IsEnoughByGuid(item.GUID, item.Count))
                    {
                        return EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOCOUNT;
                    }
                }
                else if(item.GUID == 0)
                {
                    if (!m_itemSys.IsEnoughByItemID(item.ItemID, item.Count))
                    {
                        return EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOCOUNT;
                    }
                }
            }
            return EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_SUCCESS;
        }

        private void SendAddItem2Bag(List<TargetItem> items)
        {
            if(items == null || items.Count == 0) return;
            //给背包发送要增加的道具            
            foreach(TargetItem item in items)
            {
                m_itemSys.AddItemsByID(item.ItemID, item.Count);
            }

        }

        private void SendRemovetem2Bag(List<SourceItem> items)
        {
            if (items == null || items.Count == 0) return;
            //给背包发送要删除的道具
            foreach(SourceItem item in items)
            {
                if (item.ItemID == 0)
                {
                    m_itemSys.RemoveItemsByGuid(item.GUID, item.Count);
                }
                else if (item.GUID == 0)
                {
                    m_itemSys.RemoveItemsByID(item.ItemID, item.Count);
                }
            }
            
        }

    }
}
