using System.Collections;
using System.Collections.Generic;
using OpenNGS;

public class BlindBoxService : Singleton<BlindBoxService>
{
    IBlindBoxClientAPI m_IBlindBoxClientAPI;
    public void Init(IBlindBoxClientAPI blindBoxClientAPI)
    {
        m_IBlindBoxClientAPI = blindBoxClientAPI;
    }

    public Dictionary<uint, int> DoDrop(uint DropID, uint executeCount)
    {
        return m_IBlindBoxClientAPI.DoDrop(DropID, executeCount);
    }

    public void ResetData()
    {
        m_IBlindBoxClientAPI.ResetData();
    }

    public void ResetWeight(uint nDropID)
    {
        m_IBlindBoxClientAPI.ResetWeight(nDropID);
    }

    public void ChangeWeight(uint nDropID, uint ItemID)
    {
        m_IBlindBoxClientAPI.ChangeWeight(nDropID, ItemID);
    }
}
