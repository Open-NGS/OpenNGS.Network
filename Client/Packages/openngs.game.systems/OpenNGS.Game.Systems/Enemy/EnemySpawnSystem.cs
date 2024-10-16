using OpenNGS;
using OpenNGS.Core;
using OpenNGS.Enemy.Data;
using OpenNGS.Levels.Data;
using OpenNGS.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using Systems;

public class EnemySpawnSystem : GameSubSystem<EnemySpawnSystem>, IEnemySpawnSystem
{
    uint initIndex = 0;//生成顺序
    bool isInit = true;
    uint m_levelID { get; set; }//关卡ID
    uint m_enemyRuleID { get; set; }//规则ID
    float m_currentTime { get ; set ; }//当前游戏进行时间

    LevelEnemyInfo currentInitEnemy;//当前生成敌人

    List<LevelEnemyInfo> _levelEnemyInfos;//读取对应关卡所有敌人
    /// <summary>
    /// 当前关卡已生成的敌人字典 敌人ID，敌人数量
    /// </summary>
    Dictionary<uint,uint> GeneratedEnemies = new Dictionary<uint,uint>();
    /// <summary>
    /// 开始顺序 规则ID
    /// </summary>
    Dictionary<uint, uint> levelInfoTimes = new Dictionary<uint, uint>();
    private IEnemySpawner m_spawner;
    protected override void OnCreate()
    {
        base.OnCreate();
        
    }
    
    //初始化关卡信息
    public void EnterLevel(uint LevelID, IEnemySpawner _spawner)
    {
        m_spawner = _spawner;
        m_levelID= LevelID;
        _levelEnemyInfos=NGSStaticData.levelEnemyInfos.GetItems(m_levelID);
        SortEnemyListByTime();
    }
    /// <summary>
    /// 计时器
    /// </summary>
    /// <param name="intervalTime">间隔时间</param>
    public  void Timer(float intervalTime)
    {
        m_currentTime += intervalTime;
        InitEnemyByTime();
    }
    //对当前敌人信息列表按生成开始时间进行排序并存入字典
    private void SortEnemyListByTime()
    {
        List<LevelEnemyInfo> sortedList=_levelEnemyInfos.OrderBy(p=>p.InitBeginTime).ToList();
        int nIdx = 0;
        foreach(LevelEnemyInfo _levelEnemyInf in sortedList)
        {
            levelInfoTimes.Add((uint)nIdx, _levelEnemyInf.RuleID);
            nIdx++;
        }
    }
    //根据时间生成敌人
    public void InitEnemyByTime()
    {
        currentInitEnemy = NGSStaticData.levelEnemyInfo.GetItem(m_levelID, levelInfoTimes[initIndex]);
        if (m_currentTime >= currentInitEnemy.InitBeginTime)    
        {
            if (isInit)
            {
                CreateEnemy();//生成
            }
        }
        if(m_currentTime >= currentInitEnemy.InitFinishTime)
        {
            initIndex++;
            isInit = true;
        }
    }

    private void CreateEnemy()
    {
        isInit = false;
        uint num = currentInitEnemy.EnemyNum;
        if (GeneratedEnemies.ContainsKey(currentInitEnemy.EnemyID))
        {
            if (GeneratedEnemies[currentInitEnemy.EnemyID] < currentInitEnemy.EnemyNum)
            {
                AddEnemy(currentInitEnemy.EnemyID, num - GeneratedEnemies[currentInitEnemy.EnemyID]);
            }
        }
        else
        {
            AddEnemy(currentInitEnemy.EnemyID, num);
        }
        
    }

    private void AddEnemy(uint enemyID, uint initNum)
    {
        if(currentInitEnemy.EnemyInitType== OpenNGS.Levels.Common.ENEMY_INITTYPE.ENEMY_INITTYPE_ROUND)
        {
            SpawnNormalEnemies(currentInitEnemy, initNum);
        }
        else if(currentInitEnemy.EnemyInitType == OpenNGS.Levels.Common.ENEMY_INITTYPE.ENEMY_INITTYPE_AREA)
        {
            SpawnEliteEnemies(currentInitEnemy, initNum);
        }
        GeneratedEnemies[enemyID] = initNum;
    }

    


