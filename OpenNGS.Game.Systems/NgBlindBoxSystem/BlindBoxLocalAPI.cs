using System.Collections;
using System.Collections.Generic;
using Systems;
using OpenNGS;

public class BlindBoxLocalAPI : Singleton<BlindBoxLocalAPI>,IBlindBoxClientAPI
{
    INgBlindBoxSystem m_NgBlindBoxSystem;
    public void Init()
    {
        m_NgBlindBoxSystem = App.GetService<INgBlindBoxSystem>();
    }

    public Dictionary<uint, int> DoDrop(uint DropID, uint executeCount)
    {
        return m_NgBlindBoxSystem.DoDrop(DropID, executeCount);
    }

    public void ResetData()
    {
        m_NgBlindBoxSystem.ResetData();
    }

    public void ResetWeight(uint nDropID)
    {
        m_NgBlindBoxSystem.ResetWeight(nDropID);
    }

    public void ChangeWeight(uint nDropID, uint ItemID)
    {
        m_NgBlindBoxSystem.ChangeWeight(nDropID, ItemID);
    }
}
