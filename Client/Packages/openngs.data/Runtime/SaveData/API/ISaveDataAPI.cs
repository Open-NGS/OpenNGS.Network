using OpenNGS.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNGS.SaveData.Storage
{
    interface ISaveDataAPI<T> where T : ISaveEntity, new()
    {
        void Init(IFileSystem fs, int capacity, int version);
        void Update();
        void Close();
        void LoadIndex(Action<IndexiesData<T>> onIndexiesLoaded);
        void LoadData(int index, string name, Action<SaveDataResult, SaveData<T>> onSaveDataLoaded);
        void SaveData(int index, string name, SaveData<T> saveData, Action<SaveDataResult> onDataSaved);
        void DeleteData(int index, string name, Action<SaveDataResult> onDataDeleted);
    }
}
