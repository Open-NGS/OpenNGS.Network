using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Neptune.GameData;

namespace Neptune
{
    /// <summary>
    /// Effect
    /// 抛体特效控制类，实现抛体的发射、跟踪、命中、目标查找、生存周期控制等
    /// </summary>
    public class BattleEffect : BattleEntity
    {
        const int MIN_RADIUS = 5;
        const int MAX_RADIUS = 50;
        /// <summary>
        /// 所属技能
        /// </summary>
        public BattleSkill Talent;
        /// <summary>
        /// 发射源
        /// </summary>
        public BattleEntity Source;

        /// <summary>
        /// 剩余弹射次数，用于弹射类型抛体
        /// </summary>
        public int LeftAttackCount = 0;
        /// <summary>
        /// 命中次数
        /// </summary>
        public int HitCount = 0;
        /// <summary>
        /// Z-轴速度
        /// </summary>
        public int SpeedZ = 0;
        /// <summary>
        /// 记录上次的位置，用于命中测试
        /// </summary>
        public UVector2 LastHitPosition;

        public RelativeSide AffectedSide;

        /// <summary>
        /// 获取全名
        /// </summary>
        public override string FullName
        {
            get { return string.Format("[{0}{1}]{2}>Effect", SideSign, !Running ? "D" : "", Talent.FullName()); }
        }

        /// <summary>
        /// 特效类型
        /// </summary>
        public EffectType Type;

        protected int doneFrameCount = 0;

        protected FlyBounceMode bounceMode;

        /// <summary>
        /// 当前飞行距离
        /// </summary>
        protected float flyDist = 0;
        /// <summary>
        /// 最大飞行距离
        /// </summary>
        public int MaxRange = int.MaxValue;

        /// <summary>
        /// 单次连锁持续时间
        /// </summary>
        public float runningTime = 0;

        /// <summary>
        /// 是否跟踪目标
        /// </summary>
        protected bool tracking = false;
        /// <summary>
        /// 当前跟踪目标
        /// </summary>
        public BattleActor Target;

        //返回到达目标点后返回
        private FlyBackType flyBackType;
        //正在返回中
        private bool isReturning = false;

        protected Dictionary<BattleActor, int> HitTimes = new Dictionary<BattleActor, int>();

        private float flyTime = 0;

        private UVector3 startPos;
        /// <summary>
        /// 获取或设置当前抛体所关联的Actor
        /// </summary>
        public IEffectAgent Joint
        {
            get { return (IEffectAgent)joint; }
            set { joint = value; }
        }

        public int EffectId;
        public float EffectCounter = -1;
        string extType = string.Empty;
        public bool IsHit = false;
        private UVector2 targetPosition;

        private string sourceName;
        int index = 0;
        public int effectAngle;
        public bool IsFollow
        {
            get { return followElement != null; }
        }
        private BattleEntity followElement = null;
        public BattleEntity FollowElement
        {
            get { return followElement; }
        }
        private int casterDistance = 0;
        public int CasterDistance
        {
            get { return casterDistance; }
        }
        public int EffectIndex;

        private int roundSpeed = 0;

        public int RoundSpeed
        {
            get { return roundSpeed; }
        }
        private float reSearhDelayTime = 0;

        /// <summary>
        /// Effect 构造函数
        /// </summary>
        /// <param name="talent">所属技能</param>
        /// <param name="type"></param>
        public BattleEffect() : base()
        {
        }

