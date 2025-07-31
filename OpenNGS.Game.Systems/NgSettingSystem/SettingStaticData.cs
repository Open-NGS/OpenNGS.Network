using System.Collections;
using System.Collections.Generic;

namespace OpenNGS.Systems
{
    public static class SettingStaticData
    {
        public static Table<OpenNGS.Setting.Data.UserSettingValueState, uint> settingValueState = new Table<Setting.Data.UserSettingValueState, uint>((item) => { return item.ID; }, false);

        public static void Init() { }
    }
}
