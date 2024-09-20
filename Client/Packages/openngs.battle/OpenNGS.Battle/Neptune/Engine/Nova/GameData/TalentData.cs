using System;
using System.Collections.Generic;

namespace Neptune.GameData
{
    [Serializable]
    public class TalentData : GameDataArray<float>
    {
        public int Level { get; set; }
        public List<AbilityData> Abilities { get; set; }

        public int ID { get; set; }
        public int TalentGroupID { get; set; }
        public string TalentName { get; set; }
        public string Description { get; set; }
        public string DescriptionEx { get; set; }
        public bool OutOfScreen { get; set; }
        public TriggerType TriggerType { get; set; }
        public int TriggerParam { get; set; }
        public int TriggerParam1 { get; set; }
        public TalentType ActiveType { get; set; }
        public string AuraAffectTag { get; set; }
        public int MaxRange { get; set; }
        public int MinRange { get; set; }
        public int SightRange { get; set; }
        public bool Instant { get; set; }

        /// <summary>
        /// 可以手动打断的技能
        /// </summary>
        public bool ManualInterrupt { get; set; }

        /// <summary>
        /// 施法时可以移动
        /// </summary>
        public bool MovingCast { get; set; }
        public bool UnfreezeTarget { get; set; }
        public int MaxTargetNum { get;set; }

        public int InitCD { get; set; }
        public int CD { get; set; }
        public int GCD { get; set; }

        public RelativeSide TargetSide { get; set; }
        public RoleType TargetClass { get; set; }
        public TargetType TargetType { get; set; }
        public LandSeaAirMode TargetLSAMode { get; set; }
        public RoleType IgnoreTarget { get; set; }

        public ContinuousType ContinuousType { get; set; }

        public float ContinuousTime { get; set; }
        public float ContinuousInterval { get; set; }
        public int IndicatoryType { get; set; }
        public List<float> IndicatoryParams { get; set; }

        public TargetType AreaCenter { get; set; }
        public RelativeSide AffectedSide { get; set; }
        public int AreaShift { get; set; }
        public int AreaShiftIncr { get; set; }
        public AreaShape AreaShape { get; set; }
        public int AreaArg1 { get; set; }
        public int AreaArg2 { get; set; }

        public int AffiliatedAreaShift { get; set; }
        public AreaShape AffiliatedAreaShape { get; set; }
        public int AffiliatedAreaArg1 { get; set; }
        public int AffiliatedAreaArg2 { get; set; }

        public AreaShape IgnoreAreaShape { get; set; }
        public int IgnoreAreaArg1 { get; set; }
        public int IgnoreAreaArg2 { get; set; }

        public int MainTargetAddRange { get; set; }

        public List<int> AbilityIDs { get; set; }
        public List<int> RemoveTargetAbilityIDs { get; set; }
        public List<int> RemoveCasterAbilityIDs { get; set; }
        public List<int> RemoveTargetMarkIDs { get; set; }
        public List<int> RemoveCasterMarkIDs { get; set; }
        public List<int> RemoveCasterTrapIDs { get; set; }
        public bool Dispel { get; set; }

        /// <summary>
        /// 根据Base属性计算Ratio值
        /// </summary>
        public bool RatioByBase { get; set; }

        public InjuryType DamageType { get; set; }
        public float BasicNum { get { return base.Values[(int)RoleAttribute.BasicNum]; } set { base.Values[(int)RoleAttribute.BasicNum] = value; } }
        public RoleType TargetTypeAffect { get; set; }
        public float TargetTypeRatio { get; set; }
        public float ExtraTargetTypeRatio { get; set; }
        public bool AffectMP { get; set; }
        public InjuryType ExtraDamageType { get; set; }
        public int ExtraBasicNum { get { return (int)base.Values[(int)RoleAttribute.ExtraBasicNum]; } set { base.Values[(int)RoleAttribute.ExtraBasicNum] = value; } }
        public bool ExtraAffectMP { get; set; }
        
        public int CostMP { get; set; }
        public float GainMP { get; set; }
        public int LFSPct { get; set; }
        public float AddLifeSteal { get { return base.Values[(int)RoleAttribute.AddLifeSteal]; } set { base.Values[(int)RoleAttribute.AddLifeSteal] = value; } }
        public float AddMagicLifeSteal { get { return base.Values[(int)RoleAttribute.AddMagicLifeSteal]; } set { base.Values[(int)RoleAttribute.AddMagicLifeSteal] = value; } }
        public int StealPct { get; set; }
        public int StealMpPct { get; set; }
        public int CRITPct { get; set; }
        public RoleAttribute BaseAttr { get; set; }
        public float BaseRatio { get { return base.Values[(int)RoleAttribute.BaseRatio]; } set { base.Values[(int)RoleAttribute.BaseRatio] = value; } }
        public RoleAttribute BaseAttrEx { get; set; }
        public float BaseRatioEx { get; set; }
        public RoleAttribute TargetAttr { get; set; }
        public float TargetRatio { get { return base.Values[(int)RoleAttribute.TargetRatio]; } set { base.Values[(int)RoleAttribute.TargetRatio] = value; } }

