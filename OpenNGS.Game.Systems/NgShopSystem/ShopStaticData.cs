using System.Collections;
using System.Collections.Generic;

namespace OpenNGS.Systems
{
    public static class ShopStaticData
    {
        public static Table<OpenNGS.Shop.Data.Shop, uint> shops = new Table<OpenNGS.Shop.Data.Shop, uint>((item) => { return item.ID; }, false);
        public static Table<OpenNGS.Shop.Data.Shelf, uint, uint> shelfs = new Table<OpenNGS.Shop.Data.Shelf, uint, uint>((item) => { return item.ShopId; }, (item) => { return item.ID; }, false);
        public static Table<OpenNGS.Shop.Data.Good, uint, uint> goods = new Table<OpenNGS.Shop.Data.Good, uint, uint>((item) => { return item.ShelfId; }, (item) => { return item.ID; }, false);
        public static Table<OpenNGS.Shop.Data.Shelf, uint> shelfDatas = new Table<OpenNGS.Shop.Data.Shelf, uint>((item) => { return item.ID; }, false);
        public static Table<OpenNGS.Shop.Data.Good, uint> goodDatas = new Table<OpenNGS.Shop.Data.Good, uint>((item) => { return item.ID; }, false);

        public static void Init() { }
    }
}
