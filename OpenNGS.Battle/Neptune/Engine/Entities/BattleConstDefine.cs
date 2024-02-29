
//这边的类型都需要转为静态数据表读取
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
}