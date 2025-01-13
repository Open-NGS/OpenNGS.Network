using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neptune.Datas;

/*
 *    W A R N I N G     W A R N I N G     W A R N I N G     W A R N I N G     W A R N I N G
 *    
 *    本类的字段必须严格匹配枚举属性   GameData.RoleAttribute 中的定义顺序
 *    修改后请运行编辑器菜单 [OpenNGS] -> [Development] -> [Check RoleAttributes] 执行检查。
 *    
 *    W A R N I N G     W A R N I N G     W A R N I N G     W A R N I N G     W A R N I N G
 */


/// <summary>
/// 角色属性类
/// </summary>
public struct RoleAttributes
{
    float _None;
    /// <summary>
    /// 力量
    /// </summary>
    public float Strength;
    /// <summary>
    /// 智力
    /// </summary>
    public float Intelligence;
    /// <summary>
    /// 敏捷
    /// </summary>
    public float Agility;
    /// <summary>
    /// 体质
    /// </summary>
    public float Constitution;
    /// <summary>
    /// 耐力
    /// </summary>
    public float Vitality;
    /// <summary>
    /// MaxHP（最大生命值）
    /// </summary>
    public float MaxHP;

    /// <summary>
    /// MP（最大魔法值）
    /// </summary>
    public float MaxMP;

    /// <summary>
    /// Rage（最大怒气值）
    /// </summary>
    public float MaxRage;

    /// <summary>
    /// Point（最大积点值）
    /// </summary>
    public float MaxPoint;
    /// <summary>
    /// 物理攻击力
    /// </summary>
    public float AttackDamage;
    /// <summary>
    /// 魔法强度（攻击力）
    /// </summary>
    public float AbilityPower;
    /// <summary>
    /// 物理防御力
    /// </summary>
    public float Armor;
    /// <summary>
    /// 魔法抗性（防御力）
    /// </summary>
    public float MagicResistance;
    /// <summary>
    /// 物理暴击
    /// </summary>
    public float Critical;
    /// <summary>
    /// 魔法暴击
    /// </summary>
    public float MagicCritical;
    /// <summary>
    /// 减少暴击伤害
    /// </summary>
    public float CriticalFactorReduction;
    /// <summary>
    /// 生命恢复速度（持续恢复）
    /// </summary>
    public float HPRegen;
    /// <summary>
    /// 魔法恢复速度（持续恢复）
    /// </summary>
    public float MPRegen;
    /// <summary>
    /// 魔法恢复速度（持续恢复）
    /// </summary>
    public float RageRegen;
    /// <summary>
    /// 魔法恢复速度（持续恢复）
    /// </summary>
    public float PointRegen;
    /// <summary>
    /// 持续减血速度
    /// </summary>
    public float HPDecrease;

    /// <summary>
    /// 生命回复（攻击时）
    /// </summary>
    public float HPAtkRecovery;
    /// <summary>
    /// 魔法回复（攻击时）
    /// </summary>
    public float MPAtkRecovery;
    /// <summary>
    /// 魔法回复（受伤时）
    /// </summary>
    public float MPDmgRecovery;
    /// <summary>
    /// 物理命中
    /// </summary>
    public float HitRate;
    /// <summary>
    /// 物理闪避
    /// </summary>
    public float Dodge;
    /// <summary>
    /// 物理防御穿透
    /// </summary>
    public float ArmorPenetration;
    /// <summary>
    /// 无视魔法防御
    /// </summary>
    public float MagicResistIgnore;
    /// <summary>
    /// 降低能量消耗
    /// </summary>
    public float ManaCostReduction;
    /// <summary>
    /// 治疗能力
    /// </summary>
    public float Heal;
    /// <summary>
    /// 物理吸血能力（攻击时吸取）
    /// </summary>
    public float LifeSteal;
    /// <summary>
    /// 魔法吸血能力（攻击时吸取）
    /// </summary>
    public float MagicLifeSteal;
    /// <summary>
    /// 生命回复（战斗结束）
    /// </summary>
    public float HPSupply;
    /// <summary>
    /// 魔法回复（战斗结束）
    /// </summary>
    public float MPSupply;
    /// <summary>
    /// 攻击速度
    /// </summary>
    public float AttackSpeed;
    /// <summary>
    /// 移动速度
    /// </summary>
    public float MoveSpeed;
    /// <summary>
    /// 攻击力系数
    /// </summary>
    public float PowerFactor;
    /// <summary>
    /// 物理免伤
    /// </summary>
    public float PhysicsImmunization;
    /// <summary>
    /// 魔法免伤
    /// </summary>
    public float MagicImmunization;
    /// <summary>
    /// 技能等级提升
    /// </summary>
    public float SkillLevel;
    /// <summary>
    /// 沉默抵抗率
    /// </summary>
    public float SilenceResistRate;
    /// <summary>
    /// 来自男性伤害减免
    /// </summary>
    public float MaleDmgReduction;
    /// <summary>
    /// 坚韧（提高BUFF抗性）
    /// </summary>
    public float Toughness;
    /// <summary>
    /// 暴击系数
    /// </summary>
    public float CriticalFactor;
    /// <summary>
    /// Gets or sets the damage reduction.
    /// add by xujiezhen
    /// </summary>
    public float DamageReduction;
    public float PhysicsDamageReduction;
    public float MagicDamageReduction;
    /// <summary>
    /// 攻击时额外造成物理伤害
    /// </summary>
    public float ExtraAttackDamage;
    /// <summary>
    /// 攻击时额外造成魔法伤害
    /// </summary>
    public float ExtraAbilityPower;
    /// <summary>
    /// 攻击时额外造成真实伤害
    /// </summary>
    public float ExtraHoly;