        public virtual void Init(BattleSkill talent, EffectType type, int effectAngle, int index = 0)
        {
            Create();
            LeftAttackCount = 0;
            SpeedZ = 0;
            doneFrameCount = 0;
            flyDist = 0;
            MaxRange = int.MaxValue;
            runningTime = 0;
            tracking = false;
            isReturning = false;
            startPos = UVector3.zero;
            IsHit = false;
            Talent = talent;
            Type = type;
            this.index = index;
            this.effectAngle = effectAngle;
            Side = talent.Caster.Side;
            AffectedSide = talent.AffectedSide;
            Source = talent.Caster;
            Target = talent.Target;
            Radius = Talent.Data.FlyRadius;
            flyBackType = Talent.Data.FlyBackType;
            targetPosition = Talent.TargetPosition;
            if (talent.Data.FlyRange > 0)
                MaxRange = Mathf.Min(EngineConst.MaxRangeValue, talent.Data.FlyRange);
            bounceMode = talent.Data.FlyBounceMode;
            HitCount = 0;
            HitTimes.Clear();
            doneFrameCount = NeptuneBattle.Instance.doneFrameCount;
            flyTime = 0f;
            followElement = null;
            roundSpeed = Talent.Data.FlyRoundSpeed;
            reSearhDelayTime = 0;
            AddToCaster();
            if (Type == EffectType.Bounce)
            {
                LeftAttackCount = (int)Talent.Data.CastNum;
                Hit(Target);
            }
            else if (Type == EffectType.Cast)
            {
                startPos = talent.AttackPoint(index);
                Position.Set(startPos.x, startPos.y);
                LastHitPosition = Position;
                Height = startPos.z;
                casterDistance = Distance(Talent.Caster, false);
                if (talent.Data.CastVector == VectorType.X)
                {// 只在施法者方向上位移

                    targetPosition = Talent.Caster.Position + talent.Caster.Orientation * MaxRange;
                }
                if (Talent.Data.CastType != CastType.Round)
                    InitSpeed();
            }

#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0}: {1} --> {2} exEffect ({3:f6},{4:f6},{5:f6})({6:f6},{7:f6},{8:f6})", Talent.Caster.FullName, Talent.FullName(), Talent.Target != null ? Talent.Target.FullName : "", Position.x, Position.y, Height, Speed.x, Speed.y, SpeedZ);
#endif
            //if (this.Skill.Caster.Player!=null && this.Skill.Caster.Player.isMainPlayer) CachedLog.Log("Skill done.time:" + Time.realtimeSinceStartup + ",doneframe:" + Logic.Instance.doneFrameCount);
        }

        public void InitSpeed()
        {
            ///在这里初始化 Effect 的初始速度
            UVector3 dist = GetTargetPosition() - startPos;
            dist = new UVector3(dist.x, dist.y, dist.z);
            SpeedZ = Talent.Data.FlySpeedZ;
            if (effectAngle != 0)
            {
                if (dist == UVector3.zero)
                {
                    dist = new UVector3(UFloat.Round(Talent.Caster.Orientation.x * EngineConst.Thousand), UFloat.Round(Talent.Caster.Orientation.y * EngineConst.Thousand), 0);
                }
                dist = Quaternion.AngleAxis(effectAngle, Vector3.back) * dist;
            }

            if (Talent.Data.CastVector == VectorType.X)
            {// 只在施法者方向上位移

                if (dist == UVector3.zero)
                {
                    Speed = Talent.Caster.Orientation * Talent.Data.FlySpeed;
                }
                else
                {
                    Vector3 spd = dist.normalized * Talent.Data.FlySpeed;
                    Speed = new UVector2(UFloat.Round(spd.x), UFloat.Round(spd.y));
                }
            }

            if (Talent.Data.CastVector == VectorType.XY)
            {// 在XY方向产生位移
                if (dist == UVector3.zero)
                {
                    Speed = Talent.Caster.Orientation * Talent.Data.FlySpeed;
                    SpeedZ = 0;
                }
                else
                {
                    dist.z = 0;
                    Vector3 speed = dist.normalized * Talent.Data.FlySpeed;
                    Speed = speed;
                    SpeedZ = UFloat.RoundToInt(speed.z);
                }
            }
            if (Talent.Data.CastVector == VectorType.XYZ)
            {// 在XYZ三个方向产生位移
                if (Talent.Data.FlyDestOffset != null && Talent.Data.FlyDestOffset.Count == 3)
                {
                    dist.x = UFloat.RoundToInt(UFloat.Round(Talent.Data.FlyDestOffset[0] * Talent.Caster.Orientation.x));
                    dist.y = UFloat.RoundToInt(UFloat.Round(Talent.Data.FlyDestOffset[1] * Talent.Caster.Orientation.y));
                    dist.z = UFloat.RoundToInt(UFloat.Round(Talent.Data.FlyDestOffset[2]));
                }
                if (Talent.Data.FlyGravity != 0)
                {
                    if (Talent.Data.FlySpeedZ != 0)
                    {
                        // 根据 Z 的初始速度和落点之间的距离 推移动速度
                        float t = UFloat.Round((-SpeedZ - Mathf.Sqrt(SpeedZ * SpeedZ - 2f * Talent.Data.FlyGravity * -dist.z)) / Talent.Data.FlyGravity);
                        Speed = (UVector2)dist / t;
                    }
                    else
                    {
                        //  根据 落点之间的距离和移动速度 推 Z的初始速度
                        float t = UFloat.Round(dist.magnitude / Talent.Data.FlySpeed);
                        SpeedZ = UFloat.RoundToInt(UFloat.Round(dist.z / t - t * (Talent.Data.FlyGravity / 2))); //(2 * dist.z + t * t * (-talent.Data.FlyGravity)) / (2 * t);
                        Speed = (UVector2)dist / t;
                    }
                }
                else
                {
                    // 导弹类型, 不受重力影响
                    Vector3 speed = dist.normalized * Talent.Data.FlySpeed;
                    Speed = (UVector2)speed;
                    if (Talent.Data.FlySpeedZ == 0)
                        SpeedZ = UFloat.RoundToInt(speed.z);
                }
            }

            Orientation = Speed.normalized;

            if (!string.IsNullOrEmpty(Talent.Data.EffectSound) && Joint != null)
            {
                Joint.PlaySound(Talent.Data.EffectSound);
            }
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log(string.Format("[{0}] InitSpeed {1} {2} {3} ({4:f6},{5:f6})", FullName, dist, Speed, SpeedZ, Orientation.x, Orientation.y));
#endif
        }

