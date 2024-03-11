using System.Collections;
using System.Collections.Generic;


public interface ITimeSystem
{
    public long GetCurTime();
    public void OnEnterFrame(float dt);
    void SetStartTime(double time);
}
