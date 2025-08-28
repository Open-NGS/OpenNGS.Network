using OpenNGS.ERPC;
using OpenNGS.Statistic.Service;
using OpenNGS.Systems;
using OpenNGS;
using Rpc;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using OpenNGS.Collection.Service;
using OpenNGS.Core;
using static OpenNGS.DataManager;

public class TempCollection : MonoBehaviour
{
    private ClientContext m_clientContext;
    // Start is called before the first frame update
    void Start()
    {
        m_clientContext = new ClientContextBase();
        Debug.Assert(m_clientContext != null, "ClientContext initialization failed!");
        CollectionAPIController.Instance.Init();
        GameInstance.Instance.Init();
        DataManager.Instance.Init(new TableSerializerJson());
    }


    [ContextMenu("TextExample")]
    private async void TextExample()
    {
        Debug.Assert(NgCollectionSystem.Instance != null, "NgCollectionClientSystem.Instance is null");
        Debug.Assert(CollectionAPIController.Instance != null, "NgCollectionLocalAPI.Instance is null");
        //List<OpenNGS.Statistic.Data.StatData> _item = NGSStaticData.teststatic.GetItems(1,1);
        AddCollectionReq addCollectionReq = new AddCollectionReq();
        addCollectionReq.playerID = 1;
        addCollectionReq.collectionID = 2;
        Task<NGSVoid> _task = CollectionServerAPIService.Instance.AddCollection(addCollectionReq, m_clientContext);
        NGSVoid result = await _task;
        //NgCollectionSystem.Instance.AddCollection(addCollectionReq, m_clientContext);
        int a = 0;
    }
    [ContextMenu("TextExample2")]
    private async void TextExample2()
    {
        //List<OpenNGS.Statistic.Data.StatData> _item = NGSStaticData.teststatic.GetItems(1,1);
        GetCollectionsReq getCollectionsReq = new GetCollectionsReq();
        getCollectionsReq.playerID = 1;
        Task<GetCollectionsRsp> _task = CollectionServerAPIService.Instance.GetCollections(getCollectionsReq, m_clientContext);
        GetCollectionsRsp result = await _task;
        //Task<GetCollectionsRsp> _task = NgCollectionSystem.Instance.GetCollections(getCollectionsReq, m_clientContext);
        //GetCollectionsRsp result = await _task;
        int a = 0;
    }
}
