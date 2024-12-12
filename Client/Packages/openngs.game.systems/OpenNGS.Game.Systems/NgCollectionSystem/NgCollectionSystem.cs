using OpenNGS;
using OpenNGS.Collection.Data;
using OpenNGS.Collection.Service;
using OpenNGS.Core;
using OpenNGS.ERPC;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class NgCollectionSystem : Singleton<NgCollectionSystem>, INiCollectionsService
{
    private CollectionContainer collectionContainer = null;
    public void AddCollectionContainer(CollectionContainer Container)
    {
        if (Container != null)
        {
            collectionContainer = Container;
            if (collectionContainer.UnlockedCollectionIDs == null)
            {
                List<uint> newList = new List<uint>();
                collectionContainer.UnlockedCollectionIDs = newList.ToArray();
            }
        }
        else
        {
            collectionContainer = new CollectionContainer();
        }
    }
    public Task<NGSVoid> AddCollection(AddCollectionReq value, ClientContext context = null)
    {
        if (!collectionContainer.UnlockedCollectionIDs.Contains(value.collectionID))
        {
            List<uint> newList = new List<uint>(collectionContainer.UnlockedCollectionIDs);
            newList.Add(value.collectionID);
            collectionContainer.UnlockedCollectionIDs = newList.ToArray();
        }
        return null;
    }

    public Task<GetCollectionsRsp> GetCollections(GetCollectionsReq value, ClientContext context = null)
    {
        var response = new GetCollectionsRsp
        {
            collections = collectionContainer.UnlockedCollectionIDs.ToArray()
        };
        return Task.FromResult(response);
    }
}
