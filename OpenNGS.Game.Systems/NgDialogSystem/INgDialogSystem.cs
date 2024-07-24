using OpenNGS.Dialog.Data;
using System.Collections.Generic;

namespace OpenNGS.Systems
{
    public interface INgDialogSystem
    {
        public uint GetDialogDisplayType(uint dialogId);
        public DialogTalk LoadDialogs(uint dialogId);
        public DialogTalk SelectChoice(DialogChoice DialogChoice);
        public DialogTalk NextDialog();
        //public List<DialogData> GetHistory();
    }

}
