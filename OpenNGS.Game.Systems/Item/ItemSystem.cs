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
namespace OpenNGS.Systems
{
    public class ItemSystem : GameSubSystem<ItemSystem>, IItemSystem
    {

        public Action OnNotifyItemListChange;
        public Action OnPlacementChange;
        public Action<uint, OpenNGS.Item.Common.ItemData> StashBoxChange;
        public Action<uint,OpenNGS.Item.Common.ItemData> BagBoxChange;
        public Action<uint,OpenNGS.Item.Common.ItemData> EquipBoxChange;

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

        //获取道具信息(通过GUID)
        public OpenNGS.Item.Common.ItemData GetItemDataByGuid(ulong uid)
        {
            ItemSaveData itemData = itemContainer.bagItems.Find(item => item.GUID == uid);
            if (itemData == null)
            {
                return null;
            }
            OpenNGS.Item.Common.ItemData item = new ItemData();
            item.ItemID = itemData.ItemID;
            item.Guid = itemData.GUID;
            item.Count = itemData.Count;
            return item;
        }

        //获取道具信息(通过itemID)
        public List<OpenNGS.Item.Common.ItemData> GetItemDataByItemId(uint itemId)
        {
            List<OpenNGS.Item.Common.ItemData> itemDatas = new List<ItemData>();
            foreach (ItemSaveData itemData in itemContainer.bagItems)
            {
                if ((ulong)itemData.ItemID == itemId)
                {
                    OpenNGS.Item.Common.ItemData item = new ItemData();
                    item.Guid = (uint)itemData.GUID;
                    item.ItemID = (uint)itemData.ItemID;
                    item.Count = (uint)itemData.Count;
                    itemDatas.Add(item);
                }
            }
            return itemDatas;
        }
        //获取某类所以道具的信息
        public List<OpenNGS.Item.Common.ItemData> GetItemInfos(OpenNGS.Item.Common.ITEM_TYPE iTEM_TYPE)
        {
            List<OpenNGS.Item.Common.ItemData> itemInfos = new List<OpenNGS.Item.Common.ItemData>();
            foreach (ItemSaveData itemInfo in itemContainer.bagItems)
            {
                if (NGSStaticData.items.GetItem(itemInfo.ItemID).ItemType == iTEM_TYPE)
                {
                    OpenNGS.Item.Common.ItemData itemData = new ItemData();
                    itemData.ItemID = itemInfo.ItemID;
                    itemData.Guid = itemInfo.GUID;
                    itemData.Count = itemInfo.Count;
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

        public List<OpenNGS.Item.Common.ItemData> GetItemInfoByKind(OpenNGS.Item.Common.ITEM_KIND iTEM_KIND)
        {
            List<OpenNGS.Item.Common.ItemData> itemInfos = new List<OpenNGS.Item.Common.ItemData>();
            foreach(ItemSaveData itemInfo in itemContainer.bagItems)
            {
                if(NGSStaticData.items.GetItem(itemInfo.ItemID).Kind == iTEM_KIND)
                {
                    OpenNGS.Item.Common.ItemData itemData = new ItemData();
                    itemData.ItemID = itemInfo.ItemID;
                    itemData.Guid = itemInfo.GUID;
                    itemData.Count = itemInfo.Count;
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

        public List<OpenNGS.Item.Common.ItemData> GetItemInfosInBag()
        {
            List<OpenNGS.Item.Common.ItemData> itemInfos = new List<ItemData>();
            foreach(ItemSaveData itemInfo in itemContainer.bagItems)
            {
                if (NGSStaticData.items.GetItem(itemInfo.ItemID).Visibility == ITEM_VISIBILITY_TYPE.ITEM_VISIBLE)
                {
                    ItemData itemData = new ItemData();
                    itemData.ItemID = itemInfo.ItemID;
                    itemData.Guid = itemInfo.GUID;
                    itemData.Count = itemInfo.Count;
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
        public List<OpenNGS.Item.Common.ItemData> GetItemInfosInStash()
        {
            List<OpenNGS.Item.Common.ItemData> itemInfos = new List<ItemData>();
            foreach (ItemSaveData itemInfo in itemContainer.stashItems)
            {
                if (NGSStaticData.items.GetItem(itemInfo.ItemID).Visibility == ITEM_VISIBILITY_TYPE.ITEM_VISIBLE)
                {
                    ItemData itemData = new ItemData();
                    itemData.ItemID = itemInfo.ItemID;
                    itemData.Guid = itemInfo.GUID;
                    itemData.Count = itemInfo.Count;
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
            List<OpenNGS.Item.Common.ItemData> itemData = GetItemDataByItemId(nItemID);
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
            OpenNGS.Item.Common.ItemData itemData = GetItemDataByGuid(nGuid);
            if (itemData == null)
            {
                return false;
            }
            bool bRes = itemData.Count >= nCounts;
            return bRes;
        }


        public bool AddItemsByID(uint nItemID, uint nCounts)
        {
            List<OpenNGS.Item.Common.ItemData> itemData = GetItemDataByItemId(nItemID);
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
                            var itemToUpdate = itemContainer.bagItems.FirstOrDefault(item => item.GUID == itemData[i].Guid);
                            if (itemToUpdate != null)
                            {
                                itemToUpdate.Count = NGSStaticData.items.GetItem(nItemID).StackMax;
                            }
                            nCounts -= volumn;
                            BagBoxChange?.Invoke(itemData[i].Guid, itemData[i]);
                        }
                        //该格子能装下
                        else
                        {
                            itemData[i].Count += nCounts;
                            var itemToUpdate = itemContainer.bagItems.FirstOrDefault(item => item.GUID == itemData[i].Guid);
                            if (itemToUpdate != null)
                            {
                                itemToUpdate.Count += nCounts;
                            }
                            nCounts = 0;
                            BagBoxChange?.Invoke(itemData[i].Guid, itemData[i]);
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
                OpenNGS.Item.Common.ItemData item = new OpenNGS.Item.Common.ItemData();
                item.ItemID = nItemID;
                //前面有空闲出来的Guid
                if(guid_free.Count > 0)
                {
                    item.Guid = guid_free.Dequeue();
                }
                //前面无空闲Guid则用后面数赋值
                else
                {
                    while (itemContainer.bagItems.Any(item => item.GUID == guid_cache) || itemContainer.equipDict.Any(item => item.index == guid_cache))
                    {
                        guid_cache++;
                    }
                    item.Guid = guid_cache;
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
                ItemSaveData itemSaveData = new ItemSaveData();
                itemSaveData.GUID = item.Guid;
                itemSaveData.ItemID = item.ItemID;
                itemSaveData.Count = item.Count;
                itemContainer.AddItem(itemSaveData);
                BagBoxChange?.Invoke(item.Guid, item);
            }
            return true;
        }
        public bool RemoveItemsByID(uint nItemID, uint nCounts)
        {
            List<OpenNGS.Item.Common.ItemData> itemData = GetItemDataByItemId(nItemID);
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
                    guid_free.Enqueue(itemData[i].Guid);//Guid空闲出来后放入队列
                    var itemToRemove = itemContainer.bagItems.FirstOrDefault(item => item.GUID == itemData[i].Guid);
                    if (itemToRemove != null)
                    {
                        itemContainer.RemoveItem(itemToRemove);
                    }
                }
                else
                {
                    var itemToRemove = itemContainer.bagItems.FirstOrDefault(item => item.GUID == itemData[i].Guid);
                    itemData[i].Count -= nCounts;
                    if (itemToRemove != null)
                    {
                        itemToRemove.Count = (itemData[i].Count - nCounts);
                    }
                    break;
                }
                BagBoxChange?.Invoke(itemData[i].Guid, itemData[i]);
            }
            return true;
        }

        public bool RemoveItemsByGuid(uint nGuid, uint nCounts)
        {
            OpenNGS.Item.Common.ItemData itemData = GetItemDataByGuid(nGuid);
            //没有该物品或移除数大于拥有数
            if (itemData == null || itemData.Count < nCounts)
            {
                return false;
            }
            //物品移除后无剩余
            if (itemData.Count == nCounts)
            {
                guid_free.Enqueue(nGuid);//Guid空闲出来后放入队列
                itemData.Count = 0;
                var itemToRemove = itemContainer.bagItems.FirstOrDefault(item => item.GUID == nGuid);
                if (itemToRemove != null)
                {
                    itemContainer.RemoveItem(itemToRemove);
                }
            }
            //物品移除后有剩余
            else
            {
                itemData.Count -= nCounts;
                var itemToUpdate = itemContainer.bagItems.FirstOrDefault(item => item.GUID == nGuid);
                if (itemToUpdate != null)
                {
                    itemToUpdate.Count -= nCounts;
                }
            }
            BagBoxChange?.Invoke(itemData.Guid, itemData);
            return true;
        }
        public uint GetGuidByItemID(uint nItemID)
        {
            List<OpenNGS.Item.Common.ItemData> itemData = GetItemDataByItemId(nItemID);
            if (itemData != null && itemData.Count > 0)
            {
                return itemData[0].Guid;
            }
            return 0;
        }
        public uint GetItemCountByGuidID(uint nGuid)
        {
            OpenNGS.Item.Common.ItemData itemData = GetItemDataByGuid(nGuid);
            if(itemData == null)
            {
                return 0;
            }
            return itemData.Count;
        }

        public uint GetItemTotalCountByItemID(uint itemID)
        {
            uint sum = 0;
            List<ItemData> items = GetItemDataByItemId(itemID);
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

        public void ItemInBag(uint nGuid)
        {
            var changeitem = itemContainer.stashItems.FirstOrDefault(item => item.GUID == nGuid);
            if (changeitem != null)
            {
                itemContainer.RemoveStashItem(changeitem);
                itemContainer.AddItem(changeitem);
            }
        }
        public void ItemOutBag(uint nGuid)
        {
            var changeitem = itemContainer.bagItems.FirstOrDefault(item => item.GUID == nGuid);
            if (changeitem != null)
            {
                itemContainer.AddStashItem(changeitem);
                itemContainer.RemoveItem(changeitem);
            }
        }
        public OpenNGS.Item.Common.EQUIP_RESULT_TYPE Equipped(uint index, uint nGuid)
        {
            //if (index >= itemContainer.equips.Count || !IsEnoughByGuid(nGuid, 1))
            //{
            //    return EQUIP_RESULT_TYPE.EQUIP_RESULT_TYPE_ERROR;
            //}
            OpenNGS.Item.Common.ItemData itemData = GetItemDataByGuid(nGuid);
            if (itemData == null || itemData.Count < 1)
            {
                return EQUIP_RESULT_TYPE.EQUIP_RESULT_TYPE_ERROR;
            }
            var itemToRemove = itemContainer.bagItems.FirstOrDefault(item => item.GUID == nGuid);
            if (itemToRemove != null)
            {
                itemContainer.RemoveItem(itemToRemove);
                equips equipDict = new equips();
                equipDict.equip = itemToRemove;
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
                itemContainer.AddItem(equippedItem.equip);
            }
            return EQUIP_RESULT_TYPE.EQUIP_RESULT_TYPE_SUCCESS;
        }

        public List<equips> GetEquippedList()
        {
            return itemContainer.equipDict ;
        }
        public void AddAction_stashChange(Action<uint, OpenNGS.Item.Common.ItemData> ac)
        {
            StashBoxChange += ac;
        }
        //添加道具栏变更事件
        public void AddAction_bagChange(Action<uint, OpenNGS.Item.Common.ItemData> ac)
        {
            BagBoxChange += ac;
        }
        //添加装备栏变更事件
        public void AddAction_equipChange(Action<uint, OpenNGS.Item.Common.ItemData> ac)
        {
            EquipBoxChange += ac;
        }
        public void RemoveAction_stashChange(Action<uint, OpenNGS.Item.Common.ItemData> ac)
        {
            BagBoxChange -= ac;
        }
        //删去道具栏变更事件
        public void RemoveAction_bagChange(Action<uint, OpenNGS.Item.Common.ItemData> ac)
        {
            BagBoxChange -= ac;
        }
        //删去装备栏变更事件
        public void RemoveAction_equipChange(Action<uint,OpenNGS.Item.Common.ItemData> ac)
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