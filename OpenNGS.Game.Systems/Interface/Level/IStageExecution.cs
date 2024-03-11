using System.Collections;
using System.Collections.Generic;


public interface IStageExecution
{
    // private
    public bool IsExecutionValid();

    public void Execution();

    public bool StageUpdate(float deltaTime);
}
