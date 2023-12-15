using System;

namespace OpenNGS.IRPC
{

    public struct IRPCMessage
    {
        public enum MsgType {
            Request = 3,
            Response = 4,
        }

        public byte[] msg;
        public Int32 len;
        public Int32 type;
        public Endpoint endpoint;
    }

    public delegate bool MessageSender(IRPCMessage msg);

    public class IRPCLite
    {
        private ClientTransport m_clientTrans;
        private ServerTransport m_serverTrans;

        public IRPCLite()
        {
        }

        public void Init(MessageSender sender)
        {
            Log.Debug("Init IRPCLite");
            m_clientTrans = new DefaultClientTransport(sender);
            m_serverTrans = new DefaultServerTransport(sender);
        }

        public void InitClient(RPCClient rpcClient)
        {
            rpcClient.Transport = m_clientTrans;
        }

        public void InitService(RPCService rpcService)
        {
            rpcService.Transport = m_serverTrans;
        }

        public void OnMessage(IRPCMessage msg)
        {
            if (msg.type == (Int32)IRPCMessage.MsgType.Request)
            {
                m_serverTrans.OnMessage(msg);
            }
            if (msg.type == (Int32)IRPCMessage.MsgType.Response)
            {
                m_clientTrans.OnMessage(msg);
            }
        }
    }

}
