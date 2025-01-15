
using System;

namespace OpenNGS.Platform
{
    public class PlatformData
    {
        private OPENNGS_PLAT_RESULT result;

        public OPENNGS_PLAT_RESULT Result
        {
            get { return result; }
            set { result = value; }
        }

        public string Message { get; set; }

        internal bool Success()
        {
            return result == OPENNGS_PLAT_RESULT.Success;
        }
    }

    public interface IPlatfromModule
    {
        OPENNGS_PLATFORM_MODULE Module { get; }
    }

    public interface IPlatformProvider
    {
        bool Init();
        void CreateMocule(OPENNGS_PLATFORM_MODULE module);

        IPlatfromModule GetModule(OPENNGS_PLATFORM_MODULE module);

        bool IsSupported(OPENNGS_PLATFORM_MODULE module);
    }
}
