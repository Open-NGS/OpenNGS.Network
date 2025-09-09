#if UNITY_EDITOR
using OpenNGS.Core;
using OpenNGS.SaveData.Setting;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OpenNGS.Platform
{
    class SaveSettingRegister
    {
        private static List<BuildTargetGroup> supportedPlatforms;
        private static GUIContent[] platformToolbarIcons;
        private static int selectedPlatformIndex = 0;
        private static SaveSettingData m_settingData = null;
        private static string[] SaveTypeOptions = Enum.GetNames(typeof(SaveSettingType));
        private static List<BuildTarget> m_lstBuildTarget = new List<BuildTarget>()
        {
            BuildTarget.StandaloneWindows, BuildTarget.iOS, BuildTarget.Android, BuildTarget.PS4, BuildTarget.PS5
        };

        [SettingsProvider]
        public static SettingsProvider CreateSaveDataSettingsProvider()
        {
            var provider = new SettingsProvider("Project/OpenNgsSetting/SaveDataSettings", SettingsScope.Project)
            {
                label = "Save Data Settings",
                guiHandler = (searchContext) =>
                {
                    InitializeSupportedPlatforms();

                    string strFileName = "SaveSettingData.json";
                    m_settingData = SaveSettingManager.LoadSettingsFromFile(strFileName);
                    _PlatformInit(m_settingData);
                    if (supportedPlatforms.Count == 0)
                    {
                        EditorGUILayout.HelpBox("No build support modules are installed.", MessageType.Warning);
                        return;
                    }

                    selectedPlatformIndex = GUILayout.Toolbar(selectedPlatformIndex, platformToolbarIcons, GUILayout.Height(30));
                    EditorGUILayout.Space(10);

                    BuildTargetGroup currentPlatform = supportedPlatforms[selectedPlatformIndex];

                    EditorGUILayout.Space(5);

                    EditorGUI.BeginChangeCheck();

                    bool bNeedSave = _DrawSettingsList(m_settingData, "Save Data Settings", currentPlatform);

                    if (EditorGUI.EndChangeCheck())
                    {
                        SaveSettingManager.SaveSaveSettings(m_settingData, strFileName);
                    }
                },
                keywords = new HashSet<string>(new[] { "OpenNGS", "Save", "Data", "Storage", "Platform" })
            };

            return provider;
        }


        private static void _PlatformInit(SaveSettingData _settingData)
        {
            if(_settingData.LstSettings == null)
            {
                _settingData.LstSettings = new List<SavePlatformSetting>();
            }
            foreach (BuildTargetGroup _targetGroup in supportedPlatforms)
            {
                uint nGroup = (uint)_targetGroup;
                SavePlatformSetting _platformSetting = _settingData.LstSettings.Find(item => item.PlatformID == (uint)_targetGroup);
                if (_platformSetting == null)
                {
                    _platformSetting = new SavePlatformSetting((uint)_targetGroup, false, SaveSettingType.SaveGame);
                    _settingData.LstSettings.Add(_platformSetting);
                }
            }
        }

        private static bool _DrawSettingsList(SaveSettingData _settingData, string title, BuildTargetGroup currentPlatform)
        {
            bool bRes = false;
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            SavePlatformSetting _findPlatform = _settingData.LstSettings.Find(item => item.PlatformID == (uint)currentPlatform);
            if(_findPlatform == null)
            {
                _findPlatform = new SavePlatformSetting((uint)currentPlatform, false, SaveSettingType.SaveGame);
                _settingData.LstSettings.Add(_findPlatform);
            }
            if (_findPlatform != null)
            {
                DrawSettingField(_findPlatform, currentPlatform);

                EditorGUILayout.Space(10);
            }
            return bRes;
        }

        private static void DrawSettingField(SavePlatformSetting setting, BuildTargetGroup currentPlatform)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            setting.UseAccount = EditorGUILayout.Toggle(nameof(setting.UseAccount), setting.UseAccount);

            DrawEnumValueField(setting);
            EditorGUILayout.EndVertical();
        }

        private static void DrawEnumValueField(SavePlatformSetting baseSetting)
        {
            if (SaveTypeOptions == null || SaveTypeOptions.Length == 0)
            {
                EditorGUILayout.HelpBox("No enum options defined.", MessageType.Warning);
                return;
            }

            uint currentValue = (uint)baseSetting.SettingType;

            int selectedIndex = EditorGUILayout.Popup(nameof(baseSetting.SettingType), (int)currentValue, SaveTypeOptions);
            baseSetting.SettingType = (SaveSettingType)selectedIndex;
        }

        private static void InitializeSupportedPlatforms()
        {
            if (supportedPlatforms != null) return;

            supportedPlatforms = new List<BuildTargetGroup>();
            var icons = new List<GUIContent>();

            foreach (BuildTargetGroup group in System.Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (group == BuildTargetGroup.Unknown) continue;

                foreach (BuildTarget target in m_lstBuildTarget)
                {
                    if (BuildPipeline.IsBuildTargetSupported(group, target))
                    {
                        if (!supportedPlatforms.Contains(group))
                        {
                            supportedPlatforms.Add(group);
                            string platformName = ObjectNames.NicifyVariableName(group.ToString());
                            Texture2D platformIcon = EditorGUIUtility.FindTexture("BuildSettings." + group.ToString() + ".Small");
                            icons.Add(new GUIContent(platformName, platformIcon));
                        }
                    }
                }
            }
            platformToolbarIcons = icons.ToArray();
        }
    }
}
#endif