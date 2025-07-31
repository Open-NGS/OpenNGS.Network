using System.Collections.Generic;
using OpenNGS.Quest.Common;
using OpenNGS.Quest.Data;

namespace OpenNGS.Systems
{
    public interface INgQuestSystem
    {
        public List<QuestData> AddQuest(uint questGroupID);
        public void RemoveQuest(uint uid);
        public List<QuestData> GetQuests(uint status);
        public List<QuestData> GetGroupQuests(uint questGroupID);
        public void AddQuestContainer(QuestContainer Container);
        void UpdateQuest(uint groupid, uint questid, Quest_Status _status );
    }
}
