using OpenNGS.Dialog.Data;
using OpenNGS.Dialog.Service;
using System.Collections.Generic;
using Systems;

namespace OpenNGS.Systems
{
    public class NgDialogSystem : GameSubSystem<NgDialogSystem>, INgDialogSystem
    {
        private List<DialogTalkConfig> DialogTalks = new List<DialogTalkConfig>();
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
            return loadDialogRsp;
        }

        // 选择选项
        public LoadDialogRsp SelectChoice(ChoiceRep _choiceRep)
        {
            //uint[] choices = _choiceRep.SelectChoiceIDs;
            //DialogChoice _dialogChoice;
            //foreach (uint choiceId in choices)
            //{
            //    _dialogChoice = DialogStaticData.Choice.GetItem(choiceId);
            //    if(_dialogChoice == null)
            //    {
            //        NgDebug.LogErrorFormat("Can not found DialogChoice : {0}", choiceId);
            //        continue;
            //    }
            //    if(_dialogChoice.NextDialogIndex != 0)
            //    {
            //        CurrentIndex = (int)_dialogChoice.NextDialogIndex;
            //        break;
            //    }
            //}
            //DisplayDialog();
            LoadDialogRsp loadDialogRsp = new LoadDialogRsp();
            loadDialogRsp.DialogTalkID = CurrentTalkID;
            return loadDialogRsp;
        }

        // 下一对话
        public LoadDialogRsp NextDialog()
        {
            LoadDialogRsp loadDialogRsp = new LoadDialogRsp();
            return loadDialogRsp;
        }

        // 内部方法: 显示对话
        private void DisplayDialog()
        {
            if (CurrentIndex < DialogTalks.Count)
            {
                //DialogTalkConfig _talk = DialogTalks[CurrentIndex];
                //CurrentTalkID = DialogTalks[CurrentIndex].TalkID;
                //History.Add(DialogTalks[CurrentIndex].TalkID);
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