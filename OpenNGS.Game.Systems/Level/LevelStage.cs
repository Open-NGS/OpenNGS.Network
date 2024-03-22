using OpenNGS.Levels.Common;
using OpenNGS.Systems;
using System;
using System.Collections.Generic;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelStage : ILevelStage
{
    public List<StageExecution> lstBeginExecution;
    public List<StageExecution> lstUpdateExecution;
    public List<StageExecution> lstEndExecution;

    private int curBeginExcIdx;
    private int curEndExcIdx;

    public LevelStage()
    {
        lstBeginExecution = new List<StageExecution>();
        lstUpdateExecution = new List<StageExecution>();
        lstEndExecution = new List<StageExecution>();
        curBeginExcIdx = 0;
        curEndExcIdx = 0;
    }

    public void AddExecutions(STAGE_EXECUTION_TYPE type, List<StageExecution> executions)
    {
        switch (type)
        {
            case STAGE_EXECUTION_TYPE.STAGE_EXECUTION_TYPE_BEGIN:
                lstBeginExecution = executions;
                break;
            case STAGE_EXECUTION_TYPE.STAGE_EXECUTION_TYPE_PROCESS:
                lstUpdateExecution = executions;
                break;
            case STAGE_EXECUTION_TYPE.STAGE_EXECUTION_TYPE_END:
                lstEndExecution = executions;
                break;
        }
    }

    public void OnStageBegin()
    {
        if (lstBeginExecution == null || lstBeginExecution.Count == 0) return;
        if (curEndExcIdx == lstEndExecution.Count) return;
        //Debug.Log("开始阶段的开始状态");
        if (lstBeginExecution[curBeginExcIdx].IsExecutionValid())
        {
            lstBeginExecution[curBeginExcIdx].Execution();
            curBeginExcIdx++;
        }
    }

    public bool GetBeginStageExecute()
    {
        if (lstBeginExecution == null || lstBeginExecution.Count == 0) return true;
        return curBeginExcIdx < lstBeginExecution.Count;
    }

    private bool bRes;
    public void  OnStageUpdate(float deltaTime)
    {
        foreach (var _updateExecution in lstUpdateExecution)
        {
            bRes = bRes || _updateExecution.StageUpdate(deltaTime);
        }
    }

    public bool GetUpdateStageExecute()
    {
        if (lstUpdateExecution.Count == 0 || lstUpdateExecution == null) return true;

        return bRes == false;
    }

    public void OnStageEnd()
    {
        if (lstEndExecution == null || lstEndExecution.Count == 0) return;
        if (curEndExcIdx == lstEndExecution.Count) return;
        //Debug.Log("开始阶段的开始状态");
        if (lstEndExecution[curEndExcIdx].IsExecutionValid())
        {
            lstEndExecution[curEndExcIdx].Execution();
            curEndExcIdx++;
        }
    }

    public bool GetEndStageExecute()
    {
        if (lstEndExecution == null || lstEndExecution.Count == 0) return true;
        return curEndExcIdx < lstEndExecution.Count;
    }
}
