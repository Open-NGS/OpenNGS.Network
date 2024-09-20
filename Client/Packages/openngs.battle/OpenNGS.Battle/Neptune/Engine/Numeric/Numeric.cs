using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neptune.Datas;
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
        public virtual float CalculateSkillPower(Skill talent, Entity from, Actor target, ref float ratio)
        {
            float force = UFloat.Round(UFloat.Round(talent.Data.BaseRatio * talent.Caster.AttributeFinalValue[(int)talent.Data.BaseAttr]) + talent.Data.BasicNum);
            if (talent.Data.BaseRatioEx > 0)
                force += UFloat.Round(talent.Data.BaseRatioEx * talent.Caster.AttributeFinalValue[(int)talent.Data.BaseAttrEx]);
            if (talent.Data.TargetRatio > 0)
                force = UFloat.Round(force + force * (talent.Data.TargetRatio * target.AttributeFinalValue[(int)talent.Data.TargetAttr]));

            ratio = 1;
            if (from != null && from.DamageFactor > 0)
                ratio = from.DamageFactor;

            //根据目标类型影响攻击力
            if ((talent.Data.TargetTypeAffect & target.Data.RoleType) > 0 || ((talent.Data.TargetTypeAffect & RoleType.Demon) == RoleType.Demon && target.Config.IsDemon))
            {
                ratio = UFloat.Round(ratio * talent.Data.TargetTypeRatio);
            }
            force = UFloat.Round(force * ratio);
            if (talent.Data.ExtraDamageTargetRatio > 0)
            {
                float extraDamage = UFloat.Round(talent.Data.ExtraDamageTargetRatio * target.AttributeFinalValue[(int)talent.Data.ExtraDamageTargetAttr]);
                if ((talent.Data.TargetTypeAffect & target.Data.RoleType) > 0 || ((talent.Data.TargetTypeAffect & RoleType.Demon) == RoleType.Demon && target.Config.IsDemon))
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
        public bool SkillDodge(Skill talent, Actor target, InjuryType injuryType)
        {
            if (injuryType == InjuryType.AttackDamage && !talent.Data.NoDodge)
            {
                float attributesDodg = Math.Max(0, target.AttributeFinalValue.Dodge - talent.Caster.AttributeFinalValue.HitRate);
                if ((attributesDodg / (100 + attributesDodg)) > Util.Random.Rand())
                {
                    return true;
                }
            }
            return false;
        }
        protected DamageResult result = new DamageResult();
        /// <summary>
        /// 计算最终伤害
        /// </summary>
        /// <param name="target">攻击目标</param>
        /// <param name="injuryType">伤害类型</param>
        /// <param name="attr">目标属性</param>
        /// <param name="force">攻击力</param>
        /// <param name="from">攻击来源</param>
        /// <returns></returns>
        public virtual DamageResult CalcFinalInjury(Actor target, InjuryType injuryType, RoleAttribute attr, float force, Actor from, Skill fromSkill)
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
                        Defense = target.AttributeFinalValue.Armor;
                        float armorPenetration = from.AttributeFinalValue.ArmorPenetration;
                        float armorPenetrationRatio = Math.Min(Math.Max(0, from.AttributeFinalValue.ArmorPenetrationRatio), 100);
                        if (Defense <= 0)
                        {
                            armorPenetration = 0;
                            armorPenetrationRatio = 0;
                        }
                        PenetrationRatio = Math.Max(Defense * (1 - armorPenetrationRatio * NeptuneConst.Hundredth) - armorPenetration, 0);
                        Defense = UFloat.Round(PenetrationRatio / (600 + PenetrationRatio));
                        Critical = from.AttributeFinalValue.Critical;
                        Immunization = target.AttributeFinalValue.PhysicsImmunization;
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


                        Defense = target.AttributeFinalValue.MagicResistance;
                        float magicResistIgnore = from.AttributeFinalValue.MagicResistIgnore;
                        float magicResistIgnoreRatio = Math.Min(Math.Max(0, from.AttributeFinalValue.MagicResistIgnoreRatio), 100);
                        if (Defense <= 0)
                        {
                            magicResistIgnore = 0;
                            magicResistIgnoreRatio = 0;
                        }
                        PenetrationRatio = Math.Max(Defense * (1 - magicResistIgnoreRatio * NeptuneConst.Hundredth) - magicResistIgnore, 0);
                        Defense = UFloat.Round(PenetrationRatio / (600 + PenetrationRatio));
                        Critical = from.AttributeFinalValue.MagicCritical;
                        Immunization = target.AttributeFinalValue.MagicImmunization;
                        break;
                    }
                default:
                    break;
            }

            result.IsCritical = target.InjuryCrit * Critical * NeptuneConst.Hundredth > Util.Random.Rand() && !target.IsRoleType(RoleType.Building);
            float criticalFactor = 100;
            if (result.IsCritical)
            {
                criticalFactor = from.AttributeFinalValue.CriticalFactor + 200;//from.Attributes.CriticalFactor > 0 ? from.Attributes.CriticalFactor + 200 : 200;
                criticalFactor = UFloat.Round(Math.Min(350f, Math.Max(100, criticalFactor - target.AttributeFinalValue.CriticalFactorReduction)));
            }

            force = force < 0 ? 0 : force;
            //float injury = force * force / (force + Defense * target.InjuryFactor);
            float injury = UFloat.Round(force * UFloat.Round((1 - Defense)));
            injury = UFloat.RoundToInt(injury);
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("    {0} damage {1} {2} {3:f6} {4:f6} ({5:f6},{6:f6},{7:f6},{8:f6}) {9:f6}", target.FullName, Double.IsNaN(injury) ? "-nan" : "" + injury.ToString("f6"), attr, force, result.IsCritical ? "(Crit!)" : "", Critical, Immunization, Defense, target.Attributes.CriticalFactorReduction, target.InjuryCrit);
#endif
            if (criticalFactor != 100 || from.AttributeFinalValue.PowerFactor != 1)
            {
                injury = UFloat.Round(injury * (criticalFactor * NeptuneConst.Hundredth) * from.AttributeFinalValue.PowerFactor);
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("    {0} injury {1:f6} criticalFactor {2:f6} PowerFactor:{3:f6}", target.FullName, injury, criticalFactor, from.Attributes.PowerFactor);
#endif
            }

            float damageRatio = from.PassiveEffect.BaseRatio;
            int extDmgRatio = 0;
            if (fromSkill != null)
            {
                extDmgRatio = fromSkill.GetExtraDamageRatio(target.Data.RoleType);
                damageRatio += extDmgRatio * NeptuneConst.Hundredth;
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

            result.IsImmunization = Immunization * NeptuneConst.Hundredth >= 1;
            if (result.IsImmunization)
            {
                return result;
            }

            //Damage Reduction
            float damageReduce = 0;
            if (InjuryType.Heal != injuryType && (target.AttributeFinalRatio.DamageReduction != 0 || target.AttributeFinalValue.DamageReduction != 0))
                damageReduce = (target.AttributeFinalRatio.DamageReduction == 0 ? damageReduce : UFloat.Round(UFloat.Round(target.AttributeFinalRatio.DamageReduction / 100.0f) * injury)) + target.AttributeFinalValue.DamageReduction;
            if (InjuryType.AttackDamage == injuryType && (target.AttributeFinalRatio.PhysicsDamageReduction != 0 || target.AttributeFinalValue.PhysicsDamageReduction != 0))
                damageReduce = (target.AttributeFinalRatio.PhysicsDamageReduction == 0 ? damageReduce : UFloat.Round((target.AttributeFinalRatio.PhysicsDamageReduction / 100.0f) * injury)) + target.AttributeFinalValue.PhysicsDamageReduction;
            if (InjuryType.AbilityPower == injuryType && (target.AttributeFinalRatio.MagicDamageReduction != 0 || target.AttributeFinalValue.MagicDamageReduction != 0))
                damageReduce = (target.AttributeFinalRatio.MagicDamageReduction == 0 ? damageReduce : UFloat.Round(UFloat.Round(target.AttributeFinalRatio.MagicDamageReduction / 100.0f) * injury)) + target.AttributeFinalValue.MagicDamageReduction;

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

            if (from != null && target.AttributeFinalValue.MaleDmgReduction > 0 && from.Data.Gender == "KEY.33675")
            {
                float reduce = target.AttributeFinalValue.MaleDmgReduction * 0.01f;
                injury = injury * (1 - reduce);
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("    {0} injury {1:f6} reduce {2:f6}", target.FullName, injury, reduce);
#endif
            }

            //Final Injury Correction
            injury = Double.IsNaN(injury) ? 0 : injury;
            //Immunization
            injury = Math.Max(1, injury * (1 - Immunization * NeptuneConst.Hundredth));

            //按照实际打出伤害
            //if (attr == RoleAttribute.MaxMP)
            //    injury = Math.Min(target.MP, injury);
            //else
            //    injury = Math.Min(target.HP, injury);
            injury = injury * target.dsFactor;

            //伤害随机+(-)5
            int rndDmg = (int)(Util.Random.Range(NeptuneConst.DmgRandom + 1) - 1) - NeptuneConst.DmgRandom / 2;
            injury += rndDmg;
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("    {0} injury {1:f6} rndDmg {2:f6}", target.FullName, injury, rndDmg);
#endif

            //Fatel Injury
            result.IsFatal = injury > target.AttributeFinalValue.MaxHP * (target.Data.IRHPRatio != 0 ? target.Data.IRHPRatio : 0.08f);

            //foreach (Ability ability in target.Abilities.ToArray())
            //{
            //    //Skill extension = ability as Skill;
            //    injury = ability != null ? ability.OnInjury(injury, injuryType, fromSkill) : injury;
            //}
            injury = Math.Max(1, injury);
            for (int i = 0; i < target.tAbilities.Length; i++)
            {
                Ability ability = target.tAbilities[i] as Ability;
                injury = ability != null ? ability.OnInjury(injury, injuryType, fromSkill) : injury;
            }
            if (injury <= 0)
            {
                return result;
            }
            result.FinalDamage = UFloat.RoundToInt(injury);
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("    {0} FinalDamage {1:f6} injury:{2:f6}", target.FullName, result.FinalDamage, injury);
#endif
            return result;
        }
        DamageResult result2 = new DamageResult();
        /// <summary>
        /// 计算最终伤害
        /// </summary>
        /// <param name="target"></param>
        /// <param name="injuryType"></param>
        /// <param name="force"></param>
        /// <param name="from"></param>
        /// <param name="fromAbility"></param>
        /// <returns></returns>
        public virtual DamageResult CalcFinalInjury(Actor target, InjuryType injuryType, float force, Actor from, Ability fromAbility)
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
                        //                         Defense = UFloat.Round(Defense/(600 + Defense * (1 + PenetrationRatio*NeptuneConst.Hundredth)));
                        //                     }
                        //                     else
                        //                     {
                        //                         Defense = UFloat.Round(Defense/(600 - Defense * (1 - PenetrationRatio*NeptuneConst.Hundredth)));
                        //                     }
                        Defense = target.AttributeFinalValue.Armor;
                        float armorPenetration = from.AttributeFinalValue.ArmorPenetration;
                        float armorPenetrationRatio = Math.Min(Math.Max(0, from.AttributeFinalValue.ArmorPenetrationRatio), 100);
                        if (Defense <= 0)
                        {
                            armorPenetration = 0;
                            armorPenetrationRatio = 0;
                        }
                        PenetrationRatio = Math.Max(Defense * (1 - armorPenetrationRatio * NeptuneConst.Hundredth) - armorPenetration, 0);
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
                        //                         Defense = UFloat.Round(Defense/(600 + Defense*(1 + PenetrationRatio*NeptuneConst.Hundredth)));
                        //                     }
                        //                     else
                        //                     {
                        //                         Defense = UFloat.Round(Defense/(600 - Defense*(1 - PenetrationRatio*NeptuneConst.Hundredth)));
                        //                     }


                        Defense = target.AttributeFinalValue.MagicResistance;
                        float magicResistIgnore = from.AttributeFinalValue.MagicResistIgnore;
                        float magicResistIgnoreRatio = Math.Min(Math.Max(0, from.AttributeFinalValue.MagicResistIgnoreRatio), 100);
                        if (Defense <= 0)
                        {
                            magicResistIgnore = 0;
                            magicResistIgnoreRatio = 0;
                        }
                        PenetrationRatio = Math.Max(Defense * (1 - magicResistIgnoreRatio * NeptuneConst.Hundredth) - magicResistIgnore, 0);
                        Defense = UFloat.Round(PenetrationRatio / (600 + PenetrationRatio));
                        break;
                    }
                default:
                    break;
            }
            force = force < 0 ? 0 : force;

            float injury = UFloat.Round(force * UFloat.Round((1 - Defense)));
            injury = UFloat.RoundToInt(injury);
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("    {0} damage {1} {2:f6} {3:f6} {4:f6}", target.FullName, Double.IsNaN(injury) ? "-nan" : "" + injury.ToString("f6"), force, PenetrationRatio, Defense);
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
            if (InjuryType.Heal != injuryType && (target.AttributeFinalRatio.DamageReduction != 0 || target.AttributeFinalValue.DamageReduction != 0))
                damageReduce = target.AttributeFinalRatio.DamageReduction== 0 ? damageReduce : UFloat.Round(UFloat.Round(target.AttributeFinalRatio.DamageReduction / 100.0f) * injury) + target.AttributeFinalValue.DamageReduction;
            if (InjuryType.AttackDamage == injuryType && (target.AttributeFinalRatio.PhysicsDamageReduction != 0 || target.AttributeFinalValue.PhysicsDamageReduction != 0))
                damageReduce = target.AttributeFinalRatio.PhysicsDamageReduction == 0 ? damageReduce : UFloat.Round((target.AttributeFinalRatio.PhysicsDamageReduction / 100.0f) * injury) + target.AttributeFinalValue.PhysicsDamageReduction;
            if (InjuryType.AbilityPower == injuryType && (target.AttributeFinalRatio.MagicDamageReduction != 0 || target.AttributeFinalValue.MagicDamageReduction != 0))
                damageReduce = target.AttributeFinalRatio.MagicDamageReduction == 0 ? damageReduce : UFloat.Round(UFloat.Round(target.AttributeFinalRatio.MagicDamageReduction / 100.0f) * injury) + target.AttributeFinalValue.MagicDamageReduction;

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
                result2.FinalDamage = UFloat.RoundToInt(UFloat.Round(injury));

#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("    {0} FinalDamage {1:f6} > {2}", target.FullName, injury, result2.FinalDamage);
#endif
            return result2;
        }


        public float GetLifeStealFactor(Actor role, Skill talent)
        {
            float factor = talent.Data.LifeStealFactor * NeptuneConst.Hundredth;
            if (factor <= 0)
                return 0;
            if (talent.Data.DamageType == InjuryType.AbilityPower)
            {
                return UFloat.Round((role.AttributeFinalValue.MagicLifeSteal + talent.Data.AddMagicLifeSteal) * NeptuneConst.Hundredth * factor);
            }
            if (talent.Data.DamageType == InjuryType.AttackDamage)
            {
                return UFloat.Round((role.AttributeFinalValue.LifeSteal + talent.Data.AddLifeSteal) * NeptuneConst.Hundredth * factor);
            }
            return 0;
            //return (from.Attributes.LifeSteal + talentData.ADDLifeSteal) / (100 + from.Attributes.ADDLifeSteal + talentData.ADDLifeSteal + target.Level) * talentData.LFSPct * 0.01f;
        }

    }

}