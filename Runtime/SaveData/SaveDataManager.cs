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
    internal class SaveDataManager<T> : SaveDataManager where T : SaveData, new()
    {
        public static new SaveDataManager<T> Instance;

        public virtual T Current
        {
            get
            {
                return this.SaveData;
            }
        }
        private T SaveData;


        protected override void OnCreateSaveData(out SaveData save)
        {
            save = new T();
        }



        protected virtual void OnSaveDataLoaded(byte[] data)
        {
            //byte[] data = Proto.Protobuf.Serialize<SaveData>(saveData);

            //byte[] data;
        }

        protected virtual SaveDataResult OnSaveDataSave()
        {
            using (MemoryStream ds = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize<T>(ds, SaveData);
                //this.activeData.Data = ds.GetBuffer();
                //if (this.activeData.Data == null)
                //{
                //    return SaveDataResult.InvalidData;
                //}
            }
            return SaveDataResult.Success;
        }

        public void NewSaveData(bool save)
        {
            if (this.ActiveIndex < 0 || this.ActiveIndex >= this.Capacity)
                throw new IndexOutOfRangeException();

            Time.ResetGameTime(0);

            T data = new T();
            this.Save(data);
            //this.Index.New(this.ActiveIndex);
            //this.activeData = this.GetSaveData(this.ActiveIndex);

            if (save) this.Save();
            else
                LoadReady = true;
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

        public SaveData CurrentData
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

        private string mainSaveName = "data";


        private int activeIndex = -1;

        public int ActiveIndex
        {
            get
            {
                return this.activeIndex;
            }
            set
            {
                this.activeIndex = value;
                this.activeData = this.GetSaveData(this.activeIndex);
            }
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

        private bool mInited = false;


        public SaveDataManager()
        {
#if (UNITY_PS4|| UNITY_PS5) && !UNITY_EDITOR
            storage = new SaveDataAPI();
#else
            storage = new SaveDataStorage();
#endif
        }

        static public void Initialize<T>(IFileSystem fs, int capacity, SaveDataMode mode) where T : SaveData, new()
        {
            if (Instance != null && Instance.mInited)
            {
                return;
            }
            Instance = new SaveDataManager<T>();

            Instance.mInited = true;
            Instance.SaveDataMode = mode;
            Instance.Capacity = capacity;
            Instance.storage.Init(fs, capacity, mode);
            Instance.LoadIndex();
        }


        public void LoadIndex()
        {
            storage.LoadIndex(OnIndexiesLoaded);
        }
        void OnIndexiesLoaded()
        {
            this.activeIndex = this.FindValidIndex();
            //this.activeData = this.GetSaveData(this.activeIndex);
        }


        public void Load()
        {
#if DEBUG_LOG
            Debug.LogFormat("SaveData >> LoadData:[{0}]Name:{1} start", this.activeIndex, this.mainSaveName);
#endif
            LoadData(this.activeData, this.mainSaveName, OnDataLoaded);
        }

        public void Load(SaveData data)
        {
#if DEBUG_LOG
            Debug.LogFormat("SaveData >> LoadData:[{0}] start", data.DirName);
#endif
            LoadReady = false;
            LoadData(data, this.mainSaveName, OnDataLoaded);
        }

        void OnDataLoaded(SaveDataResult result, SaveData saveData)
        {
#if DEBUG_LOG
            Debug.LogFormat("SaveData >> OnDataLoaded:[{0}]Name:{1} - {2}", this.activeIndex, this.mainSaveName, result);
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
            if (this.CurrentData != null && this.OnLoaded != null)
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

        public void Save(SaveData data)
        {
            this.lastSaveTime = Time.Timestamp;
            data.Time = lastSaveTime;
            data.Totaltime = Time.TotalGameTime;

            if (this.OnBeforeSave != null) this.OnBeforeSave(SaveDataResult.Success);
#if DEBUG_LOG
            Debug.LogFormat("SaveData >> SaveData:[{0}]Name:{1} start", this.activeIndex, this.mainSaveName);
#endif
            this.SaveData(data, this.mainSaveName, OnDataSaved);
        }

        void OnDataSaved(SaveDataResult result)
        {
#if DEBUG_LOG
            Debug.LogFormat("SaveData >> OnDataSaved:[{0}]Name:{1} - {2}", this.activeIndex, this.mainSaveName, result);
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
            this.activeIndex = index;
            this.DeleteData(index, this.mainSaveName, OnDataDeleted);
        }

        void OnDataDeleted(SaveDataResult result)
        {
#if DEBUG_LOG
            Debug.LogFormat("SaveData >> OnDataDeleted:{0} - {1}", this.activeIndex, result);
#endif
            if (result == SaveDataResult.Success)
            {
                this.ActiveIndex = this.FindValidIndex();
            }
        }

        public bool Exists(int index)
        {
            if (index < 0 || index >= this.Capacity)
                throw new IndexOutOfRangeException();

            if (this.Slots == null)
                return false;
            var data = this.Slots[index];
            if (data != null && data.Status == SaveDataResult.Success)
                return true;
            return false;
        }

        public SaveData GetSaveData(int index)
        {
            if (index < 0 || index >= this.Capacity || index >= this.m_slots.Count)
                throw new IndexOutOfRangeException();

            if (this.Slots == null)
                return null;

            return this.Slots[index];
        }

        public void NewSaveData(bool save)
        {
            if (this.ActiveIndex < 0 || this.ActiveIndex >= this.Capacity)
                throw new IndexOutOfRangeException();

            Time.ResetGameTime(0);
            this.activeData = this.GetSaveData(this.ActiveIndex);
            if (save) this.Save();
            else
                LoadReady = true;

            if (this.OnLoaded != null)
            {
                this.OnLoaded(SaveDataResult.Success);
            }
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

        private int FindValidIndex()
        {
            if (this.Slots.Count == 0) return -1;
            if (activeIndex >= this.Slots.Count)
                return this.Slots.Count - 1;
            return this.activeIndex;
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
            if (this.m_slots == null)
                this.m_slots = new List<SaveData>();
            this.m_slots.Add(data);
        }


        private void LoadData(SaveData saveData, string name, Action<SaveDataResult, SaveData> onDataLoaded)
        {
            if (string.IsNullOrEmpty(saveData.DirName))
                throw new ArgumentNullException("saveData.DirName");

            storage.LoadData(saveData, name, onDataLoaded);
        }

        private void SaveData(SaveData saveData, string name, Action<SaveDataResult> onDataSaved)
        {
            if (string.IsNullOrEmpty(saveData.DirName))
                throw new ArgumentNullException("saveData.DirName");

            storage.SaveData(saveData, name, onDataSaved);
        }

        private void DeleteData(int index, string name, Action<SaveDataResult> onDataDeleted)
        {
            if (index < 0 || index >= this.Capacity)
                throw new IndexOutOfRangeException();

            storage.DeleteData(name, onDataDeleted);
        }

        internal IEnumerator WaitReady()
        {
            while (!this.LoadReady)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// List all savedata
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<SaveData> ListAll()
        {
            throw new NotImplementedException();
        }
    }
}