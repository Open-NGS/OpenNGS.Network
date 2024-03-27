using OpenNGS.Levels.Common;
using OpenNGS.Levels.Data;
using System;
using System.Collections;
using System.Collections.Generic;


public interface ICondition
{
    public void InitCondition(uint ConditionParam1, uint ConditionParam2);
    public bool IsConditionValid();
}
