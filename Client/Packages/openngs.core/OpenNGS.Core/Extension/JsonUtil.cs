using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using OpenNGS.IO;
using UnityEngine;
using File = System.IO.File;

namespace OpenNGS.Extension
{
    public static class JsonUtil
    {
        public static void WriteJson(string path, object obj, Formatting fmt = Formatting.None)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = fmt;
            settings.NullValueHandling = NullValueHandling.Ignore;

            var json = JsonConvert.SerializeObject(obj, settings);
            // File.WriteAllText(path, json);
            FileSystem.Write(path, System.Text.Encoding.UTF8.GetBytes(json));
            
            Debug.Log($"WriteJson:{path}");
        }
        
        public static T LoadJson<T>(string path)
        {
            var dataAsJson = FileSystem.ReadAllText(path);
            // Debug.LogError($"[FileExtension]: {dataAsJson}");
            return JsonConvert.DeserializeObject<T>(dataAsJson);
        }
    }
}
