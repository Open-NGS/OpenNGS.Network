using System;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Neptune.GameData;
using static UnityEngine.GUILayout;
using Unity.VisualScripting.YamlDotNet.Core;

namespace Neptune
{

    /// <summary>
    /// Skill 技能类
    /// </summary>
    public class BattleSkill : AbilityBase
    {
        public int instanceId;

        //public Actor Caster;
        public UVector3 CastPosition;
        public UVector3 TargetPosition;
        public BattleEntity AttackPointElement;
        public Vector2 Orientation;
        public Vector2 Direction;
        public BattleActor Owner;
        public Vector2 LastDashDir = Vector2.zero;
        //protected static float defaultEvtX;
        //protected static float defaultEvtY;

        public TalentData Data;

        public TalentGroupData GroupData;
        public int Level;

        public int MaxRange;
        public int MinRange;
        public int SightRange;

        public int MaxRangeSqr;
        public int MinRangeSqr;
        public int SightRangeSqr;

        public Action onStartAct = null;
        public Action onEndAct = null;
        /// <summary>
        /// 如果 收到中断 攻击 取消 人机 模拟事件
        /// </summary>
        public bool cancelAIEventsIfInterrupted = false;

        public int MaxTargetNum;
        private int CastingEffectIndex;
        public virtual bool Casting { get; set; }
        //public virtual bool Instant { get { return this.Data.Instant; }}

        //bool IsCanCast = false;

        protected int CurrentActionID;
        public float CurrentActionElapsed;
        public TalentAction CurrentAction;
        public List<TalentAction> Actions;
        public int NextActID = 0;
        public AniEventData NextAct = null;

        //int talentTick = -1;
        public bool IsEnabled = true;

        // 正在释放的陷阱
        protected List<BattleTrap> castingTrapList = new List<BattleTrap>();

        private bool hasTalentCasting;

        public bool IsActivated = false;

        public bool ToBeCurrentTalent = true;
        public Dictionary<TargetType, ITargetSelector> TargetSelectors;

        public ITargetSelector TargetSelector
        {
            get { return TargetSelectors.ContainsKey(Data.TargetType) ? TargetSelectors[Data.TargetType] : null; }
        }

        public RelativeSide AffectedSide
        {
            get { return (RelativeSide)((int)Caster.StandSide * (int)Data.AffectedSide); }
        }

        public RelativeSide TargetSide
        {
            get { return (RelativeSide)((int)Caster.StandSide * (int)Data.TargetSide); }
        }

        public Dictionary<BattleActor, int> targetContinuousEffects = new Dictionary<BattleActor, int>();

        protected int hitTargetNum;
        public int RebirthNum;
        //记录被攻击次数
        private Dictionary<BattleActor, int> dividedHitTarget = null;

        private Dictionary<int, Dictionary<int, int>> addMarkHitCount = null;
        //技能释放时间
        public float CastDuration = 0f;
        //攻击间隔时间
        private float ContinuousInterval = 0f;
        protected bool isCanContinuous;
        private float ContinuousElapsed = 0;

        public float castFlyTime = 0f;

        private UVector2 obstacleWallHit;

        private float damageFactor = 1.0f;

        public float DamageFactor
        {
            get { return damageFactor; }
        }

        public virtual float CD
        {
            get
            {
                if (Data.NoSpeeder && !Data.NoCDReduct)
                {
                    return UFloat.Round(Data.CD * EngineConst.Milli *
                           (1 - Math.Max(Math.Min(Caster.Attributes.CDReduction, 40), 0) * EngineConst.Hundredth));
                }
                else
                {
                    return UFloat.Round(Data.CD * EngineConst.Milli);
                }
            }
        }
        public BattleSkill()
        {

        }


        public virtual void Create(TalentGroupData group, TalentData data, BattleActor caster, int level)
        {
            Create();

            instanceId = 0;

            AttackPointElement = null;
            Owner = null;

            onStartAct = null;
            onEndAct = null;
            cancelAIEventsIfInterrupted = false;
            CurrentAction = null;
            IsActivated = false;

            targetContinuousEffects.Clear();
            if (addMarkHitCount != null)
                addMarkHitCount.Clear();
            dividedHitTarget = null;
            CastPosition = UVector3.zero;
            castFlyTime = 0f;
            CastingEffectIndex = 0;
            GroupData = group;
            Data = data;
            Caster = caster;
            Caster.RoleSkin.RoleSkinTalentReplace(Data);
            Level = level;
            Actions = new List<TalentAction>();
            MinRange = Data.MinRange;
            MinRangeSqr = MinRange * MinRange;
            MaxTargetNum = Data.MaxTargetNum;
            InitCD();
            Casting = false;
            CastDuration = 0;
            hasTalentCasting = false;
            Direction = Vector2.zero;
            LastDashDir = Vector2.zero;
            Target = null;

            CurrentActionID = 0;
            CurrentActionElapsed = 0;
            NextActID = 0;
            NextAct = null;
            HitTimes = 0;
            IsEnabled = true;
            damageFactor = 1.0f;
            obstacleWallHit = UVector2.zero;

            if (Data.MaxRange == 0)
            {
                MaxRange = EngineConst.MaxRangeValue;
            }
            else
            {
                MaxRange = Math.Min(EngineConst.MaxRangeValue, Data.MaxRange);
            }
            MaxRangeSqr = MaxRange * MaxRange;
            /*if (this.Data.SightRange == 0)
            {
                this.SightRange = this.Data.MaxRange + 500;
            }
            else*/
            {
                SightRange = Math.Min(EngineConst.MaxRangeValue, Data.SightRange);
                SightRangeSqr = SightRange * SightRange;
            }

            InitActions(Caster.RoleSkin.GetModelKey(Caster.Data.Model));
            if (Data.TargetType == TargetType.Self)
                Data.TargetSide = RelativeSide.Both;
            else if (Data.TargetType == TargetType.Target)
                Data.TargetSide = RelativeSide.Enemy;
            TargetSelectors = new Dictionary<TargetType, ITargetSelector>()
        {
            {TargetType.Random, new RandomSelector()},
            {TargetType.Weakest, new WeakestSelector()},
            {TargetType.MaxHP, new MaxHPSelector()},
            {TargetType.MinHP, new MinHPSelector()},
            {TargetType.Nearest, new NearestSelector(Caster)},
            {TargetType.Farthest, new FarthestSelector(Caster)},
            {TargetType.MaxMP, new MaxMPSelector()},
            {TargetType.MinMP, new MinMPSelector()},
            {TargetType.MaxIntelligence, new MaxIntelligenceSelector()},
            {TargetType.MaxAttackDamage, new MaxADSelector()},
            {TargetType.DeadBody, new RandomSelector()}
        };
            isCanContinuous = DetectionContinuous();

            castingTrapList.Clear();
        }

        public override void Delete()
        {
            NObjectPool<BattleSkill>.Delete(this);
        }

        public override void OnDelete()
        {

        }


