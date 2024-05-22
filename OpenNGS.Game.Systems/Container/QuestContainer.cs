using System.Linq;
using OpenNGS.Quest.Common;
using OpenNGS.Systems;

namespace OpenNGS.Quest.Data
{
    public partial class QuestContainer
    {
        public void AddQuestGroup(QuestGroupData quest)
        {
            QuestList.Add(quest);
        }
        public void RemoveQuestGroup(QuestGroupData quest)
        {
            QuestList.Remove(quest);
        }

        public void UpdateQuest(QuestData quest, OpenNGS.Quest.Common.QUEST_STATUS status)
        {
            quest.Status = status;
        }
        public QuestGroupData GetQuestGroupById(int QuestGroupID)
        {
            return QuestList.FirstOrDefault(i => i.QuestGroupID == QuestGroupID);
        }

        public QuestData GetQuestById(uint questGroupID, uint questID)
        {
            QuestGroupData questGroup = QuestList.FirstOrDefault(i => i.QuestGroupID == questGroupID);
            if(questGroup != null )
            {
                foreach (var _quest in questGroup.Quests)
                {
                    if (_quest.QuestID == questID)
                    {
                        return _quest;
                    }
                }
            }
            return null;
        }

    }
}