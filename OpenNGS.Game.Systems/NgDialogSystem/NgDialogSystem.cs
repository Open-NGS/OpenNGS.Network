using OpenNGS.Dialog.Data;
using OpenNGS.Item.Data;
using OpenNGS.Systems;
using System.Collections;
using System.Collections.Generic;
using Systems;
using UnityEngine;
using static UnityEditor.Progress;

namespace OpenNGS.Systems
{
    public class NgDialogSystem : GameSubSystem<NgDialogSystem>, INgDialogSystem
    {
        private List<DialogTalk> DialogTalks = new List<DialogTalk>();
        private List<DialogChoice> Choices = new List<DialogChoice>();
        private List<object> History = new List<object>();
        private DialogTalk CurrentDialog;
        private int CurrentIndex;

        protected override void OnCreate()
        {
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
        public uint GetDialogDisplayType(uint dialogId)
        {
            return NGSStaticData.Dialogue.GetItem(dialogId).DisplayType;
        }

        // 加载对话
        public DialogTalk LoadDialogs(uint dialogId)
        {
            DialogTalks.Clear();
            History.Clear();
            CurrentIndex = 0;
            uint[] dialogDataIds = NGSStaticData.Dialogue.GetItem(dialogId).DialogTalkIDs;
            foreach (uint dialogDataId in dialogDataIds)
            {
                DialogTalk dialogData = NGSStaticData.DialogTalk.GetItem(dialogDataId);
                DialogTalks.Add(dialogData);
            }
            DisplayDialog();
            return CurrentDialog;
        }

        // 选择选项
        public DialogTalk SelectChoice(DialogChoice choice)
        {
            if (Choices.Contains(choice))
            {
                choice.IsSelected = true;
                CurrentIndex = (int)choice.NextDialogIndex;
                DisplayDialog();
            }
            return CurrentDialog;
        }

        // 下一对话
        public DialogTalk NextDialog()
        {
            CurrentIndex++;
            DisplayDialog();
            return CurrentDialog;
        }

        // 内部方法: 显示对话
        private void DisplayDialog()
        {
            if (CurrentIndex < DialogTalks.Count)
            {
                CurrentDialog = DialogTalks[CurrentIndex];
                History.Add(CurrentDialog);
                Choices.Clear();
                if (CurrentDialog.ChoiceIDs != null)
                {
                    uint[] choices = CurrentDialog.ChoiceIDs;
                    foreach (uint choiceId in choices)
                    {
                        DialogChoice choice = NGSStaticData.Choice.GetItem(choiceId);
                        Choices.Add(choice);
                    }
                    History.Add(Choices);
                }
            }
        }


        // 获取对话历史
        public List<object> GetHistory()
        {
            return History;
        }

        //public void DisplayHistory()
        //{
        //    DialogSystem dialogSystem = // 获取DialogSystem实例
        //    List<object> history = dialogSystem.GetHistory();

        //    foreach (var entry in history)
        //    {
        //        if (entry is DialogChoice choice)
        //        {
        //            Color choiceColor = choice.IsSelected ? Color.red : Color.white;
        //            Debug.Log($"Choice: {choice.Text} (Color: {choiceColor})");
        //        }
        //        else if (entry is DialogTalk dialog)
        //        {
        //            Debug.Log($"Dialog: {dialog.Text}");
        //        }
        //    }
        //}


    }
}