        public bool DetectionContinuous()
        {
            if (Data.ContinuousType == ContinuousType.Continuous)
            {
                if (!Data.Instant && Actions != null)
                {
                    for (int i = 0; i < Actions.Count; i++)
                    {
                        TalentAction talentAction = Actions[i];
                        if (talentAction.Events != null)
                        {
                            for (int j = 0; j < talentAction.Events.Count; j++)
                            {
                                if (string.Equals(talentAction.Events[j].Type, "attackspot", StringComparison.OrdinalIgnoreCase))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public virtual void LevelUp(int level)
        {
            if (Level < level)
                Level = level;
            if (Level == level && Level >= 1)
            {
                TalentData talentData = Caster.GetTalentData(GroupData, Level, Caster.ID);
                if (talentData != null)
                {
                    Data = talentData;
                    Caster.RoleSkin.RoleSkinTalentReplace(Data);
                    //                this.Data.Abilities = new List<AbilityData>();
                    //                 if (this.Data.AbilityIDs != null && this.Data.AbilityIDs.Count > 0)
                    //                 {
                    //                     foreach (int abilityID in this.Data.AbilityIDs)
                    //                     {
                    //                         AbilityData ability_info = EngineDataManager.Instance.Abilities.Value[abilityID].Clone();
                    //                         this.Data.Abilities.Add(ability_info);
                    //                     }
                    //                 }


                    MinRange = Data.MinRange;
                    MinRangeSqr = MinRange * MinRange;
                    MaxTargetNum = Data.MaxTargetNum;

                    if (Data.TargetType == TargetType.Self)
                        Data.TargetSide = RelativeSide.Both;
                    else if (Data.TargetType == TargetType.Target)
                        Data.TargetSide = RelativeSide.Enemy;

                    MaxRange = Data.MaxRange == 0 ? EngineConst.MaxRangeValue : Math.Min(EngineConst.MaxRangeValue, Data.MaxRange);
                    SightRange = Math.Min(EngineConst.MaxRangeValue, Data.SightRange);// == 0 ? this.MaxRange + 500 : this.Data.SightRange;
                    MaxRangeSqr = MaxRange * MaxRange;
                    SightRangeSqr = SightRange * SightRange;
                }
            }


            foreach (BattleActor role in Caster.SummonList)
            {
                if (role != null && !role.IsDead)
                {
                    for (int i = 0; i < role.tActiveTalents.Length; i++)
                    {
                        BattleSkill talent = role.tActiveTalents[i] as BattleSkill;
                        if (talent == null) continue;
                        if (talent.GroupData.ParentID == Data.TalentGroupID)
                        {
                            talent.LevelUp(talent.Level + 1);
                        }
                    }
                }
            }


            for (int i = 0; i < Caster.tActiveTalents.Length; i++)
            {
                BattleSkill talent = Caster.tActiveTalents[i] as BattleSkill;
                if (talent == null) continue;
                if (Data.TalentGroupID > 0 && talent.GroupData.ParentID == Data.TalentGroupID)
                {
                    talent.LevelUp(talent.Level + 1);
                }
                if (!talent.IsEnabled) continue;
                if ((talent.Data.TriggerType & TriggerType.TalentLevel) > 0 && (talent.Data.TriggerParam1 == 0 || talent.Data.TriggerParam1 == Data.TalentGroupID))
                {
                    if (talent.CanUse(Caster) == ResultType.Success)
                    {
                        talent.Start(Caster);
                    }
                }
            }
            for (int i = 0; i < Caster.PassiveTalents.Length; i++)
            {
                PassiveSkill talent = Caster.PassiveTalents[i];
                if (talent == null) continue;
                if (Data.TalentGroupID > 0 && talent.GroupData.ParentID == Data.TalentGroupID)
                {
                    talent.LevelUp(talent.Level + 1);
                }
            }
        }

        /*
         *******************************
         *  Skill Basic Behavior
         *******************************
         */

        public virtual void Init(bool isInitCD = true)
        {
            Target = null;
            HitTimes = 0;
            if (isInitCD)
                InitCD();
            Casting = false;
            hasTalentCasting = false;
            CurrentActionElapsed = 0;
            CurrentActionID = 0;
            NextActID = 0;
            CurrentAction = null;
            NextAct = null;
        }

        public void InitCD()
        {
            Duration = UFloat.Round(Data.InitCD / EngineConst.Thousand);
        }

        void ResetCD(TalentStatus status)
        {
            if (Data.CDMode == status || Data.CDMode == TalentStatus.None && status == TalentStatus.Start)
            {
                Duration = CD;
            }
        }
        /// <summary>
        /// Start talent to target
        /// </summary>
        /// <param name="target"></param>
        public virtual void Start(BattleActor target)
        {
            Start(target, UVector3.zero);
        }

        public void Start(BattleActor target, UVector3 position)
        {
            //技能开始，初始化数据
            //播放启动特效
            //挂载Ability
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} began to cast {1} --> {2}", Caster.FullName, FullName(), target != null ? target.FullName : "nil");
#endif
            CastDuration = 0f;
            Casting = true;
            hasTalentCasting = true;
            Target = target;
            hitTargetNum = 0;
            RebirthNum = 0;
            TargetPosition = position;
            Orientation = EngineConst.Vector2Zero;
            ContinuousInterval = 0;



            if (FindTarget(target) == null)
            {
                End();
                return;
            }

            Caster.CommonCooldown = UFloat.RoundToInt(Data.GCD * EngineConst.Milli);
            ContinuousElapsed = 0;

            HitTimes = 0;
            IsEnabled = true;
            ResetCD(TalentStatus.Start);
            //消耗魔法值
            CostMp(CostMPMode.Start);
            if (Data.Instant)
            {//瞬发技能，立即生效
                OnHitFrame();
            }
            else
            {

                if (!Data.MovingCast)
                {
                    Caster.moveDirLock = true;
                }

                if (Caster != Target && !Caster.IsRoleType(RoleType.Building) && Target.Position != Caster.Position)
                {
                    Caster.Orientation = (Target.Position - Caster.Position).normalized;
                    //Debug.Log("Direction[" + this.Caster + "] : set by TalentStart" + this.Caster.Orientation);
                }
                StartAction(0);
            }

            if (Caster.Joint != null)
            {
                if (Caster.IsHero && (Data.TriggerType == TriggerType.Manual || Data.TriggerType == TriggerType.Auto))
                {
                    //if (this.Caster != null && this.Caster.Player != null && this.Caster.Player.isMainPlayer) CachedLog.Log("time:" + Time.realtimeSinceStartup + ",frame:" + Logic.Instance.doneFrameCount + ",talent start.dir:" + this.Caster.Orientation.ToString());
                    Caster.Joint.SetDirection(Caster.Orientation);
                }
                bool createEffect = true;
                if (Caster.Joint.Controller != null)
                {
                    createEffect = Caster.Joint.Controller.InSight;
                }
                if (createEffect)
                    CastingEffectIndex = Caster.Joint.AddEffect(Data.CastingEffect, EffectType.Cast, UVector3.zero);
            }
            PlayExtraEffects(EffectPlayType.OnCast, Caster.Position);

            // 播放音效
            if (NeptuneBattle.Instance.Scene != null && !string.IsNullOrEmpty(Data.StartSound))
            {
                Caster.PlaySound(Data.StartSound);
            }
            // 播放VO
            if (NeptuneBattle.Instance.Scene != null && Data.PlayVoice != null && Caster.Player != null && Caster.Player.isMainPlayer)
            {
                Caster.PlayRandomVoice(Data.PlayVoice, true);
            }

            if (Data.UnfreezeTarget)
            {
                if (Target != null)
                {
                    Target.ResumeActor();
                }
            }

            if (HasAbility(AbilityTriggerType.Start))
            {
                CreateTalentAbilitis(Target, AbilityTriggerType.Start);
            }
            //攻击时触发技能
            Caster.OnAttack(this);

            //给自己添加MARK
            AddMark(Caster, Data.MarksToSelf, AddMarkType.Start);
            if (Data.TrapIds != null && Data.TrapIds.Count > 0 && Target != null)
                CreateTrap(TrapTriggerType.Start, Target.Position, Target);
            if (Data.Instant && Data.ContinuousType != ContinuousType.Continuous)
            {//瞬发技能，立即结束
                End();
            }
            else if (GroupData.ParentID == 0 || Actions != null && Actions.Count > 0 && Actions[0] != null && ToBeCurrentTalent)
            {
                Caster.CurrentTalent = this;
            }
            if (onStartAct != null)
                onStartAct();
            Caster.CallBackTalentStart(this);
        }

        /// <summary>
        /// Start talent at position
        /// </summary>
        /// <param name="position"></param>
        public virtual void Start(UVector3 position)
        {
            //定点释放的技能,定点必是AOE
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} began to cast {1} --> {2}", Caster.FullName, FullName(), position);
#endif
            CastDuration = 0f;
            hasTalentCasting = true;
            Casting = true;
            hitTargetNum = 0;
            RebirthNum = 0;
            Target = null;
            TargetPosition = position;
            Caster.CommonCooldown = UFloat.RoundToInt(Data.GCD * EngineConst.Milli);
            ContinuousElapsed = 0;
            ContinuousInterval = 0;
            HitTimes = 0;
            IsEnabled = true;
            ResetCD(TalentStatus.Start);
            CostMp(CostMPMode.Start);
            if (Data.Instant)
            {//瞬发技能，立即生效
                OnHitFrame();
            }
            else
            {
                if (!Data.MovingCast)
                {
                    Caster.moveDirLock = true;
                }
                //if (this.Caster.CurrentTalent != null)
                //{
                //    this.Caster.CurrentTalent.Break();
                //}
                //this.Caster.Orientation = ((Vector2)this.TargetPosition - this.Caster.Position).normalized;
                StartAction(0);
            }

            if (Caster.Joint != null)
            {
                if (Caster.IsHero && (Data.TriggerType == TriggerType.Manual || Data.TriggerType == TriggerType.Auto))
                {
                    //if (this.Caster != null && this.Caster.Player != null && this.Caster.Player.isMainPlayer) CachedLog.Log("time:" + Time.realtimeSinceStartup + ",frame:" + Logic.Instance.doneFrameCount + ",talent start1.dir:" + this.Caster.Orientation.ToString());
                    Caster.Joint.SetDirection(Caster.Orientation);
                }
                CastingEffectIndex = Caster.Joint.AddEffect(Data.CastingEffect, EffectType.Normal, UVector3.zero);
            }
            PlayExtraEffects(EffectPlayType.OnCast, Caster.Position);

            if (Data.UnfreezeTarget)
            {
                if (Target != null)
                {
                    Target.ResumeActor();
                }
            }

            if (HasAbility(AbilityTriggerType.Start))
            {
                CreateTalentAbilitis(Target, AbilityTriggerType.Start);
            }
            //攻击时触发技能
            Caster.OnAttack(this);

            //给自己添加MARK
            AddMark(Caster, Data.MarksToSelf, AddMarkType.Start);
            if (Data.TrapIds != null && Data.TrapIds.Count > 0)
                CreateTrap(TrapTriggerType.Start, Caster.Position, Caster);
            if (Data.Instant)
            {//瞬发技能，立即结束
                End();
            }

            else if (GroupData.ParentID == 0 || Actions != null && Actions.Count > 0 && Actions[0] != null && ToBeCurrentTalent)
            {
                Caster.CurrentTalent = this;
            }
            if (onStartAct != null)
                onStartAct();

            // 播放音效
            if (NeptuneBattle.Instance.Scene != null && !string.IsNullOrEmpty(Data.StartSound))
            {
                Caster.PlaySound(Data.StartSound);
            }
            // 播放VO
            if (NeptuneBattle.Instance.Scene != null && Data.PlayVoice != null && Caster.Player != null && Caster.Player.isMainPlayer)
            {
                Caster.PlayRandomVoice(Data.PlayVoice, true);
            }
            Caster.CallBackTalentStart(this);
        }

        public virtual void OnEnterFrame(float dtAction, float dtCd)
        {
#if BATTLE_LOG
            BattleActor lastTarget = Target;
#endif
            if (!Caster.IsDead && Casting && this == Caster.CurrentTalent)
            {
                if (HitTimes == 0 && Data.TargetType == TargetType.Target &&
                    (Target == null || Target.IsDead && !Data.DontBreakWhenTargetDead) && TargetPosition == UVector3.zero)
                {
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("  {0} Skill OnEnterFrame {1} {2} {3}", Caster.FullName, FullName(),
                            Target == null, Target != null ? Target.IsDead.ToString() : "Null");
#endif
                    Break();
                }
                else
                {
                    EventHandler(dtAction);
                    if (Data.ContinuousType == ContinuousType.Continuous)
                    {
                        if (HitTimes > 0)
                            ContinuousElapsed += dtCd;

                        if (Data.ContinuousTime > 0 && ContinuousElapsed >= Data.ContinuousTime
                            || Data.TargetType == TargetType.Target && (
                            Target == null
                                || Target != null && (Target.IsDead || Caster.Distance(Target, EngineConst.EnableRadiusInDistance) > Data.MaxRange)
                            )
                            )
                        {
                            End();
                        }
                        else if (isCanContinuous)
                        {
                            ContinuousInterval += dtCd;
                            if (ContinuousInterval >= Data.ContinuousInterval)
                            {
                                ContinuousInterval = 0;
                                OnHitFrame();
                                if (Data.ContinuousTime < Data.ContinuousInterval * (HitTimes + 1))
                                    End();
                            }
                        }
                    }
                    CastDuration = UFloat.Round(CastDuration + dtCd);
                }
            }

            UpdateCD(dtAction, dtCd);
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("   {0} PT:{1} CT:{2}", ToString(), lastTarget == null ? "null" : lastTarget.FullName, Target == null ? "null" : Target.FullName);
#endif
        }

        public virtual void UpdateCD(float dtAction, float dtCd)
        {
            if (Duration > 0)
            {
                Duration = UFloat.Round(Duration - dtCd);
                if (Duration <= 0)
                {
                    Caster.OnColdDown(this);
                }
            }
        }

        /// <summary>
        /// Frame Event Handler
        /// </summary>
        /// <param name="dtAction"></param>
        public bool EventHandler(float dtAction)
        {
            bool result = true;
            float time = UFloat.Round(CurrentActionElapsed + dtAction);
            CurrentActionElapsed = time;
            AniEventData nextEvent = NextAct;
            while (nextEvent != null && time >= nextEvent.Time)
            {
                string type = "AttackSpot";
                if (!string.IsNullOrEmpty(nextEvent.Type))
                    type = nextEvent.Type;

#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("{0}: {1} OnHitFrame {2:f6} > {3:f6} {4:f6} {5}", Caster.FullName, FullName(), time, nextEvent.Time, dtAction, type);
#endif

                switch (type)
                {
                    case "attackspot":
                    case "AttackSpot":
                        OnHitFrame();
                        break;
                    case "dash":
                    case "Dash":
                        DoDash(nextEvent.X, (int)nextEvent.Y, (int)nextEvent.Z, (int)nextEvent.Param1, nextEvent.Param2 <= 0, (int)nextEvent.Param3, nextEvent.Param4 <= 0, nextEvent.Param5, nextEvent.Param6, (int)nextEvent.Param7, (int)nextEvent.Param8);
                        break;
                    case "teleport":
                    case "Teleport":
                        DoTeleport((TargetType)nextEvent.X, (int)nextEvent.Y);
                        break;
                    case "talent":
                    case "Skill":
                        int talentID = (int)nextEvent.X;
                        if (Caster.ChangeTalentStack.ContainsKey(talentID))
                        {
                            List<int> abilitys = Caster.ChangeTalentStack[talentID];
                            if (abilitys != null && abilitys.Count > 0)
                            {
                                Ability ability = Caster.FindAbilityByChangeIndex(abilitys.Max(), talentID);
                                if (ability != null)
                                    talentID = ability.ToTalentID;
                            }
                        }

                        DoTalent(talentID);
                        break;
                    case "invisibility":
                    case "Invisibility":
                        break;
                    case "end":
                    case "End":
                        End();
                        result = false;
                        break;
                }
                int idx = NextActID + 1;

                if (CurrentAction == null || !result)
                {
                    break;
                }
                nextEvent = idx < CurrentAction.Events.Count ? CurrentAction.Events[idx] : null;
                NextActID = idx;
                NextAct = nextEvent;
            }

#if BATTLE_LOG
            if (EngineGlobal.BattleLog && nextEvent == null)
                NeptuneBattle.log("{0}: {1} OnHitFrame NextEvent is null {2:f6}", Caster.FullName, FullName(), time);
#endif
            return result;
        }

        // 清除当前技能正在释放的陷阱
        public void ClearCastingTrap(bool casterDead = false)
        {
            if (castingTrapList.Count > 0)
            {
                foreach (BattleTrap trap in castingTrapList.ToArray())
                {
                    if (trap.Interrupt(casterDead))
                    {
                        NeptuneBattle.Instance.RemoveTrap(trap);
                        castingTrapList.Remove(trap);
                    }
                }
            }
        }

        public virtual BattleActor FindTarget(BattleActor defaultTarget = null)
        {
            if (Data.TargetType == TargetType.Target || Caster.AbilityEffects.Taunt)
            {

                if (defaultTarget != null || Data.FollowSourceTalentTarget)
                {
                    Target = defaultTarget;
                }
                else
                {
                    int dist = 0;
                    BattleActor target = Caster.FindTarget(ref dist, this, true, Direction.x, Direction.y);
                    if (dist <= MaxRange)
                    {
                        Target = target;
                    }
                    else
                    {
                        Target = null;//这里应该是空的
                    }
                }
            }
            else if (Data.TargetType == TargetType.Self)
            {
                Target = Caster;
            }
            else if (Data.TargetType == TargetType.Owner)
            {
                Target = Caster.Owner;
            }
            else if (Data.TargetType == TargetType.DeadBody)
            {
                Target = null;
                foreach (BattleActor role in NeptuneBattle.Instance.Roles.Where(BattleActor.PredicateDeadBody))
                {
                    Target = role;
                }
                return Target;
            }
            else if (TargetSelector != null)
            {
                float max = -float.MaxValue;
                BattleActor result = null;
                int sightrange = SightRange;
                if (sightrange <= 0)
                {
                    sightrange = MaxRange;
                }
                foreach (BattleActor role in NeptuneBattle.Instance.AOIManager.GetSurvivors(TargetPosition, Caster.Side, TargetSide, sightrange, Caster.Radius))
                {
                    if (Data.TargetSelectorWithOutSelf && role == Caster)
                        continue;

                    if (!CheckTargetClass(role))
                        continue;

                    if (role != Caster && role.AbilityEffects.Void || TargetSide != RelativeSide.Friend && Caster.NotSelectable(role))
                        continue;

                    if (Caster.AbilityEffects.Charm && role == Caster)
                        continue;

                    int distance = role.Distance(TargetPosition, 0, EngineConst.EnableRadiusInDistance);
                    if (distance >= MinRange && distance <= sightrange)
                    {
                        float v = TargetSelector.Select(role);
                        if (max < v)
                        {
                            max = v;
                            result = role;
                        }
                    }
                }
                Target = result;
            }
            else if (Data.TargetType == TargetType.Position)
            {

            }
            else if (Data.TargetType == TargetType.Direction)
            {

            }
            else if (Data.TargetType == TargetType.All)
            {
                Target = defaultTarget;
            }
            else
                Logger.LogError(string.Format("target type error: {0}", Data.TargetType) + " | talent.id = " + Data.ID);

#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} FindTarget:{1} {2} {3}", Caster.FullName, FullName(), Target == null ? "null" : Target.FullName, Data.TargetType);
#endif

            return Target;
        }

        public virtual bool CheckTargetClass(BattleActor role)
        {
            if (Caster.AbilityEffects.OnlyAttackBuilding)
            {
                if (role.Data.RoleType == RoleType.Building)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (Data.TargetClass == RoleType.Any)
                return true;
            if ((Data.TargetClass & RoleType.Demon) > 0 && role.Config.IsDemon)
                return true;
            if ((Data.TargetClass & RoleType.Building) > 0 && role.Data.RoleType == RoleType.Building)
                return true;
            if ((Data.TargetClass & role.Data.RoleType) > 0)
                return true;
            return false;
        }

        public virtual void Active()
        {
        }

        public virtual void End(bool isBreak = false)
        {
            ResetCD(TalentStatus.End);
            CostMp(CostMPMode.End);
            if (Caster.IsActiveCasting)
            {
                NeptuneBattle.Instance.Resume();
                Caster.IsActiveCasting = false;
            }
            Casting = false;
            hasTalentCasting = false;
            Caster.OnTalentEnd(this);
            //瞬发技能不改变角色状态
            if (!Data.Instant)
            {
                if (GroupData.ParentID == 0 || GroupData.ChildGroupID == 2)
                {
                    Caster.CurrentTalent = null;
                }
                if (Data.MovingCast && Caster.MoveDirection != EngineConst.Vector2Zero && Caster.CanMove && Caster.DashTime <= 0 && Caster.RepelTime <= 0)
                {
                    if (Caster.RoleST != RoleState.Move)
                    {
                        Caster.RoleST = RoleState.Move;
                        Caster.Move();
                    }
                }
                else
                    Caster.Idle(isBreak);
            }

            // End Trap If any
            if (!IsActivated)
            {
                ClearCastingTrap(Caster.IsDead);
                CastDuration = 0;
            }
            if (Data.RemoveCastingEffect)
            {
                if (Caster.Joint != null)
                {
                    if (CastingEffectIndex > 0)
                        Caster.Joint.RemoveEffect(CastingEffectIndex);
                }
            }
            foreach (KeyValuePair<BattleActor, int> kv in targetContinuousEffects)
            {
                kv.Key.Joint.RemoveEffect(kv.Value);
            }
            targetContinuousEffects.Clear();
            Orientation = EngineConst.Vector2Zero;
            Direction = EngineConst.Vector2Zero;
            if (onEndAct != null)
                onEndAct();
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} Skill End {1} {2} {3}", Caster.FullName, FullName(), isBreak, Data.Instant);
#endif
        }

        public virtual void Break(BattleSkill breakTalent = null)
        {
            float cdRate = 1;
            if (EngineConst.TalentInterruptCdRate >= 0)
            {
                if (HitTimes == 0)
                {//没有命中目标被打断时CD减半   每个项目根据需求在EngineConst中配置CD比率
                    cdRate = EngineConst.TalentInterruptCdRate;
                }
                Duration = UFloat.Round(cdRate * CD);
                Caster.CommonCooldown = UFloat.Round(cdRate * UFloat.RoundToInt(Data.GCD * EngineConst.Milli));
            }
            if (Caster.Joint != null)
            {
                if (!string.IsNullOrEmpty(Data.StartSound))
                {
                    Caster.Joint.StopSound();
                }
                //if (Data.PlayVoice != null)
                //{
                //    this.Caster.Joint.StopVoice();
                //}

            }
            if (Caster != null)
                Caster.CallbackByBreakTalent();
            End(true);
        }

        public virtual UVector3 AttackPoint(int index = 0)
        {
            AniEventData evt = NextAct ?? new AniEventData();
            bool element = false;
            if (evt != NextAct)
            {
                if (AttackPointElement != null)
                {
                    evt.X = AttackPointElement.Position.x;
                    evt.Y = AttackPointElement.Position.y;
                    element = true;
                }
                else
                {
                    evt.X = defaultEvtX;
                    evt.Y = defaultEvtY;
                }

            }
            UVector3 bonePos = new UVector3(evt.X, evt.Y, evt.Z);
            if (Data.FlyStartOffset != null && Data.FlyStartOffset.Count != 0 && Data.FlyStartOffset.Count % 3 == 0)
            {
                if (index * 3 > Data.FlyStartOffset.Count - 1)
                {
                    index = 0;
                }
                bonePos = bonePos + new UVector3(Data.FlyStartOffset[3 * index], Data.FlyStartOffset[3 * index + 1], Data.FlyStartOffset[3 * index + 2]);
            }
            if (element)
            {
                return bonePos;
            }

            //Vector2 bonePos = new Vector2(evt.X, evt.Y);
            Vector2 scale = Caster.GetCurrentScale();
            UFloat.Round(ref scale);
            float roleScale = Caster.GetRoleScale();
#if GAME_2D
        UVector3 hitPos = new UVector3(this.Caster.Position.x + bonePos.x * scale.x * roleScale, this.Caster.Position.y, bonePos.y * scale.y * roleScale);

#else
            UVector3 localPos = new UVector3(bonePos.x * scale.x * roleScale, bonePos.y * scale.y * roleScale, bonePos.z * scale.y * roleScale);

            //algorithm1
            int eAngle = MathUtil.VectorToAngleInt(Caster.Orientation, Vector3.right);

            float angle = UFloat.Round(eAngle * Mathf.Deg2Rad);
            float sinAngle = UFloat.Round(Mathf.Sin(angle));
            float cosAngle = UFloat.Round(Mathf.Cos(angle));

            int newX = UFloat.RoundToInt(UFloat.Round(localPos.x * cosAngle - localPos.y * sinAngle));
            int newY = UFloat.RoundToInt(UFloat.Round(localPos.x * sinAngle + localPos.y * cosAngle));
            UVector3 hitPos = new UVector3(Caster.Position.x + newX, Caster.Position.y + newY, Caster.Height + localPos.z);
#endif

#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0}: {1} --> {2} exEffect AttackPoint ({3:f6},{4:f6},{5:f6}) Evt({6:f6},{7:f6}) {8:f6},{9:f6},{10:f6},{11:f6}", Caster.FullName, FullName(), Target != null ? Target.FullName : "", hitPos.x, hitPos.y, hitPos.z, bonePos.x, bonePos.y, roleScale, scale.x, scale.y, eAngle);
#endif
            return hitPos;
        }

        /*
         *******************************
         *  Skill Status Verify
         *******************************
         */

        public virtual ResultType CanUse(BattleActor target, bool checkTarget = true, bool ignoredInvisible = false)
        {
            ResultType result = ResultType.Success;
            if (Level <= 0)
            {
                result = ResultType.Failed; goto end;
            }
            if (IsActivated || Duration > 0 && !(GroupData.Index == 1 && !checkTarget && Duration - 0.4f <= 0))
            {
                result = ResultType.Cooldown; goto end;
            }

            if (Caster.AbilityEffects.OnlyNormalAttack && !Data.NoSilence)
            {
                result = ResultType.OnlyNormalAttack; goto end;
            }

            //缴械
            if (Caster.AbilityEffects.Disarmed && Data.SkillType == SkillType.Normal)
            {
                result = ResultType.Disarmed; goto end;
            }


            if (Data.CostMP > Caster.GetCurrentCostValue())
            {
                result = ResultType.MPNotEnough; goto end;
            }

            //不是驱散
            if (!Data.Unrestricted)
            {
                if ((Data.TriggerType & TriggerType.Manual) > 0 || (Data.TriggerType & TriggerType.Auto) > 0)
                {
                    if (Caster.AbilityEffects.Sleep)
                    {
                        result = ResultType.Stun; goto end;
                    }

                    if (Caster.AbilityEffects.Fear)
                    {
                        result = ResultType.Fear; goto end;
                    }

                    if (this != Caster.BasicTalent && Caster.AbilityEffects.MindChain)
                    {
                        result = ResultType.MindChain; goto end;
                    }
                }
                if ((Data.TriggerType & TriggerType.Manual) > 0 && !Data.NoSilence)
                {
                    if (Data.DamageType == InjuryType.AttackDamage && Caster.AbilityEffects.Disable)
                    {
                        result = ResultType.Disable; goto end;
                    }

                    if (Data.DamageType != InjuryType.AttackDamage && Caster.AbilityEffects.Inhibition)
                    {
                        result = ResultType.Silence; goto end;
                    }
                }
            }



            if (checkTarget && Data.TargetType != TargetType.Direction && Data.TargetType != TargetType.Position)
            {//需要目标才是使用的技能
                target = FindTarget(target);//检测每个技能是否可释放的时候同时检测是否有可用目标    

                if (target == null || Data.TargetClass > 0 && (target.Data.RoleType & Data.TargetClass) <= 0)
                {
                    result = ResultType.NoTarget; goto end;
                }

                if (target != Caster && target.AbilityEffects.Void || Caster.GetRelation(target.Side) != RelativeSide.Friend && !ignoredInvisible && Caster.NotSelectable(target))
                {
                    result = ResultType.Untargetable; goto end;
                }

                if ((int)Caster.GetRelation(target) * (int)TargetSide < 0)
                {
                    result = ResultType.SideDiff; goto end;
                }


                #region 临时在这里增加关于海陆空的逻辑判断，将来如果需求持续扩展可考虑重构

                if (Data.TargetLSAMode == LandSeaAirMode.Air && !target.IsAirForce)
                {
                    result = ResultType.Untargetable; goto end;
                }

                if (Data.TargetLSAMode == LandSeaAirMode.Ground && target.IsAirForce)
                {
                    result = ResultType.Untargetable; goto end;
                }
                #endregion
                if (!Data.FollowSourceTalentTarget)
                {
                    int distance = Caster.Distance(target, EngineConst.EnableRadiusInDistance);
                    if (distance > SightRange && SightRange > 0)
                    {
                        result = ResultType.OutOfSightRange; goto end;
                    }
                    int range = MaxRange;
                    if (distance > range)
                    {
                        result = ResultType.TooFar; goto end;
                    }

                    if (distance < MinRange)
                    {
                        result = ResultType.TooNear; goto end;
                    }

#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("  {0} Skill.CanUse : {1} {2} ({3} - {4})", FullName(), result, distance, MinRange, MaxRange);
#endif
                }

            }
            if (Caster.TargetPos != UVector2.zero && (Data.TargetType == TargetType.Direction || Data.TargetType == TargetType.Position))
            {
                int distance = Caster.Distance(Caster.TargetPos, 0, EngineConst.EnableRadiusInDistance);
                int maxRange = Data.TargetType == TargetType.Position && Caster.IsHero && checkTarget ? MaxRange + EngineConst.AttackRange : MaxRange;
                if (distance > maxRange)
                {
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("  {0} Skill.CanUse : TooFar {1} < {2} {3}", FullName(), distance, maxRange, Caster.TargetPos);
#endif
                    result = ResultType.TooFar; goto end;
                }
            }

            if (!Data.OutOfScreen && Caster.IsOutOfBattleArea)
            {
                result = ResultType.OutOfScreen;
            }

        end:

#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("  {0} Skill.CanUse : {1}", FullName(), result);
#endif
            return result;
        }

        /// <summary>
        /// 检测剩余魔法是否够释放技能
        /// </summary>
        public virtual bool CheckMPIsEnough()
        {
            return Data.CostMP <= Caster.GetCurrentCostValue();
        }

        public virtual bool IsReady()
        {
            return true;
        }

        public virtual bool CanActive()
        {
            return false;
        }

        public bool CheckRoleType(BattleActor role, RoleType type, bool defaultValue)
        {
            if (type == RoleType.Any)
                return defaultValue;
            if (((int)type & (int)RoleType.Natural) == (int)RoleType.Natural)
            {
                return !role.Config.IsDemon;
            }
            if (((int)type & (int)RoleType.Demon) == (int)RoleType.Demon)
            {
                return role.Config.IsDemon;
            }
            if (((int)type & (int)RoleType.Strength) == (int)RoleType.Strength)
            {
                return role.Data.MainAttrib == RoleAttribute.Strength;
            }
            if (((int)type & (int)RoleType.Agility) == (int)RoleType.Agility)
            {
                return role.Data.MainAttrib == RoleAttribute.Agility;
            }
            if (((int)type & (int)RoleType.Intelligence) == (int)RoleType.Intelligence)
            {
                return role.Data.MainAttrib == RoleAttribute.Intelligence;
            }
            return defaultValue;
        }
        protected virtual bool CheckIgnoreTarget(BattleActor target)
        {

            if (hitTargetNum > MaxTargetNum && MaxTargetNum > 0)
            {//
                return true;
            }
            if (Data.IgnoreTarget == RoleType.Any)
                return false;
            if (target.IsRoleType(Data.IgnoreTarget))
            {
                return true;
            }

            if (target.Data.MainAttrib.ToString() == Data.IgnoreTarget.ToString())
            {
                if (NeptuneBattle.Instance.Scene != null)
                {
                    NeptuneBattle.Instance.Scene.PopupText(PopupType.Void, string.Empty, 0, target.Joint, Caster.Joint, false, target.Side);
                }
                return true;
            }
            return false;
        }

        public bool HasAbility(AbilityTriggerType triggerType, AbilityTriggerMode abilityTriggerMode = AbilityTriggerMode.AfterInijury)
        {
            if (Data.Abilities == null || Data.Abilities.Count <= 0)
            {
                return false;
            }
            for (int i = 0; i < Data.Abilities.Count; i++)
            {
                if (Data.Abilities[i].TriggerType == triggerType && (triggerType == AbilityTriggerType.Start || Data.Abilities[i].AbilityTriggerMode == abilityTriggerMode))
                {
                    return true;
                }
            }
            return false;
        }

        /*
         *******************************
         *  Skill Action Relative
         *******************************
         */

        public void InitActions(string modelName)
        {
            if (string.IsNullOrEmpty(modelName))
            {
                return;
            }
            Actions.Clear();
            ModelData model = NeptuneBattle.Instance.DataProvider.GetModelData(modelName);
            if (model == null)
            {
                Logger.LogError("Model not existed : " + modelName);
                return;
            }

            string resourceName = model.Resource;
            if (string.IsNullOrEmpty(resourceName))
            {
                return;
            }

            Dictionary<string, AnimationConfigData> resAniConfigData = NeptuneBattle.Instance.DataProvider.GetAnimationConfigDatas(resourceName);
            if (resAniConfigData == null)
            {
                Logger.LogWarning(string.Format("{0} not found in AnimationConfig", resourceName));
                return;
            }

            Dictionary<string, Dictionary<float, AniEventData>> resAniEventData = NeptuneBattle.Instance.DataProvider.GetAniEvents(resourceName);
            if (resAniEventData == null)
            {
                Debug.LogWarning(string.Format("{0} not found in AniEvents", resourceName));
                return;
            }

            string actionName;
            if (Data.Actions == null || Data.Actions.Count <= 0)
                return;
            for (int i = 0; i < Data.Actions.Count; i++)
            {
                actionName = Data.Actions[i];

                if (!resAniConfigData.ContainsKey(actionName))
                {
                    Logger.LogWarning(string.Format("{0}.{1} not found in AnimationConfig", resourceName, actionName));
                    continue;
                }

                List<AniEventData> events = null;
                bool isHaveEnd = Data.EndEvent > 0 ? true : false;
                bool isFind = false;
                if (!resAniEventData.ContainsKey(actionName))
                {
                    Logger.LogWarning(string.Format("{0}.{1} not found in AniEventData", resourceName, actionName));
                    //continue;
                }
                else
                {
                    Dictionary<float, AniEventData> eventsMap = resAniEventData[actionName];
                    if (eventsMap == null)
                    {
                        continue;
                    }

                    events = new List<AniEventData>();
                    foreach (AniEventData evt in eventsMap.Values)
                    {
                        // 替换End帧事件
                        if (isHaveEnd && evt.Type.Equals("End"))
                        {
                            evt.Time = Data.EndEvent;
                            isFind = true;
                        }
                        events.Add(evt);
                    }

                    events.Sort(new ActionEventComparer());
                }

                // 附加End帧事件
                if (isHaveEnd && !isFind)
                {
                    AniEventData tempAniEventData = new AniEventData();
                    tempAniEventData.Type = "End";
                    tempAniEventData.Time = Data.EndEvent;
                    if (events == null)
                        events = new List<AniEventData>();
                    events.Add(tempAniEventData);
                    events.Sort(new ActionEventComparer());
                }

                TalentAction act = new TalentAction();
                act.ActionName = actionName;
                Actions.Add(act);
                act.Events = events;
                if (resAniConfigData != null && resAniConfigData.ContainsKey(actionName))
                {
                    act.TotalTime = resAniConfigData[actionName].TotalTime;
                }
            }
        }

        public virtual void StartAction(int idx)
        {
            try
            {
                TalentAction action = Actions[idx];
                if (action != null)
                {
                    CurrentActionID = idx;
                    CurrentAction = action;
                    CurrentActionElapsed = 0;
                    NextActID = 0;
                    NextAct = action.Events == null ? null : action.Events[0];
                    Caster.SetAnimation(action.ActionName, false, true);
                }
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("Action Error!Skill[RoleID {0}   {1}:{2}]\n{3}", Caster.Data.ID, Data.TalentGroupID, Data.TalentName, ex);
            }
        }

        public virtual void OnAnimationEnd()
        {
            if (CurrentActionID >= Actions.Count - 1)
            {
                if (Data.ContinuousType == ContinuousType.Continuous && isCanContinuous && CastDuration < Data.ContinuousTime)
                {
                    return;
                }
                End();
            }
            else
                StartAction(CurrentActionID + 1);
        }

        /*
         *******************************
         *  Skill Effect Process
         *******************************
         */

        public virtual void OnHitFrame()
        {
            ResetCD(TalentStatus.HitFrame);
            //移除施法者身上的指定ID的buff
            //Caster.RemoveAbilitys(AbilityRemoveType.OnHit);
            CostMp(CostMPMode.HitFrame);
            //移除施法者身上的指定ID的buff
            if (Data.RemoveCasterAbilityIDs != null && Data.RemoveCasterAbilityIDs.Count > 1)
            {
                for (int i = 0; i < Data.RemoveCasterAbilityIDs.Count; i += 2)
                {
                    int id = Data.RemoveCasterAbilityIDs[i];
                    int count = Data.RemoveCasterAbilityIDs[i + 1];
                    Caster.RemoveAbility(id, count);
                }

                //foreach (int ability in Data.RemoveCasterAbilityIDs)

            }
            if (Data.RemoveCasterMarkIDs != null && Data.RemoveCasterMarkIDs.Count > 1)
            {
                for (int i = 0; i < Data.RemoveCasterMarkIDs.Count; i += 2)
                {
                    int id = Data.RemoveCasterMarkIDs[i];
                    int count = Data.RemoveCasterMarkIDs[i + 1];
                    Caster.RemoveMarkByID(id, count);
                }
            }
            if (Data.RemoveCasterTrapIDs != null && Data.RemoveCasterTrapIDs.Count > 1)
            {
                for (int i = 0; i < Data.RemoveCasterTrapIDs.Count; i += 2)
                {
                    int id = Data.RemoveCasterTrapIDs[i];
                    int count = Data.RemoveCasterTrapIDs[i + 1];
                    NeptuneBattle.Instance.RemoveTrap(Caster, id, count);
                }
            }

            if (Caster.IsActiveCasting)
            {
                NeptuneBattle.Instance.Resume();
                Caster.IsActiveCasting = false;
            }
            hasTalentCasting = false;
            HitTimes = HitTimes + 1;

            if (Data.ContinuousType == ContinuousType.Activate && HitTimes == 1)
            {
                StartActivate();
            }

            if (Data.TargetType != TargetType.Position && Data.TargetType != TargetType.Direction && TargetPosition == UVector3.zero)
            {
                if (Data.TargetType == TargetType.Random || Target == null || Target.IsDead && !Data.FollowSourceTalentTarget)
                {
                    FindTarget(null);
                }

                if (Target == null)
                {
                    if (Caster != null && Casting && (Data.TriggerType & TriggerType.Manual) > 0)
                    {
                        int dist = 0;
                        BattleActor target = Caster.FindTarget(ref dist, this, true, Direction.x, Direction.y);
                        if (target == null)
                        {
                            return;
                        }

                        FindTarget(target);
                    }
                }
                if (Target == null && TargetPosition == UVector3.zero)
                {
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("{0} lost Target break.", ToString());
#endif
                    Break();//目标丢失，可能死亡或其他情况，此时不应直接返回，而应中断当前技能 add by Ray:ray@raymix.net 2016-04-13
                    return;
                }
                TargetPosition = Target.Position;
            }
            if (addMarkHitCount != null && addMarkHitCount.Count > 0)
                addMarkHitCount.Clear();
            switch (Data.CastType)
            {
                case CastType.Cast://投掷
                    {
                        CreateSingleCast();
                    }
                    break;
                case CastType.Bounce://释放弹射
                    {
                        BattleEffect bounce = NObjectPool<BattleEffect>.New();
                        bounce.Init(this, EffectType.Bounce, 0);
                        NeptuneBattle.Instance.AddEffect(bounce);
                        AddMarkCount(bounce.EffectIndex);
                    }
                    break;
                case CastType.Multiple://多重箭效果
                    {
                        CreateMultipleCast();
                    }
                    break;
                case CastType.Divided://分裂箭效果
                    {
                        CreateDividedCast();
                    }
                    break;
                //case CastType.Boomerang: //回旋镖
                //    {
                //        CreateBoomerangCast();
                //    }
                //    break;
                case CastType.Scatter:
                    CreateScatterCast();
                    break;
                case CastType.Round:
                    CreateRoundCast();
                    break;
                case CastType.None:
                    AddMarkCount(0);
                    HitAt(TargetPosition, Target);
                    break;
                default:
                    Logger.LogError(string.Format("Unkonwn CastType: {0}", Data.CastType));
                    break;
            }
            Caster.SetCostValue(Caster.GetCurrentCostValue() + Data.GainMP * NeptuneBattle.Instance.MPBonus);
            if (Caster.BasicTalent != null && (GroupData.TalentGroupID == Caster.BasicTalent.GroupData.TalentGroupID ||
                GroupData.ParentID == Caster.BasicTalent.GroupData.TalentGroupID))
            {
                if (Caster.Attributes.MPAtkRecovery > 0)
                {
                    Caster.Remedy(Caster.Attributes.MPAtkRecovery + Caster.Attributes.MPAtkRecovery_a * EngineConst.Hundredth * Caster.MaxMP, RoleAttribute.MaxMP);
                }
                if (Caster.Attributes.HPAtkRecovery > 0)
                {
                    Caster.Remedy(Caster.Attributes.HPAtkRecovery + Caster.Attributes.HPAtkRecovery_a * EngineConst.Hundredth * Caster.MaxHP);
                }
            }


            if (Data.MoveForward != 0)
            {
                Caster.Position = Caster.Position + Data.MoveForward * Caster.Orientation;
                if (Caster.Joint != null)
                    Caster.Joint.DoActionEvent(ActionEventType.Teleport);
            }
            if (Data.TeleportEventParam != null && Data.TeleportEventParam.Count > 1)
            {
                DoTeleport((TargetType)Data.TeleportEventParam[0], Data.TeleportEventParam[1]);
            }
            if (Data.DashEventParam != null && Data.DashEventParam.Count > 6)
            {
                float delaytime = Data.DashEventParam.Count > 7 ? Data.DashEventParam[7] : 0;
                float pausetime = Data.DashEventParam.Count > 8 ? Data.DashEventParam[8] : 0;
                float dashdirmode = Data.DashEventParam.Count > 9 ? Data.DashEventParam[9] : 0;
                float acceleration = Data.DashEventParam.Count > 10 ? Data.DashEventParam[10] : 0;
                DoDash(Data.DashEventParam[0], (int)Data.DashEventParam[1], (int)Data.DashEventParam[2], (int)Data.DashEventParam[3], Data.DashEventParam[4] <= 0, (int)Data.DashEventParam[5], Data.DashEventParam[6] <= 0, delaytime, pausetime, (int)dashdirmode, (int)acceleration);
            }
            if (Data.SummonActDatas != null && Data.SummonActDatas.Count > 0)
            {
                foreach (SummonActData summonactdata in Data.SummonActDatas)
                {
                    if (summonactdata.SummonTodoType != SummonTodoType.None)
                    {
                        int count = summonactdata.SummonToDoParam;
                        foreach (BattleActor summon in Caster.SummonList)
                        {
                            if (summon == null || summon.IsDead)
                                continue;
                            if (summonactdata.ToDoSummonID != 0 && summonactdata.ToDoSummonID != summon.SummonData.ID)
                                continue;
                            if (summonactdata.SummonTodoType == SummonTodoType.Dead)
                            {
                                if (summonactdata.SummonToDoParam > 0)
                                {
                                    if (count <= 0)
                                        continue;
                                    count--;
                                }
                                summon.LifeTime = 0.0f;
                            }
                            else if (summonactdata.SummonTodoType == SummonTodoType.SummonCastTalent)
                            {
                                summon.CastTalent(summonactdata.SummonToDoParam, Caster.Target);
                            }
                        }
                    }
                }
            }

            PlayExtraEffects(EffectPlayType.OnHitFrame, Caster.Position);
        }

        void DoDash(float time, int speed, int distance, int minDistance, bool ignoreObstacle, int maxDistance, bool NoRoot, float delayPauseTime, float pauseTime, int uselastdashdirmode = 0, int acceleration = 0)
        {//执行冲刺
            time = UFloat.Round(time);
            //TODO: 冲刺过程的碰撞伤害，可通过在这里设置正在冲刺的标志，碰撞及伤害可通过配置Trap类来实现（可考虑通过子技能实现效果配置）
            UVector2 targetPos = TargetPosition != UVector3.zero ? (UVector2)TargetPosition : Target != null ? Target.Position : Caster.Position;
            int length = (targetPos - Caster.Position).magnitude;

            if (Target != null)
            {
                if (Target.IsRoleType(RoleType.Building))
                {
                    length -= Caster.Data.CollideRadius + Target.Data.CollideRadius;
                }
            }

            if (uselastdashdirmode == 1 && LastDashDir != EngineConst.Vector2Zero)
            {
                Caster.Orientation = LastDashDir;
            }
            else if (uselastdashdirmode == 2)
            {
                if (Caster.moveDirection != EngineConst.Vector2Zero)
                    Caster.Orientation = Caster.moveDirection;
                else
                    length = 0;
            }
            else
            {
                Vector2 orientation = (targetPos - Caster.Position).normalized;
                if (orientation != Vector2.zero)
                    Caster.Orientation = orientation;
            }

            Caster.DashAcceleration = Caster.Orientation * acceleration;
            Caster.DashLastDistane = UVector2.zero;
            Caster.DashTotalTime = 0;
            Caster.DashSpeed = UVector2.zero;
            LastDashDir = Caster.Orientation;
            if (minDistance < EngineConst.MinDistance)
            {
                minDistance = EngineConst.MinDistance;
            }
            if (Data.TargetType == TargetType.Target && Target == null || length <= minDistance || maxDistance != 0 && length >= maxDistance)
            {
                //目标类型的冲刺如果没有目标就不冲了
                return;
            }
            int endspeed = speed;
            if (time > 0)
            {//开始冲刺
                if (speed > 0)
                {
                    if (distance > 0)
                    {
                        Caster.DashTime = UFloat.Round(time - UFloat.Round(distance * 1f / speed));
                        Caster.DashTime = Caster.DashTime < 0f ? 0 : Caster.DashTime;
                    }
                    else
                    {
                        Caster.DashTime = time;
                    }
                    Caster.MoveSpeed = Caster.Orientation * speed;
                    endspeed = speed + UFloat.RoundToInt(acceleration * time);
                }
                else if (speed == 0)
                {
                    Caster.DashTime = time;
                    int temp = UFloat.RoundToInt(UFloat.Round((length + distance) / time) - UFloat.Round(acceleration * time * 0.5f));
                    Caster.MoveSpeed = Caster.Orientation * temp;
                    endspeed = temp + UFloat.RoundToInt(acceleration * time);
                }
                else
                {
                    Caster.DashTime = time;
                    Caster.MoveSpeed = Caster.Orientation * speed;
                    endspeed = speed + UFloat.RoundToInt(acceleration * time);
                }

            }
            else
            {
                if (speed != 0)
                {

                    if (acceleration != 0)
                        endspeed = UFloat.RoundToInt(Mathf.Sqrt((length + distance) * acceleration * 2 + speed * speed));
                    if (speed < 0)
                        endspeed = -endspeed;
                    Caster.DashTime = UFloat.Round((length + distance) / (speed + endspeed) * 2);
                    Caster.MoveSpeed = Caster.Orientation * speed;
                }

            }

            UVector2 destPos = Caster.Position + Caster.MoveSpeed * Caster.DashTime + Caster.DashAcceleration * UFloat.Round(Caster.DashTime * Caster.DashTime * 0.5f);
            obstacleWallHit = NeptuneBattle.Instance.RuleManager.ObstacleWallHit(Caster.Position, destPos);
            if (Data.CheckTrapObstacle && obstacleWallHit == UVector2.zero)
            {
                obstacleWallHit = NeptuneBattle.Instance.HitTrap(Caster, Caster.Position, destPos);
            }
            if (obstacleWallHit != UVector2.zero)
            {
                targetPos = obstacleWallHit - Caster.MoveSpeed.normalized * 50;
                if ((Caster.Position - targetPos).magnitude <= EngineConst.MinDistance || MathUtil.Dot((targetPos - Caster.Position).normalized, Caster.MoveSpeed.normalized) < 0)
                {
                    Caster.DashTime = 0;
                    Caster.MoveSpeed = UVector2.zero;
                    Caster.DashAcceleration = UVector2.zero;
                    return;
                }
            }
            else
            {
                targetPos = Caster.NavAgent.GetDashObstacleHit(Caster.Position, destPos, ignoreObstacle);
            }
            float moveSpeedMagnitude = UFloat.Round((Caster.MoveSpeed.magnitude + Math.Abs(endspeed)) * 0.5f);
            if (moveSpeedMagnitude != 0)
            {
                Caster.DashTime = UFloat.Round((Caster.Position - targetPos).magnitude / moveSpeedMagnitude);
            }
            else
            {
                Caster.DashTime = 0;
            }
            if (Caster.DashTime <= 0.0001f)
            {
                Caster.MoveSpeed = UVector2.zero;
                Caster.DashAcceleration = UVector2.zero;
            }
            else
            {
                Caster.DashDelayPauseTime = delayPauseTime;
                Caster.DashPauseTime = pauseTime;
                Caster.DashNoRoot = NoRoot;
            }
            Caster.DashSpeed = Caster.MoveSpeed;
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} DoDash :{1}  {2:f6} {3:f6}  {4:f6}  {5:f6} {6:f6} {6:f6} {7:f6}", Caster.FullName, FullName(), Caster.DashTime, Caster.MoveSpeed, Caster.DashPauseTime, Caster.DashDelayPauseTime, moveSpeedMagnitude,
                    endspeed, destPos);
#endif
        }

        void DoTeleport(TargetType targetType, int distance)
        {//执行瞬移
         //TODO：根据技能表配置的瞬移参数，计算瞬移目标位置
            UVector2 targetPos = TargetPosition != UVector3.zero ? (UVector2)TargetPosition : Target != null ? Target.Position : Caster.Position;
            Vector2 direction = (targetPos - Caster.Position).normalized;
            UFloat.Round(ref direction);
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} {1} DoTeleport Start: Target={2} type={3} dist={4} dir=({5:f6},{6:f6}) caster:{7}", Caster.FullName, FullName(), targetPos, targetType, distance, direction.x, direction.y, Caster.Position);
#endif
            UVector2 pos = targetPos;
            if (direction == EngineConst.Vector2Zero)
            {
                direction = Caster.Orientation;
            }
            targetPos = Caster.Position;
            Vector2 offset = EngineConst.Vector2Zero;
            if (targetType == TargetType.Self)
            {
                offset = Caster.Orientation * distance;
                UFloat.Round(ref offset);
                targetPos += offset;
            }
            else if (targetType == TargetType.Target)
            {
                if (Target != null)
                {
                    offset = direction * distance;
                    targetPos = Target.Position + offset;
                    Caster.Orientation = direction;
                }
            }
            else if (targetType == TargetType.Position && direction != Vector2.zero)
            {
                distance += UVector2.Distance(pos, Caster.Position);
                if (distance >= MaxRange)
                {
                    distance = MaxRange;
                    offset = direction * distance;
                    UFloat.Round(ref offset);
                    targetPos += offset;
                }
                else
                    targetPos = pos;
                Caster.Orientation = direction;
            }
            else if (targetType == TargetType.Direction)
            {
                offset = direction * distance;
                UFloat.Round(ref offset);
                targetPos += offset;
                Caster.Orientation = direction;
            }
            if ((targetPos - Caster.Position).magnitude <= EngineConst.MinDistance)
            {
                return;
            }
            UVector2 perPos = targetPos;

#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} {1} DoTeleport Ready: Target={2} type={3} dist={4} dir=({5:f6},{6:f6}) offset=({7:f6},{8:f6})", Caster.FullName, FullName(), targetPos, targetType, distance, direction.x, direction.y, offset.x, offset.y);
#endif
            obstacleWallHit = NeptuneBattle.Instance.RuleManager.ObstacleWallHit(Caster.Position, targetPos);
            if (Data.CheckTrapObstacle && obstacleWallHit == UVector2.zero)
            {
                obstacleWallHit = NeptuneBattle.Instance.HitTrap(Caster, Caster.Position, targetPos);
            }
            if (obstacleWallHit != UVector2.zero)
            {

                targetPos = obstacleWallHit + Caster.MoveSpeed.normalized * 50;
                if ((Caster.Position - targetPos).magnitude > EngineConst.MinDistance && MathUtil.Dot((targetPos - Caster.Position).normalized, Caster.MoveSpeed.normalized) >= 0)
                {
                    Caster.Position = targetPos;
                }
            }
            else
            {
                targetPos = Caster.NavAgent.GetTeleportPos(Caster.Position, targetPos);

                int result = (Caster.Position - perPos).magnitude < (Caster.Position - targetPos).magnitude ? 1 : -1;
                if (targetPos == perPos)
                {
                    result = 0;
                }
                Caster.Position = targetPos + result * Caster.Orientation.normalized * 33;
            }

#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} {1} DoTeleport Final: Target={2} type={3} dist={4} dir=({5:f6},{6:f6}) pos:{7}", Caster.FullName, FullName(), targetPos, targetType, distance, direction.x, direction.y, Caster.Position);
#endif
            if (Caster.OrcaAgent != null)
            {
                Caster.OrcaAgent.SetPosition(Caster.Position);
                Caster.OrcaAgent.Sync();
            }
            if (Caster.Joint != null)
                Caster.Joint.DoActionEvent(ActionEventType.Teleport);
        }

        void DoTalent(int talentID)
        {//执行技能
            BattleSkill talent = Caster.GetTalentById(talentID);
            if (talent != null)
            {
                //this.Caster.UseTalent(talent, this.Target);
                if (talent.Data.TargetType == TargetType.Self)
                {
                    talent.Target = Caster;
                }
                else if (talent.Data.TargetType == TargetType.Target)
                {
                    talent.Target = Target;
                }
                else
                {
                    talent.Target = null;
                }
                if (talent.CanUse(talent.Target) == ResultType.Success)
                {
                    if (talent.Data.TargetType == TargetType.Position || talent.Data.TargetType == TargetType.Direction)
                    {
                        talent.Start(TargetPosition);
                    }
                    else
                    {
                        talent.Start(talent.Target);
                    }

                }
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("{0} DoTalent :{1}  {2}", Caster.FullName, talent.FullName(), talent.Target == null ? "null" : talent.Target.FullName);
#endif
            }
        }


        /// <summary>
        /// 创建单个抛射器
        /// </summary>
        void CreateSingleCast()
        {
            if (Data.FlyDmgFactor != null && Data.FlyDmgFactor.Count > 0)
            {
                if (dividedHitTarget == null)
                    dividedHitTarget = new Dictionary<BattleActor, int>();
                else
                    dividedHitTarget.Clear();
            }
            BattleEffect cast = CreateCast();
            if (Data.FlyBounce > 0)
            {
                cast.SetBounce(Data.FlyBounce);
            }
            if (Target != null)
            {
                if (Data.FlyTrackingTarget)
                {
                    cast.TrackTarget(Target);
                }
            }

            NeptuneBattle.Instance.AddEffect(cast);
            AddMarkCount(cast.EffectIndex);
        }

        /// <summary>
        /// 创建多重抛射器(多重箭)
        /// </summary>
        void CreateMultipleCast()
        {
            List<UVector2> OffsetMap = new List<UVector2>() {
          new UVector2(-5, 14),
          new UVector2(-24, 19),
          new UVector2(0, 0),
          new UVector2(-41, 42),
          new UVector2(34, -36)
        };
            float count = (Data.CastNum - HitTimes) / Data.CastTimes + 1;
            BattleActor oldTarget = Target;
            for (int i = 1; i <= count; i++)
            {
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("{0}: {1} --> {2} crete projectile {3}: {4}＝({5} - {6})/5+1", Caster.FullName, ToString(), Target != null ? Target.FullName : "", i, count, Data.CastNum, HitTimes);
#endif
                BattleEffect cast = CreateCast(i - 1);
                if (i > 1)
                    FindTarget();
                OffsetMap[i - 1] = OffsetMap[i - 1] + new Vector2(5, -14);
                if (i == 1 || Target != oldTarget)
                {
                    //Vector2 dist = new Vector2(this.Target.Position.x + OffsetMap[i - 1].x * this.Caster.InitDirection.x, this.Target.Position.y + OffsetMap[i - 1].y * this.Caster.InitDirection.y) - this.Caster.Position;
                    UVector2 dist = new UVector2(Target.Position.x + OffsetMap[i - 1].x * Vector2.right.x, Target.Position.y + OffsetMap[i - 1].y * Vector2.right.y) - Caster.Position;
                    cast.Speed = dist * Data.FlyGravity / (-cast.SpeedZ - (float)Mathf.Sqrt(cast.SpeedZ * cast.SpeedZ - 2f * Data.FlyGravity * cast.Height));
                    // cast.Speed = dist * this.Data.FlyGravity / (-cast.SpeedZ - (float)Mathf.Pow((float)Mathf.Pow(cast.SpeedZ, 2f) - 2f * this.Data.FlyGravity * cast.Height, 0.5f));
                    NeptuneBattle.Instance.AddEffect(cast);
                    AddMarkCount(cast.EffectIndex);
                }
                else
                {
                    cast.Delete();
                }
            }
        }

        /// <summary>
        /// 创建分裂抛射器(分裂箭)
        /// </summary>
        void CreateDividedCast()
        {
            if (dividedHitTarget == null)
                dividedHitTarget = new Dictionary<BattleActor, int>();
            else
                dividedHitTarget.Clear();
            if (Data.FlyCastDirections != null && Data.FlyCastDirections.Count > 0)
            {
                int counter = 1;
                for (int i = 0; i < Data.FlyCastDirections.Count; i++)
                {
                    int angle = Data.FlyCastDirections[i];
                    BattleEffect effect = NObjectPool<BattleEffect>.New();
                    effect.Init(this, EffectType.Cast, angle, counter - 1);
                    NeptuneBattle.Instance.AddEffect(effect);
                    AddMarkCount(effect.EffectIndex);
                    counter++;
                }
            }
            else
            {
                BattleActor oldTarget = Target;
                int counter = 1;
                bool isAttackMainTarget = false;
                BattleEntity pos = Caster;
                if (AttackPointElement != null)
                {
                    pos = AttackPointElement;
                }
                isAttackMainTarget = true;
                BattleEffect cast = null;
                if (Target != null)
                {
                    cast = CreateCast(counter - 1);
                    cast.DamageFactor = 1;
                    cast.TrackTarget(Target);
                    NeptuneBattle.Instance.AddEffect(cast);
                    AddMarkCount(cast.EffectIndex);
                }

                //            foreach (Actor role in Logic.Instance.GetSurvivors(this.Caster, this.AffectedSide, this.Data.MaxRange))
                foreach (BattleActor role in NeptuneBattle.Instance.AOIManager.GetSurvivors(pos, AffectedSide, MaxRange))
                {
                    if (counter >= Data.CastNum)
                        break;
                    if (!CheckTargetClass(role))
                        continue;
                    if (role == oldTarget)
                        continue;
                    if (Data.TargetSelectorWithOutSelf && role == Caster)
                        continue;
                    /*if (counter == this.Data.CastNum - 1 && !isAttackMainTarget)
                    {
                        this.Target = oldTarget;
                        isAttackMainTarget = true;
                    }
                    else */
                    if (pos.Distance(role, EngineConst.EnableRadiusInDistance) < MaxRange)
                    {
                        Target = role;
                    }
                    else
                        continue;
                    counter++;
                    cast = CreateCast(counter - 1);
                    //                 if (this.Target == oldTarget)
                    //                 {
                    //                     isAttackMainTarget = true;
                    //                     cast.DamageFactor = 1;
                    //                 }
                    //                 else
                    cast.DamageFactor = Data.Param1;
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("{0}: {1} --> {2} DividedCast {3}: {4:f6}", Caster.FullName, FullName(), Target.FullName, counter, cast.DamageFactor);
#endif
                    cast.TrackTarget(Target);
                    NeptuneBattle.Instance.AddEffect(cast);
                    AddMarkCount(cast.EffectIndex);
                }
            }
        }


        /// <summary>
        /// 创建散射
        /// </summary>
        void CreateScatterCast()
        {
            var angle = Data.Param2;
            var castCount = Data.CastNum;

            if ((int)(angle * castCount) == 360)
            {
                var casterOri = Caster.Orientation;
                for (int i = 0; i < castCount; i++)
                {
                    Vector2 castDirection = MathUtil.Deflection(casterOri, angle * i);

                    // create a cast but dont need to know wheather it has a target
                    BattleEffect cast = CreateCast(i);
                    if (cast != null)
                    {
                        cast.Speed = castDirection * cast.Speed.magnitude;
                    }
                    NeptuneBattle.Instance.AddEffect(cast);
                    AddMarkCount(cast.EffectIndex);
                }
            }
        }

        void CreateRoundCast()
        {

        }
        public virtual BattleEffect CreateCast(int index = 0)
        {
            BattleEffect cast = NObjectPool<BattleEffect>.New();
            cast.Init(this, EffectType.Cast, 0, index);
            return cast;
        }

        /// <summary>
        /// 命中位置
        /// </summary>
        /// <param name="hitpos">在指定位置命中</param>
        /// <param name="target"></param>
        /// <param name="from"></param>
        /// <param name="hitType"></param>
        public virtual void HitAt(UVector2 hitpos, BattleActor target, BattleEntity from = null, TalentHitType hitType = TalentHitType.TalentHit)
        {

            var orientation = Orientation != EngineConst.Vector2Zero ? Orientation : Caster.Orientation;
            if (Data.AreaCenter == TargetType.Self)
                hitpos = Caster.Position;
            UVector2 pos = hitpos + Data.AreaShift * orientation;
            bool isHit = false;
            BattleActor soundTarget = null;
            if (Data.AreaCenter != TargetType.None && Data.AreaShape != AreaShape.None)
            {
                if (target != null && CheckTargetClass(target) && (target.GetRelation(from == null ? Caster.Side : from.Side) == AffectedSide || AffectedSide == RelativeSide.Both))
                {
                    UVector2 distance = target.Position - pos;
                    bool inIgnore = false;
                    if (Data.IgnoreAreaShape != AreaShape.None && EngineUtil.HitTest(distance, Data.IgnoreAreaShape, new UVector2(from == null ? Data.IgnoreAreaArg1 + Caster.Radius + target.Radius : Data.IgnoreAreaArg1, Data.IgnoreAreaArg2),
                        orientation))
                    {
                        inIgnore = true;
                    }
                    if (!inIgnore && (EngineUtil.HitTest(distance, Data.AreaShape, new UVector2(from == null ? Data.AreaArg1 + Caster.Radius + target.Radius + Data.MainTargetAddRange : Data.AreaArg1 + Data.MainTargetAddRange, Data.AreaArg2),
                        orientation) || HitCheckAffiliatedAreaShape(target, hitpos, orientation, new UVector2(from == null ? Caster.Radius + target.Radius + Data.MainTargetAddRange : Data.MainTargetAddRange, 0))))
                    {
                        if (HitTarget(target, from, hitType) > 0)
                            soundTarget = target;
                    }
                }

                int range = Data.AreaArg1;
                if (Data.AreaShape == AreaShape.Rectangle)
                {
                    range = (int)new Vector2(Data.AreaArg1, Data.AreaArg2).magnitude;
                }
                //            foreach (Actor role in Logic.Instance.GetSurvivors(this.Caster, this.AffectedSide, this.Data.MaxRange))
                foreach (BattleActor role in NeptuneBattle.Instance.AOIManager.GetSurvivors(pos, from == null ? Caster.Side : from.Side, AffectedSide, Data.MaxRange + (Data.AreaCenter == TargetType.Self ? 0 : range), from == null ? Caster.Radius : from.Radius))
                {
                    if (hitTargetNum > MaxTargetNum && MaxTargetNum > 0)
                    {
                        break;
                    }
                    if (Data.TargetSelectorWithOutSelf && role == Caster)
                        continue;
                    if (!CheckTargetClass(role))
                        continue;


                    if (target == role)
                        continue;

                    if (!(Caster.Data.AttackAir || Data.CanAttackAir) && role.IsAirForce)
                        continue;

                    UVector2 dist = role.Position - pos;
                    //TODO:换成矢量方向后计算需效验
                    //dist.x = dist.x * this.Caster.Orientation; 
                    if (Data.IgnoreAreaShape != AreaShape.None && EngineUtil.HitTest(dist, Data.IgnoreAreaShape, new UVector2(from == null ? Data.IgnoreAreaArg1 + Caster.Radius + role.Radius : Data.IgnoreAreaArg1, Data.IgnoreAreaArg2),
                       orientation))
                    {
                        continue;
                    }
                    if (EngineUtil.HitTest(dist, Data.AreaShape, new UVector2(from == null ? Data.AreaArg1 + Caster.Radius + role.Radius : Data.AreaArg1, Data.AreaArg2),
                        orientation) || HitCheckAffiliatedAreaShape(role, hitpos, orientation, new UVector2(from == null ? Caster.Radius + role.Radius : 0, 0)))
                    {

                        if (HitTarget(role, from, hitType) > 0 && soundTarget == null)
                        {
                            soundTarget = role;
                        }
                    }
                }
                if (soundTarget != null && soundTarget.Joint != null)
                {
                    isHit = true;
                    if (!string.IsNullOrEmpty(Data.HitSound))
                        soundTarget.PlaySound(Data.HitSound);
                }
            }
            else if (target != null)
            {
                HitTarget(target, from, hitType);
                isHit = true;
            }
            castingTrapList.Clear();
            if (Data.TrapIds != null && Data.TrapIds.Count > 0)
            {
                TrapTriggerType trapTriggerType = TrapTriggerType.None;
                if (hitType == TalentHitType.TalentHit)
                {
                    trapTriggerType = TrapTriggerType.Hit;
                }
                else
                {
                    if ((hitType & TalentHitType.EffectHit) > 0)
                        trapTriggerType |= TrapTriggerType.Hit;
                    if ((hitType & TalentHitType.EffectEnd) > 0)
                        trapTriggerType |= TrapTriggerType.EffectEnd;
                }
                CreateTrap(trapTriggerType, pos, target);
            }
            if (Data.TriggerTrapID > 0)
            {
                NeptuneBattle.Instance.TriggerTrap(Caster, Data.TriggerTrapID);
            }

            TalentSummon(hitpos);//技能召唤

            if (!string.IsNullOrEmpty(Data.DirectEffect))
            {
                bool createEffect = true;
                if (Caster.Joint != null && Caster.Joint.Controller != null)
                {
                    createEffect = Caster.Joint.Controller.InSight;
                }
                if (createEffect)
                    NeptuneBattle.PlayEffect(Data.DirectEffect, pos, Caster.Orientation, 0, Data.DirectOrder);
                //Logic.PlayEffect(this.Data.DirectEffect, pos, Vector2.one, 0, this.Data.DirectOrder);

            }
            NeptuneBattle.Instance.OnRoleHit(Owner);
            PlayExtraEffects(EffectPlayType.OnHitAt, pos);
        }

        public void CreateTrap(TrapTriggerType triggerType, Vector2 pos, BattleActor target, Action _act = null)
        {
            foreach (int trapId in Data.TrapIds)
            {
                if ((NeptuneBattle.Instance.DataProvider.GetTrapData(trapId).TriggerType & triggerType) > 0)
                {
                    BattleTrap trap = ObjectFactory.Create(trapId, this, pos, target);
                    if (_act != null) trap.OnEndAct(_act);
                    if (trap != null)
                    {
                        NeptuneBattle.Instance.AddTrap(trap);
                        castingTrapList.Add(trap);
                    }
                }
            }
        }

        public BattleTrap GetCreateTrap(TrapTriggerType triggerType, Vector2 pos, BattleActor target, Action _act = null)
        {
            foreach (int trapId in Data.TrapIds)
            {
                if ((NeptuneBattle.Instance.DataProvider.GetTrapData(trapId).TriggerType & triggerType) > 0)
                {
                    BattleTrap trap = ObjectFactory.Create(trapId, this, pos, target);
                    if (_act != null) trap.OnEndAct(_act);
                    if (trap != null)
                    {
                        NeptuneBattle.Instance.AddTrap(trap);
                        castingTrapList.Add(trap);
                        return trap;
                    }
                }
            }
            return null;
        }

        protected virtual void TalentSummon(UVector2 pos)
        {
            if (Data.SummonID > 0)
            {
                SummonData data = NeptuneBattle.Instance.DataProvider.GetSummonData(Data.SummonID, Data.SummonLevel);
                if (data == null)
                    return;
                //TODO:执行召唤
                for (int i = 0; i < Data.SummonNumber; i++)
                {
                    //初始化角色配置
                    RoleInfo hero = new RoleInfo();
                    hero.tid = Data.SummonID;
                    hero.level = Data.SummonLevel == 0 ? Caster.Level : Data.SummonLevel;
                    RoleConfig role_config = new RoleConfig();
                    //role_config.HPFactor = (float)EngineDataManager.Instance.RoleConfigs.Value[(int)this.Data.SummonID].HPFactorArena;
                    role_config.IsMonster = true;
                    //Actor.InitConfigPredictInfo(role_config, this.Data.SummonID, false);

                    //创建Role
                    BattleActor role = ObjectFactory.Create(hero, Caster.Side, role_config, null, pos + Caster.Orientation * 1, Caster.Orientation);
                    role.Player = Caster.Player;
                    NeptuneBattle.Instance.AddActor(role, Caster, pos + Caster.Orientation * 1);
                    role.InitSummon(data, role_config, Caster, Data.SummonLevel);
                }
            }
        }

        public virtual int HitTarget(BattleActor target, BattleEntity from = null, TalentHitType hitType = TalentHitType.TalentHit)
        {
            BattleEntity Source = from ?? Caster;

            if (CheckIgnoreTarget(target))
                return 0;

            if (Data.Dispel)
            {//驱散具有最高优先级
                for (int cIndex = 0; cIndex < Caster.tAbilities.Length; cIndex++)
                {
                    Ability ability = Caster.tAbilities[cIndex];
                    if (ability != null && ability.Caster.Side != Caster.Side)
                    {
                        Caster.RemoveAbilityAt(cIndex);
                    }
                }
                //List<Ability> needRemoveAbilities = this.Caster.Abilities.Where(ability => ability.Caster.Side != this.Caster.Side).ToList();
                //this.Caster.RemoveAbilities(needRemoveAbilities);
            }
            //不受释放限制
            if (Data.Unrestricted)
            {
                for (int cIndex = 0; cIndex < Caster.tAbilities.Length; cIndex++)
                {
                    Ability ability = Caster.tAbilities[cIndex];
                    if (ability != null && ability.Caster.Side != Caster.Side && ability.IsControlAbility)
                    {
                        Caster.RemoveAbilityAt(cIndex);
                    }
                }

                //List<Ability> needRemoveAbilities = this.Caster.Abilities.Where(ability =>
                //    ability.Caster.Side != this.Caster.Side && ability.IsControlAbility).ToList();
                //this.Caster.RemoveAbilities(needRemoveAbilities);
            }


            //Lyon ：target.IsActiveCasting 用于HCU3D 放大招冻结相关逻辑 我们这边不需要
            //if (target ==null || target.IsDead || target.AbilityEffects.Invincible || target.IsActiveCasting && Source.Side != target.Side)
            if (target == null || target.IsDead || target.AbilityEffects.Invincible)
            {
                return 0;
            }
            BattleEffect effect = from as BattleEffect;
            if (effect != null && effect.Joint != null)
                effect.Joint.OnHit(target);
            if (target.Joint != null && !string.IsNullOrEmpty(Data.ContinuousEffect))
            {
                if (!targetContinuousEffects.ContainsKey(target))
                {
                    targetContinuousEffects[target] = target.Joint.AddEffect(Data.ContinuousEffect, EffectType.Normal, UVector3.zero, Caster.Joint.Controller);
                }
            }
            if (Data.ContinuousType != ContinuousType.None && HitTimes == 1)
            {
                return 0;
            }
            hitTargetNum++;

            TriggerMark(target, Data.MarksToTrigger, MarkTriggerType.BeforeInijury);
            if (HasAbility(AbilityTriggerType.Hit, AbilityTriggerMode.BeforeInijury))
            {
                CreateTalentAbilitis(target, AbilityTriggerType.Hit, hitType);
            }

            float factor = 0;
            float baseForce = NeptuneBattle.Instance.Numeric.CalculateTalentForce(this, Source, target, ref factor);
            float curHpDamage = 0;
            if (Data.CurHpDamageRatio > 0)
            {
                curHpDamage = UFloat.Round(UFloat.Round(Data.CurHpDamageRatio * EngineConst.Hundredth) * target.HP);
                if (Data.TargetTypeAffect == target.Data.RoleType || (Data.TargetTypeAffect & RoleType.Demon) == RoleType.Demon && target.Config.IsDemon)
                {
                    if (Data.ExtraTargetTypeRatio > 0)
                        curHpDamage = UFloat.Round(curHpDamage * Data.ExtraTargetTypeRatio);
                }
                if (target.IsCreep)
                {
                    curHpDamage = Math.Min(curHpDamage, EngineConst.CreepCurHpMaxDamage);
                }
            }
            baseForce = UFloat.Round(baseForce + curHpDamage);
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0}: {1} --> {2} (-{3:f6}) {4:f6}", Caster.FullName, FullName(), target.FullName, baseForce, factor);
#endif

            if (Data.EffectFlyTimeDamageRatio != 0 && castFlyTime > 0 && ((hitType & TalentHitType.EffectHit) > 0 || (hitType & TalentHitType.EffectEnd) > 0))
            {
                baseForce = Mathf.Max(baseForce * Mathf.Min(UFloat.Round(1 + castFlyTime * Data.EffectFlyTimeDamageRatio), Data.MaxEffectFlyTimeDamageRatio), 1.0f);
            }
            Caster.ExecutePassive(TriggerType.Damage, target, this);

            if (effect != null && Data.FlyDmgFactor != null && Data.FlyDmgFactor.Count > 0)
            {
                if (CastType.Cast == Data.CastType && Data.CastEffectDmgFactorType == CastEffectDmgFactorType.All)
                {
                    float dmgFactor = effect.HitCount < Data.FlyDmgFactor.Count ? Data.FlyDmgFactor[effect.HitCount] : Data.FlyDmgFactor[Data.FlyDmgFactor.Count - 1];
                    baseForce = UFloat.Round(baseForce * dmgFactor);
                    effect.HitCount++;
                }
                else if (CastType.Divided == Data.CastType || CastType.Cast == Data.CastType)
                {
                    if (dividedHitTarget.ContainsKey(target))
                        dividedHitTarget[target]++;
                    else
                        dividedHitTarget.Add(target, 0);
                    int index = dividedHitTarget[target] >= Data.FlyDmgFactor.Count ? Data.FlyDmgFactor.Count - 1 : dividedHitTarget[target];
                    baseForce = UFloat.Round(baseForce * Data.FlyDmgFactor[index]);//分裂箭伤害衰减
                }
                else
                {
                    if (Data.FlyBounce > effect.LeftAttackCount)
                    {
                        baseForce = UFloat.Round(baseForce * Data.FlyDmgFactor[Data.FlyBounce - effect.LeftAttackCount]);//计算弹射伤害
                    }
                }
            }
            baseForce = UFloat.Round(baseForce * DamageFactor);
            InjuryType injuryType = Data.DamageType;
            int result = 0;
            if (injuryType != InjuryType.None)
            {
                if (Data.AffectedSide == RelativeSide.Both && Caster.GetRelation(target) == RelativeSide.Friend)
                {//影响全体的技能，如果目标为友方，自动转为治疗效果
                    injuryType = InjuryType.Heal;
                    if (Data.Param1 > 0)
                        baseForce = UFloat.Round(baseForce * Data.Param1);
                }
                RoleAttribute attributeType = Data.AffectMP ? target.Data.MPType == MPType.Rage ? RoleAttribute.MaxRage : RoleAttribute.MaxMP : RoleAttribute.MaxHP;

                if (injuryType == InjuryType.Heal)
                {
                    if (Data.TargetSide != RelativeSide.Both && Caster.Side != target.Side)//绝对不治疗敌方
                        return 0;

                    float healValue = attributeType == RoleAttribute.MaxHP ? Caster.Attributes.Heal : 0;
                    baseForce = UFloat.Round(baseForce * (1 + healValue / 100));
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("{0}: {1} --> {2} (+{3:f6})", Caster.FullName, FullName(), target.FullName, baseForce);
#endif
                    target.Remedy(baseForce, attributeType, Caster);

                    Caster.OnRemedy(target);
                }
                else
                {
                    if (Caster.Side == target.Side) //绝对不攻击友方
                    {
                        return 0;
                    }

                    if (TargetDodge(target, injuryType))
                    {
                        return 0;
                    }
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("{0}: {1} --> {2} PWR(-{3:f6})", Caster.FullName, FullName(), target.FullName, baseForce);
#endif
                    target.InjuryFactor = 1;
                    target.InjuryCrit = (Data.CRITPct + Caster.PassiveEffect.CRITPct) / 100f;
                    int critCount = 0;
                    target.ExecutePassive(TriggerType.IsAttacked, Caster, this);
                    if (target.PassiveEffect.ApportionTotalRatio > 0 && attributeType == RoleAttribute.MaxHP)
                    {
                        baseForce = target.ApportionInjury(injuryType, attributeType, baseForce);
                    }

                    result = CalculateInjury(injuryType, attributeType, target, baseForce, Caster, ref critCount);
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("{0}: {1} --> {2} DMG(-{3})", Caster.FullName, FullName(), target.FullName, result);
#endif

                    if (result <= 0)
                    {
                        return 0;
                    }

                    //追加技能额外伤害开始
                    if (Caster.Attributes.ExtraAbilityPower > 0)
                    {
                        int extraDamage = Caster.Attributes.ExtraAbilityPower;
                        if (Data.ExtraDamageType == InjuryType.AbilityPower)
                        {
                            extraDamage += Data.ExtraBasicNum;
                        }
                        target.CalculateInjury(Data.ExtraDamageType, Data.ExtraAffectMP ? RoleAttribute.MaxMP : RoleAttribute.MaxHP, extraDamage, Caster, this, ref critCount);
                    }
                    if (Caster.Attributes.ExtraAttackDamage > 0)
                    {
                        int extraDamage = Caster.Attributes.ExtraAttackDamage;
                        if (Data.ExtraDamageType == InjuryType.AttackDamage)
                        {
                            extraDamage += Data.ExtraBasicNum;
                        }
                        target.CalculateInjury(Data.ExtraDamageType, Data.ExtraAffectMP ? RoleAttribute.MaxMP : RoleAttribute.MaxHP, extraDamage, Caster, this, ref critCount);
                    }
                    if (Caster.Attributes.ExtraHoly > 0)
                    {
                        int extraDamage = Caster.Attributes.ExtraHoly;
                        if (Data.ExtraDamageType == InjuryType.Holy)
                        {
                            extraDamage += Data.ExtraBasicNum;
                        }
                        target.CalculateInjury(Data.ExtraDamageType, Data.ExtraAffectMP ? RoleAttribute.MaxMP : RoleAttribute.MaxHP, extraDamage, Caster, this, ref critCount);
                    }
                    if (Data.ExtraBasicNum > 0)
                        target.CalculateInjury(Data.ExtraDamageType, Data.ExtraAffectMP ? RoleAttribute.MaxMP : RoleAttribute.MaxHP, Data.ExtraBasicNum, Caster, this, ref critCount);

                    // 计算对特定目标的额外伤害
                    CalculateTargetExtraInjury(Caster, target, this, Data.GetTargetExtraDamageData(), ref critCount);

                    //追加技能额外伤害结束
                    float addHp = 0;
                    float addMp = 0;
                    if (!target.IsRoleType(RoleType.Building) && (Data.StealRoleType <= 0 || target.IsRoleType(Data.StealRoleType))
                        && (!Data.StealMainTarget || Target == target))
                    {
                        if (Data.StealPct > 0)
                        {//生命偷取
                            addHp = UFloat.Round(result * Data.StealPct * EngineConst.Hundredth);
                        }
                        addHp = UFloat.Round(addHp + result * NeptuneBattle.Instance.Numeric.GetLifeStealFactor(Caster, this));
                        if (Data.StealMpPct > 0)
                        {//魔法偷取
                            addMp = UFloat.Round(result * Data.StealMpPct * EngineConst.Hundredth);
                        }
                    }

                    if (addHp > 1)
                    {
#if BATTLE_LOG
                        if (EngineGlobal.BattleLog)
                            NeptuneBattle.log("  {0} leeches {1:f6} {2} form {3}", Caster.FullName, addHp, injuryType, target.FullName);
#endif
                        Caster.Remedy(addHp, attributeType);
                    }

                    if (addMp > 1)
                    {
#if BATTLE_LOG
                        if (EngineGlobal.BattleLog)
                            NeptuneBattle.log("  {0} leeches {1:f6} {2} form {3}", Caster.FullName, addMp, injuryType, target.FullName);
#endif
                        Caster.Remedy(addMp, RoleAttribute.MaxMP);
                    }



                    if (critCount > 0)
                    {
                        //造成暴击触发
                        Caster.OnCrit(this);
                    }
                    //造成伤害触发
                    Caster.OnDamage(target, this);
                    //被攻击时技能触发
                    target.OnIsAttacked(Caster, this, result);

                }
            }
            bool abilityMiss = false;
            if (HasAbility(AbilityTriggerType.Hit))
            {
                abilityMiss = CreateTalentAbilitis(target, AbilityTriggerType.Hit, hitType);
            }
            //移除目标身上的指定ID的buff
            /*if(!target.IsDead)
            {
                target.RemoveAbilitys(AbilityRemoveType.OnHurt);
            }*/
            //移除目标身上的指定ID的buff
            if (!target.IsDead && Data.RemoveTargetAbilityIDs != null && Data.RemoveTargetAbilityIDs.Count > 1)
            {
                for (int i = 0; i < Data.RemoveTargetAbilityIDs.Count; i += 2)
                {
                    int id = Data.RemoveTargetAbilityIDs[i];
                    int count = Data.RemoveTargetAbilityIDs[i + 1];
                    target.RemoveAbility(id, count);
                }
            }


            if (!abilityMiss && (0 < Data.RepelHeight || Data.RepelBackDist != 0 && Data.RepelBackTime > 0))
            {
                float backTime = Data.RepelBackTime;
                float backDist = Data.RepelBackDist;
                float acceleration = Data.RepelAcceleration;
                Vector2 direction;
                Vector2 offset = Data.RepelOffset != null && Data.RepelOffset.Count > 1 ? new Vector2(Data.RepelOffset[0], Data.RepelOffset[1]) : Vector2.zero;
                Vector2 offsetX;
                Vector2 offsetY;
                if (Data.AttractDirectType == AttractDirectType.TargetPosition)
                {
                    direction = target.Position - (UVector2)TargetPosition;
                    offsetX = direction.normalized * offset.x;
                    offsetY = MathUtil.GetRight(direction.normalized) * offset.y;
                    direction += offsetX + offsetY;
                }
                else
                {
                    if ((target.Position - Source.Position).magnitude < 0.01f || Data.AttractDirectType == AttractDirectType.SourceDirection || Data.TargetType == TargetType.Direction || Data.TargetType == TargetType.Position)
                    {
                        direction = Source.Orientation;
                    }
                    else
                    {
                        direction = target.Position - Source.Position;
                        offsetX = direction.normalized * offset.x;
                        offsetY = MathUtil.GetRight(direction.normalized) * offset.y;
                        direction += offsetX + offsetY;
                    }
                }



                // 吸引效果，保留吸引速度，但是使其不越过吸引中心
                if (backDist < 0)
                {
                    if (Data.AttractDirectType == AttractDirectType.TargetPosition)
                    {
                        direction = target.Position - (UVector2)TargetPosition;
                    }
                    else if (Data.AttractDirectType == AttractDirectType.SourceDirection)
                    {
                        direction = Source.Orientation;
                    }
                    else
                    {
                        direction = target.Position - Caster.Position;
                    }
                    offsetX = direction.normalized * offset.x;
                    offsetY = MathUtil.GetRight(direction.normalized) * offset.y;
                    direction += offsetX + offsetY;
                    int collideRadius = Data.AttractDirectType == AttractDirectType.Caster ? Caster.Data.CollideRadius + target.Data.CollideRadius : 0;
                    float limitDist = UFloat.Round(collideRadius - direction.magnitude);
                    if (backDist < limitDist)
                    {
                        if (limitDist >= 0)
                        {
                            backTime = 0;
                            backDist = 0;
                        }
                        else
                        {
                            float scaleFactor = limitDist / backDist;
                            backTime = UFloat.Round(backTime * scaleFactor);
                            backDist = limitDist;
                        }
                    }
                }
                target.Repel(Data.RepelHeight, Data.RepelGravityFactor, backTime, direction.normalized * backDist, direction.normalized * acceleration, Data.RepelIgnoreObstacle, Data.CheckTrapObstacle, Data.RepelResistIgnorance);
            }

            //HandleMark(target);

            TriggerMark(target, Data.MarksToTrigger, MarkTriggerType.AfterInijury);
            if (!target.IsDead)
            {
                AddMark(target, Data.MarksToAdd, AddMarkType.Hit, effect);
            }
            AddMark(Caster, Data.MarksToSelf, AddMarkType.Hit, effect);
            if (!target.IsDead && Data.RemoveTargetMarkIDs != null && Data.RemoveTargetMarkIDs.Count > 1)
            {
                for (int i = 0; i < Data.RemoveTargetMarkIDs.Count; i += 2)
                {
                    int id = Data.RemoveTargetMarkIDs[i];
                    int count = Data.RemoveTargetMarkIDs[i + 1];
                    target.RemoveMarkByID(id, count);
                }
            }
            if (target.Joint != null)
            {
                IActorController controller = Caster.Joint != null ? Caster.Joint.Controller : null;
                target.Joint.AddEffect(Data.HitEffect, EffectType.Hit, new Vector3(0, 0, Data.HitOrder), controller);
                if (!(Data.AreaCenter != TargetType.None && Data.AreaShape != AreaShape.None) && !string.IsNullOrEmpty(Data.HitSound))
                    target.PlaySound(Data.HitSound);
            }
            return result;
        }

