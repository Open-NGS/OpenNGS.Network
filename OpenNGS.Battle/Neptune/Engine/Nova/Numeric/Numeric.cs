using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neptune.GameData;
using UnityEngine;

namespace Neptune
{
    public class Numeric : INumeric
    {
        /// <summary>
        /// 计算技能基础攻击力
        /// </summary>
        /// <param name="talent">技能</param>
        /// <param name="from">攻击来源</param>
        /// <param name="target">攻击目标</param>
        /// <param name="ratio">攻击力系数</param>
        /// <returns></returns>
        public virtual float CalculateTalentForce(BattleSkill talent, BattleEntity from, BattleActor target, ref float ratio)
        {
            float force = UFloat.Round(UFloat.Round(talent.Data.BaseRatio * talent.Caster.Attributes[(int)talent.Data.BaseAttr]) + talent.Data.BasicNum);
            if (talent.Data.BaseRatioEx > 0)
                force += UFloat.Round(talent.Data.BaseRatioEx * talent.Caster.Attributes[(int)talent.Data.BaseAttrEx]);
            if (talent.Data.TargetRatio > 0)
                force = UFloat.Round(force + force * (talent.Data.TargetRatio * target.Attributes[(int)talent.Data.TargetAttr]));

            ratio = 1;
            if (from != null && from.DamageFactor > 0)
                ratio = from.DamageFactor;

            //根据目标类型影响攻击力
            if ((talent.Data.TargetTypeAffect & target.Data.RoleType) > 0 || (talent.Data.TargetTypeAffect & RoleType.Demon) == RoleType.Demon && target.Config.IsDemon)
            {
                ratio = UFloat.Round(ratio * talent.Data.TargetTypeRatio);
            }
            force = UFloat.Round(force * ratio);
            if (talent.Data.ExtraDamageTargetRatio > 0)
            {
                float extraDamage = UFloat.Round(talent.Data.ExtraDamageTargetRatio * target.Attributes[(int)talent.Data.ExtraDamageTargetAttr]);
                if ((talent.Data.TargetTypeAffect & target.Data.RoleType) > 0 || (talent.Data.TargetTypeAffect & RoleType.Demon) == RoleType.Demon && target.Config.IsDemon)
                {
                    if (talent.Data.ExtraTargetTypeRatio > 0)
                        extraDamage = UFloat.Round(extraDamage * talent.Data.ExtraTargetTypeRatio);
                }
                force = UFloat.Round(force + extraDamage);
            }

#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0}: {1} --> {2} Force:{3:f6} PA:{4} BA:{5:f6} BN:{6:f6} PR:{7:f6} CF:{8:f6}", talent.Caster.FullName, talent.FullName(), target.FullName, force, talent.Data.BaseAttr, talent.Caster.Attributes[(int)talent.Data.BaseAttr], talent.Data.BasicNum, talent.Data.BaseRatio, ratio);
#endif
            return force;
        }

