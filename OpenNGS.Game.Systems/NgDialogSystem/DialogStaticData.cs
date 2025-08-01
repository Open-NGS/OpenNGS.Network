using System.Collections;
using System.Collections.Generic;

namespace OpenNGS.Systems
{
    public static class DialogStaticData
    {
        public static Table<OpenNGS.Dialog.Data.DialogTalkConfig, uint, uint> DialogTalk = new Table<OpenNGS.Dialog.Data.DialogTalkConfig, uint,uint>((item) => { return item.DialogID; }, (item) => { return item.TalkID; }, false);
        public static Table<OpenNGS.Dialog.Data.DialogConfig, uint> Dialogue = new Table<OpenNGS.Dialog.Data.DialogConfig, uint>((item) => { return item.ID; }, false);
        public static Table<OpenNGS.Dialog.Data.DialogChoiceConfig, uint> Choice = new Table<OpenNGS.Dialog.Data.DialogChoiceConfig, uint>((item) => { return item.ChoiceID; }, false);
        public static Table<OpenNGS.Dialog.Data.ChoiceResultConfig, uint> ChoiceRes = new Table<OpenNGS.Dialog.Data.ChoiceResultConfig, uint>((item) => { return item.ResultID; }, false);

        public static void Init() { }
    }
}
