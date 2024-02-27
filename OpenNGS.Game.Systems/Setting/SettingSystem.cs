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
        ISaveInfo saveInfo = m_saveSystem.GetSettingData();
        if (saveInfo != null && saveInfo is SaveFileData_Setting)
        {
            _setting = (SaveFileData_Setting)saveInfo;
        }
    }

    public bool SettingInfo()
    {
        if (_setting._audio.Count != 0)
        {
            return true;
        }
        _setting = new SaveFileData_Setting();
        return false;
    }

    // 垂直同步
    public VerticalSynchronizationData GetFrames()
    {
        return _setting._vertical;
    }

    // 音频
    public Dictionary<string, AudioSettinData> GetAudioSetting()
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



    public void SetVertical(VerticalSynchronizationData state)
    {
        _setting._vertical.state = state.state;
        m_saveSystem.SetSettingData(_setting);
        m_saveSystem.SettingSaveFile();
    }

    public void SetAudio(AudioSettinData audio)
    {
        if (_setting._audio.ContainsKey(audio.AduioName))
        {
            _setting._audio[audio.AduioName] = audio;
        }
        else
        {
            _setting._audio.Add(audio.AduioName, audio);
        }
        m_saveSystem.SetSettingData(_setting);
        m_saveSystem.SettingSaveFile();
    }
    public void SetKeyControl(KeyControlSettingData keyControl)
    {
        if (_setting._keyControl.ContainsKey(keyControl.KeyName))
        {
            _setting._keyControl[keyControl.KeyName] = keyControl;
        }
        else
        {
            _setting._keyControl.Add(keyControl.KeyName, keyControl);
        }
        m_saveSystem.SetSettingData(_setting);
        m_saveSystem.SettingSaveFile();
    }

    public void SetLanguage(LanguageData language)
    {
        if (_setting._language.ContainsKey(language.languageName))
        {
            _setting._language[language.languageName] = language;
        }
        else
        {
            _setting._language.Add(language.languageName, language);
        }
        m_saveSystem.SetSettingData(_setting);
        m_saveSystem.SettingSaveFile();
    }

    public void SetResolution(ResolutionRatiosData resolution)
    {
        resolution.ResName = resolution.ResName;
        m_saveSystem.SetSettingData(_setting);
        m_saveSystem.SettingSaveFile();
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
