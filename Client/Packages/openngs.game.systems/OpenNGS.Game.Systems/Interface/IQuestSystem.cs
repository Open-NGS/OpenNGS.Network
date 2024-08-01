using OpenNGS.Quest.Data;
using System.Collections.Generic;

namespace OpenNGS.Systems
{
    public interface IQuestSystem
    {

        public void AddQuestGroup(uint questGroupID);
        public void RemoveQuestGroup(uint questGroupID);
        public void UpdateQuest(uint questGroupID, uint questID, OpenNGS.Quest.Common.Quest_Status status);
        public QuestGroupData GetQuestGroupById(uint questGroupID);
        public QuestData GetQuestById(uint questGroupID, uint questID);
        public List<QuestGroup> GetActiveQuestGroups();
        public List<QuestGroup> GetCompletedQuestGroups();
        public void StartQuest(uint questGroupID);
        public void CompleteQuest(uint questGroupID, uint questID);
        void AddQuestContainer(QuestContainer Container);
        bool IsQuestGroupCompleted(uint questGroupID);
        void CompleteQuestGroup(uint questGroupID);
    }
}