    /// <summary>
    /// 生成普通敌人
    /// </summary>
    /// <param name="enemyInfo">LevelEnemyInfo</param>
    /// <param name="initNum">生成数量</param>
    private void SpawnNormalEnemies(LevelEnemyInfo enemyInfo,uint initNum)
    {
        if (initNum == 0)
        {
            return;
        }
        for(int i = 0; i < initNum; i++)
        {
            NGSVector3 SpawnPosition = new NGSVector3();
            Random random = new Random();
            float spawnPosX = random.Next((int)-enemyInfo.MaxDistance, (int)enemyInfo.MaxDistance);
            float spawnPosZ = random.Next((int)-enemyInfo.MaxDistance, (int)enemyInfo.MaxDistance);
            float squaresSum = (float)Math.Pow(spawnPosX, 2) + (float)Math.Pow(spawnPosZ, 2);
            if (Math.Pow(enemyInfo.MinDistance, 2)<squaresSum && squaresSum< Math.Pow(enemyInfo.MaxDistance, 2))
            {
                SpawnPosition.X = spawnPosX;
                SpawnPosition.Y = 0;
                SpawnPosition.Z = spawnPosZ;
                NGSVector3 _dir = new NGSVector3();
                m_spawner.SpawnEnemy(enemyInfo.EnemyID, SpawnPosition, _dir);
            }
            else
            {
                i--;continue;
            }
        }
    }
    /// <summary>
    /// 生成精英敌人
    /// </summary>
    /// <param name="enemyInfo">LevelEnemyInfo</param>
    /// <param name="intNum">生成数量</param>
    private void SpawnEliteEnemies(LevelEnemyInfo enemyInfo, uint intNum)
    {
        for (int i = 0; i < intNum; i++)
        {
            NGSVector3 SpawnPosition = new NGSVector3();
            SpawnPosition.X = enemyInfo.AreaPos[0];
            SpawnPosition.Y = enemyInfo.AreaPos[1];
            SpawnPosition.Z = enemyInfo.AreaPos[2];
            NGSVector3 _dir = new NGSVector3();
            m_spawner.SpawnEnemy(enemyInfo.EnemyID, SpawnPosition, _dir);
        }
    }
    //随机正负
    private bool JudgePosOrNeg()
    {
        int value = UnityEngine.Random.Range(0, 2);
        if (value == 0) { return false; }
        else { return true; }
    }

    /// <summary>
    /// 敌人死亡或销毁
    /// </summary>
    /// <param name="enemyID">敌人ID</param>
    /// <param name="num">敌人死亡/销毁数量</param>
    public void EnemyDestory(uint enemyID,uint num)
    {
        if(GeneratedEnemies.ContainsKey(enemyID))
        {
            GeneratedEnemies[enemyID] = GeneratedEnemies[enemyID] - num;
            JudgeTimeAndReturnEnemyNum(enemyID);
            LevelEnemyInfo info = NGSStaticData.levelEnemyInfo.GetItem(m_levelID, m_enemyRuleID);
            //是否为普通敌人
            if (info.EnemyInitType == OpenNGS.Levels.Common.ENEMY_INITTYPE.ENEMY_INITTYPE_ROUND)
            {
                JudgeNormalEnemyNum(info);
            }
            //是否为精英敌人
            else if (info.EnemyInitType == OpenNGS.Levels.Common.ENEMY_INITTYPE.ENEMY_INITTYPE_ROUND)
            {
                JudgeEliteEnemyNum(info);
            }
        }
        else { return; }
        
    }
    //判断场景内普通敌人数量并生成
    private void JudgeNormalEnemyNum(LevelEnemyInfo info)
    {
        if (GeneratedEnemies.ContainsKey(info.EnemyID))
        {
            if(info.EnemyNum > GeneratedEnemies[info.EnemyID])
            {
                SpawnNormalEnemies(info, info.EnemyNum - GeneratedEnemies[info.EnemyID]);
                GeneratedEnemies[info.EnemyID] = info.EnemyNum;
            }
        }   
    }
    //判断场景内精英敌人数量并生成
    private void JudgeEliteEnemyNum(LevelEnemyInfo info)
    {
        
        if (GeneratedEnemies[info.EnemyID]==0)
        {
            SpawnEliteEnemies(info, (info.EnemyNum));
            GeneratedEnemies[info.EnemyID] = info.EnemyNum;
        }
    }

    //根据传入时间返回对应规则ID
    private void JudgeTimeAndReturnEnemyNum(uint enemyID)
    {
        List<uint> lstrule = GetRuleID(enemyID);
        foreach (var _ruleID in lstrule)
        {
            //根据场景ID,规则id 找对应LevelEnemyInfo
            LevelEnemyInfo info = NGSStaticData.levelEnemyInfo.GetItem(m_levelID, _ruleID);
            if (info.InitBeginTime <= m_currentTime && m_currentTime < info.InitFinishTime)
            {
                m_enemyRuleID= _ruleID; break;
            }

        }
    }
    //根据敌人ID获取规则ID列表
    private List<uint> GetRuleID(uint enemyID)
    {
        List<uint> enemyInfosRuleIDs = new List<uint>();
        foreach (var info in _levelEnemyInfos)
        {
            if (info.EnemyID == enemyID)
            {
                enemyInfosRuleIDs.Add(info.RuleID);
            }
        }
        return enemyInfosRuleIDs;
    }
    public override string GetSystemName()
    {
        return "com.openngs.system.EnemySpawnSystem";
    }
    



}
