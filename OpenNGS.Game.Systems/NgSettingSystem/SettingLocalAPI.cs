using OpenNGS;
using OpenNGS.Setting.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingLocalAPI : Singleton<SettingLocalAPI>,ISettingClientAPI
{
    INgSettingSystem ngSettingSystem = App.GetService<NgSettingSystem>();
    public void AddActionOnSettingChange(Action<int, int> ac)
    {
        ngSettingSystem.AddActionOnSettingChange(ac);
    }

    public void AddSettingContainer(UserSettingContainer container)
    {
        ngSettingSystem.AddSettingContainer(container);
    }

    public void ChangeSetting(int settingType, int value)
    {
        ngSettingSystem.ChangeSetting(settingType, value);
    }

    public UserSettingValueState GetSetting(int settingType)
    {
        return ngSettingSystem.GetSetting(settingType);
    }
}
