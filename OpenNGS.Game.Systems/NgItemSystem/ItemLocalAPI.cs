using OpenNGS;
using OpenNGS.Item.Common;
using OpenNGS.Item.Data;
using OpenNGS.Item.Service;
using OpenNGS.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLocalAPI : INgItemSystem
{
    INgItemSystem ngItemSystem=App.GetService<INgItemSystem>();
    private static ItemLocalAPI _instance;
    private static readonly object _lock = new object();

    private ItemLocalAPI() { }

    public static ItemLocalAPI Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new ItemLocalAPI();
                }
                return _instance;
            }
        }
    }

    public AddItemRsp AddItemsByID(AddItemReq _req)
    {
        return ngItemSystem.AddItemsByID(_req);
    }

    public AddItemRsp RemoveItemsByGrid(RemoveItemReq _req)
    {
        return ngItemSystem.RemoveItemsByGrid(_req);
    }

    public AddItemRsp ExchangeGrid(ChangeItemData _changeItemData)
    {
        return ngItemSystem.ExchangeGrid(_changeItemData);
    }

    public AddItemRsp SortItems(uint nCol)
    {
        return ngItemSystem.SortItems(nCol);
    }

    public List<ItemSaveState> GetItemDatasByColIdx(uint nColIdx)
    {
        return ngItemSystem.GetItemDatasByColIdx(nColIdx);
    }

    public void AddItemContainer(ItemContainer Container)
    {
        ngItemSystem.AddItemContainer(Container);
    }

    public ItemResultType CanRemoveItem(RemoveReq _req)
    {
        throw new NotImplementedException();
    }

    public ItemResultType CanAddItem(AddReq _req)
    {
        throw new NotImplementedException();
    }

    public AddItemRsp AddItems(AddReq _req)
    {
        throw new NotImplementedException();
    }

    public AddItemRsp RemoveItems(RemoveReq _req)
    {
        throw new NotImplementedException();
    }
}
