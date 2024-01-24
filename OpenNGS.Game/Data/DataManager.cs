using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenNGS.Tables;
using System.IO;
using Data;
using OpenNGS.IO;
using OpenNGS.Serialization;

public class DataManager : OpenNGS.Singleton<DataManager>
{
    const string DataPath = "data";
    const string Ext = ".bin";


    private static List<ITable> globalTables = new List<ITable>();

    private static List<ITable> seasonTables = new List<ITable>();

    public class TableSerializer : ISerializer
    {
        public T Deserialize<T>(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                T instance = global::ProtoBuf.Serializer.Deserialize<T>(ms);
                return instance;
            }
        }
    }

    public DataManager()
    {
        OpenNGSTable.Serializer = new TableSerializer();
    }

    public string GetDataFile(string name, bool season)
    {
        if (season)
        {
            var filename = Path.Combine(Application.streamingAssetsPath, DataPath, SeasonManager.Instance.CurrentSeason.ToString(), name + Ext);
            if (!FileSystem.FileExists(filename))
            {
                name = name.ToLower();
                filename = Path.Combine(Application.streamingAssetsPath, DataPath, SeasonManager.Instance.CurrentSeason.ToString(), name + Ext);
            }
            return filename;
        }
        else
        {
            var filePath = Path.Combine(Application.streamingAssetsPath, DataPath, name + Ext);
            if (!FileSystem.FileExists(filePath))
            {
                name = name.ToLower();
                filePath = Path.Combine(Application.streamingAssetsPath, DataPath, name + Ext);

            }

            return filePath;
        }
    }
    internal void AddTable(ITable table)
    {
        if (!table.IsSeasonTable)
            globalTables.Add(table);
        else
            seasonTables.Add(table);
    }

    public void Init()
    {
        this.LoadGlobalTables();
    }

    /// <summary>
    ///  加载全局配置表,游戏启动时
    /// </summary>
    public void LoadGlobalTables()
    {
        foreach(var table in globalTables)
        {
            table.Load();
        }
    }

    /// <summary>
    /// 加载当前赛季配置表，进入游戏时
    /// </summary>
    public void LoadSeasonTables()
    {
        foreach (var table in seasonTables)
        {
            table.Load();
        }
    }

    public void Clear()
    {
        globalTables.Clear();
        seasonTables.Clear();
    }
}
