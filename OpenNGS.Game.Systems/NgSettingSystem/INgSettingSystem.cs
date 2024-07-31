using OpenNGS.Setting.Data;
using System;


public interface INgSettingSystem
{
    public void ChangeSetting(int settingType, int value);
    public UserSettingValueState GetSetting(int settingType);
    public void AddSettingContainer(UserSettingContainer container);
    public void AddActionOnSettingChange(Action<int, int> ac);
}
