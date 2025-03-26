using System.Collections.Generic;
using Systems;
using OpenNGS.Quest.Common;
using System.Linq;
using System;
using OpenNGS.Quest.Data;



namespace OpenNGS.Systems
{
    public class NgQuestSystem : GameSubSystem<NgQuestSystem>, INgQuestSystem
    {
        private QuestContainer questContainer = null;
        private uint m_uid = 1;
        protected override void OnCreate()
        {
            base.OnCreate();
        }
        public override string GetSystemName()
        {
            return "com.openngs.system.quest";
        }
        public List<QuestData> AddQuest(uint questGroupID)
        {
            QuestGroup questGroup = QuestStaticData.QuestGroup.GetItem(questGroupID);
            List<QuestData> res = new List<QuestData>();
            if (questGroup != null && !questGroup.IsBan)
            {
                switch (questGroup.QuestPickRule)
                {
                    case Quest_PickRule.PickRule_None:
                        res = AddAllQuests(questGroup);
                        break;
                    case Quest_PickRule.PickRule_Random:
                        res = AddRandomQuests(questGroup, questGroup.PickNum);
                        break;
                    case Quest_PickRule.PickRule_Order:
                        res = AddOrderQuest(questGroup);
                        break;
                    case Quest_PickRule.PickRule_All:
                        res = AddAllQuests(questGroup);
                        break;
                    default:
                        break;
                }
            }
            return res;
        }
        private List<QuestData> AddOrderQuest(QuestGroup questGroup)
        {
            List<QuestData> acceptedQuests = new List<QuestData>();
            List<QuestData> questDatas = questContainer.GetQuestDatas(questGroup.QuestGroupID);
            Dictionary<uint, Quest.Data.Quest> questsDic = QuestStaticData.Quest.GetItems(questGroup.QuestGroupID);
            foreach (Quest.Data.Quest quest in questsDic.Values)
            {
                int index = 0;
                if (index == questDatas.Count)
                {
                    QuestData questData = GenerateQuestInstance(quest);
                    acceptedQuests.Add(questData);
                    break;
                }
                index++;
            }
            return acceptedQuests;
        }

        private List<QuestData> AddRandomQuests(QuestGroup questGroup, uint pickNum)
        {
            List<QuestData> acceptedQuests = new List<QuestData>();
            Dictionary<uint, Quest.Data.Quest> questsDic = QuestStaticData.Quest.GetItems(questGroup.QuestGroupID);
            List<QuestData> questDatas = questContainer.GetQuestDatas(questGroup.QuestGroupID);
            Dictionary<uint, float> weights = new Dictionary<uint, float>();
            foreach(Quest.Data.Quest quest in questsDic.Values)
            {
                if (quest != null && !quest.IsBan)
                {
                    weights.Add(quest.QuestID, quest.Weight);
                }
            }
            foreach(QuestData data in questDatas)
            {
                weights.Remove(data.QuestID);
            }
            var totalWeight = weights.Values.Sum();
            var selectedQuests = ExcuteRandom(pickNum, weights, totalWeight);

            foreach (var questID in selectedQuests)
            {
                Quest.Data.Quest quest = QuestStaticData.Quest.GetItem(questGroup.QuestGroupID, questID);
                if(quest != null)
                {
                    QuestData questData = GenerateQuestInstance(quest);
                    acceptedQuests.Add(questData);
                }
            }
            return acceptedQuests;

        }
        private List<uint> ExcuteRandom(uint optionCount, Dictionary<uint, float> weightDic, float toatlWeight)
        {
            List<uint> result = new List<uint>();
            if (optionCount >= weightDic.Count)
            {
                foreach (uint id in weightDic.Keys)
                {
                    result.Add(id);
                }
                return result;
            }
            Random random = new Random();
            List<uint> droped = new List<uint>();
            float curWeight = toatlWeight;
            Dictionary<uint, uint> poolSlectCount = new Dictionary<uint, uint>();
            for (int i = 0; i < optionCount; i++)
            {
                if (curWeight <= 0)
                {
                    break;
                }
                float curNum = (float)NextDouble(random, 0, curWeight);
                foreach (KeyValuePair<uint, float> weight in weightDic)
                {
                    if (droped.Contains(weight.Key))
                    {
                        curNum -= 0;
                    }
                    else
                    {
                        curNum -= weight.Value;
                    }
                    if (curNum < 0)
                    {
                        result.Add(weight.Key);
                        droped.Add(weight.Key);
                        curWeight -= weight.Value;
                        break;
                    }
                }
            }
            return result;
        }
        private double NextDouble(Random ran, float minValue, float maxValue)
        {
            double randNum = ran.NextDouble() * (maxValue - minValue) + minValue;
            return randNum;
        }

        private List<QuestData> AddAllQuests(QuestGroup questGroup)
        {
            List<QuestData> acceptedQuests = new List<QuestData>();
            Dictionary<uint,Quest.Data.Quest> questsDic = QuestStaticData.Quest.GetItems(questGroup.QuestGroupID);
            foreach(Quest.Data.Quest quest in questsDic.Values)
            {
                QuestData questData=GenerateQuestInstance(quest);
                acceptedQuests.Add(questData);
            }
            return acceptedQuests;
        }
        private QuestData GenerateQuestInstance(Quest.Data.Quest quest)
        {
            QuestData questData = new QuestData();
            questData.Uid = GenerateUniqueId();
            questData.GroupID = quest.GroupID;
            questData.QuestID = quest.QuestID;
            questData.StatID = quest.StatID;
            questData.CurVal = 0;
            questData.Status = (uint)Quest_Status.Status_Accepted;
            questContainer.AddQuestData(questData);
            return questData;
        }
        private uint GenerateUniqueId()
        {
            List<uint> usedIDs = questContainer.QuestList.Select(item => item.Uid).ToList();
            if(usedIDs.Count == 0)
            {
                return m_uid;
            }
            while (usedIDs.Contains(m_uid))
            {
                m_uid++;
            }
            return m_uid;
        }

        public void RemoveQuest(uint uid)
        {
            QuestData quest=questContainer.GetQuestData(uid);
            if(quest != null)
            {
                questContainer.RemoveQuestData(quest);
            }
        }
        public List<QuestData> GetGroupQuests(uint questGroupID)
        {
            List<QuestData> quests = questContainer.GetQuestDatas(questGroupID);
            return quests;
        }

        public List<QuestData> GetQuests(uint status)
        {
            List<QuestData> quests = questContainer.GetQuestDatasByStatus(status);
            return quests;
        }

        public void AddQuestContainer(QuestContainer Container)
        {
            if (Container != null)
            {
                questContainer = Container;
            }
            else
            {
                questContainer = new QuestContainer();
            }
        }


        protected override void OnClear()
        {
            questContainer = null;
            base.OnClear();
        }
    }
}
