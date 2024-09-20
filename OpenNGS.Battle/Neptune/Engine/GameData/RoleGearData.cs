using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameData
{
    public class RoleGearData : Data.DataBase<RoleGearData>
    {
        public string Desc { get; set; }
        public string Equip1 { get; set; }
        public int Equip1ID { get; set; }
        public string Equip2 { get; set; }
        public int Equip2ID { get; set; }
        public string Equip3 { get; set; }
        public int Equip3ID { get; set; }
        public string Equip4 { get; set; }
        public int Equip4ID { get; set; }
        public string Equip5 { get; set; }
        public int Equip5ID { get; set; }
        public string Equip6 { get; set; }
        public int Equip6ID { get; set; }
        public double EquipLevel { get; set; }
        public float Power { get; set; }
        public int Hero_ID { get; set; }
        public string Hero_Name { get; set; }
        public int Init1ID { get; set; }
        public int Init2ID { get; set; }
        public int Init3ID { get; set; }
        public int Init4ID { get; set; }
        public int Init5ID { get; set; }
        public int Init6ID { get; set; }
        public string LV { get; set; }
        public int LvReq { get; set; }
        public int Quality { get; set; }
        public string Type { get; set; }
    }
}
