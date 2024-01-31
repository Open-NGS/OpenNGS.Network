using OpenNGS.Exchange.Data;
using OpenNGS.Shop.Common;
using OpenNGS.Shop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Systems
{
    public interface IShopSystem
    {
        public SHOP_RESULT_TYPE BuyItem(BuyItemInfo item);
        public SHOP_RESULT_TYPE SellItem(SellItemInfo item);
        public Dictionary<uint, Good> GetGoodsInfo(uint currentShopId, uint shelfId);
        public Dictionary<uint, Shelf> GetShelfsInfo(uint currentShopId);
    }
}
