using OpenNGS.Quest.Common;
using OpenNGS.Quest.Data;
using System.Collections.Generic;

namespace OpenNGS.Systems
{
    public interface IQuestClientAPI
    {
        public List<QuestData> AddQuest(uint questGroupID);
        public void RemoveQuest(uint uid);
        public List<QuestData> GetQuests(uint status);
        public List<QuestData> GetGroupQuests(uint questGroupID);
        public void AddQuestContainer(QuestContainer Container);
    }
}
