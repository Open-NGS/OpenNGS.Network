using OpenNGS.Exchange.Common;
using System.Collections;
using System.Collections.Generic;

namespace OpenNGS.Systems
{
    public interface IMakeSystem
    {
        // 制作
        public EXCHANGE_RESULT_TYPE Make();
        // 材料
        public void MakeMaterials(OpenNGS.Item.Common.ItemData itemData);
        // 图纸
        public void MakeDesign(OpenNGS.Item.Common.ItemData itemInfo);
        //幸运石
        public void LuckyStone(OpenNGS.Item.Common.ItemData itemInfo);


    }
}