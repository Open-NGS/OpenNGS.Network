using OpenNGS.Systems;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.SceneManagement;
using static UnityEngine.Networking.UnityWebRequest;

public class LevelStage 
{
    protected int levelId;
    protected int levelTime;
    protected LevelProcessSystem levelProcessSystem;

    //public List<StageExecution> LstBeginExecution = null;
    //public List<StageExecution> LstUpdateExecution = null;
    //public List<StageExecution> LstEndExecution = null;


    public List<ILevelStage> lstStages = new List<ILevelStage>();
    public int currentStageIndex = 0;
    private bool m_bNextStage = false;
    public LevelStage(int id, int time)
    {
        levelId = id;
        levelTime = time;

    }

    public void InitStages()
    {
        // 创建关卡并添加到列表中
        LevelStageBegin _stageBegin = new LevelStageBegin();
        _stageBegin.Init(levelId);
        lstStages.Add(_stageBegin);
        LevelStageProcess _stageProcess = new LevelStageProcess();
        _stageProcess.Init(levelId);
        lstStages.Add(_stageProcess);
        LevelStageEnd _stageEnd = new LevelStageEnd();
        _stageEnd.Init(levelId);
        lstStages.Add(_stageEnd);
    }
    public void StartBegin()
    {
        lstStages[currentStageIndex].OnStageBegin();
    }

    public void UpdateStages(float deltaTime)
    {
        if (m_bNextStage)
        {
            NextStage();
            m_bNextStage = false;
        }

        if (currentStageIndex < lstStages.Count)
        {
            //lstStages[currentStageIndex].OnStageUpdate(deltaTime);
            if (lstStages[currentStageIndex].OnStageUpdate(deltaTime))
            {
                m_bNextStage = true;
            }
            if (currentStageIndex == lstStages.Count - 1)
            {
                lstStages[currentStageIndex].OnStageEnd();
                m_bNextStage = false;
                currentStageIndex++;
            }
        }
    }

    // 进入下一个关卡阶段
    public void NextStage()
    {
        if (currentStageIndex < lstStages.Count - 1)
        {
            lstStages[currentStageIndex].OnStageEnd(); // 先执行当前阶段的结束逻辑
            currentStageIndex++;
            lstStages[currentStageIndex].OnStageBegin(); // 进入下一个阶段的开始逻辑
        }
    }
}