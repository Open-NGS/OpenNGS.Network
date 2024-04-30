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
        //获得画面数据
        public VerticalSynchronization GetVerticals();
        // 获得音频数据
        public List<AudioSettingInfo> GetAudioSetting();
    // 获得按键数据
        public List<KeyControlSettingInfo> GetKeyControl();
    // 获得语言数据
        public Language GetLanguage();
        // 获得分辨率数据
        public ResolutionRatios GetResolution();


        public void SetVertical(VerticalSynchronization state);
        public void SetAudio(AudioSettingInfo audio);
        public void SetKeyControl(KeyControlSettingInfo keyControl);
        public void SetLanguage(Language language);
        public void SetResolution(ResolutionRatios resolution);

        public void AddSettingContainer(SettingContainer container);

    }
}
