using System.Collections;
using System.Collections.Generic;

public interface INgBlindBoxSystem
{
    public Dictionary<uint, int> DoDrop(uint DropID, uint executeCount);
    public void ChangeWeight(uint nDropID, uint ItemID);
    public void ResetWeight(uint nDropID);
    public void ResetData();
}
