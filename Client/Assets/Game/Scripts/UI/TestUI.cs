using OpenNGS.Systems;
using OpenNGS;
using OpenNGS.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameInstance.Instance.Init();
        TestStatisticData.Init();
        NGSStaticData.Init();
        DataManager.Instance.Init();
        UIManager.Init();
        UIManager.Instance.Open("UI_TESTNPC");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
