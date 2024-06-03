
using System.Linq;

namespace OpenNGS.Item.Data
{
    public partial class ItemContainer
    {

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