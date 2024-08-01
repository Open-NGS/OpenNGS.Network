using OpenNGS.Quest.Data;
using System.Collections.Generic;
using Systems;
using OpenNGS.Quest.Common;
using QuestGroup = OpenNGS.Quest.Data.QuestGroup;
using System.Linq;

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
            var availableQuests = questGroup.Quests
            .Select(questID => new
            {
                Quest = NGSStaticData.Quest.GetItem(questID),
                Weight = NGSStaticData.Quest.GetItem(questID)?.Weight ?? 0,
                questData = questContainer.GetQuestById(questGroup.QuestGroupID, questID)
            })
            .Where(q => q.Quest != null && !q.Quest.IsBan)
            .ToList();

            var randomQuests = new List<QuestData>();
            var random = new System.Random();
            var totalWeight = availableQuests.Sum(q => q.Weight);
            for (int i = 0; i < pickNum; i++)
            {
                double randomValue = random.NextDouble() * totalWeight;
                double weightSum = 0;

                foreach (var questWithWeight in availableQuests)
                {
                    weightSum += questWithWeight.Weight;
                    if (weightSum >= randomValue)
                    {
                        randomQuests.Add(questWithWeight.questData);
                        availableQuests.Remove(questWithWeight);
                        break;
                    }
                }
            }
            foreach (var quest in randomQuests)
            {
                questGroupData.QuestDataList.Add(new QuestData { QuestID = quest.QuestID, Status = Quest_Status.Status_Available });
            }
            questContainer.AddQuestGroup(questGroupData);
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

        //public void AddQuest(uint questID)
        //{
        //    if (questContainer.QuestGroupList.Any(qg => qg.QuestDataList.Any(q => q.QuestID == questID)))
        //    {
        //        return;
        //    }
        //    QuestData questData = new QuestData { QuestID = questID, Status = Quest_Status.Status_Available };
        //    QuestGroupData defaultGroup = questContainer.QuestGroupList.FirstOrDefault();
        //    if (defaultGroup == null)
        //    {
        //        defaultGroup = new QuestGroupData { QuestGroupID = 0 };
        //        questContainer.AddQuestGroup(defaultGroup);
        //    }
        //    defaultGroup.QuestDataList.Add(questData);
        //}

        //public void FinishQuest(uint questID)
        //{
        //    var questGroup = questContainer.QuestGroupList.FirstOrDefault(qg => qg.QuestDataList.Any(q => q.QuestID == questID));
        //    if (questGroup != null)
        //    {
        //        var quest = questGroup.QuestDataList.FirstOrDefault(q => q.QuestID == questID);
        //        if (quest != null)
        //        {
        //            quest.Status = Quest_Status.Status_Completed;
        //        }
        //    }
        //}

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
        protected override void OnClear()
        {
            questContainer = null;
            base.OnClear();
        }
    }
}
