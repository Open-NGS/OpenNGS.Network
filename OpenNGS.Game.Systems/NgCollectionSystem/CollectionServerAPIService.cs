using OpenNGS;
using OpenNGS.Collection.Service;
using OpenNGS.Core;
using OpenNGS.ERPC;
using System.Threading.Tasks;

public class CollectionServerAPIService : Singleton<CollectionServerAPIService>, INiCollectionsService
{

    public async Task<NGSVoid> AddCollection(AddCollectionReq value, ClientContext context = null)
    {
        return await CollectionAPIController.Instance.AddCollection(value, context);
    }

    public async Task<GetCollectionsRsp> GetCollections(GetCollectionsReq value, ClientContext context = null)
    {
        return await CollectionAPIController.Instance.GetCollections(value, context);
    }
}
