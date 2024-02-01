using OpenNGS.Exchange.Common;
using OpenNGS.Make.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.Systems
{
    public interface IMakeSystem
    {
        public EXCHANGE_RESULT_TYPE Forged(uint makeGridId, OpenNGS.Item.Data.Item item);
    }
}