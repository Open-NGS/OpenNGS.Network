using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelStageEnd : ILevelStage
{
    public List<StageExecution> lstBeginExecution;
    public List<StageExecution> lstUpdateExecution;
    public List<StageExecution> lstEndExecution;

    public void Init(List<StageExecution> LstBeginExecution, List<StageExecution> LstUpdateExecution, List<StageExecution> LstEndExecution)
    {
        lstBeginExecution = LstBeginExecution;
        lstUpdateExecution = LstUpdateExecution;
        lstEndExecution = LstEndExecution;
    }
    public void OnStageBegin()
    {
        Debug.Log("结束阶段的开始状态");
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
        Debug.Log("结束阶段的过程状态");
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
        Debug.Log("结束阶段的结束状态");
        foreach (var _stageExecution in lstEndExecution)
        {
            if (_stageExecution.IsExecutionValid())
            {
                _stageExecution.Execution();
            }
        }
    }

}

//public class LevelStageEnd : LevelStage
//{
//    public LevelStageEnd(int id, int time) : base(id, time)
//    {
//        //LstBeginExecution = new List<StageExecution>();
//        //LstUpdateExecution = new List<StageExecution>();
//        //LstEndExecution = new List<StageExecution>();
//    }
//    public override void OnStageBegin()
//    {
//        Debug.Log("结束阶段的开始状态");
//        base.OnStageBegin();
//    }

//    public override bool OnStageUpdate(float deltaTime)
//    {
//        Debug.Log("结束阶段的过程状态");
//        return base.OnStageUpdate(deltaTime);

//    }

//    public override void OnStageEnd()
//    {
//        Debug.Log("结束阶段的结束状态");
//        LstUpdateExecution.Clear();
//        base.OnStageEnd();
//    }
//}
