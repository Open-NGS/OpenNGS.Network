using UnityEngine;
using System.Collections.Generic;
using System;
using Neptune.GameData;
using Neptune;

/// <summary>
/// 陷阱：在一定范围内持续存在，并能在不同阶段对范围内的目标释放不同的技能
/// </summary>
public class BattleTrap: BattleEffect
{
    /// <summary>
    /// 触发该陷阱的技能
    /// </summary>
    //private Skill Skill;

    /// <summary>
    /// 陷阱生效的队伍 0 - 全部； 1 - 友方； -1 - 敌方 
    /// </summary>
    private RelativeSide affectSide;

    /// <summary>
    /// 陷阱持续时间
    /// </summary>
    private int duringTime;

    /// <summary>
    /// 陷阱形状
    /// </summary>
    private AreaShape shapeType;

    /// <summary>
    /// 无视陷阱形状
    /// </summary>
    private AreaShape ignoreShapeType;

    /// <summary>
    /// 圆形陷阱的半径，矩形陷阱的长度，或扇形陷阱的半径
    /// </summary>
    private int shapeParam1;

    /// <summary>
    /// 矩形陷阱的宽度
    /// </summary>
    private int shapeParam2;

    /// <summary>
    /// 忽视圆形陷阱的半径，矩形陷阱的长度，或扇形陷阱的半径
    /// </summary>
    private int ignoreShapeParam1;

    /// <summary>
    /// 忽视矩形陷阱的宽度
    /// </summary>
    private int ignoreShapeParam2;

    /// <summary>
    /// 区域的倍率
    /// </summary>
    private float shapeChangeRatio;

    /// <summary>
    /// 忽视区域的倍率
    /// </summary>
    private int ignoreShapeChangeRatio;

    /// <summary>
    /// 区域的时间
    /// </summary>
    private float shapeChangeTimeDelta;

    private float shapeChangeTime;

    /// <summary>
    /// 忽视区域的时间
    /// </summary>
    private float ignoreShapeChangeTime;
    private float ignoreShapeChangeTimeDelta;

    /// <summary>
    /// 跟随类型： 0-不跟随，自主移动； 1-跟随目标， 2-跟随释放者
    /// </summary>
    private int followType;

    /// <summary>
    /// 陷阱特效ID
    /// </summary>
    private string effectName;

    public String EffectName { get { return this.effectName; } }

    private int zorder;
    public int ZOrder { get { return this.zorder; } set { this.zorder = value; } }

    /////// 关联技能属性 ///////
    private int startAction;
    private TargetType targetType;
    private int enterAction;
    private int triggerAction;
    private int effectAction;
    private float triggerInterval;
    private int leaveAction;
    private int endAction;
    private int manualAction;
    private string endEffect;

    // Internal variables
    public int trapId;
    private bool started = false;
    private float startDelay;
    private float lifeTime;
    private float triggerTimer;
    private List<int> RemoveAbilitys;
    private string triggerSound;
    private string startSound;
    private string endSound;
    private BattleActor targetRole;
    private Vector2 posOffset;
    public Vector2 PosOffset
    {
        get { return posOffset; }
    }
    //private List<Actor> startAffectRoleList;
    private BattleSkill startTalent;
    private List<BattleActor> inRangeRoleList;
    private BattleSkill enterTalent;
    private BattleSkill leaveTalent;
    private BattleSkill triggerTalent;
    private BattleSkill effectTalent;
    private List<BattleActor> leaveAffectRoleList;

    private bool interruptEnd;
    private bool casterDeadEnd;
    private float TrapSpeed = 0;
    private float TrapAngle = 0;

    private LimitTriggerType limitTriggerType;
    private int enterLimitTimes;
    private int leaveLimitTimes;
    private int intervalLimitTimes;
    private int manualLimitTimes;
    private bool IsShowIndicator;
    public TrapData TrapData;
    private int OrcaObstacleRadius;
    private TriggerType triggerType;
    private int triggerParam;
    private int triggerParam1;
    private Vector2 positionOffset;
    Action _act;
    public List<int> obstacleIds = null;
    private List<List<UVector2>> orcaObstacleList = new List<List<UVector2>>();

    //环绕子弹成员
    private float intervalTime = 0;
    private List<BattleEffect> casts = null;
    private float effectInterval = 0;

    public BattleTrap()
    {

    }

