using System;
using System.Collections.Generic;

namespace Neptune.GameData
{
    [Serializable]
    public class MarkData 
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Effect { get; set; }
        public string Desc { get; set; }
        public int MaxCount { get; set; }           //最大层数
        public int TriggerCount { get; set; }       //触发层数
        public int CostCount { get; set; }          //每次触发的消耗层数
        public bool Auto { get; set; }              //是否自动触发
        public float TriggerCD { get; set; }        //触发间隔 只用于自动触发类的印记 单位：秒
        public float LastTime { get; set; }         //印记的持续时间 时间到达后层数清零 单位：秒
        public int AddTalentID { get; set; }        //印记触发的技能ID
        public int TriggerTalentID { get; set; }    // 
        public List<int> TalentIDs { get; set; }    //印记触发的技能ID数组 不为空时 从该数组中随机选择一个释放 为空时 释放TalentID
        public OverlayType MarkOverlayType { get; set; }

        public RoleType RoleType { get; set; }

        public RoleType EffectRoleType { get; set; }
        public MarkTriggerType MarkTriggerType { get; set; }
        public bool UseTargetPosition { get; set; }
        public AddMarkType AddMarkType { get; set; }
        public bool DontRemoveOnDeath { get; set; }
        public MarkData() 
        {

        }

        public MarkData Clone()
        {
            MarkData clone = this.MemberwiseClone() as MarkData;
            return clone;
        }
    }
}