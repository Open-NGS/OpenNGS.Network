using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Neptune
{
    public class OpRecord
    {
        public struct OpExt
        {
            public int Index;
            public int ExIndex;
        };
        public int OpCode;
        public int Round;
        public int Tick;
        public int RoleIdx;
        public OpExt Ext;
    }
}
