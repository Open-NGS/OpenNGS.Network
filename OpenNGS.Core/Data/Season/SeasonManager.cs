using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonManager : OpenNGS.Singleton<SeasonManager>
{

    public int CurrentSeason { get; private set; }


    /// <summary>
    /// 初始化赛季
    /// </summary>
    /// <param name="seasonId"></param>
    public void Init(int seasonId)
    {
        this.CurrentSeason = seasonId;
    }



}
