using OpenNGS.Levels.Data;
using OpenNGS.Rank.Data;
using OpenNGS.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Systems;

public class LevelProcessSystem : GameSubSystem<LevelProcessSystem>, ILevelProcessSystem
{
    private List<ILevelStage> levelStages;
    
    private bool m_bNextStage = false;
    private bool m_bStart = false;

    protected override void OnCreate()
    {
        base.OnCreate();
        levelStages = new List<ILevelStage>();
    }

    public override string GetSystemName()
    {
        return "com.openngs.system.LevelProcessSystem";
    }

    public void InitLevelStage(ILevelStageFactor factor)
    {
        levelStages = factor.GetLevelStage();
        //StartBegin();
    }

    public void StartBegin()
    {
        if (levelStages == null || levelStages.Count == 0) return;
        currentStage = levelStages[currentStageIndex];
        m_bStart = true;
    }

    private ILevelStage currentStage;
    private int currentStageIndex;
    public void UpdateStages(float deltaTime)
    {
        if (!m_bStart) return;
        if (levelStages == null || levelStages.Count == 0) return;

        if (currentStage.GetBeginStageExecute())
        {
            currentStage.OnStageBegin();
            return;
        }
        else if (currentStage.GetUpdateStageExecute())
        {
            currentStage.OnStageUpdate(deltaTime);
            return;
        }
        else if(currentStage.GetEndStageExecute())
        {
            currentStage.OnStageEnd();
            return;
        }
        else
        {
            currentStageIndex++;
            if(currentStageIndex < levelStages.Count)
            {
                currentStage = levelStages[currentStageIndex];
            }
        }
    }
}