        private UVector3 GetTargetPosition()
        {
            UVector3 targetPos = targetPosition;
            if (isReturning && flyBackType == FlyBackType.StartPos)
            {
                targetPos = startPos;
            }
            else if (isReturning && flyBackType == FlyBackType.Caster)
            {
                targetPos = Talent.Caster.Position;
            }
            if (Target != null && Talent.Data.TargetType != TargetType.Direction && Talent.Data.FlyTrackingTarget)
                targetPos = Target.GetHitPoint();
            return targetPos;
        }


        /// <summary>
        /// 主循环
        /// </summary>
        /// <param name="dt">Tick周期</param>
        public override void OnEnterFrame(float dt)
        {
            if (IsFollow)
                return;


            if (reSearhDelayTime > 0)
            {
                reSearhDelayTime = UFloat.Round(reSearhDelayTime - dt);
                if (reSearhDelayTime <= 0)
                {
                    reSearchBounceTarget();
                }
                return;
            }

            if (Type == EffectType.Cast && doneFrameCount == NeptuneBattle.Instance.doneFrameCount)
            {
                CalculateFlyCurve(dt, true);
                return;
            }
            flyTime += dt;
            if (Type == EffectType.Cast && tracking && Target != null && (Target.IsDead || Target.AbilityEffects.Void))
            {
                targetPosition = Target.Position;
                Target = null;
            }
            runningTime = UFloat.Round(runningTime - dt);

            if (Source != null)
            {
                sourceName = Source.FullName;
            }

            if (Type == EffectType.Bounce)
            {
                BounceEffectHit();
            }
            else
            {

                LastHitPosition = Position;
                base.OnEnterFrame(dt);
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log(string.Format("[{0}] OnEnterFrame {1} {2} {3}", FullName, Position, Speed, Speed * dt));
#endif

                if ((!tracking || Target == null) && !isReturning)
                {
                    if (flyDist + UFloat.Round(Speed.magnitude * dt) > MaxRange)
                    {
                        flyDist = MaxRange - UFloat.Round(Speed.magnitude * dt);
                        Position = new UVector2(startPos.x, startPos.y) + Speed.normalized * MaxRange;
                    }
                }
                EffectiveHeightHit(dt);
                CalculateFlyCurve(dt);
                if (Running)
                {
                    bool isFlyToTargetPos = IsFlyToTargetPos();
                    if (isFlyToTargetPos)
                    {//到达目标位置
                        if (isReturning)
                        {//如果是返程
#if BATTLE_LOG
                            if (EngineGlobal.BattleLog)
                                NeptuneBattle.log(string.Format("【{0}】Stop by returning end {1}", Talent.Data.FlyResource, Talent.TargetPosition));
#endif
                            Stop();
                            return;
                        }

                        if (!Talent.Data.FlyPiercing && !Talent.Data.FlyLockTarget)
                        {//非穿刺、非锁定
#if BATTLE_LOG
                            if (EngineGlobal.BattleLog)
                                NeptuneBattle.log(string.Format("【{0}】Stop by hit potision {1}", Talent.Data.FlyResource, Talent.TargetPosition));
#endif
                            if (CheckFlyBackStop())
                            {
                                Talent.HitAt(Position, null, this, TalentHitType.EffectEnd);
                                Stop();
                            }
                            return;
                        }


                        if (tracking && CheckFlyBackStop())
                        {
                            Talent.HitAt(Position, null, this, TalentHitType.EffectEnd);
                            Stop();
                            return;
                        }
                    }

                    //不锁定目标或者 锁定目标 目标为空的时候才检查最大飞行距离 
                    if ((Height < 0 || (!tracking || Target == null) && flyDist >= MaxRange || IsOutOfBattleArea) && !isReturning)
                    {

                        if (CheckFlyBackStop())
                        {
#if BATTLE_LOG
                            if (EngineGlobal.BattleLog)
                                NeptuneBattle.log(string.Format("【{0}】Stop by OutOfBattleArea", Talent.Data.FlyResource));
#endif
                            if (Talent.Data.FlyPiercing)
                            {
                                Talent.HitAt(Position, null, this, TalentHitType.EffectEnd);
                            }
                            Stop();
                        }

                    }

                    if (tracking)
                    {
                        if (Target == null || !Target.IsDead)
                        {
                            //如果启用目标跟踪模式
                            UVector2 dist = (UVector2)GetTargetPosition() - Position;
                            Speed = dist.normalized * Speed.magnitude;
#if BATTLE_LOG
                            if (EngineGlobal.BattleLog)
                                NeptuneBattle.log(string.Format("[{0}] Tracking {1} {2} {3}", FullName, Position, dist, Speed));
#endif
                        }
                        else
                        {
                            Stop();
                        }
                    }
                }
            }
        }
        public bool IsFlyToTargetPos()
        {
            UVector2 targetPos = GetTargetPosition();
            int maxRadius = Target != null ? Mathf.Max(MAX_RADIUS, Target.Radius) : MAX_RADIUS;
            return Mathf.Abs(UVector2.Distance(targetPos, Position) + UVector2.Distance(targetPos, LastHitPosition) - UVector2.Distance(Position, LastHitPosition)) < Mathf.Max(MIN_RADIUS, Mathf.Min(Radius, maxRadius)) /*|| this.Height < this.GetTargetPosition().z*/;
        }
        private bool CheckFlyBackStop()
        {
            if (!isReturning)
            {
                if (flyBackType == FlyBackType.Caster)
                {
                    isReturning = true;
                    TrackTarget(Talent.Caster);
                    HitTimes.Clear();
                    return false;
                }
                if (flyBackType == FlyBackType.StartPos)
                {
                    //开始返回到子弹出生点
                    isReturning = true;
                    tracking = false;
                    Target = null;
                    Speed = ((UVector2)startPos - Position).normalized * Speed.magnitude * (Talent.Data.FlyBackSpeedRatio == 0 ? 1 : Talent.Data.FlyBackSpeedRatio);
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log(string.Format("[{0}] CheckFlyBackStop StartPos {1} {2} {3}", FullName, Position, startPos, Speed));
#endif
                    HitTimes.Clear();
                    return false;
                }
            }
            return true;
        }

