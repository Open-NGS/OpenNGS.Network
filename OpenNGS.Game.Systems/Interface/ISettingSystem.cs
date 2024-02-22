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
        // 第一次保存

        public void SetVertical(VerticalSynchronizationData state);
        public void SetAudio(AudioSettinData audio);
        public void SetKeyControl(KeyControlSettingData keyControl);
        public void SetLanguage(LanguageData language);
        public void SetResolution(ResolutionRatiosData resolution);
    }
}
