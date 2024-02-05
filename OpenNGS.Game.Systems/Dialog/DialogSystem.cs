using OpenNGS.Dialog.Data;
using System.Collections.Generic;
using Systems;
using UnityEngine;
using UnityEngine.Events;

namespace OpenNGS.Systems
{
    public class DialogSystem : GameSubSystem<DialogSystem>, IDialogSystem
    {
        private ISaveSystem m_saveSystem;
        private SaveFileData_Dialog m_dialogData;
        protected override void OnCreate()
        {
            m_saveSystem = App.GetService<ISaveSystem>();
            LoadDialogData();
            base.OnCreate();
        }

        public override string GetSystemName()
        {
            return "com.openngs.system.dialogue";
        }

        public void SetDialogID(int dialogID)
        {
            m_dialogData.DialogDataID = dialogID;
            SaveDialogData();
        }

        public int GetDialogID()
        {
            return m_dialogData.DialogDataID;
        }

        private void LoadDialogData()
        {
            ISaveInfo saveInfo = m_saveSystem.GetFileData("DIALOG");
            if (saveInfo != null && saveInfo is SaveFileData_Dialog)
            {
                m_dialogData = (SaveFileData_Dialog)saveInfo;
            }
            else
            {
                // 如果没有保存数据，创建一个新的对话数据
                m_dialogData = new SaveFileData_Dialog();
            }
        }

        private void SaveDialogData()
        {
            m_saveSystem.SetFileData("DIALOG", m_dialogData);
            m_saveSystem.SaveFile();
        }

        //#region C2S
        //public void RequestDialogue(uint dialogueID)
        //{
        //    // 模拟异步获取对话数据

        //}
        //#endregion

        //#region S2C
        //public void OnDialogueResultReceived(DialogueInstanceArray dialogueResult)
        //{
        //    OnDialogueResult?.Invoke(dialogueResult);
        //    //模拟服务器数据，并调用UI函数
        //}
        //#endregion
    }
}
