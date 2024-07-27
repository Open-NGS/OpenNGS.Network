using System;
using System.Collections.Generic;

namespace OpenNGS.ERPC
{
    public class ServerContext
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
        private Dictionary<string, string> m_rspMeta;
        private Endpoint m_endpoint;

        public ServerContext()
        {
            m_reqMeta = new Dictionary<string, string>();
            m_rspMeta = new Dictionary<string, string>();
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

        public Dictionary<string, string> ReqMeta
        {
            get { return m_reqMeta; }
            set { m_reqMeta = value; }
        }
        public Dictionary<string, string> RspMeta
        {
            get { return m_rspMeta; }
            set { m_rspMeta = value; }
        }

        internal Endpoint Endpoint { get { return m_endpoint; } set { m_endpoint = value; } }
    }
}
