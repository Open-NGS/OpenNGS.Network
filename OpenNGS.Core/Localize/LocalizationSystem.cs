using OpenNGS.Logs.Appenders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using OpenNGS.Extension;
using UnityEngine;
using OpenNGS.IO;
using File = System.IO.File;
using Path = System.IO.Path;

namespace OpenNGS.Localize
{

    public class LocalizationSystem : OpenNGS.Singleton<LocalizationSystem>
    {
        public Action OnLanguageChanged;

        private const string LocalizationFilePath = "Localization/{0}/Localization.json";
        private const string DefaultLocalizationFilePath = "Localization/Localization.json";

        private Dictionary<string, string> _localizationStrings = new Dictionary<string, string>();


        private SystemLanguage _lan;
        public SystemLanguage Lan
        {
            get { return _lan; }
            set { _lan = value; }
        }

        public void Init(SystemLanguage lan = SystemLanguage.ChineseSimplified)
        {
            Lan = lan;
            LoadLocalizationFile();
        }

        private void LoadLocalizationFile()
        {
            string file = Path.Combine(Application.streamingAssetsPath, string.Format(LocalizationFilePath, GetLangName(Lan)));
            if (!FileSystem.FileExists(file))
            {
                file = Path.Combine(Application.streamingAssetsPath, DefaultLocalizationFilePath);
            }

            // var dataAsJson = FileSystem.Read(file).ToString();
            _localizationStrings = JsonUtil.LoadJson<Dictionary<string, string>>(file);
            // var dataAsJson = File.ReadAllText(file);
            // _localizationStrings = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataAsJson);
        }

        public void SetLanguage(SystemLanguage language)
        {
            if (this.Lan != language)
            {
                Lan = language;
                LoadLocalizationFile();
                OnLanguageChanged?.Invoke();
            }
        }

        public string GetText(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return key;
            string value = "";
            if (_localizationStrings != null && !_localizationStrings.TryGetValue(key, out value))
            {
                NgDebug.LogError($"Localize key: {key} not found");
                value = string.Format("Text[{0}]",key);
            }
            return value;
        }

        public static string GetLangName(SystemLanguage lan)
        {
            string langName = "";
            switch (lan)
            {
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.Chinese:
                    langName = "zh-CN";
                    break;
                case SystemLanguage.English:
                    langName = "en";
                    break;
                case SystemLanguage.Japanese:
                    langName = "ja";
                    break;
                case SystemLanguage.ChineseTraditional:
                    langName = "zh-hant";
                    break;
                case SystemLanguage.French:
                    langName = "fr";
                    break;
                case SystemLanguage.German:
                    langName = "de";
                    break;
                case SystemLanguage.Portuguese:
                    langName = "pt";
                    break;
                case SystemLanguage.Spanish:
                    langName = "es";
                    break;
                case SystemLanguage.Russian:
                    langName = "ru";
                    break;
                case SystemLanguage.Arabic:
                    langName = "ar";
                    break;
                case SystemLanguage.Korean:
                    langName = "ko";
                    break;
                case SystemLanguage.Polish:
                    langName = "pl";
                    break;
                case SystemLanguage.Thai:
                    langName = "th";
                    break;
            }
            return langName;
        }

    }
}
