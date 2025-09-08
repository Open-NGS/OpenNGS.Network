using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
            var saveData = GetCurrentPlatformSaveData();
            return saveData != null ? saveData.UseAccount : true; // 返回一个安全的默认值
        }

        public static uint GetSaveDataPathType()
        {
            var saveData = GetCurrentPlatformSaveData();
            return saveData != null ? saveData.SavePathType : 0; // 返回一个安全的默认值
        }


        /// <summary>
        /// 辅助函数：将当前的运行时平台映射到 BuildTargetGroup。
        /// </summary>
        private static BuildTargetGroup GetCurrentBuildTargetGroup()
        {
#if UNITY_EDITOR
            // 在编辑器中，我们返回当前在 Build Settings 窗口中选中的平台
            return EditorUserBuildSettings.selectedBuildTargetGroup;
#else
            // 在实际构建的版本中，根据运行时平台返回对应的组
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsServer:
                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.LinuxServer:
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXServer:
                    return BuildTargetGroup.Standalone;
                case RuntimePlatform.Android:
                    return BuildTargetGroup.Android;
                case RuntimePlatform.IPhonePlayer:
                    return BuildTargetGroup.iOS;
                case RuntimePlatform.WebGLPlayer:
                    return BuildTargetGroup.WebGL;
                case RuntimePlatform.PS4:
                    return BuildTargetGroup.PS4;
                case RuntimePlatform.PS5:
                    return BuildTargetGroup.PS5;
                case RuntimePlatform.XboxOne:
                    return BuildTargetGroup.XboxOne;
                // 添加其他你需要的平台...
                default:
                    Debug.LogWarning("Current platform is not explicitly handled. Falling back to Standalone.");
                    return BuildTargetGroup.Standalone;
            }
#endif
        }

        private static PerPlatformSaveData GetCurrentPlatformSaveData()
        {
            if (settings == null) Initialize();
            if (settings == null) return null; // 初始化失败

            BuildTargetGroup currentGroup = GetCurrentBuildTargetGroup();
            return settings.SaveDataSettings.GetSettingsForPlatform(currentGroup);
        }
    }
}