    public void Create(int trapId, BattleSkill talent, Vector2 position, BattleActor target = null)
    {
        base.Create();

        this.Type = EffectType.Trap;
        this.Talent = talent;
        this.Side = talent.Caster.Side;
        this.trapId = trapId;
        this.Position = position;
        targetRole = target;
        this.Target = target;
        this.TrapData = NeptuneBattle.Instance.DataProvider.GetTrapData(trapId).Clone();
        this.Talent.Caster.RoleSkin.RoleSkinTrapReplace(this.TrapData);
        this.affectSide = TrapData.AffectSide;
        this.lifeTime = UFloat.Round(TrapData.DuringTime / 1000f);
        this.shapeType = TrapData.ShapeType;
        this.ignoreShapeType = TrapData.IgnoreShapeType;
        this.shapeParam1 = TrapData.ShapeParam1;
        this.shapeParam2 = TrapData.ShapeParam2;
        this.ignoreShapeParam1 = TrapData.IgnoreShapeParam1;
        this.ignoreShapeParam2 = TrapData.IgnoreShapeParam2;
        this.shapeChangeRatio = UFloat.Round(TrapData.ShapeChangeRatio * EngineConst.Hundredth);
        this.ignoreShapeChangeRatio = TrapData.IgnoreShapeChangeRatio;
        this.shapeChangeTime = UFloat.Round(TrapData.ShapeChangeTime * EngineConst.Milli);
        this.shapeChangeTimeDelta = this.shapeChangeTime;
        this.ignoreShapeChangeTime = UFloat.Round(TrapData.IgnoreShapeChangeTime * EngineConst.Milli);
        this.ignoreShapeChangeTimeDelta = this.ignoreShapeChangeTime;
        this.followType = TrapData.FollowType;
        this.effectName = TrapData.EffectName;
        this.zorder = TrapData.EffectZOrder;
        this.OrcaObstacleRadius = TrapData.OrcaObstacleRadius;
        TrapSpeed = TrapData.Speed;
        TrapAngle = TrapData.OffsetAngle;
        Vector2 dir = new Vector2(TrapData.DirectionX, TrapData.DirectionY);
        if (dir != EngineConst.Vector2Zero)
            this.Orientation = dir;
        else
            this.Orientation = this.Talent.Caster.Orientation;
        this.startDelay = UFloat.Round(TrapData.StartDelay / 1000f);
        this.startAction = TrapData.StartAction;
        this.enterAction = TrapData.EnterAction;
        this.triggerAction = TrapData.TriggerAction;
        this.triggerInterval = UFloat.Round(TrapData.TriggerInterval / 1000f);
        this.leaveAction = TrapData.LeaveAction;
        this.endAction = TrapData.EndAction;
        this.endEffect = TrapData.EndEffect;
        this.manualAction = TrapData.ManualAction;
        this.targetType = TrapData.TargetType;
        this.interruptEnd = TrapData.InterruptEnd;
        this.casterDeadEnd = TrapData.CasterDeadEnd;
        this.effectAction = TrapData.EffectAction;
        this.limitTriggerType = (LimitTriggerType)TrapData.LimitTriggerType;
        this.enterLimitTimes = TrapData.LimitTimes;
        this.leaveLimitTimes = TrapData.LimitTimes;
        this.intervalLimitTimes = TrapData.LimitTimes;
        this.manualLimitTimes = TrapData.LimitTimes;
        this.RemoveAbilitys = TrapData.RemoveAbilitys;
        this.endSound = TrapData.EndSound;
        this.triggerSound = TrapData.TriggerSound;
        this.startSound = TrapData.StartSound;
        this.IsShowIndicator = TrapData.IsShowIndicator;
        this._act = null;
        this.triggerTalent = null;
        this.triggerType = TrapData.EnterTriggerType;
        this.triggerParam = TrapData.TriggerParam;
        this.triggerParam1 = TrapData.TriggerParam1;
        this.effectInterval = UFloat.Round(TrapData.EffectInterval * EngineConst.Milli);
        this.positionOffset = this.TrapData.PosOffset != null && this.TrapData.PosOffset.Count > 1 ? new Vector2(this.TrapData.PosOffset[0], this.TrapData.PosOffset[1]) : EngineConst.Vector2Zero;
        this.Height = this.TrapData.PosOffset != null && this.TrapData.PosOffset.Count > 2 ? this.TrapData.PosOffset[2] : 0;
        this.effectTalent = null;
        triggerTimer = 0;
        inRangeRoleList = new List<BattleActor>();
        started = false;
        if (this.effectAction != null)
            this.casts = new List<BattleEffect>();
        else
            this.casts = null;
        this.AddToCaster();
        this.Source = talent.Caster;

#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("{0}: {1} --> {2} crete Trap {3}  startAction:{4}", this.Talent.Caster.FullName, this.ToString(), this.Target != null ? this.Target.FullName : "", this.trapId, this.startAction);
#endif
    }

