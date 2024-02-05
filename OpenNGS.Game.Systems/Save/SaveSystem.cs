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
        public static string SAVE_TECHNOLOGY_TAG = "TECHNOLOGY";


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
            SaveDataManager<SaveFileData>.Instance.Init(fs, capture, version);
            saveInfo = new Dictionary<string, ISaveInfo>();

            if (SaveDataManager<SaveFileData>.Instance.Current == null)
            {
                SaveDataManager<SaveFileData>.Instance.NewSaveData(true);
            }

            InitDicInfo();
        }

        private void InitDicInfo()
        {
            SaveFileData saveData = SaveDataManager<SaveFileData>.Instance.Current;
            saveInfo.Clear();

            saveInfo[SAVE_ITEM_TAG] = saveData.saveItems;
            saveInfo[SAVE_RANK_TAG] = saveData.saveRanks;
            saveInfo[SAVE_CHARACTER_TAG] = saveData.charaInfos;
            saveInfo[SAVE_DIALOG_TAG] = saveData.dialogData;
            saveInfo[SAVE_TECHNOLOGY_TAG] = saveData.technologyData;
        }

        public void AddFile()
        {
            SaveFile();
            SaveDataManager<SaveFileData>.Instance.ActiveIndex++;
            SaveDataManager<SaveFileData>.Instance.NewSaveData(true);
            InitDicInfo();
        }

        public void DeleteFile(int targeIndex)
        {
            SaveDataManager<SaveFileData>.Instance.Delete(targeIndex);
        }

        public void SaveFile()
        {
            SaveDataManager<SaveFileData>.Instance.Current.saveItems = saveInfo[SAVE_ITEM_TAG] as SaveFileData_Item;
            SaveDataManager<SaveFileData>.Instance.Current.saveRanks = saveInfo[SAVE_RANK_TAG] as SaveFileData_Rank;
            SaveDataManager<SaveFileData>.Instance.Current.charaInfos = saveInfo[SAVE_CHARACTER_TAG] as SaveFileData_Character;
            SaveDataManager<SaveFileData>.Instance.Current.dialogData = saveInfo[SAVE_DIALOG_TAG] as SaveFileData_Dialog;
            SaveDataManager<SaveFileData>.Instance.Current.technologyData = saveInfo[SAVE_TECHNOLOGY_TAG] as SaveFileData_Technology;
            SaveDataManager<SaveFileData>.Instance.Save();
        }

        public bool ChangeFile(int targeIndex)
        {
            if (targeIndex < 0 || targeIndex >= SaveDataManager<SaveFileData>.Instance.Capacity)
                return false;

            SaveDataManager<SaveFileData>.Instance.ActiveIndex = targeIndex;
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