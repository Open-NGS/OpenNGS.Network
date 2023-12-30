using System.Reflection;
using System.Threading.Tasks;
using OpenNGS.IRPC;
using OpenNGS.IRPC.Configuration;
using Rpc.Game;

namespace Rpc
{
    public class UserSettingClient : IUserSettingService
    {
        private readonly RPCClient _client;
        string _name;

        public UserSettingClient(RPCClient client, string name)
        {
            _client = client;
            _name = name;
        }

        public Task<GetUserSettingRsp> GetUserSetting(GetUserSettingReq value, ClientContext context = default(ClientContext))
        {
            ServiceAttribute sa = typeof(IUserSettingService).GetCustomAttribute<ServiceAttribute>(true);
            context.FuncName = "/" + sa.Name + "/" + MethodBase.GetCurrentMethod().Name;
            context.SetService(_name);
            return this._client.UnaryInvoke<GetUserSettingReq, GetUserSettingRsp>(context, value);
        }

        public Task<UpdateUserSettingRsp> UpdateUserSetting(UpdateUserSettingReq value, ClientContext context = default(ClientContext))
        {
            ServiceAttribute sa = typeof(IUserSettingService).GetCustomAttribute<ServiceAttribute>(true);
            context.FuncName = "/" + sa.Name + "/" + MethodBase.GetCurrentMethod().Name;
            context.SetService(_name);
            return this._client.UnaryInvoke<UpdateUserSettingReq, UpdateUserSettingRsp>(context, value);
        }
    }
}