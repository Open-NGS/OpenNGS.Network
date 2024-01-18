using OpenNGS.Rank.Data;
using OpenNGS.Setting.Data;
using OpenNGS.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SettingSystem : EntitySystem
{
    public UnityAction<GetSettingRsq> OnGetSetting;
    public UnityAction<List<GameSettingInfo>> OnGetSettingList;

    protected override void OnCreate()
    {
        base.OnCreate();
    }

    public override string GetSystemName()
    {
        return "com.openngs.system.GameSetting";
    }
    #region C2S
    // 发送请求
    public void RequestGameSetting(uint CharacterID)
    {

    }
    #endregion

    #region S2C
    // 请求响应
    public void OnSettingRsp(GetSettingRsq rsp)
    {
        OnGetSetting?.Invoke(rsp);
        List <GameSettingInfo> settinginfos = new List <GameSettingInfo>();
        OnGetSettingList?.Invoke(settinginfos);
    }
    #endregion

    // 发送
    public void SendGameSetting(uint CharacterID, OpenNGS.Setting.Data.GameSettingInfo settingInfo)
    {

    }
}
