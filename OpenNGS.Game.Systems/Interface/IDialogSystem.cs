using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Systems
{
    public interface IDialogSystem
    {
        public void SetDialogID(int dialogid);
        public int GetDialogID();
    }
}
