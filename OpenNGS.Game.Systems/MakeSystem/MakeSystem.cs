using OpenNGS.Exchange.Common;
using OpenNGS.Exchange.Data;
using OpenNGS.Make.Data;
using OpenNGS.Systems;
using System.Collections.Generic;

public class MakeSystem : EntitySystem
{
    ExchangeSystem exchangeSystem = new ExchangeSystem();

    List<SourceItem> sourcesList = new List<SourceItem>();
    List<TargetItem> targetsList = new List<TargetItem>();

    private IItemSystem m_itemSys = null;
    public void RegisteItemSystem(IItemSystem _itemSys)
    {
        m_itemSys = _itemSys;
    }

    /// <summary>
    /// 制作
    /// </summary>
    /// <param name="makeGridId">格子ID</param>
    /// <param name="item">道具ID</param>
    public EXCHANGE_RESULT_TYPE Forged(uint makeGridId,ItemInfo item)
    {
        SourceItem sources = null;
        TargetItem targets = null;
        ItemInfo itemInfo;
        MakeInfo makeInfo;

        sourcesList.Clear();
        targetsList.Clear();

<<<<<<< HEAD
        //itemInfo = NGSStaticData.items.GetItem(item.ID);
        //makeInfo = NGSStaticData.makes.GetItem(item.ID);
        //// 制作书
        //sources.GUID = makeGridId;
        //sources.Count = itemInfo.StackMax;
        //sourcesList.Add(sources);
        //// 材料
        //foreach (var mater in makeInfo.Materials)
        //{
        //    uint guid = m_itemSys.GetItemCountByGuidID(mater.ID);
        //    sources.GUID = guid;
        //    sources.Count = mater.StackMax;
        //    sourcesList.Add(sources);
        //}
        //foreach (var items in makeInfo.ItemID)
        //{
        //    targets.ItemID = items.ID;
        //    targets.Count = items.StackMax;
        //    targetsList.Add(targets);
        //}
=======
        itemInfo = m_itemSys.GetItemInfo(item.ID);
        makeInfo = m_itemSys.GetItemByItmes(item.ID);
        // 制作书
        sources.GUID = makeGridId;
        sources.Count = itemInfo.StackMax;
        sourcesList.Add(sources);
        // 材料
        foreach (var mater in makeInfo.Materials)
        {
            uint guid = m_itemSys.GetItemCountByGuidID(mater.ID);
            sources.GUID = guid;
            sources.Count = mater.StackMax;
            sourcesList.Add(sources);
        }
        foreach (var items in makeInfo.ItemID)
        {
            targets.ItemID = items.ID;
            targets.Count = items.StackMax;
            targetsList.Add(targets);
        }
>>>>>>> 71f5ca0148fbed141d2b8cbd4766b3a3ad5e18b9
        return exchangeSystem.ExchangeItem(sourcesList, targetsList);
    }

    protected override void OnCreate()
    {
        base.OnCreate();
    }

    public override string GetSystemName()
    {
        return "com.openngs.system.Make";
    }
}
