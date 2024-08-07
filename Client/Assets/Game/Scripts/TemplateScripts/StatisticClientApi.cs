using OpenNGS;
using OpenNGS.ERPC;
using OpenNGS.Statistic.Service;
using System.Threading.Tasks;

public class StatisticClientApi : Singleton<StatisticClientApi>, INiStatisticService
{
    public Task<AddStatRsp> AddStat(AddStatReq value, ClientContext context = null)
    {
        throw new System.NotImplementedException();
    }

    public Task<GetStatDataRsp> GetStatData(GetStatDataReq value, ClientContext context = null)
    {
        throw new System.NotImplementedException();
    }
}
