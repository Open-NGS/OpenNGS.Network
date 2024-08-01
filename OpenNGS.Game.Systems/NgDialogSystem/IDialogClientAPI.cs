using OpenNGS.Dialog.Service;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.Systems
{
    public interface IDialogClientAPI
    {
        public uint GetDialogDisplayType(uint dialogId);
        public LoadDialogRsp LoadDialogs(uint dialogId);
        public LoadDialogRsp SelectChoice(ChoiceRep _choiceRep);
        public LoadDialogRsp NextDialog();
        public List<uint> GetHistory();
    }
}
