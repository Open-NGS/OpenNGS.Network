using System.Collections;
using System.Collections.Generic;

namespace OpenNGS.Systems
{
    public static class UIStaticData
    {
#if UNITY_5_3_OR_NEWER
        public static Table<OpenNGS.UI.Data.UIConfig, uint> uiConfig = new Table<UI.Data.UIConfig, uint>((item) => { return item.Id; }, false);
#endif
        public static void Init() { }
    }
}