#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;

namespace OpenNGS.Platform
{
    static class OpenNgsSettingsRegister
    {
        // 缓存检测到的、已安装构建支持的平台
        private static List<BuildTargetGroup> supportedPlatforms;
        private static GUIContent[] platformToolbarIcons;
        private static int selectedPlatformIndex = 0;

        private static List<BuildTarget> m_lstBuildTarget = new List<BuildTarget>() 
        { BuildTarget.StandaloneWindows, BuildTarget.iOS,BuildTarget.Android,BuildTarget.PS4,BuildTarget.PS5};

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


        // --- 更新 Save Data Settings Provider ---
        [SettingsProvider]
        public static SettingsProvider CreateSaveDataSettingsProvider()
        {
            var provider = new SettingsProvider("Project/OpenNgsSetting/SaveDataSettings", SettingsScope.Project)
            {
                label = "Save Data Settings",
                guiHandler = (searchContext) =>
                {
                    // 确保平台列表已初始化
                    InitializeSupportedPlatforms();

                    var settings = GetOrCreateSettings();

                    if (supportedPlatforms.Count == 0)
                    {
                        EditorGUILayout.HelpBox("No build support modules are installed. Please install a module (e.g., Windows, Android, iOS) via the Unity Hub to see platform-specific settings.", MessageType.Warning);
                        return;
                    }

                    // 绘制平台选项卡工具栏
                    selectedPlatformIndex = GUILayout.Toolbar(selectedPlatformIndex, platformToolbarIcons, GUILayout.Height(30));

                    EditorGUILayout.Space(10);

                    // 获取当前选中平台的 BuildTargetGroup
                    BuildTargetGroup currentPlatform = supportedPlatforms[selectedPlatformIndex];

                    // 获取或创建该平台的配置数据
                    PerPlatformSaveData platformData = settings.SaveDataSettings.GetOrCreateSettingsForPlatform(currentPlatform);

                    EditorGUI.BeginChangeCheck();

                    // 绘制该平台的具体设置
                    platformData.UseAccount = EditorGUILayout.Toggle(new GUIContent("Use Account", "Enable account-based save data."), platformData.UseAccount);

                    OPENNGS_SAVE_DATA_PATH_TYPE _selectEnum = (OPENNGS_SAVE_DATA_PATH_TYPE)platformData.SavePathType;
                    _selectEnum = (OPENNGS_SAVE_DATA_PATH_TYPE)EditorGUILayout.EnumPopup(new GUIContent("Save Data Path Type", "选择保存数据路径的类型"), _selectEnum);
                    platformData.SavePathType = (uint)_selectEnum;

                    if (EditorGUI.EndChangeCheck())
                    {
                        // 标记 ScriptableObject 为已修改状态，以便保存
                        EditorUtility.SetDirty(settings);
                        AssetDatabase.SaveAssets();
                    }
                },
                keywords = new HashSet<string>(new[] { "OpenNGS", "Save", "Data", "Account", "Path", "Platform" })
            };

            return provider;
        }

        /// <summary>
        /// 初始化并扫描所有支持的平台。
        /// </summary>
        private static void InitializeSupportedPlatforms()
        {
            if (supportedPlatforms != null) return;

            supportedPlatforms = new List<BuildTargetGroup>();
            var icons = new List<GUIContent>();

            // 遍历所有可能的 BuildTargetGroup
            foreach (BuildTargetGroup group in System.Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (group == BuildTargetGroup.Unknown) continue;

                // 检查这个平台组的构建支持是否已安装
                foreach(BuildTarget _target in m_lstBuildTarget)
                {
                    if (BuildPipeline.IsBuildTargetSupported(group, _target))
                    {
                        supportedPlatforms.Add(group);
                        string platformName = ObjectNames.NicifyVariableName(group.ToString());
                        Texture2D platformIcon = EditorGUIUtility.FindTexture("BuildSettings." + group.ToString() + ".Small");
                        icons.Add(new GUIContent(platformName, platformIcon));
                    }
                }
            }
            platformToolbarIcons = icons.ToArray();
        }
    }
}
#endif