        protected virtual void EffectiveHeightHit(float time)
        {
            float deltaDistance = UFloat.Round(SpeedZ * time);
            if (tracking && Target != null && Mathf.Abs(GetTargetPosition().z - Height) < Mathf.Abs(5 * deltaDistance))
            {
                SpeedZ = SpeedZ / 2;
                if (Height + deltaDistance < GetTargetPosition().z)
                {
                    SpeedZ = 0;
                    deltaDistance = 0;
                }
            }
            Height = Height + UFloat.RoundToInt(deltaDistance);

            flyDist = flyDist + UFloat.Round(Speed.magnitude * time);
            if (isReturning && !Talent.Data.FlyPiercing)//如果非穿刺子弹在返回过程中不在检测命中
                return;
            if (Talent.Data.FlyEffectiveHeight == 0 || Height > 0 && Height < Talent.Data.FlyEffectiveHeight)
            {//只在有效高度范围内进行命中逻辑判断
                float dist = 1;
                BattleActor target1 = null;

                if (Talent.Data.FlyLockTarget)
                {
                    if (Target != null && HitTest(Target) >= 0)
                    {
                        target1 = Target;
                    }
                }
                else
                {
                    //                foreach (Actor role in Logic.Instance.GetSurvivors(this, this.AffectedSide,this.Radius+EngineConst.RoleCollideRadiusMax+ Skill.Data.FlySpeed))
                    foreach (BattleActor role in NeptuneBattle.Instance.AOIManager.GetSurvivors(this, AffectedSide, Radius + EngineConst.RoleCollideRadiusMax + Talent.Data.FlySpeed))
                    {
                        if (!Talent.CheckTargetClass(role))
                            continue;
                        if (Talent.Data.TargetSelectorWithOutSelf && role == Talent.Caster)
                            continue;
                        float hit = HitTest(role);
                        if (hit < 0)
                        {
                            if (HitTimes.ContainsKey(role))
                            {
                                HitTimes.Remove(role);
                            }

                            continue;
                        }
                        else
                        {
                            if (HitTimes.ContainsKey(role))
                            {
                                continue;
                            }
                        }

                        if (Talent.Data.FlyPiercing /* && this.Skill.Caster.Orientation == this.Orientation 2016/05/11 Ray@raymix.net 注释掉这里以解决穿刺子弹穿刺无效的问题 */)
                        {
                            TalentHitType hitType = TalentHitType.EffectHit;
                            if ((Talent.Data.FlyPiercingType & role.Data.RoleType) > 0)
                            {
                                if (flyBackType == FlyBackType.None || isReturning)
                                {
                                    hitType |= TalentHitType.EffectEnd;
                                }
                            }
                            Hit(role, hitType);
                            if ((hitType & TalentHitType.EffectEnd) > 0)
                            {
                                if (flyBackType != FlyBackType.None)
                                {
                                    CheckFlyBackStop();
                                }
                                else
                                    Stop();
                            }
                        }

                        else if (dist > hit)
                        {
                            dist = hit;
                            target1 = role;
                        }
#if BATTLE_LOG
                        if (EngineGlobal.BattleLog)
                            NeptuneBattle.log(string.Format("[{0}] HitTest Pos:{1} Actor:{2} Hit:{3} Dist:{4}", FullName, Position, role.Position, hit, dist));
#endif
                    }
                }
                if (target1 != null)
                {
                    //Debug.Log(string.Format("EffectiveHeightHit:【{0}】Stop by Hit  {1}", this.Skill.Data.FlyResource, this.Position));
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log(string.Format("[{0}] Stop by Hit  {1}", FullName, Position));
#endif
                    TalentHitType hitType = TalentHitType.EffectHit;
                    if (flyBackType == FlyBackType.None || isReturning)
                    {
                        hitType |= TalentHitType.EffectEnd;
                    }
                    Hit(target1, hitType);
                    IsHit = true;
                    if (flyBackType != FlyBackType.None)
                    {
                        CheckFlyBackStop();
                    }
                    else
                        Stop();
                }
                else
                {
                    //Debug.Log(string.Format("EffectiveHeightHit:【{0}】Hit Nothing {1}", this.Skill.Data.FlyResource, this.Position));
                }
            }

        }
        protected bool CalculateFlyCurve(float time, bool init = false)
        {
            if (Talent.Data.FlyWarpSpeed > 0)
            {//曲线速度计算
                if (Talent.Target == null)
                {
                    return false;
                }
                float deltaWs = UFloat.Round(Talent.Data.FlyWarpSpeed * time);

                UVector2 dist = Talent.Target.Position - Position;
                Vector3 v = new UVector3(dist.x, dist.y, -Height).normalized;
                if (deltaWs < SpeedZ)
                {
                    SpeedZ = UFloat.RoundToInt(SpeedZ - deltaWs);
                    deltaWs = 0;
                }
                else if (SpeedZ > 0)
                {
                    deltaWs = (deltaWs - SpeedZ) * 8;
                    SpeedZ = 0;
                }
                else
                    deltaWs = deltaWs * 8;
                Speed = new UVector2(UFloat.Round(Speed.x + UFloat.Round(v.x * deltaWs)), UFloat.Round(Speed.y + UFloat.Round(v.y * deltaWs)));
                SpeedZ = UFloat.RoundToInt(SpeedZ + v.z * deltaWs);
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log(string.Format("[{0}] CalculateFlyCurve {1} {2} {3} {4} ({5:f6},{6:f6},{7:f6}) {8:f6}", FullName, Position, dist, Speed, SpeedZ, v.x, v.y, v.z, deltaWs));
#endif
            }
            else if (!init)
            {
                SpeedZ = UFloat.RoundToInt(SpeedZ + Talent.Data.FlyGravity * time);
            }
            return true;
        }

