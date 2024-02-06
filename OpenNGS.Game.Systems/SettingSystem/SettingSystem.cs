using OpenNGS.Audio;
using OpenNGS.Collections.Generic;
using OpenNGS.Rank.Data;
using OpenNGS.Setting.Common;
using OpenNGS.Setting.Data;
using OpenNGS.Systems;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;

//using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

public class SettingSystem : EntitySystem, ISettingSystem
{
    public GetSettingRsq OnGetSetting;

    public string pathName = "";
    GameObject go = new GameObject();

    // 整体音效
    public void OverAllToogle(bool on)
    {
        SettingConfig.OverallOn = on;
    }

    // 音效
    public void SoundToogle(bool on)
    {
        SettingConfig.SoundOn = on;
    }

    // 音乐
    public void MusicToogle(bool on)
    {
        SettingConfig.MusicOn = on;
    }

    // 语音
    public void VoiceToogle(bool on)
    {
        SettingConfig.VoiceOn = on;
    }

    // 音乐大小
    public void MusicVolume(float vol)
    {
        SettingConfig.MusicVolume = (int)vol;
    }

    // 音效大小
    public void SoundVolume(float vol)
    {
        SettingConfig.SoundVolume = (int)vol;
    }

    // 整体音频
    public void OverallVolume(float vol)
    {
        SettingConfig.OverallVolume = (int)vol;
    }

    // 语音大小
    public void VoiceVolume(float vol)
    {
        SettingConfig.VoiceVolume = (int)vol;
    }

    // 垂直同步
    public void VerticalSync(bool on)
    {
        SettingConfig.VerticalSync = on;
    }
    
    // 按键控制
    public void KeyControl(string key,string value)
    {
        SettingConfig.KeyControl(key,value);
    }
    // 分辨率
    public void Resolution(RESOLUTIONRATION_TYPE _TYPE)
    {
        SettingConfig.Resolution = _TYPE;
    }

    // 保存
    public void Save()
    {
        SettingConfig.Save();
    }

    #region C2S
    // 发送请求
    public void RequestGameSetting()
    {

    }
    #endregion

    #region S2C
    // 请求响应
    public void OnSettingRsp(GetSettingRsq rsp)
    {
        if (rsp.Result == OpenNGS.Setting.Common.RESULT_TYPE.RESULT_TYPE_SUCCESS)
        {
            OnGetSetting = rsp;
            
        }

        else
        {
            Debug.LogError(""+rsp.Result);
        }
    }
    #endregion

    // 上传数据
    public void SendGameSetting(SettingInfo settingInfo)
    {

    }
    protected override void OnCreate()
    {
        base.OnCreate();

    }

    public override string GetSystemName()
    {
        return "com.openngs.system.GameSetting";
    }

  
}
