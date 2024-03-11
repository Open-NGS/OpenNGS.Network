
using UnityEngine.UI;

public class StartCondition : ICondition
{
    public bool isStart = false;

    public bool IsConditionValid()
    {
        return isStart;
    }
}