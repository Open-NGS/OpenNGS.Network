using System.Collections;
using System.Collections.Generic;


public class PlayerDieCondition : ICondition
{
    public virtual bool IsConditionValid()
    {
        return true;
        // 检查场景中是否存在标签为 "Player" 的对象
        //GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        //return playerObject == null; // 如果玩家对象不存在，则返回 true，表示玩家已经死亡
    }
}