    public override void Delete()
    {
        NObjectPool<BattleTrap>.Delete(this);
    }

    public override void OnEnterFrame(float dt)
    {
        if (!this.Running)
        {
            //            Logger.LogWarning("Trap already stopped, should not be updated, TrapId: " + trapId);
            return;
        }

        if (!started)
        {
            if (this.startDelay <= 0)
            {
                OnStart();
            }
            else
            {
                this.startDelay = this.startDelay - dt;
            }
            return;
        }

        UpdatePosition(dt);
        UpdateArea(dt);
        DetectTargetsInRange();

        PerformTriggerAction(dt);
        updateEffect(dt);
        CheckLifeTime(dt);
    }

    // 打断陷阱，返回陷阱是否已经被终止
    public bool Interrupt(bool casterDead = false)
    {
        if (this.Running == false)
        {
            // 陷阱已经正常终止
            return true;
        }
        // 技能被打断，陷阱将被中断，并且不触发EndAction

        if ((casterDead && this.casterDeadEnd) || this.interruptEnd)
        {
            this.OnStop(!this.TrapData.CloseEndAction);
            return true;
        }

        return false;
    }

    private void OnStart()
    {
        started = true;

        // 通知表现层创建陷阱特效
        CreateTrapEffect();

        RecordPositionOffset();
        if (this.OrcaObstacleRadius > 0)
            GenericObstacleWall(this.Position, this.OrcaObstacleRadius);
        // 释放开始技能
        PerformStartAction();
        createRoundEffect();
    }

    // 清除StartAction和EnterAction，释放EndAction
    private void OnStop(bool endAction = true)
    {
        ClearActions();
        
        if (endAction)
            PerformEndAction();
        // 在陷阱中心点播放结束特效
        if (!string.IsNullOrEmpty(this.endEffect))
        {
            IBattleAgent effectJoint = NeptuneBattle.PlayEffect(this.endEffect, this.Position, this.Orientation, this.Height, this.ZOrder, null, null, this.Talent.Caster);
            //             if (!string.IsNullOrEmpty(EndSound) && effectJoint != null)
            //             {
            //                 effectJoint.PlaySound(EndSound);
            //             }
        }
        if (!string.IsNullOrEmpty(endSound) && this.joint != null)
        {
            this.joint.PlaySound(endSound);
        }
        this.Stop();

        // 从主技能中删除对该陷阱的引用
        if (this.Talent != null)
        {
            this.Talent.ClearCastingTrap();
        }
    }

    private void CreateTrapEffect()
    {
        if (!this.TrapData.StartDontSetOrientation)
            this.Orientation = Talent.Caster.Orientation;
        this.Speed = Quaternion.AngleAxis(this.TrapAngle, Vector3.back) * this.Orientation.normalized * this.TrapSpeed;

        if (positionOffset != EngineConst.Vector2Zero)
        {
            positionOffset = this.positionOffset.x * this.Orientation + this.positionOffset.y * MathUtil.GetRight(this.Orientation);
            this.Position += positionOffset;
        }
        if (string.IsNullOrEmpty(this.effectName))
        {
            return;
        }
        //this.joint = Logic.PlayEffect(this.effectName, this.Position, this.Orientation, 0, this.ZOrder, this);
        this.joint = NeptuneBattle.PlayEffect(this.effectName, this.Position, this.Orientation, this.Height, this.ZOrder, this, OnControllerCreate);
        if (!string.IsNullOrEmpty(startSound) && this.joint != null)
        {
            this.joint.PlaySound(startSound);
        }
    }

