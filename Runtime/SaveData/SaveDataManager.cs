using OpenNGS.Configs;
using OpenNGS.IO;
using OpenNGS.SaveData.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenNGS.SaveData
{
    public class SaveDataManager<T> : SaveDataManager where T : SaveData, new()
    {
        public static new SaveDataManager<T> Instance;

        public new T Current
        {
            get
            {
                return (T)this.activeData;
            }
        }

        protected override void OnCreateSaveData(out SaveData save)
        {
            save = new T();
        }


        public new T NewSaveData(bool save)
        {
            T data = base.NewSaveData(save) as T;
            return data;
        }

        static public void Initialize(string name, IFileSystem fs, int capacity, SaveDataMode mode)
        {
            Initialize(name, fs, capacity, mode, null);
        }

        static public void Initialize(string name, IFileSystem fs, int capacity, SaveDataMode mode, ISaveDataAPI api)
        {
            if (Instance != null && Instance.mInited)
            {
                return;
            }
            Instance = Create<T>(name, fs, capacity, mode, api);
        }
    }

    public class SaveDataManager
    {
        public delegate void SaveDataEvent(SaveDataResult result);

        public event SaveDataEvent OnBeforeSave;
        public event SaveDataEvent OnSaved;
        public event SaveDataEvent OnLoaded;

        public bool LoadReady { get; protected set; }

        public static SaveDataManager Instance;

        public SaveData Current
        {
            get
            {
                return this.activeData;
            }
        }

        public SaveData GetActiveSaveData()
        {
            if (this.activeData == null) return null;
            return this.activeData;
        }

        public int Count
        {
            get
            {
                if (this.m_slots == null) return 0;
                return this.m_slots.Count;
            }
        }

        ILocalSaveData storage;
        internal ISaveDataAPI SaveAPI { get; private set; }
        protected SaveData activeData = null;
        int lastSaveTime;


        public SaveDataMode SaveDataMode { get; private set; }

        /// <summary>
        /// Save Data max capacity
        /// </summary>
        public int Capacity { get; private set; }

        private List<SaveData> m_slots = new List<SaveData>();

        /// <summary>
        /// 存档列表
        /// </summary>
        public List<SaveData> Slots
        {
            get
            {
                return m_slots;
            }
        }

        public bool Encrypt { get; internal set; }
        internal string Name { get; private set; }

        protected bool mInited = false;


        public SaveDataManager()
        {
#if (UNITY_PS4 || UNITY_PS5 || UNITY_PLAYSTATION) && !UNITY_EDITOR
            storage = new SaveDataAPI(this);
#else
            storage = new SaveDataStorage(this);
#endif
        }

        static public void Initialize<T>(string name, IFileSystem fs, int capacity, SaveDataMode mode) where T : SaveData, new()
        {
            Initialize<T>(name, fs, capacity, mode, null);
        }
        static public void Initialize<T>(string name, IFileSystem fs, int capacity, SaveDataMode mode, ISaveDataAPI api) where T : SaveData, new()
        {
            if (Instance != null && Instance.mInited)
            {
                return;
            }
            Instance = Create<T>(name, fs, capacity, mode,api);
        }

        static public SaveDataManager<T> Create<T>(string name, IFileSystem fs, int capacity, SaveDataMode mode, ISaveDataAPI api) where T : SaveData, new()
        {
            var manager = new SaveDataManager<T>();
            manager.mInited = true;
            manager.SaveDataMode = mode;
            manager.Capacity = capacity;
            manager.Name = name;
            manager.storage.Init(fs, capacity, mode, api);
            manager.LoadIndex();
            return manager;
        }

        public void LoadIndex()
        {
            storage.LoadIndex(OnIndexiesLoaded);
        }
        void OnIndexiesLoaded()
        {
            this.activeData = this.GetSaveData(0);
        }

        /// <summary>
        /// 读取默认活动存档
        /// </summary>
        public void Load()
        {
            if (this.activeData == null)
            {
                this.OnDataSaved(SaveDataResult.NotFound);
                return;
            }
            Load(this.activeData);
        }

        public void Load(SaveData savedata)
        {
#if DEBUG_LOG
            Debug.LogFormat("SaveData >> LoadData:[{0}] start", savedata.DirName);
#endif
            LoadReady = false;
            LoadData(savedata, OnDataLoaded);
        }

        void OnDataLoaded(SaveDataResult result, SaveData saveData)
        {
#if DEBUG_LOG
            Debug.LogFormat("SaveData >> OnDataLoaded:[{0}] {1}", saveData.DirName, result);
#endif
            if (result == SaveDataResult.Success || result == SaveDataResult.Recovered)
            {
            }
            saveData.Status = result;
            this.activeData = saveData;

            if (this.activeData == null)
            {
                Debug.LogFormat("SaveData >> OnDataLoaded:{0}", result);
            }
            LoadReady = true;
            this.activeData.Migrate(saveData.Version);
            if (this.Current != null && this.OnLoaded != null)
            {
                this.OnLoaded(result);
            }
        }

        public void Save()
        {
            LoadReady = false;
            if (this.activeData == null)
            {
                this.OnDataSaved(SaveDataResult.NotFound);
                return;
            }

            this.Save(activeData);
        }

        public void Save(SaveData savedata)
        {
#if DEBUG_LOG
            Debug.LogFormat("SaveData >> OnDataLoaded:[{0}]", savedata.DirName);
#endif
            if (!this.m_slots.Contains(savedata)) { 
                this.m_slots.Add(savedata);
            }
            if (this.activeData != savedata)
            { 
                this.activeData = savedata;
            }
            this.lastSaveTime = Time.Timestamp;

            savedata.Time = lastSaveTime;
            savedata.Totaltime = (uint)Time.TotalGameTime;

            if (this.OnBeforeSave != null) this.OnBeforeSave(SaveDataResult.Success);
            this.SaveData(savedata, OnDataSaved);
#if DEBUG_LOG
            Debug.LogFormat("SaveData >> SaveData:[{0}] end", savedata.DirName);
#endif
        }

        void OnDataSaved(SaveDataResult result)
        {
#if DEBUG_LOG
            Debug.LogFormat("SaveData >> OnDataSaved:[{0}]:{1}", this.activeData.DirName, result);
#endif
            LoadReady = true;
            if (this.OnSaved != null)
            {
                this.OnSaved(result);
            }
        }

        public void Delete(int index)
        {
#if DEBUG_LOG
            Debug.LogFormat("SaveData >> DeleteData:{0} start", index);
#endif
            if (index < 0 || index >= this.Capacity)
                throw new IndexOutOfRangeException();
            this.DeleteData(index, OnDataDeleted);
        }

        void OnDataDeleted(SaveDataResult result)
        {
#if DEBUG_LOG
            Debug.LogFormat("SaveData >> OnDataDeleted:{0}", result);
#endif
        }

        public bool Exists(int index)
        {
            if (index < 0 || index >= this.Capacity)
                throw new IndexOutOfRangeException();

            if (this.m_slots == null)
                return false;
            var data = this.m_slots[index];
            if (data != null)
                return true;
            return false;
        }

        public SaveData GetSaveData(int index)
        {
            if (index < 0 || index >= this.Capacity || index >= this.m_slots.Count)
                return null;

            if (this.m_slots == null || this.m_slots.Count == 0)
                return null;

            return this.m_slots[index];
        }

        public virtual SaveData NewSaveData(bool save)
        {
#if DEBUG_LOG
            Debug.LogFormat("SaveData >> NewSaveData:[{0}] start", save);
#endif
            if (this.Count >= this.Capacity)
                throw new ArgumentOutOfRangeException();

            Time.ResetGameTime(0);
            this.activeData = this.NewSaveData();
            if (save) 
                this.Save();
            else
                LoadReady = true;

            if (this.OnLoaded != null)
            {
                this.OnLoaded(SaveDataResult.Success);
            }
#if DEBUG_LOG
            Debug.LogFormat("SaveData >> NewSaveData:[{0}] end", save);
#endif
            return this.activeData;
        }
        internal SaveData NewSaveData()
        {
            OnCreateSaveData(out SaveData savedata);
            AddSaveData(savedata);
            return savedata;
        }

        protected virtual void OnCreateSaveData(out SaveData save)
        {
            save = new SaveData();
        }


        private void AddSaveData(SaveData data)
        {
#if DEBUG_LOG
            Debug.LogFormat("SaveData >> AddSaveData:[{0}]", data.DirName);
#endif
            if (this.m_slots == null)
                this.m_slots = new List<SaveData>();

            if (!this.m_slots.Contains(data))
                this.m_slots.Add(data);
        }



        /// <summary>
        /// Shutdown SaveData System
        /// </summary>
        public void Terminate()
        {
            storage.Terminate();
        }

        public void Update()
        {
            storage.Update();
        }



        public void AutoSave()
        {
            if (Time.Timestamp - lastSaveTime > 5)
            {
                Save();
            }
        }

        private void LoadData(SaveData saveData, Action<SaveDataResult, SaveData> onDataLoaded)
        {
            if (string.IsNullOrEmpty(saveData.DirName))
                throw new ArgumentNullException("saveData.DirName");

            storage.LoadData(saveData, onDataLoaded);
        }

        private void SaveData(SaveData saveData, Action<SaveDataResult> onDataSaved)
        {
            if (string.IsNullOrEmpty(saveData.DirName))
                throw new ArgumentNullException("saveData.DirName");

            storage.SaveData(saveData, onDataSaved);
        }

        private void DeleteData(int index, Action<SaveDataResult> onDataDeleted)
        {
            if (index < 0 || index >= this.Capacity || index >= this.m_slots.Count)
                throw new IndexOutOfRangeException();
            storage.DeleteData(this.m_slots[index].DirName, onDataDeleted);
        }

        internal IEnumerator WaitReady()
        {
            while (!this.LoadReady)
            {
                yield return new WaitForEndOfFrame();
            }
        }

    }
}