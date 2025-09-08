using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OpenNGS.Platform
{
    // PlatformEnvironment.cs
    public enum PlatformEnvironment
    {
        Development,
        Production
    }

    [Serializable]
    public class EnvironmentConfig
    {
        public string EEGamesAuthUrl;
        public string EEGamesAvatarUrl;
        public string EEGamesReportUrl;
        public string EEGamesNoticeUrl;
    }

    [Serializable]
    public class PlatformSettings
    {
        [SerializeField]
        private PlatformEnvironment currentEnvironment = PlatformEnvironment.Development;

        [SerializeField]
        private EnvironmentConfig developmentConfig = new EnvironmentConfig();

        [SerializeField]
        private EnvironmentConfig productionConfig = new EnvironmentConfig();

        public PlatformEnvironment CurrentEnvironment
        {
            get => currentEnvironment;
            set => currentEnvironment = value;
        }

        public EnvironmentConfig GetCurrentConfig()
        {
            return currentEnvironment == PlatformEnvironment.Development ? developmentConfig : productionConfig;
        }

        public void SetCurrentConfig(EnvironmentConfig config)
        {
            if (currentEnvironment == PlatformEnvironment.Development)
            {
                developmentConfig = config;
            }
            else
            {
                productionConfig = config;
            }
        }
    }
    [Serializable]
    public class SaveDataSettings
    {
        // 我们不能直接序列化字典，所以使用一个列表来存储平台和其对应的设置
        [SerializeField]
        private List<PlatformSaveDataEntry> platformSettings = new List<PlatformSaveDataEntry>();

        // 内部类，用于在列表中存储键值对
        [Serializable]
        private class PlatformSaveDataEntry
        {
            public BuildTargetGroup platform;
            public PerPlatformSaveData settings;
        }

#if UNITY_EDITOR
        /// <summary>
        /// (仅编辑器) 获取或创建一个指定平台的设置项。
        /// </summary>
        public PerPlatformSaveData GetOrCreateSettingsForPlatform(BuildTargetGroup platform)
        {
            foreach (var entry in platformSettings)
            {
                if (entry.platform == platform)
                {
                    return entry.settings;
                }
            }

            // 如果没找到，就创建一个新的
            var newSettings = new PerPlatformSaveData();
            platformSettings.Add(new PlatformSaveDataEntry { platform = platform, settings = newSettings });
            return newSettings;
        }
#endif

        /// <summary>
        /// (运行时) 获取指定平台的设置项。
        /// </summary>
        public PerPlatformSaveData GetSettingsForPlatform(BuildTargetGroup platform)
        {
            foreach (var entry in platformSettings)
            {
                if (entry.platform == platform)
                {
                    return entry.settings;
                }
            }

            // 运行时如果找不到配置，可以返回 null 或一个默认配置
            Debug.LogWarning($"Save data settings for platform {platform} not found. Returning default settings.");
            return new PerPlatformSaveData();
        }
    }

    [Serializable]
    public class PerPlatformSaveData
    {
        public bool UseAccount = true;
        public uint SavePathType = 0;
        // 你也可以在这里加入之前讨论过的 SteamRootLocation 枚举
        // public SteamPathHelper.SteamRootLocation RootLocation = SteamPathHelper.SteamRootLocation.WinMyDocuments;
    }


    public class OpenNgsSettings : ScriptableObject
    {
        public const string k_SettingsPath = "Assets/Resources/Settings/OpenNgsSettings.asset";

        [SerializeField]
        private PlatformSettings platformSettings = new PlatformSettings();

        [SerializeField]
        private SaveDataSettings saveDataSettings = new SaveDataSettings();

        public PlatformSettings PlatformSettings => platformSettings;
        public SaveDataSettings SaveDataSettings => saveDataSettings;
    }
}