    private void OnControllerCreate(IEffectController go)
    {
        go.SetEffect(this);
        go.Direction = this.Orientation;
        if (go == null || shapeType != AreaShape.Circle || !IsShowIndicator) return;
        if (TrapData.ShowIndicatorSide != RelativeSide.Both && NeptuneBattle.Instance.RuleManager != null && TrapData.ShowIndicatorSide != this.Talent.Caster.GetRelation(NeptuneBattle.Instance.RuleManager.GetCurrentPlayer().CurrentRole))
            return;
        int type = 0;
        List<float> indicatorParams = new List<float>();
        indicatorParams.Add(shapeParam1);
        indicatorParams.Add(shapeParam2);
        if (shapeType == AreaShape.Circle)
        {
            type = 1;
        }
        else if (shapeType == AreaShape.Rectangle)
        {
            type = 5;
        }
        else if (shapeType == AreaShape.Sector || shapeType == AreaShape.Quadrant || shapeType == AreaShape.SemiCircle)
        {
            type = 4;
        }
        go.AddFloorEffect(Talent.Caster, type, indicatorParams);
    }

    // 如果需要跟随，计算当前位置与跟随目标的偏移量
    private void RecordPositionOffset()
    {
        switch (this.followType)
        {
            case 1:
                // 跟随目标
                if (targetRole != null)
                    this.posOffset = this.Position - targetRole.Position;

                break;
            case 2:
                // 跟随释放者
                this.posOffset = this.Position - this.Talent.Caster.Position;
                break;
            default:
                break;
        }
    }

    private void UpdatePosition(float dt)
    {
        switch (this.followType)
        {
            case 0:
                // 不跟随，自主移动
                this.Position += this.Speed * dt;
                break;
            case 1:
                // 跟随目标
                if (targetRole != null)
                    this.Position = targetRole.Position + posOffset;
                break;
            case 2:
                // 跟随释放者
                this.Position = this.Talent.Caster.Position + posOffset;
                break;
        }

        // 设置特效的位置
    }

    private void PerformStartAction()
    {
        // 释放StartAction
        if (this.startAction == 0)
        {
            return;
        }

        startTalent = this.Talent.Caster.GetTalentById(startAction);
        if (startTalent == null)
        {
            Logger.LogError("No Start Skill found from Active Skill List of Caster, ID: " + startAction);
            this.Running = false;
            return;
        }
        CastTrapTalent(startTalent);
    }

    // 检测陷阱范围内的角色变化，并对有变化的角色应用Enter或Leave技能
    private void DetectTargetsInRange()
    {
        bool hasChange = false;
        BattleSkill enterTalent = null;
        List<BattleActor> newList = new List<BattleActor>();
        //        foreach (Actor role in Logic.Instance.GetSurvivors(this, this.affectSide, shapeParam1+ shapeParam2 + 500))
        foreach (BattleActor role in NeptuneBattle.Instance.AOIManager.GetSurvivors(this, this.affectSide, shapeParam1 + shapeParam2 + 500))
        {
            if (this.TrapData.SelfTrigger && role != this.Talent.Caster)
                continue;
            if (!CheckTriggerType(role))
                continue;
            if (WithinArea(role) && ((role.Data.RoleType & this.TrapData.RoleType) > 0 || this.TrapData.RoleType == RoleType.Any))
            {
                newList.Add(role);
                if (!inRangeRoleList.Contains(role))
                {
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("Trap[{0}] RoleEnter {1}", this.trapId, role.FullName);
#endif
                    // 新进入陷阱范围的角色
                    enterTalent = OnEnterRange(role);
                    hasChange = true;
                    if ((limitTriggerType & LimitTriggerType.Enter) > 0 && (--enterLimitTimes) <= 0)
                    {
                        this.OnStop(!this.TrapData.CloseEndAction);
                        break;
                    }
                }
            }
        }

        foreach (BattleActor role in inRangeRoleList)
        {
            if (!newList.Contains(role))
            {
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("Trap[{0}] RoleLeave {1}", this.trapId, role.FullName);
#endif
                // 角色移动出了陷阱范围
                OnLeaveRange(role);
                hasChange = true;
                if ((limitTriggerType & LimitTriggerType.Leave) > 0 && (--leaveLimitTimes) <= 0)
                {
                    this.OnStop(!this.TrapData.CloseEndAction);
                    break;
                }
            }
        }


        if (hasChange)
        {
            inRangeRoleList.Clear();
            inRangeRoleList = newList;
        }
    }

