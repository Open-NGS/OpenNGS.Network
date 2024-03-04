using JetBrains.Annotations;
using OpenNGS;
using OpenNGS.Enemy.Data;
using OpenNGS.Levels.Data;
using OpenNGS.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using Systems;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawnSystem<T> : GameSubSystem<EnemySpawnSystem<T>>, IEnemySpawnSystem<T>
{
    List<LevelEnemyInfo> _levelEnemyInfos;//读取对应关卡敌人
    List<LevelEnemyInfo> _normalEnemyInfos= new List<LevelEnemyInfo>();//对应关卡普通敌人列表
    List<LevelEnemyInfo> _eliteEnemyInfos=new List<LevelEnemyInfo>();//对应关卡精英敌人列表
    /// <summary>
    /// 已生成的敌人字典 uint为敌人ID，int为敌人数量
    /// </summary>
    Dictionary<uint,int> GeneratedEnemies = new Dictionary<uint,int>();
    private IEnemySpawner<T> m_spawner;
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    public void InitEnemyAtBegin(uint LevelID, IEnemySpawner<T> _spawner)
    {
        m_spawner = _spawner;
        _levelEnemyInfos = NGSStaticData.levelEnemyInfos.GetItems(LevelID);
        ClasifyEnemyType();
        InitAtGameBgein();
    }
    //对敌人类型分类
    public void ClasifyEnemyType()
    {
        if (_levelEnemyInfos.Count > 0)
        {
            for (int i = 0; i < _levelEnemyInfos.Count; i++)
            {
                if (_levelEnemyInfos[i].EnemyInitType == OpenNGS.Levels.Common.ENEMY_INITTYPE.ENEMY_INITTYPE_ROUND)
                {
                    _normalEnemyInfos.Add(_levelEnemyInfos[i]);
                }
                if(_levelEnemyInfos[i].EnemyInitType == OpenNGS.Levels.Common.ENEMY_INITTYPE.ENEMY_INITTYPE_AREA)
                {
                    _eliteEnemyInfos.Add(_levelEnemyInfos[i]);
                }
            }
        }
    }
    //游戏开始时生成敌人
    public void InitAtGameBgein()
    {
        for (int i=0;i< _normalEnemyInfos.Count; i++)
        {
            if (_normalEnemyInfos[i].IsActiveAtBegin == true)
            {
                SpawnNormalEnemies(_normalEnemyInfos[i], (int)_normalEnemyInfos[i].EnemyNum);
                GeneratedEnemies.Add(_normalEnemyInfos[i].EnemyID, (int)_normalEnemyInfos[i].EnemyNum);
            }
        }
        for (int i = 0; i < _eliteEnemyInfos.Count; i++)
        {
            if (_eliteEnemyInfos[i].IsActiveAtBegin == true)
            {
                SpawnEliteEnemies(_eliteEnemyInfos[i], (int)_eliteEnemyInfos[i].EnemyNum);
                GeneratedEnemies.Add(_eliteEnemyInfos[i].EnemyID, (int)_eliteEnemyInfos[i].EnemyNum);
            }
        }
    }

    /// <summary>
    /// 生成普通敌人
    /// </summary>
    /// <param name="enemyInfo">LevelEnemyInfo</param>
    /// <param name="intNum">生成数量</param>
    public void SpawnNormalEnemies(LevelEnemyInfo enemyInfo,int intNum)
    {
        float lastX = 0;
        for(int i = 0; i < intNum; i++)
        {
            GameObject playerPrefab = GameObject.FindGameObjectWithTag("Player");
            Vector3 SpawnPosition = playerPrefab.transform.position;
            float spawnPosX = UnityEngine.Random.Range(-enemyInfo.MinDistance, enemyInfo.MinDistance);
            float spawnPosZ;
            
            if (spawnPosX < lastX - 1 || spawnPosX > lastX + 1)
            {
                if (JudgePosOrNeg())
                {
                    spawnPosZ = Mathf.Sqrt(Mathf.Pow(enemyInfo.MinDistance, 2) - Mathf.Pow(spawnPosX, 2));
                }
                else
                {
                    spawnPosZ = -Mathf.Sqrt(Mathf.Pow(enemyInfo.MinDistance, 2) - Mathf.Pow(spawnPosX, 2));
                }
                SpawnPosition = new Vector3(playerPrefab.transform.position.x + spawnPosX,
                    playerPrefab.transform.position.y, playerPrefab.transform.position.z + spawnPosZ);
                //先生成一个cube
                //Instantiate(normalEnemy, SpawnPosition, Quaternion.identity);
                m_spawner.SpawnEnemy(enemyInfo.EnemyID, SpawnPosition);
                lastX = spawnPosX;
            }
            else
            {
                lastX = spawnPosX;
                i--;
                continue;
            }

        }
    }
    /// <summary>
    /// 生成精英敌人
    /// </summary>
    /// <param name="enemyInfo">LevelEnemyInfo</param>
    /// <param name="intNum">生成数量</param>
    public void SpawnEliteEnemies(LevelEnemyInfo enemyInfo, int intNum)
    {
        for (int i = 0; i < intNum; i++)
        {
            Vector3 SpawnPosition = new Vector3((float)enemyInfo.AreaPos[0], 
                (float)enemyInfo.AreaPos[1], (float)enemyInfo.AreaPos[2]);
            //Instantiate(eliteEnemy, SpawnPosition, Quaternion.identity);
            m_spawner.SpawnEnemy(enemyInfo.EnemyID, SpawnPosition);
        }
    }
    //随机正负
    public bool JudgePosOrNeg()
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
    public void EnemyDestory(uint enemyID,int num)
    {
        if(GeneratedEnemies.ContainsKey(enemyID))
        {
            GeneratedEnemies[enemyID] = GeneratedEnemies[enemyID] - num;
        }
    }
    //判断场景内普通敌人数量并生成
    public void JudgeNormalEnemyNum(uint levelID,uint enemyID)
    {
        LevelEnemyInfo info = NGSStaticData.levelEnemyInfo.GetItem(levelID,enemyID);
        if (GeneratedEnemies.ContainsKey(enemyID))
        {
            if(info.EnemyNum > GeneratedEnemies[enemyID])
            {
                SpawnNormalEnemies(info, (int)info.EnemyNum - (int)GeneratedEnemies[enemyID]);
                GeneratedEnemies[enemyID] = (int)info.EnemyNum;
            }
        }   
    }
    //判断场景内精英敌人数量并生成
    public void JudgeEliteEnemyNum(uint levelID,uint enemyID)
    {
        LevelEnemyInfo info = NGSStaticData.levelEnemyInfo.GetItem(levelID, enemyID);
        if (!GeneratedEnemies.ContainsKey(enemyID))
        {
            SpawnEliteEnemies(info, (int)info.EnemyNum);
            GeneratedEnemies[enemyID] = (int)info.EnemyNum;
        }
    }
    public override string GetSystemName()
    {
        return "com.openngs.system.EnemySpawnSystem";
    }
    
    //public void InitEnemyAtBegin(uint LevelID, IEnemySpawner _spawner)
    //{
    //    m_spawner = _spawner;
    //}


}