        /// <summary>
        /// 命中目标
        /// </summary>
        /// <param name="target">目标单位</param>
        public virtual void Hit(BattleActor target = null, TalentHitType hitType = TalentHitType.EffectHit)
        {
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("Hit {0}", this);
#endif
            //Notify bullet hit target position
            NeptuneBattle.Instance.OnRoleHit(Talent.Owner);

            UVector3 hitPosition = Position;
            runningTime = UFloat.Round(runningTime + Talent.Data.BounceInterval);
            if (target != null)
            {
                if (!HitTimes.ContainsKey(target))
                    HitTimes[target] = 0;
                HitTimes[target]++;
                hitPosition = target.Position;
            }
            Talent.castFlyTime = flyTime;
            if (Type == EffectType.Cast)
            {
                //if (this.Skill.Data.AreaCenter == TargetType.None)//范围为空
                //{//直接命中
                //    this.Skill.HitTarget(target, this);
                //}
                //else
                //{//Area类型
                //    this.Skill.HitAt(hitPosition, target, this);
                //}
                Talent.HitAt(hitPosition, target, this, hitType);
            }
            else//is Bounce
            {
                LeftAttackCount = LeftAttackCount - 1;
                Talent.HitTarget(Target, this, hitType);

                if (NeptuneBattle.Instance.Scene == null)
                    return;

                if (string.IsNullOrEmpty(Talent.Data.BounceEffect))
                    return;

                IEffectAgent effect = ObjectFactory.Create(this, EffectType.Bounce);
                NeptuneBattle.Instance.Scene.AddJoint(effect);
            }
        }


