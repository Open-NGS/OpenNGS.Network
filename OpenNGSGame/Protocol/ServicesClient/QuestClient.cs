using Common;
using OpenNGS.IRPC;
using OpenNGS.IRPC.Configuration;
using System.Reflection;
using System.Threading.Tasks;
using protocol;
using Rpc.Game;

namespace Rpc
{
    public class QuestClient : IQuestSerivce
    {
        RPCClient client;
        string name;
        public QuestClient(RPCClient client, string name)
        {
            this.client = client;
            this.name = name;
        }

        public Task<GetQuestGroupRewardRsp> GetQuestGroupReward(GetQuestGroupRewardReq value, ClientContext context = default(ClientContext))
        {
            ServiceAttribute sa = typeof(IQuestSerivce).GetCustomAttribute<ServiceAttribute>(true);
            context.FuncName = "/" + sa.Name + "/" + MethodBase.GetCurrentMethod().Name;
            context.SetService(name);
            return this.client.UnaryInvoke<GetQuestGroupRewardReq, GetQuestGroupRewardRsp>(context, value);
        }

        public Task<GetQuestsRsp> GetQuests(OpenNGSCommon.GetRequest value, ClientContext context = default(ClientContext))
        {
            ServiceAttribute sa = typeof(IQuestSerivce).GetCustomAttribute<ServiceAttribute>(true);
            context.FuncName = "/" + sa.Name + "/" + MethodBase.GetCurrentMethod().Name;
            context.SetService(name);
            return this.client.UnaryInvoke<OpenNGSCommon.GetRequest, GetQuestsRsp>(context, value);
        }

        public Task<GetQuestRewardRsp> GetQuestReward(GetQuestRewardReq value, ClientContext context = default(ClientContext))
        {
            ServiceAttribute sa = typeof(IQuestSerivce).GetCustomAttribute<ServiceAttribute>(true);
            context.FuncName = "/" + sa.Name + "/" + MethodBase.GetCurrentMethod().Name;
            context.SetService(name);
            return this.client.UnaryInvoke<GetQuestRewardReq, GetQuestRewardRsp>(context, value);
        }
    }
}
