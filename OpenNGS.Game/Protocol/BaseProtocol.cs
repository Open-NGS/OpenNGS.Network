using System;
using OpenNGS.Network;
using OpenNGS;

namespace protocol
{
    //默认包体，整合了原有的request，scshead结构
    public class SCSPkg : IProtoExtension
    {
        //实际协议包的包头，由pb序列化反序列化
        public MsgHead head;

        //消息体，包含消息包基础描述信息和实际协议包中的pb结构体
        public SCSBody body;

        public static readonly int ClassID = 0;

        public int GetClassID() { return ClassID; }


        public static SCSPkg New<T>() where T : IProtoExtension
        {
            return New(typeof(T));
        }
        //根据给定pb消息类标号构建对应消息包,一般用于请求协议包
        public static SCSPkg New(Type type)
        {
            SCSPkg pkg = (SCSPkg)ProtoPool.Instance.Get(type);

            pkg.body.AssignProtoBuffClass(type);

            return pkg;
        }

        //创建默认消息包，body中的pb结构由clientextend中自动生成的factory接口填充
        public static SCSPkg New()
        {
            SCSPkg pkg = (SCSPkg)ProtoPool.Instance.Get(typeof(SCSPkg));
            return pkg;
        }

        public void Clear()
        {

        }

        public void OnSpawn()
        {
            head = (MsgHead)ProtoPool.Instance.Get(typeof(MsgHead));
            body = (SCSBody)ProtoPool.Instance.Get(typeof(SCSBody));
        }

        public void OnRelease()
        {
            ProtoPool.Instance.Release(head);
            head = null;

            ProtoPool.Instance.Release(body);
            body = null;
        }

    }

    //默认协议包体，对应原有的request结构
    public class SCSBody : IProtoExtension
    {
        public uint sequence;
        // 请求操作
        public Opcode op;
        // 请求数据
        public IProtoExtension req;

        //response消息对应的结构类型
        public Type rsp_type;
        //response消息操作
        public Opcode rsp_op;
        // 请求回调
        public Services.MessageContext rspcontext;
        // 发送级别
        //public RequestLevel reqLvl = RequestLevel.NotResend;
        // 是否已经发送
        public bool isSend;

        public static readonly int ClassID = 1;

        public int GetClassID() { return ClassID; }
        public void AssignProtoBuffClass(Type type)
        {
            req = ProtoPool.Instance.Get(type);
        }

        public void Clear()
        {

        }

        public void OnSpawn()
        {
            if (rspcontext == null)
                rspcontext = new Services.MessageContext();
        }

        public void OnRelease()
        {
            ProtoPool.Instance.Release(req);

            rspcontext?.Reset();
            
            req = null;
        }
    }
}
