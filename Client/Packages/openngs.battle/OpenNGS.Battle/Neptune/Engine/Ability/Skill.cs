using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Neptune.Datas;


namespace Neptune
{
    /// <summary>
    /// Skill 技能类
    /// </summary>
    public class Skill : AbilityBase
    {
        public int instanceId;

        //public Actor Caster;
        public UVector3 CastPosition;
        public UVector3 TargetPosition;
        public Entity AttackPointElement;
        public Vector2 Orientation;
        public Vector2 Direction;
        public Actor Owner;
        public Vector2 LastDashDir = Vector2.zero;
        //protected static float defaultEvtX;
        //protected static float defaultEvtY;

        public SkillData Data;
        public SkillGroupData GroupData;
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
        public SkillAction CurrentAction;
        public List<SkillAction> Actions;
        public int NextActID = 0;
        public AniEventData NextAct = null;

        //int talentTick = -1;
        public bool IsEnabled = true;

        // 正在释放的陷阱
        protected List<Trap> castingTrapList = new List<Trap>();

        private bool hasSkillCasting;

        public bool IsActivated = false;

        public bool ToBeCurrentSkill = true;
        public Dictionary<TargetType, ITargetSelector> TargetSelectors;

        public ITargetSelector TargetSelector
        {
            get { return this.TargetSelectors.ContainsKey(this.Data.TargetType) ? this.TargetSelectors[this.Data.TargetType] : null; }
        }

        public RelativeSide AffectedSide
        {
            get { return (RelativeSide)((int)this.Caster.StandSide * (int)this.Data.AffectedSide); }
        }

        public RelativeSide TargetSide
        {
            get { return (RelativeSide)((int)this.Caster.StandSide * (int)this.Data.TargetSide); }
        }

        public Dictionary<Actor, int> targetContinuousEffects = new Dictionary<Actor, int>();

        protected int hitTargetNum;
        public int RebirthNum;
        //记录被攻击次数
        private Dictionary<Actor, int> dividedHitTarget = null;

