using UnityEngine;
using System;
using System.Collections.Generic;

namespace OpenNGS.Platform
{

    [Serializable]
    public class EnvironmentConfig
    {
        public string EEGamesAuthUrl;
        public string EEGamesAvatarUrl;
        public string EEGamesReportUrl;
        public string EEGamesNoticeUrl;
    }
    // PlatformEnvironment.cs
    public enum PlatformEnvironment
    {
        Development,
        Production
    }
    [Serializable]

    public class PlatformSettings : ScriptableObject
    {
        public const string k_MyCustomSettingsPath = "Assets/Settings/PlatformSettings.asset";

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

    public class PlatformSettingsManager
    {
        private static PlatformSettings settings;

        public static void Initialize()
        {
#if UNITY_EDITOR
            // 在编辑器中直接从 Assets 文件夹加载
            settings = UnityEditor.AssetDatabase.LoadAssetAtPath<PlatformSettings>(PlatformSettings.k_MyCustomSettingsPath);
#else
            // 在运行时从 Resources 加载
            settings = Resources.Load<PlatformSettings>("Settings/PlatformSettings");
#endif

            if (settings == null)
            {
                Debug.LogError($"Failed to load Platform Settings from {PlatformSettings.k_MyCustomSettingsPath}!");
            }
        }

        public static PlatformEnvironment GetCurrentEnvironment()
        {
            return settings.CurrentEnvironment;
        }

        public static string GetPlatformAuthUrl()
        {
            var config = settings.GetCurrentConfig();
            return config?.EEGamesAuthUrl;
        }

        public static string GetPlatformAvatarUrl()
        {
            var config = settings.GetCurrentConfig();
            return config?.EEGamesAvatarUrl;
        }
        public static string GetPlatformReportUrl()
        {
            var config = settings.GetCurrentConfig();
            return config?.EEGamesReportUrl;
        }
    }
}