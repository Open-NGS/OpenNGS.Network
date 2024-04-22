using OpenNGS.Collections.Generic;
using OpenNGS.Data;
using OpenNGS.Item.Data;
using OpenNGS.Quest.Common;
using OpenNGS.Quest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Systems;
using QuestGroup = OpenNGS.Quest.Data.QuestGroup;
namespace OpenNGS.Systems
{
    public class QuestSystem : GameSubSystem<QuestSystem>, IQuestSystem
    {
        private QuestContainer questContainer = null;
        private IDialogSystem _dialogSys;

        protected override void OnCreate()
        {
            _dialogSys = App.GetService<IDialogSystem>();
            base.OnCreate();
        }
        public override string GetSystemName()
        {
            return "com.openngs.system.quest";
        }
        public void AddQuestGroup(uint questGroupID)
        {
            QuestGroup quest = NGSStaticData.QuestGroup.GetItem(questGroupID);
            if(!quest.IsBan && !questContainer.QuestList.Contains(quest))
            {
                questContainer.AddQuestGroup(quest);
            }
        }
        public void RemoveQuestGroup(uint questGroupID)
        {
            foreach (var questGroup in questContainer.QuestList)
            {
                if (questGroup.QuestGroupID == questGroupID)
                {
                    questContainer.RemoveQuestGroup(questGroup);
                }
            }
        }
        public void UpdateQuest(uint questGroupID, uint questID, OpenNGS.Quest.Common.QUEST_STATUS status)
        {
            OpenNGS.Quest.Data.Quest quest = questContainer.GetQuestById(questGroupID, questID);
            questContainer.UpdateQuest(quest, status);
        }

        public QuestGroup GetQuestGroupById(uint questGroupID)
        {
            return questContainer.GetQuestGroupById((int)questGroupID);
        }
        public OpenNGS.Quest.Data.Quest GetQuestById(uint questGroupID, uint questID)
        {
            return questContainer.GetQuestById(questGroupID,questID);
        }
        public bool CheckPreconditions(QuestGroup quest)
        {
            if (quest.RelyOnGroupID > 0)
            {
                return IsQuestGroupCompleted(quest.RelyOnGroupID);
            }
            else
            {
                return true;
            }

        }

        public void StartQuest(uint questGroupID)
        {
            QuestGroup questGroup = GetQuestGroupById(questGroupID);
            bool isCanStart = CheckPreconditions(questGroup);
            if (questGroup != null && !questGroup.IsBan && isCanStart)
            {
                switch (questGroup.QuestPickRule)
                {
                    case QUEST_PICK_RULE.QUEST_PICK_RULE_NONE:
                        StartAllQuestsInGroup(questGroup);
                        break;
                    case QUEST_PICK_RULE.QUEST_PICK_RULE_RANDOM:
                        StartRandomQuestsInGroup(questGroup, questGroup.PickNum);
                        break;
                    case QUEST_PICK_RULE.QUEST_PICK_RULE_ORDER:
                        StartNextAvailableQuestInGroup(questGroup);
                        break;
                    case QUEST_PICK_RULE.QUEST_PICK_RULE_ALL:
                        StartAllQuestsInGroup(questGroup);
                        break;
                    default:
                        break;
                }
            }
        }

        private void StartAllQuestsInGroup(QuestGroup questGroup)
        {
            foreach (uint questID in questGroup.Quests)
            {
                OpenNGS.Quest.Data.Quest quest = GetQuestById(questGroup.QuestGroupID, questID);
                if (quest != null && !quest.IsBan )
                {
                    ExecuteQuest(quest);
                }
            }
        }

