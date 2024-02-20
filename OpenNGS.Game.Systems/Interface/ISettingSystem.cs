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
    // 整体音效
        public bool OverAllToogle(bool on);
    // 音效
        public bool SoundToogle(bool on);
    // 音乐
        public bool MusicToogle(bool on);
    // 语音
        public bool VoiceToogle(bool on);
    // 音乐大小
        public void MusicVolume(float vol);
    // 音效大小
        public void SoundVolume(float vol);
    // 整体音频
        public void OverallVolume(float vol);
    // 语音大小
        public void VoiceVolume(float vol);
    // 垂直同步
        public bool VerticalSync(bool on);
    // 按键控制
        public void KeyControl(string key, string value);
    // 分辨率
        public void Resolution(RESOLUTIONRATION_TYPE _TYPE);
    // 保存
        public void Save();
    }
}
