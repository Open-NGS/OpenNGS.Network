using System.Reflection;
using System.Threading.Tasks;
using OpenNGS.IRPC;
using OpenNGS.IRPC.Configuration;
using protocol;
using Rpc.Game;

namespace Rpc
{
    public class ObjectiveRewardClient : IObjectiveService
    {
        private readonly RPCClient _client;
        string _name;

        public ObjectiveRewardClient(RPCClient client, string name)
        {
            _client = client;
            _name = name;
        }


        public Task<GetObjectivesRsp> GetObjectives(OpenNGSCommon.GetRequest value, ClientContext context = default(ClientContext))
        {
            ServiceAttribute sa = typeof(IObjectiveService).GetCustomAttribute<ServiceAttribute>(true);
            context.FuncName = "/" + sa.Name + "/" + MethodBase.GetCurrentMethod().Name;
            context.SetService(_name);
            return this._client.UnaryInvoke<OpenNGSCommon.GetRequest, GetObjectivesRsp>(context, value);
        }

        public Task<ObjectiveRewardRsp> ObjectiveReward(ObjectiveRewardReq value, ClientContext context = default(ClientContext))
        {
            ServiceAttribute sa = typeof(IObjectiveService).GetCustomAttribute<ServiceAttribute>(true);
            context.FuncName = "/" + sa.Name + "/" + MethodBase.GetCurrentMethod().Name;
            context.SetService(_name);
            return this._client.UnaryInvoke<ObjectiveRewardReq, ObjectiveRewardRsp>(context, value);
        }
    }
}
