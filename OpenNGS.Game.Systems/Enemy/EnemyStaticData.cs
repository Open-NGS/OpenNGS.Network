using System.Collections;
using System.Collections.Generic;

namespace OpenNGS.Systems
{
    public static class EnemyStaticData
    {
        public static Table<OpenNGS.Enemy.Data.EnemyInfo, uint> enemyInfo = new Table<Enemy.Data.EnemyInfo, uint>((item) => { return item.ID; }, false);

        public static void Init() { }
    }
}
