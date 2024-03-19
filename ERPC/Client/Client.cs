using System;
using System.Threading.Tasks;
using OpenNGS.Network;

namespace OpenNGS.ERPC
{
    public class RPCClient
    {
        private ClientTransport m_clientTrans;

        public RPCClient()
        {
            m_clientTrans = null;
        }

        public Task<RspMsg> UnaryInvoke<ReqMsg, RspMsg>(ClientContext context, ReqMsg reqMsg)
            where ReqMsg : global::ProtoBuf.IExtensible
            where RspMsg : global::ProtoBuf.IExtensible, new()
        {
            IRequestProtocol reqProto = new ERPCRequestProtocol();
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ProtoSerializer.Serialize(ms, reqMsg);
            reqProto.Body = ms.ToArray();
            return this.AsyncInvoke(context, reqProto).ContinueWith<RspMsg>(
                invokeTask =>
                {
                    // task exception
                    if (invokeTask.Status == TaskStatus.Faulted)
                    {
                        throw invokeTask.Exception.GetBaseException();
                    }
                    // not complete
                    if (invokeTask.Status != TaskStatus.RanToCompletion)
                    {
                        throw new ERPCException(ERRNO.CLIENT_SYSTEM_ERR, "unknown error");
                    }
                    IResponseProtocol invokeRsp = invokeTask.Result;
                    var rspMsg = new RspMsg();

                    global::ProtoBuf.Serializer.Merge(new System.IO.MemoryStream(invokeRsp.Body), rspMsg);

                    context.InvokeAction();

                    return rspMsg;
                });
        }

        public void OnewayInvoke<ReqMsg>(ClientContext context, ReqMsg reqMsg)
                        where ReqMsg : global::ProtoBuf.IExtensible
        {
            if (m_clientTrans == null)
            {
                throw new ERPCException(ERRNO.CLIENT_SYSTEM_ERR, "rpc client not init");
            }
            if (context.CallType != CALLTYPE.ONEWAY)
            {
                throw new ERPCException(ERRNO.INVALID_PARAM, "context is not oneway");
            }
            IRequestProtocol reqProto = new ERPCRequestProtocol();
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ProtoSerializer.Serialize(ms, reqMsg);
            reqProto.Body = ms.ToArray();
            reqProto.SetContext(context);

            ClientTransportReq transReq;
            transReq.waitMs = context.Timeout;
            transReq.endpoint = context.Endpoint;
            transReq.req = reqProto;
            m_clientTrans.SendRequest(transReq);
        }

        internal ClientTransport Transport
        {
            get { return m_clientTrans; }
            set { m_clientTrans = value; }
        }

        internal Task<IResponseProtocol> AsyncInvoke(ClientContext context, IRequestProtocol reqProto)
        {
            if (m_clientTrans == null)
            {
                return Task.FromResult<IResponseProtocol>(null);
            }
            reqProto.SetContext(context);

            ClientTransportReq transReq;
            transReq.waitMs = context.Timeout;
            transReq.endpoint = context.Endpoint;
            transReq.req = reqProto;
            return m_clientTrans.AsyncSendRecv(transReq).ContinueWith<IResponseProtocol>(
                transTask =>
                {
                    // task exception
                    if (transTask.Status == TaskStatus.Faulted)
                    {
                        throw transTask.Exception.GetBaseException();
                    }
                    // not complete
                    if (transTask.Status != TaskStatus.RanToCompletion)
                    {
                        throw new ERPCException(ERRNO.CLIENT_INVOKE_ASYNC_NET_FAIL, "Request is not complete");
                    }
                    // fetch result
                    ClientTransportRsp transRsp = transTask.Result;
                    transRsp.rsp.GetContext(ref context);
                    return transRsp.rsp;
                });
        }
    }
}