        private void HandleMark(BattleActor target)
        {
            int markID = 0;
            BattleMark mark = null;

            int count1 = Data.MarksToAdd == null ? 0 : Data.MarksToAdd.Count;
            int count2 = Data.MarksToTrigger == null ? 0 : Data.MarksToTrigger.Count;

            //给目标添加印记
            for (int i = 0; i < count1; i++)
            {
                markID = Data.MarksToAdd[i];
                mark = target.GetMarkByID(markID);

                if (mark == null)
                {
                    //目标身上尚无此种印记 创建并添加到目标身上
                    MarkData data = NeptuneBattle.Instance.DataProvider.GetMarkData(markID);
                    if (data != null)
                    {
                        mark = ObjectFactory.Create(data, target, Caster);
                        target.AddMark(markID, mark);
                    }
                    else
                    {
                        Debug.LogError("There's no data in BattleMark table, mard id:" + markID);
                    }
                }
                if (mark != null)
                {
                    mark.Deepen();
                }
            }

            //触发目标身上的印记
            for (int i = 0; i < count2; i++)
            {
                mark = target.GetMarkByID(Data.MarksToTrigger[i]);

                if (mark != null)
                {
                    mark.OuterTrigger(target.Position);
                }
            }
        }

        public void AddMark(BattleActor target, List<int> marks, AddMarkType addMarkType, BattleEffect effect = null)
        {
            int markID = 0;
            BattleMark mark = null;

            int count = marks == null ? 0 : marks.Count;
            int key = 0;
            //给目标添加印记
            for (int i = 0; i < count; i++)
            {
                markID = marks[i];
                if (effect != null)
                {
                    key = effect.EffectIndex;
                }
                //非子弹key为0 控制添加印记最大次数
                if (!checkAddMarkCount(key, markID))
                {
                    continue;
                }

                mark = target.GetMarkByID(markID);

                if (mark == null)
                {
                    //目标身上尚无此种印记 创建并添加到目标身上
                    MarkData data = NeptuneBattle.Instance.DataProvider.GetMarkData(markID);
                    if (data != null)
                    {
                        if (target == Caster && data.AddMarkType != addMarkType)
                        {
                            continue;
                        }
                        if (data.RoleType > 0 && (data.RoleType & target.Data.RoleType) <= 0)
                        {
                            continue;
                        }
                        mark = ObjectFactory.Create(data, target, Caster);
                        target.AddMark(markID, mark);
                    }
                    else
                    {
                        Debug.LogError("There's no data in BattleMark table, mard id:" + markID);
                    }
                }
                if (mark != null)
                {
                    if (target == Caster && mark.Data.AddMarkType != addMarkType)
                    {
                        continue;
                    }
                    mark.Deepen();
                }
            }
        }

