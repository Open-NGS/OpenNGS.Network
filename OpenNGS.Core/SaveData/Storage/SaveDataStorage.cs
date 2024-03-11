using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenNGS.Crypto;
using OpenNGS.IO;
using OpenNGS.SaveData;
using ProtoBuf;
using UnityEngine;

namespace OpenNGS.SaveData.Storage
{
    class SaveDataStorage<T> : ILocalSaveData<T> where T : ISaveEntity, new()
    {

        enum SaveDataType
        {
            Raw = 0,
            Cache = 1,
            Backup = 2,
            Temp = 3,
        }

        /*

        save_data/
            + {userId}/
                game_save_data
                    + indexies.dat
                    + autosave.dat
                    + 0/data0           current data.
                        data1           update before save data0.
                        data2           update when load success.
                setting_save_data
                    + indexies.dat
                    + autosave.dat
                    + 0/data0           current data.
                        data1           update before save data0.
                        data2           update when load success.

        */

        /// <summary>
        /// Save data max capacity
        /// </summary>
        public int Capacity { get; private set; }
        public int Version { get; private set; }

        /// <summary>
        /// Save data root path
        /// </summary>
        public string RootPath { get; set; }

        private IFileSystem fsSave;
        private string IndexName = "indexies.dat";

        private string mountName = "SaveData";

        private long magic = 0x5685432132698754 | 0x6654219875421325;

        public void Init(IFileSystem fs, int capacity, int version, bool isSetting)
        {
            fsSave = fs;
            this.Capacity = capacity;
            this.Version = version;

            if (!isSetting)
            {
                this.RootPath = "save_data/game_save_data";
            }
            else
            {
                this.RootPath = "save_data/setting_save_data";
            }

        }

        /// <summary>
        /// Load Indexies
        /// </summary>
        /// <returns></returns>
        public void LoadIndex(Action<IndexiesData<T>> onIndexiesLoaded)
        {
            string path = this.RootPath + "/" + this.IndexName;
            byte[] data = null;

            fsSave.Mount(mountName, true);
            if (fsSave.FileExists(path))
            {
                data = fsSave.Read(path);
            }
            fsSave.Unmount();
            IndexiesData<T> indexies = null;
            if (data == null)
            {
                indexies = RebuildIndexied();
            }
            else
            {
                try
                {
                    using(MemoryStream ms = new MemoryStream(data))
                    {
                        indexies = Serializer.Deserialize<IndexiesData<T>>(ms);
                    }
                }
                catch(Exception ex)
                {
                    Debug.LogErrorFormat("SaveDataStorate.LoadIndex Error: Index Invalid. \r\n{0}", ex.ToString());
                    indexies = null;
                }
                if (indexies == null)
                    indexies = RebuildIndexied();
            }
            onIndexiesLoaded(indexies);
        }


        private IndexiesData<T> RebuildIndexied()
        {
            IndexiesData<T> indexies = IndexiesData<T>.Create(this.Capacity,this.Version);
            indexies.Slots.Clear();
            return indexies;
        }

