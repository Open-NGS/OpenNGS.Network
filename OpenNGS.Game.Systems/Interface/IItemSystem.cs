using OpenNGS.Suit.Data;
using OpenNGS.Item.Data;
using System.Collections;
using System.Collections.Generic;
using System;
using Dynamic.Data;
using OpenNGS.Dialog.Data;

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
        public uint GetItemTotalCountByItemID(uint itemID);
        public OpenNGS.Item.Common.ItemData GetItemDataByGuid(ulong nGuid);
        public List<OpenNGS.Item.Common.ItemData> GetItemDataByItemId(uint itemId);
        //获取某种类型所有道具(二级分类)
        public List<OpenNGS.Item.Common.ItemData> GetItemInfos(OpenNGS.Item.Common.ITEM_TYPE iTEM_TYPE);
        //获取某种类型所有道具(三级分类)
        public List<OpenNGS.Item.Common.ItemData> GetItemInfoByKind(OpenNGS.Item.Common.ITEM_KIND iTEM_KIND);
        //获取背包中的所有道具
        public List<OpenNGS.Item.Common.ItemData> GetItemInfosInBag();
        //获取仓库中的所有道具
        public List<OpenNGS.Item.Common.ItemData> GetItemInfosInStash();
        public SuitData GetSuitData(uint suitID);
        public MakeDesign GetItemByItmes(uint itemId);
        public DisassembleEquipIno GetDisassembleEquipIno(uint itemId);
        //穿装备
        public OpenNGS.Item.Common.EQUIP_RESULT_TYPE Equipped(uint index, uint nGuid);
        //脱装备
        public OpenNGS.Item.Common.EQUIP_RESULT_TYPE Unequipped(uint index);

        public void ItemInBag(uint nGuid);
        public void ItemOutBag(uint nGuid);
        public List<equips> GetEquippedList();

        public void AddAction_stashChange(Action<uint, OpenNGS.Item.Common.ItemData> ac);
        //传入道具栏变更的事件
        public void AddAction_bagChange(Action<uint ,OpenNGS.Item.Common.ItemData> ac);
        //传入装备栏变更的事件
        public void AddAction_equipChange(Action<uint, OpenNGS.Item.Common.ItemData> ac);
        public void RemoveAction_stashChange(Action<uint, OpenNGS.Item.Common.ItemData> ac);
        //删去道具栏变更事件
        public void RemoveAction_bagChange(Action<uint, OpenNGS.Item.Common.ItemData> ac);
        //删去装备栏变更事件
        public void RemoveAction_equipChange(Action<uint, OpenNGS.Item.Common.ItemData> ac);
        void AddItemContainer(ItemContainer Container);
    }

}