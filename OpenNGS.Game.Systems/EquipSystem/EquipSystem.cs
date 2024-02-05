using OpenNGS.Item.Data;
using OpenNGS.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenNGS.Suit.Data;
using OpenNGS.Item.Common;
using OpenNGS;

public class EquipSystem : EntitySystem, IEquipSystem
{
    //装备背包（未装备）列表
    List<OpenNGS.Item.Data.Item> EquipInventory;
    //装备列表
    List<OpenNGS.Item.Data.Item> EquipItems = new List<OpenNGS.Item.Data.Item>();
    private IItemSystem m_itemSys = null;
    private IMakeSystem m_makeSystem = null;
    public override void InitSystem()
    {
        m_itemSys = App.GetService<IItemSystem>();
        m_makeSystem=App.GetService<IMakeSystem>();
    }

    protected override void OnCreate()
    {
        base.OnCreate();
    }

    public void GetItemInfo()//从ItemSystem获得数据
    {
        EquipInventory = m_itemSys.GetItemInfos(ITEM_TYPE.ITEM_TYPE_EQUIP);//获得装备库存数据
        
    }

    /// <summary>
    /// 使用装备
    /// </summary>
    /// <param name="ItemIndex">装备ID</param>
    public void EquipItem(uint ItemIndex)
    {
        EquipItems.Add(EquipInventory[(int)ItemIndex]);
    }

    /// <summary>
    /// 卸下装备
    /// </summary>
    /// <param name="ItemIndex">装备ID</param>
    /// <returns></returns>
    public bool UnEquipItem(uint ItemIndex)
    {
        return EquipItems.Remove(EquipItems[(int)ItemIndex]);

    }

    public List<OpenNGS.Item.Data.Item> GetEquipList()//获得当前装备列表
    {
        return EquipItems;
    }

    /// <summary>
    /// 根据装备ID获得装备信息
    /// </summary>
    /// <param name="EquipDataID">装备ID</param>
    /// <returns></returns>
    public OpenNGS.Item.Data.Item GetEquip(uint EquipDataID)
    {
        return EquipInventory.Find(t=>t.Id==EquipDataID);
    }

    /// <summary>
    /// 制作武器装备
    /// </summary>
    /// <param name="itemIndex">装备ID</param>
    public void MakeEquip(uint itemIndex)
    {
        m_makeSystem.Forged(itemIndex - 1, EquipInventory[(int)itemIndex - 1]);
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
        List<OpenNGS.Item.Data.Item> SuitEquips = new List<OpenNGS.Item.Data.Item>();//存储组成套装需要的装备
        for (uint i = 0; i < EquipIDs.Length; i++)
        {
            if (EquipItems.Contains(EquipItems[(int)EquipIDs[i]]))
            {
                SuitEquips.Add(GetEquip(EquipIDs[i]));
            }
        }
        if (EquipIDs.Length == SuitEquips.Count)
        {
            return true;
        }
        else { return false; }
    }
}