    float _BasicNum;
    float _ExtraBasicNum;
    float _BaseRatio;
    float _TargetRatio;
    float _CastNum;
    float _ShieldValue;
    float _Speed;
    float _Time;
    float _Param1;
    float _Param2;
    float _Param3;
    float _Param4;
    float _Param5;
    float _Param6;

    public float SHealHp;
    public float LostHP;
    public float LostHPValue;
    /// <summary>
    /// 技能CD减少
    /// </summary>
    public float CDReduction;
    /// <summary>
    /// 经验加成（打野）
    /// </summary>
    public float ExpAddition;
    /// <summary>
    /// 金币加成（打野）
    /// </summary>
    public float GoldAddition;
    /// <summary>
    /// 反弹伤害比率
    /// </summary>
    public float ReboundInjuryRatio;
    /// <summary>
    /// 物理防御穿透率
    /// </summary>
    public float ArmorPenetrationRatio;
    /// <summary>
    /// 磨抗忽略比率
    /// </summary>
    public float MagicResistIgnoreRatio;
    /// <summary>
    /// 吸取能力（攻击时吸取）
    /// </summary>
    float _AddLifeSteal;
    /// <summary>
    /// 魔法吸血
    /// </summary>
    float _AddMagicLifeSteal;

    /// <summary>
    /// 间隔掉血
    /// </summary>
    public float HpIntervalLost;
    /// <summary>
    /// 间隔回血
    /// </summary>
    public float HpIntervalIncrease;


    float _End;

    /// <summary>
    /// 属性比例
    /// </summary>

    public float Strength_a;
    public float Intelligence_a;
    public float Agility_a;
    public float Constitution_a;
    public float Vitality_a;
    public float MaxHP_a;
    public float MaxMP_a;
    public float MaxRage_a;
    public float MaxPoint_a;
    public float AttackDamage_a;
    public float AbilityPower_a;
    public float Armor_a;
    public float MagicResistance_a;
    public float Critical_a;
    public float MagicCritical_a;
    public float CriticalFactorReduction_a;
    public float HPRegen_a;
    public float MPRegen_a;
    public float RageRegen_a;
    public float PointRegen_a;

    public float HPDecrease_a;
    public float HPAtkRecovery_a;
    public float MPAtkRecovery_a;
    public float MPDmgRecovery_a;
    public float HitRate_a;
    public float Dodge_a;
    public float ArmorPenetration_a;
    public float MagicResistIgnore_a;
    public float ManaCostReduction_a;
    public float Heal_a;
    public float LifeSteal_a;
    public float MagicLifeSteal_a;
    public float HPSupply_a;
    public float MPSupply_a;
    public float AttackSpeed_a;
    public float MoveSpeed_a;
    public float PowerFactor_a;
    public float PhysicsImmunization_a;
    public float MagicImmunization_a;
    public float SkillLevel_a;
    public float SilenceResistRate_a;
    public float MaleDmgReduction_a;
    public float Toughness_a;
    public float CriticalFactor_a;
    public float DamageReduction_a;
    public float PhysicsDamageReduction_a;
    public float MagicDamageReduction_a;
    public float ExtraAttackDamage_a;
    public float ExtraAbilityPower_a;
    public float ExtraHoly_a;



    float _BasicNum_a;
    float _ExtraBasicNum_a;
    float _BaseRatio_a;
    float _TargetRatio_a;
    float _CastNum_a;
    float _ShieldValue_a;
    float _Speed_a;
    float _Time_a;
    float _Param1_a;
    float _Param2_a;
    float _Param3_a;
    float _Param4_a;
    float _Param5_a;
    float _Param6_a;

    public float SHealHp_a;
    public float LostHP_a;
    public float LostHPValue_a;
    public float CDReduction_a;
    public float ExpAddition_a;
    public float GoldAddition_a;
    public float ReboundInjuryRatio_a;
    public float ArmorPenetrationRatio_a;
    public float MagicResistIgnoreRatio_a;
    float _AddLifeSteal_a;
    float _AddMagicLifeSteal_a;
    public float HpIntervalLost_a;
    public float HpIntervalIncrease_a;


    float _End_a;


    unsafe float* Values;
    public unsafe float this[int enumValue]
    {
        get
        {
#if MEMORY_PROTECT
            return MathUtil.XOR(this.Values[enumValue]);
#else
            return this.Values[enumValue];
#endif
        }
        set
        {
#if MEMORY_PROTECT
            this.Values[enumValue] = MathUtil.XOR(value);
#else
            this.Values[enumValue] = value;
#endif
        }
    }

    public unsafe void Init()
    {
        fixed (float* ptr = &this._None)
        {
            this.Values = ptr;
        }
    }

    public unsafe void Reset()
    {
        throw new System.NotImplementedException();
//        for (int i = 0; i < (int)RoleAttribute.RATIOMAX; i++)
//        {
//#if MEMORY_PROTECT
//                this.Values[i] = MathUtil.XOR(0f);
//#else
//            this.Values[i] = 0f;
//#endif
//        }
    }


    public unsafe bool Inited
    {
        get { return this.Values != null; }
    }

    public const string PF_X = "_x";
    public const string PF_S = "_s";
    public const string PF_A = "_a";
    public const string PF_E = "E_";
    public const string PF_P = "Plus";

    public static string GetExtraNameField(string name)
    {
        return name + PF_S;
    }

    public static RoleAttribute GetStartElement()
    {
        return RoleAttribute.GoldAttack;
    }


    public static RoleAttribute GetEndElement()
    {
        return RoleAttribute.DustReduction;
    }
}