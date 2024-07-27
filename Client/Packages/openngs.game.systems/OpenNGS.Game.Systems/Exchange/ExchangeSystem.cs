using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNGS.Collections.Generic;
using OpenNGS.Exchange.Data;
using OpenNGS.Exchange.Common;
using Systems;
using OpenNGS.Item.Data;

namespace OpenNGS.Systems
{
    public class ExchangeSystem : GameSubSystem<ExchangeSystem>, IExchangeSystem
    {
        private IItemSystem m_itemSys = null;
        private int aaa = 100;
        protected override void OnCreate()
        {
            m_itemSys = App.GetService<IItemSystem>();
            base.OnCreate();
        }


        public override string GetSystemName()
        {
            return "com.openngs.system.exchange";
        }

        public EXCHANGE_RESULT_TYPE ExchangeItem(List<SourceItem> src, List<TargetItem> target, List<ItemSaveData> LstItemData = null)
        {
            EXCHANGE_RESULT_TYPE result = EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_SUCCESS;

            switch (CheckItemCondition(src))
            {
                case EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOENOUGH:
                    return EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOENOUGH;
                case EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOITEM:
                    return EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOITEM;
            }             
            SendRemoveItem2Bag(src);
            SendAddItem2Bag(target, LstItemData);
            return result;
        }

        private EXCHANGE_RESULT_TYPE CheckItemCondition(List<SourceItem> items)
        {
            EXCHANGE_RESULT_TYPE res = EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_SUCCESS;
            if (items.Count == 0 || items == null) return res;

            
            //去背包里查找src里面的道具是否满足条件
            foreach (SourceItem item in items)
            {
                if (item.GUID != 0)
                {
                    if(!m_itemSys.IsEnoughByGuid(item.GUID, item.Count))
                    {
                        res = EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOENOUGH;
                    }
                }
                else
                {
                    res = EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOITEM;
                }
            }
            return res;
        }

        private void SendAddItem2Bag(List<TargetItem> items, List<ItemSaveData> LstItemData)
        {
            if(items == null || items.Count == 0) return;
            //给背包发送要增加的道具            
            foreach(TargetItem item in items)
            {
                ItemSaveData _itemData = m_itemSys.AddItemsByID(item.ItemID, item.Count);
                if(LstItemData != null)
                {
                    LstItemData.Add(_itemData);
                }
            }
        }

        private void SendRemoveItem2Bag(List<SourceItem> items)
        {
            if (items == null || items.Count == 0) return;
            //给背包发送要删除的道具
            foreach(SourceItem item in items)
            {
                if (item.GUID != 0)
                {
                    m_itemSys.RemoveItemsByGuid(item.GUID, item.Count);
                }
            }
            
        }

        public void Test()
        {
            int aa = 0;
        }
    }
}
