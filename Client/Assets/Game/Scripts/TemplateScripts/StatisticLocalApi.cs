using OpenNGS;
using OpenNGS.ERPC;
using OpenNGS.ERPC.Configuration;
using OpenNGS.Statistic.Common;
using OpenNGS.Statistic.Service;
using OpenNGS.Systems;
using System.Reflection;
using System.Threading.Tasks;

public class StatisticLocalApi : Singleton<StatisticLocalApi>, INiStatisticService
{
    IRPCLite m_rpcLite;
    RPCService m_service;
    private RPCClient m_cli;
    public void Init()
    {
        m_cli = new RPCClient();
        m_rpcLite = new IRPCLite();
        m_rpcLite.Init(MessageSender);
        m_rpcLite.InitClient(m_cli);
        System.Type _insType = typeof(INiStatisticService);
        ServiceAttribute sa = _insType.GetCustomAttribute<ServiceAttribute>(true);
        m_service = new RPCService(sa.Name);
        m_rpcLite.InitService(m_service);
        m_service.AddMethod<AddStatReq, AddStatRsp>(sa.Name + "/" + "AddStat", ActionAddStatReq);
    }
    private async Task<AddStatRsp> ActionAddStatReq(ServerContext context, AddStatReq req)
    {
        await Task.Delay(100);
        AddStatRsp _rsp = new AddStatRsp();
        //INgStatisticSystem _statisticSys = App.GetService<INgStatisticSystem>();
        //if (_statisticSys != null)
        if(true)
        {
            //foreach(AddStatValue _val in value.stats)
            //{
            //    _statisticSys.StatByStatisticID(_val.id, _val.val);
            //    StatValue _statVal = new StatValue();
            //    _statVal.id = _val.id;
            //    _statVal.totalval = (ulong)_statisticSys.GetStat(_val.id);
            //    _rsp.stats.Add(_statVal);
            //}
            _rsp.result = 1;
            StatValue _statVal = new StatValue();
            _statVal.id = 1;
            _statVal.totalval = 25;
            _rsp.stats.Add(_statVal);
        }
        return _rsp;
    }
    public Task<AddStatRsp> AddStat(AddStatReq value, ClientContext context = null)
    {
        if (context != null)
        {
            ServiceAttribute sa = typeof(INiStatisticService).GetCustomAttribute<ServiceAttribute>(true);
            context.FuncName = sa.Name + "/" + MethodBase.GetCurrentMethod().Name;
        }
        return this.m_cli.UnaryInvoke<AddStatReq, AddStatRsp>(context, value);
    }

    public Task<GetStatDataRsp> GetStatData(GetStatDataReq value, ClientContext context = null)
    {
        throw new System.NotImplementedException();
    }
    public bool MessageSender(IRPCMessage msg)
    {
        m_rpcLite.OnMessage(msg);
        return true;
    }
}
