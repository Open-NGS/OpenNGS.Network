using OpenNGS.Item.Common;
using OpenNGS.Item.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Systems;


namespace OpenNGS.Systems
{
    public class NgItemSystem : GameSubSystem<NgItemSystem>, INgItemSystem
    {
        private bool m_IsCreate;
        private ulong m_Uin;
        private bool m_IsNewPlayer;

        private ItemContainer itemContainer = null;

        private uint guid_cache = 1;
        private Queue<uint> guid_free = new Queue<uint>();

        public void Init(ulong uin, bool isNewPlayer)
        {
            m_Uin = uin;
            m_IsNewPlayer = isNewPlayer;
        }
        protected override void OnCreate()
        {
            base.OnCreate();
            m_IsCreate = false;
        }
        public override string GetSystemName()
        {
            return "NgItemSystem";
        }
        public void AddItemContainer(ItemContainer Container)
        {
            if (Container != null)
            {
                itemContainer = Container;
            }
            else
            {
                itemContainer = new ItemContainer();
            }
        }
        protected override void OnClear()
        {
            itemContainer = null;
            base.OnClear();
        }
        private bool IsEnoughByGuid(uint nGuid, uint nCounts, uint nColIdx)
        {
            ItemSaveState itemSaveState = itemContainer.Col[(int)nColIdx].ItemSaveStates[(int)nGuid];
            if (itemSaveState == null)
            {
                return false;
            }
            int num = (int)(nCounts + itemSaveState.Count);
            bool bRes = itemSaveState.Count >= num;
            return bRes;
        }
        private List<ItemSaveState> GetItemByItemId(uint nColIdx,uint itemId)
        {
            List<ItemSaveState> itemDatas = new List<ItemSaveState>();
            foreach (var item in itemContainer.Col[(int)nColIdx].ItemSaveStates)
            {
                if (item.ItemID == itemId)
                {
                    ItemSaveState itemData = new ItemSaveState();
                    itemData.GUID = item.GUID;
                    itemData.ItemID = item.ItemID;
                    itemData.Count = item.Count;
                    itemDatas.Add(itemData);
                }
            }
            return itemDatas;
        }
        private void UpdateItemInContainer(uint nColIdx, ItemSaveState item)
        {
            var column = GetItemByColIdx(nColIdx);
            if (column != null)
            {
                var itemState = column.ItemSaveStates.FirstOrDefault(i => i.GUID == item.GUID);
                if (itemState != null)
                {
                    itemState.Count = item.Count;
                }
            }
        }
        private bool ItemExistsInContainer(uint nColIdx, uint guid)
        {
            var column = GetItemByColIdx(nColIdx);
            if (column != null)
            {
                return column.ItemSaveStates.Any(i => i.GUID == guid);
            }
            return false;
        }
        private ItemColumn GetItemByColIdx(uint nColIdx)
        {
            return itemContainer.Col.FirstOrDefault(c => c.ColIdx == nColIdx);
        }
        public ItemResult AddItemsByID(uint nItemID, uint nCounts,uint nColIdx)
        {
            ItemResult result = new ItemResult();
            result.ItemList = new List<ItemSaveState>();
            OpenNGS.Item.Data.Item ItemInfo = NGSStaticData.items.GetItem(nItemID);
            if (ItemInfo == null)
            {
                result.ItemResultValue = ItemResultType.ItemResultType_AddItemFail_NotExist;
                return result;
            }
            uint stackMax = ItemInfo.StackMax;
            List<ItemSaveState> itemState = GetItemByItemId(nColIdx,nItemID);
            //已获得过的物品
            if (itemState != null && itemState.Count > 0)
            {
                //先查找背包可堆叠的空位
                for (int i = 0; i < itemState.Count; i++)
                {
                    if (itemState[i].Count < stackMax)
                    {
                        uint volumn = stackMax - itemState[i].Count;
                        //该格子装不下
                        if (nCounts >= volumn)
                        {
                            itemState[i].Count = stackMax;
                            nCounts -= volumn;
                            UpdateItemInContainer(nColIdx, itemState[i]);
                            result.ItemList.Add(itemState[i]);
                            result.ItemResultValue = ItemResultType.ItemResultType_Success;
                        }
                        //该格子能装下
                        else
                        {
                            itemState[i].Count += nCounts;
                            nCounts = 0;
                            UpdateItemInContainer(nColIdx, itemState[i]);
                            result.ItemList.Add(itemState[i]);
                            result.ItemResultValue = ItemResultType.ItemResultType_Success;
                            break;
                        }
                    }
                    if (nCounts <= 0)
                    {
                        break;
                    }
                }
                return result;
            }
            while (nCounts > 0)
            {
                ItemSaveState newItem = new ItemSaveState
                {
                    ItemID = nItemID,
                    Count = Math.Min(nCounts,stackMax),
                    ColIdx = nColIdx
                };

                if (guid_free.Count > 0)
                {
                    newItem.GUID = guid_free.Dequeue();
                }
                else
                {
                    while (ItemExistsInContainer(nColIdx, guid_cache))
                    {
                        guid_cache++;
                    }
                    newItem.GUID = guid_cache;
                }
                nCounts -= newItem.Count;
                // 寻找第一个空格
                bool addedToExistingGrid = false;
                uint FirstEmptyGrid = 0;
                var column = GetItemByColIdx(nColIdx);
                for (uint grid = 0; grid < column.Capacity; grid++)
                {
                    if (!column.ItemSaveStates.Any(i => i.Grid == grid))
                    {
                        //newItem.Grid = 0;
                        FirstEmptyGrid = grid;
                        //result = AddItemToContainer(nColIdx, newItem);
                        //result.ItemList.Add(newItem);
                        addedToExistingGrid = true;
                        break;
                    }
                }
                if (!addedToExistingGrid)
                {
                    result.ItemResultValue = ItemResultType.ItemResultType_AddItemFail_NotEnoughGrid;
                    return result;
                }
                else
                {
                    foreach (var itemSaveState in column.ItemSaveStates)
                    {
                        if (FirstEmptyGrid == itemSaveState.Grid + 1)
                        {
                            itemSaveState.Grid = itemSaveState.Grid + 1;
                            break;
                        }
                        else
                        {
                            itemSaveState.Grid = itemSaveState.Grid + 1;
                        }
                    }
                    newItem.Grid = 0;
                    column.ItemSaveStates.Add(newItem);
                    result.ItemList.Add(newItem);
                    result.ItemResultValue = ItemResultType.ItemResultType_Success;
                    return result;
                }
            }
            //
            var column44 = GetItemByColIdx(nColIdx);
            return result;
        }