        private Dictionary<int, Dictionary<int, int>> addMarkHitCount = null;
        //技能释放时间
        public float CastDuration = 0f;
        //攻击间隔时间
        private float ContinuousInterval = 0f;
        private int ContinueNums = 0;
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
                if (this.Data.NoSpeeder && !this.Data.NoCDReduct)
                {
                    return UFloat.Round((this.Data.CD) * NeptuneConst.Milli *
                           (1 - Math.Max(Math.Min(this.Caster.AttributeFinalValue.CDReduction, 40), 0) * NeptuneConst.Hundredth));
                }
                else
                {
                    return UFloat.Round((this.Data.CD) * NeptuneConst.Milli);
                }
            }
        }
        //public TArray<>
        public Skill()
        {

        }


        public virtual void Create(SkillGroupData group, SkillData data, Actor caster, int level)
        {
            base.Create();

            this.instanceId = 0;

            this.AttackPointElement = null;
            this.Owner = null;

            this.onStartAct = null;
            this.onEndAct = null;
            this.cancelAIEventsIfInterrupted = false;
            this.CurrentAction = null;
            this.IsActivated = false;

            this.targetContinuousEffects.Clear();
            if (this.addMarkHitCount != null)
                this.addMarkHitCount.Clear();
            this.dividedHitTarget = null;
            this.CastPosition = UVector3.zero;
            this.castFlyTime = 0f;
            this.CastingEffectIndex = 0;
            this.GroupData = group;
            this.Data = data;
            this.Caster = caster;
            this.Caster.RoleSkin.RoleSkinSkillReplace(this.Data);
            this.Level = level;
            this.Actions = new List<SkillAction>();
            this.MinRange = this.Data.MinRange;
            this.MinRangeSqr = this.MinRange * this.MinRange;
            this.MaxTargetNum = this.Data.MaxTargetNum;
            this.InitCD();
            this.Casting = false;
            this.CastDuration = 0;
            this.hasSkillCasting = false;
            this.Direction = Vector2.zero;
            this.LastDashDir = Vector2.zero;
            this.Target = null;

            this.CurrentActionID = 0;
            this.CurrentActionElapsed = 0;
            this.NextActID = 0;
            this.NextAct = null;
            this.HitTimes = 0;
            this.IsEnabled = true;
            this.damageFactor = 1.0f;
            this.obstacleWallHit = UVector2.zero;

            if (this.Data.MaxRange == 0)
            {
                this.MaxRange = NeptuneConst.MaxRangeValue;
            }
            else
            {
                this.MaxRange = Math.Min(NeptuneConst.MaxRangeValue, this.Data.MaxRange);
            }
            this.MaxRangeSqr = this.MaxRange * this.MaxRange;
            /*if (this.Data.SightRange == 0)
            {
                this.SightRange = this.Data.MaxRange + 500;
            }
            else*/
            {
                this.SightRange = Math.Min(NeptuneConst.MaxRangeValue, this.Data.SightRange);
                this.SightRangeSqr = this.SightRange * this.SightRange;
            }

            this.InitActions(this.Caster.RoleSkin.GetModelKey(this.Caster.Data.Model));
            if (this.Data.TargetType == TargetType.Self)
                this.Data.TargetSide = RelativeSide.Both;
            else if (this.Data.TargetType == TargetType.Target)
                this.Data.TargetSide = RelativeSide.Enemy;
            this.TargetSelectors = new Dictionary<TargetType, ITargetSelector>()
        {
            {TargetType.Random, new RandomSelector()},
            {TargetType.Weakest, new WeakestSelector()},
            {TargetType.MaxHP, new MaxHPSelector()},
            {TargetType.MinHP, new MinHPSelector()},
            {TargetType.Nearest, new NearestSelector(this.Caster)},
            {TargetType.Farthest, new FarthestSelector(this.Caster)},
            {TargetType.MaxMP, new MaxMPSelector()},
            {TargetType.MinMP, new MinMPSelector()},
            {TargetType.MaxIntelligence, new MaxIntelligenceSelector()},
            {TargetType.MaxAttackDamage, new MaxADSelector()},
            {TargetType.DeadBody, new RandomSelector()}
        };
            isCanContinuous = DetectionContinuous();

            this.castingTrapList.Clear();
        }

        public override void Delete()
        {
            ObjectPool<Skill>.Delete(this);
        }

        public override void OnDelete()
        {

        }


        public bool DetectionContinuous()
        {
            if (this.Data.ContinuousType == ContinuousType.Continuous)
            {
                // 2024-06-01 ray 先取消非瞬发技能的连续释放判断
                /*if (!this.Data.Instant && this.Actions != null)
                {
                    for (int i = 0; i < this.Actions.Count; i++)
                    {
                        SkillAction talentAction = this.Actions[i];
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
                }*/
                return true;
            }
            return false;
        }

        public virtual void LevelUp(int level)
        {
            if (this.Level < level)
                this.Level = level;
            if (this.Level == level && this.Level >= 1)
            {
                SkillData talentData = this.Caster.GetSkillData(this.GroupData, this.Level, this.Caster.ID);
                if (talentData != null)
                {
                    this.Data = talentData;
                    this.Caster.RoleSkin.RoleSkinSkillReplace(this.Data);
                    //                this.Data.Abilities = new List<AbilityData>();
                    //                 if (this.Data.AbilityIDs != null && this.Data.AbilityIDs.Count > 0)
                    //                 {
                    //                     foreach (int abilityID in this.Data.AbilityIDs)
                    //                     {
                    //                         AbilityData ability_info = EngineDataManager.Instance.Abilities.Value[abilityID].Clone();
                    //                         this.Data.Abilities.Add(ability_info);
                    //                     }
                    //                 }


                    this.MinRange = this.Data.MinRange;
                    this.MinRangeSqr = this.MinRange * this.MinRange;
                    this.MaxTargetNum = this.Data.MaxTargetNum;

                    if (this.Data.TargetType == TargetType.Self)
                        this.Data.TargetSide = RelativeSide.Both;
                    else if (this.Data.TargetType == TargetType.Target)
                        this.Data.TargetSide = RelativeSide.Enemy;

                    this.MaxRange = this.Data.MaxRange == 0 ? NeptuneConst.MaxRangeValue : Math.Min(NeptuneConst.MaxRangeValue, this.Data.MaxRange);
                    this.SightRange = Math.Min(NeptuneConst.MaxRangeValue, this.Data.SightRange);// == 0 ? this.MaxRange + 500 : this.Data.SightRange;
                    this.MaxRangeSqr = this.MaxRange * this.MaxRange;
                    this.SightRangeSqr = this.SightRange * this.SightRange;
                }
            }


            foreach (Actor role in this.Caster.SummonList)
            {
                if (role != null && !role.IsDead)
                {
                    for (int i = 0; i < role.tActiveSkills.Length; i++)
                    {
                        Skill talent = role.tActiveSkills[i] as Skill;
                        if (talent == null) continue;
                        if (talent.GroupData.ParentID == this.Data.SkillGroupID)
                        {
                            talent.LevelUp(talent.Level + 1);
                        }
                    }
                }
            }


            for (int i = 0; i < this.Caster.tActiveSkills.Length; i++)
            {
                Skill talent = this.Caster.tActiveSkills[i] as Skill;
                if (talent == null) continue;
                if (this.Data.SkillGroupID > 0 && talent.GroupData.ParentID == this.Data.SkillGroupID)
                {
                    talent.LevelUp(talent.Level + 1);
                }
                if (!talent.IsEnabled) continue;
                if ((talent.Data.TriggerType & TriggerType.SkillLevel) > 0 && (talent.Data.TriggerParam1 == 0 || talent.Data.TriggerParam1 == this.Data.SkillGroupID))
                {
                    if (talent.CanUse(this.Caster) == ResultType.Success)
                    {
                        talent.Start(this.Caster);
                    }
                }
            }
            for (int i = 0; i < this.Caster.PassiveSkills.Length; i++)
            {
                PassiveSkill talent = this.Caster.PassiveSkills[i];
                if (talent == null) continue;
                if (this.Data.SkillGroupID > 0 && talent.GroupData.ParentID == this.Data.SkillGroupID)
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
            ResetModifyData();
            this.Target = null;
            this.HitTimes = 0;
            if (isInitCD)
                InitCD();
            this.Casting = false;
            this.hasSkillCasting = false;
            this.CurrentActionElapsed = 0;
            this.CurrentActionID = 0;
            this.NextActID = 0;
            this.CurrentAction = null;
            this.NextAct = null;
        }

        public void InitCD()
        {
            this.Duration = UFloat.Round(this.Data.InitCD / NeptuneConst.Thousand);
        }

        void ResetCD(SkillStatus status)
        {
            if (this.Data.CDMode == status || this.Data.CDMode == SkillStatus.None && status == SkillStatus.Start)
            {
                this.Duration = this.CD;
            }
        }
        /// <summary>
        /// Start skill to target
        /// </summary>
        /// <param name="target"></param>
        public virtual void Start(Actor target)
        {
            Start(target, UVector3.zero);
        }

        public void Start(Actor target, UVector3 position)
        {
            //技能开始，初始化数据
            //播放启动特效
            //挂载Ability
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} began to cast {1} --> {2}", this.Caster.FullName, this.FullName(), target != null ? target.FullName : "nil");
#endif
            this.CastDuration = 0f;
            this.Casting = true;
            this.hasSkillCasting = true;
            this.Target = target;
            this.hitTargetNum = 0;
            this.RebirthNum = 0;
            this.TargetPosition = position;
            this.Orientation = NeptuneConst.Vector2Zero;
            this.ContinuousInterval = 0;
            this.ContinueNums = 0;



            if (this.FindTarget(target) == null)
            {
                this.End();
                return;
            }

            this.Caster.CommonCooldown = UFloat.RoundToInt(this.Data.GCD * NeptuneConst.Milli);
            this.ContinuousElapsed = 0;

            this.HitTimes = 0;
            this.IsEnabled = true;
            this.ResetCD(SkillStatus.Start);
            //消耗魔法值
            this.CostMp(CostMPMode.Start);
            if (this.Data.Instant)
            {//瞬发技能，立即生效
                this.StartAction(0, false);
                this.OnHitFrame();
            }
            else
            {

                if (!this.Data.MovingCast)
                {
                    this.Caster.moveDirLock = true;
                }

                if (this.Caster != Target && !this.Caster.IsRoleType(RoleType.Building) && Target.Position != this.Caster.Position)
                {
                    this.Caster.Orientation = (Target.Position - this.Caster.Position).normalized;
                    //Debug.Log("Direction[" + this.Caster + "] : set by SkillStart" + this.Caster.Orientation);
                }
                this.StartAction(0, true);
            }

            if (this.Caster.Joint != null)
            {
                if (this.Caster.IsHero && (this.Data.TriggerType == TriggerType.Manual || this.Data.TriggerType == TriggerType.Auto))
                {
                    //if (this.Caster != null && this.Caster.Player != null && this.Caster.Player.isMainPlayer) CachedLog.Log("time:" + Time.realtimeSinceStartup + ",frame:" + NeptuneBattle.Instance.doneFrameCount + ",skill start.dir:" + this.Caster.Orientation.ToString());
                    this.Caster.Joint.SetDirection(this.Caster.Orientation);
                }
                bool createEffect = true;
                if (this.Caster.Joint.Controller != null)
                {
                    createEffect = ((IActorController)this.Caster.Joint.Controller).InSight;
                }
                if (createEffect)
                    CastingEffectIndex = this.Caster.Joint.AddEffect(this.Data.CastingEffect, EffectType.Cast, UVector3.zero);
            }
            PlayExtraEffects(EffectPlayType.OnCast, this.Caster.Position);



            if (this.Data.UnfreezeTarget)
            {
                if (this.Target != null)
                {
                    this.Target.ResumeActor();
                }
            }

            if (this.HasAbility(AbilityTriggerType.Start))
            {
                this.CreateSkillAbilitis(this.Target, AbilityTriggerType.Start);
            }
            //攻击时触发技能
            this.Caster.OnAttack(this);

            //给自己添加MARK
            AddMark(this.Caster, this.Data.MarksToSelf, AddMarkType.Start);
            if (this.Data.TrapIds != null && this.Data.TrapIds.Length > 0 && this.Target != null)
                CreateTrap(TrapTriggerType.Start, this.Target.Position, this.Target);
            if (this.Data.Instant && this.Data.ContinuousType != ContinuousType.Continuous)
            {//瞬发技能，立即结束
                this.End();
            }
            else if (this.GroupData.ParentID == 0 || (this.Actions != null && this.Actions.Count > 0 && this.Actions[0] != null && ToBeCurrentSkill))
            {
                this.Caster.CurrentSkill = this;
            }
            if (this.onStartAct != null)
                onStartAct();
            this.Caster.CallBackSkillStart(this);
        }

        /// <summary>
        /// Start skill at position
        /// </summary>
        /// <param name="position"></param>
        public virtual void Start(UVector3 position)
        {
            //定点释放的技能,定点必是AOE
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} began to cast {1} --> {2}", this.Caster.FullName, this.FullName(), position);
#endif
            Debug.Log(this.Data.SkillName + "被触发了");
            this.CastDuration = 0f;
            this.hasSkillCasting = true;
            this.Casting = true;
            this.hitTargetNum = 0;
            this.RebirthNum = 0;
            this.Target = null;
            this.TargetPosition = position;
            this.Caster.CommonCooldown = UFloat.RoundToInt(this.Data.GCD * NeptuneConst.Milli);
            this.ContinuousElapsed = 0;
            this.ContinuousInterval = 0;
            this.ContinueNums = 0;
            this.HitTimes = 0;
            this.IsEnabled = true;
            this.ResetCD(SkillStatus.Start);
            this.CostMp(CostMPMode.Start);
            if (this.Data.Instant)
            {//瞬发技能，立即生效
                this.StartAction(0, false);
                this.OnHitFrame();
            }
            else
            {
                if (!this.Data.MovingCast)
                {
                    this.Caster.moveDirLock = true;
                }
                //if (this.Caster.CurrentSkill != null)
                //{
                //    this.Caster.CurrentSkill.Break();
                //}
                //this.Caster.Orientation = ((Vector2)this.TargetPosition - this.Caster.Position).normalized;
                this.StartAction(0, true);
            }

            if (this.Caster.Joint != null)
            {
                if (this.Caster.IsHero && (this.Data.TriggerType == TriggerType.Manual || this.Data.TriggerType == TriggerType.Auto))
                {
                    //if (this.Caster != null && this.Caster.Player != null && this.Caster.Player.isMainPlayer) CachedLog.Log("time:" + Time.realtimeSinceStartup + ",frame:" + NeptuneBattle.Instance.doneFrameCount + ",skill start1.dir:" + this.Caster.Orientation.ToString());
                    this.Caster.Joint.SetDirection(this.Caster.Orientation);
                }
                CastingEffectIndex = this.Caster.Joint.AddEffect(this.Data.CastingEffect, EffectType.Normal, UVector3.zero);
            }
            PlayExtraEffects(EffectPlayType.OnCast, this.Caster.Position);

            if (this.Data.UnfreezeTarget)
            {
                if (this.Target != null)
                {
                    this.Target.ResumeActor();
                }
            }

            if (this.HasAbility(AbilityTriggerType.Start))
            {
                this.CreateSkillAbilitis(this.Target, AbilityTriggerType.Start);
            }
            //攻击时触发技能
            this.Caster.OnAttack(this);

            //给自己添加MARK
            AddMark(this.Caster, this.Data.MarksToSelf, AddMarkType.Start);
            if (this.Data.TrapIds != null && this.Data.TrapIds.Length > 0)
                CreateTrap(TrapTriggerType.Start, this.Caster.Position, this.Caster);
            if (this.Data.Instant)
            {//瞬发技能，立即结束
                this.End();
            }

            else if (this.GroupData.ParentID == 0 || (this.Actions != null && this.Actions.Count > 0 && this.Actions[0] != null && ToBeCurrentSkill))
            {
                this.Caster.CurrentSkill = this;
            }
            if (this.onStartAct != null)
                onStartAct();

            // 播放音效
            if (NeptuneBattle.Instance.Scene != null && !string.IsNullOrEmpty(Data.StartSound))
            {
                this.Caster.PlaySound(Data.StartSound);
            }
            // 播放VO
            if (NeptuneBattle.Instance.Scene != null && Data.PlayVoice != null && this.Caster.Player != null && this.Caster.Player.isMainPlayer)
            {
                this.Caster.PlayRandomVoice(Data.PlayVoice, true);
            }
            this.Caster.CallBackSkillStart(this);
        }

        public virtual void OnEnterFrame(float dtAction, float dtCd)
        {
#if BATTLE_LOG
            Actor lastTarget = this.Target;
#endif
            if (!Caster.IsDead && this.Casting && this == this.Caster.CurrentSkill)
            {
                if (this.HitTimes == 0 && this.Data.TargetType == TargetType.Target &&
                    ((this.Target == null || (this.Target.IsDead && !this.Data.DontBreakWhenTargetDead)) && this.TargetPosition == UVector3.zero))
                {
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("  {0} Skill OnEnterFrame {1} {2} {3}", this.Caster.FullName, this.FullName(),
                            this.Target == null, this.Target != null ? this.Target.IsDead.ToString() : "Null");
#endif
                    this.Break();
                }
                else
                {
                    if(Data.Instant == false && NextAct != null)
                    {
                        EventHandler(dtAction);
                    }
                    else if (this.Data.ContinuousType == ContinuousType.Continuous)
                    {
                        if (this.HitTimes > 0)
                            this.ContinuousElapsed += dtCd;

                        if ((this.Data.ContinuousTime > 0 && this.ContinuousElapsed >= this.Data.ContinuousTime)
                            || (this.Data.TargetType == TargetType.Target && (
                            this.Target == null
                                || (this.Target != null && (this.Target.IsDead || this.Caster.Distance(this.Target, NeptuneConst.EnableRadiusInDistance) > this.Data.MaxRange))
                            )
                            ))
                        {
                            this.End();
                        }
                        else if (isCanContinuous)
                        {
                            ContinuousInterval += dtCd;
                            if (ContinuousInterval >= this.Data.ContinuousInterval)
                            {
                                if(this.Data.ContinueNums > 0)
                                {
                                    ContinuousInterval -= this.Data.ContinuousInterval;
                                    this.ContinueNums++;
                                    this.OnHitFrame();
                                    if (this.ContinueNums == this.Data.ContinueNums)
                                        this.End();
                                }
                                else
                                {
                                    ContinuousInterval = 0;
                                    this.OnHitFrame();
                                    if (this.Data.ContinuousTime < this.Data.ContinuousInterval * (this.HitTimes + 1))
                                        this.End();
                                }
                            }
                        }
                    }
                    CastDuration = UFloat.Round(CastDuration + dtCd);
                }
            }

            UpdateCD(dtAction, dtCd);
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("   {0} PT:{1} CT:{2}", this.ToString(), lastTarget == null ? "null" : lastTarget.FullName, this.Target == null ? "null" : this.Target.FullName);
#endif
        }

        public virtual void UpdateCD(float dtAction, float dtCd)
        {
            if (this.Duration > 0)
            {
                this.Duration = UFloat.Round(this.Duration - dtCd);
                if (this.Duration <= 0)
                {
                    this.Caster.OnColdDown(this);
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
            float time = UFloat.Round(this.CurrentActionElapsed + dtAction);
            this.CurrentActionElapsed = time;
            AniEventData nextEvent = this.NextAct;
            while (nextEvent != null && time >= nextEvent.Time)
            {
                string type = "AttackSpot";
                if (!string.IsNullOrEmpty(nextEvent.Type))
                    type = nextEvent.Type;

#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("{0}: {1} OnHitFrame {2:f6} > {3:f6} {4:f6} {5}", this.Caster.FullName, this.FullName(), time, nextEvent.Time, dtAction, type);
#endif
                switch (type)
                {
                    case "attackspot":
                    case "AttackSpot":
                        {
                            this.ContinueNums++;
                            this.OnHitFrame();
                            if (this.ContinueNums == this.Data.ContinueNums)
                                this.End();
                        }
                        break;
                    case "dash":
                    case "Dash":
                        this.DoDash(nextEvent.X, (int)nextEvent.Y, (int)nextEvent.Z, (int)nextEvent.Param1, nextEvent.Param2 <= 0, (int)nextEvent.Param3, nextEvent.Param4 <= 0, nextEvent.Param5, nextEvent.Param6, (int)nextEvent.Param7, (int)nextEvent.Param8);
                        break;
                    case "teleport":
                    case "Teleport":
                        this.DoTeleport((TargetType)nextEvent.X, (int)nextEvent.Y);
                        break;
                    case "skill":
                    case "Skill":
                        int skillID = (int)nextEvent.X;
                        if (this.Caster.ChangeSkillStack.ContainsKey(skillID))
                        {
                            List<int> abilitys = this.Caster.ChangeSkillStack[skillID];
                            if (abilitys != null && abilitys.Count > 0)
                            {
                                Ability ability = this.Caster.FindAbilityByChangeIndex(abilitys.Max(), skillID);
                                if (ability != null)
                                    skillID = ability.ToSkillID;
                            }
                        }
                        this.DoSkill(skillID);
                        break;
                    case "invisibility":
                    case "Invisibility":
                        break;
                    case "end":
                    case "End":
                        this.End();
                        result = false;
                        break;
                }
                int idx = this.NextActID + 1;

                if (this.CurrentAction == null || !result)
                {
                    break;
                }
                nextEvent = idx < this.CurrentAction.Events.Count ? this.CurrentAction.Events[idx] : null;
                this.NextActID = idx;
                if (this.Data.ContinuousType != ContinuousType.Continuous)
                {
                    if (nextEvent == null && this.Data.ContinuousType != ContinuousType.Continuous)
                        this.NextAct = nextEvent;
                }
                else
                {
                    if(nextEvent == null)
                    {
                        this.NextActID = idx = 0;
                        this.NextAct = idx < this.CurrentAction.Events.Count ? this.CurrentAction.Events[idx] : null;
                        CurrentActionElapsed = 0;
                    }
                    else
                    {

                    }
                }
            }
#if BATTLE_LOG
            if (EngineGlobal.BattleLog && nextEvent == null)
                NeptuneBattle.log("{0}: {1} OnHitFrame NextEvent is null {2:f6}", this.Caster.FullName, this.FullName(), time);
#endif
            return result;
        }

        // 清除当前技能正在释放的陷阱
        public void ClearCastingTrap(bool casterDead = false)
        {
            if (castingTrapList.Count > 0)
            {
                foreach (Trap trap in castingTrapList.ToArray())
                {
                    if (trap.Interrupt(casterDead))
                    {
                        NeptuneBattle.Instance.RemoveTrap(trap);
                        castingTrapList.Remove(trap);
                    }
                }
            }
        }

        public virtual Actor FindTarget(Actor defaultTarget = null)
        {
            if (this.Data.TargetType == TargetType.Target || this.Caster.AbilityEffects.Value.Taunt)
            {

                if (defaultTarget != null || this.Data.FollowSourceSkillTarget)
                {
                    this.Target = defaultTarget;
                }
                else
                {
                    int dist = 0;
                    Actor target = this.Caster.FindTarget(ref dist, this, true, this.Direction.x, this.Direction.y);
                    if (dist <= this.MaxRange)
                    {
                        this.Target = target;
                    }
                    else
                    {
                        this.Target = null;//这里应该是空的
                    }
                }
            }
            else if (this.Data.TargetType == TargetType.Self)
            {
                this.Target = this.Caster;
            }
            else if (this.Data.TargetType == TargetType.Owner)
            {
                this.Target = this.Caster.Owner;
            }
            else if (this.Data.TargetType == TargetType.DeadBody)
            {
                this.Target = null;
                foreach (Actor role in NeptuneBattle.Instance.Roles.Where(Actor.PredicateDeadBody))
                {
                    Target = role;
                }
                return Target;
            }
            else if (this.TargetSelector != null)
            {
                float max = -float.MaxValue;
                Actor result = null;
                int sightrange = this.SightRange;
                if (sightrange <= 0)
                {
                    sightrange = this.MaxRange;
                }
                foreach (Actor role in NeptuneBattle.Instance.AOIManager.GetSurvivors(this.TargetPosition, this.Caster.Side, this.TargetSide, sightrange, this.Caster.Radius))
                {
                    if (this.Data.TargetSelectorWithOutSelf && role == this.Caster)
                        continue;

                    if (!CheckTargetClass(role))
                        continue;

                    if ((role != this.Caster && role.AbilityEffects.Value.Void) || (this.TargetSide != RelativeSide.Friend && this.Caster.NotSelectable(role)))
                        continue;

                    if (this.Caster.AbilityEffects.Value.Charm && role == this.Caster)
                        continue;

                    int distance = role.Distance(this.TargetPosition, 0, NeptuneConst.EnableRadiusInDistance);
                    if (distance >= this.MinRange && distance <= sightrange)
                    {
                        float v = this.TargetSelector.Select(role);
                        if (max < v)
                        {
                            max = v;
                            result = role;
                        }
                    }
                }
                this.Target = result;
            }
            else if (this.Data.TargetType == TargetType.Position)
            {

            }
            else if (this.Data.TargetType == TargetType.Direction)
            {

            }
            else if (this.Data.TargetType == TargetType.RandomPosition)
            {

            }
            else if (this.Data.TargetType == TargetType.TargetPosition)
            {
                if (defaultTarget != null)
                {
                    this.Target = defaultTarget;
                }
                else
                {
                    int dist = 0;
                    Actor target = this.Caster.FindTarget(ref dist, this, true, this.Direction.x, this.Direction.y);
                    if (dist <= this.MaxRange)
                    {
                        this.Target = target;
                    }
                    else
                    {
                        this.Target = null;//这里应该是空的
                    }
                }
            }
            else if (this.Data.TargetType == TargetType.All)
            {
                this.Target = defaultTarget;
            }
            else
                Logger.LogError(string.Format("target type error: {0}", this.Data.TargetType) + " | skill.id = " + Data.ID);

#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} FindTarget:{1} {2} {3}", this.Caster.FullName, this.FullName(), this.Target == null ? "null" : this.Target.FullName, this.Data.TargetType);
#endif

            return this.Target;
        }

        public virtual bool CheckTargetClass(Actor role)
        {
            if (Caster.AbilityEffects.Value.OnlyAttackBuilding)
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
            this.ResetCD(SkillStatus.End);
            this.CostMp(CostMPMode.End);
            if (this.Caster.IsActiveCasting)
            {
                NeptuneBattle.Instance.Resume();
                this.Caster.IsActiveCasting = false;
            }
            this.Casting = false;
            this.hasSkillCasting = false;
            this.Caster.OnSkillEnd(this);
            //瞬发技能不改变角色状态
            if (!this.Data.Instant)
            {
                if (this.GroupData.ParentID == 0 || this.GroupData.ChildGroupID == 2)
                {
                    this.Caster.CurrentSkill = null;
                }
                if (this.Data.MovingCast && this.Caster.MoveDirection != NeptuneConst.Vector2Zero && this.Caster.CanMove && this.Caster.DashTime <= 0 && this.Caster.RepelTime <= 0)
                {
                    if (this.Caster.RoleST != RoleState.Move)
                    {
                        this.Caster.RoleST = RoleState.Move;
                        this.Caster.Move();
                    }
                }
                else
                    this.Caster.Idle(isBreak);
            }

            // End Trap If any
            if (!this.IsActivated)
            {
                ClearCastingTrap(this.Caster.IsDead);
                CastDuration = 0;
            }
            if (this.Data.RemoveCastingEffect)
            {
                if (this.Caster.Joint != null)
                {
                    if (CastingEffectIndex > 0)
                        this.Caster.Joint.RemoveEffect(CastingEffectIndex);
                }
            }
            foreach (KeyValuePair<Actor, int> kv in this.targetContinuousEffects)
            {
                kv.Key.Joint.RemoveEffect(kv.Value);
            }
            targetContinuousEffects.Clear();
            this.Orientation = NeptuneConst.Vector2Zero;
            this.Direction = NeptuneConst.Vector2Zero;
            if (onEndAct != null)
                onEndAct();
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} Skill End {1} {2} {3}", this.Caster.FullName, this.FullName(), isBreak, this.Data.Instant);
#endif
        }

        public virtual void Break(Skill breakSkill = null)
        {
            float cdRate = 1;
            if (NeptuneConst.SkillInterruptCdRate >= 0)
            {
                if (this.HitTimes == 0)
                {//没有命中目标被打断时CD减半   每个项目根据需求在NeptuneConst中配置CD比率
                    cdRate = NeptuneConst.SkillInterruptCdRate;
                }
                this.Duration = UFloat.Round(cdRate * this.CD);
                this.Caster.CommonCooldown = UFloat.Round(cdRate * UFloat.RoundToInt(this.Data.GCD * NeptuneConst.Milli));
            }
            if (this.Caster.Joint != null)
            {
                if (!string.IsNullOrEmpty(Data.StartSound))
                {
                    this.Caster.Joint.StopSound();
                }
                //if (Data.PlayVoice != null)
                //{
                //    this.Caster.Joint.StopVoice();
                //}

            }
            if (this.Caster != null)
                this.Caster.CallbackByBreakSkill();
            this.End(true);
        }

        public virtual UVector3 AttackPoint(int index = 0)
        {
            AniEventData evt = this.NextAct ?? new AniEventData();
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
            if (this.Data.FlyStartOffset != null && this.Data.FlyStartOffset.Length != 0 && this.Data.FlyStartOffset.Length % 3 == 0)
            {
                if (index * 3 > this.Data.FlyStartOffset.Length - 1)
                {
                    index = 0;
                }
                bonePos = (bonePos + new UVector3(this.Data.FlyStartOffset[3 * index], this.Data.FlyStartOffset[3 * index + 1], this.Data.FlyStartOffset[3 * index + 2]));
            }
            if (element)
            {
                return bonePos;
            }

            UVector3 pos = this.Caster.GetAttackPoint();
            if(pos!= UVector3.zero)
            {
                return pos;
            }

            //Vector2 bonePos = new Vector2(evt.X, evt.Y);
            Vector2 scale = this.Caster.GetCurrentScale();
            UFloat.Round(ref scale);
            float roleScale = this.Caster.GetRoleScale();
#if GAME_2D
        UVector3 hitPos = new UVector3(this.Caster.Position.x + bonePos.x * scale.x * roleScale, this.Caster.Position.y, bonePos.y * scale.y * roleScale);

#else
            UVector3 localPos = new UVector3(bonePos.x * scale.x * roleScale, bonePos.y * scale.y * roleScale, bonePos.z * scale.y * roleScale);

            //algorithm1
            int eAngle = MathUtil.VectorToAngleInt(this.Caster.Orientation, Vector3.right);

            float angle = UFloat.Round(eAngle * Mathf.Deg2Rad);
            float sinAngle = UFloat.Round(Mathf.Sin(angle));
            float cosAngle = UFloat.Round(Mathf.Cos(angle));

            int newX = UFloat.RoundToInt(UFloat.Round(localPos.x * cosAngle - localPos.y * sinAngle));
            int newY = UFloat.RoundToInt(UFloat.Round(localPos.x * sinAngle + localPos.y * cosAngle));
            UVector3 hitPos = new UVector3(this.Caster.Position.x + newX, this.Caster.Position.y + newY, this.Caster.Height + localPos.z);
#endif

#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0}: {1} --> {2} exEffect AttackPoint ({3:f6},{4:f6},{5:f6}) Evt({6:f6},{7:f6}) {8:f6},{9:f6},{10:f6},{11:f6}", this.Caster.FullName, this.FullName(), this.Target != null ? this.Target.FullName : "", hitPos.x, hitPos.y, hitPos.z, bonePos.x, bonePos.y, roleScale, scale.x, scale.y, eAngle);
#endif
            return hitPos;
        }

        /*
         *******************************
         *  Skill Status Verify
         *******************************
         */

        public virtual ResultType CanUse(Actor target, bool checkTarget = true, bool ignoredInvisible = false)
        {
            ResultType result = ResultType.Success;
            if (this.Level <= 0)
            {
                result = ResultType.Failed; goto end;
            }
            if (this.IsActivated || (this.Duration > 0 && !(this.GroupData.Index == 1 && !checkTarget && this.Duration - 0.4f <= 0)))
            {
                result = ResultType.Cooldown; goto end;
            }

            if (this.Caster.AbilityEffects.Value.OnlyNormalAttack && !this.Data.NoSilence)
            {
                result = ResultType.OnlyNormalAttack; goto end;
            }

            //缴械
            if (this.Caster.AbilityEffects.Value.Disarmed && this.Data.SkillType == SkillType.Normal)
            {
                result = ResultType.Disarmed; goto end;
            }


            if (this.Data.CostMP > this.Caster.GetCurrentCostValue())
            {
                result = ResultType.MPNotEnough; goto end;
            }

            //不是驱散
            if (!this.Data.Unrestricted)
            {
                if ((this.Data.TriggerType & TriggerType.Manual) > 0 || (this.Data.TriggerType & TriggerType.Auto) > 0)
                {
                    if (this.Caster.AbilityEffects.Value.Sleep)
                    {
                        result = ResultType.Stun; goto end;
                    }

                    if (this.Caster.AbilityEffects.Value.Fear)
                    {
                        result = ResultType.Fear; goto end;
                    }

                    if (this != this.Caster.BasicSkill && this.Caster.AbilityEffects.Value.MindChain)
                    {
                        result = ResultType.MindChain; goto end;
                    }
                }
                if ((this.Data.TriggerType & TriggerType.Manual) > 0 && !this.Data.NoSilence)
                {
                    if (this.Data.DamageType == InjuryType.AttackDamage && this.Caster.AbilityEffects.Value.Disable)
                    {
                        result = ResultType.Disable; goto end;
                    }

                    if (this.Data.DamageType != InjuryType.AttackDamage && this.Caster.AbilityEffects.Value.Inhibition)
                    {
                        result = ResultType.Silence; goto end;
                    }
                }
            }



            if (checkTarget && this.Data.TargetType != TargetType.Direction && this.Data.TargetType != TargetType.Position
                && this.Data.TargetType != TargetType.RandomPosition)
            {//需要目标才是使用的技能
                target = this.FindTarget(target);//检测每个技能是否可释放的时候同时检测是否有可用目标    

                if (target == null || (this.Data.TargetClass > 0 && (target.Data.RoleType & this.Data.TargetClass) <= 0))
                {
                    result = ResultType.NoTarget; goto end;
                }

                if ((target != this.Caster && target.AbilityEffects.Value.Void) || (this.Caster.GetRelation(target.Side) != RelativeSide.Friend && !ignoredInvisible && this.Caster.NotSelectable(target)))
                {
                    result = ResultType.Untargetable; goto end;
                }

                if ((int)this.Caster.GetRelation(target) * (int)this.TargetSide < 0)
                {
                    result = ResultType.SideDiff; goto end;
                }


                #region 临时在这里增加关于海陆空的逻辑判断，将来如果需求持续扩展可考虑重构

                if (this.Data.TargetLSAMode == LandSeaAirMode.Air && !target.IsAirForce)
                {
                    result = ResultType.Untargetable; goto end;
                }

                if (this.Data.TargetLSAMode == LandSeaAirMode.Ground && target.IsAirForce)
                {
                    result = ResultType.Untargetable; goto end;
                }
                #endregion
                if (!this.Data.FollowSourceSkillTarget)
                {
                    int distance = this.Caster.Distance(target, NeptuneConst.EnableRadiusInDistance);
                    if (distance > this.SightRange && this.SightRange > 0)
                    {
                        result = ResultType.OutOfSightRange; goto end;
                    }
                    int range = this.MaxRange;
                    if (distance > range)
                    {
                        result = ResultType.TooFar; goto end;
                    }

                    if (distance < this.MinRange)
                    {
                        result = ResultType.TooNear; goto end;
                    }

#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("  {0} Skill.CanUse : {1} {2} ({3} - {4})", this.FullName(), result, distance, this.MinRange, this.MaxRange);
#endif
                }

            }
            if (this.Caster.TargetPos != UVector2.zero && (this.Data.TargetType == TargetType.Direction || this.Data.TargetType == TargetType.Position))
            {
                int distance = this.Caster.Distance(this.Caster.TargetPos, 0, NeptuneConst.EnableRadiusInDistance);
                int maxRange = this.Data.TargetType == TargetType.Position && this.Caster.IsHero && checkTarget ? this.MaxRange + NeptuneConst.AttackRange : this.MaxRange;
                if (distance > maxRange)
                {
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("  {0} Skill.CanUse : TooFar {1} < {2} {3}", this.FullName(), distance, maxRange, this.Caster.TargetPos);
#endif
                    result = ResultType.TooFar; goto end;
                }
            }

            if (!this.Data.OutOfScreen && this.Caster.IsOutOfBattleArea)
            {
                result = ResultType.OutOfScreen;
            }

        end:

#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("  {0} Skill.CanUse : {1}", this.FullName(), result);
#endif
            return result;
        }

        /// <summary>
        /// 检测剩余魔法是否够释放技能
        /// </summary>
        public virtual bool CheckMPIsEnough()
        {
            return this.Data.CostMP <= this.Caster.GetCurrentCostValue();
        }

        public virtual bool IsReady()
        {
            return true;
        }

        public virtual bool CanActive()
        {
            return false;
        }

        public bool CheckRoleType(Actor role, RoleType type, bool defaultValue)
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
        protected virtual bool CheckIgnoreTarget(Actor target)
        {
            //if(ModifyData == null)
            //{
            //    //NgDebug.LogError(string.Format("<color=yellow>cd skill [{0}] has not modify </color>", Data.ID));
            //    return true;
            //}
            //int nModifyMaxTargetNum = ModifyData.MaxTargetNum;
            //if (this.hitTargetNum > MaxTargetNum + nModifyMaxTargetNum && MaxTargetNum + nModifyMaxTargetNum > 0)
            //{//
            //    return true;
            //}
            if (this.hitTargetNum > MaxTargetNum&& MaxTargetNum > 0)
            {//
                return true;
            }
            if (this.Data.IgnoreTarget == RoleType.Any)
                return false;
            if (target.IsRoleType(this.Data.IgnoreTarget))
            {
                return true;
            }

            if (target.Data.MainAttrib.ToString() == this.Data.IgnoreTarget.ToString())
            {
                if (NeptuneBattle.Instance.Scene != null)
                {
                    NeptuneBattle.Instance.Scene.PopupText(PopupType.Void, string.Empty, 0, target.Joint, this.Caster.Joint, false, target.Side);
                }
                return true;
            }
            return false;
        }

        public bool HasAbility(AbilityTriggerType triggerType, AbilityTriggerMode abilityTriggerMode = AbilityTriggerMode.AfterInijury)
        {
            if (this.Data.Abilities == null || this.Data.Abilities.Count <= 0)
            {
                return false;
            }
            for (int i = 0; i < this.Data.Abilities.Count; i++)
            {
                if (this.Data.Abilities[i].TriggerType == triggerType && (triggerType == AbilityTriggerType.Start || this.Data.Abilities[i].AbilityTriggerMode == abilityTriggerMode))
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
            this.Actions.Clear();
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
            if (this.Data.Actions == null || this.Data.Actions.Count <= 0)
                return;
            for (int i = 0; i < this.Data.Actions.Count; i++)
            {
                actionName = this.Data.Actions[i];

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

                SkillAction act = new SkillAction();
                act.ActionName = actionName;
                this.Actions.Add(act);
                act.Events = events;
                if (resAniConfigData != null && resAniConfigData.ContainsKey(actionName))
                {
                    act.TotalTime = resAniConfigData[actionName].TotalTime;
                }
            }
        }

        public virtual void StartAction(int idx,bool invoke)
        {
            try
            {
                SkillAction action = null;
                if(idx < this.Actions.Count)
                {
                    action = this.Actions[idx];
                }
                if (action != null)
                {
                    this.CurrentActionID = idx;
                    this.CurrentAction = action;
                    this.CurrentActionElapsed = 0;
                    this.NextActID = 0;
                    this.NextAct = action.Events == null ? null : action.Events[0];
                    if (invoke)
                    {
                        bool bIsLoop = Data.ContinuousType == ContinuousType.Continuous;
                        this.Caster.SetAnimation(action.ActionName, bIsLoop, true);
                    }
                }

                // 播放音效
                if (NeptuneBattle.Instance.Scene != null && !string.IsNullOrEmpty(Data.StartSound))
                {
                    this.Caster.PlaySound(Data.StartSound);
                }
                // 播放VO
                if (NeptuneBattle.Instance.Scene != null && Data.PlayVoice != null && this.Caster.Player != null && this.Caster.Player.isMainPlayer)
                {
                    this.Caster.PlayRandomVoice(Data.PlayVoice, true);
                }

            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("Action Error!Skill[RoleID {0}   {1}:{2}]\n{3}", this.Caster.Data.ID, this.Data.SkillGroupID, this.Data.SkillName, ex);
            }
        }

        public virtual void OnAnimationEnd()
        {
            if (this.CurrentActionID >= this.Actions.Count - 1)
            {
                if (this.Data.ContinuousType == ContinuousType.Continuous && isCanContinuous && CastDuration < this.Data.ContinuousTime)
                {
                    return;
                }
                this.End();
            }
            else
                this.StartAction(CurrentActionID + 1, true);
        }

        /*
         *******************************
         *  Skill Effect Process
         *******************************
         */

        public virtual void OnHitFrame()
        {
            this.ResetCD(SkillStatus.HitFrame);
            //移除施法者身上的指定ID的buff
            //Caster.RemoveAbilitys(AbilityRemoveType.OnHit);
            this.CostMp(CostMPMode.HitFrame);
            //移除施法者身上的指定ID的buff
            if (Data.RemoveCasterAbilityIDs != null && Data.RemoveCasterAbilityIDs.Length > 1)
            {
                for (int i = 0; i < Data.RemoveCasterAbilityIDs.Length; i += 2)
                {
                    int id = Data.RemoveCasterAbilityIDs[i];
                    int count = Data.RemoveCasterAbilityIDs[i + 1];
                    Caster.RemoveAbility(id, count);
                }

                //foreach (int ability in Data.RemoveCasterAbilityIDs)

            }
            if (Data.RemoveCasterMarkIDs != null && Data.RemoveCasterMarkIDs.Length > 1)
            {
                for (int i = 0; i < Data.RemoveCasterMarkIDs.Length; i += 2)
                {
                    int id = Data.RemoveCasterMarkIDs[i];
                    int count = Data.RemoveCasterMarkIDs[i + 1];
                    Caster.RemoveMarkByID(id, count);
                }
            }
            if (Data.RemoveCasterTrapIDs != null && Data.RemoveCasterTrapIDs.Length > 1)
            {
                for (int i = 0; i < Data.RemoveCasterTrapIDs.Length; i += 2)
                {
                    int id = Data.RemoveCasterTrapIDs[i];
                    int count = Data.RemoveCasterTrapIDs[i + 1];
                    NeptuneBattle.Instance.RemoveTrap(this.Caster, id, count);
                }
            }

            if (this.Caster.IsActiveCasting)
            {
                NeptuneBattle.Instance.Resume();
                this.Caster.IsActiveCasting = false;
            }
            this.hasSkillCasting = false;
            this.HitTimes = this.HitTimes + 1;

            if (this.Data.ContinuousType == ContinuousType.Activate && this.HitTimes == 1)
            {
                this.StartActivate();
            }

            if (this.Data.TargetType != TargetType.Position && this.Data.TargetType != TargetType.Direction && this.TargetPosition == UVector3.zero)
            {
                if (this.Data.TargetType == TargetType.Random || this.Target == null || (this.Target.IsDead && !this.Data.FollowSourceSkillTarget))
                {
                    this.FindTarget(null);
                }

                if (this.Target == null)
                {
                    if (this.Caster != null && this.Casting && (this.Data.TriggerType & TriggerType.Manual) > 0)
                    {
                        int dist = 0;
                        Actor target = this.Caster.FindTarget(ref dist, this, true, this.Direction.x, this.Direction.y);
                        if (target == null)
                        {
                            return;
                        }

                        this.FindTarget(target);
                    }
                }
                if (this.Target == null && this.TargetPosition == UVector3.zero)
                {
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("{0} lost Target break.", this.ToString());
#endif
                    this.Break();//目标丢失，可能死亡或其他情况，此时不应直接返回，而应中断当前技能 add by Ray:ray@raymix.net 2016-04-13
                    return;
                }
                this.TargetPosition = new UVector3(this.Target.Position, this.Target.Height);
            }
            if (this.addMarkHitCount != null && this.addMarkHitCount.Count > 0)
                this.addMarkHitCount.Clear();
            switch (this.Data.CastType)
            {
                case CastType.Cast://投掷
                    {
                        CreateSingleCast();
                    }
                    break;
                case CastType.Bounce://释放弹射
                    {
                        Effect bounce = ObjectPool<Effect>.New();
                        bounce.Init(this, EffectType.Bounce, 0);
                        NeptuneBattle.Instance.AddEffect(bounce);
                        this.AddMarkCount(bounce.EffectIndex);
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
                    this.AddMarkCount(0);
                    this.HitAt(this.TargetPosition, this.Target);
                    break;
                default:
                    Logger.LogError(string.Format("Unkonwn CastType: {0}", this.Data.CastType));
                    break;
            }
            this.Caster.SetCostValue(this.Caster.GetCurrentCostValue() + this.Data.GainMP * NeptuneBattle.Instance.MPBonus);
            if (this.Caster.BasicSkill != null && (this.GroupData.SkillGroupID == this.Caster.BasicSkill.GroupData.SkillGroupID ||
                this.GroupData.ParentID == this.Caster.BasicSkill.GroupData.SkillGroupID))
            {
                if (this.Caster.AttributeFinalValue.MPAtkRecovery > 0)
                {
                    this.Caster.Remedy(this.Caster.AttributeFinalValue.MPAtkRecovery + this.Caster.AttributeFinalRatio.MPAtkRecovery * NeptuneConst.Hundredth * this.Caster.MaxMP, RoleAttribute.MaxMP);
                }
                if (this.Caster.AttributeFinalValue.HPAtkRecovery > 0)
                {
                    this.Caster.Remedy(this.Caster.AttributeFinalValue.HPAtkRecovery + this.Caster.AttributeFinalRatio.HPAtkRecovery * NeptuneConst.Hundredth * this.Caster.MaxHP);
                }
            }


            if (this.Data.MoveForward != 0)
            {
                this.Caster.Position = this.Caster.Position + this.Data.MoveForward * this.Caster.Orientation;
                if (this.Caster.Joint != null)
                    this.Caster.Joint.DoActionEvent(ActionEventType.Teleport);
            }
            if (this.Data.TeleportEventParam != null && this.Data.TeleportEventParam.Length > 1)
            {
                this.DoTeleport((TargetType)this.Data.TeleportEventParam[0], this.Data.TeleportEventParam[1]);
            }
            if (this.Data.DashEventParam != null && this.Data.DashEventParam.Length > 6)
            {
                float delaytime = this.Data.DashEventParam.Length > 7 ? this.Data.DashEventParam[7] : 0;
                float pausetime = this.Data.DashEventParam.Length > 8 ? this.Data.DashEventParam[8] : 0;
                float dashdirmode = this.Data.DashEventParam.Length > 9 ? this.Data.DashEventParam[9] : 0;
                float acceleration = this.Data.DashEventParam.Length > 10 ? this.Data.DashEventParam[10] : 0;
                this.DoDash(this.Data.DashEventParam[0], (int)this.Data.DashEventParam[1], (int)this.Data.DashEventParam[2], (int)this.Data.DashEventParam[3], this.Data.DashEventParam[4] <= 0, (int)this.Data.DashEventParam[5], this.Data.DashEventParam[6] <= 0, delaytime, pausetime, (int)dashdirmode, (int)acceleration);
            }
            if (this.Data.SummonActDatas != null && this.Data.SummonActDatas.Count > 0)
            {
                foreach (SummonActData summonactdata in this.Data.SummonActDatas)
                {
                    if (summonactdata.SummonTodoType != SummonTodoType.None)
                    {
                        int count = summonactdata.SummonToDoParam;
                        foreach (Actor summon in this.Caster.SummonList)
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
                            else if (summonactdata.SummonTodoType == SummonTodoType.SummonCastSkill)
                            {
                                summon.CastSkill(summonactdata.SummonToDoParam, this.Caster.Target);
                            }
                        }
                    }
                }
            }

            PlayExtraEffects(EffectPlayType.OnHitFrame, this.Caster.Position);
        }

        void DoDash(float time, int speed, int distance, int minDistance, bool ignoreObstacle, int maxDistance, bool NoRoot, float delayPauseTime, float pauseTime, int uselastdashdirmode = 0, int acceleration = 0)
        {//执行冲刺
            time = UFloat.Round(time);
            //TODO: 冲刺过程的碰撞伤害，可通过在这里设置正在冲刺的标志，碰撞及伤害可通过配置Trap类来实现（可考虑通过子技能实现效果配置）
            UVector2 targetPos = this.TargetPosition != UVector3.zero ? (UVector2)this.TargetPosition : (this.Target != null ? this.Target.Position : this.Caster.Position);
            int length = (targetPos - this.Caster.Position).magnitude;

            if (this.Target != null)
            {
                if (this.Target.IsRoleType(RoleType.Building))
                {
                    length -= (this.Caster.Data.CollideRadius + this.Target.Data.CollideRadius);
                }
            }

            if (uselastdashdirmode == 1 && LastDashDir != NeptuneConst.Vector2Zero)
            {
                this.Caster.Orientation = LastDashDir;
            }
            else if (uselastdashdirmode == 2)
            {
                if (this.Caster.moveDirection != NeptuneConst.Vector2Zero)
                    this.Caster.Orientation = this.Caster.moveDirection;
                else
                    length = 0;
            }
            else
            {
                Vector2 orientation = (targetPos - this.Caster.Position).normalized;
                if (orientation != Vector2.zero)
                    this.Caster.Orientation = orientation;
            }

            this.Caster.DashAcceleration = this.Caster.Orientation * acceleration;
            this.Caster.DashLastDistane = UVector2.zero;
            this.Caster.DashTotalTime = 0;
            this.Caster.DashSpeed = UVector2.zero;
            LastDashDir = this.Caster.Orientation;
            if (minDistance < NeptuneConst.MinDistance)
            {
                minDistance = NeptuneConst.MinDistance;
            }
            if (this.Data.TargetType == TargetType.Target && this.Target == null || length <= minDistance || (maxDistance != 0 && length >= maxDistance))
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
                        this.Caster.DashTime = UFloat.Round(time - UFloat.Round(distance * 1f / speed));
                        this.Caster.DashTime = this.Caster.DashTime < 0f ? 0 : this.Caster.DashTime;
                    }
                    else
                    {
                        this.Caster.DashTime = time;
                    }
                    this.Caster.MoveSpeed = this.Caster.Orientation * speed;
                    endspeed = speed + UFloat.RoundToInt(acceleration * time);
                }
                else if (speed == 0)
                {
                    this.Caster.DashTime = time;
                    int temp = UFloat.RoundToInt(UFloat.Round((length + distance) / time) - UFloat.Round(acceleration * time * 0.5f));
                    this.Caster.MoveSpeed = this.Caster.Orientation * temp;
                    endspeed = temp + UFloat.RoundToInt(acceleration * time);
                }
                else
                {
                    this.Caster.DashTime = time;
                    this.Caster.MoveSpeed = this.Caster.Orientation * speed;
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
                    this.Caster.DashTime = UFloat.Round((length + distance) / (speed + endspeed) * 2);
                    this.Caster.MoveSpeed = this.Caster.Orientation * speed;
                }

            }

            UVector2 destPos = this.Caster.Position + this.Caster.MoveSpeed * this.Caster.DashTime + this.Caster.DashAcceleration * UFloat.Round(this.Caster.DashTime * this.Caster.DashTime * 0.5f);
            obstacleWallHit = NeptuneBattle.Instance.RuleManager.ObstacleWallHit(this.Caster.Position, destPos);
            if (Data.CheckTrapObstacle && obstacleWallHit == UVector2.zero)
            {
                obstacleWallHit = NeptuneBattle.Instance.HitTrap(this.Caster, this.Caster.Position, destPos);
            }
            if (obstacleWallHit != UVector2.zero)
            {
                targetPos = obstacleWallHit - this.Caster.MoveSpeed.normalized * 50;
                if ((this.Caster.Position - targetPos).magnitude <= NeptuneConst.MinDistance || MathUtil.Dot((targetPos - this.Caster.Position).normalized, this.Caster.MoveSpeed.normalized) < 0)
                {
                    this.Caster.DashTime = 0;
                    this.Caster.MoveSpeed = UVector2.zero;
                    this.Caster.DashAcceleration = UVector2.zero;
                    return;
                }
            }
            else
            {
                targetPos = this.Caster.NavAgent.GetDashObstacleHit(this.Caster.Position, destPos, ignoreObstacle);
            }
            float moveSpeedMagnitude = UFloat.Round((this.Caster.MoveSpeed.magnitude + Math.Abs(endspeed)) * 0.5f);
            if (moveSpeedMagnitude != 0)
            {
                this.Caster.DashTime = UFloat.Round((this.Caster.Position - targetPos).magnitude / moveSpeedMagnitude);
            }
            else
            {
                this.Caster.DashTime = 0;
            }
            if (this.Caster.DashTime <= 0.0001f)
            {
                this.Caster.MoveSpeed = UVector2.zero;
                this.Caster.DashAcceleration = UVector2.zero;
            }
            else
            {
                this.Caster.DashDelayPauseTime = delayPauseTime;
                this.Caster.DashPauseTime = pauseTime;
                this.Caster.DashNoRoot = NoRoot;
            }
            this.Caster.DashSpeed = this.Caster.MoveSpeed;
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} DoDash :{1}  {2:f6} {3:f6}  {4:f6}  {5:f6} {6:f6} {6:f6} {7:f6}", this.Caster.FullName, this.FullName(), this.Caster.DashTime, this.Caster.MoveSpeed, this.Caster.DashPauseTime, this.Caster.DashDelayPauseTime, moveSpeedMagnitude,
                    endspeed, destPos);
