using OpenNGS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelTest : MonoBehaviour
{
    public int levelID = 0;
    private LevelSystem levelSystem;


    private void Start()
    {
        levelSystem = new LevelSystem();
        levelSystem.SetLevel(levelID);     
        levelSystem.InitStages();

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