        private void TriggerMark(BattleActor target, List<int> marks, MarkTriggerType markTriggerType)
        {
            BattleMark mark = null;

            int count = marks == null ? 0 : marks.Count;

            //触发目标身上的印记
            for (int i = 0; i < count; i++)
            {
                mark = target.GetMarkByID(marks[i]);

                if (mark != null && mark.Data.MarkTriggerType == markTriggerType)
                {
                    mark.OuterTrigger(TargetPosition);
                }
            }
        }

        private bool TargetDodge(BattleActor target, InjuryType injuryType)
        {
            bool dodge = NeptuneBattle.Instance.Numeric.TalentDodge(this, target, injuryType);
            if (dodge)
            {
                target.OnDodge(this);
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("{0}: {1} --> {2} (MISS)", Caster.FullName, FullName(), target.FullName);
#endif
                if (NeptuneBattle.Instance.Scene != null && target.Joint != null)
                {
                    target.Joint.OnDodge();
                    NeptuneBattle.Instance.Scene.PopupText(PopupType.Dodge, string.Empty, 0, target.Joint, Caster.Joint, false, target.Side);
                }
            }
            return dodge;
        }

        public virtual int CalculateInjury(InjuryType injuryType, RoleAttribute attrType, BattleActor target, float force, BattleActor from, ref int critCount)
        {
            return target.CalculateInjury(injuryType, attrType, force, from, this, ref critCount);
        }

