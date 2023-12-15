using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenNGS.Extension;
using OpenNGS.IO;

namespace OpenNGS.Configs
{
    public interface IConfig
    {
        void SetDefault();
    }

    public enum ConfigType
    {
        AppConfig = 0,
        UserConfig = 1,
    }

    public class UserConfig<T> : ConfigBase<T> where T : IConfig, new()
    {
        public static bool Read(string name)
        {
            return Read(name, ConfigType.UserConfig);
        }

        public static void Write(string name)
        {
            Write(name, ConfigType.UserConfig);
        }
    }

    public class AppConfig<T> : ConfigBase<T> where T : IConfig, new()
    {
        private string configFileName = "Config.txt";
        public static bool Read(string name)
        {
            return Read(name, ConfigType.AppConfig);
        }

        public static void Write(string name)
        {
            Write(name, ConfigType.AppConfig);
        }
    }

    public class ConfigBase<T> where T : IConfig, new()
    {
        protected static T config;
        public static T Config
        {
            get { return config; }
        }

        protected static bool Read(string name, ConfigType type)
        {
            var configPath = Path.Combine(FileSystem.DataPath, name);
            if (FileSystem.FileExists(configPath))
            {
                string json = File.ReadAllText(configPath, Encoding.UTF8);
                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        config = JsonConvert.DeserializeObject<T>(json);
                    }
                    catch (Exception ex)
                    {
                        //  throw new Exception(string.Format("Config:{0} read error : {1}", configPath, ex.Message));
                    }
                }
            }
            if (config == null)
            {
                config = new T();
                config.SetDefault();
                return false;
            }
            return true;
        }

        private static bool IsJsonPropertyChanged(string json)
        {
            var jObject = JObject.Parse(json);
            int jObjectPropertyCount = 0;
            foreach (var item in jObject.Properties())
            {
                ++jObjectPropertyCount;
            }
            Type t = typeof(T);
            return jObjectPropertyCount != t.GetProperties().Length;
        }


        protected static void Write(string name, ConfigType type)
        {
            //string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            var configPath = FileSystem.DataPath;
            //if (!string.IsNullOrEmpty(configPath))
            //{
            //    try
            //    {
            //        if (!Directory.Exists(configPath))
            //        {
            //            Directory.CreateDirectory(configPath);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception(string.Format("Can not create folder: {0} {1}", configPath, ex.Message));
            //    }
            //}
            FileExtension.WriteJson(Path.Combine(configPath,name), config, Formatting.Indented);
            //File.WriteAllText(Path.Combine(configPath,name), json, Encoding.UTF8);
        }
    }
}
