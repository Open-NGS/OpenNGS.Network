using System;
using System.Collections.Generic;

namespace Neptune.GameData
{
    public class RoleData : GameDataArray<float>
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Narrative { get; set; }
        public string Art { get; set; }
        public string ArtName { get; set; }
        public string ArtSwitch { get; set; }
        public string Portrait { get; set; }
        public string BigPortrait { get; set; }
        public string PortraitSwitch { get; set; }
        public RolePosition PositionType { get; set; }
        public string IconName { get; set; }
        public string BgColor { get; set; }
        public string Body { get; set; }
        public string BossPortrait { get; set; }
        public bool CanLegend { get; set; }
        public int Cost { get; set; }
        public int Count { get; set; }
        public RoleType RoleType { get; set; }
        public RoleAttribute MainAttrib { get; set; }
        public int InitialQuality { get; set; }
        public int InitialStars { get; set; }
        public int MaxStars { get; set; }
        public int FlyHeight { get; set; }
        public bool AttackAir { get; set; }
        public int SightRange { get; set; }
        public int LockRange { get; set; }
        public int JumpSpeed { get; set; }
        public string Gender { get; set; }
        public List<string> HeroTags { get; set; }
        public string HeroType { get; set; }
        public int HPLayers { get; set; }
        public string Model { get; set; }
        public string ModelSwitch { get; set; }
        public int CollideRadius { get; set; }

        public int PeriodGroup { get; set; }
        public float RoleHeight { get; set; }

        public bool NoBuffEffect { get; set; }


        public List<string> VoiceMove { get; set; }
        public List<string> VoiceShow { get; set; }
        public List<string> VoiceUI { get; set; }
        public string VoiceUlt { get; set; }
        public List<string> VoiceDeath { get; set; }

