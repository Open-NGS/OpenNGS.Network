using System;
using System.Collections.Generic;using OpenNGS.UI;
using UnityEngine;

public interface IViewModel
{
    void Init();
    void Open();
    void Close();
}

public class ViewModelBase : IViewModel
{
    public delegate void NotifyViewHandler(string command, params object[] args);

    private event NotifyViewHandler m_Handler;

    // call this method in view
    public void RegisterNotifyHandler(NotifyViewHandler handler)
    {
        m_Handler += handler;
    }
    public void UnregisterNotifyHandler(NotifyViewHandler handler)
    {
        m_Handler -= handler;
    }
    protected void NotifyView(string command, params object[] args)
    {
        if (m_Handler != null)
        {
            m_Handler(command, args);
        }
        else
        {
            Debug.LogError("notify handler not register");
        }
    }

    public virtual void Init() { }
    public virtual void Close() { }
    public virtual void Open() { }

}
