using OpenNGS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelTest : MonoBehaviour
{
    public int levelID = 0;
    public int levelTime = 5;
    private LevelSystem levelSystem;


    //我没有考虑一个关卡有很多LevelStage的情况，在LevelSystem初始化了一个LevelStage，如果有很多LevelStage可以在LevelSystem进行跳转
    private void Start()
    {
        levelSystem = new LevelSystem();
        levelSystem.CurLevel(levelID);
        levelSystem.SetTimer(levelTime);        
        levelSystem.InitStages();

        LevelStageProcess stageProcess = (LevelStageProcess)levelSystem.lstStages[0].lstStages[1];
        PlayerDieCondition playerDieCondition = new PlayerDieCondition();
        StageExecution updateExecution = new StageExecution();
        updateExecution.AddCondition(playerDieCondition);
        stageProcess.lstUpdateExecution.Add(updateExecution);
        levelSystem.lstStages[0].StartBegin();

    }
    private void Update()
    {
        if (levelSystem.lstStages[0].currentStageIndex < 3)
        {
            levelSystem.UpdateStages(UnityEngine.Time.deltaTime);
        }
        else
        {
            Debug.Log("关卡结束，跳转主城");
        }

    }
}