        protected void BounceEffectHit()
        {
            if (runningTime <= 0)
            {
                if (LeftAttackCount > 0)
                {
                    //先设置来源，再进行目标查找
                    Source = Target;
                    Target = SearchBounceTarget();
                    if (Target != null)
                    {
                        Hit(Target);
                        return;
                    }
                    else
                        Stop();//目标不存在时终止
                }
                Stop();
            }
            else if (Target.IsDead && LeftAttackCount <= 0 && Talent.Data.BounceInterval - runningTime > 0.3f)
            {
                Stop();
            }
        }
        private UVector2 tdistance = UVector2.zero;
        /// <summary>
        /// 命中测试
        /// </summary>
        /// <param name="role">测试目标</param>
        /// <returns></returns>
        protected float HitTest(BattleActor role)
        {
            if (tracking && Target != null && Target != role && Talent.Data.FlyLockTarget && flyBackType == FlyBackType.None)//已有追踪目标
            {
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("Already have tracking target");
#endif
                return -1.0f;
            }

            if (role.AbilityEffects.Void)//目标不可攻击
            {
                return -1.0f;
            }
            int radius = role.Data.CollideRadius + Radius;
            int sqrRadius = radius * radius;
            //Debug.LogFormat("HitTest[{0}] Target[{1}] - <Start> Radius: {2}({3},{4})", this.Skill.Caster.ID, role.ID, radius, role.Data.CollideRadius, this.Radius);

#if true

            if (NeptuneBattle.Instance.doneFrameCount <= doneFrameCount + 1)
            {
                //第一次运算时需要在初始位置计算一次半径内是否可以命中到目标
                tdistance = role.Position - LastHitPosition;//目标离子弹当前位置的距离
                if (tdistance.sqrMagnitude < sqrRadius)
                {//直接命中
                 //Debug.LogFormat("HitTest[{0}] Target[{1}] - HitPosition: {4} ( {2} : {3})", this.Skill.Caster.ID, role.ID, this.Position,role.Position, distance.magnitude / radius);
                    return UFloat.Round((float)tdistance.sqrMagnitude / sqrRadius);
                }
            }

            //2016-12-01 Ray modify bullent hit test logic.
            // 判断是否有任意一个端点在目标碰撞半径之内
            tdistance = role.Position - Position;//目标离子弹当前位置的距离
            if (tdistance.sqrMagnitude < sqrRadius)
            {//直接命中
             //Debug.LogFormat("HitTest[{0}] Target[{1}] - HitPosition: {4} ( {2} : {3})", this.Skill.Caster.ID, role.ID, this.Position,role.Position, distance.magnitude / radius);
                return UFloat.Round((float)tdistance.sqrMagnitude / sqrRadius);
            }

            //
            UVector2 range = Position - LastHitPosition;//子弹单帧飞行范围
            if (range.sqrMagnitude < tdistance.sqrMagnitude - sqrRadius)//目标远在射程外
            {
                //Debug.LogFormat("HitTest[{0}] Target[{1}] - Distance: {2} > Range :{3}", this.Skill.Caster.ID, role.ID, distance.magnitude, range.magnitude);
                return -1;
            }

            UVector2 targetPos = role.Position - LastHitPosition;
            UVector2 curPos = Position - LastHitPosition;

            int eAngle = MathUtil.VectorToAngleInt(curPos, UVector2.right);
            curPos = Quaternion.AngleAxis(eAngle, Vector3.back) * curPos;
            targetPos = Quaternion.AngleAxis(eAngle, Vector3.back) * targetPos;

            if (targetPos.x < 0 || targetPos.x > curPos.x)
            {
                //Debug.LogFormat("HitTest[{0}] Target[{1}] - Distance: {2} > Range :{3}", this.Skill.Caster.ID, role.ID, targetPos.x, curPos.x);
                return -1.0f;//目标不在飞行区间内
            }

            int dist = targetPos.y * targetPos.y;
            if (sqrRadius > 0 && dist >= sqrRadius)
            {
                //Debug.LogFormat("HitTest[{0}] Target[{1}] - Outof Radius: dist {2} > radius :{3}", this.Skill.Caster.ID, role.ID, dist, radius);
                return -1.0f;
            }

#else
        // 判断是否有任意一个端点在目标碰撞半径之内
        UVector2 d_a = role.Position - this.Position;
        if (d_a.magnitude < radius)
        {
            return d_a.magnitude / radius;
        }

        UVector2 d_b = role.Position - LastHitPosition;
        if (d_b.magnitude < radius)
        {
            return d_b.magnitude / radius;
        }

        // 判断线段两端点是否在目标点的同一侧
        UVector2 d_c = this.Position - LastHitPosition;
        if (d_c.sqrMagnitude < d_b.sqrMagnitude - radius * radius)
            return -1;

        // 线段两端点在目标点的两侧，判断目标点到线段的垂直距离是否小于半径
        float dist = Mathf.Abs(d_c.y * role.Position.x - d_c.x * role.Position.y + LastHitPosition.y * d_c.x - LastHitPosition.x * d_c.y) / d_c.magnitude;
        if (dist > radius)
            return -1;
#endif
            //Debug.LogFormat("HitTest[{0}] Target[{1}] - Hit in Radius: {4} : ({2} /{3})", this.ToString(), role.ID, dist, radius, dist / radius);
            return UFloat.Round((float)dist / sqrRadius);
        }

