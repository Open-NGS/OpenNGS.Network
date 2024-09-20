using OpenNGS;
using OpenNGS.ERPC;
using OpenNGS.Statistic.Service;
using System.Threading.Tasks;

public class StatisticService : Singleton<StatisticService>, INiStatisticService
{
    public Task<AddStatRsp> AddStat(AddStatReq value, ClientContext context = null)
    {
        return StatisticLocalApi.Instance.AddStat(value, context);
    }

    public Task<GetStatDataRsp> GetStatData(GetStatDataReq value, ClientContext context = null)
    {
        throw new System.NotImplementedException();
    }

}
