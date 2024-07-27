using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using Google.Protobuf;

namespace OpenNGS.ERPC
{

    public abstract class IRequestProtocol
    {
        public abstract byte[] Encode();
        public abstract void Decode(byte[] buff, int offset, int len);

        public abstract void SetContext(ClientContext context);
        public abstract void GetContext(ref ServerContext context);

        internal abstract UInt64 ReqID { get; set; }
        internal abstract string FuncName { get; }
        public abstract byte[] Body { get; set; }
    }

    public abstract class IResponseProtocol
    {
        public abstract byte[] Encode();
        public abstract void Decode(byte[] buff, int offset, int len);

        public abstract void SetContext(ServerContext context);
        public abstract void GetContext(ref ClientContext context);

        internal abstract UInt64 ReqID { get; set; }
        public abstract byte[] Body { get; set; }
    }

    internal struct ERPCProtocolHeader
    {
        public const UInt16 ERPC_MAGIC_REQ = 0x14A0;
        public const UInt16 ERPC_MAGIC_RSP = 0x14A1;
        public const int ERPC_FIXED_SIZE = 16;

        public UInt16 magic;
        public UInt16 headSize;
        public UInt32 pkgSize;
        public UInt32 streamID;
        public UInt32 reserve;

        internal void Encode(ref MemoryStream s)
        {
            var writer = new BinaryWriter(s);
            writer.Write(IPAddress.HostToNetworkOrder((Int16)magic));
            writer.Write(IPAddress.HostToNetworkOrder((Int16)headSize));
            writer.Write(IPAddress.HostToNetworkOrder((Int32)pkgSize));
            writer.Write(IPAddress.HostToNetworkOrder((Int32)streamID));
            writer.Write(IPAddress.HostToNetworkOrder((Int32)reserve));
        }

        internal void Decode(MemoryStream s)
        {
            var reader = new BinaryReader(s);
            magic = (UInt16)IPAddress.NetworkToHostOrder(reader.ReadInt16());
            headSize = (UInt16)IPAddress.NetworkToHostOrder(reader.ReadInt16());
            pkgSize = (UInt32)IPAddress.NetworkToHostOrder(reader.ReadInt32());
            streamID = (UInt32)IPAddress.NetworkToHostOrder(reader.ReadInt32());
            reserve = (UInt32)IPAddress.NetworkToHostOrder(reader.ReadInt32());
        }
    }

    internal class ERPCRequestProtocol : IRequestProtocol
    {
        private ERPCProtocolHeader m_fixHeader;
        private RequestProtocol m_pbHead = null;
        private byte[] m_body = null;

        public ERPCRequestProtocol()
        {
            m_fixHeader = new ERPCProtocolHeader
            {
                magic = ERPCProtocolHeader.ERPC_MAGIC_REQ,
                headSize = 0,
                pkgSize = 0,
                streamID = 0,
            };
        }

        public override byte[] Encode()
        {
            if (m_pbHead == null || m_body == null)
            {
                throw new ERPCException(ERRNO.CLIENT_ENCODE_ERR, "context or body not set");
            }
            var totalSize = m_body.Length + m_pbHead.CalculateSize() + ERPCProtocolHeader.ERPC_FIXED_SIZE;
            var buff = new MemoryStream(totalSize);
            // fix header
            m_fixHeader.headSize = (UInt16)m_pbHead.CalculateSize();
            m_fixHeader.pkgSize = (UInt32)totalSize;
            m_fixHeader.Encode(ref buff);
            // pb header
            var pbStream = new CodedOutputStream(buff);
            m_pbHead.WriteTo(pbStream);
            pbStream.Flush();
            // body
            buff.Write(m_body, 0, m_body.Length);
            return buff.GetBuffer();
        }

        public override void Decode(byte[] buff, int offset, int len)
        {
            var memStream = new MemoryStream(buff, offset, len);
            // fix header
            m_fixHeader.Decode(memStream);
            if (m_fixHeader.magic != ERPCProtocolHeader.ERPC_MAGIC_REQ || m_fixHeader.pkgSize != len)
            {
                throw new ERPCException(ERRNO.SERVER_DECODE_ERR, "message mask invalid");
            }
            if (m_fixHeader.pkgSize < m_fixHeader.headSize + ERPCProtocolHeader.ERPC_FIXED_SIZE)
            {
                throw new ERPCException(ERRNO.SERVER_DECODE_ERR, "invalid pkg size");
            }
            offset += ERPCProtocolHeader.ERPC_FIXED_SIZE;
            // pb header
            m_pbHead = new RequestProtocol();
            m_pbHead.MergeFrom(new CodedInputStream(buff, offset, m_fixHeader.headSize));
            offset += m_fixHeader.headSize;
            // body
            var bodySize = m_fixHeader.pkgSize - m_fixHeader.headSize - ERPCProtocolHeader.ERPC_FIXED_SIZE;
            m_body = new byte[bodySize];
            Buffer.BlockCopy(buff, offset, m_body, 0, (int)bodySize);
        }