        /// <summary>
        /// 启用目标跟踪
        /// </summary>
        /// <param name="pTarget">专注目标</param>
        public void TrackTarget(BattleActor pTarget)
        {
            tracking = true;
            Target = pTarget;
            Speed = (pTarget.Position - Position).normalized * Speed.magnitude * (Talent.Data.FlyBackSpeedRatio == 0 ? 1 : Talent.Data.FlyBackSpeedRatio);
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log(string.Format("[{0}] TrackTarget StartPos {1} {2} {3}", FullName, Position, pTarget.Position, Speed));
#endif
        }

        /// <summary>
        /// 启用反弹
        /// </summary>
        /// <param name="times">反弹次数</param>
        public void SetBounce(int times)
        {
            LeftAttackCount = times;
            Source = Talent.Target;
        }

        protected void AddToCaster()
        {
            if (Talent != null && Talent.Caster != null)
            {
                Talent.Caster.AddCastEffect(this);
            }
        }
        protected void RemoveFromCaster()
        {
            if (Talent != null && Talent.Caster != null)
            {
                Talent.Caster.RemoveCastEffect(this);
            }
        }

        private void reSearchBounceTarget()
        {
            BattleActor target = SearchBounceTarget();
            if (target != null)
            {//设置新目标及初始速度

                Speed = (target.Position - Position).normalized * Talent.Data.FlySpeed;
                if (tracking)
                    Target = target;
                Source = target;
                Orientation = Speed.normalized;
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("Terminated T {0} -> {1} {2}", this, target.FullName, Speed);
#endif
            }
            else
            {//无目标销毁
                RemoveFromCaster();
                base.Stop();
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("Terminated NoT {0}", this);
#endif
                if (!Running && Joint != null)
                    Joint.Stop();
            }
        }