#endif
        }

        void DoTeleport(TargetType targetType, int distance)
        {//执行瞬移
         //TODO：根据技能表配置的瞬移参数，计算瞬移目标位置
            UVector2 targetPos = this.TargetPosition != UVector3.zero ? (UVector2)this.TargetPosition : (this.Target != null ? this.Target.Position : this.Caster.Position);
            Vector2 direction = ((UVector2)targetPos - this.Caster.Position).normalized;
            UFloat.Round(ref direction);
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} {1} DoTeleport Start: Target={2} type={3} dist={4} dir=({5:f6},{6:f6}) caster:{7}", this.Caster.FullName, this.FullName(), targetPos, targetType, distance, direction.x, direction.y, this.Caster.Position);
#endif
            UVector2 pos = targetPos;
            if (direction == NeptuneConst.Vector2Zero)
            {
                direction = this.Caster.Orientation;
            }
            targetPos = this.Caster.Position;
            Vector2 offset = NeptuneConst.Vector2Zero;
            if (targetType == TargetType.Self)
            {
                offset = this.Caster.Orientation * distance;
                UFloat.Round(ref offset);
                targetPos += offset;
            }
            else if (targetType == TargetType.Target)
            {
                if (this.Target != null)
                {
                    offset = direction * distance;
                    targetPos = (UVector2)this.Target.Position + offset;
                    this.Caster.Orientation = direction;
                }
            }
            else if (targetType == TargetType.Position && direction != Vector2.zero)
            {
                distance += UVector2.Distance(pos, this.Caster.Position);
                if (distance >= this.MaxRange)
                {
                    distance = this.MaxRange;
                    offset = direction * distance;
                    UFloat.Round(ref offset);
                    targetPos += offset;
                }
                else
                    targetPos = pos;
                this.Caster.Orientation = direction;
            }
            else if (targetType == TargetType.Direction)
            {
                offset = direction * distance;
                UFloat.Round(ref offset);
                targetPos += offset;
                this.Caster.Orientation = direction;
            }
            if ((targetPos - this.Caster.Position).magnitude <= NeptuneConst.MinDistance)
            {
                return;
            }
            UVector2 perPos = targetPos;

