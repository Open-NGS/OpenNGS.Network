using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelStageFactor
{
    public void InitLevelStageFactor(uint LevelID);
    public List<ILevelStage> GetLevelStage();
}
