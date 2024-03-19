using OpenNGS.Extension;
using OpenNGS.Serialization;
using OpenNGSCommon;
using System;
using System.Collections.Generic;
using Systems;

namespace Rpc
{
    public class ClientContextBase : OpenNGS.ERPC.ClientContext
    {
        public static ulong UIN;

        public ClientContextBase() : base()
        {
            ReqMeta.TryAdd("uin", UIN.ToString());
            
            SetAction("com.openngs.xr.status_system", (byte[] val) => { StatusSystem.Instance.OnStatus(FileSerializer.Deserialize<StatusDataList>(val)); });
        }
    }
}
