using OpenNGS.Collection.Data;
using OpenNGS.Collection.Service;
using OpenNGS.Core;
using OpenNGS.ERPC;
using System.Threading.Tasks;


namespace OpenNGS.Systems
{
    public interface INgCollectionSystem
    {
        public void AddCollectionContainer(CollectionContainer Container);
        public Task<NGSVoid> AddCollection(AddCollectionReq value, ClientContext context = null);
        public Task<GetCollectionsRsp> GetCollections(GetCollectionsReq value, ClientContext context = null);
    }
}
