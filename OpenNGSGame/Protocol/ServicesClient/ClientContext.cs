using OpenNGS.Extension;
using OpenNGSCommon;
using System;
using System.Collections.Generic;
using Systems;

namespace Rpc
{
    public class ClientContextBase : OpenNGS.IRPC.ClientContext
    {
        public static ulong UIN;

        public ClientContextBase() : base()
        {
            ReqMeta.TryAdd("uin", UIN.ToString());
            
            SetAction("com.openngs.xr.status_system", (byte[] val) => { StatusSystem.Instance.OnStatus(FileExtension.Deserialize<StatusDataList>(val)); });
        }
    }
}
