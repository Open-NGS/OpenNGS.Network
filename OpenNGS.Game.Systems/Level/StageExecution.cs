using System.Collections;
using System.Collections.Generic;


public class StageExecution
{
    public List<ICondition> LstCondition = new List<ICondition>();
    public StageExecution()
    {
        LstCondition = new List<ICondition>();
    }

    public void AddCondition(ICondition condition)
    {
        LstCondition.Add(condition);
    }

    public bool IsExecutionValid()
    {
        foreach (var condition in LstCondition)
        {
            if (!condition.IsConditionValid())
            {
                return false;
            }
        }
        return true;
    }

    public virtual void Execution()
    {
        ////Debug.Log("Executing stage...");
    }

    public bool StageUpdate(float timer)
    {
        bool bRes = false;
        if (LstCondition.Count == 0)
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

    public void ExecuteSpecificExecution<T>() where T : StageExecution, new()
    {
        T specificExecution = new T();
        specificExecution.Execution();
    }
}