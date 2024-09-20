
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Neptune.GameData;
using log4net.Core;

namespace Neptune
{
    /// <summary>
    /// 战斗管理器，单例，管理所有战斗数据及逻辑
    /// </summary>
    public class NeptuneBattle : Singleton<NeptuneBattle>//战斗数据的基本判断
    {
        // 战斗状态（貌似不用了OnEnterFame）
        const int BATTLE_STAGE_INIT = -1;
        const int BATTLE_STAGE_WATCHING = 0;
        const int BATTLE_STAGE_FIGHTING = 1;
        const int BATTLE_STAGE_FINISHED = 2;
        const int BATTLE_STAGE_PLAY_ANIMATION_BEFORE_START = 3;
        // 貌似也不用了（OnEnterFame）
        const float FIGHTING_TIMEOUT = 60f * 3f;
        const float NEW_PVE_FIGHTING_TIMEOUT = 60 * 1.5f;
        const int INTERVAL_SAVE_TEMP_BATTLE = 10;    // -- 临时保存战斗结果的时间间隔，10s
        private float logicTime = 0;

        public float LogicTime { get { return logicTime; } }
        // 是否为回放
        public bool IsReplayMode;

        public bool IsNoRenderMode;

        public IBattleStatistic Statistic = null;

        public string replayFileName = "";//记录回放的文件名称
        public int replayNum = 0;//标记第几次replay.
        public int maxReplayFrame = 0;
        public int fastPlayFrameNum = 1;//通过调整这个可以回放快播，1表示正常速度
        public int quitWhenFinish = 0;//标记是否回放完成就马上退出

        public bool IsStartLoaded = false;//标记是否已经启动加载了

        public bool needWaitForStart = false;
        public bool isReEnter = false;//标记是否是重新进入游戏

        // 是否为观战
        public bool IsWatchMode;


        public int watchUserId = 0;//记录为观战的userId.
                                   // 回放中再试一次标记
        public bool IsTryAgain;
        // 是否是boss关
        public bool IsBossLevel = false;
        // 是否是pvp竞技

        // 是否为实时战斗播放
        public bool IsLiveMode;

        public bool IsLoginFromFight = false;//标记是否从中途登陆游戏.

        // 未使用？？
        public bool GMMode;
        // 战斗关卡数据
        public BattleLevelData LevelData;
        // 战斗每一波战斗数据
        public CombatConfigData CombatConfig;
        // 关卡怒气增益系数
        public float MPBonus;
        // 关卡生命值增益系数
        public float HPBonus;
        // 远征难度模式里的生命和能量恢复系数
        public float CrusadeModeRecoverBonus;
        // 回合数
        public int Round;
        // Hero Or Monster
        public bool isCreateRole;
        // Actor Index
        private int roleIndex = 0;
        public int RoleIndex
        {
            get
            {
                return roleIndex;
            }
            set
            {
                roleIndex = value;
            }
        }
        private int effectIndex = 0;
        // 前进速度系数
        public float MarchingSpeed = 1.75f;
        // 是否正在运行
        public bool Running = false;
        // 战斗引擎状态
        public bool Enabled;
        // 帧计数
        public int doneFrameCount;

        public static float realDeltaTime = EngineConst.KEY_FRAME_INTERVAL;

        //正常速度，每渲染帧的最大普通移动距离。
        public static float maxPreMoveDistMFM = 20;
        //动作帧前执行帧数，预设20帧，即计算20帧后，将会走到的地点为目标
        public static int logicFrameCount = 20;

        float lastFrameTime = 0;

        // 战斗金币掉落奖励
        public int GoldCount = 0;
        // 战斗宝箱掉落奖励统计
        public int LootCount = 0;

        // 神庙需要的额外数据
        public int monsterIndex;
        public int ownerUserId;

        // 暂停级别
        public int PauseLevel;
        // 暂停方
        public RoleSide PauseSide;

        // 战斗评星
        public int ResultStars;

        // 己方英雄数据列表保存
        public List<RoleInfo> HeroList;
        // 双方英雄角色对象列表
        public List<BattleActor> Roles = new List<BattleActor>();
        // 玩家操作数据记录（回放会用到）
        public List<OpRecord> OpRecords = new List<OpRecord>();

        // 英雄存活数（每一方的存活）
        public Dictionary<int, int> AliveCount = new Dictionary<int, int>();
        // 英雄死亡数（每一方的死亡）
        public Dictionary<int, int> DeadCount = new Dictionary<int, int>();

        // 存活角色对象保存
        public Dictionary<int, SafeList<BattleActor>> AliveRoles = new Dictionary<int, SafeList<BattleActor>>()
    {
        { (int)RoleSide.SideA , new SafeList<BattleActor>() },
        { (int)RoleSide.SideB , new SafeList<BattleActor>() },
        { (int)RoleSide.SideC , new SafeList<BattleActor>() },
        { (int)RoleSide.All , new SafeList<BattleActor>() }
    };
        public List<BattleActor> Summons = new List<BattleActor>();

        public List<BattleTeam> Teams = new List<BattleTeam>();

        // TODO: 看Effects和Traps列表是否可以合并 (YuBo)
        public List<BattleEntity> CastEffects = new List<BattleEntity>();

        private List<BattleEntity> CastTraps = new List<BattleEntity>();

        // 怪物上阵最大限制
        const int MAX_POSITION = 5;
        // 战斗状态
        private int _battleState = -1;
        // 关卡是否结束
        public bool LevelEnded;
        // 战斗结束
        public bool BattleEnd;

        // 战斗场景指针
        public IBattleScene Scene { get; set; }
        // 战斗开始时间
        public long beginTime = 0;
        // 已经无效（废弃）
        private float lastSaveTempBattleTime = 0;
        // 前进方向
        public int TargetDoneFrameCount = 0;

        public Vector3 ForwardDirection
        {
            get
            {
                if (Scene != null)
                {
                    return Scene.ForwardDirection;
                }
                return EngineConst.Vector3One;
            }
        }
        // 前进角度
        public Vector3 ForwardAngle
        {
            get
            {
                if (Scene != null)
                {
                    return Scene.ForwardAngles;
                }
                return EngineConst.Vector3One;
            }
        }
        // 这个是3d中使用的，位置节点
        public Vector3[] BattlePoints
        {
            get
            {
                if (Scene != null)
                {
                    return Scene.BattlePoints;
                }
                return new Vector3[3];
            }
        }
        // 挑战中的防守方id（不应该放在战斗逻辑中）    
        public int DefenderID;
        // 龙穴类型id（不应该放在战斗逻辑中）
        public int TreasureTypeID;
        // 怪物总数（没有使用过？？）
        int totalMonster = 0;
        //  手动操作索引（大招）
        int NextOperationIdx;

