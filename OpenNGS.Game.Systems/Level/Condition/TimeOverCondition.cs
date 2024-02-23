using OpenNGS;
using OpenNGS.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOverCondition : ICondition
{
    public float countdownTime;
    public float startTime;
    private ITimeSystem timeSys;

    // 设置倒计时时间
    public void SetCountdownTime(float time)
    {
        countdownTime = time;
    }

    // 初始化方法
    public void Init()
    {
        startTime = timeSys.GetCurTime(); 
    }

    // 判断是否达到倒计时条件
    public bool IsConditionValid()
    {
        float elapsedTime = timeSys.GetCurTime() - startTime;
        if (elapsedTime >= countdownTime)
        {
            return true;
        }
        return false;
    }
}
