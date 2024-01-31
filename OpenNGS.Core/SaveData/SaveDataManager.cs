using OpenNGS.IO;
using OpenNGS.SaveData.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenNGS.SaveData
{
    public class SaveDataManager<T> : Singleton<SaveDataManager<T>> where T : ISaveEntity, new()
    {
        public delegate void SaveDataEvent();

        public event SaveDataEvent OnBeforeSave;
        public event SaveDataEvent OnSaved;
        public event SaveDataEvent OnLoaded;

        public bool LoadReady { get; private set; }

        public T Current
        {
            get
            {
                if (this.activeData == null) return default(T);
                return this.activeData.Data;
            }
        }

        public SaveData<T> Active
        {
            get
            {
                if (this.activeData == null) return null;
                return this.activeData;
            }
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
                if (this.Index != null)
                {
                    this.Index.Current = value;
                }
            }
        }

        public int Count
        {
            get
            {
                if (this.Index == null) return 0;
                return this.Index.Slots.Count;
            }
        }


        ILocalSaveData<T> storage;
        IndexiesData<T> Index = null;
        SaveData<T> activeData = null;
        int lastSaveTime;
        /// <summary>
        /// Save Data max capacity
        /// </summary>
        public int Capacity { get; private set; }


        public Dictionary<int, SaveSlot<T>> Slots
        {
            get
            {
                if (Index == null) return null;
                return Index.Slots;
            }
        }

        private bool mInited = false;


        public SaveDataManager()
        {
#if UNITY_PS4 && !UNITY_EDITOR
            storage = new SaveDataAPI();
#else
            storage = new SaveDataStorage<T>();
#endif
        }

        public void Init(IFileSystem fs, int capacity, int version)
        {
            if (mInited)
            {
                return;
            }
            mInited = true;

            this.Capacity = capacity;
            storage.Init(fs, capacity, version);

            LoadIndex();
        }


        public void LoadIndex()
        {
            storage.LoadIndex(OnIndexiesLoaded);
        }

        void OnIndexiesLoaded(IndexiesData<T> indexies)
        {
            if (indexies != null)
            {
                this.Index = indexies;
                if(storage.Version != this.Index.Version && this.Index.Migrate(storage.Version))
                {
                    this.Save();
                }
                this.activeIndex = this.FindValidIndex();
                if(Index.Slots.ContainsKey(this.activeIndex) == true)
                {
                    this.activeData = this.GetSaveData(this.activeIndex);
                }
            }
        }

        void OnIndexiesSaved()
        {

        }



        public void Load()
        {
#if DEBUG_LOG
            Debug.LogFormat("SaveData >> LoadData:[{0}]Name:{1} start", this.activeIndex, this.mainSaveName);
#endif
            LoadReady = false;
            LoadData(this.activeIndex, this.mainSaveName, OnDataLoaded);
        }

        void OnDataLoaded(SaveDataResult result, SaveData<T> saveData)
        {
#if DEBUG_LOG
            Debug.LogFormat("SaveData >> OnDataLoaded:[{0}]Name:{1} - {2}", this.activeIndex, this.mainSaveName, result);
#endif
            if (result == SaveDataResult.Success || result == SaveDataResult.Recovered)
            {
                this.Index.Slots[this.activeIndex].SaveData = saveData;
            }
            this.Index.Slots[this.activeIndex].Status = result;
            this.activeData = this.GetSaveData(this.activeIndex);

            if (this.activeData == null)
            {
                Debug.LogFormat("SaveData >> OnDataLoaded:{0}", result);
            }
            LoadReady = true;
            this.activeData.Migrate(this.Index.Version);
            if (this.Current != null && this.OnLoaded != null)
            {
                this.OnLoaded();
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
            this.lastSaveTime = Time.Timestamp;
            this.activeData.Time = lastSaveTime;
            this.activeData.Totaltime = Time.TotalGameTime;
            this.Slots[this.ActiveIndex].Detail = this.activeData.MetaData;
            this.Slots[this.ActiveIndex].Time = lastSaveTime;

            if (this.OnBeforeSave != null) this.OnBeforeSave();
#if DEBUG_LOG
            Debug.LogFormat("SaveData >> SaveData:[{0}]Name:{1} start", this.activeIndex, this.mainSaveName);
#endif
            this.SaveData(this.activeIndex, this.mainSaveName, this.activeData, OnDataSaved);
        }

        public void SaveIndex()
        {
            storage.SaveIndex(this.Index);
        }
        void OnDataSaved(SaveDataResult result)
        {
#if DEBUG_LOG
            Debug.LogFormat("SaveData >> OnDataSaved:[{0}]Name:{1} - {2}", this.activeIndex, this.mainSaveName, result);
#endif
            if (result == SaveDataResult.Success)
            {
                storage.SaveIndex(this.Index);
            }
            LoadReady = true;
            if (this.OnSaved != null)
            {
                this.OnSaved();
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
                this.Index.Delete(this.ActiveIndex);
                this.ActiveIndex = this.FindValidIndex();
                storage.SaveIndex(this.Index);
            }
        }

        public bool Exists(int index)
        {
            if (index < 0 || index >= this.Capacity)
                throw new IndexOutOfRangeException();

            if (this.Slots == null)
                return false;
            return Slots.ContainsKey(index);
        }

        public SaveSlot<T> GetSlot(int index)
        {
            if (index < 0 || index >= this.Capacity)
                throw new IndexOutOfRangeException();

            if (this.Slots == null)
                return null;

            SaveSlot<T> slot = null;
            this.Slots.TryGetValue(index, out slot);
            return slot;
        }

        public void NewSaveData(bool save)
        {
            if (this.ActiveIndex < 0 || this.ActiveIndex >= this.Capacity)
                throw new IndexOutOfRangeException();

            Time.ResetGameTime(0);
            this.Index.New(this.ActiveIndex);
            this.activeData = this.GetSaveData(this.ActiveIndex);
            if (save) this.Save();
            else
                LoadReady = true;

            if (this.OnLoaded != null)
            {
                this.OnLoaded();
            }
        }


        /// <summary>
        /// Shutdown SaveData System
        /// </summary>
        public void Close()
        {
            storage.Close();
        }

        public void Update()
        {
            storage.Update();
        }

        private SaveData<T> GetSaveData(int index)
        {
            if (Index == null) return null;

            SaveSlot<T> idx;
            if (!this.Slots.TryGetValue(index, out idx))
            {
                return null;
            }
            return Index.Slots[index].SaveData;
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
            if (Index.Slots.Count == 0) return 0;
            int activeIdx = -1;
            foreach (var kv in Index.Slots)
            {
                if (kv.Value.Index == Index.Current)
                {
                    activeIdx = kv.Key;
                    break;
                }
            }
            if (activeIdx == -1)
                activeIdx = Index.Slots.Keys.ElementAt(0);
            return activeIdx;
        }

        private void LoadData(int index, string name, Action<SaveDataResult, SaveData<T>> onDataLoaded)
        {
            if (index < 0 || index >= this.Capacity)
                throw new IndexOutOfRangeException();

            storage.LoadData(index, name, onDataLoaded);
        }

        private void SaveData(int index, string name, SaveData<T> saveData, Action<SaveDataResult> onDataSaved)
        {
            if (index < 0 || index >= this.Capacity)
                throw new IndexOutOfRangeException();

            storage.SaveData(index, name, saveData, onDataSaved);
        }

        private void DeleteData(int index, string name, Action<SaveDataResult> onDataDeleted)
        {
            if (index < 0 || index >= this.Capacity)
                throw new IndexOutOfRangeException();

            storage.DeleteData(index, name, onDataDeleted);
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