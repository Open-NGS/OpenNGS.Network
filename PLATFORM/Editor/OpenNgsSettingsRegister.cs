#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace OpenNGS.Platform
{
    static class OpenNgsSettingsRegister
    {
        // 共享的辅助方法，用于获取或创建唯一的设置文件
        private static OpenNgsSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<OpenNgsSettings>(OpenNgsSettings.k_SettingsPath);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<OpenNgsSettings>();

                string directoryPath = Path.GetDirectoryName(OpenNgsSettings.k_SettingsPath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                AssetDatabase.CreateAsset(settings, OpenNgsSettings.k_SettingsPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        // --- 第一个 Provider: Platform Settings ---
        [SettingsProvider]
        public static SettingsProvider CreatePlatformSettingsProvider()
        {
            var provider = new SettingsProvider("Project/OpenNgsSetting/PlatformSettings", SettingsScope.Project)
            {
                label = "Platform Settings",
                guiHandler = (searchContext) =>
                {
                    var settings = GetOrCreateSettings();
                    var serializedObject = new SerializedObject(settings);

                    EditorGUI.BeginChangeCheck();

                    var platformSettingsProp = serializedObject.FindProperty("platformSettings");

                    // 环境选择下拉框
                    var currentEnvProp = platformSettingsProp.FindPropertyRelative("currentEnvironment");
                    EditorGUILayout.PropertyField(currentEnvProp, new GUIContent("Current Environment"));

                    EditorGUILayout.Space(10);

                    // 显示当前环境的配置
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    var currentConfig = settings.PlatformSettings.GetCurrentConfig();

                    currentConfig.EEGamesAuthUrl = EditorGUILayout.TextField("EEGamesAuthURL", currentConfig.EEGamesAuthUrl);
                    currentConfig.EEGamesAvatarUrl = EditorGUILayout.TextField("EEGamesAvatarURL", currentConfig.EEGamesAvatarUrl);
                    currentConfig.EEGamesReportUrl = EditorGUILayout.TextField("EEGamesReportURL", currentConfig.EEGamesReportUrl);
                    currentConfig.EEGamesNoticeUrl = EditorGUILayout.TextField("EEGamesNoticeURL", currentConfig.EEGamesNoticeUrl);

                    settings.PlatformSettings.SetCurrentConfig(currentConfig);
                    EditorGUILayout.EndVertical();

                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(settings);
                        AssetDatabase.SaveAssets();
                    }
                },
                keywords = new HashSet<string>(new[] { "OpenNGS", "Platform", "URL", "Environment" })
            };

            return provider;
        }

        // --- 第二个 Provider: Save Data Settings ---
        [SettingsProvider]
        public static SettingsProvider CreateSaveDataSettingsProvider()
        {
            var provider = new SettingsProvider("Project/OpenNgsSetting/SaveDataSettings", SettingsScope.Project)
            {
                label = "Save Data Settings",
                guiHandler = (searchContext) =>
                {
                    var settings = GetOrCreateSettings();
                    var serializedObject = new SerializedObject(settings);

                    EditorGUI.BeginChangeCheck();

                    var saveDataSettingsProp = serializedObject.FindProperty("saveDataSettings");
                    EditorGUILayout.PropertyField(saveDataSettingsProp.FindPropertyRelative("UseAccount"), new GUIContent("Use Account"));
                    EditorGUILayout.PropertyField(saveDataSettingsProp.FindPropertyRelative("RootPath"), new GUIContent("Root Path"));

                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(settings);
                        AssetDatabase.SaveAssets();
                    }
                },
                keywords = new HashSet<string>(new[] { "OpenNGS", "Save", "Data", "Account", "Path" })
            };

            return provider;
        }
    }
}
#endif