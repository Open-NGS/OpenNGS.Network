using OpenNGS.Quest.Data;
using System.Collections.Generic;
using Systems;
using OpenNGS.Quest.Common;
using QuestGroup = OpenNGS.Quest.Data.QuestGroup;
using System.Linq;
using System;
using OpenNGS.Data;

namespace OpenNGS.Systems
{
    public class NgQuestSystem : GameSubSystem<NgQuestSystem>, INgQuestSystem
    {
        private QuestContainer questContainer = null;

        protected override void OnCreate()
        {
            base.OnCreate();
        }
        public override string GetSystemName()
        {
            return "com.openngs.system.quest";
        }
        public void AddQuestGroup(uint questGroupID)
        {
            QuestGroup questGroup = NGSStaticData.QuestGroup.GetItem(questGroupID);
            if (questGroup != null && !questGroup.IsBan)
            {
                if (questContainer.QuestGroupList.All(qg => qg.QuestGroupID != questGroupID))
                {
                    switch (questGroup.QuestPickRule)
                    {
                        case Quest_PickRule.PickRule_None:
                            AddAllQuests(questGroup);
                            break;
                        case Quest_PickRule.PickRule_Random:
                            AddRandomQuests(questGroup, questGroup.PickNum);
                            break;
                        case Quest_PickRule.PickRule_Order:
                            AddOrderQuest(questGroup);
                            break;
                        case Quest_PickRule.PickRule_All:
                            AddAllQuests(questGroup);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private void AddOrderQuest(QuestGroup questGroup)
        {
            QuestGroupData questGroupData = new QuestGroupData { QuestGroupID = questGroup.QuestGroupID };
            foreach (uint questID in questGroup.Quests)
            {
                questGroupData.QuestDataList.Add(new QuestData { QuestID = questID, Status = Quest_Status.Status_Available });
            }
            questContainer.AddQuestGroup(questGroupData);
        }

        private void AddRandomQuests(QuestGroup questGroup, uint pickNum)
        {
            QuestGroupData questGroupData = new QuestGroupData { QuestGroupID = questGroup.QuestGroupID };
            Dictionary<uint, float> weights = new Dictionary<uint, float>();
            foreach (uint questID in questGroup.Quests)
            {
                OpenNGS.Quest.Data.Quest quest = NGSStaticData.Quest.GetItem(questID);
                if (quest != null && !quest.IsBan)
                {
                    weights.Add(questID, quest.Weight);
                }
            }
            var totalWeight = weights.Values.Sum();
            var selectedQuests = ExcuteRandom(pickNum, weights, totalWeight);

            foreach (var questID in selectedQuests)
            {
                var questData = new QuestData { QuestID = questID, Status = Quest_Status.Status_Available };
                questGroupData.QuestDataList.Add(questData);
            }
            questContainer.AddQuestGroup(questGroupData);

        }
        private List<uint> ExcuteRandom(uint optionCount, Dictionary<uint, float> weightDic, float toatlWeight)
        {
            Random random = new Random();
            List<uint> result = new List<uint>();
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

        public List<uint> GetQuest(uint QuestGroupID, Quest_Status status)
        {
            var questGroup = questContainer.QuestGroupList
                .FirstOrDefault(qg => qg.QuestGroupID == QuestGroupID);
            if (questGroup != null)
            {
                return questGroup.QuestDataList
                    .Where(q => q.Status == status)
                    .Select(q => q.QuestID)
                    .ToList();
            }
            return new List<uint>();
        }

        private void AddAllQuests(QuestGroup questGroup)
        {
            QuestGroupData questGroupData = new QuestGroupData { QuestGroupID = questGroup.QuestGroupID };
            foreach (uint questID in questGroup.Quests)
            {
                questGroupData.QuestDataList.Add(new QuestData { QuestID = questID, Status = Quest_Status.Status_Available });
            }
            questContainer.AddQuestGroup(questGroupData);
        }

        public void FinishQuestGroup(uint questGroupID)
        {
            var questGroup = questContainer.QuestGroupList.FirstOrDefault(qg => qg.QuestGroupID == questGroupID);
            if (questGroup != null)
            {
                foreach (var quest in questGroup.QuestDataList)
                {
                    quest.Status = Quest_Status.Status_Completed;
                }
                //questContainer.RemoveQuestGroup(questGroup);
            }
        }

        public void UpdateQuest(uint questGroupID, uint questID, OpenNGS.Quest.Common.Quest_Status status)
        {
            QuestData quest = questContainer.GetQuestById(questGroupID, questID);
            questContainer.UpdateQuest(quest, status);
        }

        public List<uint> GetQuestGroup(Quest_Status status)
        {
            return questContainer.QuestGroupList
                .Where(qg => qg.QuestDataList.All(q => q.Status == status))
                .Select(qg => qg.QuestGroupID)
                .ToList();
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
        public bool IsExistQuestGroup(uint questGroupID)
        {
            return questContainer.QuestGroupList.Any(qg => qg.QuestGroupID == questGroupID);
        }

        public bool IsExistQuestData(uint questGroupID, uint questID)
        {
            return questContainer.GetQuestById(questGroupID, questID) != null;
        }

        protected override void OnClear()
        {
            questContainer = null;
            base.OnClear();
        }

    }
}
