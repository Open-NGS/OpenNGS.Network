using OpenNGS.Quest.Data;
using System.Collections.Generic;

namespace OpenNGS.Systems
{
    public interface IQuestSystem
    {

        public void AddQuestGroup(uint questGroupID);
        public void RemoveQuestGroup(uint questGroupID);
        public void UpdateQuest(uint questGroupID, uint questID, OpenNGS.Quest.Common.QUEST_STATUS status);
        public QuestGroup GetQuestGroupById(uint questGroupID);
        public OpenNGS.Quest.Data.Quest GetQuestById(uint questGroupID, uint questID);
        public List<QuestGroup> GetActiveQuestGroups();
        public List<QuestGroup> GetCompletedQuestGroups();
        public void StartQuest(uint questGroupID);
        public void CompleteQuest(uint questGroupID, uint questID);
        void AddQuestContainer(QuestContainer Container);

    }
}