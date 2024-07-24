using OpenNGS.Item.Data;
using OpenNGS.Item.Service;
using OpenNGS.Systems;
using OpenNGS;
using System.Collections.Generic;
using OpenNGS.Item.Common;

public class ItemService : INgItemSystem
{
    INgItemSystem ngItemSystem = App.GetService<INgItemSystem>();
    private static ItemService _instance;
    private static readonly object _lock = new object();

    private ItemService() { }

    public static ItemService Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new ItemService();
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
        throw new System.NotImplementedException();
    }

    public ItemResultType CanAddItem(AddReq _req)
    {
        throw new System.NotImplementedException();
    }

    public AddItemRsp AddItems(AddReq _req)
    {
        throw new System.NotImplementedException();
    }

    public AddItemRsp RemoveItems(RemoveReq _req)
    {
        throw new System.NotImplementedException();
    }
}