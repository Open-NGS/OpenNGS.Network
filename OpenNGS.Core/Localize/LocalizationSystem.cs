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

        private const string LocalizationCodeFilePath = "Localization/{0}/CodeLocalization.json";
        private const string DefaultLocalizationCodeFilePath = "Localization/CodeLocalization.json";

        private Dictionary<string, string> _localizationStrings = new Dictionary<string, string>();

        private Dictionary<SystemLanguage, string> SysLanguageToIETF = null;


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
            _initIETF();
        }

        private void LoadLocalizationFile()
        {
            string file = Path.Combine(Application.streamingAssetsPath, string.Format(LocalizationFilePath, GetLangName(Lan)));
            if (!FileSystem.FileExists(file))
            {
                file = Path.Combine(Application.streamingAssetsPath, DefaultLocalizationFilePath);
            }

            _localizationStrings = JsonUtil.LoadJson<Dictionary<string, string>>(file);

            // code
            file = Path.Combine(Application.streamingAssetsPath, string.Format(LocalizationCodeFilePath, GetLangName(Lan)));
            if (!FileSystem.FileExists(file))
            {
                file = Path.Combine(Application.streamingAssetsPath, DefaultLocalizationCodeFilePath);
            }
            if (FileSystem.FileExists(file))
            {
                Dictionary<string,string> _codeString = JsonUtil.LoadJson<Dictionary<string, string>>(file);

                foreach (KeyValuePair<string, string> _codePair in _codeString)
                {
                    if (_localizationStrings.ContainsKey(_codePair.Key) == false)
                    {
                        _localizationStrings.Add(_codePair.Key, _codePair.Value);
                    }
                }
            }
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

        public string GetCurrentIETF()
        {
            if( SysLanguageToIETF != null)
            {
                if(SysLanguageToIETF.ContainsKey(_lan))
                {
                    return SysLanguageToIETF[_lan];
                }
            }
            return SysLanguageToIETF[SystemLanguage.ChineseSimplified];
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

        private void _initIETF()
        {
            SysLanguageToIETF = new Dictionary<SystemLanguage, string>
        {
            { SystemLanguage.Japanese, "ja" },
            { SystemLanguage.English, "en" },
            { SystemLanguage.French, "fr" },
            { SystemLanguage.German, "de" },
            { SystemLanguage.Italian, "it" },
            { SystemLanguage.Spanish, "es" },
            { SystemLanguage.ChineseSimplified, "zh" },
            { SystemLanguage.ChineseTraditional, "zh-Hant" },
            { SystemLanguage.Korean, "ko" },
            { SystemLanguage.Dutch, "nl" },
            { SystemLanguage.Portuguese, "pt" },
            { SystemLanguage.Russian, "ru" },
            { SystemLanguage.Afrikaans, "af" },
            { SystemLanguage.Arabic, "ar" },
            { SystemLanguage.Basque, "eu" },
            { SystemLanguage.Belarusian, "be" },
            { SystemLanguage.Bulgarian, "bg" },
            { SystemLanguage.Catalan, "ca" },
            { SystemLanguage.Czech, "cs" },
            { SystemLanguage.Danish, "da" },
            { SystemLanguage.Estonian, "et" },
            { SystemLanguage.Faroese, "fo" },
            { SystemLanguage.Finnish, "fi" },
            { SystemLanguage.Greek, "el" },
            { SystemLanguage.Hebrew, "he" },
            { SystemLanguage.Icelandic, "is" },
            { SystemLanguage.Indonesian, "id" },
            { SystemLanguage.Latvian, "lv" },
            { SystemLanguage.Lithuanian, "lt" },
            { SystemLanguage.Norwegian, "no" },
            { SystemLanguage.Polish, "pl" },
            { SystemLanguage.Romanian, "ro" },
            { SystemLanguage.SerboCroatian, "hr" },
            { SystemLanguage.Slovak, "sk" },
            { SystemLanguage.Slovenian, "sl" },
            { SystemLanguage.Swedish, "sv" },
            { SystemLanguage.Thai, "th" },
            { SystemLanguage.Turkish, "tr" },
            { SystemLanguage.Ukrainian, "uk" },
            { SystemLanguage.Vietnamese, "vi" },
            { SystemLanguage.Hungarian, "hu" }
        };
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
                    langName = "zh-Hant";
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
                case SystemLanguage.Vietnamese:
                    langName = "vi";
                    break;
                case SystemLanguage.Italian:
                    langName = "it";
                    break;
            }
            return langName;
        }

    }
}
