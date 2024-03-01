using System;
using UnityEngine;
using System.Collections.Generic;


namespace Neptune.GameData
{   
    [Serializable]
    public class RoleGrowthData : GameDataArray<float>
    {        
        /// <summary>
        /// 基础属性
        /// </summary>
        public float Strength { get { return base.Values[(int)RoleAttribute.Strength]; } set { base.Values[(int)RoleAttribute.Strength] = value; } }
        public float Intelligence { get { return base.Values[(int)RoleAttribute.Intelligence]; } set { base.Values[(int)RoleAttribute.Intelligence] = value; } }
        public float Agility { get { return base.Values[(int)RoleAttribute.Agility]; } set { base.Values[(int)RoleAttribute.Agility] = value; } }
        public float MaxHP { get { return base.Values[(int)RoleAttribute.MaxHP]; } set { base.Values[(int)RoleAttribute.MaxHP] = value; } }
        public float MaxMP { get { return base.Values[(int)RoleAttribute.MaxMP]; } set { base.Values[(int)RoleAttribute.MaxMP] = value; } }
        public float MaxRage { get { return base.Values[(int)RoleAttribute.MaxRage]; } set { base.Values[(int)RoleAttribute.MaxRage] = value; } }
        public float MaxPoint { get { return base.Values[(int)RoleAttribute.MaxPoint]; } set { base.Values[(int)RoleAttribute.MaxPoint] = value; } }
        public float AttackDamage { get { return base.Values[(int)RoleAttribute.AttackDamage]; } set { base.Values[(int)RoleAttribute.AttackDamage] = value; } }
        public float AbilityPower { get { return base.Values[(int)RoleAttribute.AbilityPower]; } set { base.Values[(int)RoleAttribute.AbilityPower] = value; } }
        public float Armor { get { return base.Values[(int)RoleAttribute.Armor]; } set { base.Values[(int)RoleAttribute.Armor] = value; } }
        public float MagicResistance { get { return base.Values[(int)RoleAttribute.MagicResistance]; } set { base.Values[(int)RoleAttribute.MagicResistance] = value; } }
        public float Critical { get { return base.Values[(int)RoleAttribute.Critical]; } set { base.Values[(int)RoleAttribute.Critical] = value; } }
        public float MagicCritical { get { return base.Values[(int)RoleAttribute.MagicCritical]; } set { base.Values[(int)RoleAttribute.MagicCritical] = value; } }
        public float CriticalFactorReduction { get { return base.Values[(int)RoleAttribute.CriticalFactorReduction]; } set { base.Values[(int)RoleAttribute.CriticalFactorReduction] = value; } }
        public float HPRegen { get { return base.Values[(int)RoleAttribute.HPRegen]; } set { base.Values[(int)RoleAttribute.HPRegen] = value; } }
        public float MPRegen { get { return base.Values[(int)RoleAttribute.MPRegen]; } set { base.Values[(int)RoleAttribute.MPRegen] = value; } }
        public float HPDecrease { get { return base.Values[(int)RoleAttribute.HPDecrease]; } set { base.Values[(int)RoleAttribute.HPDecrease] = value; } }
        public float HPAtkRecovery { get { return base.Values[(int)RoleAttribute.HPAtkRecovery]; } set { base.Values[(int)RoleAttribute.HPAtkRecovery] = value; } }
        public float MPAtkRecovery { get { return base.Values[(int)RoleAttribute.MPAtkRecovery]; } set { base.Values[(int)RoleAttribute.MPAtkRecovery] = value; } }
        public float MPDmgRecovery { get { return base.Values[(int)RoleAttribute.MPDmgRecovery]; } set { base.Values[(int)RoleAttribute.MPDmgRecovery] = value; } }
        public float HitRate { get { return base.Values[(int)RoleAttribute.HitRate]; } set { base.Values[(int)RoleAttribute.HitRate] = value; } }
        public float Dodge { get { return base.Values[(int)RoleAttribute.Dodge]; } set { base.Values[(int)RoleAttribute.Dodge] = value; } }
        public float ArmorPenetration { get { return base.Values[(int)RoleAttribute.ArmorPenetration]; } set { base.Values[(int)RoleAttribute.ArmorPenetration] = value; } }
        public float MagicResistIgnore { get { return base.Values[(int)RoleAttribute.MagicResistIgnore]; } set { base.Values[(int)RoleAttribute.MagicResistIgnore] = value; } }
        public float ManaCostReduction { get { return base.Values[(int)RoleAttribute.ManaCostReduction]; } set { base.Values[(int)RoleAttribute.ManaCostReduction] = value; } }
        public float Heal { get { return base.Values[(int)RoleAttribute.Heal]; } set { base.Values[(int)RoleAttribute.Heal] = value; } }
        public float LifeSteal { get { return base.Values[(int)RoleAttribute.LifeSteal]; } set { base.Values[(int)RoleAttribute.LifeSteal] = value; } }
        public float MagicLifeSteal { get { return base.Values[(int)RoleAttribute.MagicLifeSteal]; } set { base.Values[(int)RoleAttribute.MagicLifeSteal] = value; } }
        public float HPSupply { get { return base.Values[(int)RoleAttribute.HPSupply]; } set { base.Values[(int)RoleAttribute.HPSupply] = value; } }
        public float MPSupply { get { return base.Values[(int)RoleAttribute.MPSupply]; } set { base.Values[(int)RoleAttribute.MPSupply] = value; } }
        public float AttackSpeed { get { return base.Values[(int)RoleAttribute.AttackSpeed]; } set { base.Values[(int)RoleAttribute.AttackSpeed] = value; } }
        public float MoveSpeed { get { return base.Values[(int)RoleAttribute.MoveSpeed]; } set { base.Values[(int)RoleAttribute.MoveSpeed] = value; } }
        public float PowerFactor { get { return base.Values[(int)RoleAttribute.PowerFactor]; } set { base.Values[(int)RoleAttribute.PowerFactor] = value; } }
        public float PhysicsImmunization { get { return base.Values[(int)RoleAttribute.PhysicsImmunization]; } set { base.Values[(int)RoleAttribute.PhysicsImmunization] = value; } }
        public float MagicImmunization { get { return base.Values[(int)RoleAttribute.MagicImmunization]; } set { base.Values[(int)RoleAttribute.MagicImmunization] = value; } }
        public float SkillLevel { get { return base.Values[(int)RoleAttribute.SkillLevel]; } set { base.Values[(int)RoleAttribute.SkillLevel] = value; } }
        public float SilenceResistRate { get { return base.Values[(int)RoleAttribute.SilenceResistRate]; } set { base.Values[(int)RoleAttribute.SilenceResistRate] = value; } }
        public float Toughness { get { return base.Values[(int)RoleAttribute.Toughness]; } set { base.Values[(int)RoleAttribute.Toughness] = value; } }
        public float CriticalFactor { get { return base.Values[(int)RoleAttribute.CriticalFactor]; } set { base.Values[(int)RoleAttribute.CriticalFactor] = value; } }
        public float MaleDmgReduction { get { return base.Values[(int)RoleAttribute.MaleDmgReduction]; } set { base.Values[(int)RoleAttribute.MaleDmgReduction] = value; } }
        public float SHealHp { get { return base.Values[(int)RoleAttribute.SHealHp]; } set { base.Values[(int)RoleAttribute.SHealHp] = value; } }
        public float ExtraAttackDamage { get { return base.Values[(int)RoleAttribute.ExtraAttackDamage]; } set { base.Values[(int)RoleAttribute.ExtraAttackDamage] = value; } }
        public float ExtraAbilityPower { get { return base.Values[(int)RoleAttribute.ExtraAbilityPower]; } set { base.Values[(int)RoleAttribute.ExtraAbilityPower] = value; } }
        public float ExtraHoly { get { return base.Values[(int)RoleAttribute.ExtraHoly]; } set { base.Values[(int)RoleAttribute.ExtraHoly] = value; } }
        public float LostHP { get { return base.Values[(int)RoleAttribute.LostHP]; } set { base.Values[(int)RoleAttribute.LostHP] = value; } }
        public float LostHPValue { get { return base.Values[(int)RoleAttribute.LostHPValue]; } set { base.Values[(int)RoleAttribute.LostHPValue] = value; } }
        public float DamageReduction { get { return base.Values[(int)RoleAttribute.DamageReduction]; } set { base.Values[(int)RoleAttribute.DamageReduction] = value; } }
        public float PhysicsDamageReduction { get { return base.Values[(int)RoleAttribute.PhysicsDamageReduction]; } set { base.Values[(int)RoleAttribute.PhysicsDamageReduction] = value; } }
        public float MagicDamageReduction { get { return base.Values[(int)RoleAttribute.MagicDamageReduction]; } set { base.Values[(int)RoleAttribute.MagicDamageReduction] = value; } }
        public float CDReduction { get { return base.Values[(int)RoleAttribute.CDReduction]; } set { base.Values[(int)RoleAttribute.CDReduction] = value; } }
        public float ExpAddition { get { return base.Values[(int)RoleAttribute.ExpAddition]; } set { base.Values[(int)RoleAttribute.ExpAddition] = value; } }
        public float GoldAddition { get { return base.Values[(int)RoleAttribute.GoldAddition]; } set { base.Values[(int)RoleAttribute.GoldAddition] = value; } }
        public float ReboundInjuryRatio { get { return base.Values[(int)RoleAttribute.ReboundInjuryRatio]; } set { base.Values[(int)RoleAttribute.ReboundInjuryRatio] = value; } }
        public float ArmorPenetrationRatio { get { return base.Values[(int)RoleAttribute.ArmorPenetrationRatio]; } set { base.Values[(int)RoleAttribute.ArmorPenetrationRatio] = value; } }
        public float MagicResistIgnoreRatio { get { return base.Values[(int)RoleAttribute.MagicResistIgnoreRatio]; } set { base.Values[(int)RoleAttribute.MagicResistIgnoreRatio] = value; } }
        public float HpIntervalLost { get { return base.Values[(int)RoleAttribute.HpIntervalLost]; } set { base.Values[(int)RoleAttribute.HpIntervalLost] = value; } }
        public float HpIntervalIncrease { get { return base.Values[(int)RoleAttribute.HpIntervalIncrease]; } set { base.Values[(int)RoleAttribute.HpIntervalIncrease] = value; } }
        /// <summary>
        /// 属性比例
        /// </summary>

