using OpenNGS.Dialog.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OpenNGS.Systems
{
    public class DialogueSystem : EntitySystem
    {
        public UnityAction<DialogueInstanceArray> OnDialogueResult;
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        public override string GetSystemName()
        {
            return "com.openngs.system.dialogue";
        }

        #region C2S
        public void RequestDialogue(uint dialogueID)
        {
            // 模拟异步获取对话数据

        }
        #endregion

        #region S2C
        public void OnDialogueResultReceived(DialogueInstanceArray dialogueResult)
        {
            OnDialogueResult?.Invoke(dialogueResult);
            //模拟服务器数据，并调用UI函数
        }
        #endregion
    }
}
