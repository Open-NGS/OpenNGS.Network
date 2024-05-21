using System;
using System.Collections.Generic;
using OpenNGS.Dialog.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Systems
{
    public interface IDialogSystem
    {
        DialogTalk CurrentDialog { get; set; }
        List<DialogChoice> Choices { get; set; }
        public void LoadDialogs(uint dialogId);
        public void SelectChoice(DialogChoice option);
        public void NextDialog();
        public List<DialogueHistoryEntry> GetHistory();
        void SetChoiceEvaluator(IDialogChoiceCondition choiceEvaluator);
        uint GetCurrentDialogID();
    }
}