        public List<string> VoiceKill { get; set; }
        public float Strength { get { return base.Values[(int)RoleAttribute.Strength]; } set { base.Values[(int)RoleAttribute.Strength] = value; } }
        public float Agility { get { return base.Values[(int)RoleAttribute.Agility]; } set { base.Values[(int)RoleAttribute.Agility] = value; } }
        public float Intelligence { get { return base.Values[(int)RoleAttribute.Intelligence]; } set { base.Values[(int)RoleAttribute.Intelligence] = value; } }
        public float Vitality { get { return base.Values[(int)RoleAttribute.Vitality]; } set { base.Values[(int)RoleAttribute.Vitality] = value; } }
        public float Constitution { get { return base.Values[(int)RoleAttribute.Constitution]; } set { base.Values[(int)RoleAttribute.Constitution] = value; } }
        public float MaxHP { get { return base.Values[(int)RoleAttribute.MaxHP]; } set { base.Values[(int)RoleAttribute.MaxHP] = value; } }
        public float MaxMP { get { return base.Values[(int)RoleAttribute.MaxMP]; } set { base.Values[(int)RoleAttribute.MaxMP] = value; } }
        public float MaxRage { get { return base.Values[(int)RoleAttribute.MaxRage]; } set { base.Values[(int)RoleAttribute.MaxRage] = value; } }
        public float MaxPoint { get { return base.Values[(int)RoleAttribute.MaxPoint]; } set { base.Values[(int)RoleAttribute.MaxPoint] = value; } }
        public float HPRegen { get { return base.Values[(int)RoleAttribute.HPRegen]; } set { base.Values[(int)RoleAttribute.HPRegen] = value; } }
        public float MPRegen { get { return base.Values[(int)RoleAttribute.MPRegen]; } set { base.Values[(int)RoleAttribute.MPRegen] = value; } }
        public float AttackDamage { get { return base.Values[(int)RoleAttribute.AttackDamage]; } set { base.Values[(int)RoleAttribute.AttackDamage] = value; } }
        public float AbilityPower { get { return base.Values[(int)RoleAttribute.AbilityPower]; } set { base.Values[(int)RoleAttribute.AbilityPower] = value; } }
        public float MoveSpeed { get { return base.Values[(int)RoleAttribute.MoveSpeed]; } set { base.Values[(int)RoleAttribute.MoveSpeed] = value; } }
        public float Armor { get { return base.Values[(int)RoleAttribute.Armor]; } set { base.Values[(int)RoleAttribute.Armor] = value; } }
        public float MagicResistance { get { return base.Values[(int)RoleAttribute.MagicResistance]; } set { base.Values[(int)RoleAttribute.MagicResistance] = value; } }
        public float Critical { get { return base.Values[(int)RoleAttribute.Critical]; } set { base.Values[(int)RoleAttribute.Critical] = value; } }
        public float CriticalFactorReduction { get { return base.Values[(int)RoleAttribute.CriticalFactorReduction]; } set { base.Values[(int)RoleAttribute.CriticalFactorReduction] = value; } }
        public float MagicCritical { get { return base.Values[(int)RoleAttribute.MagicCritical]; } set { base.Values[(int)RoleAttribute.MagicCritical] = value; } }
        public float ReboundInjuryRatio { get { return base.Values[(int)RoleAttribute.ReboundInjuryRatio]; } set { base.Values[(int)RoleAttribute.ReboundInjuryRatio] = value; } }
        public float CDReduction { get { return base.Values[(int)RoleAttribute.CDReduction]; } set { base.Values[(int)RoleAttribute.CDReduction] = value; } }
        public float Toughness { get { return base.Values[(int)RoleAttribute.Toughness]; } set { base.Values[(int)RoleAttribute.Toughness] = value; } }
        public float MagicLifeSteal { get { return base.Values[(int)RoleAttribute.MagicLifeSteal]; } set { base.Values[(int)RoleAttribute.MagicLifeSteal] = value; } }
        public float ArmorPenetration { get { return base.Values[(int)RoleAttribute.ArmorPenetration]; } set { base.Values[(int)RoleAttribute.ArmorPenetration] = value; } }
        public float MagicResistIgnore { get { return base.Values[(int)RoleAttribute.MagicResistIgnore]; } set { base.Values[(int)RoleAttribute.MagicResistIgnore] = value; } }
        public float HitRate { get { return base.Values[(int)RoleAttribute.HitRate]; } set { base.Values[(int)RoleAttribute.HitRate] = value; } }
        public float Dodge { get { return base.Values[(int)RoleAttribute.Dodge]; } set { base.Values[(int)RoleAttribute.Dodge] = value; } }
        public float ManaCostReduction { get { return base.Values[(int)RoleAttribute.ManaCostReduction]; } set { base.Values[(int)RoleAttribute.ManaCostReduction] = value; } }
        public float Heal { get { return base.Values[(int)RoleAttribute.Heal]; } set { base.Values[(int)RoleAttribute.Heal] = value; } }
        public float LifeSteal { get { return base.Values[(int)RoleAttribute.LifeSteal]; } set { base.Values[(int)RoleAttribute.LifeSteal] = value; } }
        public float HPSupply { get { return base.Values[(int)RoleAttribute.HPSupply]; } set { base.Values[(int)RoleAttribute.HPSupply] = value; } }
        public float MPSupply { get { return base.Values[(int)RoleAttribute.MPSupply]; } set { base.Values[(int)RoleAttribute.MPSupply] = value; } }
        public float HPDecrease { get { return base.Values[(int)RoleAttribute.HPDecrease]; } set { base.Values[(int)RoleAttribute.HPDecrease] = value; } }
        public float ArmorPenetrationRatio { get { return base.Values[(int)RoleAttribute.ArmorPenetrationRatio]; } set { base.Values[(int)RoleAttribute.ArmorPenetrationRatio] = value; } }
        public float MagicResistIgnoreRatio { get { return base.Values[(int)RoleAttribute.MagicResistIgnoreRatio]; } set { base.Values[(int)RoleAttribute.MagicResistIgnoreRatio] = value; } }
        public float AttackSpeed { get { return base.Values[(int)RoleAttribute.AttackSpeed]; } set { base.Values[(int)RoleAttribute.AttackSpeed] = value; } }
        public float CriticalFactor { get { return base.Values[(int)RoleAttribute.CriticalFactor]; } set { base.Values[(int)RoleAttribute.CriticalFactor] = value; } }
        public float HpIntervalLost { get { return base.Values[(int)RoleAttribute.HpIntervalLost]; } set { base.Values[(int)RoleAttribute.HpIntervalLost] = value; } }
        public float HpIntervalIncrease { get { return base.Values[(int)RoleAttribute.HpIntervalIncrease]; } set { base.Values[(int)RoleAttribute.HpIntervalIncrease] = value; } }
        /// <summary>
        /// 攻击时额外造成物理伤害
        /// </summary>
        public float ExtraAttackDamage { get { return base.Values[(int)RoleAttribute.ExtraAttackDamage]; } set { base.Values[(int)RoleAttribute.ExtraAttackDamage] = value; } }
        /// <summary>
        /// 攻击时额外造成魔法伤害
        /// </summary>
        public float ExtraAbilityPower { get { return base.Values[(int)RoleAttribute.ExtraAbilityPower]; } set { base.Values[(int)RoleAttribute.ExtraAbilityPower] = value; } }
        /// <summary>
        /// 攻击时额外造成真实伤害
        /// </summary>
        public float ExtraHoly { get { return base.Values[(int)RoleAttribute.ExtraHoly]; } set { base.Values[(int)RoleAttribute.ExtraHoly] = value; } }

