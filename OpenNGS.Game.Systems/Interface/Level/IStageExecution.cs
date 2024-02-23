using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStageExecution
{
    // private
    public bool IsExecutionValid();

    public void Execution();

    public bool StageUpdate(float deltaTime);
}
