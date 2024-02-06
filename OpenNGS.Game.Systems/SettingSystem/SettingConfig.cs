using OpenNGS.Systems;
using OpenNGS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenNGS.Audio;
using OpenNGS.Setting.Data;
using OpenNGS.Setting.Common;
using Systems;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using System.Globalization;

public class SettingConfig :GameSubSystem<SettingConfig>
{

    private static ISaveSystem _saveSystem;

    static GameObject go ;
    static List<AudioSettingInfo> audio;
    static bool framesInfo;
    static RESOLUTIONRATION_TYPE resolution;

    static Dictionary<string,string> keys = new Dictionary<string,string>();
    static List<KeyControlSettingInfo> KeysList;
    static KeyControlSettingInfo KeysCont;

    static SettingSaveData settingData;
    static Dictionary<uint,SettingInfo> settingInfo;
    protected override void OnCreate()
    {
        _saveSystem = App.GetService<ISaveSystem>();

        settingInfo = NGSStaticData.setting;
        settingData = _saveSystem.GetFileData("SETTING") as SettingSaveData;

        audio = settingData.AudioInfo;
        resolution = settingData.Resoulution;
        framesInfo = settingData.FramesInfo.VerticalSynchronization;
        KeysList = settingData.KeyControlInfo;
        base.OnCreate();
    }

    public override string GetSystemName()
    {
        throw new System.NotImplementedException();
    }

    public static bool MusicOn
    {
        get 
        {
            if (settingData == null) { return GetAudio(ADUIO_TYPE.ADUIO_TYPE_MUSIC); }
            return audio[(int)ADUIO_TYPE.ADUIO_TYPE_MUSIC].Switch;
        }
        set
        {
            audio[(int)ADUIO_TYPE.ADUIO_TYPE_MUSIC].Switch = value;
            settingData.AudioInfo[(int)ADUIO_TYPE.ADUIO_TYPE_MUSIC].Switch = value;
            SoundManager.Instance.MusicOn = value;
        }
    }
    public static bool SoundOn
    {
        get { if (settingData == null) { return GetAudio(ADUIO_TYPE.ADUIO_TYPE_SOUND); } return audio[(int)ADUIO_TYPE.ADUIO_TYPE_SOUND].Switch; }
        set
        {
            audio[(int)ADUIO_TYPE.ADUIO_TYPE_SOUND].Switch = value;
            settingData.AudioInfo[(int)ADUIO_TYPE.ADUIO_TYPE_SOUND].Switch = value;
            SoundManager.Instance.SoundOn = value;
        }
    }

    public static bool OverallOn
    {
        get { if (settingData == null) { return GetAudio(ADUIO_TYPE.ADUIO_TYPE_OVERALLAudio); } return audio[(int)ADUIO_TYPE.ADUIO_TYPE_OVERALLAudio].Switch; }
        set
        {
            audio[(int)ADUIO_TYPE.ADUIO_TYPE_OVERALLAudio].Switch = value;
            settingData.AudioInfo[(int)ADUIO_TYPE.ADUIO_TYPE_OVERALLAudio].Switch = value;
            SoundManager.Instance.OverallOn = value;

        }
    }
    public static bool VoiceOn
    {
        get { if (settingData == null) { return GetAudio(ADUIO_TYPE.ADUIO_TYPE_VOICE); } return audio[(int)ADUIO_TYPE.ADUIO_TYPE_VOICE].Switch; }
        set
        {
            audio[(int)ADUIO_TYPE.ADUIO_TYPE_VOICE].Switch = value;
            settingData.AudioInfo[(int)ADUIO_TYPE.ADUIO_TYPE_VOICE].Switch = value;
            SoundManager.Instance.OverallOn = value;
        }
    }
   
    public static bool VerticalSync
    {
        get { if (settingData == null) { return GetVorticalSyn(); } return framesInfo; }
        set 
        {
            framesInfo = value; 
            settingData.FramesInfo.VerticalSynchronization = value;
        }
    }

