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
            TechNodeSaveData savedata = GetTechNodeById(data.ID);
            if (savedata != null)
            {
                savedata = data;
            }
            else
            {
                techDict.Add(data);
            }

        }
        public TechNodeSaveData GetTechNodeById(uint techId)
        {
            return techDict.FirstOrDefault(i => i.ID == techId);
        }

    }
}
