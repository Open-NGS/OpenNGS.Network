using OpenNGS.Suit.Data;
using OpenNGS.Item.Data;
using System.Collections;
using System.Collections.Generic;
using System;
using Dynamic.Data;
using OpenNGS.Dialog.Data;

namespace OpenNGS.Systems
{
    public interface INgItemSystem
    {
        public ItemResult AddItemsByID(uint nItemID, uint nCounts, uint nColIdx);
        public ItemResult RemoveItemsByGrid(uint nColIdx, uint nGrid, uint nCounts);
        //public uint GetItemCountByGuid(uint nColIdx, uint nGuid);
        public ItemResult ExchangeGrid(uint nSrcCol, uint nSrcGrid, uint nDstCol, uint nDstGrid);
        public void SortItems(uint nCol);
        public List<ItemSaveState> GetItemDatasByColIdx(uint nColIdx);
        void AddItemContainer(ItemContainer Container);
    }

}