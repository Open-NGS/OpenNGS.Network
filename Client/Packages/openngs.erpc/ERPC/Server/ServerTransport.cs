using System;
using System.Collections.Generic;

namespace OpenNGS.ERPC
{
    public struct ServerTransportReq
    {
        public Endpoint endpoint;
        public IRequestProtocol req;
    }

    public struct ServerTransportRsp
    {
        public Endpoint endpoint;
        public IResponseProtocol rsp;
    }

    public delegate void MessageHandler(ServerTransportReq transReq);

    public abstract class ServerTransport
    {
        internal abstract MessageHandler DefaultHandler { get; set; }

        public abstract void SetHandler(string name, MessageHandler handler);

        public abstract void SendResponse(ServerTransportRsp transRsp);

        public abstract void OnMessage(IRPCMessage msg);
    }

    internal class DefaultServerTransport : ServerTransport
    {
        private MessageSender m_sender;
        private MessageHandler m_defaultHandler;
        private Dictionary<string, MessageHandler> m_handlers;

        public DefaultServerTransport(MessageSender sender) : base()
        {
            m_sender = sender;
            m_defaultHandler = null;
            m_handlers = new Dictionary<string, MessageHandler>();
        }

        internal override MessageHandler DefaultHandler
        {
            get { return m_defaultHandler; }
            set { m_defaultHandler = value; }
        }

        public override void SetHandler(string name, MessageHandler handler)
        {
            m_handlers.Add(name, handler);
        }

        public override void SendResponse(ServerTransportRsp transRsp)
        {
            IRPCMessage msg;
            msg.msg = transRsp.rsp.Encode();
            msg.len = msg.msg.Length;
            msg.type = (Int32)IRPCMessage.MsgType.Response;
            msg.endpoint = transRsp.endpoint;
            if (m_sender(msg) == false)
            {
                throw new ERPCException(ERRNO.SERVER_NETWORK_ERR, "Write message fail");
            }
        }

        public override void OnMessage(IRPCMessage msg)
        {
            var reqProto = new ERPCRequestProtocol();
            reqProto.Decode(msg.msg, 0, msg.len);
            MessageHandler handler = GetHandler(reqProto.FuncName);
            if (handler == null)
            {
                Log.Info("cannot deal func: " + reqProto.FuncName);
                return;
            }

            ServerTransportReq transReq;
            transReq.endpoint = msg.endpoint;
            transReq.req = reqProto;
            try
            {
                handler(transReq);
            }
            catch(Exception e)
            {
                Log.Info("handler request fail: " + e.Message);
            }
        }

        private MessageHandler GetHandler(string funcName)
        {
            // return only default handler
            if (m_defaultHandler != null && m_handlers.Count == 0)
            {
                return m_defaultHandler;
            }
            // return only handler
            if (m_defaultHandler == null && m_handlers.Count == 1)
            {
                var enumerator = m_handlers.GetEnumerator();
                enumerator.MoveNext();
                return enumerator.Current.Value;
            }
            // find handler
            int splitPos = funcName.LastIndexOf('/');
            string serviceName = funcName.Substring(0, splitPos);
            MessageHandler handler = null;
            if (m_handlers.TryGetValue(serviceName, out handler))
            {
                return handler;
            }
            return m_defaultHandler;
        }
    }
}