#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} {1} DoTeleport Ready: Target={2} type={3} dist={4} dir=({5:f6},{6:f6}) offset=({7:f6},{8:f6})", this.Caster.FullName, this.FullName(), targetPos, targetType, distance, direction.x, direction.y, offset.x, offset.y);
#endif
            obstacleWallHit = NeptuneBattle.Instance.RuleManager.ObstacleWallHit(this.Caster.Position, targetPos);
            if (Data.CheckTrapObstacle && obstacleWallHit == UVector2.zero)
            {
                obstacleWallHit = NeptuneBattle.Instance.HitTrap(this.Caster, this.Caster.Position, targetPos);
            }
            if (obstacleWallHit != UVector2.zero)
            {

                targetPos = obstacleWallHit + this.Caster.MoveSpeed.normalized * 50;
                if ((this.Caster.Position - targetPos).magnitude > NeptuneConst.MinDistance && MathUtil.Dot((targetPos - this.Caster.Position).normalized, this.Caster.MoveSpeed.normalized) >= 0)
                {
                    this.Caster.Position = targetPos;
                }
            }
            else
            {
                if(this.Caster.NavAgent != null)
                {
                    targetPos = this.Caster.NavAgent.GetTeleportPos(this.Caster.Position, targetPos);
                }

                int result = (this.Caster.Position - perPos).magnitude < (this.Caster.Position - targetPos).magnitude ? 1 : -1;
                if (targetPos == perPos)
                {
                    result = 0;
                }
                this.Caster.Position = targetPos + result * this.Caster.Orientation.normalized * 33;
            }

