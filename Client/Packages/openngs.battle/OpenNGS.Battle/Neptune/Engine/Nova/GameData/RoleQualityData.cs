using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neptune.GameData
{
    public class RoleQualityData : GameDataArray<float>
    {
        public float PlusAgility { get; set; }
        public float PlusIntelligence { get; set; }
        public float PlusStrength { get; set; }
        public float AttackDamage { get { return base.Values[(int)RoleAttribute.AttackDamage]; } set { base.Values[(int)RoleAttribute.AttackDamage] = value; } }
        public float Agility { get { return base.Values[(int)RoleAttribute.Agility]; } set { base.Values[(int)RoleAttribute.Agility] = value; } }
        public float AbilityPower { get { return base.Values[(int)RoleAttribute.AbilityPower]; } set { base.Values[(int)RoleAttribute.AbilityPower] = value; } }
        public float Armor { get { return base.Values[(int)RoleAttribute.Armor]; } set { base.Values[(int)RoleAttribute.Armor] = value; } }
        public float ArmorPenetration { get { return base.Values[(int)RoleAttribute.ArmorPenetration]; } set { base.Values[(int)RoleAttribute.ArmorPenetration] = value; } }
        public float ManaCostReduction { get { return base.Values[(int)RoleAttribute.ManaCostReduction]; } set { base.Values[(int)RoleAttribute.ManaCostReduction] = value; } }
        public float Critical { get { return base.Values[(int)RoleAttribute.Critical]; } set { base.Values[(int)RoleAttribute.Critical] = value; } }
        public float Dodge { get { return base.Values[(int)RoleAttribute.Dodge]; } set { base.Values[(int)RoleAttribute.Dodge] = value; } }
        public float Power { get; set; }
        public float Heal { get { return base.Values[(int)RoleAttribute.Heal]; } set { base.Values[(int)RoleAttribute.Heal] = value; } }
        public float HitRate { get { return base.Values[(int)RoleAttribute.HitRate]; } set { base.Values[(int)RoleAttribute.HitRate] = value; } }
        public float MaxHP { get { return base.Values[(int)RoleAttribute.MaxHP]; } set { base.Values[(int)RoleAttribute.MaxHP] = value; } }
        public float HPSupply { get { return base.Values[(int)RoleAttribute.HPSupply]; } set { base.Values[(int)RoleAttribute.HPSupply] = value; } }
        public int ID { get; set; }
        public float Intelligence { get { return base.Values[(int)RoleAttribute.Intelligence]; } set { base.Values[(int)RoleAttribute.Intelligence] = value; } }
        public float LifeSteal { get { return base.Values[(int)RoleAttribute.LifeSteal]; } set { base.Values[(int)RoleAttribute.LifeSteal] = value; } }
        public float MagicCritical { get { return base.Values[(int)RoleAttribute.MagicCritical]; } set { base.Values[(int)RoleAttribute.MagicCritical] = value; } }
        public float MagicImmunization { get { return base.Values[(int)RoleAttribute.MagicImmunization]; } set { base.Values[(int)RoleAttribute.MagicImmunization] = value; } }
        public float MaxMP { get { return base.Values[(int)RoleAttribute.MaxMP]; } set { base.Values[(int)RoleAttribute.MaxMP] = value; } }
        public float MaxRage { get { return base.Values[(int)RoleAttribute.MaxRage]; } set { base.Values[(int)RoleAttribute.MaxRage] = value; } }
        public float MaxPoint { get { return base.Values[(int)RoleAttribute.MaxPoint]; } set { base.Values[(int)RoleAttribute.MaxPoint] = value; } }
        public float MPSupply { get { return base.Values[(int)RoleAttribute.MPSupply]; } set { base.Values[(int)RoleAttribute.MPSupply] = value; } }
        public float MagicResistance { get { return base.Values[(int)RoleAttribute.MagicResistance]; } set { base.Values[(int)RoleAttribute.MagicResistance] = value; } }
        public float MagicResistIgnore { get { return base.Values[(int)RoleAttribute.MagicResistIgnore]; } set { base.Values[(int)RoleAttribute.MagicResistIgnore] = value; } }
        public string Name { get; set; }
        public string NameQuality { get; set; }
        public float PhysicsImmunization { get { return base.Values[(int)RoleAttribute.PhysicsImmunization]; } set { base.Values[(int)RoleAttribute.PhysicsImmunization] = value; } }
        public int Quality { get; set; }
        public int serialnumber { get; set; }
        public float SilenceResistRate { get; set; }
        public float SkillLevel { get; set; }
        public float Strength { get { return base.Values[(int)RoleAttribute.Strength]; } set { base.Values[(int)RoleAttribute.Strength] = value; } }
        public float Toughness { get { return base.Values[(int)RoleAttribute.Toughness]; } set { base.Values[(int)RoleAttribute.Toughness] = value; } }
        public string Type { get; set; }

		//
		public int QualitySkill{get;set;}
		public int QualitySkillID {get;set;}
		public int Increment {get;set;}
		//

        public RoleQualityData()
            : base((int)RoleAttribute.RATIOMAX)
        {

        }
    }
}