        public void SaveIndex(IndexiesData<T> index)
        {
            string path = this.RootPath + "/" + this.IndexName;
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] data = null;
                Serializer.Serialize<IndexiesData<T>>(ms,index);
                data = ms.ToArray();
                if (data == null)
                {
                    return;
                }
                fsSave.Mount(mountName, false);
                bool result = fsSave.Write(path, data);
                fsSave.Unmount();

            }
            return;
        }

        public void LoadData(int index, string name, Action<SaveDataResult, SaveData<T>> onSaveDataLoaded)
        {
            // Load local save data
            SaveData<T> saveData;
            SaveDataResult result = LoadLocalSaveData(index, name, out saveData);
            if (onSaveDataLoaded != null)
                onSaveDataLoaded(result, saveData);
            // Cloud Save Data support later

        }

        public void SaveData(int index, string name, SaveData<T> saveData, Action<SaveDataResult> onDataSaved)
        {
            // Load local save data
            SaveDataResult result = SaveLocalSaveData(index, name, saveData);
            if (onDataSaved != null)
                onDataSaved(result);
            // Cloud Save Data support later

        }

        private SaveDataResult SaveLocalSaveData(int index, string name, SaveData<T> saveData)
        {
            SaveDataResult result = SaveDataResult.InvalidData;
            string slotPath = this.RootPath + "/" + index;
            string baseName = slotPath + "/" + name;
#if !UNITY_SWITCH
            string rawFile = baseName + (int)SaveDataType.Raw;
            string cacheFile = baseName + (int)SaveDataType.Cache;
            string backupFile = baseName + (int)SaveDataType.Backup;
            string tempFile = baseName + (int)SaveDataType.Temp;
#endif

#if UNITY_SWITCH
            // Nintendo Switch Guideline 0080
            UnityEngine.Switch.Notification.EnterExitRequestHandlingSection();
#endif
            fsSave.Mount(mountName, false);
            if (!fsSave.DirectoryExists(RootPath))
            {
                fsSave.CreateDirectory(RootPath);
            }

            if (!fsSave.DirectoryExists(slotPath))
            {
                fsSave.CreateDirectory(slotPath);
            }

#if UNITY_SWITCH
            result = this.WriteSaveData(baseName, saveData);
#else
            result = this.WriteSaveData(tempFile, saveData);
            if (result == SaveDataResult.Success)
            {
                fsSave.Copy(tempFile, rawFile);
                fsSave.Move(tempFile, cacheFile);
            }
#endif

            fsSave.Unmount();
#if UNITY_SWITCH
            // Nintendo Switch Guideline 0080
            UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();
#endif
            return result;
        }

        public void Close()
        {

        }

        private SaveDataResult LoadLocalSaveData(int index, string name,out SaveData<T> saveData)
        {
            string baseName = this.RootPath + "/" + index + "/" + name;
#if !UNITY_SWITCH
            string rawFile = baseName + (int)SaveDataType.Raw;
            string cacheFile = baseName + (int)SaveDataType.Cache;
            string backupFile = baseName + (int)SaveDataType.Backup;
#endif
            saveData = null;

            fsSave.Mount(mountName, false);
#if UNITY_SWITCH
            SaveDataResult result = SaveDataResult.NotFound;
            bool baseExist = fsSave.FileExists(baseName);
            if (baseExist)
            {
                result = ReadSaveData(baseName, out saveData);
            }
#else
            bool rawExist = fsSave.FileExists(rawFile);
            bool cacheExist = fsSave.FileExists(cacheFile);
            bool backupExist = fsSave.FileExists(backupFile);

            if (!rawExist && !cacheExist && !backupExist)
            {
                return SaveDataResult.NotFound;
            }

            SaveDataResult result = SaveDataResult.NotFound;
            if (rawExist)
            {
                result = ReadSaveData(rawFile, out saveData);
                if (result == SaveDataResult.Success)
                {
                    fsSave.Copy(rawFile, backupFile);
                    return result;
                }
            }
            if (cacheExist)
            {
                result = ReadSaveData(cacheFile, out saveData);
                if (result == SaveDataResult.Success)
                {
                    fsSave.Copy(cacheFile, rawFile);
                    return SaveDataResult.Recovered;
                }
            }
            if (backupExist)
            {
                result = ReadSaveData(backupFile, out saveData);
                if (result == SaveDataResult.Success)
                {
                    fsSave.Copy(backupFile, rawFile);
                    return SaveDataResult.Recovered;
                }
            }
#endif
            fsSave.Unmount();
            return result;
        }


        private SaveDataResult ReadSaveData(string name, out SaveData<T> saveData)
        {
            saveData = null;
            byte[] data = fsSave.Read(name);
            if (data == null)
            {
                return SaveDataResult.IOError;
            }
            byte[] buff = data;
#if UNITY_STANDALONE
            int size = BitConverter.ToInt32(data, 0);
            if (size == data.Length - 4)
            {
                buff = AES.Decrypt(data, 4, data.Length - 4, magic.ToString("X8"), magic.ToString("X8"));
            }
#endif
            using (MemoryStream ms = new MemoryStream(buff))
            {
                saveData = Serializer.Deserialize<SaveData<T>>(ms);
                if (saveData == null)
                {
                    return SaveDataResult.InvalidData;
                }
            }
            return SaveDataResult.Success;
        }

        public void DeleteData(int index, string name, Action<SaveDataResult> onDataDelete)
        {
            string baseName = this.RootPath + "/" + index + "/" + name;
#if !UNITY_SWITCH
            string rawFile = baseName + (int)SaveDataType.Raw;
            string cacheFile = baseName + (int)SaveDataType.Cache;
            string backupFile = baseName + (int)SaveDataType.Backup;
#endif
            SaveDataResult result = SaveDataResult.Success;
            fsSave.Mount(mountName, false);
#if UNITY_SWITCH
            if (fsSave.FileExists(baseName))
            {
                if (fsSave.Delete(baseName)) result = SaveDataResult.Success;
            }
#else
            if (fsSave.FileExists(rawFile))
            {
                if (fsSave.Delete(rawFile)) result = SaveDataResult.Success;
            }
            if (fsSave.FileExists(cacheFile))
            {
                fsSave.Delete(cacheFile);
            }
            if (fsSave.FileExists(backupFile))
            {
                fsSave.Delete(backupFile);
            }
#endif
            fsSave.Unmount();
            if (onDataDelete != null)
                onDataDelete(result);
        }

        private SaveDataResult WriteSaveData(string name, SaveData<T> saveData)
        {
            byte[] data;
            using (MemoryStream ds = new MemoryStream())
            {
                Serializer.Serialize<SaveData<T>>(ds, saveData);
                data = ds.GetBuffer();
                if (data == null)
                {
                    return SaveDataResult.InvalidData;
                }
            }
            byte[] buff = data;
#if UNITY_STANDALONE
            string key = magic.ToString("X8");
            buff = AES.Encrypt(data, key, key);
            System.IO.MemoryStream ms = new System.IO.MemoryStream(buff.Length + 4);
            ms.Write(BitConverter.GetBytes(buff.Length), 0, 4);
            ms.Write(buff, 0, buff.Length);
            ms.Flush();
            buff = ms.ToArray();
#endif
            if (!fsSave.Write(name, buff))
            {
                return SaveDataResult.IOError;
            }
            return SaveDataResult.Success;
        }

        public void Update()
        {
            
        }
    }
}
