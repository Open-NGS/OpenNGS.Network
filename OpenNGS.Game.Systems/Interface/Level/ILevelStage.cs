using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ILevelStage
{
    public void Init(List<StageExecution> LstBeginExecution, List<StageExecution> LstUpdateExecution,List<StageExecution> LstEndExecution);
    public void OnStageBegin();
    public bool OnStageUpdate(float deltaTime);
    public void OnStageEnd();
}

