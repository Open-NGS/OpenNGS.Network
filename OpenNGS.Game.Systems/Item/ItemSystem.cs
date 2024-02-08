using Newtonsoft.Json;
using OpenNGS.Systems;
using System;
using System.Collections.Generic;
using UnityEngine;
using OpenNGS.Item;
using OpenNGS.Item.Data;
using OpenNGS.Item.Common;
using OpenNGS.Suit.Data;
using Systems;

namespace OpenNGS.Systems
{
    public class ItemSystem : GameSubSystem<ItemSystem>, IItemSystem
    {
        public Action OnNotifyItemListChange;
        public Action OnPlacementChange;

        public Dictionary<ulong, long> TempPlaceDic = new Dictionary<ulong, long>();
        public Dictionary<ulong, long> EternalPlaceDic = new Dictionary<ulong, long>();

        private bool m_IsCreate;
        public List<int> m_Filters = new() { 0, 1, 2, 3 };

        private ulong m_Uin;
        private bool m_IsNewPlayer;

        private ISaveSystem m_saveSystem = null;
        private SaveFileData_Item m_itemData = null;

        private uint guid_cache = 1;
        private Queue<uint> guid_free = new Queue<uint>();

        public void Init(ulong uin, bool isNewPlayer)
        {
            m_Uin = uin;
            m_IsNewPlayer = isNewPlayer;
        }
        protected override void OnCreate()
        {
            m_saveSystem = App.GetService<ISaveSystem>();
            base.OnCreate();
            m_IsCreate = false;
            GetItems();
        }
        //获取存档数据
        public void GetItems()
        {
            ISaveInfo saveInfo = m_saveSystem.GetFileData("ITEM");
            if (saveInfo != null && saveInfo is SaveFileData_Item)
            {
                m_itemData = (SaveFileData_Item)saveInfo;
            }
            else
            {
                m_itemData = new SaveFileData_Item();
            }
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
            foreach(ItemSaveData itemData in m_itemData._items.Values)
            {
                if((ulong)itemData.GUID == uid)
                {
                    OpenNGS.Item.Common.ItemData item = new ItemData();
                    item.Guid = (uint)itemData.GUID;
                    item.ItemID = (uint)itemData.ItemID;
                    item.Count = (uint)itemData.Count;
                    return item;
                }
            }
            return null;
        }

        //获取道具信息(通过itemID)
        public List<OpenNGS.Item.Common.ItemData> GetItemDataByItemId(uint itemId)
        {
            List<OpenNGS.Item.Common.ItemData> itemDatas = new List<ItemData>();
            foreach (ItemSaveData itemData in m_itemData._items.Values)
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
            foreach (ItemSaveData itemInfo in m_itemData._items.Values)
            {
                if (NGSStaticData.items.GetItem(itemInfo.ItemID).ItemType == iTEM_TYPE)
                {
                    OpenNGS.Item.Common.ItemData itemData = new ItemData();
                    itemData.ItemID = (uint)itemInfo.ItemID;
                    itemData.Guid = (uint)itemInfo.GUID;
                    itemData.Count = (uint)itemInfo.Count;
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
        //    Debug.Log($"{nameof(ItemSystem)} GetItems");
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
        //    //    Debug.Log($"{nameof(ItemSystem)} UseItemRequest Success");
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
            //    Debug.Log($"{nameof(ItemSystem)} 找回所有 ReplaceItemsRequest Success");
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

        //    Debug.Log($"{nameof(ItemSystem)} {status.OpCode} OnStatus" + ":" + JsonConvert.SerializeObject(itemLst));

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
        //        Debug.Log($"{nameof(ItemSystem)} 登录清空放置状态");
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
            //新添加的物品
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
                            m_itemData._items[itemData[i].Guid].Count = (int)NGSStaticData.items.GetItem(nItemID).StackMax;
                            nCounts -= volumn;
                        }
                        //该格子能装下
                        else
                        {
                            itemData[i].Count += nCounts;
                            m_itemData._items[itemData[i].Guid].Count += (int)nCounts;
                            nCounts = 0;
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
                    while (m_itemData._items.ContainsKey(guid_cache))
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
                itemSaveData.ItemID = (int)item.ItemID;
                itemSaveData.Count = (int)item.Count;
                m_itemData._items[item.Guid] = itemSaveData;
            }
            //更新动态数据
            m_saveSystem.SetFileData("ITEM", m_itemData);
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
                    guid_free.Enqueue(itemData[i].Guid);//Guid空闲出来后放入队列
                    m_itemData._items.Remove(itemData[i].Guid);
                }
                else
                {
                    itemData[i].Count = itemData[i].Count - nCounts;
                    m_itemData._items[itemData[i].Guid].Count = (int)itemData[i].Count;
                    break;
                }
            }
            //更新动态数据
            m_saveSystem.SetFileData("ITEM", m_itemData);
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
                m_itemData._items.Remove(nGuid);
            }
            //物品移除后有剩余
            else
            {
                m_itemData._items[nGuid].Count = (int)itemData.Count;
            }
            //更新动态数据
            m_saveSystem.SetFileData("ITEM", m_itemData);
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
    }

}