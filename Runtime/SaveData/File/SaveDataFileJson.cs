using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.SaveData.File
{
    public class SaveDataFileJson<T> : SaveDataFile where T : class,new()
    {
        public T Value { get; set; }


        public SaveDataFileJson(SaveData save, string Dirname) : base(save, Dirname)
        {
            Value = new T();
        }

        protected override byte[] GetData()
        {
            var json = JsonConvert.SerializeObject(Value);
            return Encoding.UTF8.GetBytes(json);
        }

        protected override void SetData(byte[] data)
        {
            var json = Encoding.UTF8.GetString(data);
            Value = JsonConvert.DeserializeObject<T>(json);
        }
    }
}
