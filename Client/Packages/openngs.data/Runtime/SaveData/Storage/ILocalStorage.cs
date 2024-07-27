using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNGS.SaveData.Storage
{
    interface ILocalStorage
    {
        bool Init();
        byte[] Read(string v);
        bool FileExists(string filename);
        bool DirectoryExists(string filename);
        void Copy(string source, string destination);
        void Move(string source, string destination);
        bool Write(string name, byte[] data);
        void CreateDirectory(string slotPath);
        bool Delete(string path);
        void Close();
    }
}
