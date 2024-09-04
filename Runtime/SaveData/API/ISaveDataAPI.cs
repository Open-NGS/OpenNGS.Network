using OpenNGS.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNGS.SaveData.Storage
{
    public interface ISaveDataAPI
    {
        void Init(IFileSystem fs, int capacity, SaveDataMode mode);
        void Update();
        void Close();
        void LoadIndex(Action onIndexiesLoaded);
        void LoadData(SaveData saveData, string name, Action<SaveDataResult, SaveData> onSaveDataLoaded);
        void SaveData(SaveData saveData, string name, Action<SaveDataResult> onDataSaved);
        void DeleteData(string name, Action<SaveDataResult> onDataDeleted);
    }
}
