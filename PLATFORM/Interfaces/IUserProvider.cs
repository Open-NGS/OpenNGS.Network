using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Platform
{
    public interface IUserProvider : IModuleProvider
    {

        long GetUserID();
    }
}
