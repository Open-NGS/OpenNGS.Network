using OpenNGS.Systems;
using OpenNGS;
using System.Collections.Generic;
using OpenNGS.Dialog.Service;

public class DialogLocalAPI : INgDialogSystem
{
    INgDialogSystem ngDialogSystem = App.GetService<INgDialogSystem>();
    private static DialogLocalAPI _instance;
    private static readonly object _lock = new object();

    private DialogLocalAPI() { }

    public static DialogLocalAPI Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new DialogLocalAPI();
                }
                return _instance;
            }
        }
    }

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
