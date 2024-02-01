using OpenNGS.Configs;
using OpenNGS.IO.Posix;
using OpenNGS.SaveData;
using OpenNGS.Systems;
using OpenNGS.UI.DataBinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.Systems
{
    public class SaveSystem : EntitySystem, ISaveSystem
    {
        public static string SAVE_ITEM_TAG = "ITEM";
        public static string SAVE_RANK_TAG = "RANK";
        public static string SAVE_CHARACTER_TAG = "CHARACTER";
        public static string SAVE_DIALOG_TAG = "DIALOG";

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        public override string GetSystemName()
        {
            return "com.openngs.system.save";
        }

        private Dictionary<string, ISaveInfo> saveInfo;

        private int Capture;
        private int index;

        public void Init(int capture, int version)
        {
            PosixFileSystem fs = new PosixFileSystem();
            SaveDataManager<SaveData>.Instance.Init(fs, capture, version);
            this.Capture = capture;
            this.index = SaveDataManager<SaveData>.Instance.ActiveIndex;
            saveInfo = new Dictionary<string, ISaveInfo>();

            if (SaveDataManager<SaveData>.Instance.GetSlot(index) == null)
            {
                this.index = 0;
                InitFilt();
            }

            InitDicInfo();
        }

        private void InitFilt()
        {
            SaveDataManager<SaveData>.Instance.ActiveIndex = index;
            SaveDataManager<SaveData>.Instance.NewSaveData(true);
        }

        private void InitDicInfo()
        {
            SaveData saveData = SaveDataManager<SaveData>.Instance.GetSlot(index).SaveData.Data;
            saveInfo.Clear();

            saveInfo[SAVE_ITEM_TAG] = saveData.saveItems;
            saveInfo[SAVE_RANK_TAG] = saveData.saveRanks;
            saveInfo[SAVE_CHARACTER_TAG] = saveData.charaInfos;
            saveInfo[SAVE_DIALOG_TAG] = saveData.dialogData;
        }

        public bool AddFile()
        {
            if (index + 1 >= Capture)
            {
                return false;
            }
            SaveFile();
            index++;
            SaveDataManager<SaveData>.Instance.ActiveIndex = index;
            SaveDataManager<SaveData>.Instance.NewSaveData(true);
            InitDicInfo();
            return true;
        }

        public bool DeleteFile(int targeIndex)
        {
            if (targeIndex < 0 || targeIndex >= Capture) return false;
            
            SaveDataManager<SaveData>.Instance.Delete(targeIndex);
            return true;
        }

        public void SaveFile()
        {
            SaveDataManager<SaveData>.Instance.GetSlot(index).SaveData.Data.saveItems = saveInfo[SAVE_ITEM_TAG] as ItemData;
            SaveDataManager<SaveData>.Instance.GetSlot(index).SaveData.Data.saveRanks = saveInfo[SAVE_RANK_TAG] as RankData;
            SaveDataManager<SaveData>.Instance.GetSlot(index).SaveData.Data.charaInfos = saveInfo[SAVE_CHARACTER_TAG] as CharacterSaveData;
            SaveDataManager<SaveData>.Instance.GetSlot(index).SaveData.Data.dialogData = saveInfo[SAVE_DIALOG_TAG] as DialogData;
            SaveDataManager<SaveData>.Instance.Save();
        }

        public bool ChangeFile(int targeIndex)
        {
            if (targeIndex < 0 || targeIndex >= Capture) return false;
            this.index = targeIndex;
            SaveDataManager<SaveData>.Instance.ActiveIndex = targeIndex;
            InitDicInfo();
            return true;
        }

        public void SetFileData(string name, ISaveInfo data)
        {
            if (data == null) return;
            saveInfo[name] = data;
        }

        public ISaveInfo GetFileData(string name)
        {
            if (name == null) return null;
            ISaveInfo data = null;
            if (!saveInfo.TryGetValue(name, out data)) return null;
            return data;
        }
    }
}