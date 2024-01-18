using OpenNGS.Rank.Data;
using OpenNGS.Setting.Data;
using OpenNGS.Systems;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.Events;

public class SettingSystem : EntitySystem
{
    public UnityAction<GetSettingRsq> OnGetSetting;

    public UnityAction<List<GameSettingInfo>> OnGetSettingList;

    public Dictionary<uint,GameSettingLabel> OnGameSettingLabels;
    public Dictionary<uint,GameSettingInfo> OnGameSettingInfos;

    public UnityAction OnSave;

    public void Init()
    {
        GetSettingLabel();
        GetSettingInfo();
    }

    public Dictionary<uint, GameSettingLabel> GetSettingLabel()
    {
        if (OnGameSettingLabels == null)
        {
            OnGameSettingLabels = Table<GameSettingLabel, uint>.map;
            return OnGameSettingLabels;
        }
        return OnGameSettingLabels;
    }

    public Dictionary<uint, GameSettingInfo> GetSettingInfo()
    {
        if (OnGameSettingInfos == null)
        {
            OnGameSettingInfos = Table<GameSettingInfo, uint>.map;
            return OnGameSettingInfos;
        }
        return OnGameSettingInfos;
    }

    public void Save(uint characterID, Dictionary<uint, GameSettingInfo> settingInfo)
    {
        if (settingInfo != OnGameSettingInfos)
        {
            SendGameSetting(characterID, settingInfo);
        }
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
    public void SendGameSetting(uint CharacterID, Dictionary<uint, GameSettingInfo> settingInfo)
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
