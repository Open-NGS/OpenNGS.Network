
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static UnityEditor.Experimental.GraphView.Port;

namespace OpenNGS.Item.Data
{
    public partial class ItemContainer
    {
        public void InitializeColumns(uint colIdx, uint capacity)
        {
            if (!Col.Any(col => col.ColIdx == colIdx))
            {
                Col.Add(new ItemColumn { ColIdx = colIdx, Capacity = capacity });
            }
        }
    }
}