        /// <summary>
        /// 属性比例
        /// </summary>

        public float Strength_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Strength)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Strength)] = value; } }
        public float Intelligence_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Intelligence)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Intelligence)] = value; } }
        public float Agility_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Agility)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Agility)] = value; } }
        public float Vitality_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Vitality)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Vitality)] = value; } }
        public float Constitution_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Constitution)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Constitution)] = value; } }
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
        public float MaleDmgReduction_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MaleDmgReduction)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MaleDmgReduction)] = value; } }
        public float Toughness_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Toughness)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.Toughness)] = value; } }
        public float CriticalFactor_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.CriticalFactor)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.CriticalFactor)] = value; } }
        public float DamageReduction_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.DamageReduction)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.DamageReduction)] = value; } }
        public float PhysicsDamageReduction_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.PhysicsDamageReduction)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.PhysicsDamageReduction)] = value; } }
        public float MagicDamageReduction_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MagicDamageReduction)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MagicDamageReduction)] = value; } }
        public float ExtraAttackDamage_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ExtraAttackDamage)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ExtraAttackDamage)] = value; } }
        public float ExtraAbilityPower_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ExtraAbilityPower)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ExtraAbilityPower)] = value; } }
        public float ExtraHoly_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ExtraHoly)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ExtraHoly)] = value; } }
        public float SHealHp_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.SHealHp)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.SHealHp)] = value; } }
        public float LostHP_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.LostHP)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.LostHP)] = value; } }
        public float LostHPValue_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.LostHPValue)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.LostHPValue)] = value; } }
        public float CDReduction_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.CDReduction)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.CDReduction)] = value; } }
        public float ExpAddition_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ExpAddition)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ExpAddition)] = value; } }
        public float GoldAddition_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.GoldAddition)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.GoldAddition)] = value; } }
        public float ReboundInjuryRatio_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ReboundInjuryRatio)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ReboundInjuryRatio)] = value; } }
        public float ArmorPenetrationRatio_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ArmorPenetrationRatio)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.ArmorPenetrationRatio)] = value; } }
        public float MagicResistIgnoreRatio_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MagicResistIgnoreRatio)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.MagicResistIgnoreRatio)] = value; } }
        public float HpIntervalLost_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.HpIntervalLost)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.HpIntervalLost)] = value; } }
        public float HpIntervalIncrease_a { get { return base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.HpIntervalIncrease)]; } set { base.Values[RoleAttributes.GetRatioValueField(RoleAttribute.HpIntervalIncrease)] = value; } }

        public int MPGainRate { get; set; }

        public int DSFactor { get; set; }
        public int BasicTalent { get; set; }
        public float IRHPRatio { get; set; }

        public float Weight { get; set; }
        public AttackMode AttackMode { get; set; }

        public string BirthAction { get; set; }

        public string VoiceArtist { get; set; }
        public string Voiceatk { get; set; }
        public string Voiceatk2 { get; set; }
        //public string VoiceMove { get; set; }
        public string VoiceReady { get; set; }
        //public string Voiceult { get; set; }
        public string VoiceUpgrade { get; set; }
        public bool Enable { get; set; }
        public string Extend { get; set; }

        public int GoldMetallurgy { get; set; }
        public float Grow { get; set; }
        public RaceType Race { get; set; }
        public string[] Career { get; set; }
        public string[] Ability { get; set; }
        public float ModelScale { get; set; }

        public string PortraitBackground { get; set; }
        public string LargePortrait { get; set; }

        public int[] RelatedRoles { get; set; }

        //Attached role
        public int AttachedRole;
        public List<int> AttachedPosition { get; set; }
        //
        public string DeathEffect { get; set; }
        public string RebornEffect { get; set; }
        public string LevelupEffect { get; set; }

        public float DeployTime { get; set; }

        public float LifeTime { get; set; }
        public float LifeGrowthFactor { get; set; }

        public float ChargeDelay { get; set; }
        public int ChargeSpeed { get; set; }
        public int ChargeTalent { get; set; }
        public string ChargeAction { get; set; }
        public MPType MPType { get; set; }
        public bool Grid { get; set; }


        public RoleData Clone()
        {
            RoleData clone = this.MemberwiseClone() as RoleData;
            //没有对List 做深拷贝
            clone.Values = (float[])this.Values.Clone();
            return clone;
        }

        public RoleData() : base((int)RoleAttribute.RATIOMAX)
        {
        }
    }
}