        public ItemResult RemoveItemsByGrid(uint nColIdx, uint nGrid, uint nCounts)
        {
            ItemResult result = new ItemResult();
            var column = GetItemByColIdx(nColIdx);
            if (column == null)
            {
                result.ItemResultValue = ItemResultType.ItemResultType_RemoveItemFail_GridNotExist;
                return result;
            }

            var itemState = column.ItemSaveStates.FirstOrDefault(i => i.Grid == nGrid);
            if (itemState == null || itemState.Count < nCounts)
            {
                result.ItemResultValue = ItemResultType.ItemResultType_RemoveItemFail_GridNotExist;
                return result;
            }

            itemState.Count -= nCounts;
            if (itemState.Count == 0)
            {
                column.ItemSaveStates.Remove(itemState);
                guid_free.Enqueue(nGrid);
            }
            result.ItemResultValue = ItemResultType.ItemResultType_Success;
            result.ItemList.Add(itemState);
            return result;
        }

        public uint GetItemCountByGuid(uint nColIdx, uint nGuid)
        {
            var column = GetItemByColIdx(nColIdx);
            if (column != null)
            {
                var itemState = column.ItemSaveStates.FirstOrDefault(i => i.GUID == nGuid);
                if (itemState != null)
                {
                    return itemState.Count;
                }
            }
            return 0;
        }

        public ItemResult ExchangeGrid(uint nSrcCol, uint nSrcGrid, uint nDstCol, uint nDstGrid)
        {
            ItemResult result = new ItemResult();
            result.ItemList = new List<ItemSaveState>();
            var itemStateSrc = GetItemDatasByColIdx(nSrcCol).FirstOrDefault(i => i.Grid == nSrcGrid);
            var itemStateDst = GetItemDatasByColIdx(nDstCol).FirstOrDefault(i => i.Grid == nDstGrid);
            if (itemStateSrc != null || itemStateDst != null)
            {
                result.ItemResultValue = ItemResultType.ItemResultType_AddItemFail_NotExist;
                return result;
            }
            else
            {
                itemStateSrc.ColIdx = nDstCol;
                itemStateSrc.Grid = nDstGrid;
                itemStateDst.ColIdx = nSrcCol;
                itemStateDst.Grid = nSrcGrid;

                var columnDst = GetItemByColIdx(nDstCol);
                columnDst.ItemSaveStates.Remove(itemStateDst);
                columnDst.ItemSaveStates.Add(itemStateSrc);
                var columnSrc = GetItemByColIdx(nSrcCol);
                columnSrc.ItemSaveStates.Remove(itemStateSrc);
                columnSrc.ItemSaveStates.Add(itemStateDst);

                result.ItemList.Add(itemStateSrc);
                result.ItemList.Add(itemStateDst);
                result.ItemResultValue = ItemResultType.ItemResultType_Success;
                return result;
            }
        }
        public void SortItems(uint nCol)
        {
            List<ItemSaveState> itemSaveDatas = GetItemDatasByColIdx(nCol);
            foreach (var group in itemSaveDatas.GroupBy(i => NGSStaticData.items.GetItem(i.ItemID).Kind))
            {
                itemSaveDatas.RemoveAll(i => NGSStaticData.items.GetItem(i.ItemID).Kind == group.Key);
                itemSaveDatas.AddRange(group.OrderByDescending(i => NGSStaticData.items.GetItem(i.ItemID).Rarity));
            }
            foreach (var group in itemSaveDatas.GroupBy(i => NGSStaticData.items.GetItem(i.ItemID).Kind))
            {
                itemSaveDatas.RemoveAll(i => NGSStaticData.items.GetItem(i.ItemID).Kind == group.Key);
                itemSaveDatas.AddRange(group.OrderBy(i => i.ItemID));
            }

            for (uint i = 0; i < itemSaveDatas.Count; i++)
            {
                bags bag = itemContainer.bagDict.Find(item => item.bagItem.GUID == itemSaveDatas[(int)i].GUID);
                if (bag != null)
                {
                    bag.index = i;
                }
                else
                {
                    var stashItem = itemContainer.stashDict.FirstOrDefault(item => item.stashItem.GUID == itemSaveDatas[(int)i].GUID);
                    stashItem.index = i;
                }
            }
        }
        public List<ItemSaveState> GetItemDatasByColIdx(uint nColIdx)
        {
            List<ItemSaveState> result = new List<ItemSaveState>();
            var column = GetItemByColIdx(nColIdx);
            foreach(ItemSaveState itemState in column.ItemSaveStates)
            {
                result.Add(itemState);
            }
            return result;
        }
    }
}

