using OpenNGS;
using protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{

    public class MessageContext
    {

        public event Action<int, IProtoExtension> Handler;
        public MessageContext()
        {

        }
        public MessageContext(Action<int, IProtoExtension> callback)
        {
            this.Handler += callback;
        }


        public void OnResponse(int errcode, IProtoExtension rsp)
        {
            if (this.Handler != null)
                this.Handler(errcode, rsp);
        }

        public void Reset()
        {
            this.Handler = null;
        }

    }

    public class MessageService : OpenNGS.Singleton<MessageService>
    {
#if true
        public MessageContext SendMessage(SCSPkg pkg)
        {
    
           // MessageContext context = new MessageContext();
           // pkg.body.rspcontext = context;
            NetworkModule.Instance.SendRequest(pkg);

            /*为了兼容demo现有逻辑，暂时保留现有回调机制，但这部分逻辑会有时序问题，SendRequest可能会失败，
            失败时会立即调用回调并返回Fail，但此时回调事件并没有被设置，并且pkg已经被回收对象池，这时body会是null
            暂时判断下防止报错，过后统一使用反射协议回调管线*/
            if (pkg.body != null)
                return pkg.body.rspcontext;
            else
            {
                MessageContext context = new MessageContext();
                return context;
            }

        }
#else

        public MessageContext<TRES> SendMessage<TREQ, TRES>(ECSOpcode op, TREQ req) 
            where TREQ : global::ProtoBuf.IExtensible
            where TRES : global::ProtoBuf.IExtensible
        {
            MessageContext<TRES> context = new MessageContext<TRES>();
            NetworkModule.mInstance.SendRequest<TREQ, TRES>(op, req, context.OnResponse);
            return context;
        }
#endif
    }
}
