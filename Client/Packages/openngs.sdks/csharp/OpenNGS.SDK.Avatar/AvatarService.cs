using OpenNGS.SDK.Core;

namespace OpenNGS.SDK.Avatar
{
    public class AvatarService
    {
        static IAvatarService s_Instance;

        public static IAvatarService Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    throw new ServicesInitializationException("Singleton is not initialized. " +
                        "Please call OpenNGSPlatformServices.Initialize() to initialize.");
                }
                return s_Instance;
            }
            set => s_Instance = value;
        }
    }
}