        private void StartRandomQuestsInGroup(QuestGroup questGroup, uint pickNum)
        {
            var availableQuests = questGroup.Quests
                .Select(questID => new
                {
                    Quest = GetQuestById(questGroup.QuestGroupID, questID),
                    Weight = GetQuestById(questGroup.QuestGroupID, questID)?.Weight ?? 0
                })
                .Where(q => q.Quest != null && !q.Quest.IsBan)
                .ToList();

            // 使用权重来随机选择指定数量的任务
            var randomQuests = new List<Quest.Data.Quest>();
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
                        randomQuests.Add(questWithWeight.Quest);
                        availableQuests.Remove(questWithWeight); // 从列表中移除已选择的任务，以避免重复
                        break;
                    }
                }
            }
            foreach (var quest in randomQuests)
            {
                ExecuteQuest(quest);
            }
        }

        private void StartNextAvailableQuestInGroup(QuestGroup questGroup)
        {
            if (!questGroup.IsBan && questContainer.QuestList.Contains(questGroup))
            {
                uint[] quests = questGroup.Quests;
                foreach (uint questid in quests)
                {
                    var quest = NGSStaticData.Quest.GetItem(questid);
                    if (quest != null && quest.IsQuestHead && !quest.IsBan)
                    {
                        ExecuteQuest(quest);
                    }
                }
            }
        }

        private void ExecuteQuest(Quest.Data.Quest quest)
        {
            quest.Status = QUEST_STATUS.QUEST_STATUS_IN_PROGRESS;
            questContainer.UpdateQuest(quest, quest.Status);
            if (quest.DialogID > 0)
            {
                _dialogSys.LoadDialogs(quest.DialogID);
            }
        }

        public void CompleteQuest(uint questGroupID, uint questID)
        {
            Quest.Data.Quest quest = questContainer.GetQuestById(questGroupID, questID);
            if (quest != null && quest.Status == QUEST_STATUS.QUEST_STATUS_IN_PROGRESS)
            {
                quest.Status = QUEST_STATUS.QUEST_STATUS_COMPLETED;
                questContainer.UpdateQuest(quest, quest.Status);
                AwardReward(quest);
                Quest.Data.Quest nextQuest = questContainer.GetQuestById(questGroupID, quest.NextQuestID);
                if(nextQuest != null && nextQuest.Status!= QUEST_STATUS.QUEST_STATUS_IN_PROGRESS)
                {
                    NextQuest(questGroupID, quest.NextQuestID);
                }
            }
        }
        public void CompleteQuestGroup(uint questGroupID)
        {
            QuestGroup questGroup = questContainer.GetQuestGroupById((int)questGroupID);
            //questGroup.NextQuestGroupID;
        }
        private void NextQuest(uint questGroupID, uint questID)
        {
            Quest.Data.Quest quest = questContainer.GetQuestById(questGroupID, questID);
            questContainer.UpdateQuest(quest, QUEST_STATUS.QUEST_STATUS_COMPLETED);
            var nextQuest = NGSStaticData.Quest.GetItem(quest.NextQuestID);
            if (nextQuest != null && !nextQuest.IsBan)
            {
                ExecuteQuest(nextQuest);
            }
            else
            {
                CompleteQuestGroup(questGroupID);
            }
        }

        public void AwardRewards(QuestGroup quest)
        {
        }
        private void AwardReward(Quest.Data.Quest quest)
        {
            // 实现奖励发放逻辑
            // 例如，根据 quest.RewardID 从奖励系统中检索并应用奖励
        }


        public List<QuestGroup> GetActiveQuestGroups()
        {
            List<QuestGroup> activeQuestGroups = new List<QuestGroup>();
            foreach (var questGroup in questContainer.QuestList)
            {
                if (questGroup.IsBan)
                {
                    continue; // 如果任务组被废弃，则跳过这个任务组
                }
                bool hasActiveQuest = questGroup.Quests.Any(questID =>
                {
                    Quest.Data.Quest quest = GetQuestById(questGroup.QuestGroupID, questID);
                    if (quest.IsBan)
                    {
                        return false;
                    }
                    return quest.Status == QUEST_STATUS.QUEST_STATUS_AVAILABLE || quest.Status == QUEST_STATUS.QUEST_STATUS_IN_PROGRESS;
                });

                if (hasActiveQuest)
                {
                    activeQuestGroups.Add(questGroup);
                }
            }

            return activeQuestGroups;
        }

        public List<QuestGroup> GetCompletedQuestGroups()
        {
            List<QuestGroup> completedQuestGroups = new List<QuestGroup>();

            foreach (var questGroup in questContainer.QuestList)
            {
                if (IsQuestGroupCompleted(questGroup.QuestGroupID))
                {
                    completedQuestGroups.Add(questGroup);
                }
            }

            return completedQuestGroups;
        }
        public bool IsQuestGroupCompleted(uint questGroupID)
        {
            QuestGroup questGroup = GetQuestGroupById(questGroupID);
            if (questGroup == null) return false;
            foreach (var quest in questGroup.Quests)
            {
                Quest.Data.Quest questDetail = GetQuestById(questGroupID, quest);
                if (!questDetail.IsBan && questDetail.Status != QUEST_STATUS.QUEST_STATUS_COMPLETED)
                {
                    return false;
                }
            }
            return true;
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