using Dynamic.Data;
using OpenNGS;
using OpenNGS.Item.Data;
using OpenNGS.Setting.Data;
using OpenNGS.Systems;
using System.Collections.Generic;
using Systems;


public class SettingSystem : GameSubSystem<SettingSystem>, ISettingSystem
{
    public GetSettingRsq OnGetSetting;
    private SettingContainer _container = null;

    protected override void OnCreate()
    {
        base.OnCreate();
    }

    // 画面
    public VerticalSynchronization GetVerticals()
    {
        return _container.Vertical;
    }

    // 音频
    public List<AudioSettingInfo> GetAudioSetting()
    {
        return _container.Audio;
    }

    // 按键
    public List<KeyControlSettingInfo> GetKeyControl()
    {
        return _container.KeyControl;
    }

    // 语言
    public Language GetLanguage()
    {
        return _container.Language;
    }
    // 分辨率
    public ResolutionRatios GetResolution()
    {
        return _container.ResolutionRatios;
    }



    public void SetVertical(VerticalSynchronization state)
    {
        _container.SetVertical(state);
    }

    public void SetAudio(AudioSettingInfo audio)
    {
        _container.SetAudio(audio);
    }
    public void SetKeyControl(KeyControlSettingInfo keyControl)
    {
        _container.SetKeyControl(keyControl);
    }

    public void SetLanguage(Language language)
    {
        _container.SetLanguage(language);
    }

    public void SetResolution(ResolutionRatios resolution)
    {
        _container.SetResolutionRatios(resolution);
    }


    public void AddSettingContainer(SettingContainer container)
    {
        if (container != null)
        {
            _container = container;
        }
        else
        {
            _container = new SettingContainer();
        }
    }

    protected override void OnClear()
    {
        _container = null;
        base.OnClear();
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
            //Debug.LogError(""+rsp.Result);
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
