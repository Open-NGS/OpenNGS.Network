using OpenNGS;
using OpenNGS.ERPC;
using OpenNGS.Statistic.Service;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class StatisticClientSystem : Singleton<StatisticClientSystem>, INiStatisticService
{
    public Task<AddStatRsp> AddStat(AddStatReq value, ClientContext context = null)
    {
        return StatisticService.Instance.AddStat(new AddStatReq(), context);
    }

    public Task<GetStatDataRsp> GetStatData(GetStatDataReq value, ClientContext context = null)
    {
        throw new System.NotImplementedException();
    }
}
