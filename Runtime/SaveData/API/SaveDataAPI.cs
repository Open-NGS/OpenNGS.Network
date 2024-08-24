using OpenNGS.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using static UnityEditor.Experimental.GraphView.Port;

namespace OpenNGS.SaveData.Storage
{
    class SaveDataAPI<T> : ILocalSaveData<T> where T : ISaveEntity, new()
    {

        ISaveDataAPI<T> memory = null;

        public int Version { get;private set; }

        public void Init(IFileSystem fs, int capacity, int version, bool isSetting)
        {
            this.Version = version;
#if UNITY_PS4
            memory = new PlayStationSaveDataAPI();
#endif
            memory.Init(fs,capacity, version);
        }

        public void LoadIndex(Action<IndexiesData<T>> onIndexiesLoaded)
        {
            memory.LoadIndex(onIndexiesLoaded);
        }

        public void SaveIndex(IndexiesData<T> indexies)
        {
            
        }

        public void LoadData(int index, string name, Action<SaveDataResult, SaveData<T>> onSaveDataLoaded)
        {
            memory.LoadData(index, name, onSaveDataLoaded);
        }

        public void SaveData(int index, string name, SaveData<T> saveData, Action<SaveDataResult> onDataSaved)
        {
            memory.SaveData(index, name, saveData, onDataSaved);
        }

        public void DeleteData(int index, string name, Action<SaveDataResult> onDataDeleted)
        {
            memory.DeleteData(index, name, onDataDeleted);
        }

        public void Update()
        {
            memory.Update();
        }

        public void Close()
        {
            memory.Close();
        }
    }
}
