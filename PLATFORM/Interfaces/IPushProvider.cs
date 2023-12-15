using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Platform
{
    public interface IPushProvider : IModuleProvider
    {
        void RegisterPush();
        void SetAccount(string accountId);
        void DeleteAccount(string accountId);
    }
}
