using Dynamic.Data;
using OpenNGS.Setting.Common;
using OpenNGS.Setting.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Systems
{
    public interface ISettingSystem
    {
        public bool SettingInfo();
        //获得画面数据
        public VerticalSynchronizationData GetFrames();
    // 获得音频数据
        public Dictionary<string, AudioSettinData> GetAudioSettin();
    // 获得按键数据
        public Dictionary<string, KeyControlSettingData> GetKeyControl();
    // 获得语言数据
        public Dictionary<string, LanguageData> GetLanguage();

        public void SetFrames(bool state);
        public void SetAudio(string name, int value, bool state);
        public void SetKeyControl(string name, string key);
        public void SetLanguage(string name);
        public void SetResolution(string name);

    }
}