        public void OnHPChanged(float currentHP, float maxHP)
        {
            if ((Data.TriggerType & TriggerType.HPLostPerPct) > 0)
            {//生命减少每百分比检测
                if (currentHP > KeepHp)
                    KeepHp = currentHP;

                if (KeepHp - currentHP >= maxHP * Data.TriggerParam * 0.01f)
                {
                    if (!Caster.AbilityEffects.MindChain)
                    {
                        OnHitFrame();
                    }
                    KeepHp = currentHP;
                }
            }
        }

        public bool CreateTalentAbilitis(BattleActor target, AbilityTriggerType triggerType, TalentHitType hitType = TalentHitType.TalentHit)
        {
            bool abilityMiss = false;
            int abilityCount = Data.Abilities.Count;
            Ability addAbility = null;
            for (int i = 0; i < abilityCount; i++)
            {
                AbilityData ability = Data.Abilities[i];
                addAbility = null;
                if (ability.TriggerType != triggerType)
                {
                    continue;
                }
                if (ability.TargetType == TargetType.Target)
                {//目标型Ability
                    bool resist = false;
                    bool bSucess = true;
                    if (target == null || target.IsDead && !ability.CanAddOnDeath)
                        bSucess = false;
                    else
                    {

                        if (ability.TriggerType == AbilityTriggerType.Hit)
                        {
                            if ((int)ability.TargetSide * (int)Caster.GetRelation(target) < 0)
                            {
                                continue;
                            }

                            if (CheckRoleType(target, ability.IgnoreTarget, false))
                            {
                                continue;
                            }

                            if (bSucess && !ability.Irresistible)
                            {
                                float probability = 0.3f;
                                if (ability.Probability > 0)
                                    probability = 1 - ability.Probability;
                                if (Util.Random.Rand() >= probability)
                                {
                                    float factor = ability.LevelFactor;
                                    if (factor != 0)
                                    {
                                        float talentLevel = Level;
                                        if (talentLevel < 30)
                                        {
                                            talentLevel = 10 + talentLevel / 30f * 20f;
                                        }
                                        factor = (float)(factor * Util.Random.Rand());
                                        if (target.Level > talentLevel + factor)
                                        {
                                            bSucess = false;
                                        }
                                    }
                                }
                                else
                                {
                                    bSucess = false;
                                }

                                if (bSucess)
                                {
                                    //if (target.Attributes != null)
                                    {
                                        RoleAttribute attribName = ability.ResistAttribute;
                                        if (attribName != RoleAttribute.None)
                                        {
                                            float rand = (float)Util.Random.Rand();
                                            if (rand < (float)target.Attributes[(int)attribName] / 100)
                                            {
                                                resist = true;
                                                bSucess = false;
                                            }
                                        }
                                    }
                                }
                                if (bSucess && !string.IsNullOrEmpty(ability.TargetHeroTag))
                                {
                                    if (target.Data.HeroTags.IndexOf(ability.TargetHeroTag) < 0)
                                    {
                                        bSucess = false;
                                    }
                                }
                                if (bSucess && target.AbilityEffects.MindGain && ability.ControlEffects != null)
                                {
                                    if (ability.ControlEffects.Contains((int)ControlEffect.MindChain))
                                    {
                                        bSucess = false;
                                    }
                                }
                            }
                        }
                    }

                    if (bSucess)
                    {
                        addAbility = target.AddAbility(ability, Caster, this);
                    }
                    else
                    {
                        abilityMiss = true;
                        if (NeptuneBattle.Instance.Scene != null && target != null && target.Joint != null)
                        {
                            NeptuneBattle.Instance.Scene.PopupText(resist ? PopupType.Resistance : PopupType.Miss, string.Empty, 0, target.Joint, Caster.Joint, false, target.Side);
                        }
                    }
                }
                else if (ability.TargetType == TargetType.Self)
                {//自身Ability
                    addAbility = Caster.AddAbility(ability, Caster, this);
                }
                //子弹飞行距离对buff时间的影响
                if (addAbility != null && Data.EffectRelatedAbilityID == addAbility.AbilityData.ID && ((hitType & TalentHitType.EffectHit) > 0 || (hitType & TalentHitType.EffectEnd) > 0))
                {
                    float extendTime = Mathf.Min(UFloat.Round(castFlyTime * Data.EffectExtendAbilityTimeRatio), Data.MaxEffectExtendAbilityTime);
                    addAbility.Duration = UFloat.Round(addAbility.Duration + extendTime);
                }
            }
            return abilityMiss;
        }

