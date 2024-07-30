using OpenNGS.Setting.Data;
using OpenNGSCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INgSettingSystem 
{
    public void ChangeSetting(int settingType, int value);
    public UserSettingValueState GetSetting(int settingType);
    public void AddItemContainer(UserSettingContainer container);
    public void AddActionOnSettingChange(Action<int,int> ac);
}
