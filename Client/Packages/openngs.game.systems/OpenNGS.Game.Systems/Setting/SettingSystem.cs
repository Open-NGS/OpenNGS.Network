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
    public OpenNGS.Setting.Data.VerticalSynchronizationData GetVerticals()
    {
        return _container.Vertical;
    }

    // 音频
    public List<AudioSettingData> GetAudioSetting()
    {
        return _container.Audio;
    }

    // 按键
    public List<OpenNGS.Setting.Data.KeyControlSettingData> GetKeyControl()
    {
        return _container.KeyControl;
    }

    // 语言
    public OpenNGS.Setting.Data.LanguageData GetLanguage()
    {
        return _container.Language;
    }
    // 分辨率
    public OpenNGS.Setting.Data.ResolutionRatiosData GetResolution()
    {
        return _container.ResolutionRatios;
    }



    public void SetVertical(OpenNGS.Setting.Data.VerticalSynchronizationData state)
    {
        //_container.SetVertical(state);
    }

    public void SetAudio(AudioSettingData audio)
    {
        //_container.SetAudio(audio);
    }
    public void SetKeyControl(OpenNGS.Setting.Data.KeyControlSettingData keyControl)
    {
        //_container.SetKeyControl(keyControl);
    }

    public void SetLanguage(OpenNGS.Setting.Data.LanguageData language)
    {
        //_container.SetLanguage(language);
    }

    public void SetResolution(OpenNGS.Setting.Data.ResolutionRatiosData resolution)
    {
        //_container.SetResolutionRatios(resolution);
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

    //#region S2C
    //// 请求响应
    //public void OnSettingRsp(GetSettingRsq rsp)
    //{
    //    if (rsp.Result == OpenNGS.Setting.Common.RESULT_TYPE.RESULT_TYPE_SUCCESS)
    //    {
    //        OnGetSetting = rsp;
    //    }

    //    else
    //    {
    //        //Debug.LogError(""+rsp.Result);
    //    }
    //}
    //#endregion

    // 上传数据
    public override string GetSystemName()
    {
        return "com.openngs.system.GameSetting";
    }

  
}
