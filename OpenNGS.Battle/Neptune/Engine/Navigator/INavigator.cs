using Neptune;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/// <summary>
/// Navigator Interface
/// implement to custom navigation
/// </summary>
public interface INavigator
{
    NavAgent CreateAgent(BattleEntity element, int areaMask);
    void LoadNavGridData(byte[] data);
    void Clear();
}