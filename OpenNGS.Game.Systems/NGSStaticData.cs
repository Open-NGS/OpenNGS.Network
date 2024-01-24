using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.Systems
{
    public static class NGSStaticData
    {
        public static Table<OpenNGS.Shop.Data.Shop, uint> shops = new Table<OpenNGS.Shop.Data.Shop, uint>((item) => { return item.ID; }, false);
        public static Table<OpenNGS.Shop.Data.Shelf, uint> shelfs = new Table<OpenNGS.Shop.Data.Shelf, uint>((item) => { return item.ID; }, false);
        public static Table<OpenNGS.Shop.Data.Good, uint> goods = new Table<OpenNGS.Shop.Data.Good, uint>((item) => { return item.ID; }, false);

        public static void Init() { }
    }
}
