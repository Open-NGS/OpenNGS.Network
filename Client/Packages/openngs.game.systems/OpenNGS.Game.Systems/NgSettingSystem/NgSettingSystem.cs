using OpenNGS.Item.Data;
using OpenNGS.Setting.Data;
using OpenNGS.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using Systems;

public class NgSettingSystem : GameSubSystem<NgSettingSystem>,INgSettingSystem
{
    private Action<int, int> OnSettingChange;
    private UserSettingContainer settingContainer;

    protected override void OnCreate()
    {
        base.OnCreate();
    }
    public void ChangeSetting(int settingType, int value)
    {
        settingContainer.ChangeSetting(settingType, value);
        if (OnSettingChange != null)
        {
            OnSettingChange.Invoke(settingType, value);
        }
    }

    public UserSettingValueState GetSetting(int settingType)
    {
        return settingContainer.GetSetting(settingType);
    }
    public void AddSettingContainer(UserSettingContainer Container)
    {
        if (Container != null)
        {
            settingContainer = Container;
        }
        else
        {
            settingContainer = new UserSettingContainer();
        }
    }
    public override string GetSystemName()
    {
        return "com.openngs.system.NgSettingSystem";
    }

    public void AddActionOnSettingChange(Action<int, int> ac)
    {
        OnSettingChange += ac;
    }
}
