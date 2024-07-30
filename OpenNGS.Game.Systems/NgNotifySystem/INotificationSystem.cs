using OpenNGS.UI;
using OpenNGSCommon;
using System;
using System.Collections;
using System.Collections.Generic;

public interface INotificationSystem
{
    public void RegisterTipsContainer(ITipsContainer tipContainer);
    public void RegisterMsgBoxContanier(IMessageBoxContainer msgBoxContanier);

    public void ShowNotify(uint notifyId, params object[] args);
    public void ShowNotify(ResultCode result, params object[] args);
    public IMessageTips ShowTips(TipsPositions pos, string text);
    public IMessageTips ShowTips(TipsPositions pos, uint messageId, params object[] args);
    public void CloseTips(IMessageTips tips);
    public IMessageBox ShowMessageBox(string title, string content, string option, MessageBoxButtons style = MessageBoxButtons.OKCancel, Action<IMessageBox> onResult = null);
    public IMessageBox ShowMessageBox(string content, string title, MessageBoxButtons style = MessageBoxButtons.OKCancel, Action<IMessageBox> onResult = null);
    public IMessageBox ShowMessageBox(uint messageId, MessageBoxButtons style = MessageBoxButtons.OKCancel, Action<IMessageBox> onResult = null, params object[] args);
    public void ShowErrorMessage(uint messageId, ResultCode errorCode, MessageBoxButtons style = MessageBoxButtons.OKCancel, Action<IMessageBox> onResult = null, params object[] args);
    public void CloseMessageBox();
}