    public static int MusicVolume
    {
        get { if (settingData == null) { GetSound(ADUIO_TYPE.ADUIO_TYPE_MUSIC); } return (int)audio[(int)ADUIO_TYPE.ADUIO_TYPE_MUSIC].Value; }

        set
        {
            audio[(int)ADUIO_TYPE.ADUIO_TYPE_MUSIC].Value = (uint)value;
            settingData.AudioInfo[(int)ADUIO_TYPE.ADUIO_TYPE_MUSIC].Value = (uint)value;
            AudioSystem.Instance.SetVolume(go,value);
        }
    }
    public static int SoundVolume
    {
        get { if (settingData == null) { GetSound(ADUIO_TYPE.ADUIO_TYPE_SOUND); } return (int)audio[(int)ADUIO_TYPE.ADUIO_TYPE_SOUND].Value; }
        set
        {
            audio[(int)ADUIO_TYPE.ADUIO_TYPE_SOUND].Value = (uint)value;
            settingData.AudioInfo[(int)ADUIO_TYPE.ADUIO_TYPE_SOUND].Value = (uint)value;
            AudioSystem.Instance.SetVolume(go, value);
        }
    }
    public static int OverallVolume
    {
        get { if (settingData == null) { GetSound(ADUIO_TYPE.ADUIO_TYPE_OVERALLAudio); } return (int)audio[(int)ADUIO_TYPE.ADUIO_TYPE_OVERALLAudio].Value; }
        set
        {
            audio[(int)ADUIO_TYPE.ADUIO_TYPE_OVERALLAudio].Value = (uint)value;
            settingData.AudioInfo[(int)ADUIO_TYPE.ADUIO_TYPE_OVERALLAudio].Value = (uint)value;
            AudioSystem.Instance.SetVolume(go, value);
        }
    }
    public static int VoiceVolume
    {
        get { if (settingData == null) { GetSound(ADUIO_TYPE.ADUIO_TYPE_VOICE); } return (int)audio[(int)ADUIO_TYPE.ADUIO_TYPE_VOICE].Value; }
        set
        {
            audio[(int)ADUIO_TYPE.ADUIO_TYPE_VOICE].Value = (uint)value;
            settingData.AudioInfo[(int)ADUIO_TYPE.ADUIO_TYPE_VOICE].Value = (uint)value;
            AudioSystem.Instance.SetVolume(go, value);
        }
    }

    public static void KeyControl(string keyName,string keyValue)
    {
        if (KeysList.Exists(r => r.KeyName == keyName))
        {
            for (int i = 0; i < KeysList.Count; i++)
            {
                if(KeysList[i].KeyName == keyName)
                {
                    KeysList[i].KeyName = keyName;
                    KeysList[i].Key = keyValue;
                }
            }
        }
        else
        {
            KeysCont = new KeyControlSettingInfo();
            KeysCont.KeyName = keyName;
            KeysCont.Key = keyValue;
            KeysList.Add(KeysCont);
        }
        settingData.KeyControlInfo = KeysList;
    }

    public static RESOLUTIONRATION_TYPE Resolution
    {
        get { if (settingData == null) { return ResolutionRatios(); } return resolution; }
        set
        {
            resolution = value;
        }
    }

    // 默认音效开关
    public static bool GetAudio(ADUIO_TYPE _TYPE)
    {
        foreach (var item in settingInfo.Values)
        {
             return item.audioSettingInfo[(int)_TYPE].Switch;
        }
        return false;
    }
    // 默认音效大小
    public static int GetSound(ADUIO_TYPE _TYPE)
    {
        foreach (var item in settingInfo.Values)
        {
            return (int)item.audioSettingInfo[(int)_TYPE].Value;
        }
        return 0;
    }

    // 默认垂直同步
    public static bool GetVorticalSyn()
    {
        foreach (var item in settingInfo.Values)
        {
            return item.FramesInfo.VerticalSynchronization;
        }
        return false;
    }

    // 默认语言
    public static RESOLUTIONRATION_TYPE ResolutionRatios()
    {
        foreach (var item in settingInfo.Values)
        {
            return item.ResolutionRatios;
        }
        return 0;
    }

    // 默认按键控制
    public List<KeyControlSettingInfo> KeyControl()
    {
        foreach (var item in settingInfo.Values)
        {
            return item.keyControlSettingInfo;
        }
       return new List<KeyControlSettingInfo>();
    }
    public static void Save()
    {
        _saveSystem.SettingSaveFile(settingData);
    }
}
