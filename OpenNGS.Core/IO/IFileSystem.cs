using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenNGS.IO
{
    public interface IFileSystem
    {
        void Mount(string mountName, bool @readonly);

        bool MountRom(string mountName);
        bool MountSaveData(string mountName, bool readOnly);
        bool MountCacheData(string mountName, bool readOnly);
        void Unmount();

        bool FileExists(string filename);
        bool DirectoryExists(string filename);
        byte[] Read(string path);
        bool Write(string name, byte[] data);
        void CreateDirectory(string slotPath);
        void Copy(string source, string destination);
        void Move(string source, string destination);
        bool Delete(string path);
        Stream OpenRead(string path);
        string ReadAllText(string path, Encoding encoding);

        bool Rename(string srcFileName, string destFileName);
    }
}