    // 检测和释放周期触发技能
    private void PerformTriggerAction(float dt)
    {
        if (triggerAction == 0)
        {
            // No Trigger Action Configured
            return;
        }

        triggerTimer += dt;
        if (triggerTimer >= triggerInterval)
        {
            triggerTimer = UFloat.Round(triggerTimer - triggerInterval);

            if (triggerTalent == null)
            {
                triggerTalent = this.Talent.Caster.GetTalentById(triggerAction);
                if (triggerTalent == null)
                {
                    Logger.LogError("No Trigger Skill found from Active Skill List of Caster, ID: " + triggerAction);
                    return;
                }
            }

            CastTrapTalent(triggerTalent);
            if (!string.IsNullOrEmpty(triggerSound) && this.joint != null)
            {
                this.joint.PlaySound(triggerSound);
            }
            if ((limitTriggerType & LimitTriggerType.Interval) > 0 && (--intervalLimitTimes) <= 0)
            {
                this.OnStop(!this.TrapData.CloseEndAction);
            }
        }
    }

    public void OnEndAct(Action act)
    {
        _act = act;
    }

    // 对范围内的目标施加EndAction
    private void PerformEndAction()
    {
        if (_act != null)
        {
            _act();
        }

        if (endAction == 0)
        {
            return;
        }

        BattleSkill endTalent = this.Talent.Caster.GetTalentById(endAction);
        if (endTalent == null)
        {
            Logger.LogError("No Trigger Skill found from Active Skill List of Caster, ID: " + endAction);
            return;
        }

        CastTrapTalent(endTalent);


    }

    // TODO: 清除已经释放的技能中的buff的影响
    private void ClearActions()
    {

    }

    private BattleSkill OnEnterRange(BattleActor role)
    {
        if (enterAction == 0)
        {
            return null;
        }

        BattleSkill enterTalent = Talent.Caster.GetTalentById(enterAction);
        if (enterTalent == null)
        {
            Logger.LogError("No Enter Skill Found From Caster Skill list, ID: " + enterAction);
            return null;
        }

        ResultType result = enterTalent.CanUse(role);
        if (result == ResultType.Success || result == ResultType.TooFar || result == ResultType.TooNear || result == ResultType.OutOfSightRange)
        {
            if (this.TrapData.EffectPointType == EffectPointType.TrapPoint)
                enterTalent.AttackPointElement = this;
            if (enterTalent.Data.TargetType == TargetType.Position)
            {
                enterTalent.Start(this.Position);
            }
            else
                enterTalent.Start(role);
        }
        return enterTalent;
    }

