using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Platform
{
    public interface IRemoteStorageProvider : IModuleProvider
    {
        bool IsEnabledForApp();

        int GetFileCount();
        bool FileDelete(string fileName);

        string GetFileNameAndSize(int i, out object _);
    }
}
