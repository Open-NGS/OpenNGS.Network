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
//using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

public class SettingSystem : EntitySystem
{
    public GetSettingRsq OnGetSetting;
    public UnityAction<Dictionary<uint, GameSettingLabel>>OnGameSettingLabel;
    public Dictionary<uint, GameSettingLabel> OnGameSettingLabels;

    public UnityAction<List<AudioSettingInfo>> Onaudio; // 音频
    public UnityAction<List<KeyControlSettingInfo>> OnkeyControl;  // 按键
    public UnityAction<OpenNGS.Setting.Data.ScreenSettingInfo> Onscreen; // 画面
    public UnityAction<OpenNGS.Setting.Data.Language> Onlanguage; //语言

    public Dictionary<uint, AudioSettingInfo> sadsadsadsInfo; 


    public SettingInfo settingInfo;

    public AudioSettingInfo audio;
    public KeyControlSettingInfo keyControl;
    public Language language;

    public string pathName = "";

    public void Init()
    {
        if (OnGameSettingLabels == null) { OnGameSettingLabels = Table<GameSettingLabel, uint>.map; }
    }

    // 设置标签
    public void GetGameSettingLabel()
    {
        OnGameSettingLabel.Invoke(OnGameSettingLabels);
    }

    // 获得音频
    public void GetAduio()
    {
        Onaudio.Invoke(settingInfo.audioSettingInfo);
    }
   
    // 修改音效
    public void SetAudio(uint value,bool start,OpenNGS.Setting.Common.ADUIO_TYPE _TYPE)
    {
        settingInfo.audioSettingInfo[(int)_TYPE].AduioType = _TYPE;
        settingInfo.audioSettingInfo[(int)_TYPE].Value = value;
        settingInfo.audioSettingInfo[(int)_TYPE].Switch = start;
    }
    // 获得按键
    public void GetKeyControl()
    {
        OnkeyControl.Invoke(settingInfo.keyControlSettingInfo);
    }

    // 修改按键
    public void SetKeyControl(OpenNGS.Setting.Common.KEYVALUE_CONTROL_TYPE _TYPE,string key)
    {
        settingInfo.keyControlSettingInfo[(int)_TYPE].Key = key;
        settingInfo.keyControlSettingInfo[(int)_TYPE].KeyType = _TYPE;
    }

    // 获得分辨率
    public void GetResolutionRatio()
    {
        Onscreen.Invoke(OnGetSetting.SettingInfo.screenSettingInfo);
    }

    // 修改垂直同步
    public void SetVerticalSynchronization(bool start)
    {
        settingInfo.screenSettingInfo.VerticalSynchronization = start;
    }

    // 修改分辨率
    public void SetResolutionRatio(OpenNGS.Setting.Common.RESOLUTIONRATION_TYPE _TYPE)
    {
        settingInfo.screenSettingInfo.ResolutionRatio = _TYPE;
    }
    // 获取语言
    public void GetLanguage()
    {
        Onlanguage.Invoke(OnGetSetting.SettingInfo.language);
    }

    // 修改语言
    public void SetLanguage(OpenNGS.Setting.Common.LANGUAGE_TYPE _TYPE)
    {
        settingInfo.language.languageType = _TYPE;
    }

    // 保存
    public void Save()
    {
        MakeFile(pathName);
        SendGameSetting(settingInfo);
    }

    // 本地Save
    public void MakeFile(string pathName)
    {
        if (File.Exists(pathName))
        {
            File.Delete(pathName);
            FileStream NewText = File.Create(pathName);
            NewText.Close();
        }
        else
        {
            FileStream NewText = File.Create(pathName);
            NewText.Close();
        }
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

            Onaudio.Invoke(rsp.SettingInfo.audioSettingInfo);
            OnkeyControl.Invoke(rsp.SettingInfo.keyControlSettingInfo);
            Onscreen.Invoke(rsp.SettingInfo.screenSettingInfo);
            Onlanguage.Invoke(rsp.SettingInfo.language);
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
