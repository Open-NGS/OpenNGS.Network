using OpenNGS.Exchange.Common;
using OpenNGS.Exchange.Data;
using OpenNGS.Make.Data;
using OpenNGS.Systems;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting.YamlDotNet.Core;
using UnityEditor;
using UnityEngine.Events;

public class MakeSystem : EntitySystem
{
    public Dictionary<uint,MakeInfo>MakeInfos;
    public Dictionary<uint,ItemInfo> ItemInfos;
    public UnityAction<MakeInfo> MakeInfoAction;

    ExchangeSystem exchangeSystem = new ExchangeSystem();

    List<SourceItem> sourcesList;
    List<TargetItem> targetsList;

    private IItemSystem m_itemSys = null;
    public void RegisteItemSystem(IItemSystem _itemSys)
    {
        m_itemSys = _itemSys;
    }

    // 获取制作数据
    public void GetMakeInfo(Dictionary<uint, MakeInfo> makeInfo, Dictionary<uint, ItemInfo> itemInfo)
    {
        MakeInfos = makeInfo;
        ItemInfos = itemInfo;
    }

    // 制作准备
    public void MakePrepare(uint Makeid)
    {
        SourceItem sources = null;
        TargetItem targets = null;

        ItemInfo itemInfo;
        MakeInfo makeInfo = null;

        sourcesList.Clear();
        targetsList.Clear();

        if (ItemInfos.TryGetValue(Makeid, out itemInfo))
        {
            // 制作书
            uint makeGuid = m_itemSys.GetGuidByItemID(itemInfo.ID);
            sources.GUID = makeGuid;
            sources.Count = itemInfo.Number;
            sourcesList.Add(sources);
            // 材料
            if(MakeInfos.TryGetValue(Makeid, out makeInfo))
            {
                foreach (var item in makeInfo.Materials)
                {
                    uint guid = m_itemSys.GetGuidByItemID(item.ID);
                    sources.GUID = guid;
                    sources.Count = item.Number;
                    sourcesList.Add(sources);
                }
                foreach (var item in makeInfo.ItemID)
                {
                    targets.ItemID = item.ID;
                    targets.Count = item.Number;
                    targetsList.Add(targets);
                }
            }
        }
        MakeInfoAction.Invoke(makeInfo);
    }

    // 制作
    public EXCHANGE_RESULT_TYPE Forged()
    {
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
