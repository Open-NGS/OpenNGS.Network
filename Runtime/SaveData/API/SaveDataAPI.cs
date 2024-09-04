using OpenNGS.IO;
using System;
using System.Collections.Generic;

namespace OpenNGS.SaveData.Storage
{
    class SaveDataAPI : ILocalSaveData
    {

        ISaveDataAPI memory = null;

        public void Init(IFileSystem fs, int capacity, SaveDataMode mode)
        {
#if UNITY_PLAYSTATION
            if(memory == null)
            {
                throw new Exception("PlayStation SaveData uninitialized.");
            }
#endif
            memory.Init(fs,capacity, mode);
        }

        public void LoadIndex(Action onIndexiesLoaded)
        {
            memory.LoadIndex(onIndexiesLoaded);
        }

        public void LoadData(SaveData saveData, string name, Action<SaveDataResult, SaveData> onSaveDataLoaded)
        {
            memory.LoadData(saveData, name, onSaveDataLoaded);
        }

        public void SaveData(SaveData saveData, string name, Action<SaveDataResult> onDataSaved)
        {
            memory.SaveData(saveData, name, onDataSaved);
        }

        public void DeleteData(string name, Action<SaveDataResult> onDataDeleted)
        {
            memory.DeleteData(name, onDataDeleted);
        }

        public void Update()
        {
            memory.Update();
        }

        public void Terminate()
        {
            memory.Close();
        }
    }
}
