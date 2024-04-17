using OpenNGS.Collections.Generic;
using OpenNGS.Dialog.Common;
using OpenNGS.Dialog.Data;
using OpenNGS.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using Systems;
using UnityEngine;
using static UnityEditor.Progress;

namespace OpenNGS.Systems
{
    public class DialogueHistoryEntry
    {

        public DialogTalk DialogTalk { get; private set; }
        public DialogChoice ChosenChoice { get; private set; }
        public bool IsChoice { get; private set; }
        public Color ChoiceColor { get; private set; }
        public DialogueHistoryEntry(DialogTalk data, bool isChoice = false, DialogChoice Choice = null, Color choiceColor = default(Color))
        {
            DialogTalk = data;
            ChosenChoice = Choice;
            IsChoice = isChoice;
            ChoiceColor = choiceColor == default(Color) ? Color.white : choiceColor;
        }
    }

    public class DialogSystem : GameSubSystem<DialogSystem>, IDialogSystem
    {
        //private IQuestSystem _questSys;
        public List<DialogTalk> DialogTalks = new List<DialogTalk>();
        public List<DialogChoice> Choices = new List<DialogChoice>();
        public List<DialogueHistoryEntry> History = new List<DialogueHistoryEntry>();
        public DialogTalk CurrentDialog;
        private int CurrentIndex;
        DialogTalk IDialogSystem.CurrentDialog
        {
            get => CurrentDialog;
            set => CurrentDialog = value;
        }
        List<DialogChoice> IDialogSystem.Choices
        {
            get => Choices;
            set => Choices = value;
        }
        protected override void OnCreate()
        {
            //_questSys = App.GetService<IQuestSystem>();
            base.OnCreate();
        }
        protected override void OnClear()
        {
            base.OnClear();
        }
        public override string GetSystemName()
        {
            return "com.openngs.system.dialog";
        }

        public void LoadDialogs(uint dialogId)
        {
            DialogTalks.Clear();
            History.Clear();
            CurrentIndex = 0;
            DialogList dialog = NGSStaticData.Dialogue.GetItem(dialogId);
            uint[] dialogDataIds = dialog.DialogTalkIDs;
            foreach (uint dialogDataId in dialogDataIds)
            {
                DialogTalk dialogData = NGSStaticData.DialogTalk.GetItem(dialogDataId);
                DialogTalks.Add(dialogData);
            }
            DisplayDialog();
        }

        private void DisplayDialog()
        {
            if (CurrentIndex < DialogTalks.Count)
            {
                CurrentDialog = DialogTalks[CurrentIndex];
                //DialogUI.Instance.ShowDialog(CurrentDialog);
                History.Add(new DialogueHistoryEntry(CurrentDialog));
                if (CurrentDialog.ChoiceIDs != null)
                {
                    uint[] choices = CurrentDialog.ChoiceIDs;
                    ShowOption(choices);
                }
                else
                {
                    Choices.Clear();
                }
            }
        }
        public void NextDialog()
        {
            CurrentIndex++;
            DisplayDialog();
        }


        private bool IsOptionAvailable(DialogChoice Choice)
        {
            DIALOG_CHOICE_CONDITION_TYPE ConditionType;
            ConditionType = Choice.DialogChoiceCondition;
            switch (ConditionType)
            {
                case DIALOG_CHOICE_CONDITION_TYPE.DIALOG_CHOICE_CONDITION_TYPE_NONE:
                    return true;
                case DIALOG_CHOICE_CONDITION_TYPE.DIALOG_CHOICE_CONDITION_TYPE_PLAYER_LEVEL:
                    return true;
                //return CheckPlayerLevel(RequiredLevel);
                case DIALOG_CHOICE_CONDITION_TYPE.DIALOG_CHOICE_CONDITION_TYPE_PLAYER_INVENTORY:
                    return true;
                //return CheckPlayerInventory(RequiredItem);
                case DIALOG_CHOICE_CONDITION_TYPE.DIALOG_CHOICE_CONDITION_TYPE_PLAYER_QUEST_PROGRESS:
                    return true;
                //return CheckPlayerQuestProgress(RequiredQuest, RequiredQuestStage);
                case DIALOG_CHOICE_CONDITION_TYPE.DIALOG_CHOICE_CONDITION_TYPE_TIME_OF_DAY:
                    return true;
                //return CheckTimeOfDay(RequiredTimeOfDay);
                case DIALOG_CHOICE_CONDITION_TYPE.DIALOG_CHOICE_CONDITION_TYPE_LOCATION:
                    return true;
                //return CheckPlayerLocation(RequiredLocation);
                case DIALOG_CHOICE_CONDITION_TYPE.DIALOG_CHOICE_CONDITION_TYPE_PREVIOUS_CHOICE:
                    return true;
                //return CheckPreviousChoice(RequiredPreviousChoice);
                default:
                    return false;
            }
        }

        public void ShowOption(uint[] options)
        {
            Choices.Clear();
            foreach (uint optionid in options)
            {
                DialogChoice choice = NGSStaticData.Choice.GetItem(optionid);
                if (IsOptionAvailable(choice))
                {
                    Choices.Add(choice);
                }
            }
            //DialogUI.Instance.ShowOptions(Options);
        }
        public void SelectChoice(DialogChoice choice)
        {
            foreach (DialogChoice Choice in Choices)
            {
                if (Choice == choice)
                {
                    History.Add(new DialogueHistoryEntry(null, true, Choice, Color.red));
                }
                else
                {
                    History.Add(new DialogueHistoryEntry(null, true, Choice));
                }
            }
            //if (choice.EffectID > 0)
            //{
            //    //ExecuteOptionEffect(option.EffectID);
            //}
            //else
            //{
            CurrentIndex = (int)choice.NextDialogIndex;
            DisplayDialog();
        }
        //private void ExecuteOptionEffect(uint effectID)
        //{
        //    Effect effect = NGSStaticData.Effect.GetItem(effectID);
        //    if (effect.QuestGroupID > 0)
        //    {
        //        _questSys.AddQuest(effect.QuestGroupID);

        //    }
        //    //function
        //}

        public List<DialogueHistoryEntry> GetHistory()
        {
            return History;
        }

        public void SetDialogID(uint dialogid)
        {
            throw new NotImplementedException();
        }

        public uint GetDialogID()
        {
            throw new NotImplementedException();
        }

        //private void EndDialogue()
        //{
        //    DialogUI.Instance.HideDialog();
        //}

    }
}