        public void PlayExtraEffects(EffectPlayType type, Vector2 position)
        {
            if (NeptuneBattle.Instance.Scene == null) return;
            if (Caster.Joint != null && Caster.Joint.Controller != null)
            {
                if (!Caster.Joint.Controller.InSight)
                    return;
            }
            if (Data.ExtraEffects != null && Data.ExtraEffects.Count > 0)
            {

                foreach (EffectData eff in Data.ExtraEffects)
                {
                    if (eff.Type == type)
                    {
                        Vector2 pos = EngineConst.Vector2Zero;

                        if (eff.RefPos == EffectPosRefType.Caster)
                        {
                            pos = Caster.Position;
                        }
                        if (eff.RefPos == EffectPosRefType.Target)
                        {
                            pos = Target.Position;
                        }

                        if (eff.RefPos == EffectPosRefType.Default)
                        {
                            pos = position;
                        }
                        pos = pos + new Vector2(eff.X, eff.Y);


                        Vector2 scale = EngineConst.Vector2Zero;
                        if (eff.RefDir != EffectDirRefType.None)
                        {
                            BattleActor effObj = eff.RefDir == EffectDirRefType.Caster ? Caster : Target;
                            scale = EngineConst.Vector2Zero;
                        }

                        NeptuneBattle.PlayEffect(eff.Name, pos, Caster.Orientation, 0, eff.Z, null, null, Caster);
                    }
                }
            }
        }

