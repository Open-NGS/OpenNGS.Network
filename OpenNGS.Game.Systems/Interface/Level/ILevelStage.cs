using OpenNGS.Levels.Common;
using System.Collections;
using System.Collections.Generic;

public interface ILevelStage
{
    public void AddExecutions(STAGE_EXECUTION_TYPE type, List<StageExecution> executions);
    public void OnStageBegin();
    public bool GetBeginStageExecute();
    public void OnStageUpdate(float deltaTime);
    public bool GetUpdateStageExecute();
    public void OnStageEnd();
    public bool GetEndStageExecute();

}

