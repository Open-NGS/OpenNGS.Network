using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neptune.GameData
{
    /// <summary>
    /// AttrTransData
    /// </summary>
    public class AttrTransData
    {
        public RoleAttribute Base { get; set; }
        public RoleAttribute Trans { get; set; }
        public float Ratio { get; set; }
    }
}
