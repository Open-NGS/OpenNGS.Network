using System.Collections;
using System.Collections.Generic;

namespace OpenNGS.Systems
{
    public static class BlindBoxStaticData
    {
        public static Table<OpenNGS.BlindBox.Data.Drop, uint> drops = new Table<OpenNGS.BlindBox.Data.Drop, uint>((item) => { return item.DropID; }, false);
        public static Table<OpenNGS.BlindBox.Data.DropRule, uint> droprules = new Table<OpenNGS.BlindBox.Data.DropRule, uint>((item) => { return item.DropRuleID; }, false);
        public static ListTableBase<OpenNGS.BlindBox.Data.DropGroup, uint> dropgroups = new ListTableBase<OpenNGS.BlindBox.Data.DropGroup, uint>((item) => { return item.DropGroupID; }, false);

        public static void Init() { }
    }
}
