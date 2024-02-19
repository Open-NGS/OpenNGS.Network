using OpenNGS.Item.Data;
using OpenNGS.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenNGS.Suit.Data;
using OpenNGS.Item.Common;
using OpenNGS;
using Systems;
using OpenNGS.Exchange.Data;

public class EquipSystem : GameSubSystem<EquipSystem>, IEquipSystem
{
    //装备背包（未装备）列表
    List<OpenNGS.Item.Common.ItemData> EquipInventory;
    //材料列表
    List<OpenNGS.Item.Common.ItemData> CraftInventory;
    //装备(可装备)列表
    List<OpenNGS.Item.Common.ItemData> EquipItems = new List<OpenNGS.Item.Common.ItemData>();
    //传给交易系统
    List<SourceItem> sourcesList = new List<SourceItem>();
    List<TargetItem> targetsList = new List<TargetItem>();
    private IItemSystem m_itemSys = null;
    private IMakeSystem m_makeSystem = null;
    private IExchangeSystem m_exchangeSystem = null;

    protected override void OnCreate()
    {
        m_itemSys = App.GetService<IItemSystem>();
        m_makeSystem = App.GetService<IMakeSystem>();
        m_exchangeSystem=App.GetService<IExchangeSystem>();
        base.OnCreate();
    }

    public void GetItemInfo()//从ItemSystem获得数据
    {
        EquipInventory = m_itemSys.GetItemInfos(ITEM_TYPE.ITEM_TYPE_EQUIP);//获得装备库存数据
        CraftInventory = m_itemSys.GetItemInfos(ITEM_TYPE.ITEM_TYPE_CRAFT);//获得材料库存数据
    }

    ///// <summary>
    ///// 使用装备
    ///// </summary>
    ///// <param name="GridIndex">格子ID</param>
    //public void EquipItem(uint GridIndex)
    //{
    //    EquipItems.Add(EquipInventory[(int)GridIndex]);
    //}

    ///// <summary>
    ///// 卸下装备
    ///// </summary>
    ///// <param name="GridIndex">格子ID</param>
    ///// <returns></returns>
    //public bool UnEquipItem(uint GridIndex)
    //{
    //    return EquipItems.Remove(EquipItems[(int)GridIndex]);

    //}

    //public List<OpenNGS.Item.Common.ItemData> GetEquipList()//获得当前装备列表
    //{
    //    return EquipItems;
    //}

 

    /// <summary>
    /// 制作武器装备
    /// </summary>
    /// <param name="GridIndex">格子ID</param>
    public void MakeEquip(uint GridIndex)
    {
        uint guid = CraftInventory[(int)GridIndex].Guid;
        //m_makeSystem.Forged(guid);
    }

    /// <summary>
    /// 分解装备
    /// </summary>
    /// <param name="GridIndex">格子ID</param>
    public void DisassembleEquip(uint GridIndex)
    {
        SourceItem source=new SourceItem();
        TargetItem target=new TargetItem();
        uint guid = EquipInventory[(int)GridIndex].Guid;
        //根据guidID找到要分解的装备
        source.Count = m_itemSys.GetItemDataByGuid(guid).Count;
        source.GUID = guid;
        sourcesList.Add(source);
        //根据装备确定返回材料
        target.ItemID = EquipInventory[(int)GridIndex].ItemID;
        target.Count=m_itemSys.GetDisassembleEquipIno(target.ItemID).MaterialNum;
        targetsList.Add(target);
        m_exchangeSystem.ExchangeItem(sourcesList, targetsList);
    }


    /// <summary>
    /// 传入套装ID判断是否已达成触发条件
    /// </summary>
    /// <param name="suitDataID">套装ID</param>
    /// <returns></returns>
    public bool JudgeSuit(uint suitDataID)
    {
        SuitData suitData = m_itemSys.GetSuitData(suitDataID);
        uint[] EquipIDs = suitData.ConsistEquipID;
        List<OpenNGS.Item.Common.ItemData> SuitEquips = new List<OpenNGS.Item.Common.ItemData>();//存储组成套装需要的装备
        for (uint i=0,j = 0; i < EquipIDs.Length; i++)
        {
            if (EquipItems[(int)i].ItemID == EquipIDs[j])
            {
                j++;
                //SuitEquips.Add(m_itemSys.GetItemDataByItemId(EquipIDs[i]));
            }
        }
        if (EquipIDs.Length == SuitEquips.Count)
        {
            return true;
        }
        else { return false; }
    }
    public override string GetSystemName()
    {
        return "com.openngs.system.EquipSystem";
    }
}
