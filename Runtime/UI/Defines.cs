using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenNGS.UI
{

    public enum UIVisiblity
    {
        Normal = 0, //正常状态


    }
    public enum UISystemType
    {
        /// <summary>
        /// UGUI
        /// Default UI System
        /// </summary>
        UGUI = 0,

        /// <summary>
        /// FairyGUI System
        /// </summary>
        FairyGUI = 1,

        /// <summary>
        /// UIWidgets
        /// </summary>
        UIWidgets = 2,
    }


    public enum DialogResult
    {
        /// <summary>
        /// Nothing is returned from the dialog box. This means that the modal dialog continues running.
        /// </summary>
        None = 0,

        /// <summary>
        /// The dialog box return value is OK (usually sent from a button labeled OK).
        /// </summary>
        OK = 1,

        /// <summary>
        /// The dialog box return value is Cancel (usually sent from a button labeled Cancel).
        /// </summary>
        Cancel = 2,

        /// <summary>
        /// The dialog box return value is Abort (usually sent from a button labeled Abort).
        /// </summary>
        Abort = 3,

        /// <summary>
        /// The dialog box return value is Retry (usually sent from a button labeled Retry).
        /// </summary>
        Retry = 4,

        /// <summary>
        /// The dialog box return value is Ignore (usually sent from a button labeled Ignore).
        /// </summary>
        Ignore = 5,

        /// <summary>
        /// The dialog box return value is Yes (usually sent from a button labeled Yes).
        /// </summary>
        Yes = 6,

        /// <summary>
        /// The dialog box return value is No (usually sent from a button labeled No).
        /// </summary>
        No = 7,
    }

    /// <summary>
    /// Tips Position
    /// 10 positions preset.
    /// </summary>
    public enum TipsPositions
    {
        PositionAll = -1,
        Position0 = 1,
        Position1 = Position0 << 1,
        Position2 = Position0 << 2,
        Position3 = Position0 << 3,
        Position4 = Position0 << 4,
        Position5 = Position0 << 5,
        Position6 = Position0 << 6,
        Position7 = Position0 << 7,
    }

    public enum TipsType
    {
        /// <summary>
        /// 普通提示
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 跑马灯
        /// </summary>
        Marquee = 1,
    }

    public enum MessageBoxButtons
    {
        OK = 0,
        OKCancel = 1,
        AbortRetryIgnore = 2,
        YesNoCancel = 3,
        YesNo = 4,
        RetryCancel = 5,
    }

    public enum MessageContextMenuType
    {
        Buttons,
        ToggleGroup,
    }

    public interface ITipsContainer
    {
        IMessageTips ShowTips(TipsType type, string content, TipsPositions position, float speed, int loop);
        void CloseTips(IMessageTips tips);
    }

    public interface IMessageBoxContainer
    {
        IMessageBox ShowMessageBox(string title, string content, MessageBoxButtons buttons, Action<IMessageBox> onResult);
        void CloseMessageBox();
    }

    public interface IMessageToastContainer
    {
        IMessageToast ShowToast(string content, float showDuration, float fadeDuration);
    }

    public interface IMessageContextMenuContainer
    {
        IMessageContextMenu ShowContextMenu();
    }


    public interface INotification
    {
        void Close();
    }

    public interface IMessageTips : INotification
    {

    }

    public interface IMessageToast : INotification
    {
        
    }

    public interface IMessageContextMenu : INotification
    {
        
    }

    public interface IMessageBox : INotification
    {
        DialogResult DialogResult { get; }
        void SetOption(bool selected);
        int SelectOption { get; }
    }

    public interface IUISystem
    {
        void InitSystem();
        IView CreateView(int id, string package, string component, int layer, bool cache);
    }
}
