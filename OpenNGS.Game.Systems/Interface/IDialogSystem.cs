using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Systems
{
    public interface IDialogSystem
    {
        public void SetDialogID(uint dialogid);
        public uint GetDialogID();
    }
}