        public string FullName()
        {
            return "[" + Data.ID + "]" + (!string.IsNullOrEmpty(GroupData.DisplayName) ? GroupData.DisplayName : Data.TalentName) + "[" + Level + "]";
        }

        ///
        protected void RandomHitTarget(int indexNum)
        {
            List<BattleActor> survivorsList = NeptuneBattle.Instance.GetSurvivors(Caster, AffectedSide).Cast<BattleActor>().ToList();
            int tabNum = survivorsList.Count;
            Dictionary<int, bool> bDictionary = new Dictionary<int, bool>(tabNum);
            List<int> hitList = new List<int>();
            for (int t = 1; t <= tabNum; t++)
            {
                bDictionary[t] = false;
            }
            if (tabNum <= indexNum)
                for (int i = 1; i <= tabNum; i++)
                {
                    hitList.Add(i);
                }
            else
                for (int i = 1; i <= indexNum; i++)
                {
                    int ri = (int)Math.Ceiling((float)Util.Random.Rand() * (tabNum + 1 - i));
                    for (int j = 1; j <= tabNum; j++)
                    {
                        if (!bDictionary[j])
                        {
                            ri = ri - 1;
                            if (ri == 0)
                            {
                                hitList.Add(j);
                                bDictionary[j] = true;
                                break;
                            }
                        }
                    }
                }
            foreach (int v in hitList)
            {
                HitTarget(survivorsList[v - 1]);
            }
        }

