using System.Reflection;
using System.Threading.Tasks;
using OpenNGS.ERPC;
using OpenNGS.ERPC.Configuration;
using Rpc.Game;

namespace Rpc
{
    public class RedPointClient : IRedPointService
    {
        private readonly RPCClient _client;
        string _name;

        public RedPointClient(RPCClient client, string name)
        {
            _client = client;
            _name = name;
        }

        public Task<GetRedPointsRsp> GetRedPoints(GetRedPointsReq value, ClientContext context = default(ClientContext))
        {
            ServiceAttribute sa = typeof(IRedPointService).GetCustomAttribute<ServiceAttribute>(true);
            context.FuncName = "/" + sa.Name + "/" + MethodBase.GetCurrentMethod().Name;
            context.SetService(_name);
            return this._client.UnaryInvoke<GetRedPointsReq, GetRedPointsRsp>(context, value);
        }
    }
}