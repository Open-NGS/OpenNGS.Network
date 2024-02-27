using OpenNGS.Exchange.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.Systems
{
    public interface IMakeSystem
    {
        // ÖÆ×÷
        public EXCHANGE_RESULT_TYPE Material();
        // ²ÄÁÏ
        public void MakeMaterials(OpenNGS.Item.Common.ItemData itemData);
        // Í¼Ö½
        public void MkaeDesign(OpenNGS.Item.Common.ItemData itemInfo);



    }
}