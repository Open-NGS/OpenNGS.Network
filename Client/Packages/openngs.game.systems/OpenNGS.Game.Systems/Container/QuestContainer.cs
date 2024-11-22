using System.Collections.Generic;
using System.Linq;
using OpenNGS.Data;
using OpenNGS.Quest.Common;
using OpenNGS.Systems;

namespace OpenNGS.Quest.Data
{
    public partial class QuestContainer
    {
        public void AddQuestData(QuestData data)
        {
            QuestList.Add(data);
        }
        public void RemoveQuestData(QuestData data)
        {
            QuestList.Remove(data); 
        }
        public List<QuestData> GetQuestDatas(uint groupID)
        {
            List<QuestData> quests=new List<QuestData>();
            foreach (QuestData data in QuestList)
            {
                if (data.GroupID == groupID)
                {
                    quests.Add(data);
                }
            }
            return quests;
        }
        public List <QuestData> GetQuestDatasByStatus(uint status)
        {
            List<QuestData> quests = new List<QuestData>();
            foreach (QuestData data in QuestList)
            {
                if (data.Status == status)
                {
                    quests.Add(data);
                }
            }
            return quests;
        }
        public QuestData GetQuestData(uint uid)
        {
            foreach (QuestData data in QuestList)
            {
                if (data.Uid == uid)
                {
                    return data;
                }
            }
            return null;
        }
    }
}