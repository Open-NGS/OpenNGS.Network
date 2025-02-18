using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.IO;
namespace OpenNGS.Platform
{
    static class PlatformSettingsRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreatePlatformSettingsProvider()
        {
            var provider = new SettingsProvider("Project/Platform Settings", SettingsScope.Project)
            {
                label = "Platform Settings",
                guiHandler = (searchContext) =>
                {
                    var settings = GetOrCreateSettings();
                    var serializedObject = new SerializedObject(settings);

                    EditorGUI.BeginChangeCheck();

                    // 环境选择下拉框
                    EditorGUILayout.Space(10);

                    var newEnvironment = (PlatformEnvironment)EditorGUILayout.EnumPopup(
                        "Current Environment",
                        settings.CurrentEnvironment
                    );

                    // 如果环境改变，先保存当前配置
                    if (newEnvironment != settings.CurrentEnvironment)
                    {
                        settings.CurrentEnvironment = newEnvironment;
                        EditorUtility.SetDirty(settings);
                    }

                    EditorGUILayout.Space(10);

                    // 显示当前环境的配置
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    var currentConfig = settings.GetCurrentConfig();

                    currentConfig.EEGamesAuthUrl = EditorGUILayout.TextField(
                        "EEGamesAuthURL",
                        currentConfig.EEGamesAuthUrl
                    );

                    currentConfig.EEGamesAvatarUrl = EditorGUILayout.TextField(
                        "EEGamesAvatarURL",
                        currentConfig.EEGamesAvatarUrl
                    );

                    currentConfig.EEGamesReportUrl = EditorGUILayout.TextField(
                        "EEGamesReportURL",
                        currentConfig.EEGamesReportUrl
                    );

                    currentConfig.EEGamesNoticeUrl = EditorGUILayout.TextField(
                        "EEGamesNoticeURL",
                        currentConfig.EEGamesNoticeUrl
                    );

                    settings.SetCurrentConfig(currentConfig);
                    EditorGUILayout.EndVertical();

                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(settings);
                        AssetDatabase.SaveAssets();
                    }
                },

                keywords = new HashSet<string>(new[] { "Platform", "URL", "Environment" })
            };

            return provider;
        }

        static PlatformSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<PlatformSettings>(PlatformSettings.k_MyCustomSettingsPath);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<PlatformSettings>();

                string directoryPath = Path.GetDirectoryName(PlatformSettings.k_MyCustomSettingsPath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                AssetDatabase.CreateAsset(settings, PlatformSettings.k_MyCustomSettingsPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }
    }
}