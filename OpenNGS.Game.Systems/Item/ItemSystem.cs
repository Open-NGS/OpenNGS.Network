using Newtonsoft.Json;
using OpenNGS.Systems;
using System;
using System.Collections.Generic;

using OpenNGS.Item;
using OpenNGS.Item.Data;
using OpenNGS.Item.Common;
using OpenNGS.Suit.Data;
using Systems;
using System.Linq;
using OpenNGSCommon;
using ItemData = OpenNGS.Item.Common.ItemData;
using Codice.Client.BaseCommands;
using Codice.Client.BaseCommands.Merge.Xml;

namespace OpenNGS.Systems
{
    public class ItemSystem : GameSubSystem<ItemSystem>, IItemSystem
    {

        public Action OnNotifyItemListChange;
        public Action OnPlacementChange;
        public Action<uint, OpenNGS.Item.Data.ItemSaveData> StashBoxChange;
        public Action<uint, OpenNGS.Item.Data.ItemSaveData> BagBoxChange;
        public Action<uint,OpenNGS.Item.Data.ItemSaveData> EquipBoxChange;

        public Dictionary<ulong, long> TempPlaceDic = new Dictionary<ulong, long>();
        public Dictionary<ulong, long> EternalPlaceDic = new Dictionary<ulong, long>();
        private bool m_IsCreate;
        public List<int> m_Filters = new() { 0, 1, 2, 3 };

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

        public SuitData GetSuitData(uint suitID)
        {
            SuitData suitData = new SuitData();
            return suitData;
        }
        
        public MakeDesign GetItemByItmes(uint itemId)
        {
            MakeDesign makeInfo = new MakeDesign();
            return makeInfo;
        }
        public ItemSaveData GetItemSaveDataByGuid(ulong guid)
        {
            bags bag = itemContainer.bagDict.Find(item => item.bagItem.GUID == guid);
            stashs stash = itemContainer.stashDict.Find(item => item.stashItem.GUID == guid);
            equips equip = itemContainer.equipDict.Find(item => item.equip.GUID == guid);
            if (bag != null)
            {
                return bag.bagItem;
            }
            else if(stash != null)
            {
                return stash.stashItem;
            }
            else if(equip != null)
            {
                return equip.equip;
            }
            else { return null; }
        }
        //获取道具信息(通过GUID)
        public bags GetItemDataByGuid(ulong uid)
        {
            bags bag = itemContainer.bagDict.Find(item => item.bagItem.GUID == uid);
            if (bag == null)
            {
                return null;
            }
            //OpenNGS.Item.Common.ItemData item = new ItemData();
            //item.ItemID = bag.bagItem.ItemID;
            //item.Guid = bag.bagItem.GUID;
            //item.Count = bag.bagItem.Count;
            return bag;
        }

        //获取道具信息(通过itemID)
        public List<OpenNGS.Item.Data.ItemSaveData> GetItemDataByItemId(uint itemId)
        {
            List<OpenNGS.Item.Data.ItemSaveData> itemDatas = new List<ItemSaveData>();
            foreach (var bag in itemContainer.bagDict)
            {
                if (bag.bagItem.ItemID == itemId)
                {
                    OpenNGS.Item.Data.ItemSaveData item = new ItemSaveData();
                    item.GUID = (uint)bag.bagItem.GUID;
                    item.ItemID = (uint)bag.bagItem.ItemID;
                    item.Count = (uint)bag.bagItem.Count;
                    itemDatas.Add(item);
                }
            }
            return itemDatas;
        }
        //获取某类所以道具的信息
        public List<OpenNGS.Item.Data.ItemSaveData> GetItemInfos(OpenNGS.Item.Common.ITEM_TYPE iTEM_TYPE)
        {
            List<OpenNGS.Item.Data.ItemSaveData> itemInfos = new List<OpenNGS.Item.Data.ItemSaveData>();
            foreach (bags bag in itemContainer.bagDict)
            {
                if (NGSStaticData.items.GetItem(bag.bagItem.ItemID).ItemType == iTEM_TYPE)
                {
                    OpenNGS.Item.Data.ItemSaveData itemData = new ItemSaveData();
                    itemData.ItemID = bag.bagItem.ItemID;
                    itemData.GUID = bag.bagItem.GUID;
                    itemData.Count = bag.bagItem.Count;
                    itemInfos.Add(itemData);
                }
            }
            //放回某类物品前排序,避免物品混乱
            itemInfos.Sort((a, b) =>
            {
                return (int)(a.ItemID - b.ItemID);
            });
            return itemInfos;
        }

