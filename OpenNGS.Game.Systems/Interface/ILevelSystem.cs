using OpenNGS.Exchange.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelSystem
{
    public void SetLevel(int levelid);
    public void UpdateStages(float deltaTime);
}
