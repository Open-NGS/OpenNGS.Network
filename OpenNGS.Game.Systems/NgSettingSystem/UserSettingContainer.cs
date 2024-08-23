using OpenNGS.Setting.Common;
using OpenNGS.Setting.Data;
using System.Collections.Generic;

namespace OpenNGS.Setting.Data
{
    public partial class UserSettingContainer
    {
        public void AddSetting(UserSettingValueState state)
        {
            if (ValueState != null)
            {
                ValueState.Add(state);
            }
        }
        public void ChangeSetting(int settingType, int value)
        {
            if (ValueState != null)
            {
                UserSettingValueState item = ValueState.Find(item => (item.UserSettingType == settingType));
                if (item != null)
                {
                    item.Value = value;
                }
            }
        }
        public UserSettingValueState GetSetting(int settingType)
        {
            if (ValueState != null)
            {
                UserSettingValueState item = ValueState.Find(item => (item.UserSettingType == settingType));
                return item;
            }
            else
            {
                return null;
            }
        }
    }
}
