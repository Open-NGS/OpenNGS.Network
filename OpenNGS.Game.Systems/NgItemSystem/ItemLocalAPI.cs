using OpenNGS;
using OpenNGS.Item.Common;
using OpenNGS.Item.Data;
using OpenNGS.Item.Service;
using OpenNGS.Systems;
using System;
using System.Collections.Generic;

public class ItemLocalAPI : Singleton<ItemLocalAPI>,INgItemSystem
{
    INgItemSystem ngItemSystem=App.GetService<INgItemSystem>();

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
