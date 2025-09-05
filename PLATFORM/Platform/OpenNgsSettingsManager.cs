using UnityEngine;

namespace OpenNGS.Platform
{
    public class OpenNgsSettingsManager
    {
        private static OpenNgsSettings settings;

        public static void Initialize()
        {
            if (settings != null) return;

#if UNITY_EDITOR
            // 在编辑器中直接从 Assets 文件夹加载
            settings = UnityEditor.AssetDatabase.LoadAssetAtPath<OpenNgsSettings>(OpenNgsSettings.k_SettingsPath);
#else
            // 在运行时从 Resources 加载
            string resourcesPath = OpenNgsSettings.k_SettingsPath;
            if (resourcesPath.StartsWith("Assets/Resources/"))
            {
                // 去掉 "Assets/Resources/" 前缀
                resourcesPath = resourcesPath.Substring("Assets/Resources/".Length);
            }
            // 去掉文件扩展名
            int lastDotIndex = resourcesPath.LastIndexOf('.');
            if (lastDotIndex != -1)
            {
                resourcesPath = resourcesPath.Substring(0, lastDotIndex);
            }
            settings = Resources.Load<OpenNgsSettings>(resourcesPath);
#endif

            if (settings == null)
            {
                Debug.LogError($"Failed to load OpenNGS Settings from {OpenNgsSettings.k_SettingsPath}!");
            }
        }

        public static PlatformEnvironment GetCurrentEnvironment()
        {
            if (settings == null) Initialize();
            return settings.PlatformSettings.CurrentEnvironment;
        }

        public static string GetPlatformAuthUrl()
        {
            if (settings == null) Initialize();
            var config = settings.PlatformSettings.GetCurrentConfig();
            return config?.EEGamesAuthUrl;
        }

        public static string GetPlatformAvatarUrl()
        {
            if (settings == null) Initialize();
            var config = settings.PlatformSettings.GetCurrentConfig();
            return config?.EEGamesAvatarUrl;
        }

        public static string GetPlatformReportUrl()
        {
            if (settings == null) Initialize();
            var config = settings.PlatformSettings.GetCurrentConfig();
            return config?.EEGamesReportUrl;
        }

        public static string GetPlatformNoticeUrl()
        {
            if (settings == null) Initialize();
            var config = settings.PlatformSettings.GetCurrentConfig();
            return config?.EEGamesNoticeUrl;
        }

        public static bool GetUseAccount()
        {
            if (settings == null) Initialize();
            return settings.SaveDataSettings.UseAccount;
        }

        public static string GetRootPath()
        {
            if (settings == null) Initialize();
            return settings.SaveDataSettings.RootPath;
        }
    }
}