using OpenNGS.Systems;
using OpenNGS;
using System.Collections;
using System.Collections.Generic;
using Systems;
using OpenNGSCommon;
using OpenNGS.UI;
using System;

public class NotificationSystem : GameSubSystem<NotificationSystem>, INotificationSystem
{
    ITipsContainer m_tipsContainer;
    IMessageBoxContainer m_msgContainer;

    protected override void OnCreate()
    {
        base.OnCreate();
    }

    public void RegisterTipsContainer(ITipsContainer tipContainer)
    {
        m_tipsContainer = tipContainer;
    }

    public void RegisterMsgBoxContanier(IMessageBoxContainer msgBoxContanier)
    {
        m_msgContainer = msgBoxContanier;
    }

    public void ShowNotify(uint notifyId, params object[] args)
    {
        
    }

    public void ShowNotify(ResultCode result, params object[] args)
    {
        
    }

    public IMessageTips ShowTips(TipsPositions pos, string text)
    {
        IMessageTips tip = null;
        if (m_tipsContainer != null)
        {
            tip = m_tipsContainer.ShowTips(TipsType.Normal, text, pos, 0, 0);
        }
        return tip;
    }

    public IMessageTips ShowTips(TipsPositions pos, uint messageId, params object[] args)
    {
        return null;
    }

    public void CloseTips(IMessageTips tips)
    {
        if (m_tipsContainer != null)
        {
            m_tipsContainer.CloseTips(tips);
        }
    }

    public IMessageBox ShowMessageBox(string title, string content, string option, MessageBoxButtons style = MessageBoxButtons.OKCancel, Action<IMessageBox> onResult = null)
    {
        return null;
    }

    public IMessageBox ShowMessageBox(string content, string title, MessageBoxButtons style = MessageBoxButtons.OKCancel, Action<IMessageBox> onResult = null)
    {
        IMessageBox message = null;
        if (m_msgContainer != null)
        {
            message = m_msgContainer.ShowMessageBox(title, content, style, onResult);
        }
        return message;
    }

    public IMessageBox ShowMessageBox(uint messageId, MessageBoxButtons style = MessageBoxButtons.OKCancel, Action<IMessageBox> onResult = null, params object[] args)
    {
        IMessageBox message = null;
        if (m_msgContainer != null)
        {
            message = m_msgContainer.ShowMessageBox(messageId, style, onResult, args);
        }
        return message;
    }

    public void ShowErrorMessage(uint messageId, ResultCode errorCode, MessageBoxButtons style = MessageBoxButtons.OKCancel, Action<IMessageBox> onResult = null, params object[] args)
    {
        
    }

    public void CloseMessageBox()
    {
        if (m_msgContainer != null)
        {
            m_msgContainer.CloseMessageBox();
        }
    }

    public override string GetSystemName()
    {
        return "openngs.system.messagesystem";
    }

    protected override void OnClear()
    {
        m_tipsContainer = null; 
        m_msgContainer = null;
        base.OnClear();
    }
}
