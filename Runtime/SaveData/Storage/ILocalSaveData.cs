using OpenNGS.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNGS.SaveData.Storage
{
    interface ILocalSaveData
    {
        void Init(IFileSystem fs, int capacity, SaveDataMode mode, ISaveDataAPI api);
        void Update(); 
        void Terminate();

        void LoadIndex(Action onIndexiesLoaded);
        void LoadData(SaveData saveData, Action<SaveDataResult, SaveData> onSaveDataLoaded);
        void SaveData(SaveData saveData, Action<SaveDataResult> onDataSaved);
        void DeleteData(string name, Action<SaveDataResult> onDataDeleted);

    }
}
