using System.Collections;
using System.Collections.Generic;

namespace OpenNGS.Systems
{
    public static class DialogStaticData
    {
        public static Table<OpenNGS.Dialog.Data.DialogTalk, uint> DialogTalk = new Table<OpenNGS.Dialog.Data.DialogTalk, uint>((item) => { return item.DialogTalkID; }, false);
        public static Table<OpenNGS.Dialog.Data.DialogList, uint> Dialogue = new Table<OpenNGS.Dialog.Data.DialogList, uint>((item) => { return item.DialogID; }, false);
        public static Table<OpenNGS.Dialog.Data.DialogChoice, uint> Choice = new Table<OpenNGS.Dialog.Data.DialogChoice, uint>((item) => { return item.DialogChoiceID; }, false);

        public static void Init() { }
    }
}
