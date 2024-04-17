
using System.Linq;

namespace OpenNGS.Item.Data
{
    public partial class ItemContainer
    {

        public void AddItem(ItemSaveData item)
        {
            items.Add(item);
        }
        public void RemoveItem(ItemSaveData item)
        {
            items.Remove(item);
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
        public ItemSaveData GetItemById(int itemId)
        {
            return items.FirstOrDefault(i => i.GUID == itemId);
        }

    }
}