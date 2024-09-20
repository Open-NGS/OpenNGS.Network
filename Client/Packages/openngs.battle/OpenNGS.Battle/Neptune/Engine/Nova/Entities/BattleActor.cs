using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using System.Linq;
using Neptune.GameData;
using Neptune;

/// <summary>
/// Actor
/// 游戏角色的基本元件
/// <seealso cref="Element">Entity</seealso>>
/// </summary>
public class BattleActor : BattleEntity, ISafeListElement
{
    /// <summary>
    /// 获取单位的类型ID
    /// </summary>
    public BattlePlayer Player;
    public int ID { get { return this.Hero.tid; } }
    public int Index = 0;
    public float HP = 0;
    public float MP = 0;
    public float Rage = 0;
    public float Point = 0;
    public override string Name { get { return this.SummonData == null ? this.Data.Model : this.SummonData.Model; } }
    public int Level { get { return this.Hero.level; } }
    public int Stars { get { return this.Hero.stars; } }
    public int Quality;
    public int MonIdx;
    public long Money { get { return this.Config.Money; } }

    public float CommonCooldown = 0;

    public float QualityRatio;

    public int GlobalIdx;
    public int TeamIdx = -1;

    public bool CanUseActiveTalent = false;
    public bool IsActiveCasting = false;

    public string AniId = string.Empty;
    public bool nonOnce = false;
    public float AniTotaltime = 0;
    public float AniPlayedTime = 0;

    public BattleSkill CurrentTalent;
    public BattleSkill ActiveTalent;

    public bool IsHero { get { return (this.Data.RoleType & RoleType.Hero) == RoleType.Hero && !this.Config.IsMonster; } }
    //boss战测试
    public bool IsBoss = false;
    public bool IsCreep = false;
    public int AttackRange;
    public bool PlayEffectOnDeath;
    public bool DisapearOnDeath = true;
    /// <summary>
    /// 当前目标
    /// </summary>
    public BattleActor Target;

    /// <summary>
    /// 锁定的目标
    /// </summary>
    public BattleActor LockedTarget;

    public UVector2 Destination = UVector2.zero;

    public bool activeTalentReadyToGo;

    //角色移动方向
    public Vector2 MoveDirection
    {
        get
        {
            if (moveDirLock)
                return Vector2.zero;

            return moveDirection;

        }
        set
        {

            moveDirection = value;

        }
    }
    public bool moveDirLock = false;

    public Vector2 moveDirection;
    private Vector2 moveOrientation;
    public bool IsAIMode = true;
    public bool IsAutoAttackMode = false;
    public AttackAIMode attackAIMode = AttackAIMode.AutoSelectSkill;

    protected bool isCloseAutoAttack = false;

    public int cacheSkillIndex = 0;
    public IActorAgent Joint
    {
        get { return (IActorAgent)this.joint; }
        set { this.joint = value; }
    }

    public bool IsDead { get { return this.RoleST == RoleState.Death || this.RoleST == RoleState.Dead; } }

    public UVector2 PreviousPosition = UVector2.zero;
    public UVector2 MoveSpeed = UVector2.zero;

    public float DashTime = 0;
    public float DashTotalTime = 0;
    public float DashDelayPauseTime = 0;
    public float DashPauseTime = 0;
    public UVector2 DashSpeed = UVector2.zero;
    public UVector2 DashAcceleration = UVector2.zero;
    public UVector2 DashLastDistane = UVector2.zero;
    public bool DashNoRoot = false;
    public RoleState RoleST;
    public float dsFactor = 1;

    /// <summary>
    /// 敌对立场
    /// </summary>
    public RelativeSide AntiSide;


    public bool IsDeadBody;

    public float RepelTime = -1;
    public float RepelTotalTime = 0;
    public UVector2 repelLastDistane = UVector2.zero;
    public UVector2 RepelAcceleration = UVector2.zero;
    public UVector2 RepelSpeed = UVector2.zero;

    public float MoveSpeedMultiplier = 1;

    public int TransformGroupId;
    //public bool IsTransformByManual;

    public float MoveSpeedRatio = 1;
    public float AttackSpeedRatio = 1;
    public bool LowHP = false;
    public bool LowMP = false;

    public float InjuryFactor = 1;
    public float InjuryCrit = 1;

    public bool IsReverse = false;

    public bool Visible;
    public bool isTrasnparent;
    public List<int> DeathNote = new List<int>();
    public List<BattleActor> SummonList = new List<BattleActor>();
    public bool IsAirForce = false;
    public bool CanBuyEquipment = true;
    public UVector2 TargetPos = UVector2.zero;
    public Vector2 Direction = UVector2.zero;
    /// <summary>
    /// navigate agent
    /// </summary>
    public NavAgent NavAgent;

    public OrcaAgent OrcaAgent;

    // Skill
    public BattleSkill BasicTalent;
    public Dictionary<int, List<int>> TalentConditon = new Dictionary<int, List<int>>();

    // 以 Skill Group ID 为 key
    protected Dictionary<int, BattleSkill> Talents = new Dictionary<int, BattleSkill>();

    /// <summary>
    /// 组合技能子技能列表
    /// </summary>
    public Dictionary<int, BattleSkill> ChildTalents = new Dictionary<int, BattleSkill>();
    /// <summary>
    /// 主动技能列表
    /// </summary>
    //public List<Skill> ActiveTalents = new List<Skill>();
    public TArray<BattleSkill> tActiveTalents = new TArray<BattleSkill>(50);
    public List<BattleSkill> TriggerOnDamageTalents = new List<BattleSkill>();
    public List<BattleSkill> TriggerOnRoleDeathTalents = new List<BattleSkill>();
    public List<BattleSkill> TriggerOnMoveTalent = new List<BattleSkill>();
    /// <summary>
    /// 用字典索引的主动技能列表，用于加快查找速度
    /// </summary>
    //private Dictionary<int, List<Skill>> activeTalentsDict = new Dictionary<int, List<Skill>>();

    /// <summary>
    /// 被动技能列表
    /// </summary>
    public TArray<PassiveSkill> PassiveTalents = new TArray<PassiveSkill>(20);
    /// <summary>
    /// 光环技能列表
    /// </summary>
    public List<TalentData> AuraTalents = new List<TalentData>();
    /// <summary>
    /// 对敌光环技能列表
    /// </summary>
    public List<TalentData> EnemyAuraTalents = new List<TalentData>();
    /// <summary>
    /// 角色属性
    /// </summary>
    public RoleAttributes Attributes
    {
        get { return this.AllAttributes[(int)AttributeType.Final]; }
        set { this.AllAttributes[(int)AttributeType.Final] = value; }
    }
    /// <summary>
    /// 基本属性
    /// </summary>
    public RoleAttributes BasicAttributes = new RoleAttributes();

    /// <summary>
    /// 角色属性
    /// </summary>
    public RoleAttributes[] AllAttributes = new RoleAttributes[(int)AttributeType.Max];

    /// <summary>
    /// 父属性
    /// </summary>
    public RoleAttributes ParentAttributes = new RoleAttributes();


    public List<TransformData> TransformInfos = new List<TransformData>();

    /// <summary>
    /// Skill 效果
    /// </summary>
    public AbilityEffects AbilityEffects = new AbilityEffects();

    /// <summary>
    /// 被动效果
    /// </summary>
    public TalentData PassiveEffect = new TalentData();

    // Use this for initialization
    public RoleConfig Config;
    /// <summary>
    /// 英雄数据
    /// </summary>
    public RoleInfo Hero;
    /// <summary>
    /// 单位属性数据
    /// </summary>
    public RoleData Data;
    /// <summary>
    /// 装备数据
    /// </summary>
    public RoleGearData GearData;
    /// <summary>
    /// 品质数据
    /// </summary>
    public RoleQualityData QualityData;
    /// <summary>
    /// 下一品阶数据
    /// </summary>
    public RoleQualityData NextLvQualityData;

    //public HireData HireData;
    //end
    public RoleGrowthData GrowthData;

    public RoleSkin RoleSkin;
    /// <summary>
    /// 获取最大HP
    /// </summary>
    public float MaxHP { get { return this.Attributes.MaxHP; } }
    /// <summary>
    /// 获取最大MP
    /// </summary>
    public float MaxMP { get { return this.Attributes.MaxMP; } }

    public float MaxRage { get { return this.Attributes.MaxRage; } }
    public float MaxPoint { get { return this.Attributes.MaxPoint; } }
    private bool lerpRotate = false;
    /// <summary>
    /// 方向
    /// </summary>
    public Vector2 Orientation
    {
        get { return base.Orientation; }
        set
        {
            base.Orientation = value;
            if (!lerpRotate)
            {
                this.moveOrientation = base.Orientation;
            }
        }
    }

    public override void SetPosition(UVector2 pos)
    {
        base.SetPosition(pos);
        if (OrcaAgent != null)
        {
            OrcaAgent.SetPosition(this.Position);
        }
    }

    public float MaxProtection { get; set; }

    public float CurrentHPRatio
    {
        get
        {
            return UFloat.Round(HP / MaxHP);
        }
    }

    public float CurrentMPRatio
    {
        get
        {
            if (MaxMP > 0.0f)
            {
                return UFloat.Round(MP / MaxMP);
            }
            else
            {
                return 1f;
            }
        }
    }

    public virtual bool IsInGrass
    {
        get
        {
            return this.AbilityEffects.Grass;
        }
    }

    /// <summary>
    /// Ability列表
    /// </summary>
    //public List<Ability> Abilities = new List<Ability>();

    public TArray<Ability> tAbilities = new TArray<Ability>(100);

    private Dictionary<int, int> AbilitiesDict = new Dictionary<int, int>(50);

    private Dictionary<BattleSkill, List<Ability>> talentAbilityDict = new Dictionary<BattleSkill, List<Ability>>(10);
    /// <summary>
    /// 角色被添加的印记
    /// </summary>
    public TArray<BattleMark> _marksDic = new TArray<BattleMark>(100);

    private List<BattleEffect> CastringEffects = new List<BattleEffect>(10);

    public Dictionary<int, float> zoomIndex = new Dictionary<int, float>();
    /// <summary>
    /// 召唤兽属性数据
    /// </summary>
    public SummonData SummonData;

    public BattleActor AttachedRole;

    public float LifeTime = float.NaN;
    public float RebirthTime = 0.0f;
    public float RebrithHp = 0.0f;
    public float RebrithMp = 0.0f;
    public float RebrithHpRatio = 0.0f;
    public float RebrithMpRatio = 0.0f;
    public float Scale = 1.0f;
    public float ZoomCurValue = 1.0f;
    /// <summary>
    /// 所有者
    /// </summary>
    public BattleActor Owner;

    /// <summary>
    /// 部署时间
    /// </summary>
    public float DeployTime = 1;

    /// <summary>
    /// 出生时间
    /// </summary>
    protected float BirthTime = 0;

    private float finalKillTime = 0;
    private BattleActor finalKiller = null;
    public BattleActor FinalKiller
    {
        get
        {
            if (this.finalKillTime + EngineConst.KillTime < NeptuneBattle.Instance.LogicTime)
                return null;
            return this.finalKiller;
        }
    }

    private Dictionary<BattleActor, float> attackerMap = new Dictionary<BattleActor, float>();
    private List<BattleActor> assistList = new List<BattleActor>(5);

    public bool ShowDamagedEffect = EngineConst.ShowDamagedEffect;

    // Callback
    public UnityAction callbackByDead = null;
    public UnityAction callbackByRebirth = null;
    public UnityAction callbackByLevelUp = null;
    public UnityAction callbackByBreakTalent = null;
    public UnityAction callbackAutoMoveByAttack = null; // 自动攻击的移动回调
    //TODO:这些回调函数都要处理掉
    public UnityAction<BattleActor, int> callbackByAttack = null;
    private event UnityAction<Ability> callbackByAddAbility = null;
    public event UnityAction<bool> callbackByUseTalent = null;
    public event UnityAction<BattleSkill> callbackByTalentStart = null;

    public string ModelName = "";


    float jumpingTime = 0;
    bool isJumping = false;
    int shoveForce = 100;
    int shoveThreshold = 10;
    protected float birthTime = 0.0f;

    /// <summary>
    /// 注册一个添加Ability后的通知回调
    /// </summary>
    public void RegisterCallbackByAddAbility(UnityAction<Ability> callbackByAddAbility)
    {
        this.callbackByAddAbility += callbackByAddAbility;
    }

    /// <summary>
    /// 移出一个添加Ability后的通知回调
    /// </summary>
    public void RemoveCallbackByAddAbility(UnityAction<Ability> callbackByAddAbility)
    {
        this.callbackByAddAbility -= callbackByAddAbility;
    }

    /// <summary>
    /// 获取是否不受影响
    /// </summary>
    public bool IsUnaffected
    {
        get
        {
            return this.AbilityEffects.Unaffected;
        }
    }


    private bool IsDeath { get { return this.RoleST == RoleState.Death; } }

    /// <summary>
    /// 获取初始方向
    /// </summary>
    public Vector2 InitDirection
    {
        get
        {
            return ((int)this.Side * (BattleField.Reverse ? Vector2.left : Vector2.right)).normalized;
        }
    }

    /// <summary>
    /// 不可选中状态
    /// </summary>
    public virtual bool NotSelectable(BattleActor target)
    {
        if (target == null || target.AbilityEffects.Void)
            return true;
        if (target.AbilityEffects.Bare)
            return false;
        if (target.AbilityEffects.Invisible)
            return true;
        if (target.AbilityEffects.Vision)
            return false;
        return target.AbilityEffects.Grass;
    }

    /// <summary>
    /// 获取Model名称
    /// </summary>
    //public string ModelName
    //{
    //    get { return GetLastModelName(); }
    //}


    public string GetLastModelName()
    {
        string ret = this.ModelStack.GetLastValue();

        if (ret != default(string))
        {
            ModelName = ret;
            return ret;
        }
        else
        {
            return string.Empty;
        }
    }

    public int GetLastModelIndex()
    {
        int ret = this.ModelStack.GetLastKey();
        if (ret > 0)
        {
            ModelName = ModelStack.GetLastValue();
        }
        return ret;
    }

    public int GetLastTransformId()
    {
        return this.TransformStack.GetLastValue();
    }

    /// <summary>
    /// 获取目标是否为传奇英雄
    /// </summary>
    /// <param name="hero">目标</param>
    /// <returns></returns>
    public static bool IsLegend(RoleInfo hero, bool exist = true)
    {
        return ((hero != null) && hero.legend != null && hero.legend.Status == 1) && exist;
    }
    public RoleExtra ExtraData;

    public double rand = 0;


    public bool IsExtraInited = false;
    public int CurrentTransform;
    public bool needTransformModel = true;
    //Scale compute
    bool isZoomRunning = false;
    float zoomRunningTime = 0;
    float zoomDuration = 0;
    float zoomValue = 0;
    protected float moveAnimDT;
    protected float attackAnimDT;
    bool Reentrancy = false;
    protected bool shove;
    //Dictionary<int, string> ModelStack = new Dictionary<int, string>();
    private StableStack<string> ModelStack = new StableStack<string>(16);
    private StableStack<int> TransformStack = new StableStack<int>(16);
    private StableStack<int> transformIDToModel = new StableStack<int>(16);
    //        Dictionary<int, int> transformIDToModel = new Dictionary<int, int>();



    public Dictionary<int, List<int>> ChangeTalentStack = new Dictionary<int, List<int>>();
    public int changeIndex;
    public bool CanMove = true;
    public bool bInitMove = true;
    public const int BuildingWeight = 100;

    public BattleSkill TargetTalent = null;
    protected string RoleID = string.Empty;

    protected float movingTime = 0;
    protected bool charging = false;
    public bool isShake = true;
    /// <summary>
    /// 记录
    /// </summary>
    public bool BeingShoved = false;

    private Dictionary<int, RoleGrowthData> RoleGrowthDatas;


    // 处理 草丛 攻击状态 
    float grassShowDuration = 3f;
    int lastAttackStateFrame;
    public bool isLastingAttackState = false;
    public bool isInsideGrass = false;


    Vector2 currentMoveDir = UVector2.zero;
    public List<OrcaLine> OrcaLines;
    private int speedmagnitude = 0;

    public bool IsRoleType(RoleType type)
    {
        return (this.Data.RoleType & type) == type;
    }


    public static RoleData LoadRoleData(int tid)
    {
        RoleData roleData = NeptuneBattle.Instance.DataProvider.GetRoleData(tid);
        if (roleData == null)
        {
            Debug.LogError("LoadRoleData Actor failed, ID not existed:" + tid);
            return null;
        }
        return roleData.Clone();
    }

    public BattleActor() { }

    /// <summary>
    /// Actor 构造
    /// </summary>
    /// <param name="hero">英雄数据</param>
    /// <param name="RoleSide">阵营</param>
    /// <param name="config">配置</param>

    public virtual void Create(RoleInfo hero, RoleData data, RoleSide side, RoleConfig config, RoleExtra extraData, UVector3 pos, Vector2 orientation)
    {
        base.Create();
        this.Index = 0;
        this.CommonCooldown = 0;
        this.Scale = 1.0f;
        this.ZoomCurValue = 1.0f;
        this.GlobalIdx = 0;
        this.TeamIdx = -1;
        this.CanUseActiveTalent = false;
        this.IsActiveCasting = false;

        this.AniId = string.Empty;
        this.nonOnce = false;
        this.AniTotaltime = 0;
        this.AniPlayedTime = 0;

        this.CurrentTalent = null;
        this.ActiveTalent = null;

        this.IsCreep = false;
        this.AttackRange = 0;
        this.PlayEffectOnDeath = false;
        this.DisapearOnDeath = true;

        this.Target = null;
        this.LockedTarget = null;
        this.Destination = UVector2.zero;

        this.moveDirLock = false;
        this.moveDirection = UVector2.zero;

        this.PreviousPosition = UVector2.zero;
        this.MoveSpeed = UVector2.zero;
        this.DashTime = 0;
        this.DashNoRoot = false;
        this.RepelTime = -1;
        this.RepelSpeed = UVector2.zero;
        this.MoveSpeedMultiplier = 1;
        this.MoveSpeedRatio = 1;
        this.AttackSpeedRatio = 1;
        this.LowHP = false;
        this.LowMP = false;
        this.InjuryFactor = 1;
        this.InjuryCrit = 1;
        this.IsReverse = false;
        this.birthTime = 0;
        this.isTrasnparent = false;
        this.DeathNote.Clear();
        this.SummonList.Clear();

        this.CanBuyEquipment = true;
        this.TargetPos = UVector2.zero;
        this.Direction = UVector2.zero;
        this.BasicTalent = null;
        this.TalentConditon.Clear();
        this.Talents.Clear();
        this.ChildTalents.Clear();
        this.tActiveTalents.Clear();
        this.TriggerOnDamageTalents.Clear();
        this.TriggerOnRoleDeathTalents.Clear();
        this.TriggerOnMoveTalent.Clear();
        this.PassiveTalents.Clear();
        this.AuraTalents.Clear();
        this.EnemyAuraTalents.Clear();
        for (int i = 0; i < (int)AttributeType.Max; i++)
        {
            this.AllAttributes[i].Init();
            this.AllAttributes[i].Reset();
        }
        //this.Attributes.Init();
        //this.Attributes.Reset();

        //this.ParentAttributes = null;
        this.TransformInfos.Clear();
        this.AbilityEffects.Reset();
        this.PassiveEffect.Reset();
        this.zoomIndex.Clear();
        this.changeIndex = 0;

        this.GearData = null;
        this.QualityData = null;
        this.NextLvQualityData = null;
        this.GrowthData = null;
        this.MaxProtection = 0;

        this.AbilitiesDict.Clear();
        this.SummonData = null;
        this.AttachedRole = null;
        this.RebrithHp = 0.0f;
        this.RebrithMp = 0.0f;
        this.RebrithHpRatio = 0.0f;
        this.RebrithMpRatio = 0.0f;
        this.Owner = null;
        this.ShowDamagedEffect = EngineConst.ShowDamagedEffect;

        this.callbackByDead = null;
        this.callbackByRebirth = null;
        this.callbackByLevelUp = null;
        this.callbackByBreakTalent = null;
        this.callbackByAttack = null;
        this.callbackByAddAbility = null;
        this.callbackByUseTalent = null;
        this.callbackByTalentStart = null;

        this.ModelName = "";


        this.jumpingTime = 0;
        this.isJumping = false;
        this.shoveForce = 100;
        this.shoveThreshold = 10;

        this.currentMoveDir = UVector2.zero;
        this.OrcaLines = null;

        this.rand = 0;

        this.IsExtraInited = false;
        this.CurrentTransform = 0;
        this.needTransformModel = true;
        this.isZoomRunning = false;
        this.zoomRunningTime = 0;
        this.zoomDuration = 0;
        this.zoomValue = 0;
        this.moveAnimDT = 0;
        this.attackAnimDT = 0;
        this.Reentrancy = false;
        this.shove = false;
        this.ModelStack.Clear();
        this.TransformStack.Clear();
        this.transformIDToModel.Clear();
        this.ChangeTalentStack.Clear();
        this.CanMove = true;
        this.bInitMove = true;

        this.movingTime = 0;
        this.charging = false;
        this.isShake = false;
        this.TargetTalent = null;

        this.grassShowDuration = 3f;
        this.lastAttackStateFrame = 0;
        this.isLastingAttackState = false;
        this.isInsideGrass = false;

        this.RoleST = RoleState.Base;
        this.IsDeadBody = false;
        this.IsAIMode = true;
        this.basicAttributesInited = false;
        this.cacheSkillIndex = 0;
        this.attackAIMode = AttackAIMode.AutoSelectSkill;
        this.Config = config;

        this.Hero = hero;
        this.ExtraData = extraData;
        this.Data = data;

        this.RoleSkin = new RoleSkin(this.Config.RoleSkinID);
        this.RoleSkin.RoleSkinRoleReplace(this.Data);
        this.Orientation = orientation.normalized;
        UFloat.Round(ref base.Orientation);
        this.InitQuality();
        this.Side = side;
        this.AntiSide = RelativeSide.Enemy;
        this.Radius = UFloat.RoundToInt(this.Data.CollideRadius * this.Config.Scale);
        this.DeployTime = data.DeployTime;
        this.IsBoss = config.IsBoss;
        this.LifeTime = float.NaN;
        if (this.Data.LifeTime > 0)
        {
            this.LifeTime = this.Data.LifeTime;
        }
        this.Visible = true;
        if (!string.IsNullOrEmpty(this.Data.Model))
        {
            this.ModelStack.Push(this.Data.Model);
            ModelName = this.Data.Model;
            //            this.ModelStack[1] = (string)this.Data.Model;
            //            ModelName = this.ModelStack[1];
        }

        isCloseAutoAttack = false;

        activeTalentReadyToGo = true;
        this.TransformGroupId = this.Data.PeriodGroup;
        if (this.Data.DSFactor > 0)
            this.dsFactor = this.Data.DSFactor;

        this.InitTalent();
        this.InitAttributes();
        this.InitExtraInfo();

        this.Idle();
        if (this.Data.PeriodGroup > 0)
            this.SetTransformInfo();

        this.Position = UVector2.zero;
        this.Position.x = pos.x;
        this.Position.y = pos.y;
        this.Height = (int)(pos.z + this.Data.FlyHeight);

        this.IsAirForce = this.Data.FlyHeight > 0;

        AddJoint();
        RoleID = data.Name;

        RoleGrowthDatas = NeptuneBattle.Instance.DataProvider.GetRoleGrowthDatas(this.Data.ID);

        talentAbilityDict.Clear();

        if (NeptuneBattle.Instance.Navigator != null)
        {
            if (this.IsAirForce || this.IsRoleType(RoleType.Building))
            {
                //静态物件不使用Unit移动组件
            }
            else
            {
                if (this.NavAgent == null)
                    this.NavAgent = NeptuneBattle.Instance.Navigator.CreateAgent(this, NavArea.AllArea);
                else
                    this.NavAgent.Init();
                //NavAgent.SetWayPoints(pos);
            }
        }

        this.AttachRole();
        BattleMark();

        this.finalKiller = null;
        this.finalKillTime = 0;

        this.CastringEffects.Clear();
    }

    /// <summary>
    /// 
    /// </summary>
    public void AddJoint()
    {
        if (NeptuneBattle.Instance.Scene != null && !NeptuneBattle.Instance.IsNoRenderMode && this.joint == null)
        {
            this.joint = ObjectFactory.Create(this);
            NeptuneBattle.Instance.Scene.AddJoint(this.Joint);
        }
    }
    public override void Delete()
    {
        NObjectPool<BattleActor>.Delete(this);
    }

