using System;
using System.Collections.Generic;

namespace OpenNGS.ERPC
{
    public class ClientContext
    {
        private Status m_status;
        private UInt64 m_reqID;
        private int m_timeout;
        private int m_callType;
        private int m_messageType;
        private int m_contextType;
        private string m_funcName;
        private string m_caller;
        private string m_callee;
        private Dictionary<string, string> m_reqMeta;
        private Dictionary<string,  byte[]> m_rspMeta;
        private Endpoint m_endpoint;
        private string m_actionName;
        private Action<byte[]> m_rspAction; 

        public ClientContext()
        {
            m_timeout = 5000;
            m_callType = CALLTYPE.UNARY;
            m_contextType = CONTENTTYPE.PB;
            m_caller = string.Empty;
            m_callee = string.Empty;
            m_reqMeta = new Dictionary<string, string>();
            m_rspMeta = new Dictionary<string,  byte[]>();
        }

        public Status Status { get { return m_status; } set { m_status = value; } }
        public UInt64 RequestID { get { return m_reqID; } set { m_reqID = value; } }
        public int Timeout { get { return m_timeout; } set { m_timeout = value; } }
        public int CallType { get { return m_callType; } set { m_callType = value; } }
        public int MessageType { get { return m_messageType; } set { m_messageType = value; } }
        public int ContentType { get { return m_contextType; } set { m_contextType = value; } }
        public string FuncName { get { return m_funcName; } set { m_funcName = value; } }
        public string Caller { get { return m_caller; } set { m_caller = value; } }
        public string Callee { get { return m_callee; } set { m_callee = value; } }
        public Endpoint Endpoint { get { return m_endpoint; } set { m_endpoint = value; } }

        public Dictionary<string, string> ReqMeta
        {
            get { return m_reqMeta; }
            set { m_reqMeta = value; }
        }
        public Dictionary<string, byte[]> RspMeta
        {
            get { return m_rspMeta; }
            set { m_rspMeta = value; }
        }
 
        public void SetService(string name)
        {
            m_endpoint.targetName = name;
            m_endpoint.targetID = UInt64.MaxValue;
            m_endpoint.routeInfo = string.Empty;
        }

        public void SetServiceInst(string name, UInt64 id)
        {
            m_endpoint.targetName = name;
            m_endpoint.targetID = id;
            m_endpoint.routeInfo = string.Empty;
        }

        public void SetRouteInfo(string info)
        {
            m_endpoint.routeInfo = info;
        }

        public void SetAction(string name, Action<byte[]> action)
        {
            m_actionName = name;
            m_rspAction = action;
        }

        public void InvokeAction()
        {
            if (RspMeta.TryGetValue(m_actionName, out byte[] val))
            {
                m_rspAction?.Invoke(val);
            }
        }
    }
}
