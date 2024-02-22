using Dynamic.Data;
using OpenNGS;
using OpenNGS.Setting.Data;
using OpenNGS.Systems;
using System.Collections.Generic;
using Systems;
using UnityEngine;

public class SettingSystem : GameSubSystem<SettingSystem>, ISettingSystem
{
    public GetSettingRsq OnGetSetting;
    private ISaveSystem m_saveSystem = null;
    public SaveFileData_Setting _setting = null;

    protected override void OnCreate()
    {
        m_saveSystem = App.GetService<ISaveSystem>();
        base.OnCreate();
        GetSetting();
    }

    // 获取存档数据
    public void GetSetting()
    {
        ISaveInfo saveInfo = m_saveSystem.GetFileData("SETTING");
        if (saveInfo != null && saveInfo is SaveFileData_Setting)
        {
            _setting = (SaveFileData_Setting)saveInfo;
        }
        else
        {
            _setting = new SaveFileData_Setting();
        }
    }

    public bool SettingInfo()
    {
        return _setting != null;
    }

    // 垂直同步
    public VerticalSynchronizationData GetFrames()
    {
        return _setting._frames;
    }

    // 音频
    public Dictionary<string, AudioSettinData> GetAudioSettin()
    {
        return _setting._audio;
    }

    // 按键
    public Dictionary<string, KeyControlSettingData> GetKeyControl()
    {
        return _setting._keyControl;
    }

    // 语言
    public Dictionary<string, LanguageData> GetLanguage()
    {
        return _setting._language;
    }
    // 分辨率
    public ResolutionRatiosData GetResolution()
    {
        return _setting.resolution;
    }



    public void SetFrames(bool state)
    {
        _setting._frames.state = state;
        m_saveSystem.SetFileData("SETTING", _setting);
    }

    public void SetAudio(string name,int value,bool state)
    {
        AudioSettinData audio = new AudioSettinData();
        audio.AduioName = name;
        audio.Value = (uint)value;
        audio.Switch = state;
        if (_setting._audio.ContainsValue(audio))
        {
            _setting._audio[name] = audio;
        }
        else
        {
            _setting._audio.Add(name,audio);
        }
        m_saveSystem.SetFileData("SETTING", _setting);

    }
    public void SetKeyControl(string name,string key)
    {
        KeyControlSettingData keyControl = new KeyControlSettingData();
        keyControl.KeyName = name;
        keyControl.Key = key;
        if (_setting._keyControl.ContainsValue(keyControl))
        {
            _setting._keyControl[name] = keyControl;
        }
        else
        {
            _setting._keyControl.Add(name, keyControl);
        }
        m_saveSystem.SetFileData("SETTING", _setting);

    }

    public void SetLanguage(string name)
    {
        LanguageData language = new LanguageData();
        language.languageName = name;
        if (_setting._language.ContainsValue(language))
        {
            _setting._language[name] = language;
        }
        else
        {
            _setting._language.Add(name, language);
        }
        m_saveSystem.SetFileData("SETTING", _setting);

    }

    public void SetResolution(string name)
    {
        ResolutionRatiosData resolution = new ResolutionRatiosData();
        resolution.ResName = name;
        m_saveSystem.SetFileData("SETTING", _setting);
    }

    public void SaveFirst(VerticalSynchronizationData verticals, Dictionary<string, AudioSettinData> Audios, Dictionary<string, KeyControlSettingData> keyControls, Dictionary<string, LanguageData> languages, ResolutionRatiosData resolutions)
    {
        _setting._frames = verticals;
        _setting._audio = Audios;
        _setting._keyControl = keyControls;
        _setting.resolution = resolutions;
        m_saveSystem.SetFileData("SETTING", _setting);
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
    public void SendGameSetting(SaveFileData_Setting settingData)
    {

    }
    public override string GetSystemName()
    {
        return "com.openngs.system.GameSetting";
    }

  
}
