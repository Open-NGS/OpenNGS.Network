using System.Collections;
using System.Collections.Generic;


public class EndCondition : ICondition
{
    public bool isEnd = false;

    public virtual bool IsConditionValid()
    {
        return isEnd;
    }
}

