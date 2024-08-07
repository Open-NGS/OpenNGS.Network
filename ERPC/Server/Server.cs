using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenNGS.Network;

namespace OpenNGS.ERPC
{
    public abstract class IMethod
    {
        protected Int32 m_type;
        protected string m_methodName;

        public Int32 Type { get { return m_type; } }
        public string FullName { get { return m_methodName; } }
        internal abstract ServerTransport Transport { set; }

        public abstract void Invoke(ServerContext context, IRequestProtocol reqProto);
    }

    public class PbUnaryMethod<ReqMsg, RspMsg> : IMethod
            where ReqMsg : global::ProtoBuf.IExtensible, new()
            where RspMsg : global::ProtoBuf.IExtensible
    {
        public delegate Task<RspMsg> CallFunc(ServerContext context, ReqMsg req);
        private CallFunc m_call;
        private ServerTransport m_serverTrans;

        public PbUnaryMethod(string name, CallFunc func)
        {
            m_type = CALLTYPE.UNARY;
            m_methodName = name;
            m_call = func;
        }

        internal override ServerTransport Transport { set { m_serverTrans = value; } }

        public override void Invoke(ServerContext context, IRequestProtocol reqProto)
        {
            System.IO.MemoryStream inputStream = new System.IO.MemoryStream(reqProto.Body);
            var req = new ReqMsg();
            ProtoSerializer.Merge(inputStream, req);
            m_call(context, req).ContinueWith(
                rspTask =>
                {
                    try
                    {
                        IResponseProtocol rspProto = new ERPCResponseProtocol();
                        if (rspTask.Status == TaskStatus.RanToCompletion)
                        {
                            RspMsg rspMsg = rspTask.Result;
                            System.IO.MemoryStream ms = new System.IO.MemoryStream();
                            ProtoSerializer.Serialize(ms, rspMsg);
                            rspProto.Body = ms.ToArray();
                            context.Status = new Status(0);
                        }
                        else if (rspTask.Status == TaskStatus.Faulted)
                        {
                            var baseEx = rspTask.Exception.GetBaseException();
                            var irpcEx = baseEx as ERPCException;
                            if (irpcEx == null)
                            {
                                context.Status = new Status(ERRNO.INVALID_PARAM, baseEx.Message);
                            }
                            else
                            {
                                context.Status = new Status(irpcEx.Errno, 0, irpcEx.Message);
                            }
                        }
                        else
                        {
                            context.Status = new Status(ERRNO.SERVER_SYSTEM_ERR, 0, "server canceled");
                        }
                        rspProto.SetContext(context);

                        ServerTransportRsp transRsp;
                        transRsp.endpoint = context.Endpoint;
                        transRsp.rsp = rspProto;
                        m_serverTrans.SendResponse(transRsp);
                    }
                    catch(Exception e)
                    {
                        Log.Info("Send Response fail: " + e.Message + "stack: " + e.StackTrace);
                    }
                });
        }
    }

    public class PbOnewayMethod<ReqMsg> : IMethod
        where ReqMsg : global::ProtoBuf.IExtensible, new()
    {
        public delegate void CallFunc(ServerContext context, ReqMsg req);
        private CallFunc m_call;

        public PbOnewayMethod(string name, CallFunc func)
        {
            m_type = CALLTYPE.ONEWAY;
            m_methodName = name;
            m_call = func;
        }

        internal override ServerTransport Transport { set { } }

        public override void Invoke(ServerContext context, IRequestProtocol reqProto)
        {
            System.IO.MemoryStream inputStream = new System.IO.MemoryStream(reqProto.Body);
            var req = new ReqMsg();
            ProtoSerializer.Merge(inputStream, req);
            m_call(context, req);
        }
    }

    public class RPCService
    {
        private string m_serviceName;
        private Dictionary<string, IMethod> m_methods;
        private ServerTransport m_serverTrans;

        public RPCService(string name)
        {
            m_serviceName = name;
            m_methods = new Dictionary<string, IMethod>();
        }

        public void AddMethod<ReqMsg, RspMsg>(string name, PbUnaryMethod<ReqMsg, RspMsg>.CallFunc func)
            where ReqMsg : global::ProtoBuf.IExtensible, new()
            where RspMsg : global::ProtoBuf.IExtensible
        {
            var unaryMethod = new PbUnaryMethod<ReqMsg, RspMsg>(name, func);
            unaryMethod.Transport = m_serverTrans;
            m_methods.Add(name, unaryMethod);
        }

        public void AddMethod<ReqMsg>(string name, PbOnewayMethod<ReqMsg>.CallFunc func)
            where ReqMsg : global::ProtoBuf.IExtensible, new()
        {
            m_methods.Add(name, new PbOnewayMethod<ReqMsg>(name, func));
        }

        internal ServerTransport Transport
        {
            get { return m_serverTrans; }
            set
            {
                m_serverTrans = value;
                // register handler to server transport
                if (m_serviceName.Length == 0)
                {
                    m_serverTrans.DefaultHandler = this.HandlerMessage;
                }
                else
                {
                    m_serverTrans.SetHandler(m_serviceName, this.HandlerMessage);
                }
                // register transport to method
                foreach (KeyValuePair<string, IMethod> item in m_methods)
                {
                    item.Value.Transport = m_serverTrans;
                }
            }
        }

        internal void HandlerMessage(ServerTransportReq transReq)
        {
            ServerContext context = new ServerContext();
            transReq.req.GetContext(ref context);
            context.Endpoint = transReq.endpoint;

            IMethod method;
            if (m_methods.TryGetValue(transReq.req.FuncName, out method))
            {
                method.Invoke(context, transReq.req);
            }
            else
            {
                Log.Info("cannot find method for: " + transReq.req.FuncName);
            }
        }
    }
}
