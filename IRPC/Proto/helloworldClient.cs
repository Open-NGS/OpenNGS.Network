using helloworld;
using OpenNGS.IRPC;
using OpenNGS.IRPC.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace helloworld
{
    class Greeter : IGreeter
    {
        RPCClient client;
        public Greeter(RPCClient client)
        {
            this.client = client;
        }
        
        public Task<HelloReply> SayHello(HelloRequest value, ClientContext context = null)
        {
            if(context !=null)
            {
                ServiceAttribute sa = typeof(IGreeter).GetCustomAttribute<ServiceAttribute>(true);
                context.FuncName = "/" + sa.Name + "/" + MethodBase.GetCurrentMethod().Name;
            }
            return this.client.UnaryInvoke<helloworld.HelloRequest, helloworld.HelloReply>(context, value);
        }

        public void SayHelloOneWay(HelloRequest value, ClientContext context = null)
        {
            this.client.OnewayInvoke<helloworld.HelloRequest>(context, value);
        }
    }
}