        /// <summary>
        /// 终止Effect
        /// </summary>
        public override void Stop()
        {
            if (Type == EffectType.Cast)
            {
                LeftAttackCount = LeftAttackCount - 1;
                if (LeftAttackCount <= 0 || IsOutOfBattleArea || Source == null)
                {//无弹跳或超出战斗区域则终止
                    RemoveFromCaster();
                    base.Stop();
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("Terminated {0}", this);
#endif
                    if (!Running && Joint != null)
                        Joint.Stop();
                }
                else
                {//如果弹跳次数>0，则进行弹跳逻辑
                    if (Talent.Data.EffectSearchDelayTime > 0)
                    {
                        reSearhDelayTime = UFloat.Round(Talent.Data.EffectSearchDelayTime * EngineConst.Milli);
                        Speed = UVector2.zero;
                    }
                    else
                        reSearchBounceTarget();
                }


            }
            else
            {
                RemoveFromCaster();
                base.Stop();
                if (Type == EffectType.Bounce && Joint != null)
                    Joint.Stop(0.3f);
            }

        }

        /// <summary>
        /// 查找下个目标
        /// </summary>
        /// <returns></returns>
        public BattleActor SearchBounceTarget()
        {
            int minTimes = bounceMode == FlyBounceMode.Default ? int.MaxValue : 0;
            int minDist = int.MaxValue;
            int disft = bounceMode == FlyBounceMode.Default ? 80 * 80 : 0;
            BattleActor result = null;
            //        foreach (Actor role in Logic.Instance.GetSurvivors(this, this.Skill.TargetSide, this.Skill.MaxRange))
            foreach (BattleActor role in NeptuneBattle.Instance.AOIManager.GetSurvivors(this, Talent.TargetSide, Talent.MaxRange))
            {
                if (!Talent.CheckTargetClass(role))
                    continue;
                if (Talent.Data.TargetSelectorWithOutSelf && role == Talent.Caster)
                    continue;
                //不能攻击来源 
                //此单位不可作为目标
                if (role == Source || role.AbilityEffects.Void)
                    continue;

                int times = 0;
                if (HitTimes.ContainsKey(role))
                    times = HitTimes[role];
                if (bounceMode == FlyBounceMode.NewTarget && times > 0)
                    continue;
                int dist = 0;
                if (bounceMode == FlyBounceMode.Nearest || times <= minTimes)
                {
                    dist = Mathf.Abs((role.Position - Position).sqrMagnitude - disft);
                    if (dist > minDist)
                    {
                        continue;
                    }

                    if (dist > Talent.MinRangeSqr && Talent.MaxRangeSqr > dist
                        && (Talent.Data.FlyBounceRange <= 0 || Talent.Data.FlyBounceRange * Talent.Data.FlyBounceRange > dist))
                    {
                        result = role;
                        minTimes = times;
                        minDist = dist;
                    }
                }
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("    {0}: {1} exEffect SearchBounceTarget {2} {3} {4} {5} ({6:f6},{7:f6}) ", Talent.Caster.FullName, Talent.FullName(), sourceName, role.FullName, times, dist, minTimes, minDist);
#endif
            }
            return result;
        }



        public override string ToString()
        {
            return string.Format("{0} Effect > {1,-10} POS:({2}, {3}, {4})\tSPD:({5}, {6}, {7})\t{8}\t{9:f6}) DividedCast{10:f4} \r\n",
                Talent.Caster.FullName + " " + Talent.FullName(), (Talent.Target != null ? Talent.Target.FullName : "") + " -> " + (Target != null ? Target.FullName : ""), Position.x, Position.y, Height, Speed.x, Speed.y, SpeedZ, LeftAttackCount, runningTime, DamageFactor
                );
        }

        public override void Delete()
        {
            NObjectPool<BattleEffect>.Delete(this);
        }

        public void SetFollowElement(BattleEntity element)
        {
            followElement = element;
        }
    }
}