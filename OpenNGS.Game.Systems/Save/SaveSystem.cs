using OpenNGS.Configs;
using OpenNGS.IO.Posix;
using OpenNGS.SaveData;
using OpenNGS.Systems;
using OpenNGS.UI.DataBinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : EntitySystem, ISaveSystem
{
    
    protected override void OnCreate()
    {
        base.OnCreate();
        Init(10, 1);
    }

    public override string GetSystemName()
    {
        return "com.openngs.system.save";
    }

    private Dictionary<string, ISaveInfo> saveInfo;

    private int Capture;
    private int index;
    private SaveData saveData;

    public void Init(int capture, int version)
    {
        PosixFileSystem fs = new PosixFileSystem();
        SaveDataManager<SaveData>.Instance.Init(fs, capture, version);
        this.Capture = capture;
        this.index = SaveDataManager<SaveData>.Instance.ActiveIndex;
        saveInfo = new Dictionary<string, ISaveInfo>();

        if (SaveDataManager<SaveData>.Instance.Current == null)
        {
            this.index = 0;
            AddFile();
        }

        InfoInit();
    }

    private void InfoInit()
    {
        saveData = SaveDataManager<SaveData>.Instance.GetSlot(index).SaveData.Data;
        saveInfo.Clear();

        saveInfo["item"] = saveData.saveItems;
        saveInfo["rank"] = saveData.saveRanks;
    }

    public bool AddFile()
    {
        if (index + 1 >= Capture)
        {
            return false;
        }
        SaveDataManager<SaveData>.Instance.NewSaveData(true);
        InfoInit();
        //savefile();
        return true;
    }

    public bool DeleteFile(int targeIndex)
    {
        if (targeIndex < 0 || targeIndex >= Capture) return false;
        if (targeIndex == index)
        {
            ChangeFile(targeIndex - 1);
        }
        SaveDataManager<SaveData>.Instance.Delete(targeIndex);
        return true;
    }

    public void SaveFile()
    {
        SaveDataManager<SaveData>.Instance.Current.saveItems = saveInfo["item"] as ItemData;
        SaveDataManager<SaveData>.Instance.Current.saveRanks = saveInfo["item"] as RankData;

        SaveDataManager<SaveData>.Instance.Save();
    }

    public bool ChangeFile(int targeIndex)
    {
        if (targeIndex < 0 || targeIndex >= Capture) return false;
        this.index = targeIndex;
        SaveDataManager<SaveData>.Instance.ActiveIndex = targeIndex;
        InfoInit();
        return true;
    }

    public void SetFileData(string name, ISaveInfo data)
    {
        if (data == null) return;
        saveInfo[name] = data;
        SaveFile();
    }

    public ISaveInfo GetFileData(string name)
    {
        if (name == null) return null;
        ISaveInfo data = null;
        if (!saveInfo.TryGetValue(name, out data)) return null;
        return data;
    }
}
