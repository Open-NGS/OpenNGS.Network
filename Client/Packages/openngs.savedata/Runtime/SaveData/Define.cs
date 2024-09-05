using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNGS.SaveData
{

    public enum SaveDataMode
    {
        /// <summary>
        /// 单存档模式，默认只有一个存档，无须新建
        /// </summary>
        Single,

        /// <summary>
        /// 固定槽位模式，存档预设固定容量槽位，槽位顺序不变。Slot数量固定。
        /// </summary>
        FixedSlot,

        /// <summary>
        /// 多存档模式，任意多个存档，Slots数量默认为当前实际存档数量。
        /// </summary>
        Multiple, 
    }


    public enum SaveDataResult
    {
        Success = 0,
        Recovered = 1,
        NotFound = 2,
        IOError = 3,
        InvalidData = 4,
        VerifyFailed = 5,
    }
}
