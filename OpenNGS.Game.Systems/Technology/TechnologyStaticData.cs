using System.Collections;
using System.Collections.Generic;

namespace OpenNGS.Systems
{
    public static class TechnologyStaticData
    {
        public static Table<OpenNGS.Technology.Data.NodeData, uint> technologyNodes = new Table<Technology.Data.NodeData, uint>((item) => { return item.ID; }, false);

        public static void Init() { }
    }
}
