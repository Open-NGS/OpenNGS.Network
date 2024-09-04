using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.SaveData.PS5
{

    public enum SaveDataStorageType
    {
        SaveData = 0,
        SaveDataMemory = 1,
        SaveDataWithDialog = 2,
        PlayerPref = 3, // 采用 Unity内置模式
    }

    public static class PS5SaveData
    {
        static internal SaveDataStorageType Mode { get;private set; }
        public static void Init(SaveDataStorageType mdoe)
        {
            Mode = mdoe;
        }
    }
}
