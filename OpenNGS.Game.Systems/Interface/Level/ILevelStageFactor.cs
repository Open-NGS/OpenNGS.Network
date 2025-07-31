using System.Collections;
using System.Collections.Generic;

public interface ILevelStageFactor
{
    public void InitLevelStageFactor(uint LevelID);
    public List<ILevelStage> GetLevelStage();
}