#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} {1} DoTeleport Final: Target={2} type={3} dist={4} dir=({5:f6},{6:f6}) pos:{7}", this.Caster.FullName, this.FullName(), targetPos, targetType, distance, direction.x, direction.y, this.Caster.Position);
#endif
            if (this.Caster.OrcaAgent != null)
            {
                this.Caster.OrcaAgent.SetPosition(this.Caster.Position);
                this.Caster.OrcaAgent.Sync();
            }
            if (this.Caster.Joint != null)
                this.Caster.Joint.DoActionEvent(ActionEventType.Teleport);
        }

        void DoSkill(int talentID)
        {//执行技能
            Skill skill = this.Caster.GetSkillById(talentID);
            if (skill != null)
            {
                if (skill.Data.TargetType == TargetType.Self)
                {
                    skill.Target = Caster;
                }
                else if (skill.Data.TargetType == TargetType.Target)
                {
                    skill.Target = this.Target;
                }
                else
                {
                    skill.Target = null;
                }
                if (skill.CanUse(skill.Target) == ResultType.Success)
                {
                    if (skill.Data.TargetType == TargetType.Position || skill.Data.TargetType == TargetType.Direction)
                    {
                        skill.Start(this.TargetPosition);
                    }
                    else
                    {
                        skill.Start(skill.Target);
                    }

                }
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("{0} DoSkill :{1}  {2}", this.Caster.FullName, skill.FullName(), skill.Target == null ? "null" : skill.Target.FullName);
#endif
            }
        }


        /// <summary>
        /// 创建单个抛射器
        /// </summary>
        void CreateSingleCast()
        {
            if (this.Data.FlyDmgFactor != null && this.Data.FlyDmgFactor.Length > 0)
            {
                if (dividedHitTarget == null)
                    dividedHitTarget = new Dictionary<Actor, int>();
                else
                    dividedHitTarget.Clear();
            }
            Effect cast = this.CreateCast();
            if (this.Data.FlyBounce > 0)
            {
                cast.SetBounce(this.Data.FlyBounce);
            }
            if (this.Target != null)
            {
                if (this.Data.FlyTrackingTarget)
                {
                    cast.TrackTarget(this.Target);
                }
            }

            NeptuneBattle.Instance.AddEffect(cast);
            this.AddMarkCount(cast.EffectIndex);
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
            float count = (this.Data.CastNum - this.HitTimes) / this.Data.CastTimes + 1;
            Actor oldTarget = this.Target;
            for (int i = 1; i <= count; i++)
            {
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("{0}: {1} --> {2} crete projectile {3}: {4}＝({5} - {6})/5+1", this.Caster.FullName, this.ToString(), this.Target != null ? this.Target.FullName : "", i, count, this.Data.CastNum, this.HitTimes);
#endif
                Effect cast = this.CreateCast(i - 1);
                if (i > 1)
                    this.FindTarget();
                OffsetMap[i - 1] = OffsetMap[i - 1] + new Vector2(5, -14);
                if (i == 1 || this.Target != oldTarget)
                {
                    //Vector2 dist = new Vector2(this.Target.Position.x + OffsetMap[i - 1].x * this.Caster.InitDirection.x, this.Target.Position.y + OffsetMap[i - 1].y * this.Caster.InitDirection.y) - this.Caster.Position;
                    UVector2 dist = new UVector2(this.Target.Position.x + OffsetMap[i - 1].x * Vector2.right.x, this.Target.Position.y + OffsetMap[i - 1].y * Vector2.right.y) - this.Caster.Position;
                    cast.Speed = dist * this.Data.FlyGravity / (-cast.SpeedZ - (float)Mathf.Sqrt(cast.SpeedZ * cast.SpeedZ - 2f * this.Data.FlyGravity * cast.Height));
                    // cast.Speed = dist * this.Data.FlyGravity / (-cast.SpeedZ - (float)Mathf.Pow((float)Mathf.Pow(cast.SpeedZ, 2f) - 2f * this.Data.FlyGravity * cast.Height, 0.5f));
                    NeptuneBattle.Instance.AddEffect(cast);
                    this.AddMarkCount(cast.EffectIndex);
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
                dividedHitTarget = new Dictionary<Actor, int>();
            else
                dividedHitTarget.Clear();
            if (Data.FlyCastDirections != null && Data.FlyCastDirections.Length > 0)
            {
                int counter = 1;
                for (int i = 0; i < Data.FlyCastDirections.Length; i++)
                {
                    int angle = Data.FlyCastDirections[i];
                    Effect effect = ObjectPool<Effect>.New();
                    effect.Init(this, EffectType.Cast, angle, counter - 1);
                    NeptuneBattle.Instance.AddEffect(effect);
                    this.AddMarkCount(effect.EffectIndex);
                    counter++;
                }
            }
            else
            {
                Actor oldTarget = this.Target;
                int counter = 1;
                bool isAttackMainTarget = false;
                Entity pos = this.Caster;
                if (AttackPointElement != null)
                {
                    pos = AttackPointElement;
                }
                isAttackMainTarget = true;
                Effect cast = null;
                if (this.Target != null)
                {
                    cast = this.CreateCast(counter - 1);
                    cast.DamageFactor = 1;
                    cast.TrackTarget(this.Target);
                    NeptuneBattle.Instance.AddEffect(cast);
                    this.AddMarkCount(cast.EffectIndex);
                }

                //            foreach (Actor role in NeptuneBattle.Instance.GetSurvivors(this.Caster, this.AffectedSide, this.Data.MaxRange))
                foreach (Actor role in NeptuneBattle.Instance.AOIManager.GetSurvivors(pos, this.AffectedSide, this.MaxRange))
                {
                    if (counter >= this.Data.CastNum)
                        break;
                    if (!CheckTargetClass(role))
                        continue;
                    if (role == oldTarget)
                        continue;
                    if (this.Data.TargetSelectorWithOutSelf && role == this.Caster)
                        continue;
                    /*if (counter == this.Data.CastNum - 1 && !isAttackMainTarget)
                    {
                        this.Target = oldTarget;
                        isAttackMainTarget = true;
                    }
                    else */
                    if (pos.Distance(role, NeptuneConst.EnableRadiusInDistance) < this.MaxRange)
                    {
                        this.Target = role;
                    }
                    else
                        continue;
                    counter++;
                    cast = this.CreateCast(counter - 1);
                    //                 if (this.Target == oldTarget)
                    //                 {
                    //                     isAttackMainTarget = true;
                    //                     cast.DamageFactor = 1;
                    //                 }
                    //                 else
                    cast.DamageFactor = this.Data.Param1;
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("{0}: {1} --> {2} DividedCast {3}: {4:f6}", this.Caster.FullName, this.FullName(), this.Target.FullName, counter, cast.DamageFactor);
#endif
                    cast.TrackTarget(this.Target);
                    NeptuneBattle.Instance.AddEffect(cast);
                    this.AddMarkCount(cast.EffectIndex);
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
                    Effect cast = CreateCast(i);
                    if (cast != null)
                    {
                        cast.Speed = castDirection * cast.Speed.magnitude;
                    }
                    NeptuneBattle.Instance.AddEffect(cast);
                    this.AddMarkCount(cast.EffectIndex);
                }
            }
        }

        void CreateRoundCast()
        {
            var angle = Data.Param2;
            var castCount = Data.CastNum;

            //if ((int)(angle * castCount) == 360)
            {
                var casterOri = Caster.Orientation;
                for (int i = 0; i < castCount; i++)
                {
                    Vector2 castDirection = MathUtil.Deflection(casterOri, angle * i);

                    // create a cast but dont need to know wheather it has a target
                    Effect cast = CreateCast(i, (int)angle * i);
                    if (cast != null)
                    {
                        cast.Speed = castDirection * cast.Speed.magnitude;
                    }
                    NeptuneBattle.Instance.AddEffect(cast);
                    this.AddMarkCount(cast.EffectIndex);
                }
            }
        }
        public virtual Effect CreateCast(int index = 0, int effectAngle =0)
        {
            Effect cast = ObjectPool<Effect>.New();
            cast.Init(this, EffectType.Cast, effectAngle, index);
            return cast;
        }

        /// <summary>
        /// 命中位置
        /// </summary>
        /// <param name="hitpos">在指定位置命中</param>
        /// <param name="target"></param>
        /// <param name="from"></param>
        /// <param name="hitType"></param>
        public virtual void HitAt(UVector2 hitpos, Actor target, Entity from = null, SkillHitType hitType = SkillHitType.SkillHit)
        {

            var orientation = this.Orientation != NeptuneConst.Vector2Zero ? this.Orientation : this.Caster.Orientation;
            if (this.Data.AreaCenter == TargetType.Self)
                hitpos = this.Caster.Position;
            UVector2 pos = hitpos + this.Data.AreaShift * orientation;
            bool isHit = false;
            Actor soundTarget = null;
            if (this.Data.AreaCenter != TargetType.None && this.Data.AreaShape != AreaShape.None)
            {
                if (target != null && CheckTargetClass(target) && (target.GetRelation(from == null ? this.Caster.Side : from.Side) == this.AffectedSide || this.AffectedSide == RelativeSide.Both))
                {
                    UVector2 distance = target.Position - pos;
                    bool inIgnore = false;
                    if (this.Data.IgnoreAreaShape != AreaShape.None && EngineUtil.HitTest(distance, this.Data.IgnoreAreaShape, new UVector2(from == null ? this.Data.IgnoreAreaArg1 + Caster.Radius + target.Radius : this.Data.IgnoreAreaArg1, this.Data.IgnoreAreaArg2),
                        orientation))
                    {
                        inIgnore = true;
                    }
                    if (!inIgnore && (EngineUtil.HitTest(distance, this.Data.AreaShape, new UVector2(from == null ? this.Data.AreaArg1 + Caster.Radius + target.Radius + this.Data.MainTargetAddRange : this.Data.AreaArg1 + this.Data.MainTargetAddRange, this.Data.AreaArg2),
                        orientation) || HitCheckAffiliatedAreaShape(target, hitpos, orientation, new UVector2(from == null ? Caster.Radius + target.Radius + this.Data.MainTargetAddRange : this.Data.MainTargetAddRange, 0))))
                    {
                        if (this.HitTarget(target, from, hitType) > 0)
                            soundTarget = target;
                    }
                }

                int range = this.Data.AreaArg1;
                if (this.Data.AreaShape == AreaShape.Rectangle)
                {
                    range = (int)new Vector2(this.Data.AreaArg1, this.Data.AreaArg2).magnitude;
                }
                //            foreach (Actor role in NeptuneBattle.Instance.GetSurvivors(this.Caster, this.AffectedSide, this.Data.MaxRange))
                foreach (Actor role in NeptuneBattle.Instance.AOIManager.GetSurvivors(pos, from == null ? this.Caster.Side : from.Side, this.AffectedSide, this.Data.MaxRange + (this.Data.AreaCenter == TargetType.Self ? 0 : range), from == null ? this.Caster.Radius : from.Radius))
                {
                    if (this.hitTargetNum > MaxTargetNum && MaxTargetNum > 0)
                    {
                        break;
                    }
                    if (this.Data.TargetSelectorWithOutSelf && role == this.Caster)
                        continue;
                    if (!CheckTargetClass(role))
                        continue;


                    if (target == role)
                        continue;

                    if (!(this.Caster.Data.AttackAir || Data.CanAttackAir) && role.IsAirForce)
                        continue;

                    UVector2 dist = role.Position - pos;
                    //TODO:换成矢量方向后计算需效验
                    //dist.x = dist.x * this.Caster.Orientation; 
                    if (this.Data.IgnoreAreaShape != AreaShape.None && EngineUtil.HitTest(dist, this.Data.IgnoreAreaShape, new UVector2(from == null ? this.Data.IgnoreAreaArg1 + Caster.Radius + role.Radius : this.Data.IgnoreAreaArg1, this.Data.IgnoreAreaArg2),
                       orientation))
                    {
                        continue;
                    }
                    if (EngineUtil.HitTest(dist, this.Data.AreaShape, new UVector2(from == null ? this.Data.AreaArg1 + Caster.Radius + role.Radius : this.Data.AreaArg1, this.Data.AreaArg2),
                        orientation) || HitCheckAffiliatedAreaShape(role, hitpos, orientation, new UVector2(from == null ? Caster.Radius + role.Radius : 0, 0)))
                    {

                        if (this.HitTarget(role, from, hitType) > 0 && soundTarget == null)
                        {
                            soundTarget = role;
                        }
                    }
                }
                if (soundTarget != null && soundTarget.Joint != null)
                {
                    isHit = true;
                    if (!string.IsNullOrEmpty(this.Data.HitSound))
                        soundTarget.PlaySound(this.Data.HitSound);
                }
            }
            else if (target != null)
            {
                this.HitTarget(target, from, hitType);
                isHit = true;
            }
            castingTrapList.Clear();
            if (this.Data.TrapIds != null && this.Data.TrapIds.Length > 0)
            {
                TrapTriggerType trapTriggerType = TrapTriggerType.None;
                if (hitType == SkillHitType.SkillHit)
                {
                    trapTriggerType = TrapTriggerType.Hit;
                }
                else
                {
                    if ((hitType & SkillHitType.EffectHit) > 0)
                        trapTriggerType |= TrapTriggerType.Hit;
                    if ((hitType & SkillHitType.EffectEnd) > 0)
                        trapTriggerType |= TrapTriggerType.EffectEnd;
                }
                CreateTrap(trapTriggerType, pos, target);
            }
            if (this.Data.TriggerTrapID > 0)
            {
                NeptuneBattle.Instance.TriggerTrap(this.Caster, this.Data.TriggerTrapID);
            }

            this.SkillSummon(hitpos);//技能召唤

            if (!string.IsNullOrEmpty(this.Data.DirectEffect))
            {
                bool createEffect = true;
                if (this.Caster.Joint != null && this.Caster.Joint.Controller != null)
                {
                    createEffect = ((IActorController)this.Caster.Joint.Controller).InSight;
                }
                if (createEffect)
                    NeptuneBattle.PlayEffect(this.Data.DirectEffect, pos, this.Caster.Orientation, 0, this.Data.DirectOrder);
                //NeptuneBattle.PlayEffect(this.Data.DirectEffect, pos, Vector2.one, 0, this.Data.DirectOrder);

            }
            NeptuneBattle.Instance.OnRoleHit(this.Owner);
            PlayExtraEffects(EffectPlayType.OnHitAt, pos);
        }

        public void CreateTrap(TrapTriggerType triggerType, Vector2 pos, Actor target, Action _act = null)
        {
            foreach (int trapId in this.Data.TrapIds)
            {
                if (((TrapTriggerType)NeptuneBattle.Instance.DataProvider.GetTrapData(trapId).TriggerType & triggerType) > 0)
                {
                    Trap trap = ObjectFactory.Create(trapId, this, pos, target);
                    if (_act != null) trap.OnEndAct(_act);
                    if (trap != null)
                    {
                        NeptuneBattle.Instance.AddTrap(trap);
                        castingTrapList.Add(trap);
                    }
                }
            }
        }

        public Trap GetCreateTrap(TrapTriggerType triggerType, Vector2 pos, Actor target, Action _act = null)
        {
            foreach (int trapId in this.Data.TrapIds)
            {
                if (((TrapTriggerType)NeptuneBattle.Instance.DataProvider.GetTrapData(trapId).TriggerType & triggerType) > 0)
                {
                    Trap trap = ObjectFactory.Create(trapId, this, pos, target);
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

        protected virtual void SkillSummon(UVector2 pos)
        {
            if (this.Data.SummonID > 0)
            {
                SummonData data = NeptuneBattle.Instance.DataProvider.GetSummonData(this.Data.SummonID, this.Data.SummonLevel);
                if (data == null)
                    return;
                //TODO:执行召唤
                for (int i = 0; i < this.Data.SummonNumber; i++)
                {
                    //初始化角色配置
                    RoleInfo hero = new RoleInfo();
                    hero.tid = this.Data.SummonID;
                    hero.level = this.Data.SummonLevel == 0 ? this.Caster.Level : this.Data.SummonLevel;
                    RoleConfig role_config = new RoleConfig();
                    //role_config.HPFactor = (float)EngineDataManager.Instance.RoleConfigs.Value[(int)this.Data.SummonID].HPFactorArena;
                    role_config.IsMonster = true;
                    //Actor.InitConfigPredictInfo(role_config, this.Data.SummonID, false);

                    //创建Role
                    Actor role = ObjectFactory.Create(hero, this.Caster.Side, role_config, null, pos + this.Caster.Orientation * 1, this.Caster.Orientation);
                    role.Player = this.Caster.Player;
                    NeptuneBattle.Instance.AddActor(role, this.Caster, pos + this.Caster.Orientation * 1);
                    role.InitSummon(data, role_config, this.Caster, this.Data.SummonLevel);
                }
            }
        }

        public virtual int HitTarget(Actor target, Entity from = null, SkillHitType hitType = SkillHitType.SkillHit)
        {
            Entity Source = from ?? this.Caster;

            if (CheckIgnoreTarget(target))
                return 0;

            if (this.Data.Dispel)
            {//驱散具有最高优先级
                for (int cIndex = 0; cIndex < this.Caster.tAbilities.Length; cIndex++)
                {
                    Ability ability = this.Caster.tAbilities[cIndex];
                    if (ability != null && ability.Caster.Side != this.Caster.Side)
                    {
                        this.Caster.RemoveAbilityAt(cIndex);
                    }
                }
                //List<Ability> needRemoveAbilities = this.Caster.Abilities.Where(ability => ability.Caster.Side != this.Caster.Side).ToList();
                //this.Caster.RemoveAbilities(needRemoveAbilities);
            }
            //不受释放限制
            if (this.Data.Unrestricted)
            {
                for (int cIndex = 0; cIndex < this.Caster.tAbilities.Length; cIndex++)
                {
                    Ability ability = this.Caster.tAbilities[cIndex];
                    if (ability != null && ability.Caster.Side != this.Caster.Side && ability.IsControlAbility)
                    {
                        this.Caster.RemoveAbilityAt(cIndex);
                    }
                }

                //List<Ability> needRemoveAbilities = this.Caster.Abilities.Where(ability =>
                //    ability.Caster.Side != this.Caster.Side && ability.IsControlAbility).ToList();
                //this.Caster.RemoveAbilities(needRemoveAbilities);
            }


            //Lyon ：target.IsActiveCasting 用于HCU3D 放大招冻结相关逻辑 我们这边不需要
            //if (target ==null || target.IsDead || target.AbilityEffects.Invincible || target.IsActiveCasting && Source.Side != target.Side)
            if (target == null || target.IsDead || target.AbilityEffects.Value.Invincible)
            {
                return 0;
            }
            Effect effect = from as Effect;
            if (effect != null && effect.Joint != null)
                effect.Joint.OnHit(target);
            if (target.Joint != null && !string.IsNullOrEmpty(this.Data.ContinuousEffect))
            {
                if (!targetContinuousEffects.ContainsKey(target))
                {
                    targetContinuousEffects[target] = target.Joint.AddEffect(this.Data.ContinuousEffect, EffectType.Normal, UVector3.zero, (IActorController)this.Caster.Joint.Controller);
                }
            }
            if (this.Data.ContinuousType != ContinuousType.None && this.HitTimes == 1)
            {
                return 0;
            }
            this.hitTargetNum++;

            //TriggerMarkSelf(target, this.Data.MarksToTriggerSelf, MarkTriggerType.BeforeInijury);
            TriggerMarkTarget(false, target, this.Data.MarksToTriggerSelf, MarkTriggerType.BeforeInijury);
            TriggerMarkTarget(true, target, this.Data.MarksToTriggerTarget, MarkTriggerType.BeforeInijury);
            if (this.HasAbility(AbilityTriggerType.Hit, AbilityTriggerMode.BeforeInijury))
            {
                CreateSkillAbilitis(target, AbilityTriggerType.Hit, hitType);
            }

            float factor = 0;
            float baseForce = NeptuneBattle.Instance.Numeric.CalculateSkillPower(this, Source, target, ref factor);
            float curHpDamage = 0;
            if (this.Data.CurHpDamageRatio > 0)
            {
                curHpDamage = UFloat.Round(UFloat.Round(this.Data.CurHpDamageRatio * NeptuneConst.Hundredth) * target.HP);
                if (this.Data.TargetTypeAffect == target.Data.RoleType || ((this.Data.TargetTypeAffect & RoleType.Demon) == RoleType.Demon && target.Config.IsDemon))
                {
                    if (this.Data.ExtraTargetTypeRatio > 0)
                        curHpDamage = UFloat.Round(curHpDamage * this.Data.ExtraTargetTypeRatio);
                }
                if (target.IsCreep)
                {
                    curHpDamage = Math.Min(curHpDamage, NeptuneConst.CreepCurHpMaxDamage);
                }
            }
            baseForce = UFloat.Round(baseForce + curHpDamage);
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0}: {1} --> {2} (-{3:f6}) {4:f6}", this.Caster.FullName, this.FullName(), target.FullName, baseForce, factor);
#endif

            if (this.Data.EffectFlyTimeDamageRatio != 0 && this.castFlyTime > 0 && ((hitType & SkillHitType.EffectHit) > 0 || (hitType & SkillHitType.EffectEnd) > 0))
            {
                baseForce = Mathf.Max(baseForce * Mathf.Min(UFloat.Round(1 + this.castFlyTime * this.Data.EffectFlyTimeDamageRatio), this.Data.MaxEffectFlyTimeDamageRatio), 1.0f);
            }
            this.Caster.ExecutePassive(TriggerType.Damage, target, this);

            if (effect != null && this.Data.FlyDmgFactor != null && this.Data.FlyDmgFactor.Length > 0)
            {
                if (CastType.Cast == Data.CastType && this.Data.CastEffectDmgFactorType == CastEffectDmgFactorType.All)
                {
                    float dmgFactor = effect.HitCount < this.Data.FlyDmgFactor.Length ? this.Data.FlyDmgFactor[effect.HitCount] : this.Data.FlyDmgFactor[this.Data.FlyDmgFactor.Length - 1];
                    baseForce = UFloat.Round(baseForce * dmgFactor);
                    effect.HitCount++;
                }
                else if (CastType.Divided == Data.CastType || CastType.Cast == Data.CastType)
                {
                    if (dividedHitTarget.ContainsKey(target))
                        dividedHitTarget[target]++;
                    else
                        dividedHitTarget.Add(target, 0);
                    int index = dividedHitTarget[target] >= this.Data.FlyDmgFactor.Length ? this.Data.FlyDmgFactor.Length - 1 : dividedHitTarget[target];
                    baseForce = UFloat.Round(baseForce * this.Data.FlyDmgFactor[index]);//分裂箭伤害衰减
                }
                else
                {
                    if (this.Data.FlyBounce > effect.LeftAttackCount)
                    {
                        baseForce = UFloat.Round(baseForce * this.Data.FlyDmgFactor[this.Data.FlyBounce - effect.LeftAttackCount]);//计算弹射伤害
                    }
                }
            }
            baseForce = UFloat.Round(baseForce * this.DamageFactor);
            InjuryType injuryType = this.Data.DamageType;
            int result = 0;
            if (injuryType != InjuryType.None)
            {
                if (this.Data.AffectedSide == RelativeSide.Both && this.Caster.GetRelation(target) == RelativeSide.Friend)
                {//影响全体的技能，如果目标为友方，自动转为治疗效果
                    injuryType = InjuryType.Heal;
                    if (this.Data.Param1 > 0)
                        baseForce = UFloat.Round(baseForce * this.Data.Param1);
                }
                RoleAttribute attributeType = this.Data.AffectMP ? (target.Data.MPType == MPType.Rage ? RoleAttribute.MaxRage : RoleAttribute.MaxMP) : RoleAttribute.MaxHP;

                if (injuryType == InjuryType.Heal)
                {
                    if (this.Data.TargetSide != RelativeSide.Both && this.Caster.Side != target.Side)//绝对不治疗敌方
                        return 0;

                    float healValue = attributeType == RoleAttribute.MaxHP ? this.Caster.AttributeFinalValue.Heal : 0;
                    baseForce = UFloat.Round(baseForce * (1 + healValue / 100));
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("{0}: {1} --> {2} (+{3:f6})", this.Caster.FullName, this.FullName(), target.FullName, baseForce);
#endif
                    target.Remedy(baseForce, attributeType, this.Caster);

                    this.Caster.OnRemedy(target);
                }
                else
                {
                    if (this.Caster.Side == target.Side) //绝对不攻击友方
                    {
                        return 0;
                    }

                    if (TargetDodge(target, injuryType))
                    {
                        return 0;
                    }
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("{0}: {1} --> {2} PWR(-{3:f6})", this.Caster.FullName, this.FullName(), target.FullName, baseForce);
#endif
                    target.InjuryFactor = 1;
                    target.InjuryCrit = (this.Data.CRITPct + this.Caster.PassiveEffect.CRITPct) / 100f;
                    int critCount = 0;
                    target.ExecutePassive(TriggerType.IsAttacked, this.Caster, this);
                    if (target.PassiveEffect.ApportionTotalRatio > 0 && attributeType == RoleAttribute.MaxHP)
                    {
                        baseForce = target.ApportionInjury(injuryType, attributeType, baseForce);
                    }

                    result = this.CalculateInjury(injuryType, attributeType, target, baseForce, this.Caster, ref critCount);
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("{0}: {1} --> {2} DMG(-{3})", this.Caster.FullName, this.FullName(), target.FullName, result);
#endif

                    if (result <= 0)
                    {
                        return 0;
                    }

                    //追加技能额外伤害开始
                    if (this.Caster.AttributeFinalValue.ExtraAbilityPower > 0)
                    {
                        float extraDamage = this.Caster.AttributeFinalValue.ExtraAbilityPower;
                        if (this.Data.ExtraDamageType == InjuryType.AbilityPower)
                        {
                            extraDamage += this.Data.ExtraBasicNum;
                        }
                        target.CalculateInjury(this.Data.ExtraDamageType, this.Data.ExtraAffectMP ? RoleAttribute.MaxMP : RoleAttribute.MaxHP, extraDamage, this.Caster, this, ref critCount);
                    }
                    if (this.Caster.AttributeFinalValue.ExtraAttackDamage > 0)
                    {
                        float extraDamage = this.Caster.AttributeFinalValue.ExtraAttackDamage;
                        if (this.Data.ExtraDamageType == InjuryType.AttackDamage)
                        {
                            extraDamage += this.Data.ExtraBasicNum;
                        }
                        target.CalculateInjury(this.Data.ExtraDamageType, this.Data.ExtraAffectMP ? RoleAttribute.MaxMP : RoleAttribute.MaxHP, extraDamage, this.Caster, this, ref critCount);
                    }
                    if (this.Caster.AttributeFinalValue.ExtraHoly > 0)
                    {
                        float extraDamage = this.Caster.AttributeFinalValue.ExtraHoly;
                        if (this.Data.ExtraDamageType == InjuryType.Holy)
                        {
                            extraDamage += this.Data.ExtraBasicNum;
                        }
                        target.CalculateInjury(this.Data.ExtraDamageType, this.Data.ExtraAffectMP ? RoleAttribute.MaxMP : RoleAttribute.MaxHP, extraDamage, this.Caster, this, ref critCount);
                    }
                    if (this.Data.ExtraBasicNum > 0)
                        target.CalculateInjury(this.Data.ExtraDamageType, this.Data.ExtraAffectMP ? RoleAttribute.MaxMP : RoleAttribute.MaxHP, this.Data.ExtraBasicNum, this.Caster, this, ref critCount);

                    // 计算对特定目标的额外伤害
                    //CalculateTargetExtraInjury(this.Caster, target, this, this.Data.GetTargetExtraDamageData(), ref critCount);// TODO 2024 需要修复 扩展方法

                    //追加技能额外伤害结束
                    float addHp = 0;
                    float addMp = 0;
                    if (!target.IsRoleType(RoleType.Building) && (this.Data.StealRoleType <= 0 || target.IsRoleType(this.Data.StealRoleType))
                        && (!this.Data.StealMainTarget || this.Target == target))
                    {
                        if (this.Data.StealPct > 0)
                        {//生命偷取
                            addHp = UFloat.Round(result * this.Data.StealPct * NeptuneConst.Hundredth);
                        }
                        addHp = UFloat.Round(addHp + result * NeptuneBattle.Instance.Numeric.GetLifeStealFactor(this.Caster, this));
                        if (this.Data.StealMpPct > 0)
                        {//魔法偷取
                            addMp = UFloat.Round(result * this.Data.StealMpPct * NeptuneConst.Hundredth);
                        }
                    }

                    if (addHp > 1)
                    {
#if BATTLE_LOG
                        if (EngineGlobal.BattleLog)
                            NeptuneBattle.log("  {0} leeches {1:f6} {2} form {3}", this.Caster.FullName, addHp, injuryType, target.FullName);
#endif
                        this.Caster.Remedy(addHp, attributeType);
                    }

                    if (addMp > 1)
                    {
#if BATTLE_LOG
                        if (EngineGlobal.BattleLog)
                            NeptuneBattle.log("  {0} leeches {1:f6} {2} form {3}", this.Caster.FullName, addMp, injuryType, target.FullName);
#endif
                        this.Caster.Remedy(addMp, RoleAttribute.MaxMP);
                    }



                    if (critCount > 0)
                    {
                        //造成暴击触发
                        Caster.OnCrit(this);
                    }
                    //造成伤害触发
                    Caster.OnDamage(target, this);
                    //被攻击时技能触发
                    target.OnIsAttacked(this.Caster, this, result);

                }
            }
            bool abilityMiss = false;
            if (this.HasAbility(AbilityTriggerType.Hit))
            {
                abilityMiss = CreateSkillAbilitis(target, AbilityTriggerType.Hit, hitType);
            }
            //移除目标身上的指定ID的buff
            /*if(!target.IsDead)
            {
                target.RemoveAbilitys(AbilityRemoveType.OnHurt);
            }*/
            //移除目标身上的指定ID的buff
            if (!target.IsDead && Data.RemoveTargetAbilityIDs != null && Data.RemoveTargetAbilityIDs.Length > 1)
            {
                for (int i = 0; i < Data.RemoveTargetAbilityIDs.Length; i += 2)
                {
                    int id = Data.RemoveTargetAbilityIDs[i];
                    int count = Data.RemoveTargetAbilityIDs[i + 1];
                    target.RemoveAbility(id, count);
                }
            }


            if (!abilityMiss && (0 < this.Data.RepelHeight || (this.Data.RepelBackDist != 0 && this.Data.RepelBackTime > 0)))
            {
                float backTime = this.Data.RepelBackTime;
                float backDist = this.Data.RepelBackDist;
                float acceleration = this.Data.RepelAcceleration;
                Vector2 direction;
                Vector2 offset = this.Data.RepelOffset != null && this.Data.RepelOffset.Length > 1 ? new Vector2(this.Data.RepelOffset[0], this.Data.RepelOffset[1]) : Vector2.zero;
                Vector2 offsetX;
                Vector2 offsetY;
                if (this.Data.AttractDirectType == AttractDirectType.TargetPosition)
                {
                    direction = target.Position - (UVector2)this.TargetPosition;
                    offsetX = direction.normalized * offset.x;
                    offsetY = MathUtil.GetRight(direction.normalized) * offset.y;
                    direction += offsetX + offsetY;
                }
                else
                {
                    if ((target.Position - Source.Position).magnitude < 0.01f || this.Data.AttractDirectType == AttractDirectType.SourceDirection || this.Data.TargetType == TargetType.Direction || this.Data.TargetType == TargetType.Position)
                    {
                        direction = Source.Orientation;
                    }
                    else
                    {
                        direction = (target.Position - Source.Position);
                        offsetX = direction.normalized * offset.x;
                        offsetY = MathUtil.GetRight(direction.normalized) * offset.y;
                        direction += offsetX + offsetY;
                    }
                }



                // 吸引效果，保留吸引速度，但是使其不越过吸引中心
                if (backDist < 0)
                {
                    if (this.Data.AttractDirectType == AttractDirectType.TargetPosition)
                    {
                        direction = target.Position - (UVector2)this.TargetPosition;
                    }
                    else if (this.Data.AttractDirectType == AttractDirectType.SourceDirection)
                    {
                        direction = Source.Orientation;
                    }
                    else
                    {
                        direction = target.Position - this.Caster.Position;
                    }
                    offsetX = direction.normalized * offset.x;
                    offsetY = MathUtil.GetRight(direction.normalized) * offset.y;
                    direction += offsetX + offsetY;
                    int collideRadius = this.Data.AttractDirectType == AttractDirectType.Caster ? this.Caster.Data.CollideRadius + target.Data.CollideRadius : 0;
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
                target.Repel(this.Data.RepelHeight, this.Data.RepelGravityFactor, backTime, direction.normalized * backDist, direction.normalized * acceleration, this.Data.RepelIgnoreObstacle, this.Data.CheckTrapObstacle, this.Data.RepelResistIgnorance);
            }

            //HandleMark(target);

            //TriggerMarkSelf(target, this.Data.MarksToTriggerSelf, MarkTriggerType.AfterInijury);
            TriggerMarkTarget(false, target, this.Data.MarksToTriggerSelf, MarkTriggerType.AfterInijury);
            TriggerMarkTarget(true, target, this.Data.MarksToTriggerTarget, MarkTriggerType.AfterInijury);
            if (!target.IsDead)
            {
                AddMark(target, this.Data.MarksToAdd, AddMarkType.Hit, effect);
            }
            AddMark(this.Caster, this.Data.MarksToSelf, AddMarkType.Hit, effect);
            if (!target.IsDead && Data.RemoveTargetMarkIDs != null && Data.RemoveTargetMarkIDs.Length > 1)
            {
                for (int i = 0; i < Data.RemoveTargetMarkIDs.Length; i += 2)
                {
                    int id = Data.RemoveTargetMarkIDs[i];
                    int count = Data.RemoveTargetMarkIDs[i + 1];
                    target.RemoveMarkByID(id, count);
                }
            }
            if (target.Joint != null)
            {
                IActorController controller = this.Caster.Joint != null ? this.Caster.Joint.Controller : null;
                target.Joint.AddEffect(this.Data.HitEffect, EffectType.Hit, new Vector3(0, 0, this.Data.HitOrder), controller);
                if (!(this.Data.AreaCenter != TargetType.None && this.Data.AreaShape != AreaShape.None) && !string.IsNullOrEmpty(this.Data.HitSound))
                    target.PlaySound(this.Data.HitSound);
            }
            return result;
        }

        //private void HandleMark(Actor target)
        //{
        //    int markID = 0;
        //    Mark mark = null;

        //    int count1 = Data.MarksToAdd == null ? 0 : Data.MarksToAdd.Length;
        //    int count2 = Data.MarksToTrigger == null ? 0 : Data.MarksToTrigger.Length;

        //    //给目标添加印记
        //    for (int i = 0; i < count1; i++)
        //    {
        //        markID = Data.MarksToAdd[i];
        //        mark = target.GetMarkByID(markID);

        //        if (mark == null)
        //        {
        //            //目标身上尚无此种印记 创建并添加到目标身上
        //            MarkData data = NeptuneBattle.Instance.DataProvider.GetMarkData(markID);
        //            if (data != null)
        //            {
        //                mark = ObjectFactory.Create(data, target, this.Caster);
        //                target.AddMark(markID, mark);
        //            }
        //            else
        //            {
        //                Debug.LogError("There's no data in Mark table, mard id:" + markID);
        //            }
        //        }
        //        if (mark != null)
        //        {
        //            mark.Deepen();
        //        }
        //    }

        //    //触发目标身上的印记
        //    for (int i = 0; i < count2; i++)
        //    {
        //        mark = target.GetMarkByID(Data.MarksToTrigger[i]);

        //        if (mark != null)
        //        {
        //            mark.OuterTrigger(target.Position);
        //        }
        //    }
        //}

        public void AddMark(Actor target, int[] marks, AddMarkType addMarkType, Effect effect = null)
        {
            int markID = 0;
            Mark mark = null;

            int count = marks == null ? 0 : marks.Length;
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
                if (!this.checkAddMarkCount(key, markID))
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
                        if (target == this.Caster && data.AddMarkType != addMarkType)
                        {
                            continue;
                        }
                        if (data.RoleType > 0 && (data.RoleType & target.Data.RoleType) <= 0)
                        {
                            continue;
                        }
                        mark = ObjectFactory.Create(data, target, this.Caster);
                        target.AddMark(markID, mark);
                    }
                    else
                    {
                        Debug.LogError("There's no data in Mark table, mard id:" + markID);
                    }
                }
                if (mark != null)
                {
                    if (target == this.Caster && mark.Data.AddMarkType != addMarkType)
                    {
                        continue;
                    }
                    mark.Deepen();
                }
            }
        }
        private void TriggerMarkSelf(bool triggerTarget, Actor target, int[] marks, MarkTriggerType markTriggerType)
        {

        }

        private void TriggerMarkTarget(bool triggerTarget, Actor target, int[] marks, MarkTriggerType markTriggerType)
        {
            Mark mark = null;

            int count = marks == null ? 0 : marks.Length;

            //触发目标身上的印记
            for (int i = 0; i < count; i++)
            {
                if(triggerTarget)
                    mark = target.GetMarkByID(marks[i]);
                else
                    mark = Caster.GetMarkByID(marks[i]);

                if (mark != null && mark.Data.MarkTriggerType == markTriggerType)
                {
                    if(triggerTarget)
                        mark.OuterTrigger(this.Caster, this.TargetPosition);
                    else
                        mark.OuterTrigger(target, this.TargetPosition);

                }
            }
        }

        private bool TargetDodge(Actor target, InjuryType injuryType)
        {
            bool dodge = NeptuneBattle.Instance.Numeric.SkillDodge(this, target, injuryType);
            if (dodge)
            {
                target.OnDodge(this);
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("{0}: {1} --> {2} (MISS)", this.Caster.FullName, this.FullName(), target.FullName);
#endif
                if (NeptuneBattle.Instance.Scene != null && target.Joint != null)
                {
                    target.Joint.OnDodge();
                    NeptuneBattle.Instance.Scene.PopupText(PopupType.Dodge, string.Empty, 0, target.Joint, this.Caster.Joint, false, target.Side);
                }
            }
            return dodge;
        }

        public virtual int CalculateInjury(InjuryType injuryType, RoleAttribute attrType, Actor target, float force, Actor from, ref int critCount)
        {
            return target.CalculateInjury(injuryType, attrType, force, from, this, ref critCount);
        }

        public void OnHPChanged(float currentHP, float maxHP)
        {
            if ((this.Data.TriggerType & TriggerType.HPLostPerPct) > 0)
            {//生命减少每百分比检测
                if (currentHP > this.KeepHp)
                    this.KeepHp = currentHP;

                if (this.KeepHp - currentHP >= maxHP * this.Data.TriggerParam * 0.01f)
                {
                    if (!this.Caster.AbilityEffects.Value.MindChain)
                    {
                        this.OnHitFrame();
                    }
                    this.KeepHp = currentHP;
                }
            }
        }

        public bool CreateSkillAbilitis(Actor target, AbilityTriggerType triggerType, SkillHitType hitType = SkillHitType.SkillHit)
        {
            bool abilityMiss = false;
            int abilityCount = this.Data.Abilities.Count;
            Ability addAbility = null;
            for (int i = 0; i < abilityCount; i++)
            {
                AbilityData ability = this.Data.Abilities[i];
                addAbility = null;
                if (ability.TriggerType != triggerType)
                {
                    continue;
                }
                if (ability.TargetType == TargetType.Target)
                {//目标型Ability
                    bool resist = false;
                    bool bSucess = true;
                    if (target == null || (target.IsDead && !ability.CanAddOnDeath))
                        bSucess = false;
                    else
                    {

                        if (ability.TriggerType == AbilityTriggerType.Hit)
                        {
                            if ((int)ability.TargetSide * (int)this.Caster.GetRelation(target) < 0)
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
                                        float talentLevel = this.Level;
                                        if (talentLevel < 30)
                                        {
                                            talentLevel = 10 + talentLevel / 30f * 20f;
                                        }
                                        factor = (float)(factor * Util.Random.Rand());
                                        if (target.Level > (talentLevel + factor))
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
                                            if (rand < (float)target.AttributeFinalValue[(int)attribName] / 100)
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
                                if (bSucess && target.AbilityEffects.Value.MindGain && ability.ControlEffects != null)
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
                        addAbility = target.AddAbility(ability, this.Caster, this);
                    }
                    else
                    {
                        abilityMiss = true;
                        if (NeptuneBattle.Instance.Scene != null && target != null && target.Joint != null)
                        {
                            NeptuneBattle.Instance.Scene.PopupText(resist ? PopupType.Resistance : PopupType.Miss, string.Empty, 0, target.Joint, this.Caster.Joint, false, target.Side);
                        }
                    }
                }
                else if (ability.TargetType == TargetType.Self)
                {//自身Ability
                    addAbility = this.Caster.AddAbility(ability, this.Caster, this);
                }
                //子弹飞行距离对buff时间的影响
                if (addAbility != null && this.Data.EffectRelatedAbilityID == addAbility.AbilityData.ID && ((hitType & SkillHitType.EffectHit) > 0 || (hitType & SkillHitType.EffectEnd) > 0))
                {
                    float extendTime = Mathf.Min(UFloat.Round(this.castFlyTime * this.Data.EffectExtendAbilityTimeRatio), this.Data.MaxEffectExtendAbilityTime);
                    addAbility.Duration = UFloat.Round(addAbility.Duration + extendTime);
                }
            }
            return abilityMiss;
        }

        public void PlayExtraEffects(EffectPlayType type, Vector2 position)
        {
            if (NeptuneBattle.Instance.Scene == null) return;
            if (this.Caster.Joint != null && this.Caster.Joint.Controller != null)
            {
                if (!((IActorController)this.Caster.Joint.Controller).InSight)
                    return;
            }
            if (this.Data.ExtraEffects != null && this.Data.ExtraEffects.Count > 0)
            {

                foreach (EffectData eff in this.Data.ExtraEffects)
                {
                    if (eff.Type == type)
                    {
                        Vector2 pos = NeptuneConst.Vector2Zero;

                        if (eff.RefPos == EffectPosRefType.Caster)
                        {
                            pos = this.Caster.Position;
                        }
                        if (eff.RefPos == EffectPosRefType.Target)
                        {
                            pos = this.Target.Position;
                        }

                        if (eff.RefPos == EffectPosRefType.Default)
                        {
                            pos = position;
                        }
                        pos = pos + new Vector2(eff.X, eff.Y);


                        Vector2 scale = NeptuneConst.Vector2Zero;
                        if (eff.RefDir != EffectDirRefType.None)
                        {
                            Actor effObj = eff.RefDir == EffectDirRefType.Caster ? this.Caster : this.Target;
                            scale = NeptuneConst.Vector2Zero;
                        }

                        NeptuneBattle.PlayEffect(eff.Name, pos, this.Caster.Orientation, 0, eff.Z, null, null, this.Caster);
                    }
                }
            }
        }

        public string FullName()
        {
            return "[" + this.Data.ID + "]" + (!string.IsNullOrEmpty(this.GroupData.DisplayName) ? this.GroupData.DisplayName : this.Data.SkillName) + "[" + this.Level + "]";
        }

        ///
        protected void RandomHitTarget(int indexNum)
        {
            List<Actor> survivorsList = NeptuneBattle.Instance.GetSurvivors(this.Caster, this.AffectedSide).Cast<Actor>().ToList();
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
                this.HitTarget(survivorsList[v - 1]);
            }
        }

        protected void CreateSummon(RoleInfo heroData, RoleConfig config, UVector2 pos)
        {
            Actor summon = ObjectFactory.Create(heroData, Caster.Side, config, null, pos, this.Caster.Orientation);
            summon.PlayEffectOnDeath = true;
            summon.Orientation = Caster.Orientation;
            NeptuneBattle.Instance.AddActor(summon, Caster, pos, RoleStateName.Instance[RoleState.Birth]);
        }




        public virtual void OnDestroyed()
        {
        }

        protected Actor ExSkillRoleCreate(RoleInfo heroData, RoleConfig config, int tid, int level, float hpFactor = -1)
        {
            if (heroData == null)
            {
                heroData = new RoleInfo();
            }
            config.IsMonster = true;
            config.PredictQuality = true;
            config.HPFactor = hpFactor < 0 ? this.Caster.Config.HPFactor : hpFactor;
            heroData.tid = tid;
            heroData.level = level;
            return ObjectFactory.Create(heroData, this.Caster.Side, config, null, UVector3.zero, this.Caster.Orientation);
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
            if (this.Data.ProtectTime > 0)
            {
                if (CastDuration > this.Data.ProtectTime)
                {
                    return false;
                }
            }
            else if (!this.hasSkillCasting)
            {
                return false;
            }
            return true;
        }

        public virtual bool IsCastingProtectEx()
        {
            if (this.Data.SkillProtectTime > 0)
            {
                if (CastDuration <= this.Data.SkillProtectTime)
                {
                    return true;
                }
            }
            return false;
        }
        public bool CheckCastingProtectEx()
        {
            return this.Data.SkillProtectTime > 0 && !IsCastingProtectEx();
        }

        public bool CheckInterruptType(Skill talent)
        {
            if (talent == null)
                return false;
            if (this.Data.InterruptSkillType == InterruptSkillType.ALL)
            {
                return true;
            }
            if (this.Data.InterruptSkillType == InterruptSkillType.Normal && talent.Data.SkillType == SkillType.Normal)
            {
                return true;
            }
            if (this.Data.InterruptSkillType == InterruptSkillType.Skill && (talent.Data.SkillType == SkillType.Skill || talent.Data.SkillType == SkillType.ChildSkill))
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
            if (this.Data.ExtraDamageRatioByRoleType == null)
            {
                return 0;
            }
            if (this.Data.ExtraDamageRatioByRoleType.Length % 2 > 0)
            {
                Logger.LogError("this.Data.ExtraDamageRatioByRoleType.Length " + this.Data.ExtraDamageRatioByRoleType.Length);
                return 0;
            }
            List<int> list = this.Data.ExtraDamageRatioByRoleType.ToList();
            for (int i = 0; i < list.Count; i += 2)
            {
                if ((roleType & (RoleType)list[i]) == (RoleType)list[i])
                {
                    return list[i + 1];
                }
            }
            return 0;
        }

        private void CalculateTargetExtraInjury(Actor fromRole, Actor targetRole, Skill fromSkill, List<TargetExtraDamageData> extraDamageData, ref int critCount)
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

                float extraDamage = UFloat.Round(fromRole.AttributeFinalValue[(int)fromAttribute] * transRatio * NeptuneConst.Hundredth);

                targetRole.CalculateInjury(toInjuryType, RoleAttribute.MaxHP, extraDamage, fromRole, fromSkill, ref critCount);
            }

        }

        /// <summary>
        /// 激活类技能启动
        /// </summary>
        private void StartActivate()
        {
            this.IsActivated = true;
        }
        /// <summary>
        /// 激活类技能结束
        /// </summary>
        public void EndActivate()
        {
            if (this.Data.ContinuousType == ContinuousType.Activate && this.IsActivated)
            {
                ClearCastingTrap(this.Caster.IsDead);
                this.IsActivated = false;
                CastDuration = 0;
            }
        }


        public override string ToString()
        {
            return this.FullName() + string.Format(" CD:{0:f6} CastCD:{1:f6}", this.Duration, this.CastDuration);
        }
        public virtual bool CanInterrupt(InterruptType interruptType)
        {
            return (this.Data.InterruptType & interruptType) > 0;
        }

        protected virtual void CostMp(CostMPMode costmode)
        {
            if (this.Data.CostMPMode != costmode)
                return;
            this.Caster.SetCostValue(this.Caster.GetCurrentCostValue() - UFloat.Round(this.Data.CostMP * (1 - this.Caster.AttributeFinalValue.ManaCostReduction / 100)));

        }
        /// <summary>
        /// AOE类型的技能附加范围检测
        /// </summary>
        /// <param name="target"></param>
        /// <param name="hitpos"></param>
        /// <param name="orientation"></param>
        /// <param name="arg"></param>
        /// <returns></returns>

        protected virtual bool HitCheckAffiliatedAreaShape(Actor target, UVector2 hitpos, UVector2 orientation, UVector2 arg)
        {
            if (target == null || this.Data.AffiliatedAreaShape == AreaShape.None)
                return false;
            UVector2 pos = hitpos + this.Data.AffiliatedAreaShift * orientation;
            UVector2 distance = target.Position - pos;
            return EngineUtil.HitTest(distance, this.Data.AffiliatedAreaShape, new UVector2(arg.x + this.Data.AffiliatedAreaArg1, arg.y + this.Data.AffiliatedAreaArg2),
                        orientation);

        }
        public int GethitNum()
        {
            return this.hitTargetNum;
        }

        public bool CanBreak(Skill castSkill)
        {
            return (castSkill.Data.ForcedInterrupt || (this.CanInterrupt(InterruptType.Active) && this.CheckInterruptType(castSkill)) || this.CheckCastingProtectEx());
        }

        public void AddMarkCount(int key)
        {
            if (this.Data.MaxHitAddMarkCount != null && this.Data.MaxHitAddMarkCount.Length > 0)
            {
                if (this.addMarkHitCount == null)
                {
                    this.addMarkHitCount = new Dictionary<int, Dictionary<int, int>>();
                }
                for (int i = 0; i < this.Data.MaxHitAddMarkCount.Length; i += 2)
                {
                    int markid = this.Data.MaxHitAddMarkCount[i];
                    int count = this.Data.MaxHitAddMarkCount[i + 1];
                    if (!this.addMarkHitCount.ContainsKey(key))
                    {
                        this.addMarkHitCount.Add(key, new Dictionary<int, int>());
                        if (!this.addMarkHitCount[key].ContainsKey(markid))
                            this.addMarkHitCount[key].Add(markid, count);
                    }
                }



            }

        }

        private bool checkAddMarkCount(int key, int markid)
        {
            if (this.addMarkHitCount == null || !this.addMarkHitCount.ContainsKey(key) || !this.addMarkHitCount[key].ContainsKey(markid))
            {
                return true;
            }
            if (this.addMarkHitCount[key][markid] > 0)
            {
                this.addMarkHitCount[key][markid]--;
                return true;
            }
            return false;
        }

        public void SetDamageFactor(float dmgFactorPercent)
        {
            this.damageFactor = Math.Max(0, UFloat.Round(this.damageFactor + dmgFactorPercent));
        }
        public void ResetModifyData()
        {

        }
    }
}