        public List<OpenNGS.Item.Data.ItemSaveData> GetItemInfoByKind(OpenNGS.Item.Common.ITEM_KIND iTEM_KIND)
        {
            List<OpenNGS.Item.Data.ItemSaveData> itemInfos = new List<OpenNGS.Item.Data.ItemSaveData>();
            foreach (bags bag in itemContainer.bagDict)
            {
                if (NGSStaticData.items.GetItem(bag.bagItem.ItemID).Kind == iTEM_KIND)
                {
                    OpenNGS.Item.Data.ItemSaveData itemData = new ItemSaveData();
                    itemData.ItemID = bag.bagItem.ItemID;
                    itemData.GUID = bag.bagItem.GUID;
                    itemData.Count = bag.bagItem.Count;
                    itemInfos.Add(itemData);
                }
            }
            //放回某类物品前排序,避免物品混乱
            itemInfos.Sort((a, b) =>
            {
                return (int)(a.ItemID - b.ItemID);
            });
            return itemInfos;
        }

        public List<bags> GetItemInfosInBag()
        {
            List<bags> itemInfos = new List<bags>();
            foreach (bags itemInfo in itemContainer.bagDict)
            {
                if(NGSStaticData.items.GetItem(itemInfo.bagItem.ItemID) == null)
                {
                    NgDebug.LogError(string.Format("ItemSystem:GetItemInfosInBag error {0} item not found", itemInfo.bagItem.ItemID));
                }
                else
                {
                    if (NGSStaticData.items.GetItem(itemInfo.bagItem.ItemID).Visibility == ITEM_VISIBILITY_TYPE.ITEM_VISIBLE)
                    {
                        bags itemData = new bags();
                        itemData.index = itemInfo.index;
                        itemData.bagItem = itemInfo.bagItem;
                        itemInfos.Add(itemData);
                    }
                }
            }
            return itemInfos;
        }
        public List<stashs> GetItemInfosInStash()
        {
            List<stashs> itemInfos = new List<stashs>();
            foreach (stashs itemInfo in itemContainer.stashDict)
            {
                if (NGSStaticData.items.GetItem(itemInfo.stashItem.ItemID).Visibility == ITEM_VISIBILITY_TYPE.ITEM_VISIBLE)
                {
                    stashs itemData = new stashs();
                    itemData.index = itemInfo.index;
                    itemData.stashItem = itemInfo.stashItem;
                    itemInfos.Add(itemData);
                }
            }
            return itemInfos;
        }
        public List<OpenNGS.Item.Data.ItemSaveData> MergeItems(List<OpenNGS.Item.Data.ItemSaveData> itemInfos, bool isBag)
        {

            Dictionary<uint, OpenNGS.Item.Data.ItemSaveData> materialsDict = new Dictionary<uint, OpenNGS.Item.Data.ItemSaveData>();
            List<OpenNGS.Item.Data.ItemSaveData> itemDatas = new List<ItemSaveData>();
            foreach (OpenNGS.Item.Data.ItemSaveData itemSaveData in itemInfos)
            {
                if (NGSStaticData.items.GetItem(itemSaveData.ItemID).Kind == ITEM_KIND.ITEM_KIND_MATERIAL_STUFF)
                {
                    if (materialsDict.ContainsKey(itemSaveData.ItemID))
                    {
                        materialsDict[itemSaveData.ItemID].Count += itemSaveData.Count;
                        if (isBag)
                        {
                            OutBag(itemSaveData.GUID);
                        }
                        else
                        {
                            OutStash(itemSaveData.GUID);
                        }
                    }
                    else
                    {
                        materialsDict.Add(itemSaveData.ItemID, itemSaveData);
                    }
                }
                else
                {
                    itemDatas.Add(itemSaveData);
                }
            }
            List<ItemSaveData> result = materialsDict.Values.ToList();
            for (uint i = 0; i < result.Count; i++)
            {
                if (isBag)
                {
                    bags bag = itemContainer.bagDict.Find(item => item.bagItem.GUID == result[(int)i].GUID);
                    bag.bagItem.Count = result[(int)i].Count;
                }
                else
                {
                    stashs stashItem = itemContainer.stashDict.FirstOrDefault(item => item.stashItem.GUID == result[(int)i].GUID);
                    stashItem.stashItem.Count = result[(int)i].Count;
                }
                itemDatas.Add(result[(int)i]);
            }
            return itemDatas;
        }