        public float Strength_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Strength)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Strength)] = value; } }
        public float Intelligence_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Intelligence)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Intelligence)] = value; } }
        public float Agility_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Agility)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Agility)] = value; } }
        public float MaxHP_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MaxHP)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MaxHP)] = value; } }
        public float MaxMP_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MaxMP)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MaxMP)] = value; } }
        public float MaxRage_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MaxRage)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MaxRage)] = value; } }
        public float MaxPoint_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MaxPoint)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MaxPoint)] = value; } }
        public float AttackDamage_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.AttackDamage)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.AttackDamage)] = value; } }
        public float AbilityPower_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.AbilityPower)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.AbilityPower)] = value; } }
        public float Armor_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Armor)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Armor)] = value; } }
        public float MagicResistance_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MagicResistance)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MagicResistance)] = value; } }
        public float Critical_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Critical)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Critical)] = value; } }
        public float MagicCritical_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MagicCritical)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MagicCritical)] = value; } }
        public float CriticalFactorReduction_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.CriticalFactorReduction)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.CriticalFactorReduction)] = value; } }
        public float HPRegen_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.HPRegen)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.HPRegen)] = value; } }
        public float MPRegen_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MPRegen)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MPRegen)] = value; } }
        public float HPDecrease_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.HPDecrease)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.HPDecrease)] = value; } }
        public float HPAtkRecovery_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.HPAtkRecovery)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.HPAtkRecovery)] = value; } }
        public float MPAtkRecovery_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MPAtkRecovery)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MPAtkRecovery)] = value; } }
        public float MPDmgRecovery_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MPDmgRecovery)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MPDmgRecovery)] = value; } }
        public float HitRate_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.HitRate)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.HitRate)] = value; } }
        public float Dodge_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Dodge)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Dodge)] = value; } }
        public float ArmorPenetration_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ArmorPenetration)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ArmorPenetration)] = value; } }
        public float MagicResistIgnore_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MagicResistIgnore)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MagicResistIgnore)] = value; } }
        public float ManaCostReduction_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ManaCostReduction)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ManaCostReduction)] = value; } }
        public float Heal_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Heal)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Heal)] = value; } }
        public float LifeSteal_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.LifeSteal)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.LifeSteal)] = value; } }
        public float MagicLifeSteal_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MagicLifeSteal)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MagicLifeSteal)] = value; } }
        public float HPSupply_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.HPSupply)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.HPSupply)] = value; } }
        public float MPSupply_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MPSupply)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MPSupply)] = value; } }
        public float AttackSpeed_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.AttackSpeed)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.AttackSpeed)] = value; } }
        public float MoveSpeed_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MoveSpeed)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MoveSpeed)] = value; } }
        public float PowerFactor_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.PowerFactor)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.PowerFactor)] = value; } }
        public float PhysicsImmunization_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.PhysicsImmunization)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.PhysicsImmunization)] = value; } }
        public float MagicImmunization_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MagicImmunization)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MagicImmunization)] = value; } }
        public float SkillLevel_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.SkillLevel)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.SkillLevel)] = value; } }
        public float SilenceResistRate_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.SilenceResistRate)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.SilenceResistRate)] = value; } }
        public float Toughness_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Toughness)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Toughness)] = value; } }
        public float CriticalFactor_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.CriticalFactor)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.CriticalFactor)] = value; } }
        public float MaleDmgReduction_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MaleDmgReduction)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MaleDmgReduction)] = value; } }
        public float SHealHp_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.SHealHp)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.SHealHp)] = value; } }
        public float ExtraAttackDamage_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ExtraAttackDamage)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ExtraAttackDamage)] = value; } }
        public float ExtraAbilityPower_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ExtraAbilityPower)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ExtraAbilityPower)] = value; } }
        public float ExtraHoly_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ExtraHoly)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ExtraHoly)] = value; } }
        public float LostHP_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.LostHP)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.LostHP)] = value; } }
        public float LostHPValue_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.LostHPValue)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.LostHPValue)] = value; } }
        public float DamageReduction_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.DamageReduction)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.DamageReduction)] = value; } }
        public float PhysicsDamageReduction_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.PhysicsDamageReduction)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.PhysicsDamageReduction)] = value; } }
        public float MagicDamageReduction_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MagicDamageReduction)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MagicDamageReduction)] = value; } }
        public float CDReduction_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.CDReduction)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.CDReduction)] = value; } }
        public float ExpAddition_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ExpAddition)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ExpAddition)] = value; } }
        public float GoldAddition_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.GoldAddition)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.GoldAddition)] = value; } }
        public float ReboundInjuryRatio_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ReboundInjuryRatio)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ReboundInjuryRatio)] = value; } }
        public float ArmorPenetrationRatio_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ArmorPenetrationRatio)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ArmorPenetrationRatio)] = value; } }
        public float MagicResistIgnoreRatio_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MagicResistIgnoreRatio)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MagicResistIgnoreRatio)] = value; } }
        public float HpIntervalLost_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.HpIntervalLost)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.HpIntervalLost)] = value; } }
        public float HpIntervalIncrease_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.HpIntervalIncrease)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.HpIntervalIncrease)] = value; } }
 
        public RoleGrowthData():base((int)RoleAttribute.RATIOMAX*2)
        {

        }
    }
}
