using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface INetworkModuleExtension
{


    public void Init();

    void UnInit();

    public void MsgStreamProcess(int msgid, Type rsp_type, Services.MessageContext context, int errorcode, MemoryStream ms);

}