    public override void OnDelete()
    {
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i];
            if (talent != null)
            {
                this.RemoveActiveTalentAt(i);
            }
        }
        this.TriggerOnDamageTalents.Clear();
        this.TriggerOnRoleDeathTalents.Clear();
        this.ChildTalents.Clear();
        this.ClearAbilities();
        this.RemoveAllMark();
        this.ClearOrcaSimulator();
    }

    /// <summary>
    /// 设置AstartNode属性
    /// </summary>
    /// <param name="isBuilding"></param>
    protected virtual void BattleMark(bool isBuilding = true)
    {
        // --------------------------------- < 建筑物不生成碰撞盒 > ---------------------------------
        //if (Data.RoleType == RoleType.Building)
        //{
        //    NavLayer.NavMap.SetAdditionalArea(Position, Radius, NavArea.Obstacle, isBuilding);

        //    List<Vector2> vertices = NavLayer.NavMap.GetAddtionalVertices(Position, Radius);

        //    if (isBuilding)
        //    {
        //        obstacleId = Logic.Instance.Simulator.AddObstacle(vertices,OrcaObstacleStatus.DYNAMIC);
        //    }
        //    else
        //    {
        //        Logic.Instance.Simulator.RemoveObstacle(obstacleId);
        //    }

        //    Logic.Instance.Simulator.ProcessObstacles(OrcaObstacleStatus.DYNAMIC);
        //}
    }

    public void AttachRole()
    {
        if (this.Data.AttachedRole == 0)
            return;
        Vector3 attachPos = this.Position;
        if (this.Data.AttachedPosition != null && this.Data.AttachedPosition.Count == 3)
        {
            attachPos = attachPos + new Vector3(this.Data.AttachedPosition[0], this.Data.AttachedPosition[1], this.Data.AttachedPosition[2]);
        }

        RoleConfig config = new RoleConfig();
        config.IsAttached = true;

        RoleData data = BattleActor.LoadRoleData(this.Data.AttachedRole);
        RoleInfo hero = new RoleInfo();
        hero.level = this.Hero.level;
        hero.rank = this.Hero.rank;
        hero.stars = this.Hero.stars;
        hero.tid = this.Data.AttachedRole;
        this.AttachedRole = new BattleActor();
        this.AttachedRole.Create(hero, data, this.Side, config, this.ExtraData, attachPos, this.Orientation);
        this.AttachedRole.Player = this.Player;
    }

    public BattleActor(SummonData summon, bool reverse, BattleActor owner)
    {
        this.SummonData = summon;
        this.Owner = owner;
        this.IsReverse = reverse;
        this.Position.x = this.SummonData.PositionX;
        this.Position.y = this.SummonData.PositionY;
        this.SetModel(this.SummonData.Model);
        this.Visible = true;

        RoleConfig config = new RoleConfig();
        config.IsMonster = true;
        this.Config = config;
        AddJoint();
        this.SetModel(this.SummonData.Model);

        talentAbilityDict.Clear();
    }

    public virtual void InitSummon(SummonData summon, RoleConfig config, BattleActor owner, int level = 1)
    {
        this.SummonData = summon;
        this.Owner = owner;
        this.Hero.level = level;
        this.IsAIMode = true;
        this.Visible = true;
        this.Config = config;
        this.ParentAttributes = this.Owner.Attributes;
        this.ParentAttributes.Init();
        InitAttributes();
        this.Position.x += this.SummonData.PositionX;
        this.Position.y += this.SummonData.PositionY;
        if (this.NavAgent != null)
        {
            this.Position = this.NavAgent.GetTeleportPos(this.Position, this.Position);//this.NavAgent.GetObstacleHit(this.Position, this.Position, true);
            if (this.OrcaAgent != null)
            {
                this.OrcaAgent.SetPosition(this.Position);
                this.OrcaAgent.Sync();
            }
            if (this.Joint != null)
                this.Joint.SetPosition(this.Position);
        }

    }
    void InitSummonAttr()
    {
        if (this.SummonData == null)
            return;
        for (int i = 1; i < (int)RoleAttribute.MAX; i++)
        {
            this.AllAttributes[(int)AttributeType.Base][i] += this.ParentAttributes[i] * this.SummonData[i];
        }
    }

    /// <summary>
    /// 获取单位全名
    /// </summary>
    public override string FullName
    {
        get { return string.Format("[{0}{1}]{2}", this.SideSign, this.IsDead ? "D" : "", this.Name); }
    }

    public BitArray RemoveState { get; set; }

    Dictionary<int, float> attributeDic = new Dictionary<int, float>((int)RoleAttribute.MAX);

    /// <summary>
    /// 初始化技能
    /// </summary>
    protected virtual void InitTalent()
    {
        this.AttackRange = 0;
        this.ActiveTalent = null;
        this.tActiveTalents.Clear();
        //this.activeTalentsDict.Clear();
        this.PassiveTalents.Clear();
        this.AuraTalents.Clear();
        this.EnemyAuraTalents.Clear();
        this.Talents.Clear();
        this.ChildTalents.Clear();
        this.TriggerOnRoleDeathTalents.Clear();
        this.TriggerOnDamageTalents.Clear();
        this.TriggerOnMoveTalent.Clear();
        float tlv = this.QualityData != null ? this.QualityData.SkillLevel : 0;

        //tlv = this.Gears.Aggregate(tlv, (current, gear) => current + gear.Data.SkillLevel + gear.Data.PlusSkillLevel * gear.Level);


        Dictionary<int, TalentGroupData> talentGroups = NeptuneBattle.Instance.DataProvider.GetTalentGroups(this.Data.ID);
        if (talentGroups == null)
        {
            Debug.LogErrorFormat("{0} not found in TalentGroup", this.Data.ID);
            return;
        }

        TalentGroupData LegendData = null;
        LegendAttributeData legend = NeptuneBattle.Instance.DataProvider.GetLegendAttributeData(this.Hero.tid);
        if (IsLegend(this.Hero) && legend != null)
        {

            attributeDic.Clear();
            attributeDic[(int)RoleAttribute.Strength] = this.Hero.legend.StrVar;
            attributeDic[(int)RoleAttribute.Intelligence] = this.Hero.legend.IntVar;
            attributeDic[(int)RoleAttribute.Agility] = this.Hero.legend.AgiVar;


            if (attributeDic[(int)legend.Attribute1LimitType] >= legend.Attribute1LimitValue)
            {
                if (legend.Attribute1Type == RoleAttribute.SkillLevel)
                    tlv = tlv + legend.Attribute1Amount;
            }
            if (attributeDic[(int)legend.Attribute2LimitType] >= legend.Attribute2LimitValue)
            {
                if (legend.Attribute2Type == RoleAttribute.SkillLevel)
                    tlv = tlv + legend.Attribute2Amount;
            }
            if (attributeDic[(int)legend.Attribute3LimitType] >= legend.Attribute3LimitValue)
            {
                if (legend.Attribute3Type == RoleAttribute.SkillLevel)
                    tlv = tlv + legend.Attribute3Amount;
            }
            LegendData = talentGroups.Values.Single(t => t.Legend);
        }

        foreach (KeyValuePair<int, TalentGroupData> kv in talentGroups)
        {
            InitTalent(kv.Key, kv.Value, tlv, LegendData);
        }
        if (tActiveTalents.Length > 1)
            tActiveTalents.Sort(new TalentPriorityComparer());
        this.BasicTalent = GetTalentById(this.Data.BasicTalent);
        this.AttackRange = this.BasicTalent != null ? this.BasicTalent.MaxRange : 0;
#if BATTLE_LOG

        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i];
            if (talent != null)
            {
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("{0}: {1} --> BaseNum:{2:f6} ", this.FullName, talent.FullName(), talent.Data.BasicNum);
            }
        }
