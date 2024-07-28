using OpenNGS;
using OpenNGS.Setting.Data;
using System;
using System.Collections;
using System.Collections.Generic;

public class SettingLocalAPI : Singleton<SettingLocalAPI>, ISettingClientAPI
{
    INgSettingSystem m_NgSettingSystem;
    public void Init()
    {
        m_NgSettingSystem = App.GetService<INgSettingSystem>();
    }
    public void AddActionOnSettingChange(Action<int, int> ac)
    {
        m_NgSettingSystem.AddActionOnSettingChange(ac);
    }

    public void AddSettingContainer(UserSettingContainer container)
    {
        m_NgSettingSystem.AddSettingContainer(container);
    }

    public void ChangeSetting(int settingType, int value)
    {
        m_NgSettingSystem.ChangeSetting(settingType, value);
    }

    public UserSettingValueState GetSetting(int settingType)
    {
        return m_NgSettingSystem.GetSetting(settingType);
    }
}
