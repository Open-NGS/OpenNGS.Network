using OpenNGS.Rank.Data;
using OpenNGS.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Systems;

public class LevelProcessSystem : GameSubSystem<LevelProcessSystem>, ILevelProcessSystem
{
    public int levelId;
    public int levelTime;
    private bool m_bNextStage = false;
    public List<LevelStage> lstStages;
    public int currentStageIndex = 0;

    protected override void OnCreate()
    {
        base.OnCreate();
        InitStages();
    }

    public override string GetSystemName()
    {
        return "com.openngs.system.LevelProcessSystem";
    }

    public void SetLevel(int levelid)
    {
        levelId = (int)NGSStaticData.levelData.GetItem(levelid).LevelID;
        levelTime = (int)NGSStaticData.levelData.GetItem(levelid).CompletionTime;
    }



    public void InitStages()
    {
        lstStages = new List<LevelStage>();        
        LevelStage _stageBegin = new LevelStage(levelId, levelTime);
        lstStages.Add(_stageBegin);
        _stageBegin.InitStages();

    }

    public void UpdateStages(float deltaTime)
    {
        lstStages[0].UpdateStages(deltaTime);
    }

}