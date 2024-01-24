using OpenNGS.Exchange.Common;
using OpenNGS.Exchange.Data;
using OpenNGS.Make.Data;
using OpenNGS.Systems;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Events;

public class MakeSystem : EntitySystem
{
    public Dictionary<uint,MakeInfo>MakeInfos;
    public Dictionary<uint,ItemInfo> MaterialsInfo;
    public UnityAction<MakeInfo> MakeInfoAction;
    public MakeInfo makeInfo;

    ExchangeSystem exchangeSystem = new ExchangeSystem();

    private IItemSystem m_itemSys = null;
    public void RegisteItemSystem(IItemSystem _itemSys)
    {
        m_itemSys = _itemSys;
    }

    SourceItem sources;
    List<SourceItem> sourcesList;

    TargetItem targets;
    List<TargetItem> targetsList;
    public void Init()
    {
        MakeInfos = Table<MakeInfo, uint>.map;
        MaterialsInfo = Table<ItemInfo, uint>.map;
    }

    // 获得制作材料
    public void GetMakeInfo(uint id)
    {
        MakeInfos.TryGetValue(id,out makeInfo);
        foreach (var item in makeInfo.Materials)
        {
            uint guid = m_itemSys.GetGuidByItemID(makeInfo.ID);
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
        MakeInfoAction.Invoke(makeInfo);
    }

    // 制作
    public void Forged()
    {
        EXCHANGE_RESULT_TYPE type = exchangeSystem.ExchangeItem(sourcesList, targetsList);

        switch (type)
        {
            case EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NONE:
                break;
            case EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_SUCCESS:
                // UIManager.Instance.Open("制作成功");
                break;
            case EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOCOUNT:
                // UIManager.Instance.Open("数量不足"); 
                break;
            case EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOTARGET:
                // UIManager.Instance.Open("没有目标物品"); 
                break;
            default:
                break;
        }
    }
  
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    public override string GetSystemName()
    {
        return "com.openngs.system.rank";
    }
}
