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
using Quest = OpenNGS.Quest.Data.Quest;
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
            QuestGroup _questGroupInf = NGSStaticData.QuestGroup.GetItem(questGroupID);
            if(_questGroupInf != null && !_questGroupInf.IsBan)
            {
                bool bFound = false;
                foreach(QuestGroupData _groupData in questContainer.QuestList)
                {
                    if(_groupData.QuestGroupID == questGroupID)
                    {
                        bFound = true;
                    }
                }
                if(bFound == false)
                {
                    QuestGroupData _groupData = new QuestGroupData();
                    _groupData.QuestGroupID = questGroupID;
                    foreach( uint nQuestID in _questGroupInf.Quests)
                    {
                        QuestData _questData = new QuestData();
                        _questData.QuestID = nQuestID;
                        _questData.Status = QUEST_STATUS.QUEST_STATUS_NONE;
                        _groupData.Quests.Add(_questData);
                    }
                    questContainer.AddQuestGroup(_groupData);
                }
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
            QuestData quest = questContainer.GetQuestById(questGroupID, questID);
            questContainer.UpdateQuest(quest, status);
        }

        public QuestGroupData GetQuestGroupById(uint questGroupID)
        {
            return questContainer.GetQuestGroupById((int)questGroupID);
        }
        public QuestData GetQuestById(uint questGroupID, uint questID)
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
            QuestGroup _questGroupInf = NGSStaticData.QuestGroup.GetItem(questGroupID);
            if(_questGroupInf == null)
            {
                NgDebug.LogError(string.Format("QuestGroupID[{0}] has no data", questGroupID));
                return;
            }
            bool isCanStart = CheckPreconditions(_questGroupInf);
            if (_questGroupInf != null && !_questGroupInf.IsBan && isCanStart)
            {
                switch (_questGroupInf.QuestPickRule)
                {
                    case QUEST_PICK_RULE.QUEST_PICK_RULE_NONE:
                        StartAllQuestsInGroup(_questGroupInf);
                        break;
                    case QUEST_PICK_RULE.QUEST_PICK_RULE_RANDOM:
                        StartRandomQuestsInGroup(_questGroupInf, _questGroupInf.PickNum);
                        break;
                    case QUEST_PICK_RULE.QUEST_PICK_RULE_ORDER:
                        StartNextAvailableQuestInGroup(_questGroupInf);
                        break;
                    case QUEST_PICK_RULE.QUEST_PICK_RULE_ALL:
                        StartAllQuestsInGroup(_questGroupInf);
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
                QuestData _questData = GetQuestById(questGroup.QuestGroupID, questID);
                if (_questData != null)
                {
                    OpenNGS.Quest.Data.Quest _QuestInf = NGSStaticData.Quest.GetItem(_questData.QuestID);
                    if(_QuestInf != null && !_QuestInf.IsBan)
                    {
                        ExecuteQuest(_questData);
                    }
                }
            }
        }

        private OpenNGS.Quest.Data.Quest _GetQuestInf(uint nQuestID)
        {
            return NGSStaticData.Quest.GetItem(nQuestID);
        }

        private void StartRandomQuestsInGroup(QuestGroup questGroup, uint pickNum)
        {
            var availableQuests = questGroup.Quests
                .Select(questID => new
                {
                    Quest = _GetQuestInf( questID),
                    Weight = _GetQuestInf(questID)?.Weight ?? 0,
                    questData = GetQuestById(questGroup.QuestGroupID, questID)
                })
                .Where(q => q.Quest != null && !q.Quest.IsBan)
                .ToList();

            // 使用权重来随机选择指定数量的任务
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
            if (questGroup != null && !questGroup.IsBan)
            {
                foreach(QuestGroupData _groupData in questContainer.QuestList)
                {
                    if(_groupData.QuestGroupID == questGroup.QuestGroupID)
                    {
                        foreach(QuestData _questData in _groupData.Quests)
                        {
                            ExecuteQuest(_questData);
                        }
                        break;
                    }
                }
            }
        }

        private void ExecuteQuest(QuestData quest)
        {
            if(quest != null)
            {
                quest.Status = QUEST_STATUS.QUEST_STATUS_IN_PROGRESS;
                questContainer.UpdateQuest(quest, quest.Status);
                Quest.Data.Quest _questInf = NGSStaticData.Quest.GetItem(quest.QuestID);
                if (_questInf != null && _questInf.DialogID > 0)
                {
                    _dialogSys.LoadDialogs(_questInf.DialogID);
                }
            }
        }

        public void CompleteQuest(uint questGroupID, uint questID)
        {
            QuestData quest = questContainer.GetQuestById(questGroupID, questID);
            if (quest != null && quest.Status == QUEST_STATUS.QUEST_STATUS_IN_PROGRESS)
            {
                Quest.Data.Quest _questInf = NGSStaticData.Quest.GetItem(questID);
                if(_questInf != null)
                {
                    quest.Status = QUEST_STATUS.QUEST_STATUS_COMPLETED;
                    questContainer.UpdateQuest(quest, quest.Status);
                    AwardReward(_questInf);
                    QuestData nextQuest = questContainer.GetQuestById(questGroupID, _questInf.NextQuestID);
                    if (nextQuest != null && nextQuest.Status != QUEST_STATUS.QUEST_STATUS_IN_PROGRESS)
                    {
                        NextQuest(questGroupID, _questInf.NextQuestID);
                    }
                }
            }
        }
        public void CompleteQuestGroup(uint questGroupID)
        {
            QuestGroup _QuestGroup = NGSStaticData.QuestGroup.GetItem(questGroupID);
            if (_QuestGroup != null)
            {
                foreach (uint _questID in _QuestGroup.Quests)
                {
                    CompleteQuest(questGroupID, _questID);
                }
            }
        }
        private void NextQuest(uint questGroupID, uint questID)
        {
            QuestData _quest = questContainer.GetQuestById(questGroupID, questID);
            if(_quest != null)
            {
                questContainer.UpdateQuest(_quest, QUEST_STATUS.QUEST_STATUS_COMPLETED);
                var _CurQuestInf = NGSStaticData.Quest.GetItem(_quest.QuestID);
                if(_CurQuestInf == null)
                {
                    return;
                }
                var nextQuest = GetQuestById(questGroupID, questID);
                if (nextQuest != null && !_CurQuestInf.IsBan)
                {
                    ExecuteQuest(nextQuest);
                }
                else
                {
                    CompleteQuestGroup(questGroupID);
                }
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
            // todo
            //foreach (var questGroup in questContainer.QuestList)
            //{
            //    if (questGroup.IsBan)
            //    {
            //        continue; // 如果任务组被废弃，则跳过这个任务组
            //    }
            //    bool hasActiveQuest = questGroup.Quests.Any(questID =>
            //    {
            //        Quest.Data.Quest quest = GetQuestById(questGroup.QuestGroupID, questID);
            //        if (quest.IsBan)
            //        {
            //            return false;
            //        }
            //        return quest.Status == QUEST_STATUS.QUEST_STATUS_AVAILABLE || quest.Status == QUEST_STATUS.QUEST_STATUS_IN_PROGRESS;
            //    });

            //    if (hasActiveQuest)
            //    {
            //        activeQuestGroups.Add(questGroup);
            //    }
            //}

            return activeQuestGroups;
        }

        public List<QuestGroup> GetCompletedQuestGroups()
        {
            List<QuestGroup> completedQuestGroups = new List<QuestGroup>();

            //todo
            //foreach (var questGroup in questContainer.QuestList)
            //{
            //    if (IsQuestGroupCompleted(questGroup.QuestGroupID))
            //    {
            //        completedQuestGroups.Add(questGroup);
            //    }
            //}

            return completedQuestGroups;
        }
        public virtual bool IsQuestGroupCompleted(uint questGroupID)
        {
            QuestGroup questGroup = NGSStaticData.QuestGroup.GetItem(questGroupID);
            if (questGroup == null) return false;
            foreach (var quest in questGroup.Quests)
            {
                QuestData questDetail = GetQuestById(questGroupID, quest);
                if(questDetail != null)
                {
                    OpenNGS.Quest.Data.Quest _questInf = NGSStaticData.Quest.GetItem(questDetail.QuestID);
                    if(_questInf != null)
                    {
                        if (!_questInf.IsBan && questDetail.Status != QUEST_STATUS.QUEST_STATUS_COMPLETED)
                        {
                            return false;
                        }
                    }
                }
                else
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