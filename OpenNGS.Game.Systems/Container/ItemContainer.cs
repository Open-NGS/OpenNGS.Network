
using System.Linq;

namespace OpenNGS.Item.Data
{
    public partial class ItemContainer
    {

        public void AddItem(ItemSaveData item)
        {
            stashItems.Add(item);
        }
        public void RemoveItem(ItemSaveData item)
        {
            stashItems.Remove(item);
        }
        public void AddBagItem(ItemSaveData item)
        {
            bagItems.Add(item);
        }
        public void RemoveBagItem(ItemSaveData item)
        {
            bagItems.Remove(item);
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
            return stashItems.FirstOrDefault(i => i.GUID == itemId);
        }

    }
}