using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipSystem 
{
    //使用装备
    public void EquipItem(uint ItemIndex);
    //卸下装备
    public bool UnEquipItem(uint ItemIndex);
    //获取装备数据
    public List<OpenNGS.Item.Common.ItemData> GetEquipList();
}
