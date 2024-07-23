using OpenNGS.Item.Common;
using System.Collections;
using System.Collections.Generic;

public interface IEquipSystem 
{
    //通过二级分类获取数据
    public List<OpenNGS.Item.Data.ItemSaveData> GetItemInfoByType(OpenNGS.Item.Common.ITEM_TYPE iTEM_TYPE);
    //通过三级级分类获取数据
    public List<OpenNGS.Item.Data.ItemSaveData> GetItemInfoByKind(OpenNGS.Item.Common.ITEM_KIND iTEM_KIND);
    //制作装备
    public bool MakeEquip(Dictionary<ITEM_KIND, ItemData> keyValuePairs);


}
