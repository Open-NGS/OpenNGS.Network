using UnityEngine;
using System.IO;

namespace OpenNGS.SaveData.Setting
{
    public static class SaveSettingManager
    {
        private const string SaveSettingsPath = "SaveSettingData.json";
        private static SaveSettingData saveSettings;

        public static SaveSettingData SaveSettingData
        {
            get
            {
                if (saveSettings == null)
                {
                    saveSettings = LoadSettingsFromFile(SaveSettingsPath);
                }
                return saveSettings;
            }
        }

        private static string _GetRootPath(string filePath)
        {
            string strPath = Path.Combine(Application.streamingAssetsPath,"SaveSetting", filePath);
            return strPath;
        }
        public static SaveSettingData LoadSettingsFromFile(string filePath)
        {
            string strPath = _GetRootPath(filePath);
            SaveSettingData _retSettingData = null;
            if (System.IO.File.Exists(strPath))
            {
                try
                {
                    string json = System.IO.File.ReadAllText(strPath);
                    _retSettingData = JsonUtility.FromJson<SaveSettingData>(json);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Failed to load settings from {strPath}: {ex.Message}");
                }
            }
            else
            {
                try
                {
                    // 确保目录存在
                    string directory = Path.GetDirectoryName(strPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                        Debug.Log($"Created directory: {directory}");
                    }

                    // 创建默认设置数据
                    SaveSettingData defaultSettings = new SaveSettingData();

                    // 写入默认数据到文件
                    string json = JsonUtility.ToJson(defaultSettings, true);
                    System.IO.File.WriteAllText(strPath, json);

#if UNITY_EDITOR
                    UnityEditor.AssetDatabase.Refresh();
#endif

                    Debug.Log($"Created default settings file: {strPath}");
                    _retSettingData = defaultSettings;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Failed to create default settings file: {ex.Message}");
                }
            }

            return _retSettingData;
        }

        public static void SaveSettingsToFile(SaveSettingData settings, string strPath)
        {
            string filePath = _GetRootPath(strPath);
            try
            {
                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonUtility.ToJson(settings, true);
                System.IO.File.WriteAllText(filePath, json);

#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif

                Debug.Log($"Settings saved to: {filePath}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to save settings to {filePath}: {ex.Message}");
            }
        }

        public static void SaveSaveSettings(SaveSettingData _settingData, string strFile)
        {
            SaveSettingsToFile(_settingData, strFile);
        }

        public static SavePlatformSetting GetActiveSaveSetting(uint nPlatform)
        {
            SavePlatformSetting _setting = saveSettings.LstSettings.Find(item => item.PlatformID == nPlatform);
            return _setting;
        }
    }
}