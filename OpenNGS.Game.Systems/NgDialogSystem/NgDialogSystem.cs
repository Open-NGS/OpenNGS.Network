using OpenNGS.Dialog.Data;
using OpenNGS.Dialog.Service;
using System.Collections.Generic;
using Systems;

namespace OpenNGS.Systems
{
    public class NgDialogSystem : GameSubSystem<NgDialogSystem>, INgDialogSystem
    {
        private List<DialogTalk> DialogTalks = new List<DialogTalk>();
        //private List<DialogChoice> Choices = new List<DialogChoice>();
        private List<uint> History = new List<uint>();
        private uint CurrentTalkID;
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
        // 加载对话
        public LoadDialogRsp LoadDialogs(uint dialogId)
        {
            LoadDialogRsp loadDialogRsp = new LoadDialogRsp();
            DialogTalks.Clear();
            History.Clear();
            CurrentIndex = 0;
            uint[] dialogDataIds = DialogStaticData.Dialogue.GetItem(dialogId).DialogTalkIDs;
            foreach (uint dialogDataId in dialogDataIds)
            {
                DialogTalk dialogData = DialogStaticData.DialogTalk.GetItem(dialogDataId);
                DialogTalks.Add(dialogData);
            }
            DisplayDialog();
            loadDialogRsp.DialogTalkID = CurrentTalkID;
            return loadDialogRsp;
        }

        // 选择选项
        public LoadDialogRsp SelectChoice(ChoiceRep _choiceRep)
        {
            //DialogChoice choice = _choiceRep.ChoiceRepValue;
            //LoadDialogRsp loadDialogRsp = new LoadDialogRsp();
            //if (Choices.Contains(choice))
            //{
            //    choice.IsSelected = true;
            //    CurrentIndex = (int)choice.NextDialogIndex;
            //    DisplayDialog();
            //}
            //loadDialogRsp.Talk = CurrentDialog;
            //loadDialogRsp.Choice = Choices;
            //return loadDialogRsp;

            uint[] choices = _choiceRep.SelectChoiceIDs;
            DialogChoice _dialogChoice;
            foreach (uint choiceId in choices)
            {
                _dialogChoice = DialogStaticData.Choice.GetItem(choiceId);
                if(_dialogChoice == null)
                {
                    NgDebug.LogErrorFormat("Can not found DialogChoice : {0}", choiceId);
                    continue;
                }
                if(_dialogChoice.NextDialogIndex != 0)
                {
                    CurrentIndex = (int)_dialogChoice.NextDialogIndex;
                    break;
                }
            }
            DisplayDialog();
            LoadDialogRsp loadDialogRsp = new LoadDialogRsp();
            loadDialogRsp.DialogTalkID = CurrentTalkID;
            return loadDialogRsp;
        }

        // 下一对话
        public LoadDialogRsp NextDialog()
        {
            LoadDialogRsp loadDialogRsp = new LoadDialogRsp();
            CurrentIndex++;
            DisplayDialog();
            loadDialogRsp.DialogTalkID = CurrentTalkID;
            //loadDialogRsp.Choice = Choices;
            return loadDialogRsp;
        }

        // 内部方法: 显示对话
        private void DisplayDialog()
        {
            if (CurrentIndex < DialogTalks.Count)
            {
                DialogTalk _talk = DialogTalks[CurrentIndex];
                CurrentTalkID = DialogTalks[CurrentIndex].DialogTalkID;
                History.Add(DialogTalks[CurrentIndex].DialogTalkID);
                //Choices.Clear();
                //if (CurrentTalkID.ChoiceIDs != null)
                //{
                //    uint[] choices = CurrentTalkID.ChoiceIDs;
                //    foreach (uint choiceId in choices)
                //    {
                //        DialogChoice choice = NGSStaticData.Choice.GetItem(choiceId);
                //        Choices.Add(choice);
                //    }
                //    History.Add(Choices);
                //}
            }
        }


        // 获取对话历史
        public List<uint> GetHistory()
        {
            return History;
        }
    }
}