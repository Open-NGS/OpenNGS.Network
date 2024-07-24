using OpenNGS.Dialog.Data;
using OpenNGS.Dialog.Service;
using System.Collections.Generic;

namespace OpenNGS.Systems
{
    public interface INgDialogSystem
    {
        public uint GetDialogDisplayType(uint dialogId);
        public LoadDialogRsp LoadDialogs(uint dialogId);
        public LoadDialogRsp SelectChoice(ChoiceRep _choiceRep);
        public LoadDialogRsp NextDialog();
        public List<DialogData> GetHistory();
    }

}
