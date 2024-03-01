
//这边的类型都需要转为静态数据表读取
using System.Collections.Generic;

namespace Neptune.GameData
{
    /// <summary>
    /// 阵营
    /// </summary>
    public enum RoleSide
    {
        /// <summary>
        /// 无阵营
        /// </summary>
        None = 0,

        /// <summary>
        /// 所有阵营
        /// </summary>
        All = -1,

        /// <summary>
        /// 阵营A：通常为玩家己方
        /// </summary>
        SideA = 1,

        /// <summary>
        /// 阵营B：通常为玩家敌对方
        /// </summary>
        SideB = 2,

        /// <summary>
        /// 阵营C：通常为中立第三方阵营
        /// </summary>
        SideC = 3,
    }

    /// <summary>
    /// 相对阵营
    /// </summary>
    public enum RelativeSide
    {
        /// <summary>
        /// 友方阵营
        /// </summary>
        Friend = 1,
        /// <summary>
        /// 敌对阵营
        /// </summary>
        Enemy = -1,
        /// <summary>
        /// 双方
        /// </summary>
        Both = 0,
    }

    public enum ArenaFlag
    {
        SELFRIGHT,
        SELFLEFT,
        OPPONENTRIGHT,
        OPPONENTLEFT,
        NULL
    }
    public class NodeData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool Ignorable { get; set; }
        public ArenaFlag ArenaFlag { get; set; }
        public bool Tower { get; set; }
    }
    public enum TeamSide
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// 主场队伍
        /// </summary>
        Home = 1,
        /// <summary>
        /// 客场队伍
        /// </summary>
        Visitor = 2,

    }
    /// <summary>
    /// 单位类型
    /// </summary>
    public enum RoleType
    {
        /// <summary>
        /// 任意类型
        /// </summary>
        Any = 0,
        /// <summary>
        /// 原生物
        /// </summary>
        Natural = 1,
        /// <summary>
        /// 恶魔类
        /// </summary>
        Demon = 2,
        /// <summary>
        /// 力量型
        /// </summary>
        Strength = 4,
        /// <summary>
        /// 敏捷型
        /// </summary>
        Agility = 8,
        /// <summary>
        /// 智力型
        /// </summary>
        Intelligence = 16,

        /// <summary>
        /// 士兵
        /// </summary>
        Troop = 32,

        /// <summary>
        /// 英雄
        /// </summary>
        Hero = 64,
        /// <summary>
        /// 怪物
        /// </summary>
        Monster = 128,
        /// <summary>
        /// 建筑
        /// </summary>
        Building = 256,
        /// <summary>
        /// 技能
        /// </summary>
        Spell = 512,
        /// <summary>
        /// 近战
        /// </summary>
        Melee = 1024,
        /// <summary>
        /// 远程
        /// </summary>
        Remote = 2048,
    }

    public enum HeroType
    {

    }

    public enum ControlEffect
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// 固定（不可移动，待机）
        /// </summary>
        Root = 1,
        /// <summary>
        /// 沉默（禁止使用非物理攻击）
        /// </summary>
        Inhibition = 2,
        /// <summary>
        /// 残废（禁止使用物理攻击）
        /// </summary>
        Disable = 3,
        /// <summary>
        /// 静止（停止一切行为）
        /// </summary>
        Static = 4,
        /// <summary>
        /// 不可锁定（不能作为攻击目标）
        /// </summary>
        Void = 5,
        /// <summary>
        /// 无敌（不受任何攻击影响）
        /// </summary>
        Invincible = 6,
        /// <summary>
        /// 不受影响（免疫一切控制效果）
        /// </summary>
        Unaffected = 7,
        /// <summary>
        /// 魅惑（更改目标阵营）
        /// </summary>
        Charm = 8,
        /// <summary>
        /// 稳定（不会被击飞击退、拉传）
        /// </summary>
        Immoblilize = 9,
        /// <summary>
        /// 修正（重置角色方向）
        /// </summary>
        Directed = 10,
        /// <summary>
        /// 不可治疗（无法通过任何方式恢复生命）
        /// </summary>
        Incurable = 11,
        /// <summary>
        /// 不可回复（无法自动回复生命）
        /// </summary>
        Norecover = 12,
        /// <summary>
        /// 精神锁链（无法释放任何技能）
        /// </summary>
        MindChain = 13,
        /// <summary>
        /// 精神强化（不受精神锁链控制）
        /// </summary>
        MindGain = 14,
        /// <summary>
        /// 催眠
        /// </summary>
        Sleep = 15,//Root | Inhibition | Disable | Static,//15
        /// <summary>
        /// 禁锢
        /// </summary>
        Imprisonment = 16,//Sleep | Void | Invincible,//63
        /// <summary>
        /// 凝固
        /// </summary>
        Solidifying = 17,//Sleep,//15
        /// <summary>
        /// 建筑物
        /// </summary>
        Inhuman = 18,//Immoblilize | Directed
        /// <summary>
        /// 只能普通攻击
        /// </summary>
        OnlyNormalAttack = 19,
        /// <summary>
        /// 被嘲讽
        /// </summary>
        Taunt = 20,
        /// <summary>
        /// 恐惧
        /// </summary>
        Fear = 21,

        /// <summary>
        /// 重生
        /// </summary>
        Rebirth = 22,

        /// <summary>
        /// 隐身
        /// </summary>
        Invisible = 23,
        /// <summary>
        /// 减速
        /// </summary>
        SlowDown = 24,
        /// <summary>
        /// 缴械
        /// </summary>
        Disarmed = 25,
        /// <summary>
        /// 暴露Grass Invisible
        /// </summary>
        Bare = 26,
        /// <summary>
        /// 草丛
        /// </summary>
        Grass = 27,
        /// <summary>
        /// 暴露视野 Grass
        /// </summary>
        Vision = 28,
        /// <summary>
        /// 只攻击建筑
        /// </summary>
        OnlyAttackBuilding = 29,
        MAX
    }
    /// <summary>
    /// 战斗状态
    /// </summary>
    public enum BattleStatus
    {
        Ready = 0,
        /// <summary>
        /// 正常
        /// </summary>
        Normal,
        /// <summary>
        /// 2倍速
        /// </summary>
        Double,
        /// <summary>
        /// 暴死模式
        /// </summary>
        SuddenDeath,

        /// <summary>
        /// 结束
        /// </summary>
        Finish
    }
    public class ElementStatus
    {
        public int ID;
        public float Duration;
        public int Frame;
    }


    public enum AttackAIMode
    {
        /// <summary>
        /// 自动选择技能
        /// </summary>
        AutoSelectSkill,
        /// <summary>
        /// 缓存技能
        /// </summary>
        CacheSkill
    }


    public enum RoleState
    {
        /// <summary>
        /// 基本
        /// </summary>
        Base = 0,
        /// <summary>
        /// 空闲
        /// </summary>
        Idle = 1,
        /// <summary>
        /// 移动
        /// </summary>
        Move = 2,
        /// <summary>
        /// 攻击
        /// </summary>
        Attack = 3,
        /// <summary>
        /// 受伤
        /// </summary>
        Damaged = 4,
        /// <summary>
        /// 晕眩状态
        /// </summary>
        Stun = 5,
        /// <summary>
        /// 死亡状态
        /// 死亡动画播放阶段
        /// </summary>
        Death = 6,
        /// <summary>
        /// 死亡状态
        /// 挺尸阶段
        /// </summary>
        Dead = 7,
        /// <summary>
        /// 出生
        /// </summary>
        Birth = 8,
        /// <summary>
        /// 胜利
        /// </summary>
        Cheer = 9,
        /// <summary>
        /// MAX
        /// </summary>
        Max = 10,

        /// <summary>
        /// 扩展
        /// </summary>
        Transform = 11,

        // 名称映射类使用的最大下标，扩展状态类型时需要同步更新
        RoleStateMax = 12,
    }
    /// <summary>
    /// 特效类型
    /// </summary>
    public enum EffectType
    {
        Normal,//一般特效
        Cast,//抛射器
        Bounce,//弹射器
        Ability, //BUFF特效
        Hit,
        Trap //
    }

    public enum ActionEventType
    {
        /// <summary>
        /// 攻击帧
        /// </summary>
        HitFrame,
        /// <summary>
        /// 释放技能
        /// </summary>
        Talent,
        /// <summary>
        /// 冲刺
        /// </summary>
        Dash,
        /// <summary>
        /// 传送
        /// </summary>
        Teleport,
        /// <summary>
        /// 技能释放结束
        /// </summary>
        End
    }

    public enum RoleAttribute : int
    {
        None = 0,
        /// <summary>
        /// 力量
        /// </summary>
        Strength,
        /// <summary>
        /// 智力
        /// </summary>
        Intelligence,
        /// <summary>
        /// 敏捷
        /// </summary>
        Agility,
        /// <summary>
        /// 体质
        /// </summary>
        Constitution,
        /// <summary>
        /// 耐力
        /// </summary>
        Vitality,
        /// <summary>
        /// 最大HP（生命值）
        /// </summary>
        MaxHP,
        /// <summary>
        /// 最大MP（魔法值）
        /// </summary>
        MaxMP,
        /// <summary>
        /// 最大Rage（怒气值）
        /// </summary>
        MaxRage,
        /// <summary>
        /// 最大Point(积点值)
        /// </summary>
        MaxPoint,
        /// <summary>
        /// 物理攻击力
        /// </summary>
        AttackDamage,
        /// <summary>
        /// 魔法强度（攻击力）
        /// </summary>
        AbilityPower,
        /// <summary>
        /// 物理防御力
        /// </summary>
        Armor,
        /// <summary>
        /// 魔法抗性（防御力）
        /// </summary>
        MagicResistance,
        /// <summary>
        /// 物理暴击
        /// </summary>
        Critical,
        /// <summary>
        /// 魔法暴击
        /// </summary>
        MagicCritical,
        /// <summary>
        /// 减免暴击伤害
        /// </summary>
        CriticalFactorReduction,
        /// <summary>
        /// 生命恢复速度（持续恢复）
        /// </summary>
        HPRegen,
        /// <summary>
        /// 魔法恢复速度（持续恢复）
        /// </summary>
        MPRegen,
        /// <summary>
        /// 怒气恢复速度（持续恢复）
        /// </summary>
        RageRegen,
        /// <summary>
        /// 积点恢复速度（持续恢复）
        /// </summary>
        PointRegen,
        /// <summary>
        /// 持续减血速度
        /// </summary>
        HPDecrease,
        /// <summary>
        /// 生命回复（攻击时）
        /// </summary>
        HPAtkRecovery,
        /// <summary>
        /// 魔法回复（攻击时）
        /// </summary>
        MPAtkRecovery,
        /// <summary>
        /// 魔法回复（受伤时）
        /// </summary>
        MPDmgRecovery,
        /// <summary>
        /// 物理命中
        /// </summary>
        HitRate,
        /// <summary>
        /// 物理闪避
        /// </summary>
        Dodge,
        /// <summary>
        /// 物理防御穿透
        /// </summary>
        ArmorPenetration,
        /// <summary>
        /// 无视魔法防御
        /// </summary>
        MagicResistIgnore,
        /// <summary>
        /// 降低能量消耗
        /// </summary>
        ManaCostReduction,
        /// <summary>
        /// 治疗能力
        /// </summary>
        Heal,
        /// <summary>
        /// 吸取能力（攻击时吸取）
        /// </summary>
        LifeSteal,
        /// <summary>
        /// 魔法吸血
        /// </summary>
        MagicLifeSteal,
        /// <summary>
        /// 生命回复（战斗结束）
        /// </summary>
        HPSupply,
        /// <summary>
        /// 魔法回复（战斗结束）
        /// </summary>
        MPSupply,
        /// <summary>
        /// 攻击速度
        /// </summary>
        AttackSpeed,
        /// <summary>
        /// 移动速度
        /// </summary>
        MoveSpeed,
        /// <summary>
        /// 攻击力系数
        /// </summary>
        PowerFactor,
        /// <summary>
        /// 物理免伤
        /// </summary>
        PhysicsImmunization,
        /// <summary>
        /// 魔法免伤
        /// </summary>
        MagicImmunization,
        /// <summary>
        /// 技能等级提升
        /// </summary>
        SkillLevel,
        /// <summary>
        /// 沉默抵抗率
        /// </summary>
        SilenceResistRate,
        /// <summary>
        /// 男性减伤
        /// </summary>
        MaleDmgReduction,
        /// <summary>
        /// 坚韧（提高BUFF抗性）
        /// </summary>
        Toughness,
        /// <summary>
        /// 暴击系数
        /// </summary>
        CriticalFactor,
        /// <summary>
        /// Gets or sets the damage reduction.
        /// add by xujiezhen
        /// </summary>
        DamageReduction,
        PhysicsDamageReduction,
        MagicDamageReduction,
        /// <summary>
        /// 攻击时额外造成物理伤害
        /// </summary>
        ExtraAttackDamage,
        /// <summary>
        /// 攻击时额外造成魔法伤害
        /// </summary>
        ExtraAbilityPower,
        /// <summary>
        /// 攻击时额外造成真实伤害
        /// </summary>
        ExtraHoly,


        BasicNum,
        ExtraBasicNum,
        BaseRatio,
        TargetRatio,
        CastNum,
        ShieldValue,
        Speed,
        Time,
        Param1,
        Param2,
        Param3,
        Param4,
        Param5,
        Param6,
        SHealHp,
        LostHP,
        LostHPValue,
        /// <summary>
        /// 技能CD减少
        /// </summary>
        CDReduction,
        /// <summary>
        /// 经验加成（打野）
        /// </summary>
        ExpAddition,
        /// <summary>
        /// 金币加成（打野）
        /// </summary>
        GoldAddition,
        /// <summary>
        /// 反弹伤害比率
        /// </summary>
        ReboundInjuryRatio,

        /// <summary>
        /// 物理防御穿透率
        /// </summary>
        ArmorPenetrationRatio,
        /// <summary>
        /// 磨抗忽略比率
        /// </summary>
        MagicResistIgnoreRatio,
        /// <summary>
        /// 吸取能力（攻击时吸取）
        /// </summary>
        AddLifeSteal,
        /// <summary>
        /// 魔法吸血
        /// </summary>
        AddMagicLifeSteal,

        /// <summary>
        /// 间隔掉血
        /// </summary>
        HpIntervalLost,

        /// <summary>
        /// 间隔回血
        /// </summary>
        HpIntervalIncrease,

        _End,

        MAX,

        /// <summary>
        /// Ratio Value Region
        /// </summary>
        RATIOBASE = MAX - 1,

        RATIOMAX = 2 * MAX - 1,

        PLUSBASE = RATIOMAX - 1,

        PLUSMAX = 3 * MAX
    }

    public class AnimationConfigData
    {
        public string Animation { get; set; }
        public string Role { get; set; }
        public float TotalTime { get; set; }
    }


    /// <summary>
    /// 技能状态
    /// </summary>
    public enum TalentStatus
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// 技能开始
        /// </summary>
        Start = 1,
        /// <summary>
        /// HitFrame 
        /// </summary>
        HitFrame = 2,
        /// <summary>
        /// 技能结束
        /// </summary>
        End = 3
    }


    /// <summary>
    /// 技能检查结果类型
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// 可以释放
        /// </summary>
        Success = 0,
        /// <summary>
        /// 失败，默认值
        /// </summary>
        Failed,
        /// <summary>
        /// 同一帧
        /// </summary>
        SameTick,
        /// <summary>
        /// 技能还在等待冷却
        /// </summary>
        Cooldown,
        /// <summary>
        /// 技能不够血量
        /// </summary>
        HPNotEnough,
        /// <summary>
        /// 技能不够魔法
        /// </summary>
        MPNotEnough,
        /// <summary>
        /// 眩晕
        /// </summary>
        Stun,
        /// <summary>
        /// 残废（禁止使用物理攻击）
        /// </summary>
        Disable,
        /// <summary>
        /// 沉默（禁止使用技能攻击）
        /// </summary>
        Silence,
        /// <summary>
        /// 没有目标
        /// </summary>
        NoTarget,
        /// <summary>
        /// 对应的目标不能成为技能的目标
        /// </summary>
        Untargetable,
        /// <summary>
        /// 对应的目标阵营不一样
        /// </summary>
        SideDiff,
        /// <summary>
        /// 目标太远
        /// </summary>
        TooFar,
        /// <summary>
        /// 目标太近
        /// </summary>
        TooNear,
        /// <summary>
        /// 玩家超出屏幕
        /// </summary>
        OutOfScreen,
        /// <summary>
        /// 技能释放中
        /// </summary>
        TalentCastring,
        /// <summary>
        /// 精神锁链（无法释放任何技能）
        /// </summary>
        MindChain,
        /// <summary>
        /// 只能普通攻击
        /// </summary>
        OnlyNormalAttack,
        /// <summary>
        /// 恐惧状态
        /// </summary>
        Fear,
        /// <summary>
        /// 超出视野
        /// </summary>
        OutOfSightRange,
        /// <summary>
        /// 缴械
        /// </summary>
        Disarmed,
    }

    /// <summary>
    /// 技能触发类型
    /// </summary>
    public enum TriggerType
    {
        /// <summary>
        /// 自动释放
        /// </summary>
        Auto = 1,
        /// <summary>
        /// 手动释放
        /// </summary>
        Manual = 2,
        /// <summary>
        /// 死亡释放
        /// </summary>
        Death = 4,
        /// <summary>
        /// 生命每减少百分比，数值配置在 Param1
        /// </summary>
        HPLostPerPct = 8,
        /// <summary>
        /// 任何其他单位死亡
        /// </summary>
        UnitDeath = 16,
        /// <summary>
        /// 任何友方死亡
        /// </summary>
        FriendDeath = 32,
        /// <summary>
        /// 击杀目标触发
        /// </summary>
        KillTarget = 64,
        /// <summary>
        /// 血量低于m时触发（TriggerParam百分比）
        /// </summary>
        HPUnder = 128,
        /// <summary>
        /// 被攻击时触发（TriggerParam伤害类型）
        /// </summary>
        IsAttacked = 256,
        /// <summary>
        /// 攻击时触发（TriggerParam技能/普攻）
        /// </summary>
        Attack = 512,
        /// <summary>
        /// 治疗时触发
        /// </summary>
        Remedy = 1024,
        /// <summary>
        /// 造成伤害时触发
        /// </summary>
        Damage = 2048,
        /// <summary>
        /// 基于CD触发
        /// </summary>
        TriggerOnCD = 4096,
        /// <summary>
        /// 技能升级时触发
        /// </summary>
        TalentLevel = 8192,
        /// <summary>
        /// buff触发 （TriggerParam）
        /// </summary>
        Ability = 16384,
        /// <summary>
        /// 助攻触发
        /// </summary>
        Assist = 32768,
        /// <summary>
        /// 暴击触发
        /// </summary>
        Crit = 65536,
        /// <summary>
        /// 复活时触发
        /// </summary>
        Reborn = 131072,
        /// <summary>
        /// 升级时触发
        /// </summary>
        LevelUp = 262144,
        /// <summary>
        /// 破盾触发
        /// </summary>
        BrokeShield = 524288,
        /// <summary>
        /// 技能冷却触发
        /// </summary>
        ColdDown = 1048576,
        /// <summary>
        /// 蓝量低于触发
        /// </summary>
        MpUnder = 2097152,
        /// <summary>
        /// 移动触发
        /// </summary>
        Moving = 4194304,
    }

    /// <summary>
    /// Ability触发类型
    /// </summary>
    public enum AbilityTriggerType
    {
        Hit = 0,
        Start = 1,
        Death = 2
    }

    /// <summary>
    /// 基于 TriggerType Ability
    /// </summary>
    public enum TriggerAbilityType
    {
        /// <summary>
        /// buff结束时
        /// </summary>
        End = 0,
        /// <summary>
        /// 添加控制效果时
        /// </summary>
        Control = 1
    }


    /// <summary>
    /// 移除Ability的类型
    /// </summary>
    public enum AbilityRemoveType
    {
        /// <summary>
        /// 攻击时
        /// </summary>
        OnHit = 1,
        /// <summary>
        /// 受到伤害时
        /// </summary>
        OnHurt = 2,
    }

    /// <summary>
    /// BUFF传递触发类型
    /// </summary>
    public enum TransferTriggerType
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// 拥有者死亡时
        /// </summary>
        OwnerDead = 1,
    }




    /// <summary>
    /// buff传递的目标类型
    /// </summary>
    public enum TransferTargetType
    {
        /// <summary>
        ///无目标 
        /// </summary>
        None = 0,
        /// <summary>
        /// 攻击者
        /// </summary>
        Attacker = 1,
        /// <summary>
        /// 攻击者一方
        /// </summary>
        AttackerSide = 2,
        /// <summary>
        /// 获得人头者 
        /// </summary>
        FinallyHitPlayer = 3,
    }

    /// <summary>
    /// 印记触发类型
    /// </summary>
    public enum MarkTriggerType
    {

        /// <summary>
        /// 伤害计算后
        /// </summary>
        AfterInijury = 0,
        /// <summary>
        /// 伤害计算前
        /// </summary>
        BeforeInijury = 1,
    }
    /// <summary>
    /// Ability触发类型
    /// </summary>
    public enum AbilityTriggerMode
    {
        /// <summary>
        /// 伤害计算后
        /// </summary>
        AfterInijury = 0,
        /// <summary>
        /// 伤害计算前
        /// </summary>
        BeforeInijury = 1,
    }

    /// <summary>
    /// 技能命中类型
    /// </summary>
    public enum TalentHitType
    {
        /// <summary>
        /// 技能命中
        /// </summary>
        TalentHit = 0,
        /// <summary>
        /// 子弹命中
        /// </summary>
        EffectHit = 1,
        /// <summary>
        /// 子弹到达飞行距离
        /// </summary>
        EffectEnd = 2,

    }

    /// <summary>
    /// 吸引方向类型
    /// </summary>
    public enum AttractDirectType
    {
        /// <summary>
        /// 施法者
        /// </summary>
        Caster = 0,
        /// <summary>
        /// 目标点
        /// </summary>
        TargetPosition = 1,
        /// <summary>
        /// 施法者中心
        /// </summary>
        CasterCenter = 2,
        /// <summary>
        /// 击退朝向
        /// </summary>
        SourceDirection = 3,
    }

    /// <summary>
    /// 添加技能UI效果类型
    /// </summary>
    public enum AddTalentUIEffectType
    {
        /// <summary>
        /// 开始时
        /// </summary>
        TalentStart = 1,
        /// <summary>
        /// 替换时
        /// </summary>
        TalentChange = 2,
    }
    /// <summary>
    /// 添加印记类型
    /// </summary>
    public enum AddMarkType
    {
        /// <summary>
        /// 命中时
        /// </summary>
        Hit = 0,
        /// <summary>
        /// 开始时
        /// </summary>
        Start = 1

    }

    /// <summary>
    /// 技能类型
    /// </summary>
    public enum TalentType
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0,
        /// <summary>
        /// 主动技能
        /// </summary>
        Active = 1,
        /// <summary>
        /// 被动技能
        /// </summary>
        Passive = 2,
        /// <summary>
        /// 友方光环
        /// </summary>
        SelfAureole = 3,
        /// <summary>
        /// 敌方光环
        /// </summary>
        EnemyAureole = 4,
    }

    /// <summary>
    /// 技能类型
    /// </summary>
    public enum SkillType
    {
        /// <summary>
        /// 技能
        /// </summary>
        Skill = 0,
        /// <summary>
        /// 普攻
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 子技能
        /// </summary>
        ChildSkill = 2,
        /// <summary>
        /// 无
        /// </summary>
        None = 3,
    }

    /// <summary>
    /// 对召唤单位的行为类型
    /// </summary>
    public enum SummonTodoType
    {
        None = 0,
        /// <summary>
        /// KillSummon
        /// </summary>
        Dead = 1,
        /// <summary>
        /// 使召唤物释放技能
        /// </summary>
        SummonCastTalent = 2,


    }
    /// <summary>
    /// 技能升级类型
    /// </summary>
    public enum LevelUpType
    {
        /// <summary>
        /// 自定义
        /// </summary>
        None = 0,
        /// <summary>
        /// 自动升级
        /// </summary>
        Auto = 1,
        /// <summary>
        /// 主动升级
        /// </summary>
        Manual = 2,
    }

    public enum InjuryType
    {
        None = 0,
        AttackDamage = 1,
        AbilityPower = 2,
        Holy = 3,
        Heal
    }


    public enum CDMode
    {
        None = 0,
        /// <summary>
        /// 技能开始
        /// </summary>
        Start = 1,
        /// <summary>
        /// HitFrame 
        /// </summary>
        HitFrame = 2,
        /// <summary>
        /// 技能结束
        /// </summary>
        End = 3

    }

    /// <summary>
    /// 消耗MP
    /// </summary>
    public enum CostMPMode
    {
        /// <summary>
        /// 技能开始
        /// </summary>
        Start = 0,
        /// <summary>
        /// HitFrame 
        /// </summary>
        HitFrame = 1,
        /// <summary>
        /// 技能结束
        /// </summary>
        End = 2

    }

    /// <summary>
    /// 可打断的技能类型
    /// </summary>
    public enum InterruptSkillType
    {
        /// <summary>
        /// 所有
        /// </summary>
        ALL = 0,
        /// <summary>
        /// 技能
        /// </summary>
        Skill = 1,
        /// <summary>
        /// 普攻
        /// </summary>
        Normal = 2,
    }

    /// <summary>
    /// 打断类型
    /// </summary>
    public enum InterruptType
    {
        /// <summary>
        /// 主动打断
        /// </summary>
        Active = 1,
        /// <summary>
        /// 被动打断
        /// </summary>
        Passive = 2,
        /// <summary>
        /// 受到攻击时
        /// </summary>
        OnIsAttacked = 4,
        /// <summary>
        /// 替换技能
        /// </summary>
        ChangeTalent = 8,
    }
    /// <summary>
    /// 护盾种类
    /// </summary>
    public enum ShieldClass
    {
        /// <summary>
        /// 计量护盾
        /// </summary>
        Measure = 0,
        /// <summary>
        /// 充能护盾
        /// </summary>
        Rechargeable = 1,
        /// <summary>
        /// <summary>
        /// 不死护盾
        /// </summary>
        Deathless = 2,
        /// <summary>
        /// 吸收护盾
        /// </summary>
        Absorption = 3,
        /// <summary>
        /// 伤害减免护盾
        /// </summary>
        DamageReduce = 4,

    }

    /// <summary>
    /// 护盾类型
    /// </summary>
    public enum ShieldType
    {
        /// <summary>
        /// 全部免疫
        /// </summary>
        All = 0,
        /// <summary>
        /// 物理免疫
        /// </summary>
        AttackDamage = 1,
        /// <summary>
        /// 魔法免疫
        /// </summary>
        AbilityPower = 2
    }

    /// <summary>
    /// 护盾吸收伤害类型
    /// </summary>
    public enum ShieldAbsorbType
    {
        /// <summary>
        /// 所有类型
        /// </summary>
        All = 0,
        /// <summary>
        /// 普攻
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 技能
        /// </summary>
        Talent = 2,
    }

    public enum PopupType
    {
        None = 0,
        Damage = 1,
        Heal = 2,
        Gold = 3,
        AttackDamage,
        AbilityDamage,
        HolyDamage,
        Enchant = 10,
        BlueOfElimination,
        ArmorImpaired,
        AttackWeakend,
        Dodge,
        EXILE,
        Funeral,
        Haste,
        Void,
        Imprison,
        ArmorEnhanced,
        AttackEnhanced,
        KillingRewards,
        MagVoid,
        Miss,
        MultipleSpells2,
        MultipleSpells3,
        MultipleSpells4,
        PhyVoid,
        Resistance,
        Summon,
        TimeOver,
        StealMp,
        Silence = 101,//沉默
        SlowDown = 102,//减速
        Imprisonment = 103,//禁锢
        Taunt = 104,//嘲讽
        Inhibition = 105,//沉默
        Solidifying = 106,//石化
        Static = 107,//眩晕
        BlowFly = 108,//击飞
        CoolDown = 109,//冷却
        InsufficientMagic = 110,//魔法不足
        NoTarget = 111,//没有目标
        ShieldFade = 112,//护盾消失
        Frozen = 113,//冰冻
        Disarm = 114,//缴械
        Sheep = 115,//变羊
        Absorbed = 116,//吸收
        RedBuff = 117,//红buff
        BlueBuff = 118,//蓝buff
        KillBoss = 119,//击杀boss
    }

    /// <summary>
    /// 目标阵营
    /// </summary>
    public enum TargetType
    {
        None = 0,
        /// <summary>
        /// 自己
        /// </summary>
        Self = 1,
        /// <summary>
        /// 目标
        /// </summary>
        Target = 2,
        /// <summary>
        /// 随机
        /// </summary>
        Random = 3,
        /// <summary>
        /// 所有
        /// </summary>
        All = 4,
        /// <summary>
        /// 尸体
        /// </summary>
        DeadBody = 5,

        /// <summary>
        /// 位置
        /// </summary>
        Position = 6,
        /// <summary>
        /// 指定方向
        /// </summary>
        Direction = 7,

        /// <summary>
        /// 最近
        /// </summary>
        Nearest = 10,
        /// <summary>
        /// 最远
        /// </summary>
        Farthest = 11,

        /// <summary>
        /// 最弱(血量百分比最低)
        /// </summary>
        Weakest = 12,

        /// <summary>
        /// HP 最多
        /// </summary>
        MaxHP = 13,
        /// <summary>
        /// HP 最少
        /// </summary>
        MinHP = 14,

        /// <summary>
        /// MP 最多
        /// </summary>
        MaxMP = 15,
        /// <summary>
        /// MP 最少
        /// </summary>
        MinMP = 16,

        /// <summary>
        /// 力量最高
        /// </summary>
        MaxStrength = 17,
        /// <summary>
        /// 敏捷最高
        /// </summary>
        MaxAgility = 18,
        /// <summary>
        /// 智力最高
        /// </summary>
        MaxIntelligence = 19,

        /// <summary>
        /// 物理攻击最高
        /// </summary>
        MaxAttackDamage = 20,

        /// <summary>
        /// 射程最远
        /// </summary>
        MaxRange = 21,

        /// <summary>
        /// 拥有者契约者
        /// </summary>
        Owner = 22,

        NearestSummon = 101
    }

    public class TalentAction
    {
        public string ActionName;
        public double TotalTime;
        public List<GameData.AniEventData> Events;
    }


    public enum LandSeaAirMode
    {
        /// <summary>
        /// 全部
        /// </summary>
        All = 0,
        /// <summary>
        /// 地面单位
        /// </summary>
        Ground = 1,
        /// <summary>
        /// 空中单位
        /// </summary>
        Air = 2,
        /// <summary>
        /// 海上单位
        /// </summary>
        Sea = 4,
    }

    /// <summary>
    /// 特效播放类型
    /// </summary>
    public enum EffectPlayType
    {
        /// <summary>
        /// 攻击帧
        /// </summary>
        OnHitFrame = 0,
        /// <summary>
        /// 释放时
        /// </summary>
        OnCast = 1,
        /// <summary>
        /// 击中时
        /// </summary>
        OnHitAt = 2,
    }
    /// <summary>
    /// 特效释放位置参考类型
    /// </summary>
    public enum EffectPosRefType
    {
        /// <summary>
        /// 默认，SELF-AREA 时为 Caster，否则为技能命中位置
        /// </summary>
        Default = 0,
        /// <summary>
        /// 施法者位置
        /// </summary>
        Caster = 1,
        /// <summary>
        /// 目标位置
        /// </summary>
        Target = 2,
        /// <summary>
        /// 场景原点
        /// </summary>
        Origin = 3
    }

    /// <summary>
    /// 特效释放方向参考类型
    /// </summary>
    public enum EffectDirRefType
    {
        /// <summary>
        /// 施法者方向
        /// </summary>
        Caster = 0,
        /// <summary>
        /// 目标方向
        /// </summary>
        Target = 1,
        /// <summary>
        /// 无方向
        /// </summary>
        None = 2
    }
    /// <summary>
    /// 弹射目标类型
    /// </summary>
    public enum FlyBounceMode
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default = 0,
        /// <summary>
        /// 新目标
        /// </summary>
        NewTarget = 1,
        /// <summary>
        /// 最近的目标
        /// </summary>
        Nearest = 2,
    }

    /// <summary>
    /// 自动返回类型
    /// </summary>
    public enum FlyBackType
    {
        /// <summary>
        /// 不返回
        /// </summary>
        None = 0,
        /// <summary>
        /// 回到施法者
        /// </summary>
        Caster = 1,
        /// <summary>
        /// 回到子弹出生点
        /// </summary>
        StartPos = 2
    }


    public enum EffectID //对应Effect配置表
    {
        /// <summary>
        /// 出生复活特效
        /// </summary>
        Reborn = 419,
        /// <summary>
        /// 被击杀特效
        /// </summary>
        Killed = 420,
        /// <summary>
        /// 升级特效
        /// </summary>
        Upgrade = 421,
        /// <summary>
        /// 野怪死亡给予特效
        /// </summary>
        CreepDeathGift = 422,
    }

    /// <summary>
    /// 子弹伤害衰减类型
    /// </summary>
    public enum CastEffectDmgFactorType
    {
        /// <summary>
        /// 单个目标计算
        /// </summary>
        SingleTarget = 0,
        /// <summary>
        /// 所有目标计算
        /// </summary>
        All = 1,
    }

    /// <summary>
    /// 技能持续施法类型
    /// </summary>
    public enum ContinuousType
    {
        None = 0,
        //持续施法
        Continuous = 1,
        //激活类技能
        Activate = 2
    }


    public enum LimitTriggerType
    {
        None = 0,
        Enter = 1,
        Leave = 2,
        Interval = 4,
        Manual = 8
    }
    public enum TrapTriggerType
    {
        None = 0,
        Start = 1,
        EffectEnd = 2,
        Hit = 4

    }

    /// <summary>
    /// BUFF叠加类型
    /// </summary>
    public enum OverlayType
    {
        /// <summary>
        /// 始终叠加
        /// </summary>
        Always = 0,
        /// <summary>
        /// 跳过
        /// </summary>
        Skip = 1,
        /// <summary>
        /// 替换
        /// </summary>
        Replace = 2,
        /// <summary>
        /// 重置时间
        /// </summary>
        ResetTime = 3,
        /// <summary>
        /// 已原有Ability时间覆盖添加Ability时间
        /// </summary>
        CoverTime = 4,
        /// <summary>
        /// 仅重置时间
        /// </summary>
        OnlyResetTime = 5,
    }

    /// <summary>
    /// Area Shape
    /// </summary>
    public enum AreaShape
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// 圆形区域
        /// </summary>
        Circle = 1,
        /// <summary>
        /// 1/2圆形区域
        /// </summary>
        SemiCircle = 2,
        /// <summary>
        /// 1/4圆形区域
        /// </summary>
        Quadrant = 3,
        /// <summary>
        /// 矩形区域
        /// </summary>
        Rectangle = 4,
        /// <summary>
        /// 扇形
        /// </summary>
        Sector = 5
    }

    /// <summary>
    /// 子弹生成位置
    /// </summary>
    public enum EffectPointType
    {
        /// <summary>
        /// 施法者
        /// </summary>
        CasterPoint = 0,
        /// <summary>
        /// 陷阱
        /// </summary>
        TrapPoint = 1,
    }
    public enum CastType
    {
        /// <summary>
        /// 瞬间造成伤害
        /// </summary>
        None = 0,
        /// <summary>
        /// //抛射器. 普通远程攻击
        /// </summary>
        Cast = 1,
        /// <summary>
        /// //弹射器,击中一个目标后，会继续找下一个目标进行弹射
        /// </summary>
        Bounce = 2,
        /// <summary>
        /// //多重箭效果，针对同一个目标多个攻击
        /// </summary>
        Multiple = 3,
        /// <summary>
        /// //分裂箭，针对多个目标的攻击
        /// </summary>
        Divided = 4,
        /// <summary>
        /// //回旋镖, CastNum是个数
        /// </summary>
        Boomerang = 5,
        /// <summary>
        ///  //范围分散攻击，Param2是角度,CastNum是放出的个数
        /// </summary>
        Scatter = 6,
        /// <summary>
        /// 环绕类型的子弹
        /// </summary>
        Round = 7,
    }

    public enum MPType
    {
        None = 0,
        Mp = 1,
        Rage = 2,
        Point = 3,
    }

    public enum AttackMode
    {
        /// <summary>
        /// 普通模式
        /// 普通模式每次攻击前均会重新检索攻击目标
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 锁定模式
        /// 锁定模式会锁定当前目标持续攻击直到目标死亡或特殊情况出现
        /// </summary>
        Lock = 1,
    }

    /// <summary>
    /// 目标筛选
    /// </summary>
    public enum LockMode
    {

        None = 0,

        /// <summary>
        /// HP最少
        /// </summary>

        MinHp = 1,
        /// <summary>
        /// 距离最近
        /// </summary>

        Nearest = 2,

    }

    public enum FindTargetMode
    {
        None = 0,
        /// <summary>
        /// 普通
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 补刀优先
        /// </summary>
        LastHit = 2,
        /// <summary>
        /// 塔刀优先 
        /// </summary>
        TowerHit = 3,
    }

    public enum OperatingMode
    {
        None = 0,
        /// <summary>
        /// 普通模式
        /// </summary>
        NormalMode = 1,
        /// <summary>
        /// 补刀模式
        /// </summary>
        LastHitMode = 2,
    }

    /// <summary>
    /// 矢量类型
    /// </summary>
    public enum VectorType
    {
        X = 0,
        XY = 1,
        XYZ = 2,
    }

    public enum RoleSkinKeyType
    {
        /// <summary>
        /// 皮肤
        /// </summary>
        RoleSkin = 1,
        /// <summary>
        /// 角色
        /// </summary>
        Role = 2,
        /// <summary>
        /// 技能
        /// </summary>
        Talent = 3,
        /// <summary>
        /// BUFF
        /// </summary>
        Ability = 4,
        /// <summary>
        /// 陷阱
        /// </summary>
        Trap = 5,
        /// <summary>
        /// 标记
        /// </summary>
        Mark = 6,
        /// <summary>
        /// 事件帧 
        /// </summary>
        Event = 7,
    }
    /// <summary>
    /// 事件类型
    /// </summary>
    public enum RoleSkinEventType
    {
        PlaySound,
        PlayEffect,
    }

    /// <summary>
    /// 单位位置类型
    /// </summary>
    public enum RolePosition
    {
        /// <summary>
        /// 任意类型
        /// </summary>
        All = 0,
        /// <summary>
        /// 前排
        /// </summary>
        Front = 1,
        /// <summary>
        /// 中排
        /// </summary>
        Central = 2,
        /// <summary>
        /// 后排
        /// </summary>
        Back = 3
    }
    /// <summary>
    /// 种族类型
    /// </summary>
    public enum RaceType
    {
        /// <summary>
        /// 中立
        /// </summary>
        Neutral = 0,
        /// <summary>
        /// 人族
        /// </summary>
        Human = 1,
        /// <summary>
        /// 兽族
        /// </summary>
        Orc = 2,
        /// <summary>
        /// 不死族
        /// </summary>
        Undead = 3,
        /// <summary>
        /// 精灵族
        /// </summary>
        Elf = 4,
    }

    /// <summary>
    /// Attribute Type
    /// </summary>
    public enum AttributeType
    {
        /// <summary>
        /// 最终属性
        /// </summary>
        Final = 0,
        /// <summary>
        /// 基础属性
        /// </summary>
        Base = 1,
        /// <summary>
        /// 额外属性
        /// </summary>
        Extra = 2,
        /// <summary>
        /// 技能属性
        /// </summary>
        Ability = 3,
        /// <summary>
        /// 上限
        /// </summary>
        Max
    }

    /// <summary>
    /// 屏幕效果
    /// </summary>
    public enum ScreenEffect
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 冻结
        /// </summary>
        Freeze,
        /// <summary>
        /// 震动
        /// </summary>
        Shake,

    }

    public enum TEAM_SOURCE
    {
        HOME = 1,
        VISITOR
    }


    // RoleState与名字的映射类，扩展RoleState时需要把对应的名称添加到下面的映射中
    // 逻辑代码中使用RoleStateName.Instance[RoleState]代替RoleState.ToString()
    public class RoleStateName
    {
        public static RoleStateName Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RoleStateName();
                }
                return _instance;
            }
        }

        public string this[RoleState index]
        {
            get { return _names[(int)index]; }
        }

        private static RoleStateName _instance = null;

        private string[] _names = null;
        private RoleStateName()
        {
            _names = new string[(int)RoleState.RoleStateMax];
            _names[(int)RoleState.Base] = "Base";
            _names[(int)RoleState.Idle] = "Idle";
            _names[(int)RoleState.Move] = "Move";
            _names[(int)RoleState.Attack] = "Attack";
            _names[(int)RoleState.Damaged] = "Damaged";
            _names[(int)RoleState.Stun] = "Stun";
            _names[(int)RoleState.Death] = "Death";
            _names[(int)RoleState.Dead] = "Dead";
            _names[(int)RoleState.Birth] = "Birth";
            _names[(int)RoleState.Cheer] = "Cheer";
            _names[(int)RoleState.Max] = "Max";
            _names[(int)RoleState.Transform] = "Transform";
        }
    }


    public enum IndicatorType
    {
        None = 0,
        Range = 1,
        Point = 2,
        Direction = 3,
        Fanshaped = 4,
        Rectangle = 5,
        Fanshaped2 = 6, // 扇形半径连到圆心
        Direction2 = 7, // 不显示外圈圆的方向
        MulDirection = 8, // 多方向行技能指示器（类似诸葛亮1技能）
        FixedRectangle = 9 // 固定朝向的矩形
    }
}