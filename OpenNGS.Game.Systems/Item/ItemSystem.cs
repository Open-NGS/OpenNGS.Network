using Newtonsoft.Json;
using OpenNGS.Systems;
using System;
using System.Collections.Generic;
using UnityEngine;
using OpenNGS.Item;
using OpenNGS.Item.Data;

namespace OpenNGS.Systems
{
    public class ItemSystem : EntitySystem
    {
        public List<OpenNGS.Item.Common.ItemData> ItemList = new();
        public Action OnNotifyItemListChange;
        public Action OnPlacementChange;

        public Dictionary<ulong, long> TempPlaceDic = new Dictionary<ulong, long>();
        public Dictionary<ulong, long> EternalPlaceDic = new Dictionary<ulong, long>();

        private bool m_IsCreate;
        public List<int> m_Filters = new() { 0, 1, 2, 3 };

        private ulong m_Uin;
        private bool m_IsNewPlayer;

        public void Init(ulong uin, bool isNewPlayer)
        {
            m_Uin = uin;
            m_IsNewPlayer = isNewPlayer;
        }



        protected override void OnCreate()
        {
            base.OnCreate();
            m_IsCreate = false;
            GetItems();
        }

        public OpenNGS.Item.Common.ItemData GetItemData(ulong uid)
        {
            return ItemList.Find(x => x.Guid == uid);
        }

        public OpenNGS.Item.Common.ItemData GetItemDataByItemId(uint itemId)
        {
            return ItemList.Find(x => x.ItemID == itemId);
        }

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

        private void GetItems()
        {
            Debug.Log($"{nameof(ItemSystem)} GetItems");
            //ItemService.Instance.GetItemRequest(m_Uin).ContinueWith((rsp) =>
            //{
            //    OnGetItemResponse(rsp.Result);
            //});
        }

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

    }

}