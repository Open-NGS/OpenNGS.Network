using OpenNGS.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using Systems;

public class NgBlindBoxSystem : GameSubSystem<NgBlindBoxSystem>, INgBlindBoxSystem
{
    private OpenNGS.BlindBox.Data.Drop drop;
    private List<OpenNGS.BlindBox.Data.DropRule> dropRules;
    private List<OpenNGS.BlindBox.Data.DropRule> undropedRuleIDs;
    //记录需要手动更改权重的ItemID
    private Dictionary<uint, List<uint>> dic_weightChangeIDs;
    List<uint> dropedRuleIDs;

    private Dictionary<uint, int> dropedItem;
    private int seq = 0;

    protected override void OnCreate()
    {
        base.OnCreate();
        ResetData();
    }

    public void ResetData()
    {
        dic_weightChangeIDs = new Dictionary<uint, List<uint>>();
        dropedRuleIDs = new List<uint>();
        undropedRuleIDs = new List<OpenNGS.BlindBox.Data.DropRule>();
    }

    private void InitDrop(uint dropID)
    {
        drop = NGSStaticData.drops.GetItem(dropID);
        dropRules = new List<OpenNGS.BlindBox.Data.DropRule>();
        dropedItem = new Dictionary<uint, int>();
        if (drop != null)
        {
            for (int i = 0; i < drop.DropRuleIDs.Length; i++)
            {
                dropRules.Add(NGSStaticData.droprules.GetItem(drop.DropRuleIDs[i]));
            }
        }

        if (dic_weightChangeIDs.ContainsKey(dropID))
        {
        }
        else
        {
            dic_weightChangeIDs[dropID] = new List<uint>();
        }
    }
    /// <summary>
    /// 执行掉落
    /// </summary>
    /// <param name="DropID">DropID</param>
    /// <param name="executeCount">执行次数</param>
    /// <returns></returns>
    public Dictionary<uint, int> DoDrop(uint DropID, uint executeCount)
    {
        InitDrop(DropID);
        if (dropRules.Count == 1)
        {
            Random rd = new Random();
            if (rd.Next(0, 10) < dropRules[0].Probability * 10)
            {
                dropedItem = GetCountByWeight(DropID, dropRules[0].DropGroupID, dropRules[0].Multiple, drop.MaxDropNum, executeCount);
            }
        }
        else
        {
            switch (drop.RuleDropType)
            {
                case (int)OpenNGS.BlindBox.Common.Drop_ExcuteType.Drop_ExcuteType_None:
                    break;
                case (int)OpenNGS.BlindBox.Common.Drop_ExcuteType.Drop_ExcuteType_All:
                    AllDrop(executeCount, DropID);
                    break;
                case (int)OpenNGS.BlindBox.Common.Drop_ExcuteType.Drop_ExcuteType_Order:
                    OrderDrop(executeCount, DropID);
                    break;
                case (int)OpenNGS.BlindBox.Common.Drop_ExcuteType.Drop_ExcuteType_Random:
                    RandomDrop(executeCount, DropID);
                    break;
                default: break;
            }
        }

        return dropedItem;
    }
    //全部掉落
    private void AllDrop(uint executeCount, uint nDropID)
    {
        Random rd = new Random();
        foreach (var rule in dropRules)
        {
            if (rd.Next(0, 10) < rule.Probability * 10)
            {
                Dictionary<uint, int> count = GetCountByWeight(nDropID, rule.DropGroupID, rule.Multiple, drop.MaxDropNum, executeCount);
                foreach (var item in count)
                {
                    if (dropedItem.ContainsKey(item.Key))
                    {
                        dropedItem[item.Key] += item.Value;
                    }
                    else
                    {
                        dropedItem[item.Key] = item.Value;
                    }
                }
            }
        }
    }
    //顺序掉落
    private void OrderDrop(uint executeCount, uint nDropID)
    {
        Random rd = new Random();
        if (seq < dropRules.Count)
        {
            if (rd.Next(0, 10) < dropRules[seq].Probability * 10)
            {
                dropedItem = GetCountByWeight(nDropID, dropRules[seq].DropGroupID, dropRules[seq].Multiple, drop.MaxDropNum, executeCount);
                seq++;
            }

            if (seq == dropRules.Count && drop.Loop == true)
            {
                seq = 0;
            }
        }
    }
    //随机掉落
    private void RandomDrop(uint executeCount, uint nDropID)
    {
        Random rd = new Random();
        if (undropedRuleIDs.Count == 0)
        {
            undropedRuleIDs = dropRules;
        }
        int index = rd.Next(0, undropedRuleIDs.Count);
        if (drop.Repeatable == false)
        {
            if (rd.Next(0, 10) < undropedRuleIDs[index].Probability * 10)
            {
                dropedRuleIDs.Add(undropedRuleIDs[index].DropRuleID);
                dropedItem = GetCountByWeight(nDropID, undropedRuleIDs[index].DropGroupID, undropedRuleIDs[index].Multiple, drop.MaxDropNum, executeCount);
                undropedRuleIDs.Remove(undropedRuleIDs[index]);
            }
            if (dropRules.Count == dropedRuleIDs.Count && drop.Loop == true)
            {
                dropedRuleIDs.Clear();
                undropedRuleIDs.Clear();
            }
        }
        else
        {
            if (rd.Next(0, 10) < dropRules[index].Probability * 10)
            {
                dropedRuleIDs.Add(dropRules[index].DropRuleID);
                dropedItem = GetCountByWeight(nDropID, dropRules[index].DropGroupID, dropRules[index].Multiple, drop.MaxDropNum, executeCount);
            }
        }

    }
    //随机数量
    private uint RandomCount(OpenNGS.BlindBox.Data.DropGroup dropGroup, uint Multiple, uint MaxNum)
    {
        Random rd = new Random();
        uint num = 0;
        if (dropGroup.ItemCountMin == dropGroup.ItemCountMax)
        {
            if (MaxNum <= dropGroup.ItemCountMin * Multiple)
            {
                num = MaxNum;
            }
            else
            {
                num = dropGroup.ItemCountMin * Multiple;
            }
        }
        else
        {
            num = (uint)rd.Next((int)dropGroup.ItemCountMin, (int)dropGroup.ItemCountMax + 1);
            if (MaxNum <= num * Multiple)
            {
                num = MaxNum;
            }
            else { num = num * Multiple; }
        }
        return num;
    }
    //根据权重返回ItemID,数量字典
    private Dictionary<uint, int> GetCountByWeight(uint nDropID, uint GroupID, uint Multiple, uint MaxNum, uint exCount)
    {
        Random rd = new Random();
        uint allWeight = 0;
        int index = 0;
        List<uint> isdropedID = new List<uint>();
        Dictionary<uint, int> dic_itemCounts = new Dictionary<uint, int>();
        List<OpenNGS.BlindBox.Data.DropGroup> dropGroups = NGSStaticData.dropgroups.GetItems(GroupID);
        if (dic_weightChangeIDs.ContainsKey(nDropID) == false)
        {
            return dic_itemCounts;
        }
        for (int exTimes = 0; exTimes < exCount; exTimes++)
        {
            foreach (var dropGroup in dropGroups)
            {
                if (isdropedID.Contains(dropGroup.DropItemID) || dic_weightChangeIDs[nDropID].Contains(dropGroup.DropItemID))
                {
                    allWeight += (uint)(dropGroup.Weight + dropGroup.WeightInc);

                }
                else
                {
                    allWeight += dropGroup.Weight;
                }
            }
            if (allWeight == 0)
            {
                break;
            }
            int randomNum = rd.Next(0, (int)allWeight);
            for (int i = 0; i < dropGroups.Count; i++)
            {
                if (isdropedID.Contains(dropGroups[i].DropItemID) || dic_weightChangeIDs[nDropID].Contains(dropGroups[i].DropItemID))
                {
                    randomNum -= ((int)dropGroups[i].Weight + dropGroups[i].WeightInc);
                }
                else
                {
                    randomNum -= (int)dropGroups[i].Weight;
                }
                if (randomNum < 0)
                {
                    index = i; break;
                }
            }
            dic_itemCounts[dropGroups[index].DropItemID] = (int)RandomCount(dropGroups[index], Multiple, MaxNum);
            isdropedID.Add(dropGroups[index].DropItemID);
            allWeight = 0;
        }

        return dic_itemCounts;
    }
    //改变权重
    public void ChangeWeight(uint nDropID, uint ItemID)
    {
        if (dic_weightChangeIDs.ContainsKey(nDropID))
        {
            if (dic_weightChangeIDs[nDropID].Contains(ItemID) == false)
            {
                dic_weightChangeIDs[nDropID].Add(ItemID);
            }
        }
    }
    //重置权重
    public void ResetWeight(uint nDropID)
    {
        if (dic_weightChangeIDs.ContainsKey(nDropID))
        {
            dic_weightChangeIDs[nDropID].Clear();
        }
    }
    public override string GetSystemName()
    {
        return "com.openngs.system.BlindBoxSystem";
    }
}
