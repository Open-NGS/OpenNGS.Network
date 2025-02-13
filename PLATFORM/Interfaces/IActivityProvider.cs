using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Platform
{
    public interface IActivityProvider : IModuleProvider
    {
        void ActivityStart(string actitityId);

        void ActivityEnd(string actitityId, string outcome);

        void ResetAllActivities();
    }
}
