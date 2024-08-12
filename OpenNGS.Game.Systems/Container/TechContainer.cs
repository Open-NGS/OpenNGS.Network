using OpenNGS.Reward.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Technology.Data
{
    public partial class TechContainer
    {
        public void AddTechNode(TechNodeSaveData data)
        {
            techDict.Add(data);
        }
        public void RemoveTechNode(TechNodeSaveData data)
        {
            techDict.Remove(data);
        }
        public void SetTechNode(TechNodeSaveData data)
        {
            TechNodeSaveData savedata = techDict.Find(i => i.ID == data.ID);
            if (savedata != null)
            {
                savedata.ID = data.ID;
                savedata.Level = data.Level;
                savedata.Locked = data.Locked;
                savedata.Activated = data.Activated;
            }
            else
            {
                techDict.Add(data);
            }

        }
        public TechNodeSaveData GetTechNodeById(uint techId)
        {
            return techDict.Find(i => i.ID == techId);
        }

    }
}
