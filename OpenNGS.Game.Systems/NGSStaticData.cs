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
        public static Table<OpenNGS.Dialog.Data.DialogTalk, uint> DialogTalk = new Table<OpenNGS.Dialog.Data.DialogTalk, uint>((item) => { return item.DialogueDataID;}, false);
        //public static Table<OpenNGS.Make.Data.ItemInfo, uint> items = new Table<OpenNGS.Make.Data.ItemInfo, uint>((item) => { return item.ID; }, false);
        //public static Table<OpenNGS.Make.Data.MakeInfo, uint> makes = new Table<OpenNGS.Make.Data.MakeInfo, uint>((item) => { return item.ID; }, false);
        public static ListTableBase<OpenNGS.Shop.Data.Good, uint> shelfGoods = new ListTableBase<OpenNGS.Shop.Data.Good, uint>((item) => { return item.ShelfId; }, false);
        public static ListTableBase<OpenNGS.Shop.Data.ShopSell, uint> sells = new ListTableBase<OpenNGS.Shop.Data.ShopSell, uint>((item) => { return item.ShopID; }, false);

        public static void Init() { }
    }
}
