using OpenNGS.Suit.Data;
using OpenNGS.Item.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenNGS.Item.Common;
using UnityEngine.Events;

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
        public List<OpenNGS.Item.Common.ItemData> GetItemDataByItemId(uint itemId);
        //获取某种类型所有道具(二级分类)
        public List<OpenNGS.Item.Common.ItemData> GetItemInfos(OpenNGS.Item.Common.ITEM_TYPE iTEM_TYPE);
        //获取背包中的所有道具
        public List<OpenNGS.Item.Common.ItemData> GetItemInfosInBag();
        public SuitData GetSuitData(uint suitID);
        public MakeDesign GetItemByItmes(uint itemId);
        public DisassembleEquipIno GetDisassembleEquipIno(uint itemId);
        //穿装备
        public OpenNGS.Item.Common.EQUIP_RESULT_TYPE Equipped(uint index, uint nGuid);
        //脱装备
        public OpenNGS.Item.Common.EQUIP_RESULT_TYPE Unequipped(uint index);
        public Dictionary<uint, uint> GetEquippedList();
    }

}