        public RoleAttribute PassiveAttr { get; set; }
        public RoleAttribute ExtraDamageTargetAttr { get; set; }
        public float ExtraDamageTargetRatio { get; set; }
        public int CurHpDamageRatio { get; set; }
        public List<string> Actions { get; set; }
        public string CastingEffect { get; set; }
        public string HitEffect { get; set; }
        public string HitSound { get; set; }
        public int HitOrder { get; set; }
        public string DirectEffect { get; set; }
        public int DirectOrder { get; set; }
        public string ContinuousEffect { get; set; }
        public List<EffectData> ExtraEffects { get; set; }

        public CastType CastType { get; set; }
        public float CastNum { get { return base.Values[(int)RoleAttribute.CastNum]; } set { base.Values[(int)RoleAttribute.CastNum] = value; } }
        public int CastTimes { get; set; }
        public VectorType CastVector { get; set; }
        public string FlyResource { get; set; }
        public int FlyRange { get; set; }
        public int FlyGravity { get; set; }
        public int FlyEffectiveHeight { get; set; }
        public bool FlyPiercing { get; set; }
        public int FlySpeed { get; set; }
        public int FlySpeedZ { get; set; }
        public int FlyWarpSpeed { get; set; }
        public float FlyBackSpeedRatio { get; set; }
        public bool FlyLockTarget { get; set; }
        public bool FlyTrackingTarget { get; set; }
        public FlyBackType FlyBackType { get; set; }
        public int FlyBounce { get; set; }
        public FlyBounceMode FlyBounceMode { get; set; }
        public List<float> FlyDmgFactor { get; set; }
        public int FlyRadius { get; set; }
        public int FlyRoundSpeed { get; set; }
        public List<float> FlyStartOffset { get; set; }
        public List<float> FlyDestOffset { get; set; }

        public string BounceEffect { get; set; }
        public float BounceInterval { get; set; }

        /// <summary>
        /// 子弹弹射范围
        /// </summary>
        public int FlyBounceRange { get; set; }

        public int BounceAbilityID { get; set; }
        /// <summary>
        ///分裂箭角度
        /// </summary>
        public List<int> FlyCastDirections { get; set; }

        public float EndEvent { get; set; }

        public float RepelUpTime { get; set; }
        public float RepelBackTime { get; set; }
        public float RepelAcceleration { get; set; }
        public List<int> RepelOffset { get; set; }
        public int RepelBackDist { get; set; }
        public bool RepelResistIgnorance { get; set; }
        public int MoveForward { get; set; }

        /// <summary>
        /// 击飞的高度
        /// </summary>
        public float RepelHeight { get; set; }
        /// <summary>
        /// 击飞的重力加速度因子
        /// </summary>
        public float RepelGravityFactor { get; set; }

        public bool NoDodge { get; set; }
        public bool NoSpeeder { get; set; }

        public bool NoCDReduct { get; set; }

        //召唤
        public int SummonID { get; set; }
        public int SummonLevel { get; set; }
        public int SummonNumber { get; set; }

        public string Extend { get; set; }
        public float Param1 { get { return base.Values[(int)RoleAttribute.Param1]; } set { base.Values[(int)RoleAttribute.Param1] = value; } }
        public float Param2 { get { return base.Values[(int)RoleAttribute.Param2]; } set { base.Values[(int)RoleAttribute.Param2] = value; } }
        public float Param3 { get { return base.Values[(int)RoleAttribute.Param3]; } set { base.Values[(int)RoleAttribute.Param3] = value; } }
        public float Param4 { get { return base.Values[(int)RoleAttribute.Param4]; } set { base.Values[(int)RoleAttribute.Param4] = value; } }
        public float Param5 { get { return base.Values[(int)RoleAttribute.Param5]; } set { base.Values[(int)RoleAttribute.Param5] = value; } }
        public float Param6 { get { return base.Values[(int)RoleAttribute.Param6]; } set { base.Values[(int)RoleAttribute.Param6] = value; } }

        public List<int> TrapIds { get; set; }
        public int TriggerTrapID { get; set; }
        public List<int> MarksToAdd { get; set; }     //要添加到目标身上的印记
        public List<int> MarksToTrigger { get; set; } //试图触发的目标身上的印记
        public List<int> MarksToSelf { get; set; }     //要添加到自己身上的印记

        public List<int> TalentEventParam { get; set; }
        public List<float> DashEventParam { get; set; }
        public List<int> TeleportEventParam { get; set; }
        public List<int> ExtraDamageRatioByRoleType { get; set; } //技能对单位类型的额外伤害

        public bool CanAttackAir { get; set; }

        public RoleType FlyPiercingType { get; set; }
        //技能对特定目标类型的额外伤害

        public bool onlyAttackFightingTarget { get; set; }