        public bool Supplied;
        // 己方英雄id列表
        List<int> HeroIDList = new List<int>();

        /// <summary>
        /// 游戏规则管理器
        /// </summary>
        public IRuleManager RuleManager = null;

        /// <summary>
        /// 寻路接口
        /// </summary>
        public INavigator Navigator = null;

        /// <summary>
        /// 障碍规避接口
        /// </summary>
        public IOrcaSimulator Simulator = null;

        /// <summary>
        /// 战斗引擎数值接口
        /// </summary>
        public INumeric Numeric = null;

        /// <summary>
        /// AOI 管理器
        /// </summary>
        public AOIManager AOIManager = null;

        /// <summary>
        /// 引擎数据提供者
        /// </summary>
        public IEngineDataProvider DataProvider = new EngineDataManager();

        /// <summary>
        /// 类构造函数
        /// </summary>
        /// 

        // GM Controlled Debug Options  
        public bool isShowEffect = true;
        public bool isDispatchTroops = true;
        public bool isUpdateMonster = true;
        public bool isShowMiniMap = true;
        public bool popupTextOn = true;
        public bool showBloodBar = true;
        public bool showBattleNotice = true;
        public bool equipUpdate = true;
        public bool isSoundPlay = true;
        public bool applyRoleEffectOn = true;
        public bool damageEmissColorOn = true;

        public NeptuneBattle()
        {

        }

        /// <summary>
        /// 角色单位列表添加
        /// </summary>
        /// <param name="role"></param>
        /// <param name="promisor"></param>
        /// <param name="position"></param>
        /// <param name="init_action"></param>
        public void AddActor(BattleActor role, BattleActor promisor = null, UVector2 position = new UVector2(), string init_action = "")
        {
            //Logger.Log("role Add:" + role.ID);
            Roles.Add(role);
            //role.Index = this.Roles.Count;
            role.Index = RoleIndex;
            RoleIndex++;
            if (role.IsRoleType(RoleType.Spell))
                return;
            role.Config.Promisor = promisor;
            if (role.IsDead)
            {
                DeadCount[(int)role.Side]++;
            }
            else
            {
                AliveRoles[(int)role.Side].Add(role);
                AliveRoles[(int)RoleSide.All].Add(role);
                AliveCount[(int)role.Side]++;

                role.Born(init_action);
            }
            if (promisor == null)
            {
                role.PreviousPosition = role.Position;
                role.GlobalIdx = Roles.Count - 1;
            }
            else
            {
                promisor.SummonList.Add(role);
                role.Config.IsDemon = true;
                role.Position = position;
                role.PreviousPosition = role.Position;
                if (role.Joint != null)
                    role.Joint.OnEnterFrame(0);
            }
        }


        public void RoleReborn(BattleActor hero, Vector2 position)
        {
            AliveRoles[(int)hero.Side].Add(hero);
            AliveRoles[(int)RoleSide.All].Add(hero);
            AliveCount[(int)hero.Side]++;
            DeadCount[(int)hero.Side]--;
            hero.RoleST = RoleState.Idle;
            hero.IsExtraInited = false;
            hero.Position = position;
            hero.Init(true);
            hero.OnReborn();
            hero.CallBackByRebirth();
            if (Scene != null)
            {
                Scene.RoleReborn(hero);
            }

        }

        /// <summary>
        /// 生成怪物单位信息配置
        /// </summary>
        /// <param name="isboss"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private RoleConfig CreateConfig(bool isboss, int index)
        {
            RoleConfig config = new RoleConfig();
            config.IsBoss = false;
            config.IsMonster = true;
            config.Scale = 1;
            config.HPFactor = (CombatConfig.MonsterHPPct > 0 ? CombatConfig.MonsterHPPct : 100) / (float)100;
            config.DmgPerSecFactor = (CombatConfig.MonsterDPSPct > 0 ? CombatConfig.MonsterDPSPct : 100) / (float)100;
            config.Money = (int)typeof(CombatConfigData).GetProperty(string.Format("MoneyReward{0}", index)).GetValue(CombatConfig, null);
            config.PredictQuality = true;
            config.PredictTalent = true;
            config.PredictMaxQuality = false;
            if (isboss)
            {
                config.IsBoss = true;
                config.Scale = config.Scale * (CombatConfig.BOSSSIZEPct > 0 ? CombatConfig.BOSSSIZEPct : 120) / 100;
                config.HPFactor = config.HPFactor * (CombatConfig.BossHPPct > 0 ? CombatConfig.BossHPPct : 100) / 100;
                config.DmgPerSecFactor = config.DmgPerSecFactor * (CombatConfig.BossDPSPct > 0 ? CombatConfig.BossDPSPct : 100) / 100;
            }

            return config;
        }
        /// <summary>
        /// pve类战斗调用：初始化己方英雄（InitPlayerHeroes、InitMonsters）
        /// </summary>
        /// <param name="heroList"></param>
        /// <param name="isbot"></param>
        public void InitPlayerHeroes(List<RoleInfo> heroList, bool isbot)
        {
            //初始化玩家英雄数据
            HeroList = heroList;
            BubbleSort.Sort(HeroList, new HeroAttackRangeComparer());
            RoleConfig config = new RoleConfig();
            config.PredictQuality = isbot;
            config.PredictTalent = isbot;
            config.PredictMaxQuality = false;

            isCreateRole = true;
            foreach (RoleInfo hero in HeroList)
            {
                //RoleIndex = this.HeroList.IndexOf(hero);
                BattleActor role = ObjectFactory.Create(hero, RoleSide.SideA, config, null, EngineConst.Vector3Zero, Vector3.right);
                AddActor(role);
                HeroIDList.Add(hero.tid);
                //TODO:Init Actor hire data here
                role.activeTalentReadyToGo = isbot;
            }
        }
        /// <summary>
        /// pve类战斗调用：怪物初始化（InitPlayerHeroes、InitMonsters）
        /// </summary>
        /// <param name="ls"></param>
        /// <param name="boss"></param>
        private void InitMonsters(Dictionary<int, RoleExtra> ls, int boss)
        {
            RoleExtra extra = null;
            isCreateRole = false;
            for (int i = 0, curIndex = 0; i < MAX_POSITION; i++)
            {
                extra = ls != null && ls.ContainsKey(i) ? ls[i] : null;
                int id = (int)typeof(CombatConfigData).GetProperty(string.Format("Monster{0}ID", i + 1)).GetValue(CombatConfig, null);
                if (id <= 0)
                    continue;

                totalMonster++;
                if (extra != null && extra.HP == 0)
                    continue;

                int lv = (int)typeof(CombatConfigData).GetProperty(string.Format("Level{0}", i + 1)).GetValue(CombatConfig, null);
                int st = (int)typeof(CombatConfigData).GetProperty(string.Format("Stars{0}", i + 1)).GetValue(CombatConfig, null);
                RoleConfig cfg = CreateConfig(i + 1 == boss, i + 1);

                RoleInfo hero = new RoleInfo();
                hero.tid = id;
                hero.level = lv;
                hero.stars = st;
                //RoleIndex = curIndex;
                UVector2 position = new UVector2();
                position.x = (int)(BattleField.Current.rect.xMax * EngineConst.InitBattleXFactor) - EngineConst.InitialPosition[curIndex - 1].x;
                position.y = EngineConst.InitialPosition[curIndex - 1].y;
                BattleActor ro = ObjectFactory.Create(hero, RoleSide.SideB, cfg, extra, position, Vector3.left);
                ro.MonIdx = i + 1;
                AddActor(ro);
                ro.MP = (int)typeof(CombatConfigData).GetProperty(string.Format("MP{0}", ro.MonIdx)).GetValue(CombatConfig, null);
                if (ro.HP > 0)
                {
                    curIndex++;
                }
                //boss战测试
                foreach (int k in CombatConfig.BossPosition)//设定是否为boss，再根据是否为boss战来设定大小
                {
                    if (curIndex == k)
                    {
                        ro.IsBoss = true;
                    }
                }
                //boss战测试
                ro.Joint.Controller.SetPosition(ro.Position);
            }
        }