    private bool CheckTriggerType(BattleActor role)
    {
        if (role == null)
            return false;
        if (this.triggerType == 0)
        {
            return true;
        }
        if ((this.triggerType & TriggerType.HPUnder) > 0 && role.HP <= UFloat.Round(role.MaxHP * (this.triggerParam * EngineConst.Hundredth)))
        {
            return true;
        }
        if ((this.triggerType & TriggerType.MpUnder) > 0 && role.MP <= UFloat.Round(role.MaxMP * (this.triggerParam1 * EngineConst.Hundredth)) && role.MaxMP > 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 给离开陷阱的角色清除之前施加的Buff，如果LeaveAction配置了和之前的某个Action一样，表示保留这个Action产生的Buff
    /// 但是不会再次触发该Action的伤害技能
    /// </summary>
    /// <param name="role"></param>
    private void OnLeaveRange(BattleActor role)
    {
        if (this.RemoveAbilitys != null)
        {
            int actionID = 0;
            for (int i = 0; i < this.RemoveAbilitys.Count; i++)
            {
                actionID = this.RemoveAbilitys[i];
                if (actionID == startAction && startTalent != null)
                {
                    role.RemoveTalentAbility(startTalent);
                }
                if (actionID == enterAction && enterTalent != null)
                {
                    role.RemoveTalentAbility(enterTalent);
                }
                if (actionID == triggerAction && triggerTalent != null)
                {
                    role.RemoveTalentAbility(triggerTalent);
                }
            }
        }
        if (leaveAction == 0)
        {
            return;
        }

        BattleSkill leaveTalent = Talent.Caster.GetTalentById(leaveAction);
        if (leaveTalent == null)
        {
            Logger.LogError("No Enter Skill Found From Caster Skill list, ID: " + enterAction);
            return;
        }
        ResultType result = leaveTalent.CanUse(role);
        if (result == ResultType.Success || result == ResultType.TooFar || result == ResultType.TooNear || result == ResultType.OutOfSightRange)
        {
            if (this.TrapData.EffectPointType == EffectPointType.TrapPoint)
                leaveTalent.AttackPointElement = this;
            if (leaveTalent.Data.TargetType == TargetType.Position)
            {
                leaveTalent.Start(this.Position);
            }
            else
                leaveTalent.Start(role);
        }
    }

    // 清除角色身上开始技能施加的Buff
    private void ClearStartAction(BattleActor role)
    {
        if (startTalent == null)
        {
            return;
        }

        role.RemoveTalentAbility(startTalent);
    }

    // 清除角色身上进入技能施加的Buff
    private void ClearEnterAction(BattleActor role)
    {
        if (enterTalent == null)
        {
            return;
        }

        role.RemoveTalentAbility(enterTalent);
    }

    // 清除角色身上周期技能施加的Buff
    private void ClearTriggerAction(BattleActor role)
    {
        if (triggerTalent == null)
        {
            return;
        }

        role.RemoveTalentAbility(triggerTalent);
    }

    private void CheckLifeTime(float dt)
    {
        this.lifeTime = UFloat.Round(this.lifeTime - dt);
        if ((this.followType == 1 && (this.Target == null || this.Target.IsDead)) || (this.followType == 2 && (this.Talent.Caster == null || this.Talent.Caster.IsDead)))
        {
            //当跟踪目标死亡时 结束陷阱
            this.lifeTime = 0;
        }
        if (this.lifeTime <= 0.0 && this.Talent.Data.ContinuousType != ContinuousType.Activate)
        {
            this.OnStop();
        }
    }

    // 查看目标角色是否在该陷阱的影响范围内
    private bool WithinArea(BattleActor role)
    {
        bool result = false;
        UVector2 pos = role.Position - this.Position;
        UVector2 arg1 = new UVector2(shapeParam1 + role.Radius, shapeParam2);
        //是否在计算区域中
        bool result1 = EngineUtil.HitTest(pos, this.shapeType, arg1, this.Orientation);
        UVector2 arg2 = new Vector2(ignoreShapeParam1 + role.Radius, ignoreShapeParam2);
        //是否在无视区域中
        bool result2 = false;
        if (this.ignoreShapeType != AreaShape.None)
        {
            result2 = EngineUtil.HitTest(pos, this.ignoreShapeType, arg2, this.Orientation);
        }
        if (result1 && !result2)
        {
            result = true;
        }
        return result;
    }

    /// <summary>
    /// 计算区域大小
    /// </summary>
    /// <param name="dt"></param>
    public void ChangeAreaSize(float dt)
    {
        if (this.shapeChangeTimeDelta == 0)
        {
            return;
        }
        this.shapeChangeTimeDelta -= dt;
        float ratio = UFloat.Round(Mathf.Lerp(this.shapeChangeRatio, 1.0f, UFloat.Round(this.shapeChangeTimeDelta / this.shapeChangeTime)));
        if (this.shapeType != AreaShape.None)
        {
            this.shapeParam1 = UFloat.RoundToInt(this.TrapData.ShapeParam1 * ratio);
        }
        if (this.shapeType == AreaShape.Rectangle)
        {
            this.shapeParam2 = UFloat.RoundToInt(this.TrapData.ShapeParam2 * ratio);
        }
        if (this.shapeChangeTimeDelta < 0)
        {
            this.shapeChangeTimeDelta = 0;
        }
    }

    /// <summary>
    /// 计算无视区域大小
    /// </summary>
    /// <param name="dt"></param>
    public void ChangeIgnorAreaSize(float dt)
    {
        if (this.ignoreShapeChangeTimeDelta == 0)
        {
            return;
        }
        this.ignoreShapeChangeTimeDelta -= dt;
        float ignoreRatio = UFloat.Round(Mathf.Lerp(this.shapeChangeRatio, 1.0f, UFloat.Round(this.ignoreShapeChangeTimeDelta / this.ignoreShapeChangeTime)));
        if (this.ignoreShapeType != AreaShape.None)
        {
            this.ignoreShapeParam1 = UFloat.RoundToInt(this.TrapData.IgnoreShapeParam1 * ignoreRatio);
        }
        if (this.ignoreShapeType == AreaShape.Rectangle)
        {
            this.ignoreShapeParam2 = UFloat.RoundToInt(this.TrapData.IgnoreShapeParam2 * ignoreRatio);
        }
        if (this.ignoreShapeChangeTimeDelta < 0)
        {
            this.ignoreShapeChangeTimeDelta = 0;
        }

    }

    /// <summary>
    /// 区域逻辑更新
    /// </summary>
    /// <param name="dt"></param>
    public void UpdateArea(float dt)
    {
        ChangeAreaSize(dt);
        ChangeIgnorAreaSize(dt);
    }

    /// <summary>
    /// 陷阱说明性描述
    /// </summary>
    public override string FullName
    {
        get { return string.Format("[{0}{1}]{2}>Trap", this.SideSign, !this.Running ? "D" : "", this.Talent.FullName()); }
    }

    /// <summary>
    /// 停止陷阱
    /// </summary>
    public override void Stop()
    {
#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("{0}: {1} --> {2} stop Trap {3}  startAction:{4}", this.Talent.Caster.FullName, this.ToString(), this.Target != null ? this.Target.FullName : "", this.trapId, this.startAction);
#endif  
        this.RemoveObstacleWall();
        this.RemoveFromCaster();
        this.Running = false;
        if (this.joint != null)
        {
            this.joint.Stop();
        }
        if (this.casts != null && this.casts.Count > 0)
        {
            foreach (BattleEffect effect in this.casts)
            {
                if (effect != null && effect.Running)
                    effect.Stop();
            }
            this.casts.Clear();
        }
        this.casts = null;


    }

    private void CastTrapTalent(BattleSkill talent)
    {
        BattleActor target = null;
        if (this.targetType == TargetType.None)
        {
            target = this.Target;
        }
        else if( this.targetType == TargetType.Target)
        {
           target = this.Talent.Caster.Target;
        }
        else if (this.targetType == TargetType.Self)
        {
            target = this.Talent.Caster;
        }
        talent.CastPosition = this.Position;
        ResultType result = talent.CanUse(target, false, true);
        if (result == ResultType.MPNotEnough)
        {
            Talent.End();
            Talent.EndActivate();
        }
        else if (result == ResultType.Success || result == ResultType.TooFar || result == ResultType.TooNear || result == ResultType.OutOfSightRange)
        {
            talent.Orientation = this.Orientation;
            if (this.TrapData.EffectPointType == EffectPointType.TrapPoint)
                talent.AttackPointElement = this;
            if (talent.Data.TargetType != TargetType.Position && talent.Data.TargetType != TargetType.Direction)
            {
                talent.Start(target, this.Position);
            }
            else
            {
                talent.Start(this.Position);
            }

        }
    }
    /// <summary>
    /// 主动触发陷阱技能
    /// </summary>
    public void CastManualAction()
    {
        if (manualAction > 0)
        {
            BattleSkill manualTalent = Talent.Caster.GetTalentById(manualAction);
            if (manualTalent == null)
            {
                Logger.LogError("No manualTalent Found From Caster Skill list, ID: " + manualAction);
                return;
            }
            if (this.TrapData.UseTrapRange)
                manualTalent.MaxRange = this.shapeParam1;
            CastTrapTalent(manualTalent);
            if ((limitTriggerType & LimitTriggerType.Manual) > 0 && (--manualLimitTimes) <= 0)
            {
                this.OnStop();
            }
        }
    }

    private void GenericObstacleWall(UVector2 position, int radius)
    {
        List<UVector2> posList = EngineUtil.GenericObstaclePoints(position, radius, 16);
        this.orcaObstacleList.Add(posList);
        int obstacleId = NeptuneBattle.Instance.Simulator.AddObstacle(posList, OrcaObstacleStatus.DYNAMIC);
        if (this.obstacleIds == null)
            this.obstacleIds = new List<int>();
        this.obstacleIds.Add(obstacleId);
        posList = EngineUtil.GenericObstaclePoints(position, radius, 16, true);
        this.orcaObstacleList.Add(posList);
        obstacleId = NeptuneBattle.Instance.Simulator.AddObstacle(posList, OrcaObstacleStatus.DYNAMIC);
        this.obstacleIds.Add(obstacleId);
        NeptuneBattle.Instance.Simulator.ProcessObstacles(OrcaObstacleStatus.DYNAMIC);


    }

    private void RemoveObstacleWall()
    {
        if (this.obstacleIds == null)
            return;
        for (int i = 0; i < this.obstacleIds.Count; i++)
        {
            NeptuneBattle.Instance.Simulator.RemoveObstacle(this.obstacleIds[i]);
        }

        NeptuneBattle.Instance.Simulator.ProcessObstacles(OrcaObstacleStatus.DYNAMIC);
        this.obstacleIds = null;
        this.orcaObstacleList.Clear();
    }

    /// <summary>
    /// 陷阱产生的碰撞检测
    /// </summary>
    /// <param name="curPos"></param>
    /// <param name="nextPos"></param>
    /// <returns></returns>
    public UVector2 ObstacleWallHit(UVector2 curPos, UVector2 nextPos)
    {//TODO：  迁移到  OrcaSimulator中
        UVector2 hit = EngineConst.Vector2Zero;

        for (int i = 0; i < orcaObstacleList.Count; i++)
        {
            for (int j = 0; j < orcaObstacleList[i].Count; j++)
            {

                UVector2 dir;
                UVector2 pos;

                if (j < orcaObstacleList[i].Count - 1)
                {
                    dir = orcaObstacleList[i][j];
                    pos = orcaObstacleList[i][j + 1];
                }
                else
                {
                    dir = orcaObstacleList[i][j];
                    pos = orcaObstacleList[i][0];
                }
                hit = MathUtil.SegmentsIntr(curPos, nextPos, dir, pos);
                if (hit != UVector2.zero)
                {
                    return hit;
                }
            }
        }
        return hit;
    }

    private UVector2 nextPositon(float dt, ref int angle, int radius, int speed)
    {
        angle += UFloat.RoundToInt(speed * dt);
        return MathUtil.GetLocalRotatePositon(angle, radius);
    }


    private void createRoundEffect()
    {
        if (effectAction == 0)
        {
            return;
        }

        effectTalent = this.Talent.Caster.GetTalentById(effectAction);
        if (effectTalent == null)
        {
            Logger.LogError("No Trigger Skill found from Active Skill List of Caster, ID: " + effectAction);
            return;
        }


        if (effectTalent.Data.FlyCastDirections != null && effectTalent.Data.FlyCastDirections.Count > 0)
        {
            for (int i = 0; i < effectTalent.Data.FlyCastDirections.Count; i++)
            {
                int angle = effectTalent.Data.FlyCastDirections[i];
                BattleEffect cast = effectTalent.CreateCast();
                cast.SetFollowElement(this);
                cast.effectAngle = angle;
                cast.Position = nextPositon(0, ref cast.effectAngle, cast.CasterDistance, effectTalent.Data.FlyRoundSpeed) + this.Position;
                this.casts.Add(cast);
                NeptuneBattle.Instance.AddEffect(cast);
                effectTalent.AddMarkCount(cast.EffectIndex);
            }

        }
    }

    private void updateEffect(float dt)
    {
        if (this.casts == null || this.casts.Count <= 0)
            return;
        for (int i = 0; i < this.casts.Count; i++)
        {
            BattleEffect cast = this.casts[i];
            if (!cast.Running)
            {
                this.casts.Remove(cast);
                continue;
            }

            
            if (cast.IsFollow)
            {
                UVector2 nextPos = nextPositon(dt, ref cast.effectAngle, cast.CasterDistance, effectTalent.Data.FlyRoundSpeed);
                cast.Position = nextPos + this.Position;
                if (this.intervalTime <= 0)
                {
                    BattleActor role = effectTalent.FindTarget(null);
                    if (role != null && !role.IsDead)
                    {
                        cast.SetFollowElement(null);
                        cast.Target = role;
                        cast.InitSpeed();
                        if (effectTalent.Data.FlyBounce > 0)
                        {
                            cast.SetBounce(effectTalent.Data.FlyBounce);
                        }
                        if (effectTalent.Data.FlyTrackingTarget)
                        {
                            cast.TrackTarget(role);
                        }

                        if (!string.IsNullOrEmpty(effectTalent.Data.DirectEffect))
                        {
                            NeptuneBattle.PlayEffect(effectTalent.Data.DirectEffect, cast.Position, cast.Speed.normalized, cast.Height, effectTalent.Data.DirectOrder);
                        }
                        this.intervalTime = UFloat.Round(this.intervalTime + this.effectInterval);
                    }

                }
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log(string.Format("[{0}] UpdateEffect {1} {2} {3}", this.FullName, cast.FullName, cast.Position, cast.Speed));
#endif
            }
        }
        if (this.intervalTime > 0)
            this.intervalTime = UFloat.Round(this.intervalTime - dt);
    }

}
