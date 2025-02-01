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
        bool FileWrite(string saveFileName, byte[] fileData, int length);
        int GetFileSize(string fileName);
        int FileRead(string fileName, byte[] fileData, int fileSize);
    }
}
