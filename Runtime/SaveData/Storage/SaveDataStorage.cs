using System;
using System.IO;
using OpenNGS.Crypto;
using OpenNGS.IO;
using OpenNGS.SaveData;
using ProtoBuf;
using UnityEngine;

namespace OpenNGS.SaveData.Storage
{
    class SaveDataStorage : ILocalSaveData
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
                [settings] (留给系统的设置模块默认使用的存储区)
                    + main.nsd
                    + {savefile}

                autosave/ （自动保存机制默认位置）
                    + main.nsd  // 摘要信息
                    + gamedata.nsd // 实际游戏数据

                game_save_data/
                    + main.nsd

                    + autosave.dat
                    + 0/data0           current data.
                        data1           update before save data0.
                        data2           update when load success.
                setting_save_data/
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

        /// <summary>
        /// Save data root path
        /// </summary>
        public string RootPath { get; set; }

        private IFileSystem fsSave;

        private string mountName = "SaveData";

        private long magic = 0x5685432132698754 | 0x6654219875421325;

        
        private SaveDataManager sm;

        internal SaveDataStorage(SaveDataManager sm)
        {
            this.sm = sm;
        }

        public void Init(IFileSystem fs, int capacity, SaveDataMode mode, ISaveDataAPI api)
        {
            fsSave = fs;
            this.Capacity = capacity;
            //this.RootPath = "save_data/";

            this.RootPath = OpenNGS.IO.FileSystem.DataPath + "/save_data/";
        }

        /// <summary>
        /// Load Indexies
        /// </summary>
        /// <returns></returns>
        public void LoadIndex(Action onIndexiesLoaded)
        {
            string path = this.RootPath + "/";
            byte[] data = null;

            fsSave.Mount(mountName, true);

            if (!fsSave.DirectoryExists(path))
            {
                Debug.Log(path + " not found");
                onIndexiesLoaded();
                return;
            }
            Debug.Log("LoadIndex at " + path);

            var all = Directory.GetFiles(path, sm.Name, SearchOption.AllDirectories);
            foreach (var savefile in all)
            {
                SaveData item = sm.NewSaveData();
                item.DirName = Directory.GetParent(savefile).Name; 

                if (fsSave.FileExists(savefile))
                {
                    data = fsSave.Read(savefile);
                    try
                    {
                        using (MemoryStream ms = new MemoryStream(data))
                        {
                            item.Read(ms);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogErrorFormat("SaveDataStorate.LoadIndex Error: Index Invalid. \r\n{0}", ex.ToString());
                    }
                }

            }
            fsSave.Unmount();
            onIndexiesLoaded();
        }


        public void LoadData(SaveData saveData, Action<SaveDataResult, SaveData> onSaveDataLoaded)
        {
            // Load local save data
            SaveDataResult result = LoadLocalSaveData(saveData);
            if (onSaveDataLoaded != null)
                onSaveDataLoaded(result, saveData);
            // Cloud Save Data support later

        }

        public void SaveData(SaveData saveData, Action<SaveDataResult> onDataSaved)
        {
            // Load local save data
            SaveDataResult result = SaveLocalSaveData(saveData);
            if (onDataSaved != null)
                onDataSaved(result);
            // Cloud Save Data support later

        }

        private SaveDataResult SaveLocalSaveData(SaveData saveData)
        {
            SaveDataResult result = SaveDataResult.InvalidData;
            string slotPath = OpenNGS.IO.Path.Combine(this.RootPath, saveData.DirName);

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

            result = this.WriteMeta(this.RootPath, saveData);
            if (result != SaveDataResult.Success)
            {
                return result;
            }

#if UNITY_SWITCH
            result = this.WriteSaveData(slotPath, saveData);
#else
            result = this.WriteSaveData(slotPath, saveData);
#endif

            fsSave.Unmount();
#if UNITY_SWITCH
            // Nintendo Switch Guideline 0080
            UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();
#endif
            return result;
        }

        public void Terminate()
        {

        }

        private SaveDataResult LoadLocalSaveData(SaveData saveData)
        {

            string slotPath = OpenNGS.IO.Path.Combine(this.RootPath, saveData.DirName);

            fsSave.Mount(mountName, false);

            SaveDataResult result = SaveDataResult.NotFound;
            bool baseExist = fsSave.DirectoryExists(slotPath);
            if (baseExist)
            {
                result = ReadSaveData(slotPath, saveData);
            }
            fsSave.Unmount();
            return result;
        }

        private SaveDataResult ReadMeta(string name, out SaveData saveData)
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
                saveData.Read(ms);
            }
            return SaveDataResult.Success;
        }


        private SaveDataResult ReadSaveData(string filename, SaveData saveData)
        {
            foreach (var sf in saveData.Files)
            {
                var result = sf.Read(fsSave, filename);
                if (result != SaveDataResult.Success && result != SaveDataResult.Recovered)
                    return result;
            }
            return SaveDataResult.Success;
        }

        public void DeleteData(string name, Action<SaveDataResult> onDataDelete)
        {
            string baseName = this.RootPath + "/" + name + "/" + sm.Name;
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
            if (fsSave.FileExists(baseName))
            {
                if (fsSave.Delete(baseName)) result = SaveDataResult.Success;
            }
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


        private SaveDataResult WriteMeta(string folder, SaveData saveData)
        {

            byte[] data;
            using (MemoryStream ds = new MemoryStream())
            {
                saveData.Write(ds);
                ds.Flush();
                data = ds.ToArray();
                if (data == null)
                {
                    return SaveDataResult.InvalidData;
                }
            }
            byte[] buff = data;
            if (!fsSave.Write(OpenNGS.IO.Path.Combine(this.RootPath, saveData.DirName, sm.Name), buff))
            {
                return SaveDataResult.IOError;
            }
            return SaveDataResult.Success;
        }

        private SaveDataResult WriteSaveData(string folder, SaveData saveData)
        {
            foreach(var sf in saveData.Files)
            {
                if(!sf.Write(fsSave, folder))
                {
                    return SaveDataResult.IOError;
                }
            }
            return SaveDataResult.Success;
        }

        public void Update()
        {
            
        }
    }
}
