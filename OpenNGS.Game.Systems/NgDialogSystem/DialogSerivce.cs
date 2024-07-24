using OpenNGS.Systems;
using OpenNGS;
using System.Collections.Generic;
using OpenNGS.Dialog.Service;

public class DialogSerivce : Singleton<DialogSerivce>,INgDialogSystem
{
    INgDialogSystem ngDialogSystem = App.GetService<INgDialogSystem>();

    public uint GetDialogDisplayType(uint dialogId)
    {
        return ngDialogSystem.GetDialogDisplayType(dialogId);
    }

    public LoadDialogRsp LoadDialogs(uint dialogId)
    {
        return ngDialogSystem.LoadDialogs(dialogId);
    }

    public LoadDialogRsp SelectChoice(ChoiceRep _choiceRep)
    {
        return ngDialogSystem.SelectChoice(_choiceRep);
    }

    public LoadDialogRsp NextDialog()
    {
        return ngDialogSystem.NextDialog();
    }

    public List<object> GetHistory()
    {
        return ngDialogSystem.GetHistory();
    }
}