        /// <summary>
        /// 获取场上活着的索引到的英雄
        /// </summary>
        /// <param name="index"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public BattleActor GetRole(int index, RoleSide side)
        {
            if (AliveRoles[(int)side].Count >= index)
            {
                return AliveRoles[(int)side][index - 1];
            }
            return null;
        }
        /// <summary>
        /// 获取某阵营场上活着的英雄
        /// </summary>
        /// <param name="index"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public IEnumerable<BattleEntity> GetSurvivors(RoleSide side)
        {
            SafeList<BattleActor> list = AliveRoles[(int)side];
            for (int i = list.Count - 1; i >= 0; i--)
            {
                BattleActor role = list[i];
                if (list.IsRemoved(role))
                    continue;
                yield return role;
            }
            list.Final();
        }

        public IEnumerable<BattleEntity> GetSurvivors(RoleSide selfSide, RelativeSide side)
        {
            for (int si = 1; si <= EngineConst.SideNum; si++)
            {
                if (side == RelativeSide.Both || side == RelativeSide.Friend ? (int)selfSide == si : (int)selfSide != si)
                {
                    SafeList<BattleActor> list = AliveRoles[(int)(RoleSide)si];
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        BattleActor role = list[i];
                        if (list.IsRemoved(role))
                            continue;
                        yield return role;
                    }
                    list.Final();
                }
            }
        }

