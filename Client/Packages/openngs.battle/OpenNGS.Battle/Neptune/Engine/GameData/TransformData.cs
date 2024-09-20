using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameData
{
    public class TransformData
    {
        public int BasicSkill { get; set; }
        public string EnterAction { get; set; }
        public int HPRatio { get; set; }
        public int Period { get; set; }
        public int PeriodGroup { get; set; }
        public string Model { get; set; }
        public List<int> SkillList { get; set; }
        public List<int> CDInheritIndexs { get; set; }
        public string TriggerType { get; set; }
        public bool TransformModel { get; set; }
        
    }
}
