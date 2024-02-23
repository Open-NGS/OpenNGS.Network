using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelStageBegin : ILevelStage
{
    public List<StageExecution> lstBeginExecution ;
    public List<StageExecution> lstUpdateExecution ;
    public List<StageExecution> lstEndExecution ;

    public void Init(List<StageExecution> LstBeginExecution, List<StageExecution> LstUpdateExecution, List<StageExecution> LstEndExecution)
    {
        lstBeginExecution=LstBeginExecution;
        lstUpdateExecution=LstUpdateExecution;
        lstEndExecution=LstEndExecution;
    }

    public void OnStageBegin()
    {
        Debug.Log("开始阶段的开始状态");
        foreach (var _stageExecution in lstBeginExecution)
        {
            if (_stageExecution.IsExecutionValid())
            {
                _stageExecution.Execution();
            }
        }
    }



    public bool OnStageUpdate(float deltaTime)
    {
        Debug.Log("开始阶段的过程状态");
        bool bRes = false;
        if (lstUpdateExecution.Count == 0 || lstUpdateExecution == null)
        {
            return true;  // 如果没有执行块，直接返回 true 进入下一个阶段
        }
        foreach (var _updateExecution in lstUpdateExecution)
        {
            bRes = _updateExecution.StageUpdate(deltaTime);
            return bRes;
        }
        return bRes;
    }
    public void OnStageEnd()
    {
        Debug.Log("开始阶段的结束状态");
        foreach (var _stageExecution in lstEndExecution)
        {
            if (_stageExecution.IsExecutionValid())
            {
                _stageExecution.Execution();
            }
        }
    }

}