        /// <summary>
        /// 计算技能闪避
        /// </summary>
        /// <param name="talent">技能</param>
        /// <param name="target">攻击目标</param>
        /// <param name="injuryType">伤害类型</param>
        /// <returns></returns>
        public bool TalentDodge(BattleSkill talent, BattleActor target, InjuryType injuryType)
        {
            if (injuryType == InjuryType.AttackDamage && !talent.Data.NoDodge)
            {
                float attributesDodg = Math.Max(0, target.Attributes.Dodge - talent.Caster.Attributes.HitRate);
                if (attributesDodg / (100 + attributesDodg) > Util.Random.Rand())
                {
                    return true;
                }
            }
            return false;
        }
        protected InjuryResult result = new InjuryResult();
        /// <summary>
        /// 计算最终伤害
        /// </summary>
        /// <param name="target">攻击目标</param>
        /// <param name="injuryType">伤害类型</param>
        /// <param name="attr">目标属性</param>
        /// <param name="force">攻击力</param>
        /// <param name="from">攻击来源</param>
        /// <returns></returns>
        public virtual InjuryResult CalcFinalInjury(BattleActor target, InjuryType injuryType, RoleAttribute attr, float force, BattleActor from, BattleSkill fromTalent)
        {
            result.Reset();
            float Defense = 0;
            float Immunization = 0;
            float Critical = 0;
            float PenetrationRatio = 0;
            switch (injuryType)
            {
                case InjuryType.AttackDamage:
                    {
                        //                     //护甲计算
                        //                     Defense = target.Attributes.Armor - from.Attributes.ArmorPenetration;
                        //                     //护甲穿透率
                        //                     PenetrationRatio = Math.Min(Math.Max(0, from.Attributes.ArmorPenetrationRatio), 100);
                        //                     //护甲减伤率计算
                        //                     if (Defense >= 0)
                        //                     {
                        //                         Defense = UFloat.Round(Defense/(600 + Defense * (1 + PenetrationRatio*Numeric.Hundredth)));
                        //                     }
                        //                     else
                        //                     {
                        //                         Defense = UFloat.Round(Defense/(600 - Defense * (1 - PenetrationRatio*Numeric.Hundredth)));
                        //                     }
                        Defense = target.Attributes.Armor;
                        float armorPenetration = from.Attributes.ArmorPenetration;
                        float armorPenetrationRatio = Math.Min(Math.Max(0, from.Attributes.ArmorPenetrationRatio), 100);
                        if (Defense <= 0)
                        {
                            armorPenetration = 0;
                            armorPenetrationRatio = 0;
                        }
                        PenetrationRatio = Math.Max(Defense * (1 - armorPenetrationRatio * EngineConst.Hundredth) - armorPenetration, 0);
                        Defense = UFloat.Round(PenetrationRatio / (600 + PenetrationRatio));
                        Critical = from.Attributes.Critical;
                        Immunization = target.Attributes.PhysicsImmunization;
                        break;
                    }
                case InjuryType.AbilityPower:
                    {
                        //                     //魔抗
                        //                     Defense = target.Attributes.MagicResistance - from.Attributes.MagicResistIgnore;
                        //                     //魔抗穿透率
                        //                     PenetrationRatio = Math.Min(Math.Max(0, from.Attributes.MagicResistIgnoreRatio), 100);
                        //                     //魔抗减伤率
                        //                     if (Defense >= 0)
                        //                     {
                        //                         Defense = UFloat.Round(Defense/(600 + Defense*(1 + PenetrationRatio*Numeric.Hundredth)));
                        //                     }
                        //                     else
                        //                     {
                        //                         Defense = UFloat.Round(Defense/(600 - Defense*(1 - PenetrationRatio*Numeric.Hundredth)));
                        //                     }


                        Defense = target.Attributes.MagicResistance;
                        float magicResistIgnore = from.Attributes.MagicResistIgnore;
                        float magicResistIgnoreRatio = Math.Min(Math.Max(0, from.Attributes.MagicResistIgnoreRatio), 100);
                        if (Defense <= 0)
                        {
                            magicResistIgnore = 0;
                            magicResistIgnoreRatio = 0;
                        }
                        PenetrationRatio = Math.Max(Defense * (1 - magicResistIgnoreRatio * EngineConst.Hundredth) - magicResistIgnore, 0);
                        Defense = UFloat.Round(PenetrationRatio / (600 + PenetrationRatio));
                        Critical = from.Attributes.MagicCritical;
                        Immunization = target.Attributes.MagicImmunization;
                        break;
                    }
                default:
                    break;
            }

            result.IsCritical = target.InjuryCrit * Critical * EngineConst.Hundredth > Util.Random.Rand() && !target.IsRoleType(RoleType.Building);
            float criticalFactor = 100;
            if (result.IsCritical)
            {
                criticalFactor = from.Attributes.CriticalFactor + 200;//from.Attributes.CriticalFactor > 0 ? from.Attributes.CriticalFactor + 200 : 200;
                criticalFactor = UFloat.Round(Math.Min(350f, Math.Max(100, criticalFactor - target.Attributes.CriticalFactorReduction)));
            }

            force = force < 0 ? 0 : force;
            //float injury = force * force / (force + Defense * target.InjuryFactor);
            float injury = UFloat.Round(force * UFloat.Round(1 - Defense));
            injury = UFloat.RoundToInt(injury);
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("    {0} damage {1} {2} {3:f6} {4:f6} ({5:f6},{6:f6},{7:f6},{8:f6}) {9:f6}", target.FullName, double.IsNaN(injury) ? "-nan" : "" + injury.ToString("f6"), attr, force, result.IsCritical ? "(Crit!)" : "", Critical, Immunization, Defense, target.Attributes.CriticalFactorReduction, target.InjuryCrit);
#endif
            if (criticalFactor != 100 || from.Attributes.PowerFactor != 1)
            {
                injury = UFloat.Round(injury * (criticalFactor * EngineConst.Hundredth) * from.Attributes.PowerFactor);
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("    {0} injury {1:f6} criticalFactor {2:f6} PowerFactor:{3:f6}", target.FullName, injury, criticalFactor, from.Attributes.PowerFactor);
#endif
            }

            float damageRatio = from.PassiveEffect.BaseRatio;
            int extDmgRatio = 0;
            if (fromTalent != null)
            {
                extDmgRatio = fromTalent.GetExtraDamageRatio(target.Data.RoleType);
                damageRatio += extDmgRatio * EngineConst.Hundredth;
            }

            if (!damageRatio.Equals(0))
            {
                damageRatio = Math.Max(-1.0f, damageRatio);
                injury = UFloat.Round(injury * (damageRatio + 1.0f));
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("    {0} injury {1:f6} BaseRatio:{2:f6} damageRatio:{3:f6} extDmgRatio:{4}", target.FullName, injury, from.PassiveEffect.BaseRatio, damageRatio, extDmgRatio);
#endif
            }

            result.IsImmunization = Immunization * EngineConst.Hundredth >= 1;
            if (result.IsImmunization)
            {
                return result;
            }

            //Damage Reduction
            float damageReduce = 0;
            if (InjuryType.Heal != injuryType && (target.Attributes.DamageReduction_a != 0 || target.Attributes.DamageReduction != 0))
                damageReduce = (target.Attributes.DamageReduction_a == 0 ? damageReduce : UFloat.Round(UFloat.Round(target.Attributes.DamageReduction_a / 100.0f) * injury)) + target.Attributes.DamageReduction;
            if (InjuryType.AttackDamage == injuryType && (target.Attributes.PhysicsDamageReduction_a != 0 || target.Attributes.PhysicsDamageReduction != 0))
                damageReduce = (target.Attributes.PhysicsDamageReduction_a == 0 ? damageReduce : UFloat.Round(target.Attributes.PhysicsDamageReduction_a / 100.0f * injury)) + target.Attributes.PhysicsDamageReduction;
            if (InjuryType.AbilityPower == injuryType && (target.Attributes.MagicDamageReduction_a != 0 || target.Attributes.MagicDamageReduction != 0))
                damageReduce = (target.Attributes.MagicDamageReduction_a == 0 ? damageReduce : UFloat.Round(UFloat.Round(target.Attributes.MagicDamageReduction_a / 100.0f) * injury)) + target.Attributes.MagicDamageReduction;

            if (damageReduce != 0)
            {
                damageReduce = UFloat.Round(damageReduce);
                injury = Mathf.Max(1, injury - damageReduce);
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("    {0} injury {1:f6} DamageReduction :{2:f6}", target.FullName, injury, damageReduce);
#endif
            }

            //if (this.Attributes.MPDmgRecovery > 0 && oldInjury > 0)
            //{//受伤回复MP
            //    this.Remedy(this.Attributes.MPDmgRecovery, RoleAttribute.MP, this);
            //}

            if (injury <= 0)
            {
                return result;
            }

            if (from != null && target.Attributes.MaleDmgReduction > 0 && from.Data.Gender == "KEY.33675")
            {
                float reduce = target.Attributes.MaleDmgReduction * 0.01f;
                injury = injury * (1 - reduce);
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("    {0} injury {1:f6} reduce {2:f6}", target.FullName, injury, reduce);
#endif
            }

            //Final Injury Correction
            injury = double.IsNaN(injury) ? 0 : injury;
            //Immunization
            injury = Math.Max(1, injury * (1 - Immunization * EngineConst.Hundredth));

            //按照实际打出伤害
            //if (attr == RoleAttribute.MaxMP)
            //    injury = Math.Min(target.MP, injury);
            //else
            //    injury = Math.Min(target.HP, injury);
            injury = injury * target.dsFactor;

            //伤害随机+(-)5
            int rndDmg = (int)(Util.Random.Range(EngineConst.DmgRandom + 1) - 1) - EngineConst.DmgRandom / 2;
            injury += rndDmg;
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("    {0} injury {1:f6} rndDmg {2:f6}", target.FullName, injury, rndDmg);
#endif

            //Fatel Injury
            result.IsFatal = injury > target.Attributes.MaxHP * (target.Data.IRHPRatio != 0 ? target.Data.IRHPRatio : 0.08f);

            //foreach (Ability ability in target.Abilities.ToArray())
            //{
            //    //Skill extension = ability as Skill;
            //    injury = ability != null ? ability.OnInjury(injury, injuryType, fromTalent) : injury;
            //}
            injury = Math.Max(1, injury);
            for (int i = 0; i < target.tAbilities.Length; i++)
            {
                Ability ability = target.tAbilities[i] as Ability;
                injury = ability != null ? ability.OnInjury(injury, injuryType, fromTalent) : injury;
            }
            if (injury <= 0)
            {
                return result;
            }
            result.FinalInjury = UFloat.RoundToInt(injury);
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("    {0} FinalInjury {1:f6} injury:{2:f6}", target.FullName, result.FinalInjury, injury);
#endif
            return result;
        }
        InjuryResult result2 = new InjuryResult();
        /// <summary>
        /// 计算最终伤害
        /// </summary>
        /// <param name="target"></param>
        /// <param name="injuryType"></param>
        /// <param name="force"></param>
        /// <param name="from"></param>
        /// <param name="fromAbility"></param>
        /// <returns></returns>
        public virtual InjuryResult CalcFinalInjury(BattleActor target, InjuryType injuryType, float force, BattleActor from, Ability fromAbility)
        {
            result2.Reset();
            float Defense = 0;
            float PenetrationRatio = 0;
            switch (injuryType)
            {
                case InjuryType.AttackDamage:
                    {
                        //                     //护甲计算
                        //                     Defense = target.Attributes.Armor - from.Attributes.ArmorPenetration;
                        //                     //护甲穿透率
                        //                     PenetrationRatio = Math.Min(Math.Max(0, from.Attributes.ArmorPenetrationRatio), 100);
                        //                     //护甲减伤率计算
                        //                     if (Defense >= 0)
                        //                     {
                        //                         Defense = UFloat.Round(Defense/(600 + Defense * (1 + PenetrationRatio*EngineConst.Hundredth)));
                        //                     }
                        //                     else
                        //                     {
                        //                         Defense = UFloat.Round(Defense/(600 - Defense * (1 - PenetrationRatio*EngineConst.Hundredth)));
                        //                     }
                        Defense = target.Attributes.Armor;
                        float armorPenetration = from.Attributes.ArmorPenetration;
                        float armorPenetrationRatio = Math.Min(Math.Max(0, from.Attributes.ArmorPenetrationRatio), 100);
                        if (Defense <= 0)
                        {
                            armorPenetration = 0;
                            armorPenetrationRatio = 0;
                        }
                        PenetrationRatio = Math.Max(Defense * (1 - armorPenetrationRatio * EngineConst.Hundredth) - armorPenetration, 0);
                        Defense = UFloat.Round(PenetrationRatio / (600 + PenetrationRatio));
                        break;
                    }
                case InjuryType.AbilityPower:
                    {
                        //                     //魔抗
                        //                     Defense = target.Attributes.MagicResistance - from.Attributes.MagicResistIgnore;
                        //                     //魔抗穿透率
                        //                     PenetrationRatio = Math.Min(Math.Max(0, from.Attributes.MagicResistIgnoreRatio), 100);
                        //                     //魔抗减伤率
                        //                     if (Defense >= 0)
                        //                     {
                        //                         Defense = UFloat.Round(Defense/(600 + Defense*(1 + PenetrationRatio*EngineConst.Hundredth)));
                        //                     }
                        //                     else
                        //                     {
                        //                         Defense = UFloat.Round(Defense/(600 - Defense*(1 - PenetrationRatio*EngineConst.Hundredth)));
                        //                     }


                        Defense = target.Attributes.MagicResistance;
                        float magicResistIgnore = from.Attributes.MagicResistIgnore;
                        float magicResistIgnoreRatio = Math.Min(Math.Max(0, from.Attributes.MagicResistIgnoreRatio), 100);
                        if (Defense <= 0)
                        {
                            magicResistIgnore = 0;
                            magicResistIgnoreRatio = 0;
                        }
                        PenetrationRatio = Math.Max(Defense * (1 - magicResistIgnoreRatio * EngineConst.Hundredth) - magicResistIgnore, 0);
                        Defense = UFloat.Round(PenetrationRatio / (600 + PenetrationRatio));
                        break;
                    }
                default:
                    break;
            }
            force = force < 0 ? 0 : force;

            float injury = UFloat.Round(force * UFloat.Round(1 - Defense));
            injury = UFloat.RoundToInt(injury);
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("    {0} damage {1} {2:f6} {3:f6} {4:f6}", target.FullName, double.IsNaN(injury) ? "-nan" : "" + injury.ToString("f6"), force, PenetrationRatio, Defense);
#endif
            float damageRatio = from.PassiveEffect.BaseRatio;
            damageRatio = Math.Max(-1.0f, damageRatio);
            injury = UFloat.Round(injury * (damageRatio + 1.0f));
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("    {0} injury {1:f6} BaseRatio:{2:f6} damageRatio:{3:f6}", target.FullName, injury, from.PassiveEffect.BaseRatio, damageRatio);
#endif

            //Damage Reduction
            float damageReduce = 0;
            if (InjuryType.Heal != injuryType && (target.Attributes.DamageReduction_a != 0 || target.Attributes.DamageReduction != 0))
                damageReduce = target.Attributes.DamageReduction_a == 0 ? damageReduce : UFloat.Round(UFloat.Round(target.Attributes.DamageReduction_a / 100.0f) * injury) + target.Attributes.DamageReduction;
            if (InjuryType.AttackDamage == injuryType && (target.Attributes.PhysicsDamageReduction_a != 0 || target.Attributes.PhysicsDamageReduction != 0))
                damageReduce = target.Attributes.PhysicsDamageReduction_a == 0 ? damageReduce : UFloat.Round(target.Attributes.PhysicsDamageReduction_a / 100.0f * injury) + target.Attributes.PhysicsDamageReduction;
            if (InjuryType.AbilityPower == injuryType && (target.Attributes.MagicDamageReduction_a != 0 || target.Attributes.MagicDamageReduction != 0))
                damageReduce = target.Attributes.MagicDamageReduction_a == 0 ? damageReduce : UFloat.Round(UFloat.Round(target.Attributes.MagicDamageReduction_a / 100.0f) * injury) + target.Attributes.MagicDamageReduction;

            if (damageReduce != 0)
            {
                damageReduce = UFloat.Round(damageReduce);
                injury = Mathf.Max(1, injury - damageReduce);
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("    {0} injury {1:f6} damageReduce {2:f6}", target.FullName, injury, damageReduce);
#endif
            }
            injury = injury * target.dsFactor;
            //foreach (Ability ability in target.Abilities.ToArray())
            //{
            //    injury = ability != null ? ability.OnInjury(injury, injuryType, null) : injury;
            //}

            for (int i = 0; i < target.tAbilities.Length; i++)
            {
                Ability ability = target.tAbilities[i] as Ability;
                injury = ability != null ? ability.OnInjury(injury, injuryType, null) : injury;
            }
            if (injury > 0)
                result2.FinalInjury = UFloat.RoundToInt(UFloat.Round(injury));

#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("    {0} FinalInjury {1:f6} > {2}", target.FullName, injury, result2.FinalInjury);
#endif
            return result2;
        }


        public float GetLifeStealFactor(BattleActor role, BattleSkill talent)
        {
            float factor = talent.Data.LifeStealFactor * EngineConst.Hundredth;
            if (factor <= 0)
                return 0;
            if (talent.Data.DamageType == InjuryType.AbilityPower)
            {
                return UFloat.Round((role.Attributes.MagicLifeSteal + talent.Data.AddMagicLifeSteal) * EngineConst.Hundredth * factor);
            }
            if (talent.Data.DamageType == InjuryType.AttackDamage)
            {
                return UFloat.Round((role.Attributes.LifeSteal + talent.Data.AddLifeSteal) * EngineConst.Hundredth * factor);
            }
            return 0;
            //return (from.Attributes.LifeSteal + talentData.ADDLifeSteal) / (100 + from.Attributes.ADDLifeSteal + talentData.ADDLifeSteal + target.Level) * talentData.LFSPct * 0.01f;
        }

    }
}