        public void SortItems(List<OpenNGS.Item.Data.ItemSaveData> itemInfos)
        {
            itemInfos = itemInfos.OrderBy(i =>
            {
                switch (NGSStaticData.items.GetItem(i.ItemID).Kind)
                {
                    case OpenNGS.Item.Common.ITEM_KIND.ITEM_KIND_MATERIAL_STUFF:
                        return 0;
                    case OpenNGS.Item.Common.ITEM_KIND.ITEM_KIND_EQUIP_WEAPON:
                        return 1;
                    case OpenNGS.Item.Common.ITEM_KIND.ITEM_KIND_EQUIP_HEAD:
                        return 2;
                    case OpenNGS.Item.Common.ITEM_KIND.ITEM_KIND_EQUIP_BODY:
                        return 3;
                    case OpenNGS.Item.Common.ITEM_KIND.ITEM_KIND_EQUIP_ARM:
                        return 4;
                    case OpenNGS.Item.Common.ITEM_KIND.ITEM_KIND_EQUIP_LEG:
                        return 5;
                    case OpenNGS.Item.Common.ITEM_KIND.ITEM_KIND_EQUIP_SHOES:
                        return 6;
                    case OpenNGS.Item.Common.ITEM_KIND.ITEM_KIND_MATERIAL_BLUEPRINT:
                        return 7;
                    default:
                        return 8;
                }
            }).ToList();

            // 然后在每个类型内按品质从高到低排序
            foreach (var group in itemInfos.GroupBy(i => NGSStaticData.items.GetItem(i.ItemID).Kind))
            {
                itemInfos.RemoveAll(i => NGSStaticData.items.GetItem(i.ItemID).Kind == group.Key);
                itemInfos.AddRange(group.OrderByDescending(i => NGSStaticData.items.GetItem(i.ItemID).Rarity));
            }

            // 最后在每个类型内按 ID 从小到大排序
            foreach (var group in itemInfos.GroupBy(i => NGSStaticData.items.GetItem(i.ItemID).Kind))
            {
                itemInfos.RemoveAll(i => NGSStaticData.items.GetItem(i.ItemID).Kind == group.Key);
                itemInfos.AddRange(group.OrderBy(i => i.ItemID));
            }

            for (uint i = 0; i < itemInfos.Count; i++)
            {
                bags bag = itemContainer.bagDict.Find(item => item.bagItem.GUID == itemInfos[(int)i].GUID);
                if (bag != null)
                {
                    bag.index = i;
                }
                else
                {
                    var stashItem = itemContainer.stashDict.FirstOrDefault(item => item.stashItem.GUID == itemInfos[(int)i].GUID);
                    stashItem.index = i;
                }
            }
        }
        //获取道具放置数量
        public long GetItemPlaceCount(ulong itemGuid)
        {
            long count = 0;
            if (TempPlaceDic.TryGetValue(itemGuid, out var tempCount))
            {
                count += tempCount;
            }

            if (EternalPlaceDic.TryGetValue(itemGuid, out var eternalCount))
            {
                count += eternalCount;
            }

            return count;
        }


        #region C2S

        //private void GetItems()
        //{
        //    //Debug.Log($"{nameof(ItemSystem)} GetItems");
        //    //ItemService.Instance.GetItemRequest(m_Uin).ContinueWith((rsp) =>
        //    //{
        //    //    OnGetItemResponse(rsp.Result);
        //    //});
        //}

        //public async Task UseItemRequest(ItemData itemData, int count, Action<List<NgsCommon.ItemList>> callback = null)
        //{
        //    //var rsp = await ItemService.Instance.UseItemRequest(m_Uin, new ItemData
        //    //{
        //    //    Guid = itemData.Guid,
        //    //    ItemID = itemData.ItemID,
        //    //    Count = (uint)count
        //    //});

        //    //if (rsp?.result == 0)
        //    //{
        //    //    if (rsp?.itemList?.Count > 0)
        //    //    {
        //    //        callback?.Invoke(rsp?.itemList);
        //    //    }
        //    //    //Debug.Log($"{nameof(ItemSystem)} UseItemRequest Success");
        //    //}
        //    //else
        //    //{
        //    //    Debug.LogError($"UseItemRequest Error: {rsp?.result}");
        //    //}
        //    return Task.FromException(new NotImplementedException());
        //}

