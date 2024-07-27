using OpenNGS.Setting.Data;
using System.Collections.Generic;


namespace OpenNGS.Systems
{
    public interface ISettingSystem
    {
        //获得画面数据
        public OpenNGS.Setting.Data.VerticalSynchronizationData GetVerticals();
        // 获得音频数据
        public List<AudioSettingData> GetAudioSetting();
        // 获得按键数据
        public List<OpenNGS.Setting.Data.KeyControlSettingData> GetKeyControl();
        // 获得语言数据
        public OpenNGS.Setting.Data.LanguageData GetLanguage();
        // 获得分辨率数据
        public OpenNGS.Setting.Data.ResolutionRatiosData GetResolution();


        public void SetVertical(OpenNGS.Setting.Data.VerticalSynchronizationData state);

        public void SetAudio(AudioSettingData audio);

        public void SetKeyControl(OpenNGS.Setting.Data.KeyControlSettingData keyControl);


        public void SetLanguage(OpenNGS.Setting.Data.LanguageData language);

        public void SetResolution(OpenNGS.Setting.Data.ResolutionRatiosData resolution);

        public void AddSettingContainer(SettingContainer container);

    }
}
