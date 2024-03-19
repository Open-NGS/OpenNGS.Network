using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MissQ.Tools;
using OpenNGS.Extension;
using OpenNGS.Network;
using OpenNGS.Serialization;
using OpenNGSCommon;
using ProtoBuf;
using UnityEngine;
using StatusData = OpenNGSCommon.StatusData;

namespace Systems
{
    public class StatusSystem: OpenNGS.Singleton<StatusSystem>
    {
        private StatusDispatcher m_Dispatcher;

        public void Init()
        {
            m_Dispatcher = new StatusDispatcher();
            NetworkModule.Instance.onMessageHeadStatus += MessageHeadStatusHandler;
        }

        public bool Register(string systemName, Action<StatusData> callBack)
        {
            return m_Dispatcher.Register(systemName, callBack);
        }
   
        public void UnRegisterSystem(string systemName)
        {
            m_Dispatcher.UnRegisterSystem(systemName);
        }

        public void HandleStatusResponse(int errcode, OpenNGS.IProtoExtension rsp)
        {
            StatusDataList temp = (StatusDataList)rsp;
            string msg = string.Empty;
            if (NetworkModule.Instance.CheckSvrRetCode(errcode))
            {
                // NgDebug.LogJson("HandleStatusResponse Success", rsp);
                try
                {
                    OnStatus(temp);
                }
                catch (System.Exception e)
                {
                    Debug.LogErrorFormat("HandleStatusResponse Exception:{0}", e);
                }
            }
            else
            {
                Debug.LogErrorFormat("HandleStatusResponse Failed {0}", (ResultCode)errcode);
            }
        }
        
        public void OnStatus(StatusDataList status)
        {
            NgDebug.LogJson("StatusSystem OnStatus", status);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"StatusSystem:;OnStatus[{status.status_datas.Count}]");
            foreach (var data in status.status_datas)
            {
                if (data == null) continue;
                sb.AppendFormat("[{0}]{1}[{2}]", data.SystemName, data.OpCode, data.Datas.Count);
                m_Dispatcher.Dispatch(data);
                sb.AppendLine();
            }
            
            TimeHelper.RefreshServerTime((uint)status.timestamp);
            Debug.Log(sb.ToString());
        }

        public void Update()
        {
            if (m_Dispatcher != null)
                m_Dispatcher.Update();
        }

        private void MessageHeadStatusHandler(byte[] data)
        {
            OnStatus(FileSerializer.Deserialize<StatusDataList>(data));
        }
    }
}
