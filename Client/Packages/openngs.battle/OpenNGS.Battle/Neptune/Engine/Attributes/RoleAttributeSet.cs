using Neptune.Datas;
using OpenNGS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptune
{
    public class RoleAttributeSet : AttributesSingle<Neptune.Datas.RoleAttribute>
    {
        public RoleAttributeSet(AttributesSingle attributes) : base(attributes)
        {
        }
        /// <summary>
        /// 基础属性
        /// </summary>
        public float Strength { get { return this[(int)RoleAttribute.Strength]; } set { this[(int)RoleAttribute.Strength] = value; } }
        public float Intelligence { get { return this[(int)RoleAttribute.Intelligence]; } set { this[(int)RoleAttribute.Intelligence] = value; } }
        public float Agility { get { return this[(int)RoleAttribute.Agility]; } set { this[(int)RoleAttribute.Agility] = value; } }
        public float MaxHP { get { return this[(int)RoleAttribute.MaxHP]; } set { this[(int)RoleAttribute.MaxHP] = value; } }
        public float MaxMP { get { return this[(int)RoleAttribute.MaxMP]; } set { this[(int)RoleAttribute.MaxMP] = value; } }
        public float MaxRage { get { return this[(int)RoleAttribute.MaxRage]; } set { this[(int)RoleAttribute.MaxRage] = value; } }
        public float MaxPoint { get { return this[(int)RoleAttribute.MaxPoint]; } set { this[(int)RoleAttribute.MaxPoint] = value; } }
        public float AttackDamage { get { return this[(int)RoleAttribute.AttackDamage]; } set { this[(int)RoleAttribute.AttackDamage] = value; } }
        public float AbilityPower { get { return this[(int)RoleAttribute.AbilityPower]; } set { this[(int)RoleAttribute.AbilityPower] = value; } }
        public float Armor { get { return this[(int)RoleAttribute.Armor]; } set { this[(int)RoleAttribute.Armor] = value; } }
        public float MagicResistance { get { return this[(int)RoleAttribute.MagicResistance]; } set { this[(int)RoleAttribute.MagicResistance] = value; } }
        public float Critical { get { return this[(int)RoleAttribute.Critical]; } set { this[(int)RoleAttribute.Critical] = value; } }
        public float MagicCritical { get { return this[(int)RoleAttribute.MagicCritical]; } set { this[(int)RoleAttribute.MagicCritical] = value; } }
        public float CriticalFactorReduction { get { return this[(int)RoleAttribute.CriticalFactorReduction]; } set { this[(int)RoleAttribute.CriticalFactorReduction] = value; } }
        public float HPRegen { get { return this[(int)RoleAttribute.HPRegen]; } set { this[(int)RoleAttribute.HPRegen] = value; } }
        public float MPRegen { get { return this[(int)RoleAttribute.MPRegen]; } set { this[(int)RoleAttribute.MPRegen] = value; } }
        public float RageRegen { get { return this[(int)RoleAttribute.RageRegen]; } set { this[(int)RoleAttribute.RageRegen] = value; } }
        public float PointRegen { get { return this[(int)RoleAttribute.PointRegen]; } set { this[(int)RoleAttribute.PointRegen] = value; } }
        public float HPDecrease { get { return this[(int)RoleAttribute.HPDecrease]; } set { this[(int)RoleAttribute.HPDecrease] = value; } }
        public float HPAtkRecovery { get { return this[(int)RoleAttribute.HPAtkRecovery]; } set { this[(int)RoleAttribute.HPAtkRecovery] = value; } }
        public float MPAtkRecovery { get { return this[(int)RoleAttribute.MPAtkRecovery]; } set { this[(int)RoleAttribute.MPAtkRecovery] = value; } }
        public float MPDmgRecovery { get { return this[(int)RoleAttribute.MPDmgRecovery]; } set { this[(int)RoleAttribute.MPDmgRecovery] = value; } }
        public float HitRate { get { return this[(int)RoleAttribute.HitRate]; } set { this[(int)RoleAttribute.HitRate] = value; } }
        public float Dodge { get { return this[(int)RoleAttribute.Dodge]; } set { this[(int)RoleAttribute.Dodge] = value; } }
        public float ArmorPenetration { get { return this[(int)RoleAttribute.ArmorPenetration]; } set { this[(int)RoleAttribute.ArmorPenetration] = value; } }
        public float MagicResistIgnore { get { return this[(int)RoleAttribute.MagicResistIgnore]; } set { this[(int)RoleAttribute.MagicResistIgnore] = value; } }
        public float ManaCostReduction { get { return this[(int)RoleAttribute.ManaCostReduction]; } set { this[(int)RoleAttribute.ManaCostReduction] = value; } }
        public float Heal { get { return this[(int)RoleAttribute.Heal]; } set { this[(int)RoleAttribute.Heal] = value; } }
        public float LifeSteal { get { return this[(int)RoleAttribute.LifeSteal]; } set { this[(int)RoleAttribute.LifeSteal] = value; } }
        public float MagicLifeSteal { get { return this[(int)RoleAttribute.MagicLifeSteal]; } set { this[(int)RoleAttribute.MagicLifeSteal] = value; } }
        public float HPSupply { get { return this[(int)RoleAttribute.HPSupply]; } set { this[(int)RoleAttribute.HPSupply] = value; } }
        public float MPSupply { get { return this[(int)RoleAttribute.MPSupply]; } set { this[(int)RoleAttribute.MPSupply] = value; } }
        public float AttackSpeed { get { return this[(int)RoleAttribute.AttackSpeed]; } set { this[(int)RoleAttribute.AttackSpeed] = value; } }
        public float MoveSpeed { get { return this[(int)RoleAttribute.MoveSpeed]; } set { this[(int)RoleAttribute.MoveSpeed] = value; } }
        public float PowerFactor { get { return this[(int)RoleAttribute.PowerFactor]; } set { this[(int)RoleAttribute.PowerFactor] = value; } }
        public float PhysicsImmunization { get { return this[(int)RoleAttribute.PhysicsImmunization]; } set { this[(int)RoleAttribute.PhysicsImmunization] = value; } }
        public float MagicImmunization { get { return this[(int)RoleAttribute.MagicImmunization]; } set { this[(int)RoleAttribute.MagicImmunization] = value; } }
        public float SkillLevel { get { return this[(int)RoleAttribute.SkillLevel]; } set { this[(int)RoleAttribute.SkillLevel] = value; } }
        public float SilenceResistRate { get { return this[(int)RoleAttribute.SilenceResistRate]; } set { this[(int)RoleAttribute.SilenceResistRate] = value; } }
        public float MaleDmgReduction { get { return this[(int)RoleAttribute.MaleDmgReduction]; } set { this[(int)RoleAttribute.MaleDmgReduction] = value; } }
        public float Toughness { get { return this[(int)RoleAttribute.Toughness]; } set { this[(int)RoleAttribute.Toughness] = value; } }
        public float CriticalFactor { get { return this[(int)RoleAttribute.CriticalFactor]; } set { this[(int)RoleAttribute.CriticalFactor] = value; } }
        public float DamageReduction { get { return this[(int)RoleAttribute.DamageReduction]; } set { this[(int)RoleAttribute.DamageReduction] = value; } }
        public float PhysicsDamageReduction { get { return this[(int)RoleAttribute.PhysicsDamageReduction]; } set { this[(int)RoleAttribute.PhysicsDamageReduction] = value; } }
        public float MagicDamageReduction { get { return this[(int)RoleAttribute.MagicDamageReduction]; } set { this[(int)RoleAttribute.MagicDamageReduction] = value; } }
        public float ExtraAttackDamage { get { return this[(int)RoleAttribute.ExtraAttackDamage]; } set { this[(int)RoleAttribute.ExtraAttackDamage] = value; } }
        public float ExtraAbilityPower { get { return this[(int)RoleAttribute.ExtraAbilityPower]; } set { this[(int)RoleAttribute.ExtraAbilityPower] = value; } }
        public float ExtraHoly { get { return this[(int)RoleAttribute.ExtraHoly]; } set { this[(int)RoleAttribute.ExtraHoly] = value; } }
        public float SHealHp { get { return this[(int)RoleAttribute.SHealHp]; } set { this[(int)RoleAttribute.SHealHp] = value; } }
        public float LostHP { get { return this[(int)RoleAttribute.LostHP]; } set { this[(int)RoleAttribute.LostHP] = value; } }
        public float LostHPValue { get { return this[(int)RoleAttribute.LostHPValue]; } set { this[(int)RoleAttribute.LostHPValue] = value; } }
        public float CDReduction { get { return this[(int)RoleAttribute.CDReduction]; } set { this[(int)RoleAttribute.CDReduction] = value; } }
        public float ExpAddition { get { return this[(int)RoleAttribute.ExpAddition]; } set { this[(int)RoleAttribute.ExpAddition] = value; } }
        public float GoldAddition { get { return this[(int)RoleAttribute.GoldAddition]; } set { this[(int)RoleAttribute.GoldAddition] = value; } }
        public float ReboundInjuryRatio { get { return this[(int)RoleAttribute.ReboundInjuryRatio]; } set { this[(int)RoleAttribute.ReboundInjuryRatio] = value; } }
        public float ArmorPenetrationRatio { get { return this[(int)RoleAttribute.ArmorPenetrationRatio]; } set { this[(int)RoleAttribute.ArmorPenetrationRatio] = value; } }
        public float MagicResistIgnoreRatio { get { return this[(int)RoleAttribute.MagicResistIgnoreRatio]; } set { this[(int)RoleAttribute.MagicResistIgnoreRatio] = value; } }
        public float HpIntervalLost { get { return this[(int)RoleAttribute.HpIntervalLost]; } set { this[(int)RoleAttribute.HpIntervalLost] = value; } }
        public float HpIntervalIncrease { get { return this[(int)RoleAttribute.HpIntervalIncrease]; } set { this[(int)RoleAttribute.HpIntervalIncrease] = value; } }

    }

    public class ControlEffectSet : AttributesInt32<Neptune.Datas.ControlEffect>
    {
        public ControlEffectSet(AttributesInt32 attributes) : base(attributes)
        {
        }

        public bool Root { get { return this[(int)ControlEffect.Root] == 1; } set { this[(int)ControlEffect.Root] = value ? 1 : 0; } }
        public bool Inhibition { get { return this[(int)ControlEffect.Inhibition] == 1; } set { this[(int)ControlEffect.Inhibition] = value ? 1 : 0; } }
        public bool Disable { get { return this[(int)ControlEffect.Disable] == 1; } set { this[(int)ControlEffect.Disable] = value ? 1 : 0; } }
        public bool Static { get { return this[(int)ControlEffect.Static] == 1; } set { this[(int)ControlEffect.Static] = value ? 1 : 0; } }
        public bool Void { get { return this[(int)ControlEffect.Void] == 1; } set { this[(int)ControlEffect.Void] = value ? 1 : 0; } }
        public bool Invincible { get { return this[(int)ControlEffect.Invincible] == 1; } set { this[(int)ControlEffect.Invincible] = value ? 1 : 0; } }
        public bool Unaffected { get { return this[(int)ControlEffect.Unaffected] == 1   ; } set { this[(int)ControlEffect.Unaffected] = value ? 1 : 0; } }
        public bool Charm { get { return this[(int)ControlEffect.Charm] == 1; } set { this[(int)ControlEffect.Charm] = value ? 1 : 0; } }
        public bool Immoblilize { get { return this[(int)ControlEffect.Immoblilize] == 1; } set { this[(int)ControlEffect.Immoblilize] = value ? 1 : 0; } }
        public bool Directed { get { return this[(int)ControlEffect.Directed] == 1; } set { this[(int)ControlEffect.Directed] = value ? 1 : 0; } }
        public bool Incurable { get { return this[(int)ControlEffect.Incurable] == 1; } set { this[(int)ControlEffect.Incurable] = value ? 1 : 0; } }
        public bool Norecover { get { return this[(int)ControlEffect.Norecover] == 1; } set { this[(int)ControlEffect.Norecover] = value ? 1 : 0; } }
        public bool MindChain { get { return this[(int)ControlEffect.MindChain] == 1; } set { this[(int)ControlEffect.MindChain] = value ? 1 : 0; } }
        public bool MindGain { get { return this[(int)ControlEffect.MindGain] == 1; } set { this[(int)ControlEffect.MindGain] = value ? 1 : 0; } }
        public bool Sleep { get { return this[(int)ControlEffect.Sleep] == 1; } set { this[(int)ControlEffect.Sleep] = value ? 1 : 0; } }
        public bool Imprisonment { get { return this[(int)ControlEffect.Imprisonment] == 1; } set { this[(int)ControlEffect.Imprisonment] = value ? 1 : 0; } }
        public bool Solidifying { get { return this[(int)ControlEffect.Solidifying] == 1; } set { this[(int)ControlEffect.Solidifying] = value ? 1 : 0; } }
        public bool Inhuman { get { return this[(int)ControlEffect.Inhuman] == 1; } set { this[(int)ControlEffect.Inhuman] = value ? 1 : 0; } }
        public bool OnlyNormalAttack { get { return this[(int)ControlEffect.OnlyNormalAttack] == 1; } set { this[(int)ControlEffect.OnlyNormalAttack] = value ? 1 : 0; } }
        public bool Taunt { get { return this[(int)ControlEffect.Taunt] == 1; } set { this[(int)ControlEffect.Taunt] = value ? 1 : 0; } }
        public bool Fear { get { return this[(int)ControlEffect.Fear] == 1; } set { this[(int)ControlEffect.Fear] = value ? 1 : 0; } }
        public bool Rebirth { get { return this[(int)ControlEffect.Rebirth] == 1; } set { this[(int)ControlEffect.Rebirth] = value ? 1 : 0; } }
        public bool Invisible { get { return this[(int)ControlEffect.Invisible] == 1; } set { this[(int)ControlEffect.Invisible] = value ? 1 : 0; } }
        public bool Bare { get { return this[(int)ControlEffect.Bare] == 1; } set { this[(int)ControlEffect.Bare] = value ? 1 : 0 ; } }
        public bool Grass { get { return this[(int)ControlEffect.Grass] == 1; } set { this[(int)ControlEffect.Grass] = value ? 1 : 0; } }
        public bool Disarmed { get { return this[(int)ControlEffect.Disarmed] == 1; } set { this[(int)ControlEffect.Disarmed] = value ? 1 : 0; } }
        public bool Vision { get { return this[(int)ControlEffect.Vision] == 1; } set { this[(int)ControlEffect.Vision] = value ? 1 : 0; } }
        public bool OnlyAttackBuilding { get { return this[(int)ControlEffect.OnlyAttackBuilding] == 1; } set { this[(int)ControlEffect.OnlyAttackBuilding] = value ? 1 : 0; } }

    }
}
