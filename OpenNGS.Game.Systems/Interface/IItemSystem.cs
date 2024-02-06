using OpenNGS.Suit.Data;
using OpenNGS.Item.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.Systems
{
    public interface IItemSystem
    {
        public bool IsEnoughByItemID(uint nItemID, uint nCounts);
        public bool IsEnoughByGuid(uint nGuid, uint nCounts);

        public bool AddItemsByID(uint nItemID, uint nCounts);
        public bool RemoveItemsByID(uint nItemID, uint nCounts);
        public bool RemoveItemsByGuid(uint nGuid, uint nCounts);
        public bool UseItem(uint nGuid);

        public uint GetGuidByItemID(uint nItemID);
        public uint GetItemCountByGuidID(uint nGuid);
        public OpenNGS.Item.Common.ItemData GetItemDataByGuid(ulong nGuid);
        public OpenNGS.Item.Common.ItemData GetItemDataByItemId(uint itemId);
        public List<OpenNGS.Item.Common.ItemData> GetItemInfos(OpenNGS.Item.Common.ITEM_TYPE iTEM_TYPE);
        public SuitData GetSuitData(uint suitID);
        public MakeDesign GetItemByItmes(uint nItemId);
    }

}