        public override void SetContext(ClientContext context)
        {
            m_pbHead = new RequestProtocol();
            m_pbHead.CallType = context.CallType;
            m_pbHead.Timeout = (UInt32)context.Timeout;
            m_pbHead.RequestId = context.RequestID;
            m_pbHead.MessageType = context.MessageType;
            m_pbHead.ContentType = context.ContentType;
            m_pbHead.Func = context.FuncName;
            m_pbHead.Caller = context.Caller;
            m_pbHead.Callee = context.Callee;
            foreach (KeyValuePair<string, string> item in context.ReqMeta)
            {
                m_pbHead.Meta.Add(item.Key, Google.Protobuf.ByteString.CopyFromUtf8(item.Value));
            }
        }

        public override void GetContext(ref ServerContext context)
        {
            context.CallType = m_pbHead.CallType;
            context.Timeout = (Int32)m_pbHead.Timeout;
            context.RequestID = m_pbHead.RequestId;
            context.MessageType = m_pbHead.MessageType;
            context.ContentType = m_pbHead.ContentType;
            context.FuncName = m_pbHead.Func;
            context.Caller = m_pbHead.Caller;
            context.Callee = m_pbHead.Callee;
            foreach (KeyValuePair<string, ByteString> item in m_pbHead.Meta)
            {
                context.ReqMeta.Add(item.Key, item.Value.ToStringUtf8());
            }
        }

        internal override UInt64 ReqID
        {
            get { return m_pbHead.RequestId; }
            set { m_pbHead.RequestId = value; }
        }

        internal override string FuncName
        {
            get { return m_pbHead.Func; }
        }

        public override byte[] Body { get { return m_body; } set { m_body = value; } }
    }

    internal class ERPCResponseProtocol : IResponseProtocol
    {
        private ERPCProtocolHeader m_fixHeader;
        private ResponseProtocol m_pbHead = null;
        private byte[] m_body = null;

        public ERPCResponseProtocol()
        {
            m_fixHeader = new ERPCProtocolHeader
            {
                magic = ERPCProtocolHeader.ERPC_MAGIC_RSP,
                headSize = 0,
                pkgSize = 0,
                streamID = 0,
            };
        }

        public override byte[] Encode()
        {
            if (m_pbHead == null || m_body == null)
            {
                throw new ERPCException(ERRNO.SERVER_ENCODE_ERR, "context or body not set");
            }
            var totalSize = m_body.Length + m_pbHead.CalculateSize() + ERPCProtocolHeader.ERPC_FIXED_SIZE;
            var buff = new MemoryStream(totalSize);
            // fix header
            m_fixHeader.headSize = (UInt16)m_pbHead.CalculateSize();
            m_fixHeader.pkgSize = (UInt32)totalSize;
            m_fixHeader.Encode(ref buff);
            // pb header
            var pbStream = new CodedOutputStream(buff);
            m_pbHead.WriteTo(pbStream);
            pbStream.Flush();
            // body
            buff.Write(m_body, 0, m_body.Length);
            return buff.ToArray();
        }
        public override void Decode(byte[] buff, int offset, int len)
        {
            var memStream = new MemoryStream(buff, offset, len);
            // fix header
            m_fixHeader.Decode(memStream);
            if (m_fixHeader.magic != ERPCProtocolHeader.ERPC_MAGIC_RSP || m_fixHeader.pkgSize != len)
            {
                throw new ERPCException(ERRNO.CLIENT_DECODE_ERR, "message mask invalid");
            }
            if (m_fixHeader.pkgSize < m_fixHeader.headSize + ERPCProtocolHeader.ERPC_FIXED_SIZE)
            {
                throw new ERPCException(ERRNO.CLIENT_DECODE_ERR, "invalid pkg size");
            }
            offset += ERPCProtocolHeader.ERPC_FIXED_SIZE;
            // pb header
            m_pbHead = new ResponseProtocol();
            m_pbHead.MergeFrom(new CodedInputStream(buff, offset, m_fixHeader.headSize));
            offset += m_fixHeader.headSize;
            // body
            var bodySize = m_fixHeader.pkgSize - m_fixHeader.headSize - ERPCProtocolHeader.ERPC_FIXED_SIZE;
            m_body = new byte[bodySize];
            Buffer.BlockCopy(buff, offset, m_body, 0, (int)bodySize);
        }

        public override void SetContext(ServerContext context)
        {
            m_pbHead = new ResponseProtocol();
            m_pbHead.Ret = context.Status.Result;
            m_pbHead.FuncRet = context.Status.Code;
            m_pbHead.ErrorMsg = context.Status.Message;
            m_pbHead.RequestId = context.RequestID;
            m_pbHead.MessageType = context.MessageType;
            m_pbHead.ContentType = context.ContentType;
            foreach (KeyValuePair<string, string> item in context.RspMeta)
            {
                m_pbHead.Meta.Add(item.Key, Google.Protobuf.ByteString.CopyFromUtf8(item.Value));
            }
        }

        public override void GetContext(ref ClientContext context)
        {
            Status st;
            st.Result = m_pbHead.Ret;
            st.Code = m_pbHead.FuncRet;
            st.Message = m_pbHead.ErrorMsg;
            context.Status = st;
            foreach (KeyValuePair<string, ByteString> item in m_pbHead.Meta)
            {
                context.RspMeta.Add(item.Key, item.Value.ToByteArray());
            }
        }

        internal override UInt64 ReqID
        {
            get { return m_pbHead.RequestId; }
            set { m_pbHead.RequestId = value; }
        }

        public override byte[] Body { get { return m_body; } set { m_body = value; } }
    }

}