#endif
    }


    protected void InitTalent(int id, TalentGroupData data, float tlv, TalentGroupData legendData)
    {
        if (data.Legend && !BattleActor.IsLegend(this.Hero))
            return;
        if ((this.Config.IsMonster && this.Quality >= data.UnlockMon) || (!this.Config.IsMonster && this.Quality >= data.Unlock))
        {
            int level = data.InitLevel;
            if ((data.TalentGroupID == this.Data.BasicTalent || data.ParentID == this.Data.BasicTalent || data.Index == 1) && level == 0)
            {
                level = 1;
            }
            else
            {
                if (this.Config.PredictMaxQuality)
                    level = this.Level;
                else if (this.Config.PredictTalent)
                    level = this.Level;
                else if (this.Hero.skillLevels != null && this.Hero.skillLevels.Count > 0)
                {
                    level = (int)((this.Hero.skillLevels.Count >= id ? this.Hero.skillLevels[(int)id - 1].level : 0) + tlv);
                    if (data.ParentID != 0)
                    {
                        int slotid = TalentGroupData.GetTalentSlotId(this.Data.ID, data.ParentID);
                        level = (int)((slotid > 0 && this.Hero.skillLevels.Count >= slotid ? this.Hero.skillLevels[slotid - 1].level : 0) + tlv);
                    }
                }
            }

            if (level >= EngineConst.RoleStartLevel)
            {
                int legendlv = level;
                if (legendData != null)
                    legendlv = (int)((this.Hero.skillLevels.Count >= legendData.Index ? this.Hero.skillLevels[(int)legendData.Index - 1].level : 0) + tlv);

                TalentData talentData = GetTalentData(data, level, Hero.tid, legendData, legendlv);
                this.AddTalent(data, level, talentData);

            }
            else
            {
                Debug.LogFormat("Actor [{0}] InitTalent {1} Level Error.", this.ID, data.TalentGroupID);
            }
        }
    }

    public virtual TalentData GetTalentData(TalentGroupData group, int level, int roleID, TalentGroupData legendData = null, int legendlv = 0)
    {
        Dictionary<int, TalentData> talentLevels = NeptuneBattle.Instance.DataProvider.GetTalentLevelDatas(group.TalentGroupID);
        int legend = legendData == null ? 0 : 1;

        TalentData talentdata;
        if (level <= 1 || talentLevels == null || talentLevels.Count == 0)
        {
            //等级小于1 使用Talent表配置 或者 talentLevels没有等级配置
            Dictionary<int, TalentData> talents = NeptuneBattle.Instance.DataProvider.GetTalentDatas(group.TalentGroupID);
            if (!talents.ContainsKey(legend))
            {
                legend = 0;
            }
            talentdata = talents[legend].Clone();
        }
        else
        {
            if (talentLevels.ContainsKey(level))
            {
                talentdata = talentLevels[level].Clone();
            }
            else
            {
                talentdata = talentLevels[1].Clone();
            }
        }

        talentdata.Abilities = new List<AbilityData>();
        if (talentdata.AbilityIDs != null && talentdata.AbilityIDs.Count > 0)
        {
            foreach (int abilityID in talentdata.AbilityIDs)
            {
                AbilityData ability_info = NeptuneBattle.Instance.DataProvider.GetAbilityData(abilityID).Clone();
                talentdata.Abilities.Add(ability_info);
            }
        }

        TalentDataGrowth(group, level, roleID, legendData, legendlv, legend, talentdata);

        //Data adjust
        if (talentdata.CastTimes == 0)
            talentdata.CastTimes = 1;
        return talentdata;
    }

    protected virtual void TalentDataGrowth(TalentGroupData group, int level, int roleID, TalentGroupData legendData, int legendlv, int legend, TalentData talentdata)
    {
    }

    protected BattleSkill AddTalent(TalentGroupData group, int level, TalentData data, BattleActor source = null)
    {
        BattleSkill talent = null;
        switch (data.ActiveType)
        {
            case TalentType.Active:
                talent = ObjectFactory.Create(group, data, this, level);
                talent.IsEnabled = group.InitEnabled;

                if (!ChangeTalentStack.ContainsKey(group.TalentGroupID))
                {
                    ChangeTalentStack[group.TalentGroupID] = new List<int>();
                }
                //Debug.LogFormat("Actor [{0}] InitTalent {1}", this.ID, talent.Data.ID);
                if (group.ChildGroupID > 0)
                {
                    this.AddChildTalent(talent);
                }
                else
                {
                    this.AddActiveTalent(talent);
                    this.AddTriggerOnDamageTalent(talent);
                    this.AddTriggerOnRoleDeathTalent(talent);
                    this.AddTriggerOnMoveTalent(talent);
                    this.Talents[talent.Data.TalentGroupID] = talent;
                    if ((data.TriggerType & TriggerType.Manual) > 0)
                    {
                        this.ActiveTalent = talent;
                    }
                }
                break;
            case TalentType.Passive:
                this.PassiveTalents.Add(new PassiveSkill(data, group, this, null, source));
                break;
            case TalentType.SelfAureole:
                this.AuraTalents.Add(data);
                break;
            case TalentType.EnemyAureole:
                this.EnemyAuraTalents.Add(data);
                break;
        }
        return talent;
    }

    protected void AddChildTalent(BattleSkill talent)
    {
        this.ChildTalents.Add(talent.GroupData.TalentGroupID, talent);
    }
    protected void RemoveChildTalent(BattleSkill talent)
    {
        this.ChildTalents.Remove(talent.GroupData.TalentGroupID);
        talent.Delete();
    }

    protected void AddActiveTalent(BattleSkill talent)
    {
        this.tActiveTalents.Add(talent);
    }

    protected void AddTriggerOnDamageTalent(BattleSkill talent)
    {
        if (talent == null || (talent.Data.TriggerType & TriggerType.Damage) <= 0)
            return;
        this.TriggerOnDamageTalents.Add(talent);
    }

    protected void AddTriggerOnMoveTalent(BattleSkill talent)
    {
        if (talent == null || (talent.Data.TriggerType & TriggerType.Moving) <= 0)
            return;
        this.TriggerOnMoveTalent.Add(talent);
    }

    protected void AddTriggerOnRoleDeathTalent(BattleSkill talent)
    {
        if (talent == null || (talent.Data.TriggerType & (TriggerType.UnitDeath | TriggerType.FriendDeath | TriggerType.Assist | TriggerType.KillTarget)) <= 0)
            return;
        this.TriggerOnRoleDeathTalents.Add(talent);
    }

    protected void RemoveTriggerOnDamageTalent(BattleSkill talent)
    {
        if (talent == null || (talent.Data.TriggerType & TriggerType.Damage) <= 0)
            return;
        if (this.TriggerOnDamageTalents.Contains(talent))
            this.TriggerOnDamageTalents.Remove(talent);
    }

    protected void RemoveTriggerOnMoveTalent(BattleSkill talent)
    {
        if (talent == null || (talent.Data.TriggerType & TriggerType.Moving) <= 0)
            return;
        if (this.TriggerOnMoveTalent.Contains(talent))
            this.TriggerOnMoveTalent.Remove(talent);
    }
    protected void RemoveTriggerOnRoleDeathTalent(BattleSkill talent)
    {
        if (talent == null || (talent.Data.TriggerType & (TriggerType.UnitDeath | TriggerType.FriendDeath | TriggerType.Assist | TriggerType.KillTarget)) <= 0)
            return;
        if (this.TriggerOnRoleDeathTalents.Contains(talent))
            this.TriggerOnRoleDeathTalents.Remove(talent);
    }
    protected void RemoveActiveTalentAt(int index)
    {
        BattleSkill talent = tActiveTalents[index] as BattleSkill;
        if (talent == null)
        {
            Debug.LogError("No Active Skill in List for Remove: " + index);
            return;
        }
        tActiveTalents.Remove(index);
        talent.Delete();
    }



    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlaySound(string clipName)
    {
        if (Joint != null)
            Joint.PlaySound(clipName);
    }
    public void PlayVoice(string name)
    {
        if (Joint != null)
            Joint.PlayVoice(name);
    }
    public void PlayRandomVoice(List<string> name, bool playnow = false)
    {
        if (Joint != null)
            Joint.PlayRandomVoice(name, playnow);
    }
    /// <summary>
    /// 初始化品质数据
    /// </summary>
    void InitQuality()
    {
        this.Quality = (int)Mathf.Floor(this.Hero.rank > 0 ? this.Hero.rank : 1);

        if (this.Config.PredictMaxQuality)
        {
            this.Quality = 0;
            int qualityLimit = EngineConst.QUALITY_LIMIT;

            for (int quality = 1; quality <= qualityLimit; quality++)
            {
                RoleGearData data = NeptuneBattle.Instance.DataProvider.GetRoleGearData(this.ID, quality);
                if (data != null && data.LvReq <= this.Level)
                {
                    this.Quality = quality < qualityLimit ? quality + 1 : qualityLimit;
                }
            }
        }
        else if (this.Config.PredictQuality)
        {
            float qualityLv1 = this.Level * 0.1f;
            this.Quality = (int)Mathf.Floor(qualityLv1 + 0.9f);
        }

        int lmt = EngineConst.QUALITY_LIMIT;
        this.Quality = this.Quality > lmt ? lmt : this.Quality;
        this.QualityRatio = 0;

        if (this.Config.PredictQuality)
        {
            this.QualityRatio = (this.Level - 1) % 10f * 0.1f;
        }

        if (this.Quality > 0)
        {
            this.GearData = NeptuneBattle.Instance.DataProvider.GetRoleGearData((int)this.ID, (int)this.Quality);
            if (this.Quality <= 0)
            {
                return;
            }

            Dictionary<int, RoleQualityData> qualityData = NeptuneBattle.Instance.DataProvider.GetRoleQualityDatas(this.ID);
            if (qualityData != null)
            {
                this.QualityData = qualityData[(int)this.Quality];
                if (qualityData.ContainsKey((int)this.Quality + 1))
                {
                    this.NextLvQualityData = qualityData[(int)this.Quality + 1];
                }
            }
        }
    }
    /// <summary>
    /// 初始化扩展信息
    /// </summary>
    /// <param name="inited"></param>
    void InitExtraInfo(bool inited = false)
    {
        if (this.IsExtraInited)
            return;

        if (inited)
            this.IsExtraInited = true;
        float mp = this.Attributes.MaxMP;
        float rage = this.Attributes.MaxRage;
        if (this.ExtraData != null)
        {
            mp = this.Attributes.MaxMP * this.ExtraData.MP / EngineConst.PercentageRatio;
            //rage = this.Attributes.MaxRage * this.ExtraData.Rage / EngineConst.PercentageRatio;
        }

        if (this.ExtraData != null && this.ExtraData.HP > 0)
        {
            this.SetHP(this.Attributes.MaxHP * this.ExtraData.HP / EngineConst.PercentageRatio);
        }
        else
        {
            this.SetHP(this.Attributes.MaxHP);
        }

        if (this.MonIdx == 0)
        {
            this.SetMP(mp);
            //this.SetRage(rage);
        }
    }

    protected bool basicAttributesInited = false;
    /// <summary>
    /// 重新构建单数值位属性
    /// </summary>
    public virtual void InitAttributes(Dictionary<int, float> extraAttributes = null)
    {
        float lastHpPre = this.Attributes.MaxHP > 0 ? UFloat.Round(this.HP / this.Attributes.MaxHP) : 0;
        float lastMpPre = this.Attributes.MaxMP > 0 ? UFloat.Round(this.MP / this.Attributes.MaxMP) : 0;
        //初始化基础属性
        //if (!BasicAttributes.Inited)
        if (!this.basicAttributesInited)
        {
            this.InitBaseAttributes();
            this.BasicAttributes = this.AllAttributes[(int)AttributeType.Base];
            this.BasicAttributes.Init();
            this.basicAttributesInited = true;
        }
        else
        {
            this.AllAttributes[(int)AttributeType.Base] = this.BasicAttributes;
            this.AllAttributes[(int)AttributeType.Base].Init();
        }

        //初始化角色成长
        this.AttributeGrowth();

        //召唤物属性计算
        this.InitSummonAttr();

        //初始化附加属性(扩展层自定义，通常为装备)
        this.InitAttributesAddition();

        //追加额外属性
        if (null != extraAttributes)
            foreach (int key in extraAttributes.Keys) { this.AllAttributes[(int)AttributeType.Extra][(int)key] += extraAttributes[key]; }

        //初始化被动技能、光环技能、BUFF属性加成
        this.InitTalentExtensionAddition();

        if (this.Reentrancy)
            return;

        //追加配置属性
        InitRoleConfig();

        //最终属性转化   VALUE = VALUE + VALUE * RATIO
        this.InitFinalAttributes();

        //主属性转物理攻击计算
        this.InitMainAttribAddition();

        this.AllAttributes[(int)AttributeType.Final].MaxHP = UFloat.RoundToInt(this.AllAttributes[(int)AttributeType.Final].MaxHP * this.Config.HPFactor);
        this.AllAttributes[(int)AttributeType.Final].MaxMP = UFloat.RoundToInt(this.AllAttributes[(int)AttributeType.Final].MaxMP);
        this.AllAttributes[(int)AttributeType.Final].MaxRage = UFloat.RoundToInt(this.AllAttributes[(int)AttributeType.Final].MaxRage);
#if BATTLE_LOG
        for (int i = 0; i < (int)RoleAttribute.RATIOMAX; i++)
        {
            if (EngineGlobal.BattleLog && this.Attributes[i] != 0)
                NeptuneBattle.log("{0} FINAL: {1}:{2:f6} {3:f6}", this.FullName, ((RoleAttribute)i).Name(), this.AllAttributes[(int)AttributeType.Final][i], this.AllAttributes[(int)AttributeType.Base][i]);
        }
#endif  
        float powerFactor = this.AllAttributes[(int)AttributeType.Final].PowerFactor;
        this.AllAttributes[(int)AttributeType.Final].PowerFactor = Mathf.Max(0, Mathf.Min(this.Config.DmgPerSecFactor, powerFactor));
        //皮肤额外属性
        this.RoleSkin.ExtraAttr(this.Attributes);

        float lastHp = UFloat.RoundToInt(UFloat.Round(lastHpPre * this.Attributes.MaxHP));
        float oldhp = this.HP;
        if (lastHp > this.HP)
        {
            this.HP = lastHp;
        }
        float lastMp = UFloat.RoundToInt(UFloat.Round(lastMpPre * this.Attributes.MaxMP));
        if (lastMp > this.MP)
        {
            this.MP = lastMp;
        }
#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("    {0} HPReset {1:F6} = {2:F6} >> {3:F6} = {4:F6} * {5:F6}", this.FullName, this.HP, oldhp, lastHp, lastHpPre, this.Attributes.MaxHP);
#endif

        this.HP = Math.Min(this.HP, this.Attributes.MaxHP);
        this.MP = Math.Min(this.MP, this.Attributes.MaxMP);
        this.Rage = Math.Min(this.Rage, this.Attributes.MaxRage);
        this.Point = Math.Min(this.Point, this.Attributes.MaxPoint);
        AdjustMoveSpeed();
        CheckAttrLegality();


    }

    // 限定最小移动速度为150；最大移动速度800,；根据速度值所在区间进行调整，低速时提高增速，高速时削减增速
    protected virtual void AdjustMoveSpeed()
    {
        float minMoveSpeed = 150.0f;
        float maxMoveSpeed = 800.0f;
        this.AllAttributes[(int)AttributeType.Final].MoveSpeed = UFloat.RoundToInt(Math.Max(minMoveSpeed, this.Attributes.MoveSpeed));
        this.AllAttributes[(int)AttributeType.Final].MoveSpeed = UFloat.RoundToInt(Math.Min(maxMoveSpeed, this.Attributes.MoveSpeed));
        if (this.Attributes.MoveSpeed > 490)
        {
            this.AllAttributes[(int)AttributeType.Final].MoveSpeed = UFloat.RoundToInt(490 + (this.Attributes.MoveSpeed - 490) * 0.5f);
        }
        else if (this.Attributes.MoveSpeed < 300)
        {
            this.AllAttributes[(int)AttributeType.Final].MoveSpeed = UFloat.RoundToInt(minMoveSpeed + this.Attributes.MoveSpeed * 0.5f);
        }
    }

    protected virtual void CheckAttrLegality()
    {
        this.AllAttributes[(int)AttributeType.Final].AbilityPower = UFloat.RoundToInt(Mathf.Max(0, this.Attributes.AbilityPower));
        this.AllAttributes[(int)AttributeType.Final].AttackDamage = UFloat.RoundToInt(Mathf.Max(0, this.Attributes.AttackDamage));
        this.AllAttributes[(int)AttributeType.Final].Critical = UFloat.RoundToInt(Mathf.Max(0, this.Attributes.Critical));
        this.AllAttributes[(int)AttributeType.Final].MagicCritical = UFloat.RoundToInt(Mathf.Max(0, this.Attributes.MagicCritical));
        this.AllAttributes[(int)AttributeType.Final].AttackSpeed = UFloat.RoundToInt(Mathf.Max(-50, Math.Min(150, this.Attributes.AttackSpeed)));
    }

    public virtual void InitRoleConfig()
    {
        this.AllAttributes[(int)AttributeType.Base].MaxHP += this.Config.MaxHP;
        this.AllAttributes[(int)AttributeType.Base].AttackDamage += this.Config.AttackDamage;
        this.AllAttributes[(int)AttributeType.Base].Armor += this.Config.Armor;
        this.AllAttributes[(int)AttributeType.Base].MagicResistance += this.Config.MagicResistance;
    }

    /// <summary>
    /// 初始化基本属性
    /// </summary>
    void InitBaseAttributes()
    {
        for (int i = 0; i < (int)AttributeType.Max; i++)
        {
            this.AllAttributes[i].Init();
        }
        for (int i = 0; i < (int)RoleAttribute.RATIOMAX; i++)
        {
            int key = i;
            if (this.Config.PredictQuality)
            {
                //key = RoleAttributes.GetPredictNameField(i);
            }

            //float propertyValue = this.Data[RoleAttributes.GetPSNameField(i, this.Stars)];
            //float value = this.Data[i] + propertyValue * this.Level;
            float value = this.Data[i];
            if (this.QualityData != null)
            {
                float qualityValue = this.QualityData[key];
                float nextQualityValue = this.NextLvQualityData != null && this.NextLvQualityData[key] != 0 ? this.NextLvQualityData[key] : qualityValue;
                value = UFloat.Round(value + qualityValue + (nextQualityValue - qualityValue) * this.QualityRatio);
            }
            this.AllAttributes[(int)AttributeType.Base][i] = value;

#if BATTLE_LOG
            if (EngineGlobal.BattleLog && value != 0)
                NeptuneBattle.log("{0} ATTR: {1}:{2:f6} R:{3:f6}", this.FullName, ((RoleAttribute)i).Name(), value, this.QualityRatio);
#endif

        }
#if BATTLE_LOG
        //if (EngineGlobal.BattleLog)
        //    Logic.log("{0} rebuild ATTR:\n{1}\n", this.FullName, this.Attributes.ToString());
#endif
        this.AllAttributes[(int)AttributeType.Base].PowerFactor = 1;

        this.InitStarAddition();
        this.InitLegendAddition();
    }

    void InitStarAddition()
    {
        if (this.Stars > 1 && this.IsRoleType(RoleType.Monster))
        {
            for (int i = 1; i < (int)RoleAttribute.MAX; i++)
            {
                if (EngineConst.MonsterAttrs.Contains(i))
                {
                    float value = UFloat.Round(this.Attributes[i] * (7f + this.Stars) / 8f);
                    this.AllAttributes[(int)AttributeType.Base][i] = value;
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("{0} MONS: {1}:{2:f6}", this.FullName, (RoleAttribute)i, value);
#endif
                }
            }
        }
    }


    public virtual void InitAttributesAddition()
    {
        this.AllAttributes[(int)AttributeType.Extra].Reset();
    }

    public virtual void InitTalentExtensionAddition()
    {
        this.AllAttributes[(int)AttributeType.Ability].Reset();

        for (int i = 0; i < this.PassiveTalents.Length; i++)
        {
            PassiveSkill talentData = this.PassiveTalents[i];
            if (talentData != null)
            {
                if (talentData.Data.PassiveAttr > RoleAttribute.RATIOBASE && talentData.Data.RatioByBase)
                {
                    this.Owner.AllAttributes[(int)AttributeType.Ability][RoleAttributes.GetBaseValueField((int)talentData.Data.PassiveAttr)] = this.Owner.AllAttributes[RoleAttributes.GetBaseValueField((int)AttributeType.Base)][(int)talentData.Data.PassiveAttr] * talentData.Data.BasicNum;
                }
                else
                    this.AllAttributes[(int)AttributeType.Ability][(int)talentData.Data.PassiveAttr] = this.AllAttributes[(int)AttributeType.Ability][(int)talentData.Data.PassiveAttr] + talentData.Data.BasicNum;
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("{0} PASV: {1}:{2:f6}", this.FullName, talentData.Data.PassiveAttr, this.AllAttributes[(int)AttributeType.Ability][(int)talentData.Data.PassiveAttr]);
#endif
            }
        }

        if (NeptuneBattle.Instance.Enabled)
        {
            SafeList<BattleActor> list = NeptuneBattle.Instance.AliveRoles[(int)RoleSide.All];
            for (int i = list.Count - 1; i >= 0; i--)
            {
                BattleActor role = list[i];
                if (list.IsRemoved(role))
                    continue;
                if (role.Side == this.Side)
                {
                    //友方
                    foreach (TalentData data in role.AuraTalents)
                    {
                        if (data != null && (string.IsNullOrEmpty(data.AuraAffectTag) || this.Data.HeroTags.Contains(data.AuraAffectTag)))
                        {
                            if (data.PassiveAttr > RoleAttribute.RATIOBASE && data.RatioByBase)
                            {
                                this.Owner.AllAttributes[(int)AttributeType.Ability][RoleAttributes.GetBaseValueField((int)data.PassiveAttr)] = this.Owner.AllAttributes[RoleAttributes.GetBaseValueField((int)AttributeType.Base)][(int)data.PassiveAttr] * data.BasicNum;
                            }
                            else
                                this.AllAttributes[(int)AttributeType.Ability][(int)data.PassiveAttr] = UFloat.Round(this.AllAttributes[(int)AttributeType.Ability][(int)data.PassiveAttr] + data.BasicNum);
#if BATTLE_LOG
                            if (EngineGlobal.BattleLog)
                                NeptuneBattle.log("{0} AURA: {1}:{2:f6}", this.FullName, data.PassiveAttr, this.AllAttributes[(int)AttributeType.Ability][(int)data.PassiveAttr]);
#endif
                        }
                    }
                }
                else
                {
                    //敌方
                    foreach (TalentData data in role.EnemyAuraTalents)
                    {
                        if (data != null && (string.IsNullOrEmpty(data.AuraAffectTag) || this.Data.HeroTags.IndexOf(data.AuraAffectTag) > 0))
                        {
                            if (data.PassiveAttr > RoleAttribute.RATIOBASE && data.RatioByBase)
                            {
                                this.Owner.AllAttributes[(int)AttributeType.Ability][RoleAttributes.GetBaseValueField((int)data.PassiveAttr)] = this.Owner.AllAttributes[RoleAttributes.GetBaseValueField((int)AttributeType.Base)][(int)data.PassiveAttr] * data.BasicNum;
                            }
                            else
                                this.AllAttributes[(int)AttributeType.Ability][(int)data.PassiveAttr] = UFloat.Round(this.AllAttributes[(int)AttributeType.Ability][(int)data.PassiveAttr] + data.BasicNum);
#if BATTLE_LOG
                            if (EngineGlobal.BattleLog)
                                NeptuneBattle.log("{0} EAURA: {1}:{2:f6}", this.FullName, data.PassiveAttr, this.AllAttributes[(int)AttributeType.Ability][(int)data.PassiveAttr]);
#endif
                        }
                    }
                }



            }
            list.Final();


        }

        for (int i = 0; i < this.tAbilities.Length; i++)
        {
            Ability ability = this.tAbilities[i] as Ability;
            if (ability != null)
                ability.InitAbility();
        }
    }



    public virtual void InitEffects()
    {
        this.AbilityEffects.Reset();

        for (int i = 0; i < this.tAbilities.Length; i++)
        {
            Ability ability = this.tAbilities[i] as Ability;
            if (ability != null)
                ability.ReBuild();
        }

        InvisibleProcess();
        for (int i = 0; i < (int)ControlEffect.MAX; i++)
        {
            this.RemoveConflictEffect((ControlEffect)i);
        }

        if (this.AbilityEffects.Root && this.RoleST == RoleState.Move)
        {
            this.Idle();
        }
        bool isBreak = false;
        if (this.CurrentTalent != null && this.CurrentTalent.Casting)
        {

            if (this.AbilityEffects.Solidifying)
            {
                isBreak = true;
            }
            else if (this.CurrentTalent.CanInterrupt(InterruptType.Passive))
            {
                if ((!this.CurrentTalent.Data.NoSilence && (this.AbilityEffects.OnlyNormalAttack ||
                    (this.CurrentTalent.Data.DamageType == InjuryType.AttackDamage && this.AbilityEffects.Disable) ||
                    (this.CurrentTalent.Data.DamageType != InjuryType.AttackDamage && this.AbilityEffects.Inhibition)))
                    || (this.BasicTalent != this.CurrentTalent && this.AbilityEffects.MindChain) || (this.AbilityEffects.Disarmed && this.CurrentTalent.Data.SkillType == SkillType.Normal))
                {
                    isBreak = true;
                }
            }

            if (isBreak)
            {
                this.Reentrancy = true;
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("{0} InitTalentExtensionAddition break", this.CurrentTalent.ToString());
#endif
                this.CurrentTalent.Break();
                this.Reentrancy = false;
            }


        }


        if (this.AbilityEffects.Sleep)
        {
            this.Fatal();
            if (this.RoleST == RoleState.Idle || this.RoleST == RoleState.Damaged)
            {
                this.Stun();
            }
        }

        else
        {
            if (!this.IsDead && this.RoleST == RoleState.Stun)
            {
                this.Idle();
            }
        }
    }

    /// <summary>
    /// 技能被打断的回调
    /// </summary>
    public void CallbackByBreakTalent()
    {
        if (callbackByBreakTalent != null)
            callbackByBreakTalent();
    }

    public void RemoveConflictEffect(ControlEffect effect)
    {
        if (this.AbilityEffects[(int)effect] && EngineConst.ConflictAbilities.ContainsKey((int)effect))
        {
            foreach (ControlEffect key in EngineConst.ConflictAbilities[(int)effect].Keys)
            {
                if (this.AbilityEffects[(int)key])
                {
                    RemoveConflictEffect(key);
                    this.AbilityEffects[(int)key] = false;
                }

            }
        }
    }

    void InitMainAttribAddition()
    {
        if (this.Data.MainAttrib != RoleAttribute.None)
        {
            this.AllAttributes[(int)AttributeType.Final].AttackDamage = UFloat.RoundToInt(this.Attributes.AttackDamage + this.Attributes[(int)this.Data.MainAttrib]);
        }
    }

    void InitLegendAddition()
    {
        LegendAttributeData legendattr = NeptuneBattle.Instance.DataProvider.GetLegendAttributeData(this.Hero.tid);
        if (BattleActor.IsLegend(this.Hero) && legendattr != null)
        {
            Dictionary<int, float> legendattrmap = new Dictionary<int, float>()
            {
              {(int)RoleAttribute.Strength , this.Hero.legend.Str},
              {(int)RoleAttribute.Intelligence , this.Hero.legend.Int},
              {(int)RoleAttribute.Agility, this.Hero.legend.Agi}
            };
            this.AllAttributes[(int)AttributeType.Base].Strength = this.Attributes.Strength + this.Hero.legend.Str;
            this.AllAttributes[(int)AttributeType.Base].Intelligence = this.Attributes.Intelligence + this.Hero.legend.Int;
            this.AllAttributes[(int)AttributeType.Base].Agility = this.Attributes.Agility + this.Hero.legend.Agi;

            if (legendattrmap[(int)legendattr.Attribute1LimitType] >= legendattr.Attribute1LimitValue)
            {
                this.AllAttributes[(int)AttributeType.Base][(int)legendattr.Attribute1Type] = this.Attributes[(int)legendattr.Attribute1Type] + legendattr.Attribute1Amount;
#if BATTLE_LOG
                //if (EngineGlobal.BattleLog)
                //    Logic.log("{0} LEGEND: {1}:{2}", this.FullName, field, this.Attributes[field]);
#endif
            }
            if (legendattrmap[(int)legendattr.Attribute2LimitType] >= legendattr.Attribute2LimitValue)
            {
                this.AllAttributes[(int)AttributeType.Base][(int)legendattr.Attribute2Type] = this.Attributes[(int)legendattr.Attribute2Type] + legendattr.Attribute2Amount;
#if BATTLE_LOG
                //if (EngineGlobal.BattleLog)
                //    Logic.log("{0} LEGEND: {1}:{2}", this.FullName, field, this.Attributes[field]);
#endif
            }
            if (legendattrmap[(int)legendattr.Attribute3LimitType] >= legendattr.Attribute3LimitValue)
            {
                this.AllAttributes[(int)AttributeType.Base][(int)legendattr.Attribute3Type] = this.Attributes[(int)legendattr.Attribute3Type] + legendattr.Attribute3Amount;
#if BATTLE_LOG
                //if (EngineGlobal.BattleLog)
                //    Logic.log("{0} LEGEND: {1}:{2}", this.FullName, field, this.Attributes[field]);
#endif
            }
        }
    }

    void InitFinalAttributes()
    {
        this.HP = Math.Min(this.HP, this.AllAttributes[(int)AttributeType.Final].MaxHP);
        this.AllAttributes[(int)AttributeType.Final].LostHPValue = UFloat.Round((this.MaxHP - this.HP));
        this.AllAttributes[(int)AttributeType.Final].LostHP = UFloat.Round(this.AllAttributes[(int)AttributeType.Final].LostHPValue / this.MaxHP);
        for (int i = 0; i < this.tAbilities.Length; i++)
        {
            Ability ability = this.tAbilities[i] as Ability;
            if (ability != null)
                ability.TransformAttributes();
        }
        for (int i = 0; i < (int)RoleAttribute.MAX; i++)
        {
            int key = RoleAttributes.GetRatioValueField(i);
            AllAttributes[(int)AttributeType.Final][i] = AllAttributes[(int)AttributeType.Base][i] + AllAttributes[(int)AttributeType.Extra][i] + AllAttributes[(int)AttributeType.Ability][i];
            AllAttributes[(int)AttributeType.Final][key] = AllAttributes[(int)AttributeType.Base][key] + AllAttributes[(int)AttributeType.Extra][key] + AllAttributes[(int)AttributeType.Ability][key];

            if (EngineConst.RatioSeparation.ContainsKey(i))
            {
                continue;
            }

            this.AllAttributes[(int)AttributeType.Final][i] = UFloat.Round(this.AllAttributes[(int)AttributeType.Final][i] + this.AllAttributes[(int)AttributeType.Final][i] * (this.AllAttributes[(int)AttributeType.Final][key] * EngineConst.Hundredth));
        }
    }

    /// <summary>
    /// 角色属性等级成长
    /// </summary>
    protected virtual void AttributeGrowth()
    {
        if (RoleGrowthDatas == null)
        {
            return;
        }
        if (!RoleGrowthDatas.ContainsKey(this.Level))
        {
            Logger.LogError(string.Format("No AttributeGrowth Data Level [{0}] ", this.Level));
            return;
        }

        bool byLevel = RoleGrowthDatas.ContainsKey(0);
        //升级如果配置表配置0级则按照角色 Level 计算出成长值，否则直接从配置表中读取对应等级成长加值
        RoleGrowthData roleGrowthData = RoleGrowthDatas[byLevel ? 0 : this.Level];

        for (int i = 0; i < (int)RoleAttribute.MAX; i++)
        {
            if (roleGrowthData[i] == 0)
            {
                continue;
            }
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} AttributeGrowth_A: {1}:{2:f6}", this.FullName, (RoleAttribute)i, this.AllAttributes[(int)AttributeType.Base][(int)i]);
#endif
            if (byLevel)
            {
                //根据角色等级计算
                //基础属性成长
                this.AllAttributes[(int)AttributeType.Base][i] = UFloat.Round(this.AllAttributes[(int)AttributeType.Base][i] + this.GrowthData[i] * this.Level);
                //百分比成长
                int key = RoleAttributes.GetRatioValueField(i);
                float ratio = 1;
                for (int index = 1; index <= this.Level; index++)
                {
                    ratio = UFloat.Round((1 + this.GrowthData[key] * 0.01f) * ratio);
                }
                this.AllAttributes[(int)AttributeType.Base][i] = UFloat.Round(this.AllAttributes[(int)AttributeType.Base][i] + this.Data[i] * ratio);
            }
            else
            {
                //配置表读取
                //基础属性成长
                this.AllAttributes[(int)AttributeType.Base][i] = this.AllAttributes[(int)AttributeType.Base][i] + roleGrowthData[i];
            }

#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} AttributeGrowth_E: {1}:{2:f6}", this.FullName, (RoleAttribute)i, this.AllAttributes[(int)AttributeType.Base][(int)i]);
#endif
        }
    }


    /// <summary>
    /// 重置单位属性
    /// </summary>
    public virtual void Init(bool isReborn = false)
    {
        if (this.IsDead)
            this.RoleST = RoleState.Dead;
        else
            this.RoleST = RoleState.Idle;
        this.CommonCooldown = 0;
        this.CurrentTalent = null;
        this.IsActiveCasting = false;
        this.AniId = string.Empty;
        this.IsDeadBody = false;
        this.AniTotaltime = 0;
        this.AniPlayedTime = 0;
        this.RepelSpeed = UVector2.zero;
        this.RepelTime = 0;
        if (!isReborn)
        {
            this.AbilityEffects.Reset();
            this.tAbilities.Clear();
            //this.Abilities.Clear();
            this.AbilitiesDict.Clear();
            this._marksDic.Clear();
        }
        this.attackerMap.Clear();
        //foreach (Skill talent in this.ActiveTalents)
        //{
        //    talent.Init(!isReborn);
        //}
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i];
            if (talent != null)
            {
                talent.Init(!isReborn);
            }
        }
        this.InitAttributes();
        this.InitExtraInfo(true);


        if (this.Data.PeriodGroup > 0)
            this.SetTransformInfo();
        if (isReborn)
        {
            this.SetAnimation(RoleStateName.Instance[RoleState.Idle], true, false, false);
            if (Joint != null)
            {
                this.Joint.SetPosition(this.Position);
            }
        }

        if (Joint != null && Joint.Controller != null)
        {
            this.Joint.Controller.SetPosition(new Vector3(this.Position.x, this.Position.y, this.Height));
        }
        if (NeptuneBattle.Instance.Simulator != null)
        {
            if (this.OrcaAgent == null)
            {
                this.OrcaAgent = NeptuneBattle.Instance.Simulator.CreateAgent(this, this.Position, this.Radius, UFloat.RoundToInt(this.Attributes.MoveSpeed * this.MoveSpeedMultiplier), RvoContainType.Agent);
            }
            else
            {
                this.OrcaAgent.SetPosition(this.Position);
            }
            this.OrcaAgent.Sync();

        }
    }
    /// <summary>
    /// 重置技能属性
    /// </summary>
    /// <param name="transform"></param>
    protected virtual void ResetTalents(TransformData transform)
    {
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill activeTalent = this.tActiveTalents[i];
            if (activeTalent != null)
            {

                if (transform.TalentList.Contains(activeTalent.Data.TalentGroupID))
                {
                    if (!activeTalent.IsEnabled)
                    {
#if BATTLE_LOG
                        if (EngineGlobal.BattleLog)
                            NeptuneBattle.log("{0} ResetTalents:{1} Init()", this.FullName, activeTalent.FullName());
#endif
                        activeTalent.Init();
                    }
                    activeTalent.IsEnabled = true;
                }
                else
                {
                    if (activeTalent.Casting && activeTalent.CanInterrupt(InterruptType.Passive))
                    {
                        if (EngineGlobal.BattleLog)
                            NeptuneBattle.log("{0} ResetTalents break.", activeTalent.ToString());
                        activeTalent.Break();
                    }
                    activeTalent.IsEnabled = false;
                }
            }
        }

        this.BasicTalent = this.Talents[transform.BasicTalent];
        this.AttackRange = BasicTalent.MaxRange;
        this.InitAttributes();
    }

    /// <summary>
    /// 己方英雄单位信息配置
    /// </summary>
    /// <param name="config"></param>
    /// <param name="tid"></param>
    /// <param name="is_bot"></param>
    public static void InitConfigPredictInfo(RoleConfig config, int tid, bool is_bot)
    {
        config.PredictQuality = is_bot;
        config.PredictTalent = is_bot;
        config.PredictMaxQuality = false;
    }

    /// <summary>
    /// 战斗补给
    /// 在战斗结束给予补给
    /// </summary>
    /// <param name="factor"></param>
    public void Rest(float factor)
    {
        this.Orientation = this.InitDirection;
        if (this.Attributes.HPSupply > 0)
            this.Remedy(this.Attributes.HPSupply * NeptuneBattle.Instance.CrusadeModeRecoverBonus, RoleAttribute.MaxHP);

        if (this.Attributes.MPSupply > 0)
            this.Remedy(this.Attributes.MPSupply * factor * NeptuneBattle.Instance.CrusadeModeRecoverBonus, RoleAttribute.MaxMP);
    }

    /// <summary>
    /// 移除冲突Ability
    /// </summary>
    private void RemoveConlictAbilities()
    {
        //for (int i = this.Abilities.Count - 1; i >= 0; i--)
        //{
        //    Ability ability = this.Abilities[i];
        //    if (ability.IsControlAbility)
        //    {
        //        this.Abilities.RemoveAt(i);
        //        RemoveAbilityCount(ability);
        //        ability.OnDestroyed();
        //    }
        //}

        for (int i = 0; i < this.tAbilities.Length; i++)
        {
            Ability ability = this.tAbilities[i] as Ability;
            if (ability != null && ability.IsControlAbility)
            {
                this.tAbilities.Remove(i);
                RemoveAbilityCount(ability);
                ability.OnDestroyed();
            }
        }
    }
    /// <summary>
    /// 移除指定ID的Ability
    /// </summary>
    /// <param name="abilityId"></param>
    private void RemoveAbilityWithId(int abilityId)
    {
        //for (int i = this.Abilities.Count - 1; i >= 0; i--)
        //{
        //    Ability ability = this.Abilities[i];
        //    if (ability.AbilityData.ID == abilityId)
        //    {
        //        if (this.AbilityHitEffect != null && ability.AbilityData.Name == this.AbilityHitEffect.Name)
        //        {
        //            this.AbilityHitEffect = null;
        //        }
        //        ability.OnDestroyed();
        //        this.Abilities.RemoveAt(i);
        //        RemoveAbilityCount(ability);
        //    }
        //}

        for (int i = 0; i < this.tAbilities.Length; i++)
        {
            Ability ability = this.tAbilities[i] as Ability;
            if (ability != null && ability.AbilityData.ID == abilityId)
            {
                this.tAbilities.Remove(i);
                RemoveAbilityCount(ability);
                ability.OnDestroyed();
            }
        }
    }

    /// <summary>
    /// 根据移除触发类型移除目标移除buff
    /// </summary>
    /// <param name="abilityRemoveType"></param>
    /*public void RemoveAbilitys(AbilityRemoveType abilityRemoveType)
    {
        for (int i = 0; i < this.Abilities.Count; i++)
        {
            Ability ability = this.Abilities[i];
            if ((ability.AbilityData.AbilityRemoveType & (int)abilityRemoveType) > 0)
            {
                if (ability.AbilityData.RemoveByTalentIDs != null)
                {
                    for (int j = 0; j < ability.AbilityData.RemoveByTalentIDs.Count; j++)
                    {
                        //-1 表示所有技能ID
                        if (ability.AbilityData.RemoveByTalentIDs[i] == -1 || this.Data.ID == ability.AbilityData.RemoveByTalentIDs[i])
                        {
                            this.RemoveAbility(ability);
                            continue;
                        }
                    }
                }
            }
        }
    }*/
    //相同ID的ability数量
    public int FindAbilityCountWithId(int abilityId)
    {
        int count = 0;
        //for (int i = this.Abilities.Count - 1; i >= 0; i--)
        //{
        //    Ability ability = this.Abilities[i];
        //    if (ability.AbilityData.ID == abilityId)
        //    {
        //        count++;
        //    }
        //}

        for (int i = 0; i < this.tAbilities.Length; i++)
        {
            Ability ability = this.tAbilities[i] as Ability;
            if (ability != null && ability.AbilityData.ID == abilityId)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// 添加指定BUFF数量
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int AddAbilityCountByID(int id, int count = 1)
    {
        if (!AbilitiesDict.ContainsKey(id))
        {
            AbilitiesDict.Add(id, 0);
        }
        AbilitiesDict[id] += count;
        return AbilitiesDict[id];
    }

    /// <summary>
    /// 删除指定BUFF数量
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int RemoveAbilityCount(Ability ability)
    {
        if (ability == null)
            return 0;
        int id = ability.AbilityData.ID;
        if (AbilitiesDict.ContainsKey(id))
        {
            AbilitiesDict[id]--;
            if (AbilitiesDict[id] <= 0 || ability.AbilityData.Overlay == OverlayType.ResetTime || ability.AbilityData.Overlay == OverlayType.OnlyResetTime)
            {
                AbilitiesDict.Remove(id);
                return 0;
            }
            return AbilitiesDict[id];
        }
        return 0;
    }

    public int RemoveAbilityCount(int ID, int count)
    {
        if (AbilitiesDict.ContainsKey(ID))
        {
            AbilitiesDict[ID] -= count;
            if (AbilitiesDict[ID] <= 0)
            {
                AbilitiesDict.Remove(ID);
                return 0;
            }
            return AbilitiesDict[ID];
        }
        return 0;
    }

    /// <summary>
    /// 返回指定BUFFID的数量
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int GetAbilityCountById(int id)
    {
        if (AbilitiesDict.ContainsKey(id))
        {
            return AbilitiesDict[id];
        }
        return 0;
    }


    private void ResetSameAbilityTime(int abilityId, float time)
    {
        //for (int i = this.Abilities.Count - 1; i >= 0; i--)
        //{
        //    Ability ability = this.Abilities[i];
        //    if (ability.AbilityData.ID == abilityId)
        //    {
        //        ability.Duration = time;
        //    }
        //}

        for (int i = 0; i < this.tAbilities.Length; i++)
        {
            Ability ability = this.tAbilities[i] as Ability;
            if (ability != null && ability.AbilityData.ID == abilityId)
            {
                ability.SetDuration(time);
            }
        }
    }
    private Ability FindAbilityByID(int abilityId)
    {
        //Ability ability = null;
        //for (int i = this.Abilities.Count - 1; i >= 0; i--)
        //{
        //    ability = this.Abilities[i];
        //    if (ability.AbilityData.ID == abilityId)
        //    {
        //        return ability;
        //    }
        //}

        for (int i = 0; i < this.tAbilities.Length; i++)
        {
            Ability ability = this.tAbilities[i] as Ability;
            if (ability != null && ability.AbilityData.ID == abilityId)
            {
                return ability;
            }
        }
        return null;
    }
    /// <summary>
    /// 添加Ability
    /// </summary>
    /// <param name="abilityId">Ability ID</param>
    /// <param name="caster"></param>
    /// <returns></returns>
    public Ability AddAbility(int abilityId, BattleActor caster = null, BattleSkill talent = null)
    {
#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
        {
            if (abilityId == 8001)
                NeptuneBattle.log("AddAbility {0} isDeath {1} role {2}", abilityId, IsDeath.ToString(), this.ID);
        }
#endif
        Ability ability = IsDeath ? null : AddAbility(NeptuneBattle.Instance.DataProvider.GetAbilityData(abilityId), caster, talent);

        return ability;
    }

    /// <summary>
    /// 添加Ability
    /// </summary>
    /// <param name="abilityData">Ability 数据</param>
    /// <param name="caster">Buff的施加者</param>
    /// <param name="talent">产生Buff的技能</param>
    /// <returns></returns>
    //     public Ability AddAbility(AbilityData abilityData, Actor caster = null, Skill talent = null)
    //     {
    //         if (IsDeath)
    //             return null;
    //         Ability ability = Ability.Create(abilityData, this, caster, talent);
    //         return this.AddAbility(ability, caster, talent);
    //     }

    /// <summary>
    /// 添加Ability
    /// </summary>
    /// <param name="ability">Ability 实例</param>
    /// <param name="caster"></param>
    /// <returns></returns>
    public virtual Ability AddAbility(AbilityData abilityData, BattleActor caster = null, BattleSkill talent = null)
    {

#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
        {
            if (abilityData.ID == 8001)
                NeptuneBattle.log("AddAbility in {0} abilityData.CanAddOnDeath {1} role: {2}", abilityData.ID, abilityData.CanAddOnDeath.ToString(), this.ID);
        }
#endif
        if ((IsDeath && !abilityData.CanAddOnDeath) || SkipAddAbility(abilityData))
        {
            return null;
        }
#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
        {
            if (abilityData.ID == 8001)
                NeptuneBattle.log("AddAbility in {0} AddToAbilityList role:{1}", abilityData.ID, this.ID);
        }
#endif
        Ability ability = AddToAbilityList(abilityData, caster, talent);
        if (ability == null)
        {
            return null;
        }

        // 监测成功添加一个Ability后的回调
        if (callbackByAddAbility != null)
            callbackByAddAbility(ability);
        ability.CreateAbilityEffect();
        ability.SetTransformModel();
        ability.ModelScale();
        ability.ChangeTalent();
        AddTalentAbility(talent, ability);

#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("    {0}  {1} got ability:{2} ({3:f6}s)", this.FullName, this.ID, ability.AbilityData.Name, ability.Duration);
#endif


        if (ability.AbilityData.ReBuild)
        {
            this.InitAttributes();
        }
        if (ability.HasControlEffect)
        {
            this.InitEffects();
            this.OnAddAbilityControl(ability);
        }

        return ability;
    }

    private bool SkipAddAbility(AbilityData abilityData)
    {
        if (abilityData == null)
        {
            return true;
        }
        if ((abilityData.AddRoleType & this.Data.RoleType) <= 0 && abilityData.AddRoleType != RoleType.Any)
        {
            return true;
        }
        int heroType = ((int)abilityData.AddRoleType & ((int)RoleType.Melee + (int)RoleType.Remote));
        if (heroType > 0 && ((RoleType)heroType & this.Data.RoleType) <= 0)
        {
            return true;
        }
        if (this.IsUnaffected)
        {
            this.RemoveConlictAbilities();
            if (abilityData.IsControlAbility)
            {
                //ability.OnDestroyed();
                return true;
            }
        }
        else if (abilityData.IsUnControllable)
        {
            this.RemoveConlictAbilities();
        }
        if (abilityData.SkipAbilitys != null && abilityData.SkipAbilitys.Count > 0)
        {
            for (int i = 0; i < abilityData.SkipAbilitys.Count; i++)
            {
                if (FindAbilityByID(abilityData.SkipAbilitys[i]) != null)
                    return true;
            }
        }
#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            if (abilityData.ID == 8001)
                NeptuneBattle.log("  SkipAddAbility:{0} overlay: {1}", abilityData.ID, abilityData.Overlay.ToString());
#endif
        return abilityData.Overlay == OverlayType.Skip;
    }

    private Ability AddToAbilityList(AbilityData abilityData, BattleActor caster, BattleSkill talent)
    {
        if (abilityData == null)
        {
            return null;
        }
        float duration = abilityData.Time < 0 ? float.PositiveInfinity : UFloat.Round(abilityData.Time / 1000f);
        float intervalTime = abilityData.StartTime;
        float protection = 0;
        int shieldRecharge = 0;
        float shieldAdd = 0;
        if (abilityData.BaseRatio > 0)
        {
            shieldAdd = caster != null ? abilityData.BaseRatio * caster.Attributes[(int)abilityData.BaseAttribute] * EngineConst.Hundredth : abilityData.BaseRatio * this.Attributes[(int)abilityData.BaseAttribute] * EngineConst.Hundredth;
        }
        if (abilityData.Overlay == OverlayType.Always)
        {
            if (abilityData.MaxOverlayLayer > 0 && GetAbilityCountById(abilityData.ID) >= abilityData.MaxOverlayLayer)
            {
                return null;
            }
        }
        else if (abilityData.Overlay == OverlayType.Replace)
        {//覆盖，先删除旧Ability

            this.RemoveAbilityWithId(abilityData.ID);
        }
        else if (abilityData.Overlay == OverlayType.ResetTime)
        {
            Ability findAbility = FindAbilityByID(abilityData.ID);
            int count = GetAbilityCountById(abilityData.ID);
            if (abilityData.MaxOverlayLayer > 0 && count >= abilityData.MaxOverlayLayer)
            {
                if (findAbility != null)
                {
                    findAbility.SetDuration(abilityData.Time);
                    findAbility.ResetShield((abilityData.ShieldValue + shieldAdd) * abilityData.MaxOverlayLayer, abilityData.ShieldRecharge * abilityData.MaxOverlayLayer);
                }

                return null;
            }
            if (findAbility != null)
            {
                intervalTime = findAbility.intervalTime;
                protection = findAbility.Protection;
                shieldRecharge = findAbility.ShieldRecharge;

            }
            this.RemoveAbilityWithId(abilityData.ID);
            AddAbilityCountByID(abilityData.ID, count);

        }
        else if (abilityData.Overlay == OverlayType.CoverTime)
        {
            if (abilityData.MaxOverlayLayer > 0 && GetAbilityCountById(abilityData.ID) >= abilityData.MaxOverlayLayer)
            {
                return null;
            }
            //用已存在的buff剩余时间覆盖相同的buff时间
            Ability findAbility = FindAbilityByID(abilityData.ID);
            if (findAbility != null)
                duration = findAbility.Duration;

        }
        else if (abilityData.Overlay == OverlayType.OnlyResetTime)
        {
            Ability findAbility = FindAbilityByID(abilityData.ID);
            if (findAbility != null)
            {
                findAbility.SetDuration(abilityData.Time);
                if (abilityData.MaxOverlayLayer > 0 && GetAbilityCountById(abilityData.ID) >= abilityData.MaxOverlayLayer)
                    return null;
                AddAbilityCountByID(abilityData.ID);
                return null;
            }

        }
        AddAbilityCountByID(abilityData.ID);
        Ability ability = ObjectFactory.Create(abilityData, this, caster, talent);
        if (abilityData.Overlay == OverlayType.ResetTime)
        {
            ability.intervalTime = intervalTime;
            ability.ResetShield((abilityData.ShieldValue + shieldAdd) + protection, abilityData.ShieldRecharge + shieldRecharge);
            ability.AbilityAttrPlus(GetAbilityCountById(abilityData.ID));
        }
        else if (abilityData.Overlay == OverlayType.CoverTime)
        {
            ability.Duration = duration;
        }
        //this.Abilities.Add(ability);
        this.tAbilities.Add(ability);
        return ability;
    }

    /// <summary>
    /// 移除一组Ability
    /// </summary>
    /// <param name="abilities"></param>
    //public void RemoveAbilities(TArray abilities)
    //{
    //    foreach (Ability ability in abilities)
    //    {
    //        this.RemoveAbility(ability, false);
    //    }
    //    this.InitAttributes();
    //    this.InitEffects();
    //}

    /// <summary>
    /// 移除指定Ability
    /// </summary>
    /// <param name="ability">要移除的Ability</param>
    /// <param name="rebuild">是否重新构建</param>
    public virtual void RemoveAbility(Ability ability, bool rebuild = true)
    {
        if (!this.tAbilities.Contains(ability))
            return;
        //this.Abilities.Remove(ability);
        this.tAbilities.Remove(ability);
        RemoveAbilityCount(ability);
        ability.OnDestroyed();

        if (rebuild)
        {
            if (ability.AbilityData.ReBuild)
                this.InitAttributes();
            if (ability.HasControlEffect)
                this.InitEffects();
        }
    }

    public virtual void RemoveAbilityAt(int index, bool rebuild = true)
    {
        if (index < 0 || index >= this.tAbilities.Length)
        {
            return;
        }
        Ability ability = this.tAbilities[index];
        if (ability == null || !this.tAbilities.Contains(ability))
        {
            return;
        }
        this.tAbilities.Remove(ability);
        RemoveAbilityCount(ability);
        ability.OnDestroyed();

        if (rebuild)
        {
            if (ability.AbilityData.ReBuild)
                this.InitAttributes();
            if (ability.HasControlEffect)
                this.InitEffects();
        }
    }

    /// <summary>
    /// 移除指定ID的Ability
    /// </summary>
    /// <param name="id"></param>
    public void RemoveAbility(int id, int count = 0)
    {
        for (int i = 0; i < this.tAbilities.Length; i++)
        {
            Ability ability = this.tAbilities[i];
            if (ability != null && ability.AbilityData.ID == id)
            {

                if (count > 0 && (ability.AbilityData.Overlay == OverlayType.ResetTime || ability.AbilityData.Overlay == OverlayType.OnlyResetTime))
                {
                    int abilityCount = this.GetAbilityCountById(id);
                    if (count >= abilityCount)
                    {
                        this.RemoveAbilityAt(i);
                    }
                    else
                    {
                        this.RemoveAbilityCount(id, count);
                        ability.SetEffectCount(abilityCount - 2);

                        if (ability.AbilityData.ReBuild)
                            this.InitAttributes();
                    }
                    break;//continue;替换类型为ResetTime的buff相同id只会同时存在1个 所以此处直接break
                }

                this.RemoveAbilityAt(i);
                if (count > 0)
                {
                    count--;
                    if (count <= 0)
                    {
                        break;
                    }
                }

            }
        }
        //List<Ability> abilitys = this.Abilities.Where(ability => ability.AbilityData.ID == id).ToList();
        //if (abilitys != null && abilitys.Count > 0)
        //    this.RemoveAbilities(abilitys);
    }

    /// <summary>
    /// 返回一个指定ID的Ability
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Ability GetAbility(int id)
    {
        //for (int i = 0; i < this.Abilities.Count;i++ )
        //{
        //    if (this.Abilities[i].AbilityData.ID == id)
        //        return this.Abilities[i];
        //}

        for (int i = 0; i < this.tAbilities.Length; i++)
        {
            Ability ability = this.tAbilities[i];
            if (ability != null && ability.AbilityData.ID == id)
                return ability;
        }
        return null;
    }

    /// <summary>
    /// 追踪某技能产生的所有Buff
    /// </summary>
    /// <param name="talent">生成Buff的技能</param>
    /// <param name="ability">生成的Buff</param>
    private void AddTalentAbility(BattleSkill talent, Ability ability)
    {
        if (talent == null)
        {
            return;
        }

        if (talentAbilityDict.ContainsKey(talent))
        {
            List<Ability> ablist = talentAbilityDict[talent];
            if (!ablist.Contains(ability))
            {
                ablist.Add(ability);
            }
        }
        else
        {
            List<Ability> ablist = new List<Ability>();
            ablist.Add(ability);
            talentAbilityDict.Add(talent, ablist);
        }

    }

    /// <summary>
    /// 移除所有由指定技能生成的Buff
    /// </summary>
    /// <param name="talent">要清除影响的技能</param>
    public void RemoveTalentAbility(BattleSkill talent)
    {
        if (!talentAbilityDict.ContainsKey(talent))
        {
            return;
        }

        List<Ability> abList = talentAbilityDict[talent];
        if (abList.Count <= 0)
        {
            talentAbilityDict.Remove(talent);
            return;
        }

        foreach (Ability ability in abList)
        {
            if (this.tAbilities.Contains(ability))
            {
                RemoveAbility(ability);
            }
        }

        talentAbilityDict.Remove(talent);
    }
    /// <summary>
    /// 移除所有Ability
    /// </summary>
    public void ClearAbilities()
    {
        for (int i = 0; i < this.tAbilities.Length; i++)
        {
            Ability ability = this.tAbilities[i];
            if (ability != null)
            {
                ability.Delete();
                this.tAbilities.Remove(i);
            }
        }
    }

    /// <summary>
    /// 添加印记
    /// </summary>
    /// <param name="markID"></param>
    /// <param name="mark"></param>
    public void AddMark(int markID, BattleMark mark)
    {
        for (int i = 0; i < this._marksDic.Length; i++)
        {
            BattleMark m = this._marksDic[i];
            if (m != null && m.Data.ID == markID)
            {
                return;
            }
        }
        _marksDic.Add(mark);
    }

    /// <summary>
    /// 移除印记
    /// </summary>
    /// <param name="markID"></param>
    /// <returns></returns>
    public bool RemoveMarkByID(int markID)
    {
        for (int i = 0; i < this._marksDic.Length; i++)
        {
            BattleMark m = this._marksDic[i];
            if (m != null && m.Data.ID == markID)
            {
                m.OnDestroyed();
                _marksDic.Remove(i);
                return true;
            }
        }
        return false;

        /*if (_marksDic.ContainsKey(markID))
        {
            //移除成功
            _marksDic[markID].OnDestroyed();
            _marksDic.Remove(markID);
            return true;
        }
        else
        {
            //身上没有此种技能 移除失败
            return false;
        }*/
    }
    public bool RemoveMarkByID(int markID, int count)
    {
        for (int i = 0; i < this._marksDic.Length; i++)
        {
            BattleMark m = this._marksDic[i];
            if (m != null && m.Data.ID == markID)
            {
                if (count >= m._count)
                {
                    m.OnDestroyed();
                    _marksDic.Remove(i);
                }
                else
                {
                    m._count -= count;
                    m.SetEffectCount(m._count - 1);
                }

                return true;
            }
        }
        return false;
    }

    public void RemoveAllMark()
    {
        for (int i = 0; i < this._marksDic.Length; i++)
        {
            BattleMark m = this._marksDic[i];
            if (m != null && !m.Data.DontRemoveOnDeath)
            {
                m.OnDestroyed();
                this._marksDic.Remove(m);
            }
        }

        /*foreach(KeyValuePair<int, BattleMark> pair in _marksDic)
        {
            pair.Value.OnDestroyed();
        }*/
    }
    /// <summary>
    /// 获取印记
    /// </summary>
    /// <param name="markID"></param>
    /// <returns></returns>
    public BattleMark GetMarkByID(int markID)
    {
        BattleMark mark = null;

        for (int i = 0; i < this._marksDic.Length; i++)
        {
            BattleMark m = this._marksDic[i];
            if (m != null && m.Data.ID == markID)
            {
                mark = m;
                break;
            }
        }
        //_marksDic.TryGetValue(markID, out mark);
        return mark;
    }

    /// <summary>
    /// 清除死亡Ability
    /// </summary>
    public void ClearDeathAbility(BattleActor attacker)
    {
        if (this.tAbilities.Length == 0)
        {
            return;
        }

        for (int i = 0; i < this.tAbilities.Length; i++)
        {
            Ability ability = this.tAbilities[i];

            if (ability == null || ability.DontRemoveOnDeath)
                continue;
            if ((ability.AbilityData.TransferTriggerType & TransferTriggerType.OwnerDead) > 0)
            {
                ability.CheckTransferAbility(attacker);
            }

            this.tAbilities.Remove(ability);
            RemoveAbilityCount(ability);
            ability.OnDestroyed();
        }

        //foreach (Ability ability in this.Abilities.ToArray())
        //{
        //    if (!ability.IsRemoveOnDeath) 
        //        continue;
        //    if ((ability.AbilityData.TransferTriggerType & TransferTriggerType.OwnerDead) > 0)
        //    {
        //        ability.CheckTransferAbility(attacker);
        //    }
        //    ability.OnDestroyed();
        //    this.Abilities.Remove(ability);
        //    RemoveAbilityCount(ability);
        //}
        this.InitAttributes();
        this.InitEffects();
    }

    /// <summary>
    /// 设置动作
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="nonOnce"></param>
    /// <param name="stop"></param>
    public virtual void SetAnimation(string Id, bool loop = false, bool stop = false, bool immediately = false)
    {
        this.nonOnce = loop;
        if ((!loop || !Id.Equals(this.AniId)) && !this.AbilityEffects.Solidifying)
        {

            this.AniId = Id;


            if (!stop)
            {
                this.AniPlayedTime = UFloat.Round(this.AniPlayedTime - this.AniTotaltime);
                if (AniPlayedTime < 0)
                    this.AniPlayedTime = 0;
            }
            else
            {
                this.AniPlayedTime = 0;
            }



            if (Id.Length != 0)
            {
                if (false == loop)
                {
                    string modelName = this.RoleSkin.GetModelKey(ModelName);
                    ModelData model = NeptuneBattle.Instance.DataProvider.GetModelData(modelName);
                    if (model != null)
                    {
                        if (!string.IsNullOrEmpty(model.Resource))
                        {
                            Dictionary<string, Neptune.GameData.AnimationConfigData> ka = NeptuneBattle.Instance.DataProvider.GetAnimationConfigDatas(model.Resource);

                            if (ka != null && ka.ContainsKey(Id))
                            {

                                if (ka[Id].TotalTime > 0.00001)
                                {
                                    this.AniTotaltime = UFloat.Round(ka[Id].TotalTime);
                                }
                                else
                                {
                                    this.AniTotaltime = 0;
                                    Logger.LogError(string.Format("{0} -> {1}  TotalTime config is too small", Id, model.Resource));
                                }
                            }
                            else
                            {
                                this.AniTotaltime = 0;
                                Logger.LogError(string.Format("{0} -> {1}  animation config not found", Id, model.Resource));
                            }
                        }
                    }
                    else
                    {
                        Logger.LogError(string.Format("{0} -> {1} not found in model", Id, ModelName));
                    }

                }
                else
                {
                    this.AniTotaltime = 0;
                }
            }
            else
            {
                this.AniTotaltime = 0;
            }
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} Set Animation {1} {2:f6} {3:f6}", this.FullName, this.AniId, this.AniPlayedTime, this.AniTotaltime);
#endif
            if (this.Joint != null)
                this.Joint.OnNewAction(immediately);
        }
    }
    /// <summary>
    /// 设置HP
    /// </summary>
    /// <param name="hp"></param>
    public void SetHP(float hp)
    {
        if (hp != this.HP)
        {
            hp = UFloat.Round(hp);
            this.HP = Math.Min(Math.Max(hp, 0), this.MaxHP);
            this.AllAttributes[(int)AttributeType.Final].LostHPValue = UFloat.Round((this.MaxHP - this.HP));
            this.AllAttributes[(int)AttributeType.Final].LostHP = UFloat.Round(this.Attributes.LostHPValue / this.MaxHP);
        }
    }
    /// <summary>
    /// 设置MP
    /// </summary>
    /// <param name="mp"></param>
    public void SetMP(float mp)
    {
        if (mp != this.MP)
        {
            mp = UFloat.Round(mp);
            this.MP = Math.Min(Math.Max(mp, 0), this.MaxMP);
        }
    }

    /// <summary>
    /// 怒气
    /// </summary>
    /// <param name="rage"></param>
    public void SetRage(float rage)
    {
        if (rage != this.Rage)
        {
            rage = UFloat.Round(rage);
            this.Rage = Math.Min(Math.Max(rage, 0), this.MaxRage);
        }
    }

    /// <summary>
    /// 点值
    /// </summary>
    /// <param name="point"></param>
    public void SetPoint(float point)
    {
        if (point != this.Rage)
        {
            point = UFloat.Round(point);
            this.Point = Math.Min(Math.Max(point, 0), this.MaxPoint);
        }
    }

    public void SetCostValue(float value)
    {
        if (this.Data.MPType == MPType.Point)
        {
            SetPoint(value);
        }
        else if (this.Data.MPType == MPType.Rage)
        {
            SetRage(value);
        }
        else
        {
            SetMP(value);
        }

    }
    public float GetCurrentCostValue()
    {
        if (this.Data.MPType == MPType.Point)
        {
            return this.Point;
        }
        else if (this.Data.MPType == MPType.Rage)
        {
            return this.Rage;
        }
        else
        {
            return this.MP;
        }
    }
    public float GetMaxCostValue()
    {
        if (this.Data.MPType == MPType.Point)
        {
            return this.MaxPoint;
        }
        else if (this.Data.MPType == MPType.Rage)
        {
            return this.MaxRage;
        }
        else
        {
            return this.MaxMP;
        }
    }

    public virtual void SetVisible(bool visible, bool isTrasnparent)
    {
        this.isTrasnparent = isTrasnparent;
        this.Visible = visible;
        if (this.Joint != null)
        {
            this.Joint.SetVisible(visible, isTrasnparent);
        }

    }

    /// <summary>
    /// 主循环
    /// </summary>
    /// <param name="dt"></param>
    public override void OnEnterFrame(float dt)
    {
        //         if (this.RebirthTime > 0)
        //         {
        //             this.RebirthTime -= dt;
        //             if (this.RebirthTime <= 0)
        //             {
        //                 RoleRebirth();
        //             }
        //             return;
        //         }

        if (this.DeployTime > 0)
        {
            this.DeployTime -= dt * 1000f;
            if (this.DeployTime <= 0 && this.Joint != null && this.Joint.Controller != null)
            {
                ((IActorController)this.Joint.Controller).OnDeployed();
            }
            return;
        }

        if (this.AttachedRole != null)
        {
            this.AttachedRole.OnEnterFrame(dt);
        }

        //针对有存活时间的角色处理
        if (!float.IsNaN(LifeTime))
        {
            this.LifeTime -= dt;
            if (this.LifeTime <= 0)
            {
                this.End(null);
                return;
            }
        }

        //如果是召唤物
        //         if (this.SummonData != null)
        //         {
        //             if (!string.IsNullOrEmpty(this.AniId) && !this.nonOnce)
        //             {
        //                 float et = this.AniPlayedTime + dt;
        //                 if (this.AniTotaltime > 0 && et > this.AniTotaltime)
        //                 {
        //                     et = et - this.AniTotaltime;
        //                     this.OnAnimationEnd();
        //                     this.AniId = string.Empty;
        //                 }
        //                 this.AniPlayedTime = UFloat.Round(et);
        //             }
        //             if (this.Joint != null)
        //             {
        //                 this.Joint.SetDirection(this.IsReverse);
        //             }
        //             base.OnEnterFrame(dt);
        //             return;
        //         }

        //速度处理
        this.MoveSpeedRatio = this.BasicAttributes.MoveSpeed != 0 ? UFloat.Round(this.Attributes.MoveSpeed / this.BasicAttributes.MoveSpeed) : 0;

        this.MoveSpeedRatio = Math.Max(this.MoveSpeedRatio, 0.5f);
        this.MoveSpeedRatio = Math.Min(this.MoveSpeedRatio, 2.5f);

        //攻击速度处理

        //Lyon ：target.IsActiveCasting 用于HCU3D 放大招冻结相关逻辑 我们这边不需要
        if (this.CurrentTalent != null && (this.CurrentTalent.Data.NoSpeeder))
        {
            this.AttackSpeedRatio = 1;
        }
        else if (AbilityEffects.Solidifying)
        {
            this.MoveSpeedRatio = 0;
        }
        else
        {
            this.AttackSpeedRatio = UFloat.Round(1 + this.Attributes.AttackSpeed * EngineConst.Hundredth);
        }

        this.moveAnimDT = UFloat.Round(dt * MoveSpeedRatio);

        this.attackAnimDT = UFloat.Round(dt * AttackSpeedRatio);

        //动作以及技能处理
        if (!this.UpdateAction(dt))
        {
            if (this.CanStop())
                base.Stop();
            return;
        }
        this.BehaviorSelect(dt);
        this.UpdateShove(dt);
        UpdateAttackStateTimer();

        //foreach (Ability ability in this.Abilities.ToArray())
        //{//Ability更新
        //    ability.OnEnterFrame(dt);
        //    if (this.IsDead)
        //        return;
        //}

        for (int i = 0; i < this.tAbilities.Length; i++)
        {
            Ability ability = this.tAbilities[i];
            if (ability != null)
            {
                ability.OnEnterFrame(dt);
                if (this.IsDead)
                    return;
            }
        }
        //印记更新
        for (int i = 0; i < this._marksDic.Length; i++)
        {
            BattleMark mark = this._marksDic[i];
            if (mark != null)
            {
                mark.OnEnterFrame(dt);
                if (this.IsDead)
                    return;
            }
        }

        /*foreach (BattleMark mark in _marksDic.Values.ToArray())
        {
            mark.OnEnterFrame(dt);
            if (this.IsDead)
                return;
        }*/
        //处理恢复逻辑
        Recovery(dt);

        //处理主动技能
        //if (this.ActiveTalent != null)
        //{
        //    ResultType resType = this.ActiveTalent.CanUse(this.Target);
        //    this.CanUseActiveTalent = resType == ResultType.Success;
        //}
        //检测角色血量对技能的影响
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i];
            if (talent != null && talent.IsEnabled)
            {
                talent.OnHPChanged(this.HP, this.MaxHP);
            }
        }

        if (!this.IsRoleType(RoleType.Spell) && this.HP <= 0 && !(this.IsDead || this.IsDeadBody))
        {//如果没死又没血类，说明持续掉血死了
            this.End(null);
        }
    }

    protected bool CanStop()
    {
        bool canstop = false;
        if (this.RoleST == RoleState.Dead && this.Running)
        {
            if (this.Player == null && !this.Config.IsHomeBase && !this.Config.IsVisitorBase)
            {
                if (this.CastringEffects.Count == 0)
                    canstop = true;
            }
        }
#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("{0} CanStop : {1}", this.FullName, canstop);
#endif
        return canstop;
    }

    protected void FearMove()
    {
        /* 建筑物不考虑 */
        if (this.IsRoleType(RoleType.Building))
        {
            return;
        }

        UVector2 dest = UVector2.zero;
        if (this.AbilityEffects.Fear)
        {
            for (int cIndex = 0; cIndex < this.tAbilities.Length; cIndex++)
            {
                Ability ability = this.tAbilities[cIndex] as Ability;
                if (ability != null)
                {
                    if (ability.ControlEffects.ContainsKey((int)ControlEffect.Fear))
                    {
                        Vector2 fearDirection = (this.Position - ability.Caster.Position).normalized;
                        dest = this.Position + fearDirection * 100;
                        if (this.NavAgent != null)
                        {
                            AStarNode grid = NavLayer.NavMap.GetGridByLogicPos(dest);
                            if (!grid.Walkable || (grid.Area & NavAgent.AreaMask) != grid.Area)
                            {
                                /* 判断如果目标点不可走，则 沿着该方向找到 最远的可走的位置 */
                                for (int i = 0; i <= 100; i = i + 25)
                                {
                                    dest = this.Position + fearDirection * i;
                                    AStarNode tempGrid = NavLayer.NavMap.GetGridByLogicPos(dest);
                                    if (!tempGrid.Walkable || (tempGrid.Area & NavAgent.AreaMask) != tempGrid.Area)
                                    {
                                        dest = this.Position + fearDirection * (i - 25);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //foreach (Ability ability in this.Abilities)
            //{
            //    if (ability.ControlEffects.ContainsKey(ControlEffect.Fear))
            //    {
            //        Vector2 fearDirection = (this.Position - ability.Caster.Position).normalized;
            //        dest = this.Position + fearDirection * 100;
            //        AStarNode grid = NavLayer.NavMap.GetGridByLogicPos(dest);
            //        if (!grid.Walkable || (grid.Area & NavAgent.AreaMask) != grid.Area)
            //        {
            //            /* 判断如果目标点不可走，则 沿着该方向找到 最远的可走的位置 */
            //            for (int i = 0; i <= 100; i = i + 25)
            //            {
            //                dest = this.Position + fearDirection * i;
            //                AStarNode tempGrid = NavLayer.NavMap.GetGridByLogicPos(dest);
            //                if (!tempGrid.Walkable || (tempGrid.Area & NavAgent.AreaMask) != tempGrid.Area)
            //                {
            //                    dest = this.Position + fearDirection * (i - 25);
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //}
        }
        this.MoveToDest(dest);
    }

    public void ResetAttackAIMode(bool aiMode = false)
    {
        this.IsAIMode = aiMode;
        this.cacheSkillIndex = 0;
        this.attackAIMode = AttackAIMode.AutoSelectSkill;
#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("Reset Attack AIMode.");
#endif

    }
    /// <summary>
    /// 行为选择
    /// 移动处理、攻击行为选择等
    /// </summary>
    /// <param name="dt"></param>
    public virtual void BehaviorSelect(float dt)
    {
        if (EngineConst.BattlePauseLevelEnable && NeptuneBattle.Instance.PauseLevel != 0)
            return;
        if (AbilityEffects.Static)
            return;

        if (this.RoleST > RoleState.Move && cacheSkillIndex <= 0)
            return;

        if (this.AbilityEffects.Fear)
        {
            ResetAttackAIMode(true);
            FearMove();

            return;
        }
        if (this.Config.IsModel || !IsAIMode)
        {
            return;
        }
        if (this.AbilityEffects.Static
            || this.AbilityEffects.Sleep
            || this.AbilityEffects.Imprisonment
            || this.AbilityEffects.Solidifying)
        {//终止当前AI行为
            ResetAttackAIMode();
            return;
        }
        if (this.AbilityEffects.Taunt)
        {
            this.attackAIMode = AttackAIMode.AutoSelectSkill;
            this.cacheSkillIndex = 0;
        }
        float dist = 0f;

        if (attackAIMode == AttackAIMode.CacheSkill)
        {

            if (cacheSkillIndex > 0)
            {

                BattleSkill cacheTalent = GetActiveTalentBySlot(cacheSkillIndex);
                ResultType ret = ResultType.Failed;
                if (cacheTalent != null)
                {
                    ret = cacheTalent.CanUse(this.Target);
                }
                if (ret == ResultType.Success)
                {
                    // && !this.CurrentTalent.IsCastingProtectEx()
                    if (this.CurrentTalent != null && this.CurrentTalent.Casting && !cacheTalent.Data.Instant && (this.CurrentTalent.CanBreak(cacheTalent) || this.CurrentTalent.GroupData.Index == 1))
                    {
                        //this.ResetAttackAIMode();
#if BATTLE_LOG
                        if (EngineGlobal.BattleLog)
                            NeptuneBattle.log("{0} CastTalentByIndex break Current.", CurrentTalent.ToString());
#endif
                        CurrentTalent.Break(cacheTalent);
                    }
                    if (cacheTalent.Data.TargetType == TargetType.Position || cacheTalent.Data.TargetType == TargetType.Direction)
                    {
                        if (cacheTalent.GroupData.TalentGroupID == 50003)
                        {
                            TargetPos = Direction;
#if BATTLE_LOG
                            if (EngineGlobal.BattleLog)
                                NeptuneBattle.log("{0} CastTalentByIndex groupid 50003 to new pos {1}.", (cacheTalent == null) ? "null" : cacheTalent.ToString(), TargetPos);
#endif
                        }
                        else
                        {
                            if (Direction != EngineConst.Vector2Zero)
                            {
                                this.Target = null;
                                if (EngineGlobal.BattleLog)
                                    NeptuneBattle.log("{0} CastTalentByIndex set target null .", (cacheTalent == null) ? "null" : cacheTalent.ToString());
                                if (TargetPos != this.Position)
                                    this.Orientation = (TargetPos - this.Position).normalized;
                                if (cacheTalent.Data.TargetType == TargetType.Direction)
                                {
                                    TargetPos = this.Position + this.Orientation * cacheTalent.MaxRange;
#if BATTLE_LOG
                                    if (EngineGlobal.BattleLog)
                                        NeptuneBattle.log("{0} CastTalentByIndex addMaxRange to new pos {1}.", (cacheTalent == null) ? "null" : cacheTalent.ToString(), TargetPos);
#endif
                                }
                                else
                                {
                                    //TargetPos = this.Position + Direction * cacheTalent.MaxRange;
                                }

                            }
                            else
                            {
                                if (this.Target != null)
                                {
                                    TargetPos = this.Target.Position;
#if BATTLE_LOG
                                    if (EngineGlobal.BattleLog)
                                        NeptuneBattle.log("{0} CastTalentByIndex Target.Position to new pos {1}.", (cacheTalent == null) ? "null" : cacheTalent.ToString(), TargetPos);
#endif
                                    if (this.Target.Position != this.Position)
                                        this.Orientation = (this.Target.Position - this.Position).normalized;
                                }
                                else
                                {
                                    if (cacheTalent.Data.TargetType == TargetType.Position)
                                    {
                                        TargetPos = this.Position;
                                        if (EngineGlobal.BattleLog)
                                            NeptuneBattle.log("{0} CastTalentByIndex TargetType.Position to new pos {1}.", (cacheTalent == null) ? "null" : cacheTalent.ToString(), TargetPos);
                                    }
                                    else
                                    {
                                        TargetPos = this.Position + this.Orientation * cacheTalent.MaxRange;
#if BATTLE_LOG
                                        if (EngineGlobal.BattleLog)
                                            NeptuneBattle.log("{0} CastTalentByIndex MaxRange TargetType.Position to new pos {1}.", (cacheTalent == null) ? "null" : cacheTalent.ToString(), TargetPos);
#endif
                                    }

                                }
                            }
                        }
                    }

                    //                     if (!cacheTalent.Data.Instant && !cacheTalent.Data.MovingCast)
                    //                     {
                    //                         moveDir = false;
                    //                         //this.MoveDirection = Vector2.zero;
                    //                     }
                    if (this.TargetPos != UVector2.zero)
                    {
#if BATTLE_LOG
                        if (EngineGlobal.BattleLog)
                            NeptuneBattle.log("{0} CastTalentByIndex UseTalent in new pos {1}.", (cacheTalent == null) ? "null" : cacheTalent.ToString(), TargetPos);
#endif                  
                        this.UseTalent(cacheTalent, this.TargetPos);
                    }
                    else
                    {
                        //                         if (cacheTalent.Data.TargetType != TargetType.Self)
                        //                         {
                        //                             this.Target = target;
                        //                         }
#if BATTLE_LOG
                        if (EngineGlobal.BattleLog)
                            NeptuneBattle.log("{0} CastTalentByIndex UseTalent to target.{1}", (cacheTalent == null) ? "null" : cacheTalent.ToString(), (this.Target == null) ? "nil" : this.Target.ID.ToString());
#endif
                        this.UseTalent(cacheTalent, this.Target);
                    }

                    ResetAttackAIMode(this.IsAutoAttackMode);
                }
                else if (ret == ResultType.TooFar)
                {

                    if (this.RoleST > RoleState.Move)
                        return;
                    if ((this.Data.RoleType & RoleType.Building) == RoleType.Building)
                    {
                        this.Idle();
                        this.Target = null;
                    }
                    else
                    {
                        if (this.AbilityEffects.Inhuman)
                        {
                            this.Idle();
                        }
                        else
                        {
                            if (this.MoveDirection != Vector2.zero)
                            {
                                ResetAttackAIMode();
                            }
                            else if (this.TargetPos != UVector2.zero)
                            {
                                this.MoveToDest(this.TargetPos);
                            }
                            else if (this.Target != null)
                            {
                                this.MoveToDest(this.Target.Position);
                            }
                        }
                    }
                }
                else if (ret == ResultType.TooNear)
                {
                    if ((this.Data.RoleType & RoleType.Building) == RoleType.Building)
                    {
                        this.Idle();
                        this.Target = null;
                    }
                    else
                    {

                    }
                }
                else
                {

                    ResetAttackAIMode(this.IsAutoAttackMode);
                    if (this.RoleST > RoleState.Move)
                        return;
                    this.Idle();
                }
            }
            else
            {
                ResetAttackAIMode(this.IsAutoAttackMode);
                if (this.RoleST > RoleState.Move)
                    return;
                this.Idle();
            }
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} search target {1}, AIMode:{2}", this.FullName, Target != null ? Target.FullName : "nil", attackAIMode);
#endif
        }
        else
        {
            CastAutoSelectSkill();
        }
    }

    public virtual void CastAutoSelectSkill()
    {

        int dist = 0;
        BattleSkill talent = this.FindReadyTalent();
        this.Target = this.FindTarget(ref dist, talent);
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("{0} FindReadyTalent set target {1} .", (talent == null) ? "null" : talent.ToString(), (this.Target == null) ? "nil" : this.Target.ID.ToString());



#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("{0} search target {1}, AIMode:{2}", this.FullName, this.Target != null ? this.Target.FullName : "nil", attackAIMode);
#endif
        if (this.Target == null)
        {
            if (!this.AbilityEffects.Inhuman && this.Destination != UVector2.zero)//没有目标，向前进
            {
                this.CheckMove(this.Destination);
            }
            else
                this.Idle();
        }
        else
        {


#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} search talent {1}", this.FullName, talent != null ? talent.FullName() : "nil");
#endif
            if (talent != null)
            {
                if ((talent.Data.TriggerType & TriggerType.Manual) > 0 && NeptuneBattle.Instance.RuleManager.IsArenaMode)
                {
                    this.UseActiveTalent();//手动释放技能
                }
                //else if ((talent.Data.TriggerType & TriggerType.Manual) > 0 || (talent.Data.TriggerType & TriggerType.Auto) > 0)
                else if ((talent.Data.TriggerType & TriggerType.Auto) > 0 || (talent.Data.TriggerType & TriggerType.Manual) > 0 || (talent.Data.TriggerType & TriggerType.TriggerOnCD) > 0 || (talent.Data.TriggerType & TriggerType.HPUnder) > 0)
                {
                    this.UseTalent(talent, this.Target);//自动释放技能
                    if ((talent.Data.TriggerType & TriggerType.Manual) > 0 && this.Side == RoleSide.SideA)
                    {
                        this.IsActiveCasting = true;
                        NeptuneBattle logic = NeptuneBattle.Instance;
                        if (EngineConst.BattlePauseLevelEnable)
                        {
                            if (logic.PauseLevel == 0)
                            {
                                if (logic.Scene != null)
                                {
                                    logic.Scene.PlayEffect(ScreenEffect.Freeze, 0.7f, 0f);
                                }

                                logic.SuspendAllAnimation();
                            }
                            logic.PauseLevel++;
                            logic.PauseSide = this.Side;

                            this.Resume();
                            this.Highlight();
                        }
                    }
                }
            }
            else
            {
                //当没有选到技能时，不处于idle/move状态时直接返回
                if (this.RoleST > RoleState.Move)
                    return;
                if (this.AbilityEffects.Inhuman)
                    this.Idle();
                else
                {
                    this.CheckMoveToTarget(this.Target);
                }
            }
        }
    }

    protected bool UpdateAction(float dt)
    {
        bool animDone = false;
        CanMove = !this.AbilityEffects.Root && !this.IsRoleType(RoleType.Building) && !this.Config.IsModel;
        if (!string.IsNullOrEmpty(this.AniId) && !this.nonOnce)
        {//动作完成检查

            float totalPlayed = this.AniPlayedTime + attackAnimDT;
            if (/*this.AniTotaltime > 0 &&*/totalPlayed > this.AniTotaltime)
            {
                animDone = true;
                totalPlayed = totalPlayed - this.AniTotaltime;
                this.OnAnimationEnd();
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("    {0} UpdateAction OnAnimationEnd {1} {2:F6} / {3:F6} {4:f6} {5:f6}", this.FullName, this.AniId, totalPlayed, this.AniTotaltime, this.AniPlayedTime, attackAnimDT, dt);
#endif
            }
            this.AniPlayedTime = UFloat.Round(totalPlayed);
        }

        if (!this.IsDead && !this.IsRoleType(RoleType.Building))
            UpdateMovement(dt);
        //this.CheckTarget(LockedTarget);
        this.CheckTarget(Target);
        float finalDT = dt;
        if (this.Attributes.AttackSpeed != 0)
        {
            float attackSpeed = UFloat.Round(1 + this.Attributes.AttackSpeed * EngineConst.Hundredth);
            finalDT = UFloat.Round(dt * attackSpeed);
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("  {0} UpdateAction: AttackSpeed FinalDT:{1:f6} AttackSpeed:{2:f6} dt:{2:f6}", this.FullName, finalDT, attackSpeed, dt);
#endif
        }
        this.CommonCooldown = UFloat.Round(this.CommonCooldown - dt);

        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i];
            if (talent != null)
            {
                if (!talent.IsEnabled && (this.CurrentTalent != talent || !this.CurrentTalent.Casting))
                    continue;
                float delta = talent.Data.NoSpeeder ? dt : finalDT;
                talent.OnEnterFrame(attackAnimDT, delta);
            }
        }

        if (this.IsDead)
        {
            return false;
        }

        if (this.RoleST == RoleState.Attack && this.CurrentTalent != null)
        {
            if (animDone)
                this.CurrentTalent.OnAnimationEnd();
            if (this.AbilityEffects.Fear)
            {
                if (this.CurrentTalent != null && this.CurrentTalent.CanInterrupt(InterruptType.Passive))
                {
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("{0} UpdateAction break.", this.CurrentTalent.ToString());
#endif
                    this.CurrentTalent.Break();
                }
            }
        }

        if (this.IsRoleType(RoleType.Spell))
            return false;
        return true;
    }


    public void UpdateMovement(float dt)
    {

        currentMoveDir = this.MoveDirection;


        this.PreviousPosition = this.Position;

        if (this.AbilityEffects.Directed)
        {
            this.Orientation = this.InitDirection;
        }
        else
        {
#if GAME_2D
            Vector2 vx = Vector2.zero;
            if (CanMove && speedmv.magnitude > vx.magnitude)
                vx = speedmv;
            else if (this.CurrentTalent != null && this.CurrentTalent.Target != null)
                vx = this.CurrentTalent.Target.Position - this.Position;
            if (vx * this.Orientation < 0)
                this.Orientation = this.Orientation * -1;
#else
            if (this.CurrentTalent != null)
            {
                if (this.CurrentTalent.Data.RotateSpeed > 0 && this.CurrentTalent.Casting && currentMoveDir != Vector2.zero)
                {
                    this.Orientation = this.LerpVector(this.CurrentTalent.Data.RotateSpeed, dt, this.Orientation, currentMoveDir);
                }
                else if (this.CurrentTalent.Target != null && this.RoleST > RoleState.Move && this.CurrentTalent.Target.Position != this.Position)
                {
                    this.Orientation = (this.CurrentTalent.Target.Position - this.Position).normalized;
                }
                //Debug.Log("Direction[" + this.Name + "] : set by UpdateMovement" + this.Orientation);
            }
#endif
        }
        if (CanMove)
        {
            if (!charging && this.RoleST <= RoleState.Move)
            {
                this.movingTime += dt;

                if (this.Data.ChargeDelay > 0 && this.movingTime > this.Data.ChargeDelay)
                {
                    this.charging = true;
                    this.Move(this.Data.ChargeAction);
                }
            }
        }
        //修复击退BUG
        else if (this.RepelTime > 0 || this.AbilityEffects.Solidifying || !this.DashNoRoot)
        {
            if (this.DashTime > 0)
            {
                this.DashTime = 0;
                this.MoveSpeed = UVector2.zero;
                this.DashAcceleration = UVector2.zero;
                this.DashNoRoot = false;
            }

            this.currentMoveDir = UVector2.zero;
        }
        UVector2 movespeed = this.MoveSpeed;
        //移动速度计算
        if (this.DashTime > 0)
        {//如果冲刺中


            if (this.DashDelayPauseTime > 0)
            {
                this.DashDelayPauseTime = UFloat.Round(this.DashDelayPauseTime - dt);
            }
            else if (this.DashPauseTime > 0)
            {
                this.DashPauseTime = UFloat.Round(this.DashPauseTime - dt);
            }

            if (this.DashPauseTime <= 0 || this.DashDelayPauseTime > 0)
            {
                this.DashTime = UFloat.Round(this.DashTime - dt);
                this.DashTotalTime = UFloat.Round(this.DashTotalTime + dt);

                if (this.DashTime < 0)
                {
                    this.DashTotalTime = UFloat.Round(this.DashTotalTime + this.DashTime);
                }
                UVector2 curDistance = this.DashSpeed * this.DashTotalTime + this.DashAcceleration * this.DashTotalTime * this.DashTotalTime * 0.5f;

                if (this.DashTime <= 0)
                {
                    this.Position += curDistance - this.DashLastDistane;
                    this.DashAcceleration = UVector2.zero;
                    this.MoveSpeed = UVector2.zero;
                    this.DashTime = 0;
                    this.DashNoRoot = false;
                    movespeed = UVector2.zero;
                    if (this.OrcaAgent != null)
                    {
                        this.OrcaAgent.SetPosition(this.Position);
                        this.OrcaAgent.Sync();
                    }
                }
                else
                {
                    movespeed = (curDistance - this.DashLastDistane) / dt;
                    this.DashLastDistane = curDistance;
                }

                //                 if (this.DashTime <= 0)
                //                 {
                //                     if (this.DashTime < 0)
                //                     {
                //                       
                //                         float dTime = dt + this.DashTime;
                //                         this.Position += this.MoveSpeed * dTime;
                //                         if (this.OrcaAgent != null)
                //                             this.OrcaAgent.SetPosition(this.Position);
                //                     }
                //                     
                //                 }
            }


        }
        UVector2 speedkk = this.RepelSpeed;
        if (this.RepelTime > 0)
        {

            this.RepelTime = UFloat.Round(this.RepelTime - dt);
            this.RepelTotalTime = UFloat.Round(this.RepelTotalTime + dt);
            if (this.RepelTime < 0)
            {
                this.RepelTotalTime = UFloat.Round(this.RepelTotalTime + this.RepelTime);

            }
            UVector2 curDistance = this.RepelSpeed * this.RepelTotalTime + this.RepelAcceleration * this.RepelTotalTime * this.RepelTotalTime * 0.5f;
            if (this.RepelTime <= 0)
            {
                this.Position += curDistance - this.repelLastDistane;
                if (this.OrcaAgent != null)
                {
                    this.OrcaAgent.SetPosition(this.Position);
                    this.OrcaAgent.Sync();
                }

                speedkk = UVector2.zero;
            }
            else
            {
                speedkk = (curDistance - this.repelLastDistane) / dt;
                this.repelLastDistane = curDistance;
            }

            if (this.RepelTime <= 0)
            {
                this.RepelAcceleration = UVector2.zero;
                this.RepelSpeed = UVector2.zero;
                this.RepelTime = 0;
            }
        }


        UVector2 speedmv = (CanMove || DashNoRoot) && !(this.DashPauseTime > 0 && this.DashDelayPauseTime <= 0) ? movespeed : UVector2.zero;
        this.Speed = speedmv + speedkk;
        if (charging)
        {
            this.Speed = this.Speed + this.Speed.normalized * this.Data.ChargeSpeed;
        }

#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("{0} UpdateMovement:{1} MoveDir:({2:f6},{3:f6}) Multiplier:{4:f6} CanMove:{5} IsAutoAttackMode:{6}", this.FullName, this.Speed, this.currentMoveDir.x, this.currentMoveDir.y, this.MoveSpeedMultiplier, this.CanMove, this.IsAutoAttackMode);
#endif
        if (this.IsAutoAttackMode && this.currentMoveDir != EngineConst.Vector2Zero && (this.CurrentTalent == null || !this.CurrentTalent.Casting || !this.CurrentTalent.IsCastingProtect() || this.CurrentTalent.Data.MovingCast))
        {
            this.IsAutoAttackMode = false;
            ResetAttackAIMode();
        }
        Vector2 nextOrientation = this.Orientation;
        if (this.CanMove && this.DashTime <= 0 && this.RepelTime <= 0)
        {
            this.lerpRotate = true;
            if (currentMoveDir != EngineConst.Vector2Zero)
            {
                this.moveOrientation = currentMoveDir;
            }
            nextOrientation = this.LerpVector(EngineConst.RotateLerpSpeed, dt, this.Orientation, this.moveOrientation);
            UFloat.Round(ref nextOrientation);
            if (nextOrientation == EngineConst.Vector2Zero)
            {
                nextOrientation = this.Orientation;
            }

        }

        if (this.currentMoveDir != EngineConst.Vector2Zero && (!this.IsAIMode || (this.CurrentTalent != null && this.CurrentTalent.Casting && this.CurrentTalent.Data.MovingCast)))
        {
            //如果是方向移动 
            if (this.CanMove && this.DashTime <= 0 && this.RepelTime <= 0) //当前控制角色移动逻辑
            {
                if (this.RoleST != RoleState.Move && !(CurrentTalent != null && CurrentTalent.Casting && CurrentTalent.Data.MovingCast))
                {
                    this.RoleST = RoleState.Move;
                    this.Move();
                }

                this.Orientation = this.currentMoveDir != EngineConst.Vector2Zero && !(CurrentTalent != null && CurrentTalent.Casting && CurrentTalent.Data.MovingCast) ? nextOrientation : this.Orientation;
                Vector2 newSpeed = this.currentMoveDir * (this.Attributes.MoveSpeed * this.MoveSpeedMultiplier);
                UFloat.Round(ref newSpeed);
                this.MoveSpeed = new UVector2(UFloat.RoundToInt(newSpeed.x), UFloat.RoundToInt(newSpeed.y));
                this.Speed = this.MoveSpeed;
                this.MoveSpeed = UVector2.zero;

            }
            else if (this.DashTime <= 0)
            {
                this.Speed = UVector2.zero;
                this.OnMoveStop();
            }
        }
        else if (this.CurrentTalent == null || !this.CurrentTalent.Casting)
        {
            this.Orientation = nextOrientation;
        }
        this.lerpRotate = false;
        UVector2 pos = this.Position + this.Speed * dt;
        if (!BattleField.BattleAreaRect.Contains((Vector2)pos))
        {
            this.Speed = UVector2.zero;
            this.OnMoveStop();
        }

        if (isJumping)
        {
            jumpingTime += dt;
            if (jumpingTime > 2f)
                isJumping = false;
        }

        if (this.DashTime <= 0 && this.RepelTime <= 0)
        {
            this.OnMove();
        }

        if (this.Speed == UVector2.zero)
        {//没有速度直接返回
            return;
        }

        int area = 0;
        if (this.NavAgent == null || this.RoleST > RoleState.Move || this.IsAirForce || (this.currentMoveDir != EngineConst.Vector2Zero && !this.IsAIMode))
        {
            //AStarNode grid = NavLayer.NavMap.GetGridByLogicPos(pos);
            //if ((!grid.Walkable || grid.IsObstacle) && !this.IsAirForce)
            //{
            //    //不可行走区域
            //    pos = this.Position;
            //    this.Speed = Vector2.zero;
            //}
            //area = grid.Area;
            //if (!NavAgent.Walkable(pos))
            //{
            //    //不可行走区域
            //    pos = this.Position;
            //    this.Speed = Vector2.zero;
            //}
            //this.Position = pos;
        }
        else
        {
            if (this.CanMove && this.DashTime <= 0 && this.RepelTime <= 0)
                area = this.NavAgent.Move(dt);
            else
                area = this.NavAgent.GetArea(Position);
        }

        if (this.DashTime > 0 || this.RepelTime > 0)
        {
            this.Position = pos;
            if (this.OrcaAgent != null)
                this.OrcaAgent.SetPosition(this.Position);

        }

#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("{0} UpdateMovement Speed:{1} SpeedMV:{2} SpeedKK:{3} MoveDir:({4:f6},{5:f6})  MoveSpeed:{6:f6} Multiplier:{7:f6} PrevPos:{8} Pos:{9}", this.FullName, this.Speed, speedmv, speedkk, this.currentMoveDir.x, this.currentMoveDir.y, this.Attributes.MoveSpeed, this.MoveSpeedMultiplier, this.PreviousPosition, this.Position);
#endif
        this.OnMove(area);
    }
    protected virtual void OnMove(int area)
    {

    }

    protected void Jump()
    {
        if (isJumping == false)
        {
            if (this.AniId != "Jump")
                this.SetAnimation("Jump");
            isJumping = true;
            jumpingTime = 0;
        }
    }

    public Vector2 UpdateShoveResultant(float dt)
    {
        //colliders.Clear();
        UVector2 shovespeed = ShoveSpeed;
        float mystep = UFloat.Round(this.Speed.magnitude * dt);

        if (ShoveSpeed.magnitude > shoveThreshold)
        {
            shovespeed = ShoveSpeed * 0.5f;
        }
        else
        {
            shovespeed = UVector2.zero;

            foreach (BattleActor other in NeptuneBattle.Instance.GetSurvivors(RoleSide.All))
            {
                if (this != other && !this.IsRoleType(RoleType.Building) && !this.AbilityEffects.Static)
                {
                    if (this.HitTest(other, other.GlobalIdx > this.GlobalIdx ? UFloat.RoundToInt(mystep + other.Speed.magnitude * dt) : 0))
                    {
                        Vector2 dist = (this.Position - other.Position);
                        float offset = UFloat.Round(this.Radius + other.Radius - dist.magnitude);
                        if (offset > mystep)
                        {
                            //Debug.LogFormat("ShoveDist:{0} - {1} : {2}", this, other, dist.magnitude);
                            if (dist.normalized == EngineConst.Vector2Zero)
                                dist = Quaternion.AngleAxis(90, Vector3.forward) * this.Speed;
                            //colliders.Add(dist);
                            shovespeed += dist.normalized * offset * (other.Data.Weight / 100) * shoveForce;
                        }
                    }
                }
            }

            Vector3 final = shovespeed.normalized + this.Speed.normalized;
            if (shovespeed.magnitude > shoveThreshold)
            {
                if (final.magnitude < 1)
                {//追尾效正
                    if (final.magnitude == 0)
                        shovespeed = Quaternion.AngleAxis(90, Vector3.forward) * shovespeed;
                    else
                        shovespeed = (this.Speed.normalized + shovespeed.normalized).normalized * shovespeed.magnitude;
                }
                shovespeed = shovespeed.normalized * Math.Min(shovespeed.magnitude, this.Speed.magnitude);
            }
            else
                shovespeed = UVector2.zero;
        }
        this.ShoveSpeed = shovespeed;
        return shovespeed;
    }

    public void UpdateShove(float dt)
    {
        if (!RvoSimulator.Instance.RepelCollisionWithBuilding && (this.DashTime > 0 || this.RepelTime > 0))
        {
            return;
        }
        UVector2 shoveSpeed = UVector2.zero;
        if (EngineConst.EnableRVO)
        {
            shoveSpeed = UpdateShoveRVO(dt);
        }
        else
            shoveSpeed = UpdateShoveResultant(dt);

        //this.Speed += shoveSpeed;
        //this.Position += shoveSpeed * dt;
        //Owner.Speed = Owner.Speed.normalized * Mathf.Clamp(Owner.Speed.magnitude, baseSpeed.magnitude * 0.3f, baseSpeed.magnitude * 2f);
    }

    private Vector2 UpdateShoveRVO(float dt)
    {
        UVector2 shoveSpeed = UVector2.zero;
        UVector2 prefVelocity = Speed;
        UVector2 prefPosition = Position;
        if (NeptuneBattle.Instance.Simulator != null && OrcaAgent != null && !this.IsRoleType(RoleType.Building))// && !this.AbilityEffects.Static)
        {
            OrcaAgent.SetPreferredVelocity(prefVelocity);
            int moveSpeed = (isJumping && this.Data.JumpSpeed > 0)
                ? UFloat.RoundToInt(this.Data.JumpSpeed * this.MoveSpeedMultiplier)
                : this.Speed.magnitude;
            //float speed = moveSpeed * this.MoveSpeedMultiplier;

            /* 限制角色的最大速度的逻辑
                1、Dash和Repel的时候，不对速度做限制
                2、若非强制速率不变的时候，限制在一定的范围内 用于一个速度快的角色可以推动一个速度慢的角色往前走
            */
            int speed;
            if (this.DashTime > 0 || this.RepelTime > 0)
            {
                speed = moveSpeed;
            }
            else
            {
                speed = charging ? moveSpeed + this.Data.ChargeSpeed : moveSpeed;

                if (!((RvoSimulator)NeptuneBattle.Instance.Simulator).ForceSpeed)
                {
                    speed = UFloat.RoundToInt(Math.Min(speed * RvoSimulator.Instance.MinSpeedFactor,
                        RvoSimulator.Instance.MaxSpeedThreshold));
                }
            }

            OrcaAgent.SetPreferredMaxSpeed(speed);

            /* 下面的逻辑 需要移动 外面  */
            OrcaAgent.Step((float)dt);
            OrcaAgent.Update(dt);
            UVector2 oldPos = this.Position;

            this.Speed = OrcaAgent.GetVelocity();

            this.Position = OrcaAgent.GetPosition();

            this.OrcaLines = OrcaAgent.GetOrcaLines();

        }
        else if (OrcaAgent != null)
        {
            OrcaAgent.SetPosition(this.Position);
        }
#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("{0} UpdateShoveRVO Before[Pos:{1}, Speed:{2}] ----> After:[Pos:{3}, Speed:{4}] Static {5}", this.FullName, prefPosition, prefVelocity, this.Position, this.Speed, this.AbilityEffects.Static);
#endif

        return shoveSpeed;
    }

    /// <summary>
    /// 搜索基础目标
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    public virtual BattleActor FindTarget(ref int distance, BattleSkill talent = null, bool cancelLockTarget = true, float x = 0, float y = 0)
    {
        distance = int.MaxValue;
        BattleActor target = null;
        int sightRange = talent != null && talent.Data.SightRange > 0 ? talent.Data.SightRange : this.Data.SightRange;
        int minRange = talent != null ? talent.Data.MinRange : 0;
        //被嘲讽时的目标选择
        if (this.AbilityEffects.Taunt)
        {
            //foreach (Ability ability in this.Abilities)
            //{
            //    if (ability.ControlEffects.ContainsKey(ControlEffect.Taunt))
            //    {
            //        target = ability.Caster;
            //        if (!target.IsDead)
            //        {
            //            distance = this.Distance(target, EngineConst.EnableRadiusInDistance);
            //            return target;
            //        }
            //    }
            //}

            for (int cIndex = 0; cIndex < this.tAbilities.Length; cIndex++)
            {
                Ability ability = this.tAbilities[cIndex] as Ability;
                if (ability != null)
                {
                    if (ability.ControlEffects.ContainsKey((int)ControlEffect.Taunt))
                    {
                        target = ability.Caster;
                        if (!target.IsDead)
                        {
                            distance = this.Distance(target, EngineConst.EnableRadiusInDistance);
                            return target;
                        }
                    }
                }
            }
        }

        //处理目标锁定
        if (this.Data.AttackMode == AttackMode.Lock)
        {
            if (this.LockedTarget != null)
            {
                if (this.LockedTarget.IsDead)
                    this.LockedTarget = null;
                //else
                //    return this.LockedTarget;
            }
        }
        //判断已经锁定目标的攻击距离
        if (this.LockedTarget != null)
        {
            int magnitude = this.Distance(this.LockedTarget, EngineConst.EnableRadiusInDistance); // 获取与target的距离
            int maxRange = talent != null && talent.Data.SightRange > 0 ? talent.Data.SightRange : this.Data.SightRange;  //最大攻击距离
            if (magnitude > maxRange || magnitude < minRange)//最大距离
            {
                this.LockedTarget = null;
            }
        }
        if (this.LockedTarget != null)
        {
            //distance = this.Distance(this.LockedTarget, EngineConst.EnableRadiusInDistance);
            return this.LockedTarget;
        }

        //优先寻找攻击范围内目标
        //        foreach (Actor role in Logic.Instance.GetSurvivors(this,this.AntiSide,sightRange))
        foreach (BattleActor role in NeptuneBattle.Instance.AOIManager.GetSurvivors(this, this.AntiSide, sightRange))
        {
            if (role == this || this.NotSelectable(role)) continue;

            if (this.BasicTalent != null)
            {//以普通攻击的属性作为基础目标选择规则
                //对空对地、对建筑等规则的判断应在这里
                if (!this.BasicTalent.CheckTargetClass(role))
                    continue;
            }
            if (talent != null && talent.Data.TargetClass > 0 && (role.Data.RoleType & talent.Data.TargetClass) == 0)
                continue;
            if (!this.Data.AttackAir && role.IsAirForce)
                continue;

            int magnitude = this.Distance(role, EngineConst.EnableRadiusInDistance);
            //取最小距离
            if (sightRange > 0 && magnitude > sightRange) continue;//跳过视野范围外目标
            if (magnitude < minRange) continue;//跳过最小攻击范围内的目标
            if (distance < magnitude) continue;
            distance = magnitude;
            target = role;
        }
        //if (target == null && IsAIMode)
        //{//追加主要目标选择
        //    foreach (Actor role in Logic.Instance.GetSurvivors(this,this.AntiSide))
        //    {
        //        if (role == this || role.AbilityEffects.Void || role.AbilityEffects.Invisible || role.AbilityEffects.Grass) continue;

        //        if (talent != null && talent.Data.TargetClass > 0 && (role.Data.RoleType & talent.Data.TargetClass) == 0)
        //            continue;
        //        if (role.Data.RoleType != RoleType.Building) continue;

        //        if (!role.Config.IsVisitorBase && !role.Config.IsHomeBase)
        //            continue;

        //        float magnitude = this.Distance(role, true);
        //        //取最小距离
        //        if (!(MathUtil.FGreat(distance, magnitude))) continue;
        //        distance = magnitude;
        //        target = role;
        //    }
        //}

        return target;
    }
    /// <summary>
    /// 查找可使用的技能
    /// </summary>
    /// <returns></returns>
    public virtual BattleSkill FindReadyTalent(bool isAIMode = true)
    {
        if (this.CommonCooldown > 0)//未到技能释放时间
            return null;

        if (this.charging)
            return null;
        TriggerType type;
        //foreach (Skill talent in this.ActiveTalents)
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i];
            if (talent == null) continue;
            type = talent.Data.TriggerType;
            if (talent.IsEnabled && (
                (this.activeTalentReadyToGo && (type & TriggerType.Manual) > 0 && isAIMode) ||
                ((type & TriggerType.Auto) > 0 && isAIMode) ||
                 (type & TriggerType.TriggerOnCD) > 0 ||
                 (type & TriggerType.HPUnder) > 0
                ))
            {
                if ((type & TriggerType.HPUnder) > 0 && this.HP > UFloat.Round(this.MaxHP * (talent.Data.TriggerParam * EngineConst.Hundredth)))
                {
                    continue;
                }
                if (!talent.Data.Instant && this.RoleST > RoleState.Move && ((talent.Data.TriggerType & TriggerType.Manual) > 0 || (talent.Data.TriggerType & TriggerType.Auto) > 0))
                {
                    continue;
                }
                ResultType ret = talent.CanUse(this.Target);

#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("{0} find talent {1} to {2}--->{3}", this.FullName, talent.Data.TalentName != null ? talent.FullName() : "nil", ret == ResultType.Success ? "cast" : "no cast", ret);
#endif

                if (ret == ResultType.Success && talent.IsReady())//return true;
                    return talent;
            }
        }
        return null;
    }

    /// <summary>
    /// 动作完成事件
    /// </summary>
    public virtual void OnAnimationEnd()
    {
        if (this.RoleST == RoleState.Death)
        {
            this.RoleST = RoleState.Dead;
            this.SetAnimation(String.Empty);
            if (!this.Config.IsDemon)
                this.IsDeadBody = true;
        }
        else if (this.RoleST != RoleState.Attack)
        {
            this.Idle();
        }
    }

    public void OnTalentEnd(BattleSkill talent)
    {
        if (!talent.Data.Instant)
        {

            if (!talent.Data.MovingCast)
            {
                this.moveDirLock = false;
                //this.MoveDirection = Vector2.zero;
            }

        }

        if (this.IsRoleType(RoleType.Spell))
            this.End(null);
    }
    /// <summary>
    /// 进入空闲状态
    /// </summary>
    public virtual void Idle(bool forceIdle = false)
    {
        if (this.IsDead || this.RoleST == RoleState.Idle)
            return;

        this.movingTime = 0;
        if (this.charging)
        {
            this.charging = false;
            if (this.Data.ChargeTalent > 0 && this.Target != null)
            {
                GetTalentById(this.Data.ChargeTalent).Start(this.Target);
            }
        }
        ChangeState(RoleState.Idle);
        if (this.shove)
            this.Move(RoleStateName.Instance[RoleState.Move]);
        else
            this.SetAnimation(RoleStateName.Instance[RoleState.Idle], true, false, forceIdle);
    }

    public void Stun()
    {
        if (this.IsDead || this.RoleST == RoleState.Stun)
            return;
        ChangeState(RoleState.Stun);
        this.SetAnimation(RoleStateName.Instance[RoleState.Stun], true, false);
    }


    public void Move(string action = null)
    {
        string moveAction = string.IsNullOrEmpty(action) ? RoleStateName.Instance[this.RoleST] : action;
        this.SetAnimation(moveAction, true, false, true);
    }

    /// <summary>
    /// 行走到目标点
    /// </summary>
    /// <param name="dest"></param>
    public void MoveToDest(UVector2 dest)
    {
        if (this.IsDead)
            return;
        if (this.IsRoleType(RoleType.Building))
            return;

        //这里可以控制是不是需要移动
        if (this.RoleST != RoleState.Move)
        {
            this.RoleST = RoleState.Move;
            this.Move();
            this.Orientation = (dest - this.Position).normalized;
            this.movingTime = 0;
        }
        if (IsAirForce)
        {
            this.Orientation = (dest - this.Position).normalized;
        }
        this.MoveSpeed = UVector2.zero;
        if (this.Distance(dest, 0, false) > 0)
        {
            if (callbackAutoMoveByAttack != null)
                callbackAutoMoveByAttack();
            this.MoveSpeed = this.Orientation * (this.MoveSpeedMultiplier * (isJumping && this.Data.JumpSpeed > 0 ? this.Data.JumpSpeed : this.Attributes.MoveSpeed));
        }
        if (this.NavAgent != null)
        {
            this.NavAgent.SetDestination(dest);
        }

#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("{0} MoveToDest({1}) MoveSpeed:{2} Speed:{3} Pos:{4}", this.FullName, dest, this.MoveSpeed, this.Speed, this.Position);
#endif
    }



    /// <summary>
    /// 出生
    /// </summary>
    /// <param name="animation"></param>
    public void Born(string animation)
    {
        if (!string.IsNullOrEmpty(animation))
        {
            ChangeState(RoleState.Birth);
            this.SetAnimation(animation);
        }
        else
        {
            if (!string.IsNullOrEmpty(this.Data.BirthAction))
            {
                ChangeState(RoleState.Birth);
                this.SetAnimation(this.Data.BirthAction);
            }
        }
    }

    /// <summary>
    /// 使用技能
    /// </summary>
    /// <param name="talent">技能</param>
    /// <param name="target">目标</param>
    public virtual void UseTalent(BattleSkill talent, BattleActor target)
    {
        if (this.IsDead)
            return;

        if (!talent.Data.Instant)
        {
            if (this.CurrentTalent != null && this.CurrentTalent.Casting)
            {
                this.CurrentTalent.End();
            }
            //如果是瞬发技能就不改变角色状态
            ChangeState(RoleState.Attack);
            //             if (!talent.Data.MovingCast)
            //             {
            //                 moveDirLock = true;
            //             }
            cacheSkillIndex = 0;
            attackAIMode = AttackAIMode.AutoSelectSkill;
            this.CurrentTalent = talent;
        }

        if (this.Data.AttackMode == AttackMode.Lock)
        {
            if (this.LockedTarget == null && target != null && talent.Data.TargetType != TargetType.Self && talent.TargetSide == this.GetRelation(target))
            {//锁定攻击的第一个地方目标
                this.LockedTarget = target;
            }
        }
        talent.Start(target);
        this.TargetPos = UVector2.zero;
        this.Direction = UVector2.zero;
        if (callbackByUseTalent != null)
            callbackByUseTalent(talent.Data.Instant);
    }
    /// <summary>
    /// 使用技能
    /// </summary>
    /// <param name="talent">技能</param>
    /// <param name="target">目标</param>
    public virtual void UseTalent(BattleSkill talent, Vector3 targetPos)
    {
        if (this.IsDead)
            return;

        if (!talent.Data.Instant)
        {
            if (this.CurrentTalent != null && this.CurrentTalent.Casting)
            {
                this.CurrentTalent.End();
            }
            //如果是瞬发技能就不改变角色状态
            ChangeState(RoleState.Attack);
            //             if (!talent.Data.MovingCast)
            //             {
            //                 moveDirLock = true;
            //             }
            cacheSkillIndex = 0;
            attackAIMode = AttackAIMode.AutoSelectSkill;
            this.CurrentTalent = talent;
        }
        talent.Start(targetPos);
        this.TargetPos = UVector2.zero;
        this.Direction = UVector2.zero;
        if (callbackByUseTalent != null)
            callbackByUseTalent(talent.Data.Instant);
    }

    /// <summary>
    /// 使用手动技能（大招）
    /// </summary>
    public virtual void UseActiveTalent()//角色放大招
    {
        if (this.IsDead || this.ActiveTalent == null)
        {
            return;
        }
        if (this.ActiveTalent.CanActive())
        {
            this.ActiveTalent.Active();
            return;
        }
        if (this.ActiveTalent.CanUse(this.Target) != ResultType.Success)
        {
            return;
        }
        if (this.CurrentTalent != null)
        {
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} UseActiveTalent break.", this.CurrentTalent.ToString());
#endif
            this.CurrentTalent.Break();
        }
        this.UseTalent(this.ActiveTalent, this.Target);
        // 这个就是大招放大处理标记
        this.IsActiveCasting = true;
        NeptuneBattle logic = NeptuneBattle.Instance;
        if (EngineConst.BattlePauseLevelEnable)
        {
            if (logic.PauseLevel == 0)
            {
                if (logic.Scene != null)
                {
                    logic.Scene.PlayEffect(ScreenEffect.Freeze, 0.7f, 0f);
                }

                logic.SuspendAllAnimation();
            }
            logic.PauseLevel++;
            logic.PauseSide = this.Side;

            this.Resume();
            this.Highlight();
        }
    }
    public void CallBackTalentStart(BattleSkill talent)
    {
        if (talent != null && (talent.Data.TriggerType & TriggerType.Manual) > 0)
        {
            if (this.callbackByTalentStart != null)
            {
                this.callbackByTalentStart(talent);
            }
        }
    }

    /// <summary>
    /// 执行被动技能
    /// </summary>
    /// <param name="type"></param>
    /// <param name="target"></param>
    public void ExecutePassive(TriggerType type, BattleActor target, BattleSkill from)
    {
        this.PassiveEffect.Reset();
        this.PassiveEffect.ApportionRatio = 0;
        this.PassiveEffect.ApportionTotalRatio = 0;
        this.PassiveEffect.CRITPct = 0;
        bool condition = false;
        //计算己方被动
        for (int i = 0; i < this.PassiveTalents.Length; i++)
        {
            PassiveSkill talent = this.PassiveTalents[i];
            if (talent == null || talent.Caster != this)
                continue;
            if (!talent.CheckTarget(target))
                continue;
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("    {0} ExecutePassive Self TriggerType:{1}/{2} Skill:{3} TriggerParam:{4} TargetClass:{5} RoleType:{6}", target.FullName, type, talent.Data.TriggerType, talent.Data.TalentName, talent.Data.TriggerParam, talent.Data.TargetClass, target.Data.RoleType);
#endif

            if (talent.Data.TargetClass != 0 && (talent.Data.TargetClass & target.Data.RoleType) <= 0)
                continue;
            condition = false;
            if ((talent.Data.TriggerType & TriggerType.HPUnder) == TriggerType.HPUnder)
            {
                condition = UFloat.RoundToInt(target.HP) <= UFloat.RoundToInt(target.MaxHP * (talent.Data.TriggerParam * EngineConst.Hundredth));
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("    {0} ExecutePassive Self HPUnder condition:{1} HP:{2:f6} MaxHP:{3:f6} TriggerParam:{4:f6} TriggerParamFinal:{5:f6}", target.FullName, condition, target.HP, target.MaxHP, talent.Data.TriggerParam * EngineConst.Hundredth, target.MaxHP * (talent.Data.TriggerParam * EngineConst.Hundredth));
#endif
            }
            if ((talent.Data.TriggerType & type) == TriggerType.Damage)
            {
                if ((talent.Data.TriggerParam & 1) > 0)//技能
                {
                    condition |= from.Data.SkillType == SkillType.Skill;
                }
                if ((talent.Data.TriggerParam & 2) > 0)//普攻
                {
                    condition |= from.Data.SkillType == SkillType.Normal;
                }
                if ((talent.Data.TriggerParam & 4) > 0)//子技能
                {
                    condition |= from.Data.SkillType == SkillType.ChildSkill;
                }
            }
            if ((talent.Data.TriggerType & type) == TriggerType.IsAttacked)
            {
                condition |= (talent.Data.TriggerParam == 0 || talent.Data.TriggerParam == (int)from.Data.DamageType);
            }
            if (!condition)
            {
                continue;
            }
            this.PassiveEffect.CRITPct += talent.Data.CRITPct;
            this.PassiveEffect.BaseRatio += talent.Data.BaseRatio;
            this.PassiveEffect.ApportionRatio += talent.Data.ApportionRatio;
            this.PassiveEffect.ApportionTotalRatio += talent.Data.ApportionTotalRatio;
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("    {0} ExecutePassive Self TriggerType:{1} BaseRatio:{2:f6}", target.FullName, type, this.PassiveEffect.BaseRatio);
#endif
        }
    }


    /// <summary>
    /// 闪避
    /// </summary>
    /// <param name="talent1"></param>
    public virtual void OnDodge(BattleSkill talent)
    {

    }

    public void TalentBreak()
    {
        if (this.CurrentTalent != null)
        {
            this.CurrentTalent.Break();
            this.CurrentTalent = null;
        }
    }


    /// <summary>
    /// 致命伤害
    /// </summary>
    public void Fatal()
    {
        if (this.IsDead || this.CurrentTalent == null || !this.CurrentTalent.CanInterrupt(InterruptType.Passive) || !this.AbilityEffects.Static)
            return;

#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("{0} Fatal break.", this.CurrentTalent.ToString());
#endif
        this.CurrentTalent.Break();
        if (this.Joint != null && this.Joint.Controller != null)
            this.Joint.StopSound();

        //this.SetAnimation(RoleState.Damaged.ToString(), false, true);
        ChangeState(RoleState.Damaged);
    }

    public bool IsAssist(BattleActor attacker)
    {
        if (this.attackerMap.ContainsKey(attacker))
        {
            return this.attackerMap[attacker] + EngineConst.AssistsTime > NeptuneBattle.Instance.LogicTime;
        }
        return false;
    }

    protected virtual void OnAssistKill(BattleActor target, BattleActor attacker, int TotalAsset, BattleSkill fromTalent)
    {
        //助攻触发技能
        if (this != attacker)
            this.OnRoleDeath(target, attacker, fromTalent, this);

        if (this.Player != null)
        {
            this.Player.OnAssistKill(target, attacker, TotalAsset);
        }
    }


    protected void ProcessAssist(BattleActor attacker, BattleSkill fromTalent)
    {
        float gameTime = NeptuneBattle.Instance.LogicTime;
        this.assistList.Clear();
        foreach (KeyValuePair<BattleActor, float> pair in this.attackerMap)
        {
            if (pair.Key != this && gameTime - pair.Value < EngineConst.AssistsTime)
            {
                assistList.Add(pair.Key);
            }
        }

        for (int i = 0; i < this.assistList.Count; i++)
        {
            this.assistList[i].OnAssistKill(this, attacker, this.assistList.Count, fromTalent);
        }
    }
    /// <summary>
    /// 死亡
    /// </summary>
    /// <param name="attacker">击杀者</param>
    /// <param name="fromTalent"></param>
    public virtual void End(BattleActor attacker, BattleSkill fromTalent = null)
    {
        MoveDirection = UVector2.zero;
        if (this.IsDead)
            return;

#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("{0} died.", this.FullName);
#endif
        if (this.SummonData != null)
        {
            this.Player = null;
            if (this.Owner != null)
            {
                this.Owner.SummonList.Remove(this);

            }
        }

        if (attacker != null && attacker.Owner != null)
        {
            attacker = attacker.Owner;
        }

        foreach (BattleActor role in SummonList.Where(PredicateAlive))
        {
            role.LifeTime = 0.0f;
        }

        //         if (this.SummonData != null)
        //         {
        //             this.RoleST = RoleState.Death;
        //             this.SetAnimation(RoleStateName.Instance[RoleState.Death], false, true);
        //             Logic.Instance.Summons.Remove(this);
        //         }
        //         else
        {

            //this.Joint.Controller.RemoveAllEffect();
            if (this.IsActiveCasting)
            {
                this.IsActiveCasting = false;
                NeptuneBattle.Instance.Resume();
            }

            if (this.CurrentTalent != null)
            {
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("{0} Actor End break.", this.CurrentTalent.ToString());
#endif
                this.CurrentTalent.Break();
            }

            this.RoleST = RoleState.Death;

            this.CanUseActiveTalent = false;

            this.MoveSpeed = UVector2.zero;
            this.Speed = UVector2.zero;
            this.HP = 0;
            this.MP = 0;
            this.Rage = 0;
            this.Point = 0;
            //this.Joint.Controller.RemoveAllEffect();
            CheckTalentsTriggerOnDeath(attacker);
            this.LockedTarget = null;
            ResetAttackAIMode();
            bool rebirth = false;
            if (this.AbilityEffects.Rebirth)
            {
                rebirth = this.Rebirth();

            }
            this.ClearDeathAbility(attacker);
            this.RemoveAllMark();
            if (!string.IsNullOrEmpty(this.Data.DeathEffect))
            {
                this.PlayEffectOnDeath = true;
            }
            if (this.PlayEffectOnDeath)
            {
                this.AniId = string.Empty;
                this.nonOnce = false;
                this.AniTotaltime = 0;
                this.AniPlayedTime = 0;
                NeptuneBattle.PlayEffect(this.Data.DeathEffect, this.Position, UVector2.zero, this.Height, 0);
            }
            this.SetAnimation(RoleStateName.Instance[RoleState.Death], false, true);
            if (rebirth)
            {
                if (NeptuneBattle.Instance.RuleManager != null)
                    NeptuneBattle.Instance.RuleManager.OnRoleDeath(this, attacker);
                return;
            }
            this.ProcessAssist(attacker, fromTalent);
            if (this.Player != null)
            {
                this.Player.OnDeath(attacker);
            }
            //OnRoleDeath
            NeptuneBattle.Instance.OnRoleDeath(this, attacker, fromTalent);
            if (callbackByDead != null)
                callbackByDead();
        }

        isCloseAutoAttack = false;

        if (this.Joint != null)
        {
            //Logic.Instance.Scene.PopupText(PopupType.Gold, EngineConst.SymbolPlus + Mathf.Floor(this.Money), this.Joint, attacker.Joint, false, RoleSide.None);//现在要求被攻击角色冒金币
            this.Joint.OnDeath();
        }
        if (this.AttachedRole != null)
            this.AttachedRole.End(attacker, fromTalent);
        ClearOrcaSimulator();
        BattleMark(false);
        CanBuyEquipment = true;
        this.basicAttributesInited = false;
    }

    public virtual void Reset()
    {
        MoveDirection = UVector2.zero;
        this.Player = null;
        if (this.SummonData != null)
        {
            if (this.Owner != null)
            {
                this.Owner.SummonList.Remove(this);

            }
        }
        foreach (BattleActor role in SummonList.Where(PredicateAlive))
        {
            role.LifeTime = 0.0f;
        }


        {

            this.CurrentTalent = null;


            this.RoleST = RoleState.Death;

            this.CanUseActiveTalent = false;

            this.MoveSpeed = UVector2.zero;
            this.Speed = UVector2.zero;
            this.HP = 0;
            this.MP = 0;
            this.Rage = 0;
            this.Point = 0;
            this.LockedTarget = null;
            ResetAttackAIMode();

            this.RemoveAllMark();


        }

        isCloseAutoAttack = false;

        this.Joint = null;
        ClearOrcaSimulator();
        BattleMark(false);
        CanBuyEquipment = true;
        this.basicAttributesInited = false;
        CurrentTalent = null;
        ActiveTalent = null;
        Target = null;
        LockedTarget = null;
        SummonList.Clear();
        NavAgent = null;
        OrcaAgent = null;
        BasicTalent = null;
        Talents.Clear();
        ChildTalents.Clear();
        tActiveTalents.Clear();
        TriggerOnDamageTalents.Clear();
        TriggerOnRoleDeathTalents.Clear();
        TriggerOnMoveTalent.Clear();
        PassiveTalents.Clear();
        AuraTalents.Clear();
        EnemyAuraTalents.Clear();
        AllAttributes = null;
        tAbilities.Clear();
        AbilitiesDict.Clear();
        talentAbilityDict.Clear();
        _marksDic.Clear();
        CastringEffects.Clear();
        AttachedRole = null;
        Owner = null;
    }


    public void SetIsCloseAutoAttack(bool isCloseAutoAttack)
    {
        this.isCloseAutoAttack = isCloseAutoAttack;
    }

    protected virtual void ClearOrcaSimulator()
    {
        if (NeptuneBattle.Instance.Simulator != null && OrcaAgent != null)
        {
            NeptuneBattle.Instance.Simulator.AgentDead(OrcaAgent);
            OrcaAgent = null;
        }
    }

    protected void CheckTalentsTriggerOnDeath(BattleActor attacker)
    {
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i];
            if (talent == null) continue;
            if (!talent.IsEnabled)
                continue;
            if ((talent.Data.TriggerType & TriggerType.Death) > 0)
            {
                if (talent.CanUse(attacker) == ResultType.Success)
                {
                    if (talent.Data.TargetType == TargetType.Position || talent.Data.TargetType == TargetType.Direction)
                    {
                        talent.Start(this.Position);
                    }
                    else
                    {
                        talent.Start(attacker);
                    }
                }

                //死亡时创建技能
                //talent.OnHitFrame();
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("{0} cast talent1 {1} on Death", this.FullName, talent.Data.TalentName != null ? talent.Data.TalentName : "nil");
#endif
            }

            if (talent.HasAbility(AbilityTriggerType.Death))
            {
                //TODO:死亡时创建的Ability,Ability目标传递什么需要确定
                talent.CreateTalentAbilitis(null, AbilityTriggerType.Death);
            }
        }
    }

    /// <summary>
    /// 英雄复活时触发技能
    /// </summary>
    public virtual void OnReborn()
    {
        if (IsHero && Side == RoleSide.SideA && this.Joint != null)
        {
            this.Joint.AddEffect(this.Data.RebornEffect, EffectType.Normal, EngineConst.Vector3Zero);
        }
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i];
            if (talent == null) continue;
            if (!talent.IsEnabled) continue;
            if ((talent.Data.TriggerType & TriggerType.Reborn) > 0)
            {
                talent.InitCD();
                if (talent.CanUse(this) == ResultType.Success)
                {
                    talent.Start(this);
                }
            }
        }
    }

    /// <summary>
    /// 英雄升级时触发
    /// </summary>
    public virtual void OnLevelUp()
    {
        if (IsHero)
        {
            if (this.Joint != null)
                this.Joint.AddEffect(this.Data.LevelupEffect, EffectType.Normal, EngineConst.Vector3Zero);
            if (callbackByLevelUp != null)
                callbackByLevelUp();
        }
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i];
            if (talent == null) continue;
            if (!talent.IsEnabled) continue;
            if ((talent.Data.TriggerType & TriggerType.LevelUp) > 0)
            {
                if (talent.CanUse(this) == ResultType.Success)
                {
                    talent.Start(this);
                }
            }
        }
    }


    /// <summary>
    /// 计数破盾时触发
    /// </summary>
    public virtual void OnBrokeShield(int BuffID)
    {
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i];
            if (talent == null) continue;
            if (!talent.IsEnabled) continue;
            if ((talent.Data.TriggerType & TriggerType.BrokeShield) > 0 && (talent.Data.TriggerParam == 0 || talent.Data.TriggerParam == BuffID))
            {
                if (talent.CanUse(this) == ResultType.Success)
                {
                    talent.Start(this);
                }
            }
        }
    }


    /// <summary>
    /// 攻击时触发技能
    /// </summary>
    public virtual void OnAttack(BattleSkill sourceTalent)
    {
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i];
            if (talent == null) continue;
            if (!talent.IsEnabled) continue;
            if (sourceTalent != talent && (talent.Data.TriggerType & TriggerType.Attack) > 0
                  && (talent.Data.TriggerParam == 0 //任何攻击
                 || ((talent.Data.TriggerParam & 1) > 0 && sourceTalent.Data.SkillType == SkillType.Skill)  //技能
                 || ((talent.Data.TriggerParam & 2) > 0 && sourceTalent.Data.SkillType == SkillType.Normal)//普攻
                 || ((talent.Data.TriggerParam & 4) > 0 && sourceTalent.Data.SkillType == SkillType.ChildSkill)))//子技能
            {
                if (talent.CanUse(sourceTalent.Target) == ResultType.Success)
                {
                    talent.Start(sourceTalent.Target);
                }
            }
        }
    }

    /// <summary>
    /// 技能冷却时触发
    /// </summary>
    /// <param name="sourceTalent"></param>
    public virtual void OnColdDown(BattleSkill sourceTalent)
    {
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i];
            if (talent == null) continue;
            if (!talent.IsEnabled) continue;
            if (sourceTalent != talent && (talent.Data.TriggerType & TriggerType.ColdDown) > 0 && talent.Data.TriggerParam == sourceTalent.Data.ID)
            {
                if (talent.CanUse(sourceTalent.Target) == ResultType.Success)
                {
                    talent.Start(sourceTalent.Target);
                }
            }
        }
    }


    /// <summary>
    /// 暴击时触发技能
    /// </summary>
    /// <param name="sourceTalent"></param>
    public virtual void OnCrit(BattleSkill sourceTalent)
    {
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i];
            if (talent == null) continue;
            if (!talent.IsEnabled) continue;
            if (sourceTalent != talent && (talent.Data.TriggerType & TriggerType.Crit) > 0
                  && (talent.Data.TriggerParam == 0 //任何攻击
                 || ((talent.Data.TriggerParam & 1) > 0 && sourceTalent.Data.SkillType == SkillType.Skill)  //技能
                 || ((talent.Data.TriggerParam & 2) > 0 && sourceTalent.Data.SkillType == SkillType.Normal)//普攻
                 || ((talent.Data.TriggerParam & 4) > 0 && sourceTalent.Data.SkillType == SkillType.ChildSkill)))//子技能
            {
                if (talent.CanUse(sourceTalent.Target) == ResultType.Success)
                {
                    talent.Start(sourceTalent.Target);
                }
            }
        }
    }

    /// <summary>
    /// 血量低下时
    /// </summary>
    public void OnHPUnder()
    {
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i];
            if (talent == null) continue;
            if (!talent.IsEnabled) continue;
            if ((talent.Data.TriggerType & TriggerType.HPUnder) > 0 && this.HP <= this.MaxHP * (float)talent.Data.TriggerParam * EngineConst.Hundredth)
            {
                if (talent.CanUse(this) == ResultType.Success)
                {
                    talent.Start(this);
                }
            }
        }
    }

    /// <summary>
    /// 治疗时触发技能
    /// </summary>
    public void OnRemedy(BattleActor target)
    {
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i];
            if (talent == null) continue;
            if (!talent.IsEnabled) continue;
            if ((talent.Data.TriggerType & TriggerType.Remedy) > 0)
            {
                if (talent.Data.TargetType == TargetType.Self)
                {
                    talent.Target = this;
                }
                else if (talent.Data.TargetType == TargetType.Target)
                {
                    talent.Target = target;
                }
                else
                {
                    talent.Target = null;
                }

                if (talent.CanUse(talent.Target) == ResultType.Success)
                {
                    talent.Start(talent.Target);
                }

            }
        }
    }

    public void OnAddAbilityControl(Ability ability)
    {
        if (ability == null || !ability.HasControlEffect) return;
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i] as BattleSkill;
            if (talent != null)
            {
                if (!talent.IsEnabled) continue;
                if ((talent.Data.TriggerType & TriggerType.Ability) > 0 && talent.Data.TriggerParam1 == (int)TriggerAbilityType.Control && (talent.Data.TriggerParam == 0 || ability.ControlEffects.ContainsKey(talent.Data.TriggerParam)))
                {

                    if (talent.CanUse(ability.Caster, true, true) == ResultType.Success)
                    {
                        talent.Start(ability.Caster);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 移动时
    /// </summary>
    public virtual void OnMove()
    {
        if (this.TriggerOnMoveTalent.Count <= 0)
            return;
        int currentspeed = this.Speed.magnitude;
        if (this.speedmagnitude == currentspeed)
            return;
        for (int i = 0; i < this.TriggerOnMoveTalent.Count; i++)
        {
            BattleSkill talent = this.TriggerOnMoveTalent[i] as BattleSkill;
            bool condition = false;
            if (talent != null)
            {
                if (!talent.IsEnabled) continue;
                if (talent.Data.TriggerParam1 != 0)
                {
                    if (talent.Data.TriggerParam > currentspeed)
                        condition = true;
                }
                else
                {
                    if (talent.Data.TriggerParam < currentspeed)
                        condition = true;
                }
                if (condition)
                {
                    if (talent.CanUse(this) == ResultType.Success)
                    {
                        talent.Start(this);
                    }
                }
            }
        }
        this.speedmagnitude = currentspeed;
    }

    public virtual void OnMoveStop()
    {

    }


    /// <summary>
    /// 检测攻击者
    /// </summary>
    /// <param name="attacker"></param>
    public void CheckAttack(BattleActor attacker)
    {
        if (attacker == null || attacker == this)
            return;
        if (attacker.IsRoleType(RoleType.Hero))
        {
            this.finalKillTime = NeptuneBattle.Instance.LogicTime;
            this.finalKiller = attacker;
            float gameTime = NeptuneBattle.Instance.LogicTime;
            if (this.IsRoleType(RoleType.Hero))
            {
                if (this.attackerMap.ContainsKey(attacker))
                {
                    this.attackerMap[attacker] = gameTime;
                }
                else
                {
                    this.attackerMap.Add(attacker, gameTime);
                }
            }
        }

    }

    /// <summary>
    /// 受到攻击事件处理
    /// </summary>
    /// <param name="attacker"></param>
    public void OnIsAttacked(BattleActor attacker, BattleSkill from, float damage)
    {
        CheckAttack(attacker);


        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i];
            if (talent == null) continue;
            if (!talent.IsEnabled) continue;
            if ((talent.Data.TriggerType & TriggerType.IsAttacked) > 0 && (talent.Data.TriggerParam == 0 || (from != null && talent.Data.TriggerParam == (int)from.Data.DamageType)))
            {
                if (talent.Data.TriggerParam1 > 0 && from != null && (from.Data.AffectMP || talent.Data.TriggerParam1 * EngineConst.Hundredth * this.HP > damage))
                {
                    continue;
                }

                if (talent.CanUse(attacker) == ResultType.Success)
                {
                    talent.Start(attacker);
                }
            }
        }
        if (CurrentTalent != null && CurrentTalent.CanInterrupt(InterruptType.OnIsAttacked))
        {
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} OnIsAttacked break.", this.CurrentTalent.ToString());
#endif
            CurrentTalent.Break();
        }
        //// 释放回城技能时，会被伤害打断 (策划调整为用 增加 0.1s 的眩晕buff来控制)
        //if (CurrentTalent != null && CurrentTalent.Data.ID == 50003)
        //{
        //    CurrentTalent.Break();
        //}
    }
    /// <summary>
    /// 死亡事件处理
    /// </summary>
    /// <param name="role"></param>
    /// <param name="attacker"></param>
    public virtual void OnRoleDeath(BattleActor role, BattleActor attacker, BattleSkill fromTalent = null, BattleActor assistor = null)
    {
        for (int i = 0; i < this.TriggerOnRoleDeathTalents.Count; i++)
        {
            BattleSkill talent = this.TriggerOnRoleDeathTalents[i];
            if (talent == null) continue;
            if (!talent.IsEnabled) continue;
            //敌方单位死亡时创建技能
            string why = ((talent.Data.TriggerType & TriggerType.UnitDeath) > 0 && role.Side != this.Side) ? "UnitDeath" : string.Empty;
            //if (!role.IsHero && ((talent.Data.TriggerType & TriggerType.UnitDeath) == TriggerType.UnitDeath))
            //{
            //    return;
            //}
            //友方单位死亡时创建技能
            why = ((talent.Data.TriggerType & TriggerType.FriendDeath) > 0 && role.Side == this.Side) ? "FriendDeath" : why;
            //击杀目标时
            why = ((talent.Data.TriggerType & TriggerType.KillTarget) > 0 && attacker == this && (talent.Data.TriggerParam == 0 || (talent.Data.TriggerParam & (int)role.Data.RoleType) > 0) && !role.Config.IsDemon) ? "KillTarget" : why;
            //助攻
            why = ((talent.Data.TriggerType & TriggerType.Assist) > 0 && assistor == this && (talent.Data.TriggerParam == 0 || (talent.Data.TriggerParam & (int)role.Data.RoleType) > 0) && !role.Config.IsDemon) ? "AssistTarget" : why;

            //why = (fromTalent != null && (talent.Data.TriggerType & (int)TriggerType.KillTarget) > 0 && attacker == this && !role.Config.IsDemon && talent.Data.TriggerParam == fromTalent.Data.ID) ? "KillTarget" : why;

            if (why.Length > 0
                //&& (talent.Data.TriggerParam == 0 || ((talent.Data.TriggerParam & (int)role.Data.RoleType) > 0)) 
                && (UVector2.Distance(role.Position, Position) < talent.Data.MaxRange || talent.Data.MaxRange <= 0))
            {
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("{0} cast talent1 {1} on {2}", this.FullName, talent.Data.TalentName ?? "null", why);
#endif
                if ((talent.Data.TriggerType & TriggerType.TriggerOnCD) > 0)
                    talent.Duration = 0;
                else if (talent.Data.TriggerParam1 == 0 || (fromTalent != null && fromTalent.Data.ID == talent.Data.TriggerParam1))
                {
                    if (talent.CanUse(attacker) == ResultType.Success)
                    {
                        if (talent.Data.TargetType == TargetType.Position)
                        {
                            talent.Start(role.Position);
                        }
                        else
                        {
                            talent.Start(attacker);
                        }
                    }
                }
            }

        }
    }
    /// <summary>
    /// 造成伤害时触发
    /// </summary>
    /// <param name="target"></param>
    /// <param name="injuryType"></param>
    public virtual void OnDamage(BattleActor target, BattleSkill sourceTalent)
    {
        BattleActor currentTarget = null;
        for (int i = 0; i < this.TriggerOnDamageTalents.Count; i++)
        {
            BattleSkill talent = this.TriggerOnDamageTalents[i];
            if (talent == null) continue;

            if (!talent.IsEnabled) continue;
            currentTarget = target;
            if ((talent.Data.TargetClass == 0 || (talent.Data.TargetClass & target.Data.RoleType) > 0)
                 && ((talent.Data.TriggerParam & 7) == 0 //任何攻击(1 + 2 + 4)
                 || ((talent.Data.TriggerParam & 1) > 0 && sourceTalent.Data.SkillType == SkillType.Skill)  //技能
                 || ((talent.Data.TriggerParam & 2) > 0 && sourceTalent.Data.SkillType == SkillType.Normal)//普攻
                 || ((talent.Data.TriggerParam & 4) > 0 && sourceTalent.Data.SkillType == SkillType.ChildSkill)))//子技能
            {

                if ((talent.Data.TriggerParam & 120) == 0//(8 + 16 + 32 + 64)
                 || ((talent.Data.TriggerParam & 8) > 0 && sourceTalent.Data.DamageType == InjuryType.AttackDamage)
                 || ((talent.Data.TriggerParam & 16) > 0 && sourceTalent.Data.DamageType == InjuryType.AbilityPower)
                 || ((talent.Data.TriggerParam & 32) > 0 && sourceTalent.Data.DamageType == InjuryType.Heal)
                 || ((talent.Data.TriggerParam & 64) > 0 && sourceTalent.Data.DamageType == InjuryType.Holy)
                    )
                {
                    if (talent.Data.TriggerParam1 != 0 && talent.Data.TriggerParam1 != sourceTalent.Data.ID)
                        continue;
                    if (talent.Data.TargetType == TargetType.Self)
                    {
                        currentTarget = this;
                    }

                    if (talent.CanUse(currentTarget) == ResultType.Success)
                    {

                        talent.Start(currentTarget);
                    }
                }


            }
        }
        this.EnterAttackState();
    }
    public virtual void CastTalent(int talentID, BattleActor target)
    {
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i];
            if (talent == null) continue;
            if (!talent.IsEnabled) continue;
            if (talent.Data.ID == talentID)
            {
                if (talent.CanUse(target) == ResultType.Success)
                {
                    talent.Start(target);
                }
                break;
            }
        }
    }

    /// <summary>
    /// 执行治疗
    /// </summary>
    /// <param name="force"></param>
    /// <param name="type"></param>
    /// <param name="from"></param>
    public virtual void Remedy(float force, RoleAttribute type = RoleAttribute.MaxHP, BattleActor from = null)
    {
        if (this.IsDead)
            return;

        if (from != null)
        {
            force = force * from.Attributes.PowerFactor;
            //被治疗加成
            //if (this.Attributes.SHealHp > 0)
            if (type == RoleAttribute.MaxHP)
            {
                force = force * Math.Max(EngineConst.MinSHealHp, (1 + this.Attributes.SHealHp * EngineConst.Hundredth));
            }

        }


        bool isFine = true;
        this.ResumeActor();
        if (type == RoleAttribute.MaxPoint)
        {
            force = Math.Min(force, this.Attributes.MaxPoint - this.Point);
            if (force < 1)
            {
                return;
            }
            this.SetPoint(this.Point + force);
        }
        else if (type == RoleAttribute.MaxRage)
        {
            force = Math.Min(force, this.Attributes.MaxRage - this.Rage);
            if (force < 1)
            {
                return;
            }
            this.SetRage(this.Rage + force);
        }
        else if (type == RoleAttribute.MaxMP)
        {
            force = Math.Min(force, this.Attributes.MaxMP - this.MP);
            if (force < 1)
            {
                return;
            }
            this.SetMP(this.MP + force);
        }
        else if (!this.AbilityEffects.Incurable)
        {
            force = Math.Min(force, this.Attributes.MaxHP - this.HP);
            if (force < 1)
            {
                return;
            }
            this.SetHP(this.HP + force);
        }
        else
            isFine = false;
#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("    {0} gains {1:f6} {2} > {3:f6}", this.FullName, force, type, type == RoleAttribute.MaxHP ? this.HP : type == RoleAttribute.MaxRage ? this.Rage : this.MP);
#endif

        if (NeptuneBattle.Instance.Scene != null && isFine && this.joint != null && force > 1)
        {
            PopupType popType = PopupType.Heal;
            if (type == RoleAttribute.MaxMP)
                popType = PopupType.StealMp;
            NeptuneBattle.Instance.Scene.PopupText(popType, EngineConst.SymbolPlus, (int)Math.Floor((double)(force + 1f / 2f)), this.Joint, from != null ? from.Joint : null, false, RoleSide.None, type);
        }
    }

    /// <summary>
    /// 恢复逻辑
    /// </summary>
    /// <param name="dt"></param>
    protected void Recovery(float dt)
    {


        if (this.MP < this.Attributes.MaxMP)
        {
#if BATTLE_LOG
            //生命回复逻辑
            float oldmp = this.MP;
#endif
            this.SetMP(this.MP + UFloat.Round((this.Attributes.MPRegen + UFloat.RoundToInt(this.Attributes.MaxMP * this.Attributes.MPRegen_a * EngineConst.Hundredth)) * dt * NeptuneBattle.Instance.MPBonus));
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("    {0} MPRegen {1:F6} = {2:F6} + {3:F6} + {4:F6} * {5:F6} * {6:F6} * {7:F6}", this.FullName, this.MP, oldmp, this.Attributes.MPRegen, this.Attributes.MaxMP, this.Attributes.MPRegen_a, dt, NeptuneBattle.Instance.MPBonus);
#endif
        }

        if (this.Rage < this.Attributes.MaxRage)
        {
            this.SetRage(this.Rage + UFloat.Round((this.Attributes.RageRegen + UFloat.RoundToInt(this.Attributes.MaxRage * this.Attributes.RageRegen_a * EngineConst.Hundredth)) * dt * NeptuneBattle.Instance.MPBonus));
        }
        if (this.Point < this.Attributes.MaxPoint)
        {
            this.SetPoint(this.Point + UFloat.Round((this.Attributes.PointRegen + UFloat.RoundToInt(this.Attributes.MaxPoint * this.Attributes.PointRegen_a * EngineConst.Hundredth)) * dt * NeptuneBattle.Instance.MPBonus));
        }

        if (!this.AbilityEffects.Norecover && this.HP < this.Attributes.MaxHP && (this.Attributes.HPRegen > 0 || this.Attributes.HPRegen_a > 0))
        {
            if (!this.AbilityEffects.Incurable)
            {//持续回复生命
                float oldhp = this.HP;
                float AddHP = UFloat.Round((this.Attributes.HPRegen + UFloat.RoundToInt(this.Attributes.MaxHP * this.Attributes.HPRegen_a * EngineConst.Hundredth)) * dt);
                this.SetHP(this.HP + AddHP);
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("    {0} HPRegen {1:F6} = {2:F6} + {3:F6} ( {4:F6} + {5:F6} * {6:F6} * {7:F6})", this.FullName, this.HP, oldhp, AddHP, this.Attributes.HPRegen, this.Attributes.MaxHP, this.Attributes.HPRegen_a, dt);
#endif
            }
        }
        float hPDecrease = this.Attributes.HPDecrease;
        float hPDecreasea = this.Attributes.HPDecrease_a;
        if (hPDecrease > 0 || hPDecreasea > 0)
        {//持续减少生命
            float oldhp = this.HP;
            this.SetHP(this.HP - UFloat.Round((hPDecrease + UFloat.RoundToInt(hPDecreasea * this.Attributes.MaxHP * EngineConst.Hundredth)) * dt));
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("    {0} HPDecrease {1:F6} = {2:F6} - ({3:F6} + {4:F6} * {5:F6}) * {6:F6}", this.FullName, this.HP, oldhp, hPDecrease, this.Attributes.MaxHP, hPDecreasea, dt);
#endif
        }

        //2016-04-07 注释掉这里，解决持续掉血不会死亡的问题
        //if (this.HP == 0 && !this.IsDead)
        //    this.HP = 1;

        this.LowHP = this.CurrentHPRatio < 0.2f;
    }

    /// <summary>
    /// 直接伤害
    /// </summary>
    /// <param name="damange"></param>
    /// <param name="attrType"></param>
    /// <param name="from"></param>
    public virtual void DirectInjury(int damange, RoleAttribute attrType, BattleActor from)
    {
        float oldhp = this.HP;
        this.SetHP(this.HP - damange);

#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("    {0} DirectInjury From {1} HP:{2:F6} = {3:F6} - {4:F6}", this.FullName, from == null ? "NULL" : from.FullName, this.HP, oldhp, damange);
#endif

        RecordDamage(damange, from);
        if (this.HP == 0)
            this.End(from);

        if (from != this)
            NeptuneBattle.Instance.Statistic.OnDamage(from, damange);

        OnDamaged(InjuryType.AttackDamage, damange, attrType, false, from);
    }

    /// <summary>
    /// 记录受到的伤害，和攻击者造成的伤害
    /// </summary>
    public void RecordDamage(int damage, BattleActor from)
    {
        if (from.Player != null)
        {
            from.Player.RecordDamage(damage, this.Player, this.Data.RoleType);
        }

        if (this.Player != null)
        {
            Player.RecordInjured(damage, from);
        }
    }

    /// <summary>
    /// 计算伤害
    /// </summary>
    /// <param name="force"></param>
    /// <param name="type"></param>
    /// <param name="attrType"></param>
    /// <param name="from"></param>
    /// <param name="fromTalent"></param>
    /// <returns></returns>
    public virtual int CalculateInjury(InjuryType type, RoleAttribute attrType, float force, BattleActor from, BattleSkill fromTalent, ref int critCount)
    {
#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("    {0} DMGPre HP:{1} MP:{2} Rage :{3}", this.FullName, this.HP, this.MP, this.Rage);
#endif
        if (this.IsDead || force <= 0)
            return 0;

        // 反伤处理，反伤不计算护甲和伤害减免，因此需要在计算最终伤害之前计算；反伤只反弹物理技能带来的伤害；反伤伤害固定为法术伤害
        if (this.Attributes.ReboundInjuryRatio > 0 && force > 0 && from != null && !from.IsDead && !from.IsRoleType(RoleType.Building)
            && fromTalent != null && fromTalent.Data.DamageType == InjuryType.AttackDamage)
        {
            int ReboundInjury = UFloat.RoundToInt(UFloat.Round(force * UFloat.Round((this.Attributes.ReboundInjuryRatio * EngineConst.Hundredth))));
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("    {0} ReboundInjury ReboundInjury:{1:f6} lost:{2:f6} ReboundInjuryRatio:{3:f6}", this.FullName, ReboundInjury, force, this.Attributes.ReboundInjuryRatio);
#endif

            from.CalculateInjury(InjuryType.AbilityPower, attrType, ReboundInjury, this, null, ref critCount);
        }

        InjuryResult result = NeptuneBattle.Instance.Numeric.CalcFinalInjury(this, type, attrType, force, from, fromTalent);
        critCount += result.IsCritical ? 1 : 0;
        if (result.IsImmunization)
        {
            if (NeptuneBattle.Instance.Scene != null)
            {
                bool PI = this.Attributes.PhysicsImmunization * EngineConst.Hundredth >= 1;
                bool MI = this.Attributes.MagicImmunization * EngineConst.Hundredth >= 1;
                PopupType poptype = PopupType.None;
                if (PI && MI)
                    poptype = PopupType.Void;
                else if (PI)
                    poptype = PopupType.PhyVoid;
                else
                    poptype = PopupType.MagVoid;
                NeptuneBattle.Instance.Scene.PopupText(poptype, string.Empty, 0, this.Joint, from.Joint, false, this.Side);
            }
            return result.FinalInjury;
        }
        int lost = result.FinalInjury;

        //         if (from != this)
        //             Logic.Instance.Statistic.OnDamage(from, lost);
        attrType = attrType == RoleAttribute.None ? RoleAttribute.MaxHP : attrType;

#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("    {0} lost {1} {2} {3}", this.FullName, lost, attrType, result.IsCritical ? "(Crit!)" : "");
#endif
        //float lastLostHpPct = (this.Attributes.MaxHP - this.HP) / this.Attributes.MaxHP;
        if (attrType == RoleAttribute.MaxHP)
        {
            this.SetHP(this.HP - lost);
            RecordDamage(lost, from);
            NeptuneBattle.Instance.Statistic.RecordDamage(this, from, fromTalent, lost);
            if (from != this)
                NeptuneBattle.Instance.Statistic.OnDamage(from, lost);

            if (this.HP == 0)
                this.End(from, fromTalent);
            else
            {
                {
                    if (result.IsFatal &&
                    (this.CurrentTalent == null || this.CurrentTalent.CanInterrupt(InterruptType.Passive)) &&
                    !this.AbilityEffects.Unaffected)
                    {
                        this.Fatal();
#if BATTLE_LOG
                        if (EngineGlobal.BattleLog)
                            NeptuneBattle.log("    {0} was interrupted.", this.FullName);
#endif
                    }
                }
                this.SetCostValue(this.GetCurrentCostValue() + UFloat.Round(lost * this.Data.MPGainRate / this.Attributes.MaxHP * NeptuneBattle.Instance.MPBonus));
            }
        }
        else if (attrType == RoleAttribute.MaxMP)
            this.SetMP(this.MP - lost);
        else if (attrType == RoleAttribute.MaxRage)
            this.SetRage(this.Rage - lost);
        else if (attrType == RoleAttribute.MaxPoint)
            this.SetPoint(this.Point - lost);
#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("    {0} DMGPost HP:{1:f6} MP:{2:f6}", this.FullName, this.HP, this.MP);
#endif
        this.ResumeActor();
#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("    {0} ResumeActor done", this.FullName);
#endif
        OnDamaged(type, lost, attrType, result.IsCritical, from);
#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("    {0} OnDamaged done", this.FullName);
#endif

        if (callbackByAttack != null)
            callbackByAttack(from, lost);

        return lost;
    }

    /// <summary>
    /// 分摊伤害
    /// </summary>
    /// <param name="type"></param>
    /// <param name="attrType"></param>
    /// <param name="force"></param>
    /// <returns></returns>
    public virtual float ApportionInjury(InjuryType type, RoleAttribute attrType, float force)
    {
        float apportionTotalRatio = 0;
        int critCount = 0;
        for (int i = 0; i < this.PassiveTalents.Length; i++)
        {
            PassiveSkill talent = this.PassiveTalents[i];
            if (talent == null)
                continue;
            if (talent.Data.ApportionTotalRatio > 0)
            {
                foreach (BattleActor role in NeptuneBattle.Instance.AOIManager.GetSurvivors(this, talent.Data.AffectedSide, talent.Data.MaxRange))
                {
                    if ((talent.Data.TargetClass & role.Data.RoleType) <= 0 && talent.Data.TargetClass != RoleType.Any)
                        continue;
                    if (role == this)
                        continue;
                    float apportionRatio = apportionTotalRatio + talent.Data.ApportionRatio <= PassiveEffect.ApportionTotalRatio ? talent.Data.ApportionRatio : PassiveEffect.ApportionTotalRatio - apportionTotalRatio;
                    apportionTotalRatio += apportionRatio;
                    //伤害计算公式
                    role.CalculateInjury(talent.Data.DamageType, attrType, UFloat.Round(force * apportionRatio), this, null, ref critCount);
                    if (role.Joint != null)
                    {
                        role.Joint.AddEffect(talent.Data.HitEffect, EffectType.Hit, new Vector3(0, 0, talent.Data.HitOrder));
                    }
                    if (apportionTotalRatio >= PassiveEffect.ApportionTotalRatio)
                    {
                        break;
                    }
                }
            }

        }

        return UFloat.Round(force * Mathf.Max(0, (1 - apportionTotalRatio)));
    }

    /// <summary>
    /// 显示伤害
    /// </summary>
    /// <param name="damange"></param>
    /// <param name="attrType"></param>
    /// <param name="isCrit"></param>
    public virtual void OnDamaged(InjuryType injuryType, float damange, RoleAttribute attrType, bool isCrit, BattleActor from)
    {
        if (NeptuneBattle.Instance.Scene != null && this.joint != null)
        {
            int dmg = (int)Math.Ceiling(damange);
            if (dmg != 0)
            {
                if (from != null && from.Player != null && from.Player.isMainPlayer)
                {
                    if (this.RoleST == RoleState.Idle)
                    {
                        this.Joint.SetDirection((from.Position - this.Position).normalized);
                    }
                    this.Joint.OnDamaged();
                }
                PopupType popupType = PopupType.AttackDamage;
                switch (injuryType)
                {
                    case InjuryType.AbilityPower: popupType = PopupType.AbilityDamage; break;
                    case InjuryType.Holy: popupType = PopupType.HolyDamage; break;
                }

                //Logic.Instance.Scene.PopupText(popupType, EngineConst.SymbolMinus + dmg, this.Joint, from.Joint, isCrit, this.Side, attrType);
                NeptuneBattle.Instance.Scene.PopupText(popupType, string.Empty, dmg, this.Joint, from.Joint, isCrit, this.Side, attrType);

                //  这个地方注掉（估计团队副本需要处理掉落）
                //if (this.MonIdx > 0)
                //{
                //    float lostHpPct = (this.Attributes.HP - this.MaxHP) / this.Attributes.MaxHP;
                //    if (Logic.Instance.Scene != null)
                //        Logic.Instance.Scene.ShowDropLoots(Logic.Instance.Round, this, lostHpPct, lastLostHpPct);
                //}
            }
        }
    }

    /// <summary>
    /// 击飞
    /// </summary>
    /// <param name="repelHeight"></param>
    /// <param name="repelGravityFactor"></param>
    /// <param name="backTime"></param>
    /// <param name="distance"></param>
    /// <param name="acceleration"></param>
    /// <param name="checkTrapObstacle"></param>
    /// <param name="repelResistIgnorance"></param>
    /// <param name="time"></param>
    public void Repel(float repelHeight, float repelGravityFactor, float backTime, UVector2 distance, UVector2 acceleration, bool repelIgnoreObstacle, bool checkTrapObstacle = false, bool repelResistIgnorance = false)
    {
        if (this.AbilityEffects.Immoblilize || this.IsRoleType(RoleType.Building))
        {
            return;
        }

        float repelResist = UFloat.Round(EngineConst.Hundred - (repelResistIgnorance ? 0 : this.Attributes[(int)RoleAttribute.Toughness])) / EngineConst.Hundred;

        if (repelHeight > 0)
        {
            repelHeight *= UFloat.Round(1 - (this.Attributes[(int)RoleAttribute.Toughness] * this.Attributes[(int)RoleAttribute.Toughness]) * EngineConst.Hundredth * EngineConst.Hundredth);
        }
#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("  {0} Repel Up{1:f6}, Back{2:f6}, ({3}, {4})", this.FullName, repelHeight, backTime, distance.x, distance.y);
#endif

        this.RepelTime = backTime * repelResist;
        this.RepelAcceleration = acceleration;
        UVector2 perPos = distance + this.Position;
        if (this.RepelTime > 0.0001f && distance != UVector2.zero && distance.magnitude > EngineConst.MinDistance)
        {
            this.RepelTotalTime = 0;
            this.repelLastDistane = UVector2.zero;
            this.RepelSpeed = distance / this.RepelTime - this.RepelAcceleration * this.RepelTime * 0.5f;
            //末速度
            UVector2 endSpeed = this.RepelSpeed + this.RepelAcceleration * this.RepelTime;
            UVector2 obstacleWallHit = UVector2.zero;
            if (checkTrapObstacle)
            {
                obstacleWallHit = NeptuneBattle.Instance.HitTrap(this, this.Position, this.Position + distance);
            }

            if (obstacleWallHit != UVector2.zero)
            {
                distance = obstacleWallHit - this.RepelSpeed.normalized * 50;
                if ((this.Position - distance).magnitude <= EngineConst.MinDistance || MathUtil.Dot((distance - this.Position).normalized, this.RepelSpeed.normalized) < 0)
                {
                    this.RepelTime = 0;
                }
            }
            else
            {
                if (this.NavAgent != null)
                {
                    distance = this.NavAgent.GetDashObstacleHit(this.Position, this.Position + distance, repelIgnoreObstacle);
                }
            }
            if (this.NavAgent != null)
            {
                if (this.RepelTime > 0.0001f)
                {
                    this.RepelTime = UFloat.Round((float)(this.Position - distance).magnitude /
                                                  (endSpeed.magnitude + this.RepelSpeed.magnitude) * 2);
                }
                if (this.RepelTime <= 0.0001f)
                {
                    this.RepelSpeed = UVector2.zero;
                    this.RepelAcceleration = UVector2.zero;
                }
                else
                {
                    this.RepelSpeed = (distance - this.Position) / this.RepelTime -
                                      this.RepelAcceleration * this.RepelTime * 0.5f;
                }
            }
            SetRepelDirection(this.RepelSpeed);

        }
        else
        {
            this.RepelSpeed = UVector2.zero;
            this.RepelAcceleration = UVector2.zero;
        }

        // Repel Up don't impact conflict logic, so only need update on expression level
        if (repelHeight > 0 && this.Joint != null)
            this.Joint.Jump(repelHeight, repelGravityFactor);

    }
    protected virtual void SetRepelDirection(Vector2 direct)
    {

    }
    protected bool Rebirth()
    {
        Ability rebirth = null;
        //rebirth = this.Abilities.Find((v) =>
        //{
        //    if (v.ControlEffects.ContainsKey(ControlEffect.Rebirth))
        //        return true;
        //    return false;
        //});

        for (int cIndex = 0; cIndex < this.tAbilities.Length; cIndex++)
        {
            Ability ability = this.tAbilities[cIndex] as Ability;
            if (ability != null)
            {
                if (ability.ControlEffects.ContainsKey((int)ControlEffect.Rebirth))
                {
                    rebirth = ability;
                    break;
                }

            }
        }

        if (rebirth != null)
        {//复活时根据技能等级设置角色等级，并转换阵营
         //             if (rebirth.Skill.Data.Param1 > 0 && rebirth.Skill.RebirthNum < rebirth.Skill.Data.Param1)
         //             {
         //                 this.Hero.level = rebirth.Skill.Level;
         // 
         //                 Logic.Instance.AliveRoles[this.Side].Remove(this);
         //                 this.Side = rebirth.Caster.Side;
         //                 Logic.Instance.AliveRoles[this.Side].Add(this);
         //                 this.Init();
         //                 rebirth.Skill.RebirthNum++;
         //                 if (this.Joint.Controller != null)
         //                 {
         //                     this.Joint.Controller.Init();
         //                 }
         //                 return true;
         //             }
            this.RebirthTime = rebirth.Talent.Data.Param1;
            this.RebrithHp = rebirth.Talent.Data.Param2;
            this.RebrithMp = rebirth.Talent.Data.Param3;
            this.RebrithHpRatio = UFloat.Round(rebirth.Talent.Data.Param4 * EngineConst.Hundredth);
            this.RebrithMpRatio = UFloat.Round(rebirth.Talent.Data.Param5 * EngineConst.Hundredth);

            NeptuneBattle.Instance.AOIManager.RemoveRole(this);
            NeptuneBattle.Instance.AliveRoles[(int)this.Side].Remove(this);
            NeptuneBattle.Instance.AliveRoles[(int)RoleSide.All].Remove(this);
            if (this.AuraTalents.Count > 0)
            {
                NeptuneBattle.Instance.AliveRolesInitAttributes(this.Side, RelativeSide.Friend);
            }

            if (this.EnemyAuraTalents.Count > 0)
            {
                NeptuneBattle.Instance.AliveRolesInitAttributes(this.Side, RelativeSide.Enemy);
            }
            return true;
        }
        return false;
    }

    public void RoleRebirth()
    {
        NeptuneBattle.Instance.AliveRoles[(int)this.Side].Add(this);
        NeptuneBattle.Instance.AliveRoles[(int)RoleSide.All].Add(this);
        this.RoleST = RoleState.Idle;
        this.IsExtraInited = false;
        this.RebirthTime = 0;
        /* this.removed = false;*/
        this.Init(true);
        this.SetHP(this.RebrithHp + this.MaxHP * this.RebrithHpRatio);
        this.SetMP(this.RebrithMp + this.MaxMP * this.RebrithMpRatio);
        //this.OnReborn();
        CallBackByRebirth();
    }

    public void CallBackByRebirth()
    {
        if (callbackByRebirth != null)
            callbackByRebirth();
    }
    private string GetModelName(string model)
    {
        if (this.Data.Model == model)
            return model;
        if (this.Data.Model.Contains(model))
        {
            model = this.Data.Model;
        }
        return model;
    }
    /// <summary>
    /// 获取变形信息
    /// </summary>
    public void SetTransformInfo()
    {
        if (this.Data.PeriodGroup > 0)
        {
            this.TransformInfos.Clear();
            Dictionary<int, TransformData> transforms = NeptuneBattle.Instance.DataProvider.GetTransformDatas(this.TransformGroupId);
            if (transforms != null)
            {
                foreach (KeyValuePair<int, TransformData> kv in transforms)
                {
                    this.TransformInfos.Add(kv.Value);
                }
            }

            ResetTransformation();
        }
    }

    public void ResetTransformation()
    {
        this.transformIDToModel.Clear();
        TransformStack.Clear();
        this.ModelStack.Clear();

        this.CurrentTransform = 1;
        TransformData period = NeptuneBattle.Instance.DataProvider.GetTransformData(this.TransformGroupId, 1);
        if (period != null)
        {
            ModelName = this.GetModelName(period.Model);
            this.ModelStack.SetFirstValue(ModelName);
            //            this.ModelStack[1] = this.GetModelName(period.Model);
            //            ModelName = this.ModelStack[1];
        }
        this.transformIDToModel.SetFirstValue(1);
        this.TransformStack.SetFirstValue(this.CurrentTransform);
        //        TransformStack.Add(1, this.CurrentTransform);
        this.ResetTalents(period);
    }


    public bool IsEnterTransformation = false; //是否开始形态变化
