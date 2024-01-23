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
    public class ExchangeSystem : EntitySystem
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
            EXCHANGE_RESULT_TYPE result;
            if(target == null || target.Count == 0) { return EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOTARGET; }
            
            if (src == null || src.Count == 0)
            {
                result = EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_SUCCESS;
            }
            else
            {
                bool res = CheckItemCondition(src);
                if(res) 
                    result = EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_SUCCESS;
                else 
                    return EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOCOUNT;
            }
            SendRemovetem2Bag(src);
            SendAddItem2Bag(target);
            return result;
        }

        private bool CheckItemCondition(List<SourceItem> items)
        {
            //去背包里查找src里面的道具是否满足条件
            foreach(SourceItem item in items)
            {
                if(m_itemSys.IsItemEnough(item.GUID, item.Count))
                {
                    return false;
                }
            }
            return true;
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
                m_itemSys.RemoveItemsByGuid(item.GUID, item.Count);
            }
            
        }

    }
}