        /// <summary>
        /// 获取指定关系的存活单位
        /// </summary>
        /// <param name="self"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public IEnumerable<BattleEntity> GetSurvivors(BattleEntity self, RelativeSide side)
        {
            for (int si = 1; si <= EngineConst.SideNum; si++)
            {
                RoleSide selfSide = (RoleSide)si;
                if (side == RelativeSide.Both || self.GetRelation(selfSide) == side)
                {
                    SafeList<BattleActor> list = AliveRoles[(int)selfSide];
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        BattleActor role = list[i];
                        if (list.IsRemoved(role))
                            continue;
                        yield return role;
                    }
                    list.Final();
                }
            }
        }



        /// 获取某阵营场上活着的英雄
        /// </summary>
        /// <param name="self"></param>
        /// <param name="side"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        //    public IEnumerable<Entity> GetSurvivors(Entity self, RelativeSide side, float range)
        //    {
        //        //for (int si = 1; si <= EngineConst.SideNum; si++)
        //        //{
        //        //    RoleSide selfSide = (RoleSide)si;
        //        //    if (side == RelativeSide.Both || self.GetRelation(selfSide) == side)
        //        //    {
        //        //        SafeList<Actor> list = this.AliveRoles[selfSide];
        //        //        for (int i = list.Count - 1; i >= 0; i--)
        //        //        {
        //        //            Actor role = list[i];
        //        //            if (role.removed || (self.Position - role.Position).sqrMagnitude > range * range)
        //        //            {//限定最大获取范围
        //        //                continue;
        //        //            }
        //        //            yield return role;
        //        //        }
        //        //        list.Final();
        //        //    }
        //        //}
        ////        foreach (Entity survivor in AOIManager.GetSurvivors(self, range))
        ////        {   
        ////            if (side == RelativeSide.Both || self.GetRelation(survivor.Side) == side)
        ////            {
        ////                if (self.Distance(survivor, EngineConst.EnableRadiusInDistance) <= range)
        ////                {
        ////                    yield return survivor;
        ////                }
        ////                
        ////            }
        ////        }
        //        
        //    }

        /// <summary>
        /// 召唤（召唤兽）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <param name="reverse"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public BattleActor Summon(int id, int level, bool reverse, BattleActor owner)
        {
            SummonData data = Instance.DataProvider.GetSummonData(id, level);
            BattleActor summon = new BattleActor(data, reverse, owner);
            Summons.Add(summon);
            return summon;
        }
        /// <summary>
        /// 获得召唤兽
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BattleActor> GetSommons()
        {
            foreach (BattleActor role in Summons)
            {
                yield return role;
            }
        }

        /// <summary>
        /// 部署一组战斗角色
        /// </summary>
        /// <param name="side">阵营</param>
        /// <param name="heroes">单位列表</param>
        /// <param name="inputExtra">附加数据</param>
        /// <param name="spawnPoints">出生点</param>
        /// <param name="orientationPoints"></param>
        public void DeployRole(RoleSide side, List<RoleInfo> heroes, Dictionary<int, RoleExtra> inputExtra, List<Vector3> spawnPoints, Vector3 orientationPoints, BattlePlayer player)
        {
            if (heroes.Count != spawnPoints.Count)
            {
                Debug.LogError("DeployRole error: spawn point number mismatching");
                return;
            }

            for (int i = 0; i < heroes.Count; i++)
            {
                RoleInfo hero = heroes[i];
                //初始化血量数据
                RoleExtra extra = null;
                if (inputExtra != null)
                {
                    extra = inputExtra[hero.tid];
                    if (extra.HP == 0)
                    {
                        continue;
                    }
                }

#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    log("Add Actor (Multi) Id: {0} at  Position: {1}", hero.tid, spawnPoints[i]);
#endif

                BattleActor role = DeployRole(side, hero, extra, spawnPoints[i], orientationPoints, player, null, null, true);
                if (role.Joint != null)
                    role.Joint.OnBorn(i);
                role.DeployTime += i * EngineConst.KEY_FRAME_INTERVAL;
            }
        }


        public BattleActor DeployRole(RoleSide side, RoleInfo hero, RoleExtra inputExtra, Vector3 spawnPoint, Vector2 orientation, BattlePlayer player, RoleData data = null, RoleConfig role_config = null, bool isRobot = false)
        {
            //Debug.LogFormat(">>>>>>>> DeployRole : Logic:{0}", spawnPoint);
            if (role_config == null)
                role_config = new RoleConfig();
            //初始化角色配置
            BattleActor.InitConfigPredictInfo(role_config, hero.tid, false);
            role_config.RoleSkinID = hero.skinid;
            //初始化血量数据
            RoleExtra extra = null;
            if (inputExtra != null)
            {
                extra = inputExtra;
                if (extra.HP == 0)
                {
                    return null;
                }
            }
            //创建Role
            BattleActor role = null;
            if (isRobot)
                role = ObjectFactory.Create(hero, side, role_config, extra, spawnPoint, orientation, data, isRobot);
            else
                role = ObjectFactory.Create(hero, side, role_config, extra, spawnPoint, orientation, data);
            if (role.IsRoleType(RoleType.Hero))
                role_config.IsHero = true;

            AddActor(role);
            HeroIDList.Add(hero.tid);
            role.Player = player;
            return role;
        }
        /// <summary>
        /// 部署一组战斗角色
        /// </summary>
        /// <param name="side">阵营</param>
        /// <param name="heroes">单位列表</param>
        /// <param name="inputExtra">附加数据</param>
        /// <param name="spawnPoints">出生点</param>
        /// <param name="orientationPoints"></param>
        public void DeployRole(RoleSide side, List<RoleInfo> heroes, Dictionary<int, RoleExtra> inputExtra, List<Vector3> spawnPoints, List<Vector2> orientationPoints, BattlePlayer player)
        {
            if (heroes.Count != spawnPoints.Count)
            {
                Debug.LogError("DeployRole error: spawn point number mismatching");
                return;
            }

            for (int i = 0; i < heroes.Count; i++)
            {
                RoleInfo hero = heroes[i];
                //初始化血量数据
                RoleExtra extra = null;
                if (inputExtra != null)
                {
                    extra = inputExtra[hero.tid];
                    if (extra.HP == 0)
                    {
                        continue;
                    }
                }
                DeployRole(side, hero, extra, spawnPoints[i], orientationPoints[i], player, null);
            }
        }

        // 这个函数已经无效，不被调用了（OnEnterFame）
        private bool checkDoBattleAction()
        {
            while (OpRecords.Count > NextOperationIdx)
            {
                OpRecord record = OpRecords[NextOperationIdx];
                if (record.Tick == doneFrameCount)
                {
                    BattleActor role = Roles[record.RoleIdx];
                    if (role != null)
                    {
                        if (record.OpCode == 0)
                            return true;
                    }
                }
                else
                    NextOperationIdx++;
            }
            return false;
        }

        /// <summary>
        /// 处理自动操作
        /// </summary>
        void ProcessOP()
        {
            while (OpRecords.Count > NextOperationIdx)
            {
                OpRecord record = OpRecords[NextOperationIdx];
                if (record.Tick == doneFrameCount)
                {
                    BattleActor role = Roles[record.RoleIdx];
                    if (role != null)
                    {
                        if (record.OpCode == 0)
                            role.UseActiveTalent();
                    }
                    NextOperationIdx++;
                }
                else
                    break;
            }
        }

        /// <summary>
        /// 处理元件逻辑
        /// </summary>
        /// <param name="dt"></param>
        void ProcessEntities(float dt)
        {
            if (Simulator != null)
            {
                Simulator.ComputeSuperposeAgent();
            }

            int roleNum = Roles.Count;
            for (int i = 0; i < roleNum; i++)
            {
                BattleActor element = Roles[i];
                if (!element.IsPause)
                {
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        log("{0} Process >>>>>>", element.FullName);
#endif
                    element.OnEnterFrame(dt);

                    Instance.AOIManager.Move(element);
                }
                else
                {
                    if (element.Joint.Controller != null)
                        element.Joint.Controller.SetAnimationSpeed(0);
                }
                roleNum = Roles.Count;
            }

            Roles.RemoveAll(BattleEntity.RemoveStopedElement);

            roleNum = Roles.Count;
            if (Simulator != null)
            {
                for (int i = 0; i < roleNum; i++)
                {
                    BattleActor element = Roles[i];
                    if (!element.IsPause && element.OrcaAgent != null)
                    {
                        element.OrcaAgent.Sync();
                    }
                }
            }

            int CastCount = CastEffects.Count;
            for (int i = 0; i < CastCount; i++)
            {
                BattleEntity element = CastEffects[i];
                if (!element.IsPause)
                {
                    element.OnEnterFrame(dt);
                }

                CastCount = CastEffects.Count;
            }
            CastEffects.RemoveAll(BattleEntity.RemoveStopedElement);

            int trapCount = CastTraps.Count;
            for (int i = 0; i < trapCount; i++)
            {
                BattleEntity ele = CastTraps[i];
                if (!ele.IsPause)
                {
                    ele.OnEnterFrame(dt);
                }
            }
            CastTraps.RemoveAll(BattleEntity.RemoveStopedElement);

            int summonCount = Summons.Count;
            for (int i = 0; i < summonCount; i++)
            {
                BattleActor element = Summons[i];
                if (!element.IsPause)
                {
                    element.OnEnterFrame(dt);
                }
            }
            Summons.RemoveAll(BattleEntity.RemoveStopedElement);
        }


        /// <summary>
        ///  重置场上英雄角色数据
        /// </summary>
        private void ResetRoles()
        {
            BattleActor[] roles = Roles.ToArray();
            Roles.Clear();
            AliveRoles[(int)RoleSide.All].Clear();
            AliveRoles[(int)RoleSide.SideA].Clear();
            AliveRoles[(int)RoleSide.SideB].Clear();
            AliveRoles[(int)RoleSide.SideC].Clear();

            AliveCount[(int)RoleSide.SideA] = 0;
            AliveCount[(int)RoleSide.SideB] = 0;
            AliveCount[(int)RoleSide.SideC] = 0;

            DeadCount[(int)RoleSide.SideA] = 0;
            DeadCount[(int)RoleSide.SideB] = 0;
            DeadCount[(int)RoleSide.SideC] = 0;
            foreach (BattleActor role in roles)
            {
                if (role.Side == RoleSide.SideA)
                    AddActor(role);
            }
        }

        /// <summary>
        /// 初始化己方英雄出生位置？？？？
        /// </summary>
        public void InitPlayerAliveRoles()
        {
            int i = 0;
            foreach (BattleActor role in AliveRoles[(int)RoleSide.SideA])
            {
                Vector2 position = new Vector2();
                position.x = EngineConst.InitialPosition[i].x - BattleField.Current.rect.xMax * EngineConst.InitBattleXFactor;//从战场最左侧外出生
                position.y = EngineConst.InitialPosition[i].y;
                i++;
                role.Joint.Disable = false;
                role.Position = position;//在战斗中的开始位置
            };
        }
        /// <summary>
        /// 初始化进入战斗的关卡数据
        /// </summary>
        public void InitBattleCore()
        {
            ObjectFactory.Instance.Init();
            //重置关卡数据
            Enabled = true;
            doneFrameCount = 0;
            //this.fixedFrameTime = 0.125f;

            HeroIDList.Clear();
            Statistic.Clear();
            Roles.Clear();
            OpRecords.Clear();
            NextOperationIdx = 0;
            RoleIndex = 0;
            effectIndex = 0;
            //this.IsReplayMode = false;
            //this.IsWatchMode = false; 不在这里初始化
            IsLiveMode = false;
            TargetDoneFrameCount = 0;
            IsTryAgain = false;
            IsBossLevel = false;
            TreasureTypeID = 0;
            LevelEnded = false;
            CastEffects.Clear();
            CastTraps.Clear();
            Summons.Clear();
            ResetRoles();
            totalMonster = 0;
            PauseLevel = 0;
            PauseSide = RoleSide.None;
            Supplied = false;
            Running = false;
            BattleEnd = false;
            //this.accDeltaTime = fixedFrameTime;
            GoldCount = 0;
            LootCount = 0;
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
            {
                log("ResetLevel");
            }
#endif
        }

        public void SetCombatConfig(CombatConfigData combat)
        {
            CombatConfig = combat;
        }
        /// <summary>
        /// 战斗初始化(pve战斗)
        /// </summary>
        /// <param name="combat"></param>
        /// <param name="extra_list"></param>
        public void InitBattle(Dictionary<int, RoleExtra> ls = null)
        {
            Logger.LogCombat("Logic::InitBattle");
            //初始化战斗数据
            CastEffects.Clear();
            CastTraps.Clear();
            Summons.Clear();

            ResetRoles();
            totalMonster = 0;
            PauseLevel = 0;
            PauseSide = RoleSide.None;
            Supplied = false;
            Running = true;
            BattleEnd = false;
            //this.accDeltaTime = fixedFrameTime;
            RuleManager.Reset();

            Round = CombatConfig.RoundID > 0 ? CombatConfig.RoundID : 1;

            InitMonsters(ls, CombatConfig.BossPosition.Count > 0 ? CombatConfig.BossPosition[0] : 0);
            InitPlayerAliveRoles();
            foreach (BattleActor role in Roles)
            {
                role.Init();
            }
            BattleLevel level = new BattleLevel(LevelData.LevelID, Round);
            level.Config();

            if (Scene != null)
            {
                Scene.OnBattleStart("pve", CombatConfig);
            }
        }
        /// <summary>
        /// 战斗初始化(pvp战斗)
        /// </summary>
        public void InitPvpBattle()
        {
            EasyCounter.Instance.Start("Logic::InitPvpBattle");
            RuleManager.Reset();
            //初始化战斗数据
            CastEffects.Clear();
            CastTraps.Clear();
            Summons.Clear();

            BattleEnd = false;
            if (Scene != null)
            {
                Scene.OnBattleStart("pvp", CombatConfig);
            }
            EasyCounter.Instance.End("Logic::InitPvpBattle");
        }

        public void Rest(bool bEnemyRest = false)
        {
            foreach (BattleActor role in GetSurvivors(bEnemyRest ? RoleSide.All : RoleSide.SideA))
            {
                float factor = 1f;
                if (LevelData.MpsRestraint > 0)
                {
                    factor = LevelData.MpsRestraint / (float)100;
                }
                role.Rest(factor);
                role.ClearAbilities();
            }
            if (!bEnemyRest)
            {
                Supplied = true;
            }
        }

        /// <summary>
        /// 结束战斗
        /// </summary>
        public void EndBattle(bool immediately = false)
        {
            if (!IsReplayMode)
                beginTime = EngineUtil.GetServerTime() - (long)LogicTime;

            foreach (BattleEntity e in CastEffects)
            {
                e.Stop();
                e.Delete();
            }

            foreach (BattleEntity e in CastTraps)
            {
                e.Delete();
                e.Stop();
            }

            Enabled = false;
            Running = false;
            BattleEnd = false;
            LevelEnded = true;
            isReEnter = false;
            IsStartLoaded = false;

            if (RuleManager != null)
            {
                RuleManager.OnBattleEnd(immediately);
            }

            if (Scene != null)
            {//通知场景结束战斗
                Scene.OnBattleEnd(immediately);
            }


            int roleNum = Roles.Count;
            for (int i = 0; i < roleNum; i++)
            {
                Roles[i].Delete();
            }
            RvoSimulator.Destroy();

            Logger.LogCombat("Logic::EndBattle");
            if (immediately)
                Logger.Roll();
        }

        public void SendMsgFrameOp()
        {
            RuleManager.SendMsgFrameOp();
        }
        /// <summary>
        /// 战斗tick
        /// </summary>
        /// <param name="passTime"></param>
        public void onTimer(float passTime)
        {
            if (!Running)
                return;
            //先处理服务器返回的操作信息
            doneFrameCount++;
            //CachedLog.Log("update onTimer.next:" + doneFrameCount + ",max:" + NetworkWinManager.Instance.maxRecvFrameNo + ",time:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"));
            logicTime = doneFrameCount * EngineConst.KEY_FRAME_INTERVAL;

            RuleManager.Update(passTime);

            ProcessOP();
            ProcessEntities(passTime);
            //this.Simulator.Update();
            //CachedLog.Log("update onTimer End.next:" + doneFrameCount + ",max:" + NetworkWinManager.Instance.maxRecvFrameNo + ",time:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"));

            if (lastFrameTime > 0)
                realDeltaTime = Time.realtimeSinceStartup - lastFrameTime;
            lastFrameTime = Time.realtimeSinceStartup;

#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                log(ToString());
#endif

        }

        //public void Proceed()
        //{
        //    foreach (Actor role in this.GetSurvivors(RoleSide.SideA))
        //    {
        //        if (role.Config.IsDemon)
        //            role.End(null);
        //    }
        //    if (Logic.Instance.Scene != null)
        //    {
        //        if (this.Scene != null)
        //            this.Scene.ShowNextButton();

        //        foreach (Actor role in this.GetSurvivors(RoleSide.SideA))
        //        {
        //            if (role.Joint != null)
        //            {
        //                role.Joint.Waiting();
        //            }

        //        }
        //    }
        //    else
        //    {
        //        if (!this.Supplied)
        //            this.Rest(false);

        //        CombatConfigData combatcfg = EngineDataManager.Instance.CombatConfigs.Value[this.LevelData.LevelID][this.Round + 1];
        //        this.SetCombatConfig(combatcfg);
        //        this.InitBattle();
        //    }
        //}

        /// <summary>
        /// 单位死亡事件
        /// </summary>
        /// <param name="attacker"></param>
        public void OnRoleDeath(BattleActor role, BattleActor attacker, BattleSkill fromTalent = null)
        {
            Instance.AOIManager.RemoveRole(role);
            NeptuneBattle logic = Instance;
            logic.AliveRoles[(int)role.Side].Remove(role);
            logic.AliveRoles[(int)RoleSide.All].Remove(role);

            foreach (BattleActor u in logic.AliveRoles[(int)RoleSide.All])
            {
                u.OnRoleDeath(role, attacker, fromTalent);
            }

            //         if (attacker != null && attacker.IsHero && !role.Config.IsDemon)
            //         {
            //             attacker.SetMP(attacker.MP + 300);
            //             if (Logic.Instance.Scene != null && attacker.Joint != null)
            //             {
            //                 //Logic.Instance.Scene.PopupText(PopupType.KillingRewards, string.Empty, attacker.Joint, role.Joint, false, attacker.Side);//暂时不用
            //             }
            //         }

            logic.AliveCount[(int)role.Side]--;
            logic.DeadCount[(int)role.Side]++;
            if (attacker != null)
            {
                attacker.DeathNote.Add(role.ID);
            }
            if (logic.Scene != null && role.Side == RoleSide.SideB)
            {
                //TODO:显示掉落
                logic.Scene.ShowDropLoots(logic.Round, role, 1, 0);
            }

            if (role.AuraTalents.Count > 0)
            {
                AliveRolesInitAttributes(role.Side, RelativeSide.Friend);
            }

            if (role.EnemyAuraTalents.Count > 0)
            {
                AliveRolesInitAttributes(role.Side, RelativeSide.Enemy);
            }

            if (RuleManager != null)
            {
                RuleManager.OnRoleDeath(role, attacker);
            }
            if (Scene != null)
            {
                Scene.OnRoleDeath(role, attacker, fromTalent);
            }

        }

        public void OnRoleHit(BattleActor role)
        {
            if (RuleManager != null)
            {
                RuleManager.OnRoleHit(role);
            }
        }

        public void AliveRolesInitAttributes(RoleSide mySide, RelativeSide relaSide)
        {
            foreach (var element in Instance.GetSurvivors(mySide, relaSide))
            {
                var role = (BattleActor)element;
                role.InitAttributes();
            }
        }


        public void GetCurrentHeroesData(List<RoleExtra> myroles, List<RoleExtra> enemies, List<RoleExtra> hires)
        {
            foreach (BattleActor role in Roles)
            {
                if (!role.Config.IsDemon)
                {
                    RoleExtra hero = new RoleExtra();
                    hero.tid = role.ID;
                    hero.HP = (int)Math.Ceiling(role.HP * EngineConst.PercentageRatio / role.Attributes.MaxHP);
                    hero.MP = (int)Math.Ceiling(role.MP * EngineConst.PercentageRatio / role.Attributes.MaxMP);
                    hero.Rage = (int)Math.Ceiling(role.Rage * EngineConst.PercentageRatio / role.Attributes.MaxRage);
                    hero.Point = (int)Math.Ceiling(role.Point * EngineConst.PercentageRatio / role.Attributes.MaxPoint);
                    if (role.ExtraData != null)
                    {
                        hero.ExtraData = role.ExtraData.ExtraData;
                    }
                    if (role.Side == RoleSide.SideA)
                    {
                        if (myroles != null)
                        {
                            myroles.Add(hero);
                        }
                        if (hires != null/* && role.IsHired*/)
                            hires.Add(hero);
                    }
                    else if (role.Side == RoleSide.SideB)
                    {
                        if (enemies != null)
                        {
                            enemies.Add(hero);
                        }
                    }
                }
            }
        }

        public void CastTalent(BattleActor role)
        {
            OpRecord record = new OpRecord()
            {
                OpCode = 0,
                Round = Round,
                Tick = doneFrameCount,
                RoleIdx = role.GlobalIdx
            };
            Instance.OpRecords.Add(record);
        }


        public void AddEffect(BattleEffect effect)
        {
            if (effect != null)
            {
                CastEffects.Add(effect);
                ++effectIndex;
                effect.EffectIndex = effectIndex;
                if (effect.Type == EffectType.Cast)
                {
                    effect.LastHitPosition = new Vector2(effect.Position.x, effect.Position.y);
                    if (Scene != null && Instance.isShowEffect && Scene.EffecVisible(effect) && !IsNoRenderMode)
                    {
                        IEffectAgent actor = ObjectFactory.Create(effect, EffectType.Cast);
                        Scene.AddJoint(actor);
                        Scene.AddEffectOnMap(effect);
                    }
                }
            }
        }

        public void AddTrap(BattleTrap trap)
        {
            if (trap != null)
            {
                CastTraps.Add(trap);
            }
        }
        public void TriggerTrap(BattleActor role, int trapid)
        {
            BattleTrap trap = null;
            for (int i = 0; i < CastTraps.Count; i++)
            {
                trap = CastTraps[i] as BattleTrap;
                if (trap.trapId == trapid && trap.Talent.Caster == role)
                    trap.CastManualAction();
            }
        }
        public UVector2 HitTrap(BattleActor role, UVector2 curPos, UVector2 nextPos)
        {
            UVector2 hit = UVector2.zero;
            BattleTrap trap = null;
            for (int i = 0; i < CastTraps.Count; i++)
            {
                trap = CastTraps[i] as BattleTrap;
                if (trap.Talent.Caster.GetRelation(role) == trap.AffectedSide)
                {
                    hit = trap.ObstacleWallHit(curPos, nextPos);
                    if (hit != UVector2.zero)
                        break;
                }
            }
            return hit;
        }

        public void RemoveTrap(BattleActor role, int trapid, int count = 0)
        {
            BattleTrap trap = null;
            int remmovecount = 0;
            for (int i = 0; i < CastTraps.Count; i++)
            {
                if (count > 0 && remmovecount >= count)
                    break;
                trap = CastTraps[i] as BattleTrap;
                if (trap.trapId == trapid && trap.Talent.Caster == role)
                {
                    remmovecount++;
                    trap.Stop();
                }

            }
        }
        public void RemoveTrap(BattleTrap trap)
        {
            for (int i = 0; i < CastTraps.Count; i++)
            {
                if (trap == CastTraps[i])
                    trap.Stop();
            }
        }


        public void Resume(bool force = false)
        {
            if (!EngineConst.BattlePauseLevelEnable || PauseLevel == 0 && force)
            {
                PauseSide = RoleSide.None;
                return;
            }

            PauseLevel = force ? 0 : --PauseLevel;

            if (PauseLevel != 0)
            {
                return;
            }
            PauseSide = RoleSide.None;

            ResumeAllAnimation();

            if (Scene != null)
            {
                Scene.PlayEffect(ScreenEffect.Freeze, 0, 0f);
            }
        }

        public void SuspendAllAnimation(bool needMask = true)
        {
            foreach (BattleEntity role in Roles)
            {
                role.Suspend(needMask);
            }

            foreach (BattleEntity summ in Summons)
            {
                summ.Suspend(needMask);
            }

            foreach (BattleEntity cast in CastEffects)
            {
                cast.Suspend(needMask);
            }

            foreach (BattleEntity elem in CastTraps)
            {
                elem.Suspend(needMask);
            }
        }

        public void ResumeAllAnimation()
        {
            foreach (BattleEntity element in Roles)
            {
                element.Resume();
            }
            foreach (BattleEntity element in Summons)
            {
                element.Resume();
            }
            foreach (BattleEntity element in CastEffects)
            {
                element.Resume();
            }
            foreach (BattleEntity element in CastTraps)
            {
                element.Resume();
            }
        }

        public bool isElementInSelfHeroes(int tid)
        {
            foreach (RoleInfo h_ in HeroList)
            {
                if (h_.tid == tid)
                {
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            string s = string.Format("============================================================\r\nTick {0} : UFloat:{1} Round:{2}\r\n", doneFrameCount, UFloat.count, UFloat.round_count);
            UFloat.count = 0;
            UFloat.round_count = 0;
            foreach (BattleActor role in Roles)
            {
                s = s + role.ToString();
            }
            if (CastEffects.Count > 0)
                s += "CAST_EFFECT:\r\n";
            foreach (BattleEntity element in CastEffects)
            {
                s = s + element.ToString();
            }
            if (CastTraps.Count > 0)
            {
                s += "Cast_Traps: \r\n";
                foreach (BattleEntity elem in CastTraps)
                {
                    s += elem.ToString();
                }
            }
            return s;
        }

        public static void log(string log)
        {
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
            {
                Logger.LogCombat(log);
            }
#endif
        }

        public static void log(string format, params object[] args)
        {
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                log(string.Format(format, args));
#endif
        }

        //    public void LoadLevel(string name, List<RoleInfo> heroes, LevelData level, UnityEngine.Events.UnityAction onLoad, E_BattleMode mode = E_BattleMode.Training, int param = 0)
        //    {   
        //#if SERVER
        //        onLoad();
        //#else   
        //        BattleSceneLoader.Instance.LoadLevel(name, heroes, level, onLoad, mode, param);
        //#endif      
        //    }   

        public static IBattleAgent PlayEffect(string effectRes, Vector2 pos, Vector2 direction, float height, int z, BattleEffect effect = null, Action<IEffectController> onlaod = null, BattleEntity element = null)
        {
            if (Instance.Scene != null && Instance.isShowEffect && !Instance.IsNoRenderMode)
            {
                IBattleAgent joint = ObjectFactory.Create(effectRes, pos, direction, height, z, effect, onlaod, element);
                Instance.Scene.AddJoint(joint);

                return joint;
            }
            return null;
        }
        public static IBattleAgent PlayEffect(int effect_id, Vector2 pos, Vector2 scale, float height, int z)
        {
            if (Instance.IsNoRenderMode) return null;
            string effect = Instance.DataProvider.GetEffects(effect_id);
            if (string.IsNullOrEmpty(effect))
                return null;
            return PlayEffect(effect, pos, scale, height, z);
        }

        private static float lastStartTime = 0;

        /// <summary>
        /// 根据玩家逻辑帧，和速度，方向，计算相应的表现层的坐标
        /// </summary>
        /// <param name="vDirVector">//当前方向</param>
        /// <param name="vObjPostion">//当前角色的逻辑位置</param>
        /// <param name="vGoPostion">//当前角色表现层位置</param>
        /// <param name="lastVelocityf">当前角色的速度</param>
        /// <param name="passTime">当前表现层的时间</param>
        /// <param name="maxPreMoveDistMFM">玩家普通移动在一帧里移动的距离，一个固定值//每帧最大移动距离MFM</param>
        /// <returns></returns>
        public static Vector2 getGoPosition(Vector2 vDirVector, UVector2 vObjPostion, Vector2 vGoPostion, float lastVelocityf, float passTime, bool needDebug)
        {
            //CachedLog.Log("time:" + Time.realtimeSinceStartup + ",dv:" + vDirVector.ToString() + ",op:" + vObjPostion.ToString() + ",gp:" + vGoPostion.ToString()
            //    + "lastv:" + lastVelocityf + ",pt:" + passTime + ",mfm:" + maxPreMoveDistMFM);
            //在特定的时间里移动的距离
            var dDistance = lastVelocityf * passTime;
            //预计位置
            var vExpectPosition = vGoPostion + vDirVector * dDistance;
            Vector2 vLhs;
            vLhs.x = vExpectPosition.x - vObjPostion.x;
            vLhs.y = vExpectPosition.y - vObjPostion.y;

            //相差距离
            var dExpectManitude = vLhs.magnitude;

            //计算5帧后将会移动的距离，作为一个距离范围判断。
            float dMaxPreActMoveDist = maxPreMoveDistMFM * logicFrameCount;

            Vector2 retval = vExpectPosition;
            //if (needDebug) CachedLog.Log("time:" + Time.realtimeSinceStartup + ",start.vexp:" + vExpectPosition.ToString() + ",vlhs:" + vLhs.ToString() + ",dmax:" + dMaxPreActMoveDist + ",exp:" + dExpectManitude + ",maxpre:" + maxPreMoveDistMFM);
            if (dExpectManitude >= maxPreMoveDistMFM)
            {
                //已经有较大的误差，需要想办法往真正的逻辑坐标点靠近
                if (dExpectManitude < dMaxPreActMoveDist)
                {
                    //还在容忍范围内，想办法往真正的逻辑坐标点靠近
                    var dClDistance = Mathf.Clamp(dExpectManitude / dMaxPreActMoveDist, 0.05f, 0.3f);

                    var dDirDistance = Vector3.Dot(vLhs, vDirVector);

                    var vPosMaxMoveWithDir = vObjPostion + vDirVector * maxPreMoveDistMFM * (logicFrameCount + 3);

                    var vNormalized = (vPosMaxMoveWithDir - vGoPostion).normalized;

                    //if (needDebug) CachedLog.Log("time:" + Time.realtimeSinceStartup + ",long distance.dexp:" + dExpectManitude + ",dcl:" + dClDistance + ",ddir:" + dDirDistance + ",vpos:" + vPosMaxMoveWithDir.ToString() + ",vNor:" + vNormalized.ToString());
                    //判断玩家目标的角度是否大于45度角，如果小于45度就减速，如果大于45度，就加速
                    if (dDirDistance > dExpectManitude * 0.707f)
                    {
                        //表示小于45度
                        retval = vGoPostion + vNormalized * dDistance * (1 - dClDistance);
                    }
                    else
                    {
                        //表示大于45度
                        retval = vGoPostion + vNormalized * dDistance * (1 + dClDistance);
                    }
                }
                else
                {
                    //if (needDebug) CachedLog.Log("time:" + Time.realtimeSinceStartup + ",long long distance");
                    //这种情况，误差太大，直接位移到相应的逻辑坐标
                    retval = vObjPostion;
                }

            }
            else
            {
                //误差少，这种情况能容忍，所以直接返回预计的坐标
                //if (needDebug) CachedLog.Log("time:" + Time.realtimeSinceStartup + ",less distance");
            }
            float distance = (retval - vGoPostion).magnitude;

            string s = "";
            if (lastStartTime == 0)
            {
                lastStartTime = Time.realtimeSinceStartup;
            }
            else
            {
                s = ",s1:" + (Time.realtimeSinceStartup - lastStartTime);
                lastStartTime = Time.realtimeSinceStartup;
            }
            //if (needDebug) CachedLog.Log("time:" + Time.realtimeSinceStartup + ",dv:" + vDirVector.ToString() + ",op:" + vObjPostion.ToString() + ",gp:" + vGoPostion.ToString() + "lastv:" + lastVelocityf + ",pt:" + passTime + ",mfm:" + maxPreMoveDistMFM + ",ret:" + retval.ToString()+",d1:"+ dDistance+",d2:"+distance+s);

            return retval;
        }
        public static int getStageBossData(int stageId)
        {
            BattleLevelData data = Instance.DataProvider.GetLevelData(stageId);
            if (data == null)
                return 0;
            int chapterId = data.SectionID;

            Dictionary<int, CombatConfigData> dict = Instance.DataProvider.GetCombatConfigDatas(stageId);
            CombatConfigData battleData = null;
            RoleData unitData = null;

            int key = 0;
            foreach (var v in dict)
            {
                if (battleData == null || v.Key > key)
                {
                    battleData = v.Value;
                }
            }

            if (battleData != null)
            {
                if (battleData.BossPosition.Find(delegate (int ID) { return ID == 1; }) == 1)
                {
                    return battleData.Monster1ID;
                }
                if (battleData.BossPosition.Find(delegate (int ID) { return ID == 2; }) == 2)
                {
                    return battleData.Monster2ID;
                }
                if (battleData.BossPosition.Find(delegate (int ID) { return ID == 3; }) == 3)
                {
                    return battleData.Monster3ID;
                }
                if (battleData.BossPosition.Find(delegate (int ID) { return ID == 4; }) == 4)
                {
                    return battleData.Monster4ID;
                }
                if (battleData.BossPosition.Find(delegate (int ID) { return ID == 5; }) == 5)
                {
                    return battleData.Monster5ID;
                }
            }

            return 0;
        }
    }
}