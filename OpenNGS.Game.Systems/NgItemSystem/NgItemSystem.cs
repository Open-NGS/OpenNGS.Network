using OpenNGS.Item.Common;
using OpenNGS.Item.Data;
using OpenNGS.Item.Service;
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
                    itemDatas.Add(item);
                }
            }
            return itemDatas;
        }
        private void UpdateItemInContainer(uint nColIdx, ItemSaveState item)
        {
            var column = GetItemColumnByColIdx(nColIdx);
            if (column != null)
            {
                var itemState = column.ItemSaveStates.FirstOrDefault(i => i.GUID == item.GUID);
                if (itemState != null)
                {
                    itemState.Count = item.Count;
                }
            }
        }
        private bool AssignUniqueGUID(ItemSaveState item)
        {
            bool isGUIDUnique = false;
            uint generatedGUID = guid_cache;
            while (!isGUIDUnique)
            {
                isGUIDUnique = true;
                foreach (var col in itemContainer.Col)
                {
                    if (col.ItemSaveStates.Any(i => i.GUID == generatedGUID))
                    {
                        isGUIDUnique = false;
                        generatedGUID = ++guid_cache;
                        break;
                    }
                }
            }
            item.GUID = generatedGUID;
            return isGUIDUnique;
        }

        public ItemColumn GetItemColumnByColIdx(uint nColIdx)
        {
            return itemContainer.Col.FirstOrDefault(c => c.ColIdx == nColIdx);
        }
        public AddItemRsp AddItemsByID(AddItemReq _req)
        {
            if (_req == null)
            {
                UnityEngine.Debug.LogError("AddItemReq is null in AddItemsByID call.");
                return null;
            }
            uint nItemID = _req.ItemID;
            uint nCounts = _req.Counts;
            uint nColIdx = _req.ColIdx;
            AddItemRsp result = new AddItemRsp();
            if (result.Result == null)
            {
                result.Result = new ItemResult();
            }
            if (result.Result.ItemList == null)
            {
                result.Result.ItemList = new List<ItemSaveState>();
            }
            OpenNGS.Item.Data.Item ItemInfo = NGSStaticData.items.GetItem(nItemID);
            if (ItemInfo == null)
            {
                result.Result.ItemResultValue = ItemResultType.ItemResultType_AddItemFail_NotExist;
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
                            result.Result.ItemList.Add(itemState[i]);
                        }
                        //该格子能装下
                        else
                        {
                            itemState[i].Count += nCounts;
                            nCounts = 0;
                            UpdateItemInContainer(nColIdx, itemState[i]);
                            result.Result.ItemList.Add(itemState[i]);
                            result.Result.ItemResultValue = ItemResultType.ItemResultType_Success;
                            break;
                        }
                    }
                    if (nCounts <= 0)
                    {
                        break;
                    }
                }
            }
            while (nCounts > 0)
            {
                ItemSaveState newItem = new ItemSaveState
                {
                    ItemID = nItemID,
                    Count = Math.Min(nCounts,stackMax),
                    ColIdx = nColIdx
                };

                if (!AssignUniqueGUID(newItem))
                {
                    result.Result.ItemResultValue = ItemResultType.ItemResultType_AddItemFail_NotExist;
                    return result;
                }

                nCounts -= newItem.Count;
                // 寻找第一个空格
                bool addedToExistingGrid = false;
                var column = GetItemColumnByColIdx(nColIdx);
                uint FirstEmptyGrid = column.Capacity;
                for (uint grid = 0; grid < column.Capacity; grid++)
                {
                    if (!column.ItemSaveStates.Any(i => i.Grid == grid))
                    {
                        FirstEmptyGrid = grid;
                        addedToExistingGrid = true;
                        break;
                    }
                }
                if (!addedToExistingGrid)
                {
                    result.Result.ItemResultValue = ItemResultType.ItemResultType_AddItemFail_NotEnoughGrid;
                }
                else
                {
                    foreach (var itemSaveState in column.ItemSaveStates)
                    {
                        if (itemSaveState.Grid < FirstEmptyGrid)
                        {
                            itemSaveState.Grid = itemSaveState.Grid + 1;
                        }
                    }
                    newItem.Grid = 0;
                    column.ItemSaveStates.Add(newItem);
                    result.Result.ItemList.Add(newItem);
                    result.Result.ItemResultValue = ItemResultType.ItemResultType_Success;
                }
            }
            return result;
        }

        public AddItemRsp RemoveItemsByGrid(RemoveItemReq _req)
        {
            uint nColIdx = _req.ColIdx;
            uint nGrid = _req.Grid;
            uint nCounts = _req.Counts;
            AddItemRsp result = new AddItemRsp();
            var column = GetItemColumnByColIdx(nColIdx);
            if (column == null)
            {
                result.Result.ItemResultValue = ItemResultType.ItemResultType_RemoveItemFail_GridNotExist;
                return result;
            }

            var itemState = column.ItemSaveStates.FirstOrDefault(i => i.Grid == nGrid);
            if (itemState == null || itemState.Count < nCounts)
            {
                result.Result.ItemResultValue = ItemResultType.ItemResultType_RemoveItemFail_NotEnoughNum;
                return result;
            }

            itemState.Count -= nCounts;
            if (itemState.Count == 0)
            {
                column.ItemSaveStates.Remove(itemState);
                guid_free.Enqueue(nGrid);
            }
            result.Result.ItemResultValue = ItemResultType.ItemResultType_Success;
            result.Result.ItemList.Add(itemState);
            return result;
        }

        public uint GetItemCountByGuid(uint nColIdx, uint nGuid)
        {
            var column = GetItemColumnByColIdx(nColIdx);
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

        public AddItemRsp ExchangeGrid(ChangeItemData _changeItemData)
        {
            uint nSrcCol = _changeItemData.SrcCol;
            uint nSrcGrid = _changeItemData.SrcGrid;
            uint nDstCol = _changeItemData.DstCol;
            uint nDstGrid = _changeItemData.DstGrid;
            AddItemRsp result = new AddItemRsp();
            result.Result.ItemList = new List<ItemSaveState>();
            var itemStateSrc = GetItemDatasByColIdx(nSrcCol).FirstOrDefault(i => i.Grid == nSrcGrid);
            var itemStateDst = GetItemDatasByColIdx(nDstCol).FirstOrDefault(i => i.Grid == nDstGrid);
            if (nSrcCol == nDstCol && nSrcGrid == nDstGrid)
            {
                result.Result.ItemResultValue = ItemResultType.ItemResultType_ExchangeGrid_StackFull;
                return result;
            }
            var columnDst = GetItemColumnByColIdx(nDstCol);
            var columnSrc = GetItemColumnByColIdx(nSrcCol);

            bool isSrcEmpty = itemStateSrc == null;
            bool isDstEmpty = itemStateDst == null;

            if (isSrcEmpty && isDstEmpty)
            {
                result.Result.ItemResultValue = ItemResultType.ItemResultType_ExchangeGridFail_GridNotExist;
                return result;
            }

            if (isSrcEmpty && !isDstEmpty)
            {
                itemStateDst.ColIdx = nSrcCol;
                itemStateDst.Grid = nSrcGrid;

                columnDst.ItemSaveStates.Remove(itemStateDst);
                columnSrc.ItemSaveStates.Add(itemStateDst);

            }
            else if (!isSrcEmpty && isDstEmpty)
            {
                itemStateSrc.ColIdx = nDstCol;
                itemStateSrc.Grid = nDstGrid;

                columnSrc.ItemSaveStates.Remove(itemStateSrc);
                columnDst.ItemSaveStates.Add(itemStateSrc);
            }
            else
            {
                uint tempGrid = itemStateSrc.Grid;
                uint tempColIdx = itemStateSrc.ColIdx;

                itemStateSrc.Grid = itemStateDst.Grid;
                itemStateSrc.ColIdx = itemStateDst.ColIdx;

                itemStateDst.Grid = tempGrid;
                itemStateDst.ColIdx = tempColIdx;

            }
            result.Result.ItemList.Add(itemStateSrc);
            result.Result.ItemList.Add(itemStateDst);
            result.Result.ItemResultValue = ItemResultType.ItemResultType_Success;
            return result;
        }
        public AddItemRsp SortItems(uint nCol)
        {
            AddItemRsp result = new AddItemRsp();
            result.Result.ItemList = new List<ItemSaveState>();
            List<ItemSaveState> itemSaveDatas = GetItemDatasByColIdx(nCol);
            if (itemSaveDatas == null)
            {
                result.Result.ItemResultValue = ItemResultType.ItemResultType_SortItemFail_NotExist;
                return result;
            }

            itemSaveDatas = itemSaveDatas.OrderBy(i => NGSStaticData.items.GetItem(i.ItemID).Kind).ToList();
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
                ItemSaveState item = itemContainer.Col[(int)nCol].ItemSaveStates.Find(item => item.GUID == itemSaveDatas[(int)i].GUID);
                if (item != null)
                {
                    item.Grid = i;
                    result.Result.ItemList.Add(item);
                }
            }
            result.Result.ItemResultValue = ItemResultType.ItemResultType_Success;
            return result;
        }
        private List<ItemSaveState> GetItemDatasByColIdx(uint nColIdx)
        {
            List<ItemSaveState> result = new List<ItemSaveState>();
            var column = GetItemColumnByColIdx(nColIdx);
            foreach(ItemSaveState itemState in column.ItemSaveStates)
            {
                result.Add(itemState);
            }
            return result;
        }


        public ItemResultType CanAddItem(AddReq _req)
        {
            return ItemResultType.ItemResultType_Success;
        }

        public AddItemRsp AddItems(AddReq _req)
        {
            AddItemRsp addItemRsp = new AddItemRsp();
            return addItemRsp;
        }

        public ItemResultType CanRemoveItemByID(RemoveItemsByIDsReq _req)
        {
            return ItemResultType.ItemResultType_Success;
        }

        public AddItemRsp RemoveItemByID(RemoveItemsByIDsReq _req)
        {
            AddItemRsp addItemRsp = new AddItemRsp();
            return addItemRsp;
        }

        public ItemResultType CanRemoveItemByGrid(RemoveItemsByGridsReq _req)
        {
            return ItemResultType.ItemResultType_Success;
        }

        public AddItemRsp RemoveItemByGrid(RemoveItemsByGridsReq _req)
        {
            AddItemRsp addItemRsp = new AddItemRsp();
            return addItemRsp;
        }
    }
}

