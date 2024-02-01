using OpenNGS.Make.Data;
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
    public OpenNGS.Item.Data.Item GetEquip(uint EquipDataID);
    public List<OpenNGS.Item.Data.Item> GetEquipList();
    //public int AttributeCalculate(OpenNGS.Item.Data.Item EquipData, int CharacterData);//角色属性需修改
    //public void RefreshEquipUI(OpenNGS.Item.Data.Item EquipData);
}
