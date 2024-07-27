

namespace OpenNGS.Platform
{
    public interface IAppModule : IPlatfromModule
    {
        public NRequest<PlatformData> AsyncInitializeApp(string appId = null);
        public void SyncInitializeApp(string appId = null);
        public bool IsAppInitScc();
        public NRequest<PlatformData> IsUserEntitiledToApp();
    }
}
