using System.Collections;
using System.Collections.Generic;



public interface ILevelStage
{
    public void Init(int levelId);
    public void OnStageBegin();
    public bool OnStageUpdate(float deltaTime);
    public void OnStageEnd();
}

