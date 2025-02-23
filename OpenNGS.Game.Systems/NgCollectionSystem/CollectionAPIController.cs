using OpenNGS;
using OpenNGS.Collection.Data;
using OpenNGS.Collection.Service;
using OpenNGS.Core;
using OpenNGS.ERPC;
using OpenNGS.ERPC.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;


public class CollectionAPIController : Singleton<CollectionAPIController>, INiCollectionsService
{
    //private List<uint> CollectionIDs = new List<uint>();
    private Dictionary<uint, List<uint>> CollectionIDs = new Dictionary<uint, List<uint>>();

    IRPCLite m_rpcLite;
    RPCService m_service;
    private RPCClient m_cli;
    public void Init()
    {
        m_cli = new RPCClient();
        m_rpcLite = new IRPCLite();
        m_rpcLite.Init(MessageSender);
        m_rpcLite.InitClient(m_cli);
        System.Type _insType = typeof(INiCollectionsService);
        ServiceAttribute sa = _insType.GetCustomAttribute<ServiceAttribute>(true);
        m_service = new RPCService(sa.Name);
        m_rpcLite.InitService(m_service);
        m_service.AddMethod<AddCollectionReq, AddCollectionRsp>(sa.Name + "/" + "AddCollection", ActionAddCollectionReq);
        m_service.AddMethod<GetCollectionsReq, GetCollectionsRsp>(sa.Name + "/" + "GetCollections", ActionGetCollectionsReq);
    }
    public bool MessageSender(IRPCMessage msg)
    {
        m_rpcLite.OnMessage(msg);
        return true;
    }
    // 添加图鉴的方法
    private async Task<AddCollectionRsp> ActionAddCollectionReq(ServerContext context, AddCollectionReq req)
    {
        await Task.Delay(100);
        AddCollectionRsp rsp = new AddCollectionRsp();
        try
        {
            var playerCollections = GetOrCreatePlayerCollections(req.playerID);
            if (!playerCollections.Contains(req.collectionID))
            {
                playerCollections.Add(req.collectionID);
            }
            rsp.success = true;
        }
        catch (Exception ex)
        {
            NgDebug.LogError($"Error adding CollectionID {req.collectionID}: {ex.Message}");
            rsp.success = false;
        }
        return rsp;
    }
    // 获取图鉴的方法
    private async Task<GetCollectionsRsp> ActionGetCollectionsReq(ServerContext context, GetCollectionsReq req)
    {
        await Task.Delay(100);
        // 处理获取图鉴的逻辑
        GetCollectionsRsp rsp = new GetCollectionsRsp();
        try
        {
            // 假设从某个存储获取所有图鉴ID
            //rsp.collections = new List<uint>(CollectionIDs).ToArray();
            rsp.collections = CollectionIDs.ContainsKey(req.playerID)
                ? CollectionIDs[req.playerID].ToArray()
                : Array.Empty<uint>();
        }
        catch
        {

        }
        return rsp;
    }
    public partial class AddCollectionRsp : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public AddCollectionRsp()
        {
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1, IsPacked = true)]
        public bool success { get; set; }
    }

    private List<uint> GetOrCreatePlayerCollections(uint playerID)
    {
        if (!CollectionIDs.ContainsKey(playerID))
        {
            CollectionIDs[playerID] = new List<uint>();
        }
        return CollectionIDs[playerID];
    }
    /// <summary>
    /// 添加图鉴到服务端
    /// </summary>
    public Task<NGSVoid> AddCollection(AddCollectionReq req, ClientContext context = null)
    {
        if (context != null)
        {
            ServiceAttribute sa = typeof(INiCollectionsService).GetCustomAttribute<ServiceAttribute>(true);
            context.FuncName = sa.Name + "/" + MethodBase.GetCurrentMethod().Name;
        }

        try
        {
            // 调用 RPC 方法
            var response = m_cli.UnaryInvoke<AddCollectionReq, AddCollectionRsp>(context, req);

            if (response != null)
            {
                var playerCollections = GetOrCreatePlayerCollections(req.playerID);
                if (!playerCollections.Contains(req.collectionID))
                {
                    playerCollections.Add(req.collectionID);
                    NgDebug.Log($"CollectionID {req.collectionID} added to local cache.");
                }
            }
        }
        catch (Exception ex)
        {
            NgDebug.LogError($"AddCollection failed: {ex.Message}");
        }

        return Task.FromResult(new NGSVoid());
    }

    /// <summary>
    /// 获取玩家已解锁的图鉴列表
    /// </summary>
    public Task<GetCollectionsRsp> GetCollections(GetCollectionsReq value, ClientContext context = null)
    {
        if (context != null)
        {
            ServiceAttribute sa = typeof(INiCollectionsService).GetCustomAttribute<ServiceAttribute>(true);
            context.FuncName = sa.Name + "/" + MethodBase.GetCurrentMethod().Name;
        }

        try
        {
            // 调用 RPC 方法获取数据
            return m_cli.UnaryInvoke<GetCollectionsReq, GetCollectionsRsp>(context, value);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetCollections failed: {ex.Message}");
            return null;
        }
    }
}
