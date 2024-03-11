using System.Collections;
using System.Collections.Generic;


public class EnemyKilledCondition : ICondition
{
    public bool IsConditionValid()
    {
        return false;
        // 检查敌人是否全部被击败
        // 如果是，返回 true，否则返回 false
    }
}
