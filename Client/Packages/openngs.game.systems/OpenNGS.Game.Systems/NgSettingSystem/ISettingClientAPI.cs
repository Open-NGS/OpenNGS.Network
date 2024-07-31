using OpenNGS.Setting.Data;
using System.Collections;
using System.Collections.Generic;
using System;

public interface ISettingClientAPI
{
    public void ChangeSetting(int settingType, int value);
    public UserSettingValueState GetSetting(int settingType);
    public void AddSettingContainer(UserSettingContainer container);
    public void AddActionOnSettingChange(Action<int, int> ac);
}
