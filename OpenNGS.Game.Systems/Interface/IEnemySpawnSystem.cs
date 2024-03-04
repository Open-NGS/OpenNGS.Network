using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemySpawner<T>
{
    public void SpawnEnemy(uint enemyID, Vector3 pos);
}
public interface IEnemySpawnSystem<T>
{
    //游戏开始时生成敌人
    public void InitEnemyAtBegin(uint LevelID, IEnemySpawner<T> _spawner);
    //敌人销毁/生成
    public void EnemyDestory(uint enemyID, int num);
    //判断场景内普通敌人数量并生成
    public void JudgeNormalEnemyNum(uint levelID, uint enemyID);
}
