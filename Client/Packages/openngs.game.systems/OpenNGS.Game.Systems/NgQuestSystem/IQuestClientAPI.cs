using OpenNGS.Quest.Common;
using OpenNGS.Quest.Data;
using System.Collections.Generic;

namespace OpenNGS.Systems
{
    public interface IQuestClientAPI
    {
        public void AddQuestGroup(uint questGroupID);
        public void FinishQuestGroup(uint questGroupID);
        public void UpdateQuest(uint questGroupID, uint questID, OpenNGS.Quest.Common.Quest_Status status);
        public List<uint> GetQuestGroup(Quest_Status status);
        public void AddQuestContainer(QuestContainer Container);
        public bool IsExistQuestData(uint questGroupID, uint questID);
    }
}
