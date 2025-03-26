using System.Collections;
using System.Collections.Generic;

namespace OpenNGS.Systems
{
    public static class QuestStaticData
    {
        public static Table<OpenNGS.Quest.Data.QuestGroup, uint> QuestGroup = new Table<OpenNGS.Quest.Data.QuestGroup, uint>((item) => { return item.QuestGroupID; }, false);
        public static Table<OpenNGS.Quest.Data.Quest, uint, uint> Quest = new Table<OpenNGS.Quest.Data.Quest, uint, uint>((item) => { return item.GroupID; }, (item) => { return item.QuestID; }, false);

        public static void Init() { }
    }
}