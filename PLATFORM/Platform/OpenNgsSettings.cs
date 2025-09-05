using UnityEngine;
using System;

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
        public bool UseAccount;
        public string RootPath;
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