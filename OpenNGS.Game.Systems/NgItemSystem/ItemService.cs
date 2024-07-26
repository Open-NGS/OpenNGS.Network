using OpenNGS.Item.Data;
using OpenNGS.Item.Service;
using OpenNGS.Systems;
using OpenNGS;
using System.Collections.Generic;
using OpenNGS.Item.Common;

public class ItemService : Singleton<ItemService>
{
    IItemClientAPI ngItemClientSystem;
    public void Init(IItemClientAPI itemClientAPI)
    {
        ngItemClientSystem = itemClientAPI;
    }
    public AddItemRsp AddItemsByID(AddItemReq _req)
    {
        return ngItemClientSystem.AddItemsByID(_req);
    }

    public AddItemRsp RemoveItemsByGrid(RemoveItemReq _req)
    {
        return ngItemClientSystem.RemoveItemsByGrid(_req);
    }

    public AddItemRsp ExchangeGrid(ChangeItemData _changeItemData)
    {
        return ngItemClientSystem.ExchangeGrid(_changeItemData);
    }

    public AddItemRsp SortItems(uint nCol)
    {
        return ngItemClientSystem.SortItems(nCol);
    }

    public List<ItemSaveState> GetItemDatasByColIdx(uint nColIdx)
    {
        return ngItemClientSystem.GetItemDatasByColIdx(nColIdx);
    }

    public void AddItemContainer(ItemContainer Container)
    {
        ngItemClientSystem.AddItemContainer(Container);
    }


    public ItemResultType CanAddItem(AddReq _req)
    {
        throw new System.NotImplementedException();
    }

    public AddItemRsp AddItems(AddReq _req)
    {
        throw new System.NotImplementedException();
    }

    public ItemResultType CanRemoveItemByID(RemoveItemsByIDsReq _req)
    {
        throw new System.NotImplementedException();
    }

    public AddItemRsp RemoveItemByID(RemoveItemsByIDsReq _req)
    {
        throw new System.NotImplementedException();
    }

    public ItemResultType CanRemoveItemByGrid(RemoveItemsByGridsReq _req)
    {
        throw new System.NotImplementedException();
    }

    public AddItemRsp RemoveItemByGrid(RemoveItemsByGridsReq _req)
    {
        throw new System.NotImplementedException();
    }
}