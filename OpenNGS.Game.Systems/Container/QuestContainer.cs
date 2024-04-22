using System.Linq;
using OpenNGS.Quest.Common;
using OpenNGS.Systems;

namespace OpenNGS.Quest.Data
{
    public partial class QuestContainer
    {
        public void AddQuestGroup(QuestGroup quest)
        {
            QuestList.Add(quest);
        }
        public void RemoveQuestGroup(QuestGroup quest)
        {
            QuestList.Remove(quest);
        }

        public void UpdateQuest(Quest quest, OpenNGS.Quest.Common.QUEST_STATUS status)
        {
            quest.Status = status;
        }
        public QuestGroup GetQuestGroupById(int QuestGroupID)
        {
            return QuestList.FirstOrDefault(i => i.QuestGroupID == QuestGroupID);
        }

        public Quest GetQuestById(uint questGroupID, uint questID)
        {
            QuestGroup questGroup= QuestList.FirstOrDefault(i => i.QuestGroupID == questGroupID);
            foreach (var quests in questGroup.Quests)
            {
                if (quests == questID)
                {
                    OpenNGS.Quest.Data.Quest quest = NGSStaticData.Quest.GetItem(questGroupID);
                    return quest;
                }
            }
            return null;
        }

    }
}