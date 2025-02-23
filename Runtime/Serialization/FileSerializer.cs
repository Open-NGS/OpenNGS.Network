using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using OpenNGS.IO;
using ProtoBuf;
using File = System.IO.File;

namespace OpenNGS.Serialization
{
    public static class FileSerializer
    {
        public static T Deserialize<T>(byte[] byteArray)
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);
            return Deserialize<T>(stream);
        }
        
        public static T Deserialize<T>(System.IO.MemoryStream stream)
        {
            var t = Serializer.Deserialize<T>(stream);
            stream.Close();
            stream.Dispose();
            return t;
        }
        
        public static void Write<T>(string path, T obj) where T : class, new()
        {
            try
            {
                var stream = new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                Serializer.Serialize<T>(stream, obj);
                stream.Flush();
                stream.Close();
            }
            catch (System.Exception e)
            {
                NgDebug.LogErrorFormat("ProtoLoader.Load Exception: {0}", e);
            }
        }
        
        public static T Load<T>(this string path) where T : class, new()
        {
            try
            {
#if UNITY_ANDROID
                var stream = new System.IO.MemoryStream(FileSystem.Read(path));
#else
                var stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
#endif
                var ret = Serializer.Deserialize<T>(stream);
                stream.Close();
                return ret;
            }
            catch (Exception e)
            {
                NgDebug.LogErrorFormat("ProtoLoader.Load Exception: {0}", e);
            }
            return new T();
        }

        public static void WriteJson(string path, object obj, Formatting fmt = Formatting.None)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = fmt;
            settings.NullValueHandling = NullValueHandling.Ignore;

            var json = JsonConvert.SerializeObject(obj, settings);
            // File.WriteAllText(path, json);
            FileSystem.Write(path, System.Text.Encoding.UTF8.GetBytes(json));

            NgDebug.Log($"WriteJson:{path}");
        }
        
        public static T LoadJson<T>(string path)
        {
            var dataAsJson = FileSystem.ReadAllText(path);
            // Debug.LogError($"[FileSerializer]: {dataAsJson}");
            return JsonConvert.DeserializeObject<T>(dataAsJson);
        }
    }
}
