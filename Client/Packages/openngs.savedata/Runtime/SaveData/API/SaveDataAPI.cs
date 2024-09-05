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

        public void LoadData(SaveData saveData, Action<SaveDataResult, SaveData> onSaveDataLoaded)
        {
            memory.LoadData(saveData, onSaveDataLoaded);
        }

        public void SaveData(SaveData saveData, Action<SaveDataResult> onDataSaved)
        {
            memory.SaveData(saveData, onDataSaved);
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
