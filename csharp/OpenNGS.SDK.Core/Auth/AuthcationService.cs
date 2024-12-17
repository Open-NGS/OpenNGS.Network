using OpenNGS.SDK.Core;

namespace OpenNGS.SDK.Auth
{
    public static class AuthcationService
    {
        static IAuthenticationService s_Instance;

        public static IAuthenticationService Instance
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
