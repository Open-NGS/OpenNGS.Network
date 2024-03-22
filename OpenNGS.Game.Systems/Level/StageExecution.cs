using OpenNGS.Levels.Common;
using System.Collections;
using System.Collections.Generic;


public class StageExecution
{
    private List<ICondition> LstCondition;

    public StageExecution()
    {
        LstCondition = new List<ICondition>();
    }


    public virtual void InitExecute(uint LevelID){}

    public void AddCondition(List<ICondition> conditions)
    {
        LstCondition = conditions;
    }

    public bool IsExecutionValid()
    {
        if(LstCondition == null || LstCondition.Count == 0) return true;
        bool bRes = false;
        foreach (var condition in LstCondition)
        {
            bRes = bRes || condition.IsConditionValid();
        }
        return bRes;
    }

    public virtual void Execution()
    {
        ////Debug.Log("Executing stage...");
    }

    public bool StageUpdate(float timer)
    {
        bool bRes = false;
        if (LstCondition == null || LstCondition.Count == 0)
        {
            return true;  // 如果没有执行块，直接返回 true 进入下一个阶段
        }
        foreach (var condition in LstCondition)
        {
            bRes = condition.IsConditionValid();
            if (bRes)
            {
                Execution();
                break;
            }
        }
        return bRes;
    }
}