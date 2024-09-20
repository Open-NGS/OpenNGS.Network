using UnityEngine;
using System.Collections.Generic;
using Neptune.Datas;


namespace Neptune
{

    /// <summary>
    /// Effect
    /// 抛体特效控制类，实现抛体的发射、跟踪、命中、目标查找、生存周期控制等
    /// </summary>
    public class Effect : Entity
    {
        const int MIN_RADIUS = 5;
        const int MAX_RADIUS = 50;
        /// <summary>
        /// 所属技能
        /// </summary>
        public Skill Skill;
        /// <summary>
        /// 发射源
        /// </summary>
        public Entity Source;

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

        /// <summary>
        /// 生命周期
        /// </summary>
        public float LifeTime;

        public RelativeSide AffectedSide;

        /// <summary>
        /// 获取全名
        /// </summary>
        public override string FullName
        {
            get { return string.Format("[{0}{1}]{2}>Effect", this.SideSign, !this.Running ? "D" : "", this.Skill.FullName()); }
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
        public Actor Target;

        //返回到达目标点后返回
        private FlyBackType flyBackType;
        //正在返回中
        private bool isReturning = false;

        protected Dictionary<Actor, int> HitTimes = new Dictionary<Actor, int>();

        private float flyTime = 0;

        private UVector3 startPos;
        /// <summary>
        /// 获取或设置当前抛体所关联的Actor
        /// </summary>
        public IEffectAgent Joint
        {
            get { return (IEffectAgent)this.joint; }
            set { this.joint = value; }
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
        private Entity followElement = null;
        public Entity FollowElement
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
        public Effect() : base()
        {
        }

        private int RoundRadius = 0;
        private float m_fTargetAngle = 0.0f;
        public virtual void Init(Skill talent, EffectType type, int effectAngle, int index = 0)
        {
            base.Create();
            this.LeftAttackCount = 0;
            this.SpeedZ = 0;
            this.doneFrameCount = 0;
            this.flyDist = 0;
            this.MaxRange = int.MaxValue;
            this.runningTime = 0;
            this.tracking = false;
            this.isReturning = false;
            this.startPos = UVector3.zero;
            this.IsHit = false;
            this.Skill = talent;
            this.Type = type;
            this.index = index;
            this.effectAngle = effectAngle;
            this.Side = talent.Caster.Side;
            this.AffectedSide = talent.AffectedSide;
            this.Source = talent.Caster;
            this.Target = talent.Target;
            this.Radius = this.Skill.Data.FlyRadius;
            this.flyBackType = this.Skill.Data.FlyBackType;
            this.targetPosition = this.Skill.TargetPosition;
            if (talent.Data.FlyRange > 0)
                this.MaxRange = Mathf.Min(NeptuneConst.MaxRangeValue, talent.Data.FlyRange);
            this.bounceMode = talent.Data.FlyBounceMode;
            this.HitCount = 0;
            this.HitTimes.Clear();
            this.doneFrameCount = NeptuneBattle.Instance.doneFrameCount;
            this.flyTime = 0f;
            this.followElement = null;
            this.roundSpeed = this.Skill.Data.FlyRoundSpeed;
            this.reSearhDelayTime = 0;
            this.m_fAngle = 180.0f;
            this.AddToCaster();
            if (this.Type == EffectType.Bounce)
            {
                this.LeftAttackCount = (int)this.Skill.Data.CastNum;
                this.Hit(this.Target);
            }
            else if (this.Type == EffectType.Cast)
            {
                startPos = talent.AttackPoint(index);
                this.Position.Set(startPos.x, startPos.y);
                this.LastHitPosition = this.Position;
                this.Height = startPos.z;
                this.casterDistance = this.Distance(this.Skill.Caster, false);
                if (talent.Data.CastVector == VectorType.X)
                {// 只在施法者方向上位移

                    this.targetPosition = this.Skill.Caster.Position + talent.Caster.Orientation * this.MaxRange;
                }
                if (this.Skill.Data.CastType != CastType.Round)
                {
                    InitSpeed();
                }
                else
                {
                    RoundRadius = Skill.Data.MaxRange ;
                    this.LifeTime = Skill.Data.ContinuousTime / 1000f;

                    m_fAngle = UVector2.Angle(UVector2.left, 
                        this.Skill.Caster.Orientation.normalized * RoundRadius);
                    m_fAngle += effectAngle;
                    m_fTargetAngle = m_fAngle + 180f;
                    CalcRoundSpeed(0.02f);
                    //EditorApplication.isPaused = true;
                }
            }

#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0}: {1} --> {2} exEffect ({3:f6},{4:f6},{5:f6})({6:f6},{7:f6},{8:f6})", this.Skill.Caster.FullName, this.Skill.FullName(), this.Skill.Target != null ? this.Skill.Target.FullName : "", this.Position.x, this.Position.y, this.Height, this.Speed.x, this.Speed.y, this.SpeedZ);
#endif
            //if (this.Skill.Caster.Player!=null && this.Skill.Caster.Player.isMainPlayer) CachedLog.Log("Skill done.time:" + Time.realtimeSinceStartup + ",doneframe:" + NeptuneBattle.Instance.doneFrameCount);
        }

        public void InitSpeed()
        {
            ///在这里初始化 Effect 的初始速度
            UVector3 dist = GetTargetPosition() - startPos;
            dist = new UVector3(dist.x, dist.y, dist.z);
            this.SpeedZ = this.Skill.Data.FlySpeedZ;
            if (this.effectAngle != 0)
            {
                if (dist == UVector3.zero)
                {
                    dist = new UVector3(UFloat.Round(this.Skill.Caster.Orientation.x * NeptuneConst.Thousand), UFloat.Round(this.Skill.Caster.Orientation.y * NeptuneConst.Thousand), 0);
                }
                dist = Quaternion.AngleAxis(effectAngle, Vector3.back) * dist;
            }

            if (this.Skill.Data.CastVector == VectorType.X)
            {// 只在施法者方向上位移

                if (dist == UVector3.zero)
                {
                    this.Speed = this.Skill.Caster.Orientation * this.Skill.Data.FlySpeed;
                }
                else
                {
                    Vector3 spd = dist.normalized * this.Skill.Data.FlySpeed;
                    this.Speed = new UVector2(UFloat.Round(spd.x), UFloat.Round(spd.y));
                }
            }

            if (this.Skill.Data.CastVector == VectorType.XY)
            {// 在XY方向产生位移
                if (dist == UVector3.zero)
                {
                    this.Speed = this.Skill.Caster.Orientation * this.Skill.Data.FlySpeed;
                    this.SpeedZ = 0;
                }
                else
                {
                    dist.z = 0;
                    Vector3 speed = dist.normalized * this.Skill.Data.FlySpeed;
                    this.Speed = speed;
                    this.SpeedZ = UFloat.RoundToInt(speed.z);
                }
            }
            if (this.Skill.Data.CastVector == VectorType.XYZ)
            {// 在XYZ三个方向产生位移
                if (this.Skill.Data.FlyDestOffset != null && this.Skill.Data.FlyDestOffset.Length == 3)
                {
                    dist.x = UFloat.RoundToInt(UFloat.Round(this.Skill.Data.FlyDestOffset[0] * this.Skill.Caster.Orientation.x));
                    dist.y = UFloat.RoundToInt(UFloat.Round(this.Skill.Data.FlyDestOffset[1] * this.Skill.Caster.Orientation.y));
                    dist.z = UFloat.RoundToInt(UFloat.Round(this.Skill.Data.FlyDestOffset[2]));
                }
                if (this.Skill.Data.FlyGravity != 0)
                {
                    if (this.Skill.Data.FlySpeedZ != 0)
                    {
                        // 根据 Z 的初始速度和落点之间的距离 推移动速度
                        float t = UFloat.Round((-this.SpeedZ - Mathf.Sqrt(this.SpeedZ * this.SpeedZ - 2f * this.Skill.Data.FlyGravity * -dist.z)) / this.Skill.Data.FlyGravity);
                        this.Speed = (UVector2)dist / t;
                    }
                    else
                    {
                        //  根据 落点之间的距离和移动速度 推 Z的初始速度
                        float t = UFloat.Round(dist.magnitude / (this.Skill.Data.FlySpeed));
                        this.SpeedZ = UFloat.RoundToInt(UFloat.Round(dist.z / t - t * (this.Skill.Data.FlyGravity / 2))); //(2 * dist.z + t * t * (-talent.Data.FlyGravity)) / (2 * t);
                        this.Speed = (UVector2)dist / t;
                    }
                }
                else
                {
                    // 导弹类型, 不受重力影响
                    Vector3 speed = dist.normalized * this.Skill.Data.FlySpeed;
                    this.Speed = (UVector2)speed;
                    if (this.Skill.Data.FlySpeedZ == 0)
                        this.SpeedZ = UFloat.RoundToInt(speed.z);
                }
            }

            this.Orientation = this.Speed.normalized;

            if (!string.IsNullOrEmpty(this.Skill.Data.EffectSound) && this.Joint != null)
            {
                this.Joint.PlaySound(this.Skill.Data.EffectSound);
            }
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log(string.Format("[{0}] InitSpeed {1} {2} {3} ({4:f6},{5:f6})", this.FullName, dist, this.Speed, this.SpeedZ, this.Orientation.x, this.Orientation.y));
#endif
        }

        private UVector3 GetTargetPosition()
        {
            UVector3 targetPos = this.targetPosition;
            if (isReturning && this.flyBackType == FlyBackType.StartPos)
            {
                targetPos = this.startPos;
            }
            else if (isReturning && this.flyBackType == FlyBackType.Caster)
            {
                targetPos = this.Skill.Caster.Position;
            }
            if (this.Target != null && this.Skill.Data.TargetType != TargetType.Direction && this.Skill.Data.FlyTrackingTarget)
                targetPos = this.Target.GetHitPoint();
            return targetPos;
        }

        private bool CalcRoundSpeed(float dt)
        {
            bool bNeedReturn = false;
            UVector2 _targetPosition = this.targetPosition;
            if(this.Target != null)
            {
                _targetPosition = this.Target.Position;
            }

            float fDestX = UFloat.Round(RoundRadius * Mathf.Cos(m_fAngle * Mathf.Deg2Rad) + _targetPosition.x);
            float fDestY = UFloat.Round(RoundRadius * Mathf.Sin(m_fAngle * Mathf.Deg2Rad) + _targetPosition.y);
            int nDestX = UFloat.RoundToInt(fDestX);
            int nDestY = UFloat.RoundToInt(fDestY);
            UVector2 _destPos = new UVector2(nDestX , nDestY );
            //this.Speed = (_destPos - this.Position).normalized * Skill.Data.FlySpeed;
            this.Position = _destPos;
            this.Speed = UVector2.zero;
            if ( this.LifeTime > 0.0f && this.flyTime >= this.LifeTime)
            {
                this.Stop();
            }
            float fSpeed = UFloat.Round(roundSpeed * dt);
            m_fAngle = fSpeed + m_fAngle;
            if (Skill.Data.FlyBackType != FlyBackType.None)
            {
                if (m_fAngle >= m_fTargetAngle)
                {
                    m_fAngle -= m_fTargetAngle;
                    bNeedReturn = true;
                }
            }
            else
            {
                if (m_fAngle > 360f)
                {
                    m_fAngle -= 360f;
                }
            }
            return bNeedReturn;
        }


        private float m_fAngle = 180.0f;
        /// <summary>
        /// 主循环
        /// </summary>
        /// <param name="dt">Tick周期</param>
        public override void OnEnterFrame(float dt)
        {
            if (IsFollow)
                return;


            if (this.reSearhDelayTime > 0)
            {
                this.reSearhDelayTime = UFloat.Round(this.reSearhDelayTime - dt);
                if (this.reSearhDelayTime <= 0)
                {
                    reSearchBounceTarget();
                }
                return;
            }

            if (this.Type == EffectType.Cast && this.doneFrameCount == NeptuneBattle.Instance.doneFrameCount)
            {
                CalculateFlyCurve(dt, true);
                return;
            }
            this.flyTime += dt;
            if (this.Type == EffectType.Cast && this.tracking && this.Target != null && (this.Target.IsDead || this.Target.AbilityEffects.Value.Void))
            {
                this.targetPosition = this.Target.Position;
                this.Target = null;
            }
            this.runningTime = UFloat.Round(this.runningTime - dt);

            if (this.Source != null)
            {
                this.sourceName = this.Source.FullName;
            }

            if (this.Type == EffectType.Bounce)
            {
                BounceEffectHit();
            }
            else
            {

                this.LastHitPosition = this.Position;
                base.OnEnterFrame(dt);
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log(string.Format("[{0}] OnEnterFrame {1} {2} {3}", this.FullName, this.Position, this.Speed, this.Speed * dt));
#endif

                if ((!this.tracking || this.Target == null) && !isReturning)
                {
                    if (this.flyDist + UFloat.Round(this.Speed.magnitude * dt) > this.MaxRange)
                    {
                        this.flyDist = this.MaxRange - UFloat.Round(this.Speed.magnitude * dt);
                        this.Position = new UVector2(startPos.x, startPos.y) + this.Speed.normalized * this.MaxRange;
                    }
                }
                EffectiveHeightHit(dt);
                CalculateFlyCurve(dt);
                if (this.Running)
                {
                    bool isFlyToTargetPos = IsFlyToTargetPos();
                    if (isFlyToTargetPos)
                    {//到达目标位置
                        if (isReturning)
                        {//如果是返程
#if BATTLE_LOG
                            if (EngineGlobal.BattleLog)
                                NeptuneBattle.log(string.Format("【{0}】Stop by returning end {1}", this.Skill.Data.FlyResource, this.Skill.TargetPosition));
#endif
                            this.Stop();
                            return;
                        }

                        if (!this.Skill.Data.FlyPiercing && !this.Skill.Data.FlyLockTarget)
                        {//非穿刺、非锁定
#if BATTLE_LOG
                            if (EngineGlobal.BattleLog)
                                NeptuneBattle.log(string.Format("【{0}】Stop by hit potision {1}", this.Skill.Data.FlyResource, this.Skill.TargetPosition));
#endif
                            if (CheckFlyBackStop())
                            {
                                this.Skill.HitAt(this.Position, null, this, SkillHitType.EffectEnd);
                                this.Stop();
                            }
                            return;
                        }


                        if (this.tracking && CheckFlyBackStop())
                        {
                            this.Skill.HitAt(this.Position, null, this, SkillHitType.EffectEnd);
                            this.Stop();
                            return;
                        }
                    }

                    //不锁定目标或者 锁定目标 目标为空的时候才检查最大飞行距离 
                    if ((this.Height < 0 || ((!this.tracking || this.Target == null) && this.flyDist >= this.MaxRange) || this.IsOutOfBattleArea) && !isReturning)
                    {
                        if(Skill.Data.CastType == CastType.Round )
                        {
                            CalcRoundSpeed(dt);
                        }
                        else
                        {
                            if (CheckFlyBackStop())
                            {
#if BATTLE_LOG
                            if (EngineGlobal.BattleLog)
                                NeptuneBattle.log(string.Format("【{0}】Stop by OutOfBattleArea", this.Skill.Data.FlyResource));
#endif
                                if (this.Skill.Data.FlyPiercing)
                                {
                                    this.Skill.HitAt(this.Position, null, this, SkillHitType.EffectEnd);
                                }
                                this.Stop();
                            }
                        }
                    }
                    else
                    {
                        if (Skill.Data.CastType == CastType.Round)
                        {
                            if( isReturning == false)
                            {
                                bool bNeedReturn = CalcRoundSpeed(dt);
                                if (bNeedReturn == true)
                                {
                                    this.isReturning = true;
                                    if (this.flyBackType == FlyBackType.Caster)
                                    {
                                        this.tracking = true;
                                        this.Target = this.Skill.Caster;
                                        this.Speed = (this.Skill.Caster.Position - this.Position).normalized * this.Skill.Data.FlySpeed * 5;
                                    }
                                }
                            }
                            else
                            {

                            }
                        }
                    }

                    if (this.tracking)
                    {
                        if (this.Target == null || !this.Target.IsDead)
                        {
                            //如果启用目标跟踪模式
                            UVector2 dist = (UVector2)this.GetTargetPosition() - this.Position;
                            this.Speed = dist.normalized * this.Speed.magnitude;
#if BATTLE_LOG
                            if (EngineGlobal.BattleLog)
                                NeptuneBattle.log(string.Format("[{0}] Tracking {1} {2} {3}", this.FullName, this.Position, dist, this.Speed));
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
            UVector2 targetPos = this.GetTargetPosition();
            int maxRadius = this.Target != null ? Mathf.Max(MAX_RADIUS, this.Target.Radius) : MAX_RADIUS;

            int nDistanceTar_Cur = UVector2.Distance(targetPos, this.Position);
            int nDistanceTar_Last = UVector2.Distance(targetPos, this.LastHitPosition);
            int nDistanceCur_Last = UVector2.Distance(this.Position, this.LastHitPosition);

            int nDistance = Mathf.Abs(
                nDistanceTar_Cur + nDistanceTar_Last - nDistanceCur_Last
                );
            int nMaxRadius = Mathf.Max(MIN_RADIUS, Mathf.Min(this.Radius, maxRadius));
            return nDistance < nMaxRadius;
        }

        private bool CheckRoundFlyBack()
        {
            bool bRes = true;
            if (!this.isReturning)
            {
                if (this.flyBackType == FlyBackType.Caster)
                {
                    isReturning = true;
                    TrackTarget(this.Skill.Caster);
                    bRes = false;
                }
            }
            return bRes;
        }

        private bool CheckFlyBackStop()
        {
            if (!this.isReturning)
            {
                if (this.flyBackType == FlyBackType.Caster)
                {
                    isReturning = true;
                    TrackTarget(this.Skill.Caster);
                    HitTimes.Clear();
                    return false;
                }
                if (this.flyBackType == FlyBackType.StartPos)
                {
                    //开始返回到子弹出生点
                    isReturning = true;
                    this.tracking = false;
                    this.Target = null;
                    this.Speed = ((UVector2)this.startPos - this.Position).normalized * this.Speed.magnitude * (this.Skill.Data.FlyBackSpeedRatio == 0 ? 1 : this.Skill.Data.FlyBackSpeedRatio);
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log(string.Format("[{0}] CheckFlyBackStop StartPos {1} {2} {3}", this.FullName, this.Position, this.startPos, this.Speed));
#endif
                    HitTimes.Clear();
                    return false;
                }
            }
            return true;
        }

        protected virtual void EffectiveHeightHit(float time)
        {
            float deltaDistance = UFloat.Round(this.SpeedZ * time);
            if (this.tracking && this.Target != null && Mathf.Abs(GetTargetPosition().z - this.Height) < Mathf.Abs(5 * deltaDistance))
            {
                this.SpeedZ = this.SpeedZ / 2;
                if (this.Height + deltaDistance < GetTargetPosition().z)
                {
                    this.SpeedZ = 0;
                    deltaDistance = 0;
                }
            }
            this.Height = this.Height + UFloat.RoundToInt(deltaDistance);

            this.flyDist = this.flyDist + UFloat.Round(this.Speed.magnitude * time);
            if (this.isReturning && !this.Skill.Data.FlyPiercing)//如果非穿刺子弹在返回过程中不在检测命中
                return;
            if (this.Skill.Data.FlyEffectiveHeight == 0 || (this.Height > 0 && this.Height < this.Skill.Data.FlyEffectiveHeight))
            {//只在有效高度范围内进行命中逻辑判断
                float dist = 1;
                Actor target1 = null;

                if (this.Skill.Data.FlyLockTarget)
                {
                    if (this.Target != null && this.HitTest(this.Target) >= 0)
                    {
                        target1 = this.Target;
                    }
                }
                else
                {
                    //                foreach (Actor role in NeptuneBattle.Instance.GetSurvivors(this, this.AffectedSide,this.Radius+NeptuneConst.RoleCollideRadiusMax+ Skill.Data.FlySpeed))
                    foreach (Actor role in NeptuneBattle.Instance.AOIManager.GetSurvivors(this, this.AffectedSide, this.Radius + NeptuneConst.RoleCollideRadiusMax + Skill.Data.FlySpeed))
                    {
                        if (!Skill.CheckTargetClass(role))
                            continue;
                        if (this.Skill.Data.TargetSelectorWithOutSelf && role == this.Skill.Caster)
                            continue;
                        float hit = this.HitTest(role);
                        if (hit < 0)
                        {
                            if (this.HitTimes.ContainsKey(role))
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

                        if (this.Skill.Data.FlyPiercing/* && this.Skill.Caster.Orientation == this.Orientation 2016/05/11 Ray@raymix.net 注释掉这里以解决穿刺子弹穿刺无效的问题 */)
                        {
                            SkillHitType hitType = SkillHitType.EffectHit;
                            if ((this.Skill.Data.FlyPiercingType & role.Data.RoleType) > 0)
                            {
                                if (this.flyBackType == FlyBackType.None || this.isReturning)
                                {
                                    hitType |= SkillHitType.EffectEnd;
                                }
                            }
                            this.Hit(role, hitType);
                            if ((hitType & SkillHitType.EffectEnd) > 0)
                            {
                                if (this.flyBackType != FlyBackType.None)
                                {
                                    CheckFlyBackStop();
                                }
                                else
                                    this.Stop();
                            }
                            else
                            {
                                int nHitTimes = Skill.GethitNum();
                                if (Skill.MaxTargetNum > 0 && Skill.MaxTargetNum <= nHitTimes)
                                {
                                    this.Stop();
                                }
                            }
                        }

                        else if (dist > hit)
                        {
                            dist = hit;
                            target1 = role;
                        }
#if BATTLE_LOG
                        if (EngineGlobal.BattleLog)
                            NeptuneBattle.log(string.Format("[{0}] HitTest Pos:{1} Actor:{2} Hit:{3} Dist:{4}", this.FullName, this.Position, role.Position, hit, dist));
#endif
                    }
                }
                if (target1 != null)
                {
                    //Debug.Log(string.Format("EffectiveHeightHit:【{0}】Stop by Hit  {1}", this.Skill.Data.FlyResource, this.Position));
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log(string.Format("[{0}] Stop by Hit  {1}", this.FullName, this.Position));
#endif
                    SkillHitType hitType = SkillHitType.EffectHit;
                    if (this.flyBackType == FlyBackType.None || this.isReturning)
                    {
                        hitType |= SkillHitType.EffectEnd;
                    }
                    this.Hit(target1, hitType);
                    IsHit = true;
                    if (this.flyBackType != FlyBackType.None)
                    {
                        CheckFlyBackStop();
                    }
                    else
                        this.Stop();
                }
                else
                {
                    //Debug.Log(string.Format("EffectiveHeightHit:【{0}】Hit Nothing {1}", this.Skill.Data.FlyResource, this.Position));
                }
            }

        }
        protected bool CalculateFlyCurve(float time, bool init = false)
        {
            if (this.Skill.Data.FlyWarpSpeed > 0)
            {//曲线速度计算
                UVector2 _targetPos = UVector2.zero;
                if (this.Skill.Target == null)
                {
                    if(Skill.Data.TargetType != TargetType.Direction)
                    {
                        return false;
                    }
                    else
                    {
                        _targetPos = this.targetPosition;
                    }
                }
                else
                {
                    _targetPos = this.Skill.Target.Position;
                }
                float deltaWs = UFloat.Round(this.Skill.Data.FlyWarpSpeed * time);

                UVector2 dist = _targetPos - this.Position;
                Vector3 v = new UVector3(dist.x, dist.y, -this.Height).normalized;
                if (deltaWs < this.SpeedZ)
                {
                    this.SpeedZ = UFloat.RoundToInt(this.SpeedZ - deltaWs);
                    deltaWs = 0;
                }
                else if (this.SpeedZ > 0)
                {
                    deltaWs = (deltaWs - this.SpeedZ) * 8;
                    this.SpeedZ = 0;
                }
                else
                    deltaWs = deltaWs * 8;

                this.Speed = new UVector2(UFloat.Round(this.Speed.x + UFloat.Round(v.x * deltaWs)), UFloat.Round(this.Speed.y + UFloat.Round(v.y * deltaWs)));
                this.SpeedZ = UFloat.RoundToInt(this.SpeedZ + v.z * deltaWs);
                if(this.Speed.magnitude < 500)
                {
                    this.Speed = this.Speed.normalized * 500;
                }
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log(string.Format("[{0}] CalculateFlyCurve {1} {2} {3} {4} ({5:f6},{6:f6},{7:f6}) {8:f6}", this.FullName, this.Position, dist, this.Speed, this.SpeedZ, v.x, v.y, v.z, deltaWs));
#endif
            }
            else if (!init)
            {
                this.SpeedZ = UFloat.RoundToInt(this.SpeedZ + this.Skill.Data.FlyGravity * time);
            }
            return true;
        }

        /// <summary>
        /// 命中目标
        /// </summary>
        /// <param name="target">目标单位</param>
        public virtual void Hit(Actor target = null, SkillHitType hitType = SkillHitType.EffectHit)
        {
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("Hit {0}", this);
#endif
            //Notify bullet hit target position
            NeptuneBattle.Instance.OnRoleHit(this.Skill.Owner);

            UVector3 hitPosition = this.Position;
            this.runningTime = UFloat.Round(this.runningTime + this.Skill.Data.BounceInterval);
            if (target != null)
            {
                if (!this.HitTimes.ContainsKey(target))
                    this.HitTimes[target] = 0;
                this.HitTimes[target]++;
                hitPosition = target.Position;
            }
            this.Skill.castFlyTime = this.flyTime;
            if (this.Type == EffectType.Cast)
            {
                //if (this.Skill.Data.AreaCenter == TargetType.None)//范围为空
                //{//直接命中
                //    this.Skill.HitTarget(target, this);
                //}
                //else
                //{//Area类型
                //    this.Skill.HitAt(hitPosition, target, this);
                //}
                this.Skill.HitAt(hitPosition, target, this, hitType);
            }
            else//is Bounce
            {
                this.LeftAttackCount = this.LeftAttackCount - 1;
                this.Skill.HitTarget(this.Target, this, hitType);

                if (NeptuneBattle.Instance.Scene == null)
                    return;

                if (string.IsNullOrEmpty(this.Skill.Data.BounceEffect))
                    return;

                IEffectAgent effect = ObjectFactory.Create(this, EffectType.Bounce);
                NeptuneBattle.Instance.Scene.AddJoint(effect);
            }
        }


        protected void BounceEffectHit()
        {
            if (this.runningTime <= 0)
            {
                if (this.LeftAttackCount > 0)
                {
                    //先设置来源，再进行目标查找
                    this.Source = this.Target;
                    this.Target = this.SearchBounceTarget();
                    if (this.Target != null)
                    {
                        this.Hit(this.Target);
                        return;
                    }
                    else
                        this.Stop();//目标不存在时终止
                }
                this.Stop();
            }
            else if (this.Target.IsDead && this.LeftAttackCount <= 0 && (this.Skill.Data.BounceInterval - this.runningTime) > 0.3f)
            {
                this.Stop();
            }
        }
        private UVector2 tdistance = UVector2.zero;
        /// <summary>
        /// 命中测试
        /// </summary>
        /// <param name="role">测试目标</param>
        /// <returns></returns>
        protected float HitTest(Actor role)
        {
            if (this.tracking && this.Target != null && this.Target != role && this.Skill.Data.FlyLockTarget && this.flyBackType == FlyBackType.None)//已有追踪目标
            {
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("Already have tracking target");
#endif
                return -1.0f;
            }

            if (role.AbilityEffects.Value.Void)//目标不可攻击
            {
                return -1.0f;
            }
            int radius = role.Data.CollideRadius + this.Radius;
            int sqrRadius = radius * radius;
            //Debug.LogFormat("HitTest[{0}] Target[{1}] - <Start> Radius: {2}({3},{4})", this.Skill.Caster.ID, role.ID, radius, role.Data.CollideRadius, this.Radius);

#if true

            if (NeptuneBattle.Instance.doneFrameCount <= this.doneFrameCount + 1)
            {
                //第一次运算时需要在初始位置计算一次半径内是否可以命中到目标
                tdistance = role.Position - this.LastHitPosition;//目标离子弹当前位置的距离
                if (tdistance.sqrMagnitude < sqrRadius)
                {//直接命中
                 //Debug.LogFormat("HitTest[{0}] Target[{1}] - HitPosition: {4} ( {2} : {3})", this.Skill.Caster.ID, role.ID, this.Position,role.Position, distance.magnitude / radius);
                    return UFloat.Round((float)tdistance.sqrMagnitude / sqrRadius);
                }
            }

            //2016-12-01 Ray modify bullent hit test logic.
            // 判断是否有任意一个端点在目标碰撞半径之内
            tdistance = role.Position - this.Position;//目标离子弹当前位置的距离
            if (tdistance.sqrMagnitude < sqrRadius)
            {//直接命中
             //Debug.LogFormat("HitTest[{0}] Target[{1}] - HitPosition: {4} ( {2} : {3})", this.Skill.Caster.ID, role.ID, this.Position,role.Position, distance.magnitude / radius);
                return UFloat.Round((float)tdistance.sqrMagnitude / sqrRadius);
            }

            //
            UVector2 range = this.Position - LastHitPosition;//子弹单帧飞行范围
            if (range.sqrMagnitude < tdistance.sqrMagnitude - sqrRadius)//目标远在射程外
            {
                //Debug.LogFormat("HitTest[{0}] Target[{1}] - Distance: {2} > Range :{3}", this.Skill.Caster.ID, role.ID, distance.magnitude, range.magnitude);
                return -1;
            }

            UVector2 targetPos = role.Position - this.LastHitPosition;
            UVector2 curPos = this.Position - this.LastHitPosition;

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
        public void TrackTarget(Actor pTarget)
        {
            this.tracking = true;
            this.Target = pTarget;
            this.Speed = (pTarget.Position - this.Position).normalized * this.Speed.magnitude * (this.Skill.Data.FlyBackSpeedRatio == 0 ? 1 : this.Skill.Data.FlyBackSpeedRatio);
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log(string.Format("[{0}] TrackTarget StartPos {1} {2} {3}", this.FullName, this.Position, pTarget.Position, this.Speed));
#endif
        }

        /// <summary>
        /// 启用反弹
        /// </summary>
        /// <param name="times">反弹次数</param>
        public void SetBounce(int times)
        {
            this.LeftAttackCount = times;
            this.Source = this.Skill.Target;
        }

        protected void AddToCaster()
        {
            if (this.Skill != null && this.Skill.Caster != null)
            {
                this.Skill.Caster.AddCastEffect(this);
            }
        }
        protected void RemoveFromCaster()
        {
            if (this.Skill != null && this.Skill.Caster != null)
            {
                this.Skill.Caster.RemoveCastEffect(this);
            }
        }

        private void reSearchBounceTarget()
        {
            Actor target = this.SearchBounceTarget();
            if (target != null)
            {//设置新目标及初始速度

                this.Speed = (target.Position - this.Position).normalized * this.Skill.Data.FlySpeed;
                if (this.tracking)
                    this.Target = target;
                this.Source = target;
                this.Orientation = this.Speed.normalized;
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("Terminated T {0} -> {1} {2}", this, target.FullName, this.Speed);
#endif
            }
            else
            {//无目标销毁
                this.RemoveFromCaster();
                base.Stop();
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("Terminated NoT {0}", this);
#endif
                if (!this.Running && this.Joint != null)
                    this.Joint.Stop();
            }
        }

        /// <summary>
        /// 终止Effect
        /// </summary>
        public override void Stop()
        {
            if (this.Type == EffectType.Cast)
            {
                this.LeftAttackCount = this.LeftAttackCount - 1;
                if (this.LeftAttackCount <= 0 || this.IsOutOfBattleArea || this.Source == null)
                {//无弹跳或超出战斗区域则终止
                    this.RemoveFromCaster();
                    base.Stop();
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("Terminated {0}", this);
#endif
                    if (!this.Running && this.Joint != null)
                        this.Joint.Stop();
                }
                else
                {//如果弹跳次数>0，则进行弹跳逻辑
                    if (this.Skill.Data.EffectSearchDelayTime > 0)
                    {
                        this.reSearhDelayTime = UFloat.Round(this.Skill.Data.EffectSearchDelayTime * NeptuneConst.Milli);
                        this.Speed = UVector2.zero;
                    }
                    else
                        this.reSearchBounceTarget();
                }


            }
            else
            {
                this.RemoveFromCaster();
                base.Stop();
                if (this.Type == EffectType.Bounce && this.Joint != null)
                    this.Joint.Stop(0.3f);
            }

        }

        /// <summary>
        /// 查找下个目标
        /// </summary>
        /// <returns></returns>
        public Actor SearchBounceTarget()
        {
            int minTimes = this.bounceMode == FlyBounceMode.Default ? int.MaxValue : 0;
            int minDist = int.MaxValue;
            int disft = this.bounceMode == FlyBounceMode.Default ? 80 * 80 : 0;
            Actor result = null;
            //        foreach (Actor role in NeptuneBattle.Instance.GetSurvivors(this, this.Skill.TargetSide, this.Skill.MaxRange))
            foreach (Actor role in NeptuneBattle.Instance.AOIManager.GetSurvivors(this, this.Skill.TargetSide, this.Skill.MaxRange))
            {
                if (!Skill.CheckTargetClass(role))
                    continue;
                if (this.Skill.Data.TargetSelectorWithOutSelf && role == this.Skill.Caster)
                    continue;
                //不能攻击来源 
                //此单位不可作为目标
                if (role == this.Source || role.AbilityEffects.Value.Void)
                    continue;

                int times = 0;
                if (this.HitTimes.ContainsKey(role))
                    times = this.HitTimes[role];
                if (this.bounceMode == FlyBounceMode.NewTarget && times > 0)
                    continue;
                int dist = 0;
                if (this.bounceMode == FlyBounceMode.Nearest || times <= minTimes)
                {
                    dist = Mathf.Abs((role.Position - this.Position).sqrMagnitude - disft);
                    if (dist > minDist)
                    {
                        continue;
                    }

                    if (dist > this.Skill.MinRangeSqr && this.Skill.MaxRangeSqr > dist
                        && (Skill.Data.FlyBounceRange <= 0 || Skill.Data.FlyBounceRange * Skill.Data.FlyBounceRange > dist))
                    {
                        result = role;
                        minTimes = times;
                        minDist = dist;
                    }
                }
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("    {0}: {1} exEffect SearchBounceTarget {2} {3} {4} {5} ({6:f6},{7:f6}) ", this.Skill.Caster.FullName, this.Skill.FullName(), this.sourceName, role.FullName, times, dist, minTimes, minDist);
#endif
            }
            return result;
        }



        public override string ToString()
        {
            return string.Format("{0} Effect > {1,-10} POS:({2}, {3}, {4})\tSPD:({5}, {6}, {7})\t{8}\t{9:f6}) DividedCast{10:f4} \r\n",
                this.Skill.Caster.FullName + " " + this.Skill.FullName(), (this.Skill.Target != null ? this.Skill.Target.FullName : "") + " -> " + (this.Target != null ? this.Target.FullName : ""), 
                this.Position.x, this.Position.y, this.Height, this.Speed.x, this.Speed.y, this.SpeedZ, this.LeftAttackCount, runningTime, this.DamageFactor
                );
        }

        public override void Delete()
        {
            ObjectPool<Effect>.Delete(this);
        }

        public void SetFollowElement(Entity element)
        {
            this.followElement = element;
        }
    }
}