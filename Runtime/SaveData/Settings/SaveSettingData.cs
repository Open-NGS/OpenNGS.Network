using System;
using System.Collections.Generic;

namespace OpenNGS.SaveData.Setting
{
    [Serializable]
    public enum SaveSettingType
    {
        SaveGame,
        Persistent,
        Install
    }
    [Serializable]
    public class SaveSettingData
    {
        public List<SavePlatformSetting> LstSettings;
    }
    [Serializable]
    public class SavePlatformSetting
    {
        public uint PlatformID;
        public bool UseAccount;
        public SaveSettingType SettingType;
        public SavePlatformSetting(uint nPlatformID, bool bUseAccount, SaveSettingType _typ)
        {
            PlatformID = nPlatformID;
            UseAccount = bUseAccount;
            SettingType = _typ;
        }
    }
}