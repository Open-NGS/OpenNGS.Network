using OpenNGS;
using OpenNGS.Exchange.Common;
using OpenNGS.Exchange.Data;
using OpenNGS.Item.Data;
using OpenNGS.Systems;
using System;
using System.Collections.Generic;
using Systems;

public class MakeSystem : GameSubSystem<MakeSystem>, IMakeSystem
{
    List<SourceItem> sourcesList = new List<SourceItem>();
    List<SourceItem> sourcesMaterList = new List<SourceItem>();
    List<TargetItem> targetsList = new List<TargetItem>();

    public IExchangeSystem ExchangeSystem = null;
    MakeDesign makeMaterial = null;
    float Probability = 0;

    protected override void OnCreate()
    {
        ExchangeSystem = App.GetService<IExchangeSystem>();
        base.OnCreate();
    }

    /// <summary>
    /// 制作
    /// </summary>
    /// <returns></returns>
    public EXCHANGE_RESULT_TYPE Make()
    {
        Random r = new Random();
        int number = r.Next(1,10);
        // 概率条件成功进入
        //if (number <= ((makeMaterial.Probability + Probability) * 10))
        //{
        //    ExchangeSystem.ExchangeItem(sourcesList, targetsList);
        //    EXCHANGE_RESULT_TYPE _TYPE = ExchangeSystem.ExchangeItem(sourcesMaterList,null);
        //    ClearList();
        //    return _TYPE;
        //}
        //返还材料比例
        for (int i = 0; i < sourcesMaterList.Count; i++)
        {
            int probability = (int)(makeMaterial.BackPercent * 10);
            float numb = (this.sourcesMaterList[i].Count * probability)*1.0f / 10;
            uint value = (uint)Math.Floor(numb);
            uint num = makeMaterial.MaterialCount - value;
            this.sourcesMaterList[i].Count = num;
        }
        ExchangeSystem.ExchangeItem(sourcesMaterList, null);
        ExchangeSystem.ExchangeItem(sourcesList, null);
        ClearList();
        return EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NONE;
    }

    // 材料
    public void MakeMaterials(OpenNGS.Item.Common.ItemData itemData)
    {
        SourceItem sourcesMakeBook = new SourceItem();
        sourcesMakeBook.GUID = itemData.Guid;
        sourcesMakeBook.Count = itemData.Count;
        this.sourcesMaterList.Add(sourcesMakeBook);
    }

    // 图纸
    public void MakeDesign(OpenNGS.Item.Common.ItemData itemInfo)
    {
        SourceItem sourcesMakeBook = new SourceItem();
        sourcesMakeBook.GUID = itemInfo.Guid;
        sourcesMakeBook.Count = itemInfo.Count;
        this.sourcesList.Add(sourcesMakeBook);

        makeMaterial = NGSStaticData.MakeItems.GetItem(itemInfo.ItemID);// 图纸
        OpenNGS.Item.Data.Item Target = NGSStaticData.items.GetItem(makeMaterial.TargetItemID);

        // 目标物品
        TargetItem target = new TargetItem();
        target.ItemID = Target.Id;
        target.Count = 1;
        targetsList.Add(target);
    }

    // 幸运石
    public void LuckyStone(OpenNGS.Item.Common.ItemData itemInfo)
    {
        LuckyStone item = NGSStaticData.LuckyStone.GetItem(itemInfo.ItemID);
        Probability += item.UpProbability * itemInfo.Count;

        SourceItem sources = new SourceItem();
        sources.GUID = itemInfo.Guid;
        sources.Count = itemInfo.Count;
        sourcesList.Add(sources);
    }
    private void ClearList()
    {
        sourcesList.Clear();
        targetsList.Clear();
        sourcesMaterList.Clear();
    }
    public override string GetSystemName()
    {
        return "com.openngs.system.Make";
    }
}
