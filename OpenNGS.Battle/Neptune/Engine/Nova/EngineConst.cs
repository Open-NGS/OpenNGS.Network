using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Neptune.GameData;

namespace Neptune
{
    public class EngineConst
    {
        public static uint XOR = 0xCDCDCDCD;
        public static bool MoveTypeA;
        /// <summary>
        /// Hundred = 100f
        /// </summary>
        public static readonly float Hundred = 100f;
        /// <summary>
        /// Hundredth = 0.01
        /// </summary>
        public static readonly float Hundredth = UFloat.Round(0.01f);
        /// <summary>
        /// Thousand = 1000
        /// </summary>
        public static readonly float Thousand = 1000f;
        /// <summary>
        /// Milli = 0.001
        /// </summary>
        public static readonly float Milli = UFloat.Round(0.001f);

        /// <summary>
        /// 逻辑帧速率定义，目标帧率15fps，间隔66毫秒
        /// 若有修改，以下三个变量的值必须匹配
        /// </summary>
        public const float KEY_FRAME_INTERVAL = 0.066f; //6.1.6

        public const int KEY_FRAME_RATE = 15;

        // 逻辑帧间隔的毫秒数，用于网络平滑算法，定义成int来节省类型转换开销
        public const int KEY_FRAME_INTERVAL_MS = 66;


        public const int SideNum = 3;

        public const int QUALITY_LIMIT = 14;


        public const bool EnableRVO = true;

        public const float RVO_EPSILON = 0.00001f;

        public const float RVO_Fault_Tolerance = .05f;

        public const float RVO_Dot_Tolerance = .8f;

        public const int RVO_NeighborDist = 100;

        public const int RVO_NeighborObstDist = 80;

        public const int RVO_NeighborTimeHorizon = 2;

        public const int RVO_NeighborObstTimeHorizon = 2;

        public const int RVO_MaxSpeedThreshold = 1000;

        public const float RVO_MinSpeedFactor = 1.0f;

        public const bool RVO_ForceSpeed = true;

        public static Vector3 RVO_Base_Vector = Vector3.right;

        public const bool RVO_RepelCollision_Building = false;

        public const int MaxTalentSlot = 9;
        /// <summary>
        /// 最小冲刺距离
        /// </summary>
        public const int MinDistance = 5;

        /// <summary>
        /// 目标类型技能加大搜索半径
        /// </summary>
        public const int AttackRange = 50;


        /// <summary>
        /// 助攻有效时长
        /// </summary>
        public const float AssistsTime = 10f;
        /// <summary>
        /// 击杀有效时长
        /// </summary>
        public const float KillTime = 10f;

        public const int DmgRandom = 10;

        public static Vector3 Vector3Zero = Vector3.zero;
        public static Vector3 Vector3One = Vector3.one;

        public static Vector2 Vector2Zero = Vector2.zero;
        public static Vector2 Vector2One = Vector2.one;

        public static UVector2 UVector2Zero = UVector2.zero;

        public const int PercentageRatio = 10000;

        /// <summary>
        ///战斗中计算距离时是否考虑半径
        /// </summary>
        public const bool EnableRadiusInDistance = true;
        public const float Gravity = -9800f;
        public const float InitBattleXFactor = 0.2f;
        public const bool ShowDamagedEffect = false;

        public const bool BattlePauseLevelEnable = true;
        //当技能没有命中目标时CD重置比率 小于0时不使用
        public const float TalentInterruptCdRate = -1;
        public const int RoleStartLevel = 0;
        /// <summary>
        /// 对野怪造成的最大伤害上限
        /// </summary>
        public const int CreepCurHpMaxDamage = 150;
        public const int RoleCollideRadiusMax = 150;
        public const int MaxRangeValue = 18500;
        /// <summary>
        /// 最低治疗能力
        /// </summary>
        public const float MinSHealHp = 0.1f;

        public const int RotateLerpSpeed = 1000;
        //出生点位置
        public static UVector3[] InitialPosition = new UVector3[]{
        new UVector3(0,90,0),
        new UVector3(-80,-80,0),
        new UVector3(-160,90,0),
        new UVector3(-240,-80,0),
        new UVector3(-320,90,0),
    };


        public static List<int> MonsterAttrs = new List<int>(){
      (int)RoleAttribute.MaxHP,
      (int)RoleAttribute.AttackDamage,
      (int)RoleAttribute.AbilityPower,
      (int)RoleAttribute.Armor,
      (int)RoleAttribute.MagicResistance,
      (int)RoleAttribute.Critical,
      (int)RoleAttribute.MagicCritical
    };

        public static Dictionary<int, int[]> NestedAbilities = new Dictionary<int, int[]>()
        {
            {(int)ControlEffect.Solidifying ,new int[]{ (int)ControlEffect.Sleep}},
            {(int)ControlEffect.Sleep ,new int[]{ (int)ControlEffect.Root, (int)ControlEffect.Inhibition, (int)ControlEffect.Disable, (int)ControlEffect.Static}},
            {(int)ControlEffect.Imprisonment ,new int[]{ (int)ControlEffect.Sleep, (int)ControlEffect.Void, (int)ControlEffect.Invincible}},
            {(int)ControlEffect.Inhuman ,new int[]{ (int)ControlEffect.Immoblilize, (int)ControlEffect.Directed}},
        };

        public static Dictionary<int, Dictionary<int, bool>> ConflictAbilities = new Dictionary<int, Dictionary<int, bool>>()
        {
            {(int)ControlEffect.Unaffected,
                new Dictionary<int, bool>{
                {(int)ControlEffect.Solidifying,true},
                {(int)ControlEffect.Sleep,true},
                {(int)ControlEffect.Root,true},
                {(int)ControlEffect.Inhibition,true},
                {(int)ControlEffect.Disable,true},
                {(int)ControlEffect.Imprisonment,true},
                {(int)ControlEffect.Charm,true},
                {(int)ControlEffect.Taunt,true},
                {(int)ControlEffect.OnlyNormalAttack,true},
                {(int)ControlEffect.SlowDown,true},}
             },
             {(int)ControlEffect.Bare,
                 new Dictionary<int, bool>{
                 {(int)ControlEffect.Invisible,true},}
             },

        };

        public static Dictionary<int, bool> ConflictingAbilities = new Dictionary<int, bool>()
        {
            {(int)ControlEffect.Solidifying,true},
            {(int)ControlEffect.Sleep,true},
            {(int)ControlEffect.Root,true},
            {(int)ControlEffect.Inhibition,true},
            {(int)ControlEffect.Disable,true},
            {(int)ControlEffect.Imprisonment,true},
            {(int)ControlEffect.Charm,true},
            {(int)ControlEffect.Taunt,true},
            {(int)ControlEffect.OnlyNormalAttack,true},
            {(int)ControlEffect.SlowDown,true},
        };

        public const string SymbolPlus = "+";
        public const string SymbolMinus = "-";

        public static Dictionary<int, bool> RatioSeparation = new Dictionary<int, bool>()
    {
        {(int)RoleAttribute.HPRegen, true},
        {(int)RoleAttribute.MPRegen, true},
        {(int)RoleAttribute.MPAtkRecovery, true},
        {(int)RoleAttribute.HPAtkRecovery, true},
        {(int)RoleAttribute.HPDecrease, true},
        {(int)RoleAttribute.DamageReduction, true},
        {(int)RoleAttribute.PhysicsDamageReduction, true},
        {(int)RoleAttribute.MagicDamageReduction, true}
    };

    }
}