#pragma warning disable CS1570 // XML comment has badly formed XML -- 'End tag was not expected at this location.'
    /// 开始形态变换
    /// </summary>
    /// <param name="transId"></param>
    /// <param name="modelid"></param>
    public int EnterTransformation(int transformID)
#pragma warning restore CS1570 // XML comment has badly formed XML -- 'End tag was not expected at this location.'
    {
        //int trasnformIndex = this.GetLastTransformIndex() + 1;

        //DoTransform
        int modelid = 0;
        if (this.TransformGroupId > 0)
        {
            TransformData period = NeptuneBattle.Instance.DataProvider.GetTransformData(this.TransformGroupId, transformID);
            if (period != null)
            {
                modelid = this.SetModel(period.Model);

                if (this.CurrentTransform != transformID)
                {
                    this.ResetTalents(period);
                }

                if (!string.IsNullOrEmpty(period.EnterAction))
                {
                    this.SetAnimation(period.EnterAction);
                    //this.RoleST = RoleState.Transform;
                }
                needTransformModel = period.TransformModel;
            }
        }
        this.CurrentTransform = transformID;
        this.transformIDToModel.Push(modelid);
        int transformIndex = TransformStack.Push(this.CurrentTransform);
        return transformIndex;
    }

    /// <summary>
    /// 开始形态变换
    /// </summary>
    /// <param name="transfromIndex"></param>
    /// <param name="modelid"></param>
    public void RemoveTransformation(int transfromIndex)
    {
        if (this.TransformStack.ContainsKey(transfromIndex) && this.TransformStack.Count > 1)
        {
            //Remove Transform
            this.TransformStack.Remove(transfromIndex);
            int currentTransformID = TransformStack.GetLastValue();
            if (this.TransformGroupId > 0)
            {
                TransformData period = NeptuneBattle.Instance.DataProvider.GetTransformData(this.TransformGroupId, currentTransformID);

                if (period != null)
                {
                    if (this.CurrentTransform != currentTransformID)
                    {
                        this.ResetTalents(period);
                    }

                    //Remove Model
                    int modelid;
                    this.transformIDToModel.TryGetValue(transfromIndex, out modelid);
                    if (modelid != 0)
                    {
                        needTransformModel = period.TransformModel;
                        this.RemoveModel(modelid);
                    }
                }
            }
            this.CurrentTransform = currentTransformID;
            if (this.transformIDToModel.ContainsKey(transfromIndex))
            {
                this.transformIDToModel.Remove(transfromIndex);

            }
        }

    }

    public int SetModel(string name)
    {
        //        int id = GetLastModelIndex() + 1;
        string currmodel = this.ModelStack.GetLastValue();

        string modelname = this.GetModelName(name);
        int id = this.ModelStack.Push(modelname);
        ModelName = modelname;
        //如果变身模型跟当前模型相同则不做更新
        if (!this.ModelStack.ContainsKey(id - 1) || currmodel != modelname)
        {
            //foreach (Ability ability in this.Abilities)
            //    ability.ClearAbilityEffect();

            for (int cIndex = 0; cIndex < this.tAbilities.Length; cIndex++)
            {
                Ability ability = this.tAbilities[cIndex] as Ability;
                if (ability != null)
                {
                    ability.ClearAbilityEffect();
                }
            }

            for (int cIndex = 0; cIndex < this._marksDic.Length; cIndex++)
            {
                BattleMark mark = this._marksDic[cIndex];
                if (mark != null)
                    mark.ClearEffect();
            }

            this.UpdateController();
            //if (CurrentTalent != null && CurrentTalent.Casting)
            //    CurrentTalent.Break();
            //this.Idle();
        }
        IsEnterTransformation = true;
        return id;
    }
    public void CancleLockTarget()
    {
#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("CancleLockTarget {0}=> Lock:{1} Target:{2}", this.ID, this.LockedTarget != null ? this.LockedTarget.FullName : "null", this.Target != null ? this.Target.FullName : "null");
#endif
        this.LockedTarget = null;
        this.Target = null;
        if (this.NavAgent != null)
            this.NavAgent.SetDestination(this.Position);
    }
    public void SetLockTarget(BattleActor target)
    {
#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("SetLockTarget Begin {0}=>{1} {2}", this.ID, target.ID, this.LockedTarget != null ? this.LockedTarget.ID : 0);
#endif
        if (!target.IsDead && this.Distance(target, EngineConst.EnableRadiusInDistance) < this.Data.LockRange && !this.NotSelectable(target))
        {
            this.LockedTarget = target;
            this.Target = target;
        }
#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("SetLockTarget End {0} {1}", this.ID, this.LockedTarget != null ? this.LockedTarget.ID : 0);
#endif
    }
    public virtual void SetLockMode(int lockmode)
    {
    }
    public virtual void SetFindTargetMode(int findTargetMode)
    {

    }

    public virtual void SetOperatingMode(int operatingMode)
    {

    }
    public void RemoveModel(int modelId)
    {

        if (!this.ModelStack.ContainsKey(modelId))
        {
            Logger.LogError("No model!");
            return;
        }
        int oldStackTop = GetLastModelIndex();
        string lastModelName = this.ModelStack.GetLastValue();
        this.ModelStack.Remove(modelId);
        int lastIndex = this.ModelStack.GetLastKey();
        this.ModelName = this.ModelStack.GetLastValue();
        // 移出当前队列中最后一个时才对模型更新
        if (oldStackTop == modelId && (!this.ModelStack.ContainsKey(lastIndex) || lastModelName != ModelName))
        {
            //foreach (Ability ability in this.Abilities)
            //    ability.ClearAbilityEffect();

            for (int cIndex = 0; cIndex < this.tAbilities.Length; cIndex++)
            {
                Ability ability = this.tAbilities[cIndex] as Ability;
                if (ability != null)
                {
                    ability.ClearAbilityEffect();
                }
            }
            for (int cIndex = 0; cIndex < this._marksDic.Length; cIndex++)
            {
                BattleMark mark = this._marksDic[cIndex];
                if (mark != null)
                    mark.ClearEffect();
            }
            this.UpdateController();
            this.needTransformModel = true;
            if (this.RoleST == RoleState.Move)
                this.Move(RoleStateName.Instance[RoleState.Move]);
        }
        IsEnterTransformation = true;
    }
    /// <summary>
    /// 更新控制器
    /// </summary>
    public void UpdateController()
    {
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i];
            if (talent == null) continue;
            talent.InitActions(this.RoleSkin.GetModelKey(ModelName));
        }

        if (this.Joint != null)
        {
            this.Joint.CreateController();
        }
    }

    /// <summary>
    /// 获取单位比例
    /// </summary>
    /// <returns></returns>
    public float GetRoleScale()
    {
        if (string.IsNullOrEmpty(ModelName))
            return 1.0f;
        return NeptuneBattle.Instance.DataProvider.GetModelData(ModelName).Scale * (this.SummonData == null ? this.Config.Scale : 1.0f);
    }

    /// <summary>
    /// 获取当前比例
    /// </summary>
    /// <returns></returns>
    public virtual Vector2 GetCurrentScale()
    {
        //Scale = 1f;
        if (!this.isZoomRunning) return new Vector2(Scale, Scale);

        if (this.zoomRunningTime < this.zoomDuration)
        {
            this.zoomRunningTime = UFloat.Round(this.zoomRunningTime + this.attackAnimDT);
            Scale = this.zoomRunningTime >= this.zoomDuration ? this.zoomValue : UFloat.Round(UFloat.Round(this.zoomRunningTime / this.zoomDuration) * (this.zoomValue - this.ZoomCurValue) + this.ZoomCurValue);
            this.isZoomRunning = !(this.zoomRunningTime >= this.zoomDuration);
        }
        return new Vector2(Scale, Scale);
    }

    Vector3 curHitPoint;
    /// <summary>
    /// 获取受击位置
    /// </summary>
    /// <returns></returns>
    public UVector3 GetHitPoint()
    {
        curHitPoint.x = this.Position.x;
        curHitPoint.y = this.Position.y;
        curHitPoint.z = this.Height + UFloat.RoundToInt(48 * this.GetRoleScale() * this.GetCurrentScale().y);
        return curHitPoint;
    }

    public void StartZoom(float scaleX, float duration)
    {
        SetZoomInfo(true, duration, 0);
    }
    public void EndZoom()
    {
        SetZoomInfo(false, 0, 0);
    }


    public void AddCastEffect(BattleEffect effect)
    {
        this.CastringEffects.Add(effect);
#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("{0} AddCastEffect : {1} Num:{2}", this.FullName, effect.FullName, this.CastringEffects.Count);
#endif
    }

    public void RemoveCastEffect(BattleEffect effect)
    {
        this.CastringEffects.Remove(effect);
#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("{0} RemoveCastEffect : {1} Num:{2}", this.FullName, effect.FullName, this.CastringEffects.Count);
#endif
    }

    public override string ToString()
    {
        string str = string.Format("{0}", this.Data.Name);
        string cdesc = string.Format("{0:f6}", this.CommonCooldown);
        //cdesc = this.ActiveTalents.Aggregate(cdesc, (current, talent) => string.Format("{0}|{1:f3}", current, talent.Duration));

        str = string.Format("{0,-10}*{1}L{2}\tHP:{3,12:f6}/{4,12:f6}\tMP:{5,12:f6}\t({6,5}, {7,5})\t({18,5}, {19,5})\t{21}:{20:f6}\t{8}\t@{9}:%{10:f6} [{11}] {12}{13}{14}{15}{16}{17} {22}\r\n",
            this.FullName, this.Stars, this.Level, this.HP, this.Attributes.MaxHP, this.MP, this.Position.x, this.Position.y, this.RoleST.ToString(), this.AniId, this.AniPlayedTime, cdesc,
            this.AbilityEffects.Inhibition ? "S" : "", this.AbilityEffects.Disable ? "D" : "", this.AbilityEffects.Root ? "I" : "", this.AbilityEffects.Inhuman ? "B" : "", this.AbilityEffects.Immoblilize ? "ST" : "", this.AbilityEffects.Directed ? "F" : "",
            this.Speed.x, this.Speed.y, this.RepelSpeed, this.shove, this.Target == null ? "null" : this.Target.FullName
            );
        return str;
    }


    /// <summary>
    /// 检查是否可以移动到目标
    /// </summary>
    /// <param name="dest"></param>
    public virtual void CheckMove(UVector2 dest)
    {
        int distance = this.Distance(dest, 0, true);
        if (distance > this.AttackRange || this.IsOutOfBattleArea)
        {
            this.MoveToDest(dest);
        }
        else
        {
            this.Idle();
        }
    }

    public void CheckMoveToTarget(UVector2 dest)
    {
        int distance = this.Distance(dest, 0, EngineConst.EnableRadiusInDistance);
        if (distance > this.AttackRange || this.IsOutOfBattleArea)
        {
            this.MoveToDest(dest);
        }
        else
        {
            this.Idle();
        }
    }

    public virtual void CheckMoveToTarget(BattleEntity target)
    {
        int distance = this.Distance(target, EngineConst.EnableRadiusInDistance);
        //Logic.log("CheckMoveToTarget d:"+distance+",ar:"+AttackRange+",area:"+IsOutOfBattleArea.ToString());
        if (distance > this.AttackRange || this.IsOutOfBattleArea)
        {
            if (EngineConst.EnableRadiusInDistance)
            {
                this.MoveToDest(target.Position - ((target.Position - this.Position).normalizedU * target.Radius) * EngineConst.Milli);
            }
            else
                this.MoveToDest(target.Position);
        }
        else
        {
            this.Joint.SetDirection(this.Orientation);
            this.Idle();
        }
    }

    public void SpecAttack(int id)
    {
        string name = "Atk" + id;

        SetAnimation(name, false, false);
    }
    public void SpecIdle(int id)
    {
        string name = "Idle" + id;

        SetAnimation(name, false, false);
    }

    private void SetZoomInfo(bool running, float dur, float runTime)
    {

        this.isZoomRunning = running;
        this.zoomDuration = dur;
        this.zoomRunningTime = runTime;
        float scale = 1.0f;
        this.ZoomCurValue = this.Scale;
        foreach (KeyValuePair<int, float> pair in this.zoomIndex)
        {
            scale *= pair.Value;
        }
        this.zoomValue = scale;
    }

    public virtual void ChangeState(RoleState state)
    {
        this.RoleST = state;
        this.MoveSpeed = UVector2.zero;
    }


    public void EnterAttackState()
    {
        if (isInsideGrass)
        {
            lastAttackStateFrame = NeptuneBattle.Instance.doneFrameCount;
            isLastingAttackState = true;
        }
    }

    protected void UpdateAttackStateTimer()
    {
        if (!isInsideGrass) return;
        if ((NeptuneBattle.Instance.doneFrameCount - lastAttackStateFrame) > grassShowDuration * EngineConst.KEY_FRAME_RATE)
        {
            if (isLastingAttackState)
                isLastingAttackState = false;
        }
    }

    public void ResetAttackStateTimer()
    {
        isLastingAttackState = false;
    }

    public BattleSkill GetTalentById(int id)
    {
        return Talents.ContainsKey(id) ? Talents[id] : null;
    }

    public BattleSkill GetActiveTalentByID(int id)
    {
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i];
            if (talent == null) continue;
            if (talent.Data.ID == id)
                return talent;
        }
        return null;
    }

    public BattleSkill GetActiveTalentByGroupID(int id)
    {
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i];
            if (talent == null) continue;
            if (talent.GroupData.TalentGroupID == id)
                return talent;
        }
        return null;
    }


    public BattleSkill GetLevelTalentBySlot(int index)
    {
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i];
            if (talent == null) continue;
            if (talent.GroupData.Index == index && talent.GroupData.ParentID == 0 && talent.GroupData.InitEnabled)
                return talent;
        }
        return null;
    }


    public BattleSkill GetTalentExtensionById(int id)
    {
        return Talents.ContainsKey(id) ? (BattleSkill)Talents[id] : null;
    }
    public int GetTalentLevelById(int id)
    {
        return Talents.ContainsKey(id) ? Talents[id].Level : 0;
    }

    protected BattleActor ChooseRandomTarget()
    {
        BattleActor tTarget = null;
        float maxDist = float.MinValue;
        foreach (BattleActor role in NeptuneBattle.Instance.GetSurvivors(this, this.AntiSide))
        {
            if (role == null || role.AbilityEffects.Void)
            {
                continue;
            }
            float randNum = Util.Random.Rand();
            if (maxDist < randNum)
            {
                maxDist = randNum;
                tTarget = role;
            }
        }
        return tTarget;
    }

    public bool InSightRange(BattleActor target)
    {
        int magnitude = this.Distance(target, true);
        //取最小距离
        return this.Data.SightRange <= 0 || (magnitude <= this.Data.SightRange);
    }

    public BattleSkill GetActiveTalentBySlot(int slot)
    {
        BattleSkill _priorityTalent = null;
        float _priority = float.MaxValue;
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill talent = this.tActiveTalents[i];
            if (talent != null && talent.IsEnabled && slot == talent.GroupData.Index && talent.GroupData.Priority < _priority)
            {
                _priority = talent.GroupData.Priority;
                _priorityTalent = talent;
            }
        }
        return _priorityTalent;
    }

    public void SetMoveDirection(Vector2 direct)
    {
        this.MoveDirection = direct.normalized;
        UFloat.Round(ref this.moveDirection);

#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
            NeptuneBattle.log("{0} SetMoveDirection MoveDirection:({1:f6},{2:f6})  direct:({3:f6},{4:f6})", this.FullName, this.MoveDirection.x, this.MoveDirection.y, direct.x, direct.y);
#endif

        if ((this.CurrentTalent != null && this.CurrentTalent.Casting && this.CurrentTalent.IsCastingProtect() && !CurrentTalent.Data.MovingCast && CurrentTalent.Data.ContinuousType != ContinuousType.Continuous))
        {
            return;
        }
        //if (!caching)
        {
            IsAutoAttackMode = false;
            //解决释放技能时缓存被清除的问题
            ResetAttackAIMode();
        }
        if (!this.CanMove)
        {
            //这里不需要清除移动方向
            //if (!this.CanMove)
            //{
            //    this.MoveDirection = Vector2.zero;
            //}

            return;
        }

        if (this.CurrentTalent != null && this.CurrentTalent.Casting && !CurrentTalent.Data.MovingCast)
        {
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} SetMoveDirection break.", this.CurrentTalent.ToString());
#endif
            this.CurrentTalent.Break();
        }

        if (direct == EngineConst.Vector2Zero && (CurrentTalent == null || !(CurrentTalent.Casting && CurrentTalent.Data.MovingCast)))
        {
            this.Idle();
        }
    }


    public virtual BattleSkill CastTalentByIndex(int slot, Vector2 direction, int targetIndex = 0)
    {
        throw new NotImplementedException();
    }

    public virtual BattleSkill CastNormalAttackTalent()
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// 是否是技能还是普攻 true技能 false普攻
    /// </summary>
    /// <param name="talent"></param>
    /// <returns></returns>
    public virtual bool TalentOrNormal(BattleSkill talent)
    {
        if (talent == null)
            return false;
        return talent.Data.ID != BasicTalent.Data.ID;
    }

    public virtual void CheckTarget(BattleActor target)
    {
        if (target != null
            && (target.IsDead
            || this.Distance(target, EngineConst.EnableRadiusInDistance) > this.Data.LockRange
            || this.AbilityEffects.Taunt
            || this.AbilityEffects.Fear
            || this.NotSelectable(target))
            //|| (target.AbilityEffects.Invincible && this.Side != target.Side))
            )
            CancleLockTarget();
    }

    public void InvisibleProcess()
    {

        if (this.AbilityEffects.Bare && (this.AbilityEffects.Invisible || this.IsInGrass))
        {

            if (this.Side == RoleSide.SideA)
                this.SetVisible(true, true);
            else if (this.Side == RoleSide.SideB)
                this.SetVisible(true, true);
        }
        else if (this.AbilityEffects.Vision && !this.AbilityEffects.Invisible)
        {
            if (this.Side == RoleSide.SideA)
                this.SetVisible(true, this.IsInGrass);
            else if (this.Side == RoleSide.SideB)
                this.SetVisible(true, this.IsInGrass);
        }
        else if (this.AbilityEffects.Invisible || this.IsInGrass)
        {
            if (this.Side == RoleSide.SideA)
                this.SetVisible(true, true);
            else if (this.Side == RoleSide.SideB)
            {
                if ((NeptuneBattle.Instance.IsReplayMode || NeptuneBattle.Instance.IsWatchMode) && this.IsInGrass)
                    this.SetVisible(true, true);
                else
                    this.SetVisible(false, false);
            }
        }
        else
        {
            if (this.Side == RoleSide.SideA)
                this.SetVisible(true, false);
            else if (this.Side == RoleSide.SideB)
                this.SetVisible(true, false);
        }
    }




    public virtual void RemoveExtraTalentByAbility(Ability ability)
    {

    }

    public Ability FindAbilityByChangeIndex(int changeIndex, int talentid)
    {
        for (int i = 0; i < this.tAbilities.Length; i++)
        {
            Ability ability = this.tAbilities[i] as Ability;
            if (ability != null && ability.FromTalentID == talentid && ability.ChangeIndex == changeIndex)
            {
                return ability;
            }
        }
        return null;
    }


    public void InitChangeTalents(Ability removeAbility = null, int abilityID = 0)
    {
        TransformData period = null;
        if (this.TransformGroupId > 0)
        {
            int transformID = this.TransformStack.GetLastValue();
            period = NeptuneBattle.Instance.DataProvider.GetTransformData(this.TransformGroupId, transformID);
        }
        foreach (KeyValuePair<int, List<int>> changeData in ChangeTalentStack)
        {
            if (period != null && !period.TalentList.Contains(changeData.Key))
            {
                continue;
            }
            if (changeData.Value != null && changeData.Value.Count > 0)
            {
                if (abilityID != 0 && abilityID != changeData.Key)
                    continue;
                Ability ability = null;
                int from = changeData.Key;
                int talentCD = from;
                int index = 0;
                if (removeAbility != null && removeAbility.FromTalentID == changeData.Key && removeAbility.ChangeIndex > changeData.Value.Max())
                {
                    talentCD = removeAbility.ToTalentID;
                    index = removeAbility.ChangeIndex;
                    EnableTalents(changeData.Key, removeAbility.ToTalentID, false);
                }
                else if (changeData.Value.Count > 1)
                {
                    for (int i = 0; i < changeData.Value.Count; i++)
                    {

                        ability = FindAbilityByChangeIndex(changeData.Value[i], changeData.Key);
                        if (ability != null)
                        {
                            if (ability.ChangeIndex == changeData.Value.Max())
                                continue;
                            if (ability.ChangeIndex > index)
                            {
                                talentCD = ability.ToTalentID;
                                index = ability.ChangeIndex;
                            }

                            EnableTalents(changeData.Key, ability.ToTalentID, false);
                        }


                    }
                }

                ability = FindAbilityByChangeIndex(changeData.Value.Max(), changeData.Key);


                if (ability != null)
                {
                    this.ChangeTalents(from, ability.ToTalentID, talentCD);
                }
            }
            else
            {
                if (removeAbility != null && removeAbility.FromTalentID == changeData.Key)
                {
                    this.ChangeTalents(removeAbility.ToTalentID, changeData.Key, removeAbility.ToTalentID);
                }
            }
        }
    }

    public void EnableTalents(int fromTalentID, int talentID, bool enabled)
    {
        if (this.TransformGroupId > 0)
        {
            int transformID = this.TransformStack.GetLastValue();
            TransformData period = NeptuneBattle.Instance.DataProvider.GetTransformData(this.TransformGroupId, transformID);
            if (period != null)
            {
                if (!period.TalentList.Contains(fromTalentID))
                {
                    return;
                }
            }
        }
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill activeTalent = this.tActiveTalents[i] as BattleSkill;
            if (activeTalent == null) continue;
            if (activeTalent.GroupData.TalentGroupID == talentID)
            {
                if (activeTalent.IsEnabled == enabled) continue;
                if (enabled && !activeTalent.IsEnabled)
                {
                    activeTalent.Init(false);

                }
                else if (!enabled && activeTalent.IsEnabled && activeTalent == this.CurrentTalent && activeTalent.Casting && activeTalent.CanInterrupt(InterruptType.ChangeTalent))
                {
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("{0} EnableTalents Break {1} ", this.FullName, activeTalent.Data.ID);
#endif
                    activeTalent.Break();
                }
                activeTalent.IsEnabled = enabled;

            }
        }
    }

    /// <summary>
    /// 替换技能
    /// </summary>
    /// <param name="fromTalent">禁用技能</param>
    /// <param name="toTalent">开启技能</param>
    public virtual void ChangeTalents(int fromTalent, int toTalent, int InheritCDId)
    {
        BattleSkill from = null;
        BattleSkill to = null;
        for (int i = 0; i < this.tActiveTalents.Length; i++)
        {
            BattleSkill activeTalent = this.tActiveTalents[i] as BattleSkill;
            if (activeTalent == null) continue;
            if (activeTalent.GroupData.TalentGroupID == InheritCDId)
            {
                from = activeTalent;
            }

            if (activeTalent.GroupData.TalentGroupID == fromTalent)
            {
                activeTalent.IsEnabled = false;
                if (activeTalent == this.CurrentTalent && activeTalent.Casting && activeTalent.CanInterrupt(InterruptType.ChangeTalent))
                {
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("{0} ChangeTalents Break {1} ", this.FullName, activeTalent.Data.ID);
#endif
                    activeTalent.Break();
                }
            }
            else if (activeTalent.GroupData.TalentGroupID == toTalent)
            {
                if (!activeTalent.IsEnabled)
                {
                    activeTalent.Init(false);
                }
                to = activeTalent;
                activeTalent.IsEnabled = true;
            }
        }
        if (from != null && from.Duration <= 0 && from.Data.DisplaceResetCD)
        {
            from.Duration = from.CD;
        }
        if (from != null && to != null && to.GroupData.InheritCD)
            to.Duration = from.Duration;

    }

    public virtual void PlayBirthAnim()
    {
        string modelname = this.RoleSkin.GetModelKey(this.Data.Model);

        ModelData model = NeptuneBattle.Instance.DataProvider.GetModelData(modelname);
        if (model == null)
        {
            Logger.LogError("null model : " + this.Data.ID);
            return;
        }
        string resourceName = model.Resource;
        if (string.IsNullOrEmpty(resourceName))
        {
            return;
        }

        Dictionary<string, Neptune.GameData.AnimationConfigData> resAniConfigData = NeptuneBattle.Instance.DataProvider.GetAnimationConfigDatas(resourceName);
        if (resAniConfigData == null)
        {
            Logger.LogWarning(string.Format("{0} not found in AnimationConfig", resourceName));
            return;
        }

        string animName = RoleStateName.Instance[RoleState.Birth];
        if (!resAniConfigData.ContainsKey(animName))
        {
            return;
        }
        this.ChangeState(RoleState.Birth);
        this.SetAnimation(RoleStateName.Instance[RoleState.Birth], true, true);
        this.birthTime = resAniConfigData[animName].TotalTime;
    }


    public static bool PredicateAlive(BattleActor role)
    {
        return !role.IsDead;
    }


    public static bool PredicateDeadBody(BattleActor role)
    {
        return role.RoleST == RoleState.Death /*&& role.IsDeadBody */&& role.AniTotaltime < 5;
    }
    public Vector2 LerpVector(float rotateSpeed, float dt, Vector2 curDirect, Vector2 toDirect)
    {
        if (curDirect == EngineConst.Vector2Zero || toDirect == EngineConst.Vector2Zero)
            return curDirect;
        Vector3 cross = Vector3.Cross(curDirect, toDirect);
        int dir = 1;
        if (cross.z > 0)
        {
            dir = -1;
        }
        Vector3 rotate = (Quaternion.AngleAxis(rotateSpeed * dt * dir, Vector3.up) * new Vector3(curDirect.x, 0, curDirect.y));
        Vector2 delta = new Vector2(UFloat.Round(rotate.x), UFloat.Round(rotate.z));


        if (Vector2.Angle(curDirect, curDirect + delta) > Vector2.Angle(curDirect, toDirect))
        {

            return toDirect.normalized;
        }
        else
        {
            return (curDirect + delta).normalized;
        }
    }
}


