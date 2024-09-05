using Codice.Client.BaseCommands.BranchExplorer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.SaveData.File
{
    public class SaveDataFilePB<T> : SaveDataFile where T : class,new ()
    {
        public T Value { get; set; }

        public SaveDataFilePB(SaveData save, string filename) : base(save, filename)
        {
            Value = new T();
        }

        protected override byte[] GetData()
        {
            using (MemoryStream ds = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize<T>(ds, Value);
                return ds.ToArray();
            }
        }

        protected override void SetData(byte[] data)
        {
            using (MemoryStream ds = new MemoryStream(data))
            {
                Value = ProtoBuf.Serializer.Deserialize<T>(ds);
            }
        }
    }
}
