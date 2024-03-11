using OpenNGS.Core;

public interface IEnemySpawner
{
    public void SpawnEnemy(uint enemyID, NGSVector3 pos);
}
public interface IEnemySpawnSystem
{
    //游戏进行时间
    public float m_currentTime {  get; set; }
    //进入场景初始化关卡敌人信息
    public void EnterLevel(uint LevelID, IEnemySpawner _spawner);
    //根据游戏进行时间生成对应敌人
    public void InitEnemyByTime();
    //敌人销毁 并根据销毁量生成对应数量
    public void EnemyDestory(uint enemyID, uint num);

}
