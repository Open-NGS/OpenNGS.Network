using OpenNGS.Exchange.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelSystem
{
    public void CurLevel(int levelid);
    public void SetTimer(int timer);
    public void UpdateStages(float deltaTime);
}
