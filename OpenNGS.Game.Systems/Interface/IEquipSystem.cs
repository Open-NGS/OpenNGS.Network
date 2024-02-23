using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipSystem 
{
    //获取数据
    public void GetItemInfo();
    ////使用装备
    //public void EquipItem(uint ItemIndex);
    ////卸下装备
    //public bool UnEquipItem(uint ItemIndex);
    ////返回已装备列表
    //public List<OpenNGS.Item.Common.ItemData> GetEquipList();
    //制作装备
    public bool MakeEquip(uint GridIndex);
    //分解装备
    public void DisassembleEquip(uint GridIndex);

    public List<OpenNGS.Item.Common.ItemData> GetItemData();
}
