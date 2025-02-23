using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Status
{
    public interface IStatusHandler
    {
        public delegate void StatusMessageHandler(byte[] status);

        public event StatusMessageHandler OnStatus;

        bool CheckSvrRetCode(int errcode);
    }
}