        public bool RepelIgnoreObstacle { get; set; }
        public string EffectSound { get; set; }
        public List<string> TargetExtraDamage
        {
            set
            {
                if (value.Count > 0 && value.Count%4 == 0)
                {
                    _targetExtraDamageList = new List<TargetExtraDamageData>();
                    for (int i = 0; i < value.Count; i += 4)
                    {
                        TargetExtraDamageData data = new TargetExtraDamageData();
                        if (data.Parse(value[i], value[i + 1], value[i + 2], value[i + 3]))
                        {
                            _targetExtraDamageList.Add(data);
                        }
                    }
                }
            }
        } 

        private List<TargetExtraDamageData> _targetExtraDamageList = null;

        public List<TargetExtraDamageData> GetTargetExtraDamageData()
        {
            return _targetExtraDamageList;
        } 

        public float ProtectTime { get; set; }
        public float SkillProtectTime { get; set; }

        public TalentStatus CDMode { get; set; }

        public bool NoSilence { get; set; }

        public int LifeStealFactor { get; set; }

        public bool Unrestricted { get; set; }

        public string StartSound { get; set; }
        public List<string> PlayVoice { get; set; }

        public SkillType SkillType { get; set; }

        public InterruptSkillType InterruptSkillType { get; set; }

        public InterruptType InterruptType { get; set; }

        public bool ForcedInterrupt { get; set; }

        public bool FollowSourceTalentTarget { get; set; }

        public string MiniMapEffect { get; set; }

        public float ApportionRatio { get; set; }

        public float ApportionTotalRatio { get; set; }

        public bool RemoveCastingEffect { get; set; }

        public float ResetComboIndexTime { get; set; }

        public bool ComboReset { get; set; }

        public bool IndicatorUseAttackSpotDistance { get; set; }
        public AttractDirectType AttractDirectType { get; set; }

        public float EffectFlyTimeDamageRatio { get; set; }

        public float MaxEffectFlyTimeDamageRatio { get; set; }

        public bool DontBreakWhenTargetDead { get; set; }

        public CostMPMode CostMPMode { get; set; }

        public int EffectRelatedAbilityID { get; set; }
        public float EffectExtendAbilityTimeRatio { get; set; }
        public float MaxEffectExtendAbilityTime { get; set; }
        public bool CheckTrapObstacle { get; set; }
        public float RotateSpeed { get; set; }
        public CastEffectDmgFactorType CastEffectDmgFactorType { get; set; }
        public List<SummonActData> SummonActDatas { get; set; }
        public bool DisplaceResetCD { get; set; }
        public bool TargetSelectorWithOutSelf { get; set; }
        public List<int> MaxHitAddMarkCount { get; set; }
        public bool StealMainTarget { get; set; }
        public int EffectSearchDelayTime { get; set; }
        public RoleType StealRoleType { get; set; }
        public TalentData() :base((int)RoleAttribute.MAX)
        {

        }
        public TalentData Clone()
        {
            TalentData clone = this.MemberwiseClone() as TalentData;
            if (Abilities != null)
            {
                clone.Abilities = new List<AbilityData>(Abilities);    
            }
            if (AbilityIDs  != null)
            {
                clone.AbilityIDs = new List<int>(AbilityIDs);
            }
            if (MarksToAdd != null)
            {
                clone.MarksToAdd = new List<int>(MarksToAdd);
            }
            if(MarksToSelf != null)
            {
                clone.MarksToSelf = new List<int>(MarksToSelf);
            }
            if (MarksToTrigger != null)
            {
                clone.MarksToTrigger = new List<int>(MarksToTrigger);
            }
            clone.Values = (float[])this.Values.Clone();
            return clone;
        }


        public bool HasPropertys(RoleAttribute key)
        {
            return key == RoleAttribute.BasicNum || key == RoleAttribute.BaseRatio || key == RoleAttribute.ExtraBasicNum || key == RoleAttribute.AddLifeSteal || key == RoleAttribute.AddMagicLifeSteal || key == RoleAttribute.CastNum || key == RoleAttribute.Param1 || key == RoleAttribute.Param2 || key == RoleAttribute.Param3 || key == RoleAttribute.Param4 || key == RoleAttribute.Param5 || key == RoleAttribute.Param6;
        }
    }

	// 字符串映射到Enum类型的性能比较差，数据表解析时就解析成目标类型
    public class TargetExtraDamageData
    {
        public RoleAttribute fromRoleAttr;
        public InjuryType toInjuryType;
        public RoleType targetType;
        public int transRatio;

        public bool Parse(string from, string to, string target, string ratio)
        {
            try
            {
                fromRoleAttr = (RoleAttribute) Enum.Parse(typeof (RoleAttribute), from);
                toInjuryType = (InjuryType) Enum.Parse(typeof (InjuryType), to);
                targetType = (RoleType)int.Parse(target);
                transRatio = int.Parse(ratio);
            }
            catch (ArgumentException ae)
            {
                Debug.LogError("Parse Data Failed with TargetExtraDamageData: " + ae.ToString());
                return false;
            }

            return true;
        }
    }
}