        /// <summary>
        /// 全部收回
        /// </summary>
        public async void ReplaceItemsRequest()
        {
            //var rsp = await ItemService.Instance.ReplaceItemsRequest(m_Uin);
            //if (rsp?.result == 0)
            //{
            //    //Debug.Log($"{nameof(ItemSystem)} 找回所有 ReplaceItemsRequest Success");
            //}
            //else
            //{
            //    Debug.LogError($"找回所有 ReplaceItemsRequest Error {rsp?.result}");
            //}
        }

        #endregion

        #region S2C


        //public override void OnStatus(StatusData status)
        //{
        //    var itemLst = status.Messages<ItemList>();
        //    if (itemLst == null)
        //    {
        //        return;
        //    }

        //    if (status.OpCode == StatusOpCode.Status_Sync)
        //    {
        //        ItemList.Clear();
        //    }

        //    //Debug.Log($"{nameof(ItemSystem)} {status.OpCode} OnStatus" + ":" + JsonConvert.SerializeObject(itemLst));

        //    foreach (var msg in itemLst)
        //    {
        //        var items = msg.Items;
        //        if (items == null) continue;

        //        if (status.OpCode == StatusOpCode.Status_Sync)
        //        {
        //            foreach (var item in items)
        //            {
        //                ItemList.Add(item);
        //            }
        //        }
        //        else
        //        {
        //            foreach (var it in items)
        //            {
        //                var item = GetItemData(it.Guid);
        //                switch (status.OpCode)
        //                {
        //                    case StatusOpCode.Status_Add:
        //                        if (item != null)
        //                        {
        //                            item.Count += it.Count;
        //                        }
        //                        else
        //                        {
        //                            ItemList.Add(it);
        //                        }

        //                        break;
        //                    case StatusOpCode.Status_Update:
        //                        if (item == null)
        //                        {
        //                            Debug.LogError($"OnStatusUpdate item:{it.ItemID} not exist");
        //                        }
        //                        else
        //                        {
        //                            item.Attributes = it.Attributes;
        //                            item.Count = it.Count;
        //                        }

        //                        break;
        //                    case StatusOpCode.Status_Remove:
        //                        if (item == null)
        //                        {
        //                            Debug.LogError($"OnStatusRemove item:{it.ItemID} not exist");
        //                        }
        //                        else
        //                        {
        //                            item.Count -= it.Count;
        //                            if (item.Count <= 0)
        //                            {
        //                                ItemList.Remove(item);
        //                            }
        //                        }

        //                        break;
        //                    default:
        //                        if (item != null)
        //                        {
        //                            item.Attributes = it.Attributes;
        //                            item.Count = it.Count;
        //                        }
        //                        else
        //                        {
        //                            ItemList.Add(it);
        //                        }

        //                        break;
        //                }
        //            }
        //        }
        //    }

        //    //CollectPlaceItems();
        //    OnNotifyItemListChange?.Invoke();

        //    if (status.OpCode == StatusOpCode.Status_Sync && !m_IsCreate)
        //    {
        //        // 登录第一次请求完毕后，将所有状态清空
        //        //Debug.Log($"{nameof(ItemSystem)} 登录清空放置状态");
        //        m_IsCreate = true;
        //        ReplaceItemsRequest();
        //    }
        //}

        #endregion

        public override string GetSystemName()
        {
            return "com.openngs.system.item";
        }
        
        public bool IsEnoughByItemID(uint nItemID, uint nCounts)
        {
            List<OpenNGS.Item.Data.ItemSaveData> itemData = GetItemDataByItemId(nItemID);
            if(itemData == null)
            {
                return false;
            }
            uint sum = 0;
            foreach(var item in itemData)
            {
                sum += item.Count;
            }
            bool bRes = sum >= nCounts;
            return bRes;
        }

