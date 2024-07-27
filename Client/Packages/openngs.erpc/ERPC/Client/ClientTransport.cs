using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenNGS.ERPC
{
    public struct ClientTransportReq
    {
        public Int32 waitMs;
        public Endpoint endpoint;
        public IRequestProtocol req;
    }

    public struct ClientTransportRsp
    {
        public IResponseProtocol rsp;
    }

    public abstract class ClientTransport
    {
        public abstract Task<ClientTransportRsp> AsyncSendRecv(ClientTransportReq req);

        public abstract void SendRequest(ClientTransportReq req);

        public abstract void OnMessage(IRPCMessage msg);
    }

    internal class DefaultClientTransport : ClientTransport
    {
        private MessageSender m_sender;

        private object m_lock;
        private Int64 m_reqSeq;

        private class AsyncSession
        {
            internal System.Threading.Timer timer;
            internal TaskCompletionSource<ClientTransportRsp> complete;
        }
        private Dictionary<UInt64, AsyncSession> m_asyncSesses;

        public DefaultClientTransport(MessageSender sender) : base()
        {
            m_sender = sender;
            m_lock = new object();
            m_reqSeq = 0;
            m_asyncSesses = new Dictionary<ulong, AsyncSession>();
        }

        public override Task<ClientTransportRsp> AsyncSendRecv(ClientTransportReq transReq)
        {
            UInt64 reqID = (UInt64)System.Threading.Interlocked.Increment(ref m_reqSeq);
            transReq.req.ReqID = reqID;

            IRPCMessage msg;
            msg.msg = transReq.req.Encode();
            msg.len = msg.msg.Length;
            msg.type = (Int32)IRPCMessage.MsgType.Request;
            msg.endpoint = transReq.endpoint;
            if (m_sender(msg) == false)
            {
                throw new ERPCException(ERRNO.CLINET_NETWORK_ERR, "Write message fail");
            }

            AsyncSession asyncSess = new AsyncSession()
            {
                timer = new System.Threading.Timer(OnTimeOut, reqID, transReq.waitMs, 0),
                complete = new TaskCompletionSource<ClientTransportRsp>(),
            };
            lock(m_lock)
            {
                m_asyncSesses.Add(reqID, asyncSess);
            }
            return asyncSess.complete.Task;
        }

        public override void SendRequest(ClientTransportReq transReq)
        {
            IRPCMessage msg;
            msg.msg = transReq.req.Encode();
            msg.len = msg.msg.Length;
            msg.type = (Int32)IRPCMessage.MsgType.Request;
            msg.endpoint = transReq.endpoint;
            if (m_sender(msg) == false)
            {
                throw new ERPCException(ERRNO.CLINET_NETWORK_ERR, "Write message fail");
            }
        }

        private void OnTimeOut(Object seq)
        {
            UInt64 reqID = (UInt64)seq;
            AsyncSession asyncSess = null;
            lock(m_lock)
            {
                m_asyncSesses.TryGetValue(reqID, out asyncSess);
                if (asyncSess != null)
                {
                    asyncSess.complete.SetException(new ERPCException(ERRNO.CLIENT_INVOKE_TIMEOUT_ERR, "wait response timeout"));
                    m_asyncSesses.Remove(reqID);
                }
            }
        }

        public override void OnMessage(IRPCMessage msg)
        {
            var rspProto = new ERPCResponseProtocol();
            rspProto.Decode(msg.msg, 0, msg.len);
            UInt64 reqID = rspProto.ReqID;

            AsyncSession asyncSess = null;
            lock(m_lock)
            {
                m_asyncSesses.TryGetValue(reqID, out asyncSess);
                if (asyncSess != null)
                {
                    ClientTransportRsp transRsp;
                    transRsp.rsp = rspProto;
                    asyncSess.complete.SetResult(transRsp);
                    m_asyncSesses.Remove(reqID);
                }
            }
        }
    }
}
