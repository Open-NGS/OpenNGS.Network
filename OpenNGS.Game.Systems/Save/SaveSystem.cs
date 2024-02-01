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

        public void Init(int capture, int version)
        {
            PosixFileSystem fs = new PosixFileSystem();
            SaveDataManager<SaveData>.Instance.Init(fs, capture, version);
            saveInfo = new Dictionary<string, ISaveInfo>();

            if (SaveDataManager<SaveData>.Instance.Current == null)
            {
                SaveDataManager<SaveData>.Instance.NewSaveData(true);
            }

            InitDicInfo();
        }

        private void InitDicInfo()
        {
            SaveData saveData = SaveDataManager<SaveData>.Instance.Current;
            saveInfo.Clear();

            saveInfo[SAVE_ITEM_TAG] = saveData.saveItems;
            saveInfo[SAVE_RANK_TAG] = saveData.saveRanks;
            saveInfo[SAVE_CHARACTER_TAG] = saveData.charaInfos;
        }

        public void AddFile()
        {
            SaveFile();
            SaveDataManager<SaveData>.Instance.ActiveIndex++;
            SaveDataManager<SaveData>.Instance.NewSaveData(true);
            InitDicInfo();
        }

        public void DeleteFile(int targeIndex)
        {
            SaveDataManager<SaveData>.Instance.Delete(targeIndex);
        }

        public void SaveFile()
        {
            SaveDataManager<SaveData>.Instance.Current.saveItems = saveInfo[SAVE_ITEM_TAG] as ItemData;
            SaveDataManager<SaveData>.Instance.Current.saveRanks = saveInfo[SAVE_RANK_TAG] as RankData;
            SaveDataManager<SaveData>.Instance.Current.charaInfos = saveInfo[SAVE_CHARACTER_TAG] as CharacterSaveData;
            SaveDataManager<SaveData>.Instance.Current.dialogData = saveInfo[SAVE_DIALOG_TAG] as DialogData;
            SaveDataManager<SaveData>.Instance.Save();
        }

        public bool ChangeFile(int targeIndex)
        {
            if (targeIndex < 0 || targeIndex >= SaveDataManager<SaveData>.Instance.Capacity)
                return false;

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