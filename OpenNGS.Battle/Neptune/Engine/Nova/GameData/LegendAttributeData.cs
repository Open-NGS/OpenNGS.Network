using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neptune.GameData
{
    public class LegendAttributeData : GameDataBase<int>
    {
        public int Attribute1Amount { get; set; }
        public int Attribute1GS { get; set; }
        public RoleAttribute Attribute1LimitType { get; set; }
        public int Attribute1LimitValue { get; set; }
        public RoleAttribute Attribute1Type { get; set; }
        public int Attribute2Amount { get; set; }
        public int Attribute2GS { get; set; }
        public RoleAttribute Attribute2LimitType { get; set; }
        public int Attribute2LimitValue { get; set; }
        public RoleAttribute Attribute2Type { get; set; }
        public int Attribute3Amount { get; set; }
        public int Attribute3GS { get; set; }
        public RoleAttribute Attribute3LimitType { get; set; }
        public int Attribute3LimitValue { get; set; }
        public RoleAttribute Attribute3Type { get; set; }
        public string DisplayName { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
