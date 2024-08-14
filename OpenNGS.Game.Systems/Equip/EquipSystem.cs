using OpenNGS.Item.Data;
using OpenNGS.Systems;
using System.Collections;
using System.Collections.Generic;
using OpenNGS.Suit.Data;
using OpenNGS.Item.Common;
using OpenNGS;
using Systems;
using OpenNGS.Exchange.Data;
using Common;
using UnityEngine.Windows.Speech;
using OpenNGS.Exchange.Common;

public class EquipSystem : GameSubSystem<EquipSystem>, IEquipSystem
{
    //装备背包（未装备）列表
    List<OpenNGS.Item.Data.ItemSaveState> EquipInventory;
    //材料列表
    List<OpenNGS.Item.Data.ItemSaveState> CraftInventory;
    //图纸列表
    List<OpenNGS.Item.Data.ItemSaveState> BlueprintList;
    //材料列表
    List<OpenNGS.Item.Data.ItemSaveState> CraftList;
    //幸运石列表
    List<OpenNGS.Item.Data.ItemSaveState> LuckyStoneList;
    //装备(可装备)列表
    List<OpenNGS.Item.Data.ItemSaveState> EquipItems = new List<OpenNGS.Item.Data.ItemSaveState>();
    //传给交易系统
    List<SourceItem> sourcesList = new List<SourceItem>();
    List<TargetItem> targetsList = new List<TargetItem>();
    //private IItemSystem m_itemSys = null;
    //private IMakeSystem m_makeSystem = null;
    //private IExchangeSystem m_exchangeSystem = null;

    protected override void OnCreate()
    {
        //m_itemSys = App.GetService<IItemSystem>();
        //m_makeSystem = App.GetService<IMakeSystem>();
        //m_exchangeSystem=App.GetService<IExchangeSystem>();
        base.OnCreate();
    }

    ////通过二级分类获得数据
    //public List<OpenNGS.Item.Data.ItemSaveData> GetItemInfoByType(OpenNGS.Item.Common.ITEM_TYPE iTEM_TYPE)
    //{
    //    if(iTEM_TYPE== ITEM_TYPE.ITEM_TYPE_EQUIP)
    //    {
    //        EquipInventory = m_itemSys.GetItemInfos(ITEM_TYPE.ITEM_TYPE_EQUIP);//获得装备库存数据
    //        return EquipInventory;
    //    }
    //    else if(iTEM_TYPE == ITEM_TYPE.ITEM_TYPE_CRAFT)
    //    {
    //        CraftInventory = m_itemSys.GetItemInfos(ITEM_TYPE.ITEM_TYPE_CRAFT);//获得材料库存数据
    //        return CraftInventory;
    //    }
    //    else { return null; }
    //}

    ////通过三级分类获得数据
    //public List<OpenNGS.Item.Data.ItemSaveData> GetItemInfoByKind(OpenNGS.Item.Common.ITEM_KIND iTEM_KIND)
    //{
    //    if (iTEM_KIND == ITEM_KIND.ITEM_KIND_MATERIAL_BLUEPRINT)
    //    {
    //        BlueprintList = m_itemSys.GetItemInfoByKind(ITEM_KIND.ITEM_KIND_MATERIAL_BLUEPRINT);//获得图纸数据
    //        return BlueprintList;
    //    }
    //    else if (iTEM_KIND == ITEM_KIND.ITEM_KIND_MATERIAL_STUFF)
    //    {
    //        CraftList = m_itemSys.GetItemInfoByKind(ITEM_KIND.ITEM_KIND_MATERIAL_STUFF);//获得材料库存数据
    //        return CraftList;
    //    }
    //    else if (iTEM_KIND == ITEM_KIND.ITEM_KIND_MATERIAL_STONE)
    //    {
    //        LuckyStoneList = m_itemSys.GetItemInfoByKind(ITEM_KIND.ITEM_KIND_MATERIAL_STONE);//获得幸运石数据
    //        return LuckyStoneList;
    //    }
    //    else { return null; }
    //}
    

 

    ///// <summary>
    ///// 制作武器
    ///// </summary>
    ///// <param name="keyValuePairs">图纸、材料、幸运石字典</param>
    ///// <returns></returns>
    //public bool MakeEquip(Dictionary<ITEM_KIND,ItemData> keyValuePairs)
    //{
    //    foreach(var kv in keyValuePairs)
    //    {
    //        switch (kv.Key)
    //        {
    //            case ITEM_KIND.ITEM_KIND_MATERIAL_BLUEPRINT:
    //                m_makeSystem.MakeDesign(kv.Value);//传入图纸
    //                break;
    //            case ITEM_KIND .ITEM_KIND_MATERIAL_STUFF:
    //                m_makeSystem.MakeMaterials(kv.Value);//传入材料   
    //                break;
    //            case ITEM_KIND.ITEM_KIND_MATERIAL_STONE:
    //                m_makeSystem.LuckyStone(kv.Value);//传入幸运石
    //                break;
    //        }
    //    }
    //    if (m_makeSystem.Make()== EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_SUCCESS)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }

    //}

    ///// <summary>
    ///// 分解装备
    ///// </summary>
    ///// <param name="item">装备ItemData</param>
    //public void DisassembleEquip(ItemData item)
    //{
    //    //SourceItem source=new SourceItem();
        
    //    //DisassembleEquipIno MaterialInfo = NGSStaticData.disassembleEquipIno.GetItem(item.ItemID);
    //    ////将装备复制给目标物体
    //    //source.Count = item.Count;
    //    //source.GUID = item.Guid;
    //    //sourcesList.Add(source);
    //    ////确定分解获得的材料
    //    //for(int i = 0; i < MaterialInfo.MaterialID.Length; i++)
    //    //{
    //    //    TargetItem target = new TargetItem();
    //    //    target.ItemID = MaterialInfo.MaterialID[i];
    //    //    target.Count = MaterialInfo.MaterialNum[i];
    //    //    targetsList.Add(target);  
    //    //}
    //    //m_exchangeSystem.ExchangeItem(sourcesList, targetsList);
    //    //sourcesList.Clear();
    //    //targetsList.Clear();
    //}


    ///// <summary>
    ///// 传入套装ID判断是否已达成触发条件
    ///// </summary>
    ///// <param name="suitDataID">套装ID</param>
    ///// <returns></returns>
    //public bool JudgeSuit(uint suitDataID)
    //{
    //    SuitData suitData = m_itemSys.GetSuitData(suitDataID);
    //    uint[] EquipIDs = suitData.ConsistEquipID;
    //    List<OpenNGS.Item.Common.ItemData> SuitEquips = new List<OpenNGS.Item.Common.ItemData>();//存储组成套装需要的装备
    //    for (uint i=0,j = 0; i < EquipIDs.Length; i++)
    //    {
    //        if (EquipItems[(int)i].ItemID == EquipIDs[j])
    //        {
    //            j++;
    //            //SuitEquips.Add(m_itemSys.GetItemDataByItemId(EquipIDs[i]));
    //        }
    //    }
    //    if (EquipIDs.Length == SuitEquips.Count)
    //    {
    //        return true;
    //    }
    //    else { return false; }
    //}
    public override string GetSystemName()
    {
        return "com.openngs.system.EquipSystem";
    }

    public List<ItemSaveState> GetItemInfoByType(ITEM_TYPE iTEM_TYPE)
    {
        throw new System.NotImplementedException();
    }

    public List<ItemSaveState> GetItemInfoByKind(ITEM_KIND iTEM_KIND)
    {
        throw new System.NotImplementedException();
    }

    public bool MakeEquip(Dictionary<ITEM_KIND, ItemData> keyValuePairs)
    {
        throw new System.NotImplementedException();
    }

    public void DisassembleEquip(ItemData item)
    {
        throw new System.NotImplementedException();
    }
}
