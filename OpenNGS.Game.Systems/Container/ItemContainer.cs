
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
                Col.Add(new ItemColumn { ColIdx = colIdx, ItemSaveStates = new List<ItemSaveState>(), Capacity = capacity });
            }
        }
        public void AddItem(bags item)
        {
            bagDict.Add(item);
        }
        public void RemoveItem(bags item)
        {
            bagDict.Remove(item);
        }
        public void AddStashItem(stashs item)
        {
            stashDict.Add(item);
        }
        public void RemoveStashItem(stashs item)
        {
            stashDict.Remove(item);
        }
        public void AddEquips(equips item)
        {
            equipDict.Add(item);
        }
        public void RemoveEquips(equips item)
        {
            equipDict.Remove(item);
        }
        public void UpdateItem(ItemSaveData Item, uint num)
        {
            Item.Count = num;
        }
    }
}