using OpenNGS.Tables;
using System.IO;
using OpenNGS.IO;
using OpenNGS.Serialization;
using System.Collections.Generic;
using fastJSON;



namespace OpenNGS
{
    public class DataManager : OpenNGS.Singleton<DataManager>
    {
        const string DataPath = "data";
        //const string Ext = ".bin";
        const string Ext = ".json";

        public static string FileExt = "";


        private static List<ITable> globalTables = new List<ITable>();

        private static List<ITable> seasonTables = new List<ITable>();

        public class TablePBBinSerializer : ISerializer
        {
            public TablePBBinSerializer()
            {
                DataManager.FileExt = ".bin";
            }
            public T Deserialize<T>(byte[] data)
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    T instance = global::ProtoBuf.Serializer.Deserialize<T>(ms);
                    return instance;
                }
            }
        }

        public class TablePBJsonSerializer : ISerializer
        {
            public TablePBJsonSerializer()
            {
                DataManager.FileExt = ".json";
            }
            public T Deserialize<T>(byte[] data)
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    T instance = global::ProtoBuf.Serializer.Deserialize<T>(ms);
                    return instance;
                }
            }
        }
        public class TableSerializerJson : ISerializer
        {
            public TableSerializerJson()
            {
                DataManager.FileExt = ".json";
            }
            public T Deserialize<T>(byte[] data)
            {
                bool bProcessBom = false;
                if(data != null && data.Length > 3)
                {
                    if (data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF)
                    {
                        bProcessBom = true;
                    }
                }
                string json = "";
                if(bProcessBom == true)
                {
                    json = System.Text.Encoding.UTF8.GetString(data, 3, data.Length - 3);
                }
                else
                {
                    json = System.Text.Encoding.UTF8.GetString(data);
                }
                
                try
                {
                    fastJSON.JSONParameters jp = new fastJSON.JSONParameters();
                    jp.UseValuesOfEnums = true;
                    jp.EnableAnonymousTypes = true;
                    T instance2 = JSON.ToObject<T>(json, jp);
                    return instance2;
                }
                catch (System.Exception e)
                {
                    int a = 0;
                }
                T instance = JSON.ToObject<T>(json);
                return instance;
            }
        }

        public DataManager()
        {
        }

        public string GetDataFile(string name, bool season)
        {
            if (season)
            {
                var filename = System.IO.Path.Combine(FileSystem.StreamingAssetsPath, DataPath, SeasonManager.Instance.CurrentSeason.ToString(), name + DataManager.FileExt);
                if (!FileSystem.FileExists(filename))
                {
                    name = name.ToLower();
                    filename = System.IO.Path.Combine(FileSystem.StreamingAssetsPath, DataPath, SeasonManager.Instance.CurrentSeason.ToString(), name + DataManager.FileExt);
                }
                return filename;
            }
            else
            {
                var filePath = System.IO.Path.Combine(FileSystem.StreamingAssetsPath, DataPath, name + DataManager.FileExt);
                if (!FileSystem.FileExists(filePath))
                {
                    name = name.ToLower();
                    filePath = System.IO.Path.Combine(FileSystem.StreamingAssetsPath, DataPath, name + DataManager.FileExt);

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

        public void Init(ISerializer _serializer)
        {
            DataTable.Serializer = _serializer;
            this.LoadGlobalTables();
        }

        /// <summary>
        ///  加载全局配置�?游戏启动�?
        /// </summary>
        public void LoadGlobalTables()
        {
            foreach (var table in globalTables)
            {
                table.Load();
            }
        }

        /// <summary>
        /// 加载当前赛季配置表，进入游戏�?
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
}