public class HeroAttackRangeComparer : IComparer<RoleInfo>
{
    public int Compare(RoleInfo x, RoleInfo y)
    {
        int xValue = NeptuneBattle.Instance.DataProvider.GetTalentDatas(BattleActor.LoadRoleData((int)x.tid).BasicTalent)[0].MaxRange;
        int yValue = NeptuneBattle.Instance.DataProvider.GetTalentDatas(BattleActor.LoadRoleData((int)y.tid).BasicTalent)[0].MaxRange;
        return xValue.CompareTo(yValue);
    }
}

public class UnitAttackRangeComparer : IComparer<BattleActor>
{

    public int Compare(BattleActor x, BattleActor y)
    {
        Debug.Assert(false);
        return 0;
        //return NeptuneBattle.Instance.DataProvider.GetTalentDatas(BattleActor.LoadRoleData((int)x.Hero.tid).BasicTalent)[0].MaxRange.CompareTo(NeptuneBattle.Instance.DataProvider.GetTalentDatas(BattleActor.LoadRoleData((int)y.Hero.tid).BasicTalent)[0].MaxRange);
    }
}

public class TalentPriorityComparer : IComparer<BattleSkill>
{

    public int Compare(BattleSkill x, BattleSkill y)
    {
        if (x == null && y == null)
            return 0;

        if (x == null)
            return 1;
        if (y == null)
            return -1;

        if (!x.IsEnabled || x.GroupData == null)
        {
            return 1;
        }
        if (!y.IsEnabled || y.GroupData == null)
        {
            return -1;
        }
        if (x.GroupData.Priority == y.GroupData.Priority && x != y)
        {
            Logger.LogError(string.Format("{0}[{1}] --> {2}[{3}] Priority Equal {4}", x.Data.TalentName, x.GroupData.TalentGroupID, y.Data.TalentName, y.GroupData.TalentGroupID, x.GroupData.Priority));
        }
        return x.GroupData.Priority > y.GroupData.Priority ? -1 : 1;
    }
}
public class ActionEventComparer : IComparer<AniEventData>
{

    public int Compare(AniEventData x, AniEventData y)
    {
        if (x.Time == y.Time)
        {
            if (x.Type == "attackspot" && y.Type == "end")
            {
                return 1;
            }
            else if (x.Type == "end" && y.Type == "attackspot")
            {
                return -1;
            }

            return 0;
        }

        else
            return x.Time < y.Time ? -1 : 1;

    }

}

public class UnitRandComparer : IComparer<BattleActor>
{

    public int Compare(BattleActor x, BattleActor y)
    {
        return x.rand > y.rand ? 1 : -1;
    }
}