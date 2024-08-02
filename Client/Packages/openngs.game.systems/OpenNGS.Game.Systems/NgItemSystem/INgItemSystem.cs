using OpenNGS.Item.Data;
using System.Collections.Generic;
using OpenNGS.Item.Service;
using OpenNGS.Item.Common;

namespace OpenNGS.Systems
{
    public interface INgItemSystem
    {
        public AddItemRsp AddItemByID(AddItemReq _req);
        public AddItemRsp RemoveItemByGrid(RemoveItemReq _req);
        public AddItemRsp ExchangeGrid(ChangeItemData _changeItemData);
        public AddItemRsp SortItems(uint nCol);
        public ItemColumn GetItemColumnByColIdx(uint nColIdx);
        void AddItemContainer(ItemContainer Container);

        //交易系统需要
        public ItemResultType CanAddItems(AddReq _req);
        public AddItemRsp AddItems(AddReq _req);
        public ItemResultType CanRemoveItemsByID(RemoveItemsByIDsReq _req);
        public AddItemRsp RemoveItemsByID(RemoveItemsByIDsReq _req);
        public ItemResultType CanRemoveItemsByGrid(RemoveItemsByGridsReq _req);
        public AddItemRsp RemoveItemsByGrid(RemoveItemsByGridsReq _req);
        public uint GetCurrencyColById(uint itemID);
    }

}