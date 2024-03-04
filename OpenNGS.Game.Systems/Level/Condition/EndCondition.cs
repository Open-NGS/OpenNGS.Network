using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCondition : ICondition
{
    public bool isEnd = false;

    public bool IsConditionValid()
    {
        return isEnd;
    }
}

