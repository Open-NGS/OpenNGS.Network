using OpenNGS.Collections.Generic;
using OpenNGS.Dialog.Common;
using OpenNGS.Dialog.Data;
using System;
using System.Collections.Generic;
using Systems;
using UnityEngine;

namespace OpenNGS.Systems
{
    public interface IDialogChoiceCondition
    {
        bool EvaluateCondition(DialogChoice Choice);
    }
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
        private IQuestSystem _questSys;
        public List<DialogTalk> DialogTalks = new List<DialogTalk>();
        public List<DialogChoice> Choices = new List<DialogChoice>();
        public List<DialogueHistoryEntry> History = new List<DialogueHistoryEntry>();
        public DialogTalk CurrentDialog;
        private int CurrentIndex;
        private uint m_nDialogID;
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

        private IDialogChoiceCondition _choiceEvaluator;

        public void SetChoiceEvaluator(IDialogChoiceCondition choiceEvaluator)
        {
            _choiceEvaluator = choiceEvaluator;
        }
        protected override void OnCreate()
        {
            _questSys = App.GetService<IQuestSystem>();
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
            m_nDialogID = dialogId;
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
        public OpenNGS.Dialog.Common.DIALOG_TYPE GetDialogType()
        {
            DialogList dialog = NGSStaticData.Dialogue.GetItem(m_nDialogID);
            return (DIALOG_TYPE)dialog.Type;
        }
        private void DisplayDialog()
        {
            if (CurrentIndex < DialogTalks.Count)
            {
                CurrentDialog = DialogTalks[CurrentIndex];
                History.Add(new DialogueHistoryEntry(CurrentDialog));
                if (CurrentDialog.ChoiceIDs != null)
                {
                    uint[] choices = CurrentDialog.ChoiceIDs;
                    ShowChoice(choices);
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
            //if (Choice.DialogChoiceCondition == DIALOG_CHOICE_CONDITION_TYPE.DIALOG_CHOICE_CONDITION_TYPE_NONE)
            //{
            //    return true;
            //}
            if (_choiceEvaluator == null)
            {
                throw new InvalidOperationException("DialogChoiceEvaluator not set.");
            }
            else
            {
                return _choiceEvaluator.EvaluateCondition(Choice);
            }
        }

        public uint GetCurrentDialogID()
        {
            return m_nDialogID;
        }

        public void ShowChoice(uint[] options)
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
            if (choice.QuestGroupID>0)
            {
                _questSys.AddQuestGroup(choice.QuestGroupID);
                _questSys.StartQuest(choice.QuestGroupID);
            }
            CurrentIndex = (int)choice.NextDialogIndex;
            DisplayDialog();
        }

        public List<DialogueHistoryEntry> GetHistory()
        {
            return History;
        }
    }
}