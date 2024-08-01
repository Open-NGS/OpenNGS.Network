using OpenNGS;
using OpenNGS.Setting.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingService : Singleton<SettingService>
{
    ISettingClientAPI m_NgSettingClientAPI;
    public void Init(ISettingClientAPI settingClientAPI)
    {
        m_NgSettingClientAPI = settingClientAPI;
    }
    public void AddActionOnSettingChange(Action<int, int> ac)
    {
        m_NgSettingClientAPI.AddActionOnSettingChange(ac);
    }

    public void AddSettingContainer(UserSettingContainer container)
    {
        m_NgSettingClientAPI.AddSettingContainer(container);
    }

    public void ChangeSetting(int settingType, int value)
    {
        m_NgSettingClientAPI.ChangeSetting(settingType, value);
    }

    public UserSettingValueState GetSetting(int settingType)
    {
        return m_NgSettingClientAPI.GetSetting(settingType);
    }
}
