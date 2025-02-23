using System;
using System.Text;
using OpenNGS.Serialization;
using OpenNGS.Status;
using OpenNGSCommon;
using StatusData = OpenNGSCommon.StatusData;

namespace Systems
{
    public class StatusSystem: OpenNGS.Singleton<StatusSystem>
    {
        private StatusDispatcher m_Dispatcher;

        private IStatusHandler statusHandler;

        public void Init(IStatusHandler handler)
        {
            m_Dispatcher = new StatusDispatcher();
            statusHandler = handler;
            statusHandler.OnStatus += MessageHeadStatusHandler;
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
            if (statusHandler.CheckSvrRetCode(errcode))
            {
                // NgDebug.LogJson("HandleStatusResponse Success", rsp);
                try
                {
                    OnStatus(temp);
                }
                catch (System.Exception e)
                {
                    NgDebug.LogErrorFormat("HandleStatusResponse Exception:{0}", e);
                }
            }
            else
            {
                NgDebug.LogErrorFormat("HandleStatusResponse Failed {0}", (ResultCode)errcode);
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
            NgDebug.Log(sb.ToString());
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
