using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Systems
{
    public interface ISaveSystem
    {
        //对档进行处理
        public void AddFile();
        public void DeleteFile(int targeIndex);
        public void SaveFile();
        public bool ChangeFile(int targeIndex);

        //对档内部数据进行处理
        public void SetFileData(string name, ISaveInfo data);
        public ISaveInfo GetFileData(string name);
        public void SettingSaveFile();
        public void SetSettingData(ISaveInfo data);
        public ISaveInfo GetSettingData();

    }
}
