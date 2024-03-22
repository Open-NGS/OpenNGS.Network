using OpenNGS.Levels.Common;
using OpenNGS.Levels.Data;
using System;
using System.Collections;
using System.Collections.Generic;


public interface ICondition
{
    public void InitCondition(ExecuteCondtionData data);
    public bool IsConditionValid();
}
