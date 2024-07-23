using OpenNGS.Suit.Data;
using OpenNGS.Item.Data;
using System.Collections;
using System.Collections.Generic;
using System;
using Dynamic.Data;
using OpenNGS.Dialog.Data;
using OpenNGS.Item.Service;

namespace OpenNGS.Systems
{
    public interface INgItemSystem
    {
        public AddItemRsp AddItemsByID(AddItemReq _req);
        public AddItemRsp RemoveItemsByGrid(RemoveItemReq _req);
        //public uint GetItemCountByGuid(uint nColIdx, uint nGuid);
        public AddItemRsp ExchangeGrid(ChangeItemData _changeItemData);
        public AddItemRsp SortItems(uint nCol);
        public List<ItemSaveState> GetItemDatasByColIdx(uint nColIdx);
        void AddItemContainer(ItemContainer Container);
    }

}