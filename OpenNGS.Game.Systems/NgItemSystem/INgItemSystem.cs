using OpenNGS.Suit.Data;
using OpenNGS.Item.Data;
using System.Collections;
using System.Collections.Generic;
using OpenNGS.Item.Service;
using OpenNGS.Item.Common;

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

        public ItemResultType CanRemoveItem(RemoveReq _req);
        public ItemResultType CanAddItem(AddReq _req);
        public AddItemRsp AddItems(AddReq _req);
        public AddItemRsp RemoveItems(RemoveReq _req);
    }

}