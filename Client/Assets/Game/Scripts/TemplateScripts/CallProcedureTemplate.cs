using Google.Protobuf.WellKnownTypes;
using OpenNGS;
using OpenNGS.ERPC;
using OpenNGS.ERPC.Configuration;
using OpenNGS.Statistic.Service;
using OpenNGS.Systems;
using Rpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

public class CallProcedureTemplate : MonoBehaviour
{
    private ClientContext m_clientContext;
    // Start is called before the first frame update
    void Start()
    {
        m_clientContext = new ClientContextBase();
        StatisticLocalApi.Instance.Init();
        GameInstance.Instance.Init();
        NGSStaticData.Init();
        DataManager.Instance.Init();
    }


    [ContextMenu("TextExample")]
    private async void TextExample()
    {
        List<OpenNGS.Statistic.Data.StatData> _item = NGSStaticData.teststatic.GetItems(1,1);
        int a = 0;
        //Task<AddStatRsp> _task = StatisticClientSystem.Instance.AddStat(new AddStatReq(), m_clientContext);
        //AddStatRsp result = await _task;
        //int a = 0;
    }

}
