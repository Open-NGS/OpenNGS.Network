using OpenNGS.Effect.Data;
using System.Collections;
using System.Collections.Generic;

namespace OpenNGS.Systems
{
    public static class EffectStaticData
    {
        public static Table<NGSCondition, uint> conditions = new Table<NGSCondition, uint>((item) => { return item.ID; }, false);
        public static Table<NGSEffect, uint> effects = new Table<NGSEffect, uint>((item) => { return item.EffectID; }, false);

        public static void Init() { }
    }
}