        protected void CreateSummon(RoleInfo heroData, RoleConfig config, UVector2 pos)
        {
            BattleActor summon = ObjectFactory.Create(heroData, Caster.Side, config, null, pos, Caster.Orientation);
            summon.PlayEffectOnDeath = true;
            summon.Orientation = Caster.Orientation;
            NeptuneBattle.Instance.AddActor(summon, Caster, pos, RoleStateName.Instance[RoleState.Birth]);
        }




        public virtual void OnDestroyed()
        {
        }

        protected BattleActor ExTalentRoleCreate(RoleInfo heroData, RoleConfig config, int tid, int level, float hpFactor = -1)
        {
            if (heroData == null)
            {
                heroData = new RoleInfo();
            }
            config.IsMonster = true;
            config.PredictQuality = true;
            config.HPFactor = hpFactor < 0 ? Caster.Config.HPFactor : hpFactor;
            heroData.tid = tid;
            heroData.level = level;
            return ObjectFactory.Create(heroData, Caster.Side, config, null, UVector3.zero, Caster.Orientation);
        }

        /// <summary>
        /// 技能释放保护 （技能释放保护时间内玩家不能主动打断技能）
        /// 技能保护时间： 1.当配置表内有配置保护时间：技能开始释放到配置时间的时间段内为保护时间
        ///                2.配置表内未配置保护时间：技能开始释放到第一个攻击帧的时间段内为保护时间
        /// </summary>
        /// <returns></returns>
        public virtual bool IsCastingProtect()
        {

            //1.当配置表内有配置保护时间
            if (Data.ProtectTime > 0)
            {
                if (CastDuration > Data.ProtectTime)
                {
                    return false;
                }
            }
            else if (!hasTalentCasting)
            {
                return false;
            }
            return true;
        }

        public virtual bool IsCastingProtectEx()
        {
            if (Data.SkillProtectTime > 0)
            {
                if (CastDuration <= Data.SkillProtectTime)
                {
                    return true;
                }
            }
            return false;
        }
        public bool CheckCastingProtectEx()
        {
            return Data.SkillProtectTime > 0 && !IsCastingProtectEx();
        }

        public bool CheckInterruptType(BattleSkill talent)
        {
            if (talent == null)
                return false;
            if (Data.InterruptSkillType == InterruptSkillType.ALL)
            {
                return true;
            }
            if (Data.InterruptSkillType == InterruptSkillType.Normal && talent.Data.SkillType == SkillType.Normal)
            {
                return true;
            }
            if (Data.InterruptSkillType == InterruptSkillType.Skill && (talent.Data.SkillType == SkillType.Skill || talent.Data.SkillType == SkillType.ChildSkill))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 技能对特定类型单位的额外伤害系数
        /// </summary>
        /// <param name="roleType"></param>
        /// <returns></returns>
        public int GetExtraDamageRatio(RoleType roleType)
        {
            if (Data.ExtraDamageRatioByRoleType == null)
            {
                return 0;
            }
            if (Data.ExtraDamageRatioByRoleType.Count % 2 > 0)
            {
                Logger.LogError("this.Data.ExtraDamageRatioByRoleType.Count " + Data.ExtraDamageRatioByRoleType.Count);
                return 0;
            }
            List<int> list = Data.ExtraDamageRatioByRoleType;
            for (int i = 0; i < list.Count; i += 2)
            {
                if ((roleType & (RoleType)list[i]) == (RoleType)list[i])
                {
                    return list[i + 1];
                }
            }
            return 0;
        }

        private void CalculateTargetExtraInjury(BattleActor fromRole, BattleActor targetRole, BattleSkill fromTalent, List<TargetExtraDamageData> extraDamageData, ref int critCount)
        {
            if (extraDamageData == null || extraDamageData.Count <= 0 || fromRole == null || targetRole == null)
            {
                // No extra damage configured
                return;
            }

            for (int i = 0; i < extraDamageData.Count; i++)
            {
                TargetExtraDamageData damageData = extraDamageData[i];
                if ((damageData.targetType & targetRole.Data.RoleType) <= 0)
                {
                    continue;
                }

                RoleAttribute fromAttribute = damageData.fromRoleAttr;
                InjuryType toInjuryType = damageData.toInjuryType;
                int transRatio = damageData.transRatio;

                float extraDamage = UFloat.Round(fromRole.Attributes[(int)fromAttribute] * transRatio * EngineConst.Hundredth);

                targetRole.CalculateInjury(toInjuryType, RoleAttribute.MaxHP, extraDamage, fromRole, fromTalent, ref critCount);
            }

        }

        /// <summary>
        /// 激活类技能启动
        /// </summary>
        private void StartActivate()
        {
            IsActivated = true;
        }
        /// <summary>
        /// 激活类技能结束
        /// </summary>
        public void EndActivate()
        {
            if (Data.ContinuousType == ContinuousType.Activate && IsActivated)
            {
                ClearCastingTrap(Caster.IsDead);
                IsActivated = false;
                CastDuration = 0;
            }
        }


        public override string ToString()
        {
            return FullName() + string.Format(" CD:{0:f6} CastCD:{1:f6}", Duration, CastDuration);
        }
        public virtual bool CanInterrupt(InterruptType interruptType)
        {
            return (Data.InterruptType & interruptType) > 0;
        }

        protected virtual void CostMp(CostMPMode costmode)
        {
            if (Data.CostMPMode != costmode)
                return;
            Caster.SetCostValue(Caster.GetCurrentCostValue() - UFloat.Round(Data.CostMP * (1 - Caster.Attributes.ManaCostReduction / 100)));

        }
        /// <summary>
        /// AOE类型的技能附加范围检测
        /// </summary>
        /// <param name="target"></param>
        /// <param name="hitpos"></param>
        /// <param name="orientation"></param>
        /// <param name="arg"></param>
        /// <returns></returns>

        protected virtual bool HitCheckAffiliatedAreaShape(BattleActor target, UVector2 hitpos, UVector2 orientation, UVector2 arg)
        {
            if (target == null || Data.AffiliatedAreaShape == AreaShape.None)
                return false;
            UVector2 pos = hitpos + Data.AffiliatedAreaShift * orientation;
            UVector2 distance = target.Position - pos;
            return EngineUtil.HitTest(distance, Data.AffiliatedAreaShape, new UVector2(arg.x + Data.AffiliatedAreaArg1, arg.y + Data.AffiliatedAreaArg2),
                        orientation);

        }
        public int GethitNum()
        {
            return hitTargetNum;
        }

        public bool CanBreak(BattleSkill castTalent)
        {
            return castTalent.Data.ForcedInterrupt || CanInterrupt(InterruptType.Active) && CheckInterruptType(castTalent) || CheckCastingProtectEx();
        }

        public void AddMarkCount(int key)
        {
            if (Data.MaxHitAddMarkCount != null && Data.MaxHitAddMarkCount.Count > 0)
            {
                if (addMarkHitCount == null)
                {
                    addMarkHitCount = new Dictionary<int, Dictionary<int, int>>();
                }
                for (int i = 0; i < Data.MaxHitAddMarkCount.Count; i += 2)
                {
                    int markid = Data.MaxHitAddMarkCount[i];
                    int count = Data.MaxHitAddMarkCount[i + 1];
                    if (!addMarkHitCount.ContainsKey(key))
                    {
                        addMarkHitCount.Add(key, new Dictionary<int, int>());
                        if (!addMarkHitCount[key].ContainsKey(markid))
                            addMarkHitCount[key].Add(markid, count);
                    }
                }



            }

        }

        private bool checkAddMarkCount(int key, int markid)
        {
            if (addMarkHitCount == null || !addMarkHitCount.ContainsKey(key) || !addMarkHitCount[key].ContainsKey(markid))
            {
                return true;
            }
            if (addMarkHitCount[key][markid] > 0)
            {
                addMarkHitCount[key][markid]--;
                return true;
            }
            return false;
        }

        public void SetDamageFactor(float dmgFactorPercent)
        {
            damageFactor = Math.Max(0, UFloat.Round(damageFactor + dmgFactorPercent));
        }
    }
}