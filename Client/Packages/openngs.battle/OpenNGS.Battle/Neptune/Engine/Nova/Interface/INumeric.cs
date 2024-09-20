using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neptune.GameData;

namespace Neptune
{

    /// <summary>
    /// Numeric interface, provide numeric logic for BattleEngine
    /// </summary>
    public interface INumeric
    {

        /// <summary>
        /// 计算技能基础攻击力
        /// </summary>
        /// <param name="talent">技能</param>
        /// <param name="from">攻击来源</param>
        /// <param name="target">攻击目标</param>
        /// <param name="ratio">攻击力系数</param>
        /// <returns></returns>
        float CalculateTalentForce(BattleSkill talent, BattleEntity from, BattleActor target, ref float ratio);


        /// <summary>
        /// 计算技能闪避
        /// </summary>
        /// <param name="talent">技能</param>
        /// <param name="target">攻击目标</param>
        /// <param name="injuryType">伤害类型</param>
        /// <returns></returns>
        bool TalentDodge(BattleSkill talent, BattleActor target, InjuryType injuryType);

        /// <summary>
        /// 计算最终伤害
        /// </summary>
        /// <param name="target">攻击目标</param>
        /// <param name="injuryType">伤害类型</param>
        /// <param name="attr">目标属性</param>
        /// <param name="force">攻击力</param>
        /// <param name="from">攻击来源</param>
        /// <returns></returns>
        InjuryResult CalcFinalInjury(BattleActor target, InjuryType injuryType, RoleAttribute attr, float force, BattleActor from, BattleSkill fromTalent);

        /// <summary>
        /// 计算最终伤害
        /// </summary>
        /// <param name="target"></param>
        /// <param name="injuryType"></param>
        /// <param name="force"></param>
        /// <param name="from"></param>
        /// <param name="fromAbility"></param>
        /// <returns></returns>
        InjuryResult CalcFinalInjury(BattleActor target, InjuryType injuryType, float force, BattleActor from, Ability fromAbility);

        /// <summary>
        /// 计算角色吸血系数
        /// </summary>
        /// <param name="role"></param>
        /// <param name="talent"></param>
        /// <returns></returns>
        float GetLifeStealFactor(BattleActor role, BattleSkill talent);

    }
}