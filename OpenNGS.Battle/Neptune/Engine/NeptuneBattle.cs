
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Neptune.Datas;

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
        public LevelData LevelData;
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
                this.roleIndex = value;
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

        public static float realDeltaTime = NeptuneConst.KEY_FRAME_INTERVAL;

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
        public List<Actor> Roles = new List<Actor>();
        // 玩家操作数据记录（回放会用到）
        public List<OpRecord> OpRecords = new List<OpRecord>();

        // 英雄存活数（每一方的存活）
        public Dictionary<int, int> AliveCount = new Dictionary<int, int>()
        {
            { (int)RoleSide.SideA , 0 },
            { (int)RoleSide.SideB , 0 },
            { (int)RoleSide.SideC , 0 },
            { (int)RoleSide.All , 0 }
        };
        // 英雄死亡数（每一方的死亡）
        public Dictionary<int, int> DeadCount = new Dictionary<int, int>()
        {
            { (int)RoleSide.SideA , 0 },
            { (int)RoleSide.SideB , 0 },
            { (int)RoleSide.SideC , 0 },
            { (int)RoleSide.All , 0 }
        };

        // 存活角色对象保存
        public Dictionary<int, SafeList<Actor>> AliveRoles = new Dictionary<int, SafeList<Actor>>()
    {
        { (int)RoleSide.SideA , new SafeList<Actor>() },
        { (int)RoleSide.SideB , new SafeList<Actor>() },
        { (int)RoleSide.SideC , new SafeList<Actor>() },
        { (int)RoleSide.All , new SafeList<Actor>() }
    };
        public List<Actor> Summons = new List<Actor>();

        public List<Team> Teams = new List<Team>();

        // TODO: 看Effects和Traps列表是否可以合并 (YuBo)
        public List<Entity> CastEffects = new List<Entity>();

        private List<Entity> CastTraps = new List<Entity>();

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
                if (this.Scene != null)
                {
                    return this.Scene.ForwardDirection;
                }
                return NeptuneConst.Vector3One;
            }
        }
        // 前进角度
        public Vector3 ForwardAngle
        {
            get
            {
                if (this.Scene != null)
                {
                    return this.Scene.ForwardAngles;
                }
                return NeptuneConst.Vector3One;
            }
        }
        // 这个是3d中使用的，位置节点
        public Vector3[] BattlePoints
        {
            get
            {
                if (this.Scene != null)
                {
                    return this.Scene.BattlePoints;
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
        public void AddActor(Actor role, Actor promisor = null, UVector2 position = new UVector2(), string init_action = "")
        {
            //Logger.Log("role Add:" + role.ID);
            this.Roles.Add(role);
            //role.Index = this.Roles.Count;
            role.Index = RoleIndex;
            RoleIndex++;
            if (role.IsRoleType(RoleType.Spell))
                return;
            role.Config.Promisor = promisor;
            if (role.IsDead)
            {
                this.DeadCount[(int)role.Side]++;
            }
            else
            {
                this.AliveRoles[(int)role.Side].Add(role);
                this.AliveRoles[(int)RoleSide.All].Add(role);
                this.AliveCount[(int)role.Side]++;

                role.Born(init_action);
            }
            if (promisor == null)
            {
                role.PreviousPosition = role.Position;
                role.GlobalIdx = this.Roles.Count - 1;
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


        public void RoleReborn(Actor hero, Vector2 position)
        {
            this.AliveRoles[(int)hero.Side].Add(hero);
            this.AliveRoles[(int)RoleSide.All].Add(hero);
            this.AliveCount[(int)hero.Side]++;
            this.DeadCount[(int)hero.Side]--;
            hero.RoleST = RoleState.Idle;
            hero.IsExtraInited = false;
            hero.Position = position;
            hero.Init(true);
            hero.OnReborn();
            hero.CallBackByRebirth();
            if (this.Scene != null)
            {
                this.Scene.RoleReborn(hero);
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
            config.HPFactor = (this.CombatConfig.MonsterHPPct > 0 ? this.CombatConfig.MonsterHPPct : 100) / (float)100;
            config.DmgPerSecFactor = (this.CombatConfig.MonsterDPSPct > 0 ? this.CombatConfig.MonsterDPSPct : 100) / (float)100;
            config.Money = (int)typeof(CombatConfigData).GetProperty(string.Format("MoneyReward{0}", index)).GetValue(this.CombatConfig, null);
            config.PredictQuality = true;
            config.PredictSkill = true;
            config.PredictMaxQuality = false;
            if (isboss)
            {
                config.IsBoss = true;
                config.Scale = config.Scale * (this.CombatConfig.BOSSSIZEPct > 0 ? this.CombatConfig.BOSSSIZEPct : 120) / 100;
                config.HPFactor = config.HPFactor * (this.CombatConfig.BossHPPct > 0 ? this.CombatConfig.BossHPPct : 100) / 100;
                config.DmgPerSecFactor = config.DmgPerSecFactor * (this.CombatConfig.BossDPSPct > 0 ? this.CombatConfig.BossDPSPct : 100) / 100;
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
            this.HeroList = heroList;
            BubbleSort.Sort(HeroList, new HeroAttackRangeComparer());
            RoleConfig config = new RoleConfig();
            config.PredictQuality = isbot;
            config.PredictSkill = isbot;
            config.PredictMaxQuality = false;

            isCreateRole = true;
            foreach (RoleInfo hero in this.HeroList)
            {
                //RoleIndex = this.HeroList.IndexOf(hero);
                Actor role = ObjectFactory.Create(hero, RoleSide.SideA, config, null, NeptuneConst.Vector3Zero, Vector3.right);
                this.AddActor(role);
                this.HeroIDList.Add(hero.tid);
                //TODO:Init Actor hire data here
                role.activeSkillReadyToGo = isbot;
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
                extra = ((ls != null && ls.ContainsKey(i)) ? ls[i] : null);
                int id = (int)typeof(CombatConfigData).GetProperty(string.Format("Monster{0}ID", i + 1)).GetValue(this.CombatConfig, null);
                if (id <= 0)
                    continue;

                totalMonster++;
                if (extra != null && extra.HP == 0)
                    continue;

                int lv = (int)typeof(CombatConfigData).GetProperty(string.Format("Level{0}", i + 1)).GetValue(this.CombatConfig, null);
                int st = (int)typeof(CombatConfigData).GetProperty(string.Format("Stars{0}", i + 1)).GetValue(this.CombatConfig, null);
                RoleConfig cfg = CreateConfig((i + 1) == boss, i + 1);

                RoleInfo hero = new RoleInfo();
                hero.tid = id;
                hero.level = lv;
                hero.stars = st;
                //RoleIndex = curIndex;
                UVector2 position = new UVector2();
                position.x = (int)(BattleField.Current.rect.xMax * NeptuneConst.InitBattleXFactor) - NeptuneConst.InitialPosition[curIndex - 1].x;
                position.y = NeptuneConst.InitialPosition[curIndex - 1].y;
                Actor ro = ObjectFactory.Create(hero, RoleSide.SideB, cfg, extra, position, Vector3.left);
                ro.MonIdx = i + 1;
                this.AddActor(ro);
                ro.MP = (int)typeof(CombatConfigData).GetProperty(string.Format("MP{0}", ro.MonIdx)).GetValue(this.CombatConfig, null);
                if (ro.HP > 0)
                {
                    curIndex++;
                }
                //boss战测试
                foreach (int k in this.CombatConfig.BossPosition)//设定是否为boss，再根据是否为boss战来设定大小
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
        public Actor GetRole(int index, RoleSide side)
        {
            if (this.AliveRoles[(int)side].Count >= index)
            {
                return this.AliveRoles[(int)side][index - 1];
            }
            return null;
        }
        /// <summary>
        /// 获取某阵营场上活着的英雄
        /// </summary>
        /// <param name="index"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public IEnumerable<Entity> GetSurvivors(RoleSide side)
        {
            SafeList<Actor> list = this.AliveRoles[(int)side];
            for (int i = list.Count - 1; i >= 0; i--)
            {
                Actor role = list[i];
                if (list.IsRemoved(role))
                    continue;
                yield return role;
            }
            list.Final();
        }

        public IEnumerable<Entity> GetSurvivors(RoleSide selfSide, RelativeSide side)
        {
            for (int si = 1; si <= NeptuneConst.SideNum; si++)
            {
                if (side == RelativeSide.Both || side == RelativeSide.Friend ? ((int)selfSide == si) : ((int)selfSide != si))
                {
                    SafeList<Actor> list = this.AliveRoles[(int)(RoleSide)si];
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        Actor role = list[i];
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
        public IEnumerable<Entity> GetSurvivors(Entity self, RelativeSide side)
        {
            for (int si = 1; si <= NeptuneConst.SideNum; si++)
            {
                RoleSide selfSide = (RoleSide)si;
                if (side == RelativeSide.Both || self.GetRelation(selfSide) == side)
                {
                    SafeList<Actor> list = this.AliveRoles[(int)selfSide];
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        Actor role = list[i];
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
        //        //for (int si = 1; si <= NeptuneConst.SideNum; si++)
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
        ////                if (self.Distance(survivor, NeptuneConst.EnableRadiusInDistance) <= range)
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
        public Actor Summon(int id, int level, bool reverse, Actor owner)
        {
            SummonData data = NeptuneBattle.Instance.DataProvider.GetSummonData(id, level);
            Actor summon = new Actor(data, reverse, owner);
            this.Summons.Add(summon);
            return summon;
        }
        /// <summary>
        /// 获得召唤兽
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Actor> GetSommons()
        {
            foreach (Actor role in this.Summons)
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
        public void DeployRole(RoleSide side, List<RoleInfo> heroes, Dictionary<int, RoleExtra> inputExtra, List<Vector3> spawnPoints, Vector3 orientationPoints, Player player)
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
                    NeptuneBattle.log("Add Actor (Multi) Id: {0} at  Position: {1}", hero.tid, spawnPoints[i]);
#endif

                Actor role = DeployRole(side, hero, extra, spawnPoints[i], orientationPoints, player, null, null, true);
                if (role.Joint != null)
                    role.Joint.OnBorn(i);
                role.DeployTime += i * NeptuneConst.KEY_FRAME_INTERVAL;
            }
        }


        public Actor DeployRole(RoleSide side, RoleInfo hero, RoleExtra inputExtra, Vector3 spawnPoint, Vector2 orientation, Player player, RoleData data = null, RoleConfig role_config = null, bool isRobot = false)
        {
            //Debug.LogFormat(">>>>>>>> DeployRole : NeptuneBattle:{0}", spawnPoint);
            if (role_config == null)
                role_config = new RoleConfig();
            //初始化角色配置
            Actor.InitConfigPredictInfo(role_config, (int)hero.tid, false);
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
            Actor role = null;
            if (isRobot)
                role = ObjectFactory.Create(hero, side, role_config, extra, spawnPoint, orientation, data, isRobot);
            else
                role = ObjectFactory.Create(hero, side, role_config, extra, spawnPoint, orientation, data);
            if(role == null)
            {
                return null;
            }
            if (role.IsRoleType(RoleType.Hero))
                role_config.IsHero = true;

            this.AddActor(role);
            this.HeroIDList.Add(hero.tid);
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
        public void DeployRole(RoleSide side, List<RoleInfo> heroes, Dictionary<int, RoleExtra> inputExtra, List<Vector3> spawnPoints, List<Vector2> orientationPoints, Player player)
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
            while (this.OpRecords.Count > this.NextOperationIdx)
            {
                OpRecord record = this.OpRecords[this.NextOperationIdx];
                if (record.Tick == this.doneFrameCount)
                {
                    Actor role = this.Roles[record.RoleIdx];
                    if (role != null)
                    {
                        if (record.OpCode == 0)
                            return true;
                    }
                }
                else
                    this.NextOperationIdx++;
            }
            return false;
        }

        /// <summary>
        /// 处理自动操作
        /// </summary>
        void ProcessOP()
        {
            while (this.OpRecords.Count > this.NextOperationIdx)
            {
                OpRecord record = this.OpRecords[this.NextOperationIdx];
                if (record.Tick == this.doneFrameCount)
                {
                    Actor role = this.Roles[record.RoleIdx];
                    if (role != null)
                    {
                        if (record.OpCode == 0)
                            role.UseActiveSkill();
                    }
                    this.NextOperationIdx++;
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

            int roleNum = this.Roles.Count;
            for (int i = 0; i < roleNum; i++)
            {
                Actor element = this.Roles[i];
                if (!element.IsPause)
                {
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("{0} Process >>>>>>", element.FullName);
#endif
                    element.OnEnterFrame(dt);

                    NeptuneBattle.Instance.AOIManager.Move(element);
                }
                else
                {
                    if (element.Joint.Controller != null)
                        ((IActorController)element.Joint.Controller).SetAnimationSpeed(0);
                }
                roleNum = this.Roles.Count;
            }

            this.Roles.RemoveAll(Entity.RemoveStopedElement);

            roleNum = this.Roles.Count;
            if (Simulator != null)
            {
                for (int i = 0; i < roleNum; i++)
                {
                    Actor element = this.Roles[i];
                    if (!element.IsPause && element.OrcaAgent != null)
                    {
                        element.OrcaAgent.Sync();
                    }
                }
            }

            int CastCount = this.CastEffects.Count;
            for (int i = 0; i < CastCount; i++)
            {
                Entity element = this.CastEffects[i];
                if (!element.IsPause)
                {
                    element.OnEnterFrame(dt);
                }

                CastCount = this.CastEffects.Count;
            }
            this.CastEffects.RemoveAll(Entity.RemoveStopedElement);

            int trapCount = CastTraps.Count;
            for (int i = 0; i < trapCount; i++)
            {
                Entity ele = CastTraps[i];
                if (!ele.IsPause)
                {
                    ele.OnEnterFrame((dt));
                }
            }
            this.CastTraps.RemoveAll(Entity.RemoveStopedElement);

            int summonCount = this.Summons.Count;
            for (int i = 0; i < summonCount; i++)
            {
                Actor element = this.Summons[i];
                if (!element.IsPause)
                {
                    element.OnEnterFrame(dt);
                }
            }
            this.Summons.RemoveAll(Entity.RemoveStopedElement);
        }


        /// <summary>
        ///  重置场上英雄角色数据
        /// </summary>
        private void ResetRoles()
        {
            Actor[] roles = this.Roles.ToArray();
            this.Roles.Clear();
            this.AliveRoles[(int)RoleSide.All].Clear();
            this.AliveRoles[(int)RoleSide.SideA].Clear();
            this.AliveRoles[(int)RoleSide.SideB].Clear();
            this.AliveRoles[(int)RoleSide.SideC].Clear();

            this.AliveCount[(int)RoleSide.SideA] = 0;
            this.AliveCount[(int)RoleSide.SideB] = 0;
            this.AliveCount[(int)RoleSide.SideC] = 0;

            this.DeadCount[(int)RoleSide.SideA] = 0;
            this.DeadCount[(int)RoleSide.SideB] = 0;
            this.DeadCount[(int)RoleSide.SideC] = 0;
            foreach (Actor role in roles)
            {
                if (role.Side == RoleSide.SideA)
                    this.AddActor(role);
            }
        }

        /// <summary>
        /// 初始化己方英雄出生位置？？？？
        /// </summary>
        public void InitPlayerAliveRoles()
        {
            int i = 0;
            foreach (Actor role in this.AliveRoles[(int)RoleSide.SideA])
            {
                Vector2 position = new Vector2();
                position.x = NeptuneConst.InitialPosition[i].x - BattleField.Current.rect.xMax * NeptuneConst.InitBattleXFactor;//从战场最左侧外出生
                position.y = NeptuneConst.InitialPosition[i].y;
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
            this.Enabled = true;
            this.doneFrameCount = 0;
            //this.fixedFrameTime = 0.125f;

            this.HeroIDList.Clear();
            this.Statistic.Clear();
            this.Roles.Clear();
            this.OpRecords.Clear();
            this.NextOperationIdx = 0;
            this.RoleIndex = 0;
            this.effectIndex = 0;
            //this.IsReplayMode = false;
            //this.IsWatchMode = false; 不在这里初始化
            this.IsLiveMode = false;
            this.TargetDoneFrameCount = 0;
            this.IsTryAgain = false;
            this.IsBossLevel = false;
            this.TreasureTypeID = 0;
            this.LevelEnded = false;
            this.CastEffects.Clear();
            this.CastTraps.Clear();
            this.Summons.Clear();
            ResetRoles();
            this.totalMonster = 0;
            this.PauseLevel = 0;
            this.PauseSide = RoleSide.None;
            this.Supplied = false;
            this.Running = false;
            this.BattleEnd = false;
            //this.accDeltaTime = fixedFrameTime;
            this.GoldCount = 0;
            this.LootCount = 0;
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
            {
                NeptuneBattle.log("ResetLevel");
            }
#endif
        }

        public void SetCombatConfig(CombatConfigData combat)
        {
            this.CombatConfig = combat;
        }
        /// <summary>
        /// 战斗初始化(pve战斗)
        /// </summary>
        /// <param name="combat"></param>
        /// <param name="extra_list"></param>
        public void InitBattle(Dictionary<int, RoleExtra> ls = null)
        {
            Logger.LogCombat("NeptuneBattle::InitBattle");
            //初始化战斗数据
            this.CastEffects.Clear();
            this.CastTraps.Clear();
            this.Summons.Clear();

            ResetRoles();
            this.totalMonster = 0;
            this.PauseLevel = 0;
            this.PauseSide = RoleSide.None;
            this.Supplied = false;
            this.Running = true;
            this.BattleEnd = false;
            //this.accDeltaTime = fixedFrameTime;
            this.RuleManager.Reset();

            this.Round = this.CombatConfig.RoundID > 0 ? this.CombatConfig.RoundID : 1;

            InitMonsters(ls, this.CombatConfig.BossPosition.Length > 0 ? this.CombatConfig.BossPosition[0] : 0);
            InitPlayerAliveRoles();
            foreach (Actor role in this.Roles)
            {
                role.Init();
            }
            Level level = new Level(this.LevelData.ID, this.Round);
            level.Config();

            if (this.Scene != null)
            {
                this.Scene.OnBattleStart("pve", this.CombatConfig);
            }
        }
        /// <summary>
        /// 战斗初始化(pvp战斗)
        /// </summary>
        public void InitPvpBattle()
        {
            this.RuleManager.Reset();
            //初始化战斗数据
            this.CastEffects.Clear();
            this.CastTraps.Clear();
            this.Summons.Clear();

            this.BattleEnd = false;
            if (this.Scene != null)
            {
                this.Scene.OnBattleStart("pvp", this.CombatConfig);
            }
        }

        public void Rest(bool bEnemyRest = false)
        {
            foreach (Actor role in this.GetSurvivors(bEnemyRest ? RoleSide.All : RoleSide.SideA))
            {
                float factor = 1f;
                if (this.LevelData.MpsRestraint > 0)
                {
                    factor = this.LevelData.MpsRestraint / (float)100;
                }
                role.Rest(factor);
                role.ClearAbilities();
            }
            if (!bEnemyRest)
            {
                this.Supplied = true;
            }
        }

        /// <summary>
        /// 结束战斗
        /// </summary>
        public void EndBattle(bool immediately = false)
        {
            if (!IsReplayMode)
                beginTime = EngineUtil.GetServerTime() - (long)LogicTime;

            foreach (Entity e in this.CastEffects)
            {
                e.Stop();
                e.Delete();
            }

            foreach (Entity e in this.CastTraps)
            {
                e.Delete();
                e.Stop();
            }

            this.Enabled = false;
            this.Running = false;
            this.BattleEnd = false;
            this.LevelEnded = true;
            this.isReEnter = false;
            this.IsStartLoaded = false;

            if (this.RuleManager != null)
            {
                this.RuleManager.OnBattleEnd(immediately);
            }

            foreach(Actor _actor in Roles)
            {
                _actor.Joint.Stop(0.3f);
            }

            if (this.Scene != null)
            {//通知场景结束战斗
                this.Scene.OnBattleEnd(immediately);
            }
            RuleManager = null;

            int roleNum = this.Roles.Count;
            for (int i = 0; i < roleNum; i++)
            {
                this.Roles[i].Delete();
            }
            RvoSimulator.Destroy();

            Logger.LogCombat("NeptuneBattle::EndBattle");
            if (immediately)
                Logger.Roll();
        }

        public void SendMsgFrameOp()
        {
            this.RuleManager.SendMsgFrameOp();
        }
        /// <summary>
        /// 战斗tick
        /// </summary>
        /// <param name="passTime"></param>
        public void onTimer(float passTime)
        {
            if (!this.Running)
                return;
            //先处理服务器返回的操作信息
            this.doneFrameCount++;
            //CachedLog.Log("update onTimer.next:" + doneFrameCount + ",max:" + NetworkWinManager.Instance.maxRecvFrameNo + ",time:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"));
            logicTime = this.doneFrameCount * NeptuneConst.KEY_FRAME_INTERVAL;

            this.RuleManager.Update(passTime);

            this.ProcessOP();
            this.ProcessEntities(passTime);
            //this.Simulator.Update();
            //CachedLog.Log("update onTimer End.next:" + doneFrameCount + ",max:" + NetworkWinManager.Instance.maxRecvFrameNo + ",time:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"));

            if (lastFrameTime > 0)
                realDeltaTime = Time.realtimeSinceStartup - lastFrameTime;
            lastFrameTime = Time.realtimeSinceStartup;

#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log(this.ToString());
#endif

        }

        //public void Proceed()
        //{
        //    foreach (Actor role in this.GetSurvivors(RoleSide.SideA))
        //    {
        //        if (role.Config.IsDemon)
        //            role.End(null);
        //    }
        //    if (NeptuneBattle.Instance.Scene != null)
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
        public void OnRoleDeath(Actor role, Actor attacker, Skill fromSkill = null)
        {
            NeptuneBattle.Instance.AOIManager.RemoveRole(role);
            NeptuneBattle logic = NeptuneBattle.Instance;
            logic.AliveRoles[(int)role.Side].Remove(role);
            logic.AliveRoles[(int)RoleSide.All].Remove(role);

            foreach (Actor u in logic.AliveRoles[(int)RoleSide.All])
            {
                u.OnRoleDeath(role, attacker, fromSkill);
            }

            //         if (attacker != null && attacker.IsHero && !role.Config.IsDemon)
            //         {
            //             attacker.SetMP(attacker.MP + 300);
            //             if (NeptuneBattle.Instance.Scene != null && attacker.Joint != null)
            //             {
            //                 //NeptuneBattle.Instance.Scene.PopupText(PopupType.KillingRewards, string.Empty, attacker.Joint, role.Joint, false, attacker.Side);//暂时不用
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

            if (role.AuraSkills.Count > 0)
            {
                AliveRolesInitAttributes(role.Side, RelativeSide.Friend);
            }

            if (role.EnemyAuraSkills.Count > 0)
            {
                AliveRolesInitAttributes(role.Side, RelativeSide.Enemy);
            }

            if (this.RuleManager != null)
            {
                this.RuleManager.OnRoleDeath(role, attacker);
            }
            if (this.Scene != null)
            {
                this.Scene.OnRoleDeath(role, attacker, fromSkill);
            }

        }

        public void OnRoleHit(Actor role)
        {
            if (this.RuleManager != null)
            {
                this.RuleManager.OnRoleHit(role);
            }
        }

        public void AliveRolesInitAttributes(RoleSide mySide, RelativeSide relaSide)
        {
            foreach (var element in NeptuneBattle.Instance.GetSurvivors(mySide, relaSide))
            {
                var role = (Actor)element;
                role.InitAttributes();
            }
        }


        public void GetCurrentHeroesData(List<RoleExtra> myroles, List<RoleExtra> enemies, List<RoleExtra> hires)
        {
            foreach (Actor role in this.Roles)
            {
                if (!role.Config.IsDemon)
                {
                    RoleExtra hero = new RoleExtra();
                    hero.tid = role.ID;
                    hero.HP = (int)Math.Ceiling(role.HP * NeptuneConst.PercentageRatio / role.AttributeFinalValue.MaxHP);
                    hero.MP = (int)Math.Ceiling(role.MP * NeptuneConst.PercentageRatio / role.AttributeFinalValue.MaxMP);
                    hero.Rage = (int)Math.Ceiling(role.Rage * NeptuneConst.PercentageRatio / role.AttributeFinalValue.MaxRage);
                    hero.Point = (int)Math.Ceiling(role.Point * NeptuneConst.PercentageRatio / role.AttributeFinalValue.MaxPoint);
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

        public void CastSkill(Actor role)
        {
            OpRecord record = new OpRecord()
            {
                OpCode = 0,
                Round = this.Round,
                Tick = this.doneFrameCount,
                RoleIdx = role.GlobalIdx
            };
            NeptuneBattle.Instance.OpRecords.Add(record);
        }


        public void AddEffect(Effect effect)
        {
            if (effect != null)
            {
                this.CastEffects.Add(effect);
                ++this.effectIndex;
                effect.EffectIndex = this.effectIndex;
                if (effect.Type == EffectType.Cast)
                {
                    effect.LastHitPosition = new Vector2(effect.Position.x, effect.Position.y);
                    if (this.Scene != null && NeptuneBattle.Instance.isShowEffect && this.Scene.EffecVisible(effect) && !this.IsNoRenderMode)
                    {
                        IEffectAgent actor = ObjectFactory.Create(effect, EffectType.Cast);
                        this.Scene.AddJoint(actor);
                        this.Scene.AddEffectOnMap(effect);
                    }
                }
            }
        }

        public void AddTrap(Trap trap)
        {
            if (trap != null)
            {
                this.CastTraps.Add(trap);
            }
        }
        public void TriggerTrap(Actor role, int trapid)
        {
            Trap trap = null;
            for (int i = 0; i < CastTraps.Count; i++)
            {
                trap = (CastTraps[i] as Trap);
                if (trap.trapId == trapid && trap.Skill.Caster == role)
                    trap.CastManualAction();
            }
        }
        public UVector2 HitTrap(Actor role, UVector2 curPos, UVector2 nextPos)
        {
            UVector2 hit = UVector2.zero;
            Trap trap = null;
            for (int i = 0; i < CastTraps.Count; i++)
            {
                trap = (CastTraps[i] as Trap);
                if (trap.Skill.Caster.GetRelation(role) == trap.AffectedSide)
                {
                    hit = trap.ObstacleWallHit(curPos, nextPos);
                    if (hit != UVector2.zero)
                        break;
                }
            }
            return hit;
        }

        public void RemoveTrap(Actor role, int trapid, int count = 0)
        {
            Trap trap = null;
            int remmovecount = 0;
            for (int i = 0; i < CastTraps.Count; i++)
            {
                if (count > 0 && remmovecount >= count)
                    break;
                trap = (CastTraps[i] as Trap);
                if (trap.trapId == trapid && trap.Skill.Caster == role)
                {
                    remmovecount++;
                    trap.Stop();
                }

            }
        }
        public void RemoveTrap(Trap trap)
        {
            for (int i = 0; i < CastTraps.Count; i++)
            {
                if (trap == CastTraps[i])
                    trap.Stop();
            }
        }


        public void Resume(bool force = false)
        {
            if (!NeptuneConst.BattlePauseLevelEnable || this.PauseLevel == 0 && force)
            {
                this.PauseSide = RoleSide.None;
                return;
            }

            this.PauseLevel = force ? 0 : (--this.PauseLevel);

            if (this.PauseLevel != 0)
            {
                return;
            }
            this.PauseSide = RoleSide.None;

            this.ResumeAllAnimation();

            if (this.Scene != null)
            {
                this.Scene.PlayEffect(ScreenEffect.Freeze, 0, 0f);
            }
        }

        public void SuspendAllAnimation(bool needMask = true)
        {
            foreach (Entity role in this.Roles)
            {
                role.Suspend(needMask);
            }

            foreach (Entity summ in this.Summons)
            {
                summ.Suspend(needMask);
            }

            foreach (Entity cast in this.CastEffects)
            {
                cast.Suspend(needMask);
            }

            foreach (Entity elem in this.CastTraps)
            {
                elem.Suspend(needMask);
            }
        }

        public void ResumeAllAnimation()
        {
            foreach (Entity element in this.Roles)
            {
                element.Resume();
            }
            foreach (Entity element in this.Summons)
            {
                element.Resume();
            }
            foreach (Entity element in this.CastEffects)
            {
                element.Resume();
            }
            foreach (Entity element in this.CastTraps)
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
            string s = string.Format("============================================================\r\nTick {0} : UFloat:{1} Round:{2}\r\n", this.doneFrameCount, UFloat.count, UFloat.round_count);
            UFloat.count = 0;
            UFloat.round_count = 0;
            foreach (Actor role in this.Roles)
            {
                s = s + role.ToString();
            }
            if (this.CastEffects.Count > 0)
                s += "CAST_EFFECT:\r\n";
            foreach (Entity element in this.CastEffects)
            {
                s = s + element.ToString();
            }
            if (this.CastTraps.Count > 0)
            {
                s += "Cast_Traps: \r\n";
                foreach (Entity elem in this.CastTraps)
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

        public static IAgent PlayEffect(string effectRes, Vector2 pos, Vector2 direction, float height, int z, Effect effect = null, Action<IEffectController> onlaod = null, Entity element = null)
        {
            if (NeptuneBattle.Instance.Scene != null && NeptuneBattle.Instance.isShowEffect && !NeptuneBattle.Instance.IsNoRenderMode)
            {
                IAgent joint = ObjectFactory.Create(effectRes, pos, direction, height, z, effect, onlaod, element);
                NeptuneBattle.Instance.Scene.AddJoint(joint);

                return joint;
            }
            return null;
        }
        public static IAgent PlayEffect(int effect_id, Vector2 pos, Vector2 scale, float height, int z)
        {
            if (NeptuneBattle.Instance.IsNoRenderMode) return null;
            string effect = NeptuneBattle.Instance.DataProvider.GetEffects(effect_id);
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
            LevelData data = NeptuneBattle.Instance.DataProvider.GetLevelData(stageId);
            if (data == null)
                return 0;
            int chapterId = data.SectionID;

            Dictionary<int, CombatConfigData> dict = NeptuneBattle.Instance.DataProvider.GetCombatConfigDatas(stageId);
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
                if (Array.Find(battleData.BossPosition, delegate (int ID) { return ID == 1; }) == 1)
                {
                    return battleData.Monster1ID;
                }
                if (Array.Find(battleData.BossPosition, delegate (int ID) { return ID == 2; }) == 2)
                {
                    return battleData.Monster2ID;
                }
                if (Array.Find(battleData.BossPosition, delegate (int ID) { return ID == 3; }) == 3)
                {
                    return battleData.Monster3ID;
                }
                if (Array.Find(battleData.BossPosition, delegate (int ID) { return ID == 4; }) == 4)
                {
                    return battleData.Monster4ID;
                }
                if (Array.Find(battleData.BossPosition, delegate (int ID) { return ID == 5; }) == 5)
                {
                    return battleData.Monster5ID;
                }
            }

            return 0;
        }
    }
}