        public bool IsEnoughByGuid(uint nGuid, uint nCounts)
        {
            bags itemData = GetItemDataByGuid(nGuid);
            if (itemData == null)
            {
                return false;
            }
            bool bRes = itemData.bagItem.Count >= nCounts;
            return bRes;
        }
        private uint FindUnusedBagIndex(List<bags> bagDict)
        {
            for (uint i = 0; i < itemContainer.bagCapacity; i++)
            {
                // 检查索引i是否未被使用
                bool isIndexUsed = bagDict.Exists(bag => bag.index == i);
                if (!isIndexUsed)
                {
                    return i;
                }
            }
            return 0;
        }
        private uint FindUnusedStashIndex(List<stashs> stashDict)
        {
            for (uint i = 0; i < itemContainer.stashCapacity; i++)
            {
                bool isIndexUsed = stashDict.Exists(stash => stash.index == i);
                if (!isIndexUsed)
                {
                    return i;
                }
            }
            return 0;
        }
        public void MoveBagsBackIndex()
        {
            var sortedBags = itemContainer.bagDict.OrderBy(bag => bag.index).ToList();

            uint unusedIndex = FindUnusedBagIndex(itemContainer.bagDict);
            foreach (var bag in sortedBags)
            {
                if (unusedIndex == bag.index + 1)
                {
                    bag.index = bag.index + 1;
                    break;
                }
                else
                {
                    bag.index = bag.index + 1;
                }

            }
        }
        public void MoveStashsBackIndex()
        {
            var sortedBags = itemContainer.stashDict.OrderBy(stash => stash.index).ToList();
            uint unusedIndex = FindUnusedStashIndex(itemContainer.stashDict);
            foreach (var stash in sortedBags)
            {
                if (unusedIndex == stash.index + 1)
                {
                    stash.index = stash.index + 1;
                    break;
                }
                else
                {
                    stash.index = stash.index + 1;
                }

            }
        }


        public Item.Data.ItemSaveData AddItemsByID(uint nItemID, uint nCounts)
        {
            OpenNGS.Item.Data.Item ItemInfo = NGSStaticData.items.GetItem(nItemID);
            if(ItemInfo == null)
            {
                return null;
            }
            Item.Data.ItemSaveData _retItemSaveData = null;
            List<OpenNGS.Item.Data.ItemSaveData> itemData = GetItemDataByItemId(nItemID);
            //已获得过的物品
            if (itemData != null && itemData.Count > 0)
            {
                //先查找背包可堆叠的空位
                for (int i = 0; i < itemData.Count; i++)
                {
                    if (itemData[i].Count < NGSStaticData.items.GetItem(nItemID).StackMax)
                    {
                        uint volumn = NGSStaticData.items.GetItem(nItemID).StackMax - itemData[i].Count;
                        //该格子装不下
                        if ((int)nCounts - (int)volumn >= 0)
                        {
                            itemData[i].Count = NGSStaticData.items.GetItem(nItemID).StackMax;
                            var itemToUpdate = itemContainer.bagDict.FirstOrDefault(item => item.bagItem.GUID == itemData[i].GUID);
                            if (itemToUpdate != null)
                            {
                                itemToUpdate.bagItem.Count = NGSStaticData.items.GetItem(nItemID).StackMax;
                            }
                            nCounts -= volumn;
                            BagBoxChange?.Invoke(itemData[i].GUID, itemData[i]);
                            _retItemSaveData = itemData[i];
                        }
                        //该格子能装下
                        else
                        {
                            itemData[i].Count += nCounts;
                            var itemToUpdate = itemContainer.bagDict.FirstOrDefault(item => item.bagItem.GUID == itemData[i].GUID);
                            if (itemToUpdate != null)
                            {
                                itemToUpdate.bagItem.Count += nCounts;
                            }
                            nCounts = 0;
                            BagBoxChange?.Invoke(itemData[i].GUID, itemData[i]);
                            _retItemSaveData = itemData[i];
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
                OpenNGS.Item.Data.ItemSaveData item = new OpenNGS.Item.Data.ItemSaveData();
                item.ItemID = nItemID;
                //前面有空闲出来的Guid
                if(guid_free.Count > 0)
                {
                    item.GUID = guid_free.Dequeue();
                }
                //前面无空闲Guid则用后面数赋值
                else
                {
                    while (itemContainer.bagDict.Any(item => item.bagItem.GUID == guid_cache) 
                        || itemContainer.equipDict.Any(item => item.equip.GUID == guid_cache)
                        || itemContainer.stashDict.Any(item => item.stashItem.GUID == guid_cache))
                    {
                        guid_cache++;
                    }
                    item.GUID = guid_cache;
                }
                //超出最大堆叠数
                if (nCounts > NGSStaticData.items.GetItem(nItemID).StackMax)
                {
                    item.Count = NGSStaticData.items.GetItem(nItemID).StackMax;
                    nCounts -= NGSStaticData.items.GetItem(nItemID).StackMax;
                }
                else
                {
                    item.Count = nCounts;
                    nCounts = 0;
                }
                //添加到动态数据
                bags bag = new bags();
                bag.bagItem = item;
                bag.index = 0;
                MoveBagsBackIndex();
                itemContainer.AddItem(bag);
                BagBoxChange?.Invoke(item.GUID, item);
                _retItemSaveData = item;
            }
            return _retItemSaveData;
        }
        public bool RemoveItemsByID(uint nItemID, uint nCounts)
        {
            List<OpenNGS.Item.Data.ItemSaveData> itemData = GetItemDataByItemId(nItemID);
            if (itemData == null || itemData.Count <= 0 || !IsEnoughByItemID(nItemID,nCounts))
            {
                return false;
            }
            //从后往前删除共计nCounts个的该物品
            for(int i = itemData.Count - 1; i >= 0 ; i--)
            {
                //该格子物品数量未达到nCounts
                if ((int)nCounts - (int)itemData[i].Count >= 0)
                {
                    //若物品为0,在动态数据与缓存链表中删去该物品
                    nCounts -= itemData[i].Count;
                    itemData[i].Count = 0;
                    guid_free.Enqueue(itemData[i].GUID);//Guid空闲出来后放入队列
                    var itemToRemove = itemContainer.bagDict.FirstOrDefault(item => item.bagItem.GUID == itemData[i].GUID);
                    if (itemToRemove != null)
                    {
                        itemContainer.RemoveItem(itemToRemove);
                    }
                }
                else
                {
                    itemData[i].Count -= nCounts;
                    break;
                }
                BagBoxChange?.Invoke(itemData[i].GUID, itemData[i]);
            }
            return true;
        }

        public bool RemoveItemsByGuid(uint nGuid, uint nCounts)
        {
            bags itemData = GetItemDataByGuid(nGuid);
            //没有该物品或移除数大于拥有数
            if (itemData == null || itemData.bagItem.Count < nCounts)
            {
                return false;
            }
            //物品移除后无剩余
            if (itemData.bagItem.Count == nCounts)
            {
                guid_free.Enqueue(nGuid);//Guid空闲出来后放入队列
                itemData.bagItem.Count = 0;
                var itemToRemove = itemContainer.bagDict.FirstOrDefault(item => item.bagItem.GUID == nGuid);
                if (itemToRemove != null)
                {
                    itemContainer.RemoveItem(itemToRemove);
                }
            }
            //物品移除后有剩余
            else
            {
                itemData.bagItem.Count -= nCounts;
            }
            BagBoxChange?.Invoke(itemData.bagItem.GUID, itemData.bagItem);
            return true;
        }
        public uint GetGuidByItemID(uint nItemID)
        {
            List<OpenNGS.Item.Data.ItemSaveData> itemData = GetItemDataByItemId(nItemID);
            if (itemData != null && itemData.Count > 0)
            {
                return itemData[0].GUID;
            }
            return 0;
        }
        public uint GetItemCountByGuidID(uint nGuid)
        {
            bags itemData = GetItemDataByGuid(nGuid);
            if (itemData == null)
            {
                return 0;
            }
            return itemData.bagItem.Count;
        }

        public uint GetItemTotalCountByItemID(uint itemID)
        {
            uint sum = 0;
            List<OpenNGS.Item.Data.ItemSaveData> items = GetItemDataByItemId(itemID);
            foreach (var item in items)
            {
                sum += item.Count;
            }
            return sum;
        }

        public bool UseItem(uint nGuid)
        {
            if(!IsEnoughByGuid(nGuid, 1))
            {
                return false;
            }
            return RemoveItemsByGuid(nGuid, 1);
        }
        public DisassembleEquipIno GetDisassembleEquipIno(uint itemId)
        {
            DisassembleEquipIno disassembleInfo = new DisassembleEquipIno();
            return disassembleInfo;
        }
        public uint OverlayItemBagStash(uint guid1, uint guid2)
        {
            var changeitem2 = itemContainer.stashDict.FirstOrDefault(item => item.stashItem.GUID == guid2);
            if (changeitem2 == null)
            {
                var changeitem22 = itemContainer.bagDict.FirstOrDefault(item => item.bagItem.GUID == guid2);
                var changeitem11 = itemContainer.stashDict.FirstOrDefault(item => item.stashItem.GUID == guid1);
                if (changeitem11 == null)
                {
                    changeitem22.bagItem.Count += itemContainer.bagDict.FirstOrDefault(item => item.bagItem.GUID == guid1).bagItem.Count;
                    OutBag(guid1);
                    return changeitem22.index;
                }
                else
                {
                    OutStash(guid1);
                    changeitem22.bagItem.Count += changeitem11.stashItem.Count;
                    return changeitem22.index;
                }
            }
            else
            {
                var changeitem11 = itemContainer.stashDict.FirstOrDefault(item => item.stashItem.GUID == guid1);
                if (changeitem11 == null)
                {
                    changeitem2.stashItem.Count += itemContainer.bagDict.FirstOrDefault(item => item.bagItem.GUID == guid1).bagItem.Count;
                    OutBag(guid1);
                    return changeitem2.index;
                }
                else
                {
                    OutStash(guid1);
                    changeitem2.stashItem.Count += changeitem11.stashItem.Count;
                    return changeitem2.index;
                }
            }


        }
        public uint InBag(uint nGuid)
        {
            var changeitem = itemContainer.stashDict.FirstOrDefault(item => item.stashItem.GUID == nGuid);
            if (NGSStaticData.items.GetItem(changeitem.stashItem.ItemID).Kind == ITEM_KIND.ITEM_KIND_MATERIAL_STUFF && itemContainer.bagDict.FirstOrDefault(item => item.bagItem.ItemID == changeitem.stashItem.ItemID)!=null)
            {
                AddItemsByID(changeitem.stashItem.ItemID, changeitem.stashItem.Count);
                return itemContainer.bagDict.FirstOrDefault(item => item.bagItem.ItemID == changeitem.stashItem.ItemID).index;
            }
            else if (changeitem != null)
            {
                bags bag = new bags();
                bag.bagItem = changeitem.stashItem;
                bag.index = FindUnusedBagIndex(itemContainer.bagDict);
                //MoveBagsBackIndex();
                itemContainer.AddItem(bag);
                return bag.index;
            }
            return 0;
        }
        public uint InStash(uint nGuid)
        {
            var changeitem = itemContainer.bagDict.FirstOrDefault(item => item.bagItem.GUID == nGuid);
            if (NGSStaticData.items.GetItem(changeitem.bagItem.ItemID).Kind == ITEM_KIND.ITEM_KIND_MATERIAL_STUFF && itemContainer.stashDict.FirstOrDefault(item => item.stashItem.ItemID == changeitem.bagItem.ItemID) != null)
            {
                stashs stashItem = itemContainer.stashDict.FirstOrDefault(s => s.stashItem.ItemID == changeitem.bagItem.ItemID);
                stashItem.stashItem.Count += changeitem.bagItem.Count;
                return stashItem.index;
            }
            else if (changeitem != null)
            {
                stashs stash = new stashs();
                stash.stashItem = changeitem.bagItem;
                stash.index = FindUnusedStashIndex(itemContainer.stashDict);
                itemContainer.AddStashItem(stash);
                return stash.index;
            }
            return 0;
        }
        public void OutStash(uint nGuid)
        {
            var changeitem = itemContainer.stashDict.FirstOrDefault(item => item.stashItem.GUID == nGuid);
            if (changeitem != null)
            {
                itemContainer.RemoveStashItem(changeitem);
            }
        }
        public void OutBag(uint nGuid)
        {
            var changeitem = itemContainer.bagDict.FirstOrDefault(item => item.bagItem.GUID == nGuid);
            if (changeitem != null && NGSStaticData.items.GetItem(changeitem.bagItem.ItemID).ItemType != ITEM_TYPE.ITEM_TYPE_RESOURCE)
            {
                itemContainer.RemoveItem(changeitem);
            }
        }
        public int GetIndex(uint id, bool isBag)
        {
            if (isBag)
            {
                var bagItem = itemContainer.bagDict.FirstOrDefault(item => item.bagItem.GUID == id);
                if (bagItem != null)
                {
                    return (int)bagItem.index;
                }
                else
                {
                    return -1;
                }

            }
            else
            {
                var stashItem = itemContainer.stashDict.FirstOrDefault(item => item.stashItem.GUID == id);
                if (stashItem != null)
                {
                    return (int)stashItem.index;
                }
                else
                {
                    return -1;
                }
            }
        }
        public void SetIndex(uint i, uint id, bool isBag)
        {
            if (isBag)
            {
                var bagItem = itemContainer.bagDict.FirstOrDefault(item => item.bagItem.GUID == id);
                if (bagItem != null)
                {
                    bagItem.index = i;
                }
                else
                {
                    var stashItem = itemContainer.stashDict.FirstOrDefault(item => item.stashItem.GUID == id);
                    OutStash(stashItem.stashItem.GUID);
                    bags bag = new bags();
                    bag.bagItem = stashItem.stashItem;
                    bag.index = i;
                    itemContainer.AddItem(bag);
                }
            }
            else
            {
                var stashItem = itemContainer.stashDict.FirstOrDefault(item => item.stashItem.GUID == id);
                if (stashItem != null)
                {
                    stashItem.index = i;
                }
                else
                {
                    var bagItem = itemContainer.bagDict.FirstOrDefault(item => item.bagItem.GUID == id);
                    OutBag(bagItem.bagItem.GUID);
                    stashs stash = new stashs();
                    stash.stashItem = bagItem.bagItem;
                    stash.index = i;
                    itemContainer.AddStashItem(stash);
                }
            }
        }
        public void SetNull(uint i, bool isBag)
        {
            if (isBag)
            {
                var bagItem = itemContainer.bagDict.FirstOrDefault(item => item.index == i);
                if (bagItem != null)
                {
                    OutBag(bagItem.bagItem.GUID);
                }
            }
            else
            {
                var stashItem = itemContainer.stashDict.FirstOrDefault(item => item.index == i);
                if (stashItem != null)
                {
                    OutBag(stashItem.stashItem.GUID);
                }
            }

        }
        public OpenNGS.Item.Common.EQUIP_RESULT_TYPE Equipped(uint index, uint nGuid)
        {
            //if (index >= itemContainer.equips.Count || !IsEnoughByGuid(nGuid, 1))
            //{
            //    return EQUIP_RESULT_TYPE.EQUIP_RESULT_TYPE_ERROR;
            //}
            bags itemData = GetItemDataByGuid(nGuid);
            if (itemData == null || itemData.bagItem.Count < 1)
            {
                return EQUIP_RESULT_TYPE.EQUIP_RESULT_TYPE_ERROR;
            }
            var itemToRemove = itemContainer.bagDict.FirstOrDefault(item => item.bagItem.GUID == nGuid);
            if (itemToRemove != null)
            {
                itemContainer.RemoveItem(itemToRemove);
                equips equipDict = new equips();
                equipDict.equip = itemToRemove.bagItem;
                equipDict.index = index;
                itemContainer.AddEquips(equipDict);
            }
            return EQUIP_RESULT_TYPE.EQUIP_RESULT_TYPE_SUCCESS;
        }

        public OpenNGS.Item.Common.EQUIP_RESULT_TYPE Unequipped(uint index)
        {
            var equippedItem = itemContainer.equipDict.FirstOrDefault(item => item.index == index);
            if (equippedItem == null || index > equippedItem.index || index < 0)
            {
                return EQUIP_RESULT_TYPE.EQUIP_RESULT_TYPE_ERROR;
            }
            if (equippedItem != null)
            {
                itemContainer.RemoveEquips(equippedItem);
                bags bag = new bags();
                bag.bagItem = equippedItem.equip;
                bag.index = FindUnusedBagIndex(itemContainer.bagDict); ;
                //MoveBagsBackIndex();
                itemContainer.AddItem(bag);
            }
            return EQUIP_RESULT_TYPE.EQUIP_RESULT_TYPE_SUCCESS;
        }

        public List<equips> GetEquippedList()
        {
            return itemContainer.equipDict ;
        }
        public void AddAction_stashChange(Action<uint, OpenNGS.Item.Data.ItemSaveData> ac)
        {
            StashBoxChange += ac;
        }
        //添加道具栏变更事件
        public void AddAction_bagChange(Action<uint, OpenNGS.Item.Data.ItemSaveData> ac)
        {
            BagBoxChange += ac;
        }
        //添加装备栏变更事件
        public void AddAction_equipChange(Action<uint, OpenNGS.Item.Data.ItemSaveData> ac)
        {
            EquipBoxChange += ac;
        }
        public void RemoveAction_stashChange(Action<uint, OpenNGS.Item.Data.ItemSaveData> ac)
        {
            StashBoxChange -= ac;
        }
        //删去道具栏变更事件
        public void RemoveAction_bagChange(Action<uint, OpenNGS.Item.Data.ItemSaveData> ac)
        {
            BagBoxChange -= ac;
        }
        //删去装备栏变更事件
        public void RemoveAction_equipChange(Action<uint,OpenNGS.Item.Data.ItemSaveData> ac)
        {
            EquipBoxChange -= ac;
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
    }
}