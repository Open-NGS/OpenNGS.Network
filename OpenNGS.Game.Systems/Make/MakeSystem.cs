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
    List<TargetItem> targetsList = new List<TargetItem>();

    public IExchangeSystem ExchangeSystem = null;
    MakeDesign makeMaterial = null;

    protected override void OnCreate()
    {
        ExchangeSystem = App.GetService<IExchangeSystem>();
        sourcesList.Clear();
        targetsList.Clear();
        base.OnCreate();
    }

    /// <summary>
    /// 制作
    /// </summary>
    /// <returns></returns>
    public EXCHANGE_RESULT_TYPE Material()
    {
        Random r = new Random();
        int number = r.Next(1,10);
        // 概率
        if (number >= (makeMaterial.Probability * 10))
        {
            EXCHANGE_RESULT_TYPE _TYPEs = ExchangeSystem.ExchangeItem(sourcesList, targetsList);
            return _TYPEs;
        }
        //返还比例
        this.sourcesList[1].Count = this.sourcesList[1].Count * (1-(uint)makeMaterial.BackPercent);
        ExchangeSystem.ExchangeItem(sourcesList, null);

        return EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NONE;
    }

    // 材料
    public void MakeMaterials(OpenNGS.Item.Common.ItemData itemData)
    {
        SourceItem sourcesMakeBook = new SourceItem();
        sourcesMakeBook.GUID = itemData.Guid;
        sourcesMakeBook.Count = itemData.Count;
        this.sourcesList.Add(sourcesMakeBook);
    }

    // 图纸
    public void MkaeDesign(OpenNGS.Item.Common.ItemData itemInfo)
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
        targetsList.Add(target);
    }
    public override string GetSystemName()
    {
        return "com.openngs.system.Make";
    }
}
