using OpenNGS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelTest : MonoBehaviour
{
    /*
     * 测试脚本
     */

    //public int levelID = 0;
    //public LevelProcessSystem levelProcessSystem;

    //private StartCondition startCondition;
    //private EndCondition endCondition;
    //private void Start()
    //{
    //    levelProcessSystem = new LevelProcessSystem();
    //    levelProcessSystem.SetLevel(levelID);
    //    levelProcessSystem.InitStages();

    //    LevelStageBegin levelStageBegin = levelProcessSystem.lstStages[0].lstStages[0] as LevelStageBegin;
    //    OpenStartUIExecution openStartUIExecution = new OpenStartUIExecution();
    //    levelStageBegin.lstBeginExecution.Add(openStartUIExecution);
    //    CloseStartUIExecution closeStartUIExecution = new CloseStartUIExecution();
    //    levelStageBegin.lstEndExecution.Add(closeStartUIExecution);

    //    levelProcessSystem.lstStages[0].StartBegin();
    //}
    //private void Update()
    //{
    //    if (levelProcessSystem.lstStages[0].currentStageIndex < 3)
    //    {
    //        levelProcessSystem.UpdateStages(UnityEngine.Time.deltaTime);
    //    }
    //    else
    //    {
    //        Debug.Log("关卡结束，跳转主城");
    //    }

    //}
    //public void StartBotton()
    //{
    //    LevelStageBegin levelStageBegin = levelProcessSystem.lstStages[0].lstStages[0] as LevelStageBegin;
    //    startCondition = (StartCondition)levelStageBegin.lstUpdateExecution[0].LstCondition[0];
    //    startCondition.isStart = true;
    //}

    //public void EndBotton()
    //{
    //    LevelStageEnd levelStageEnd = levelProcessSystem.lstStages[0].lstStages[2] as LevelStageEnd;
    //    endCondition = (EndCondition)levelStageEnd.lstUpdateExecution[0].LstCondition[0];
    //    endCondition.isEnd = true;
    //}

}