
namespace OpenNGS.Systems
{
    public static class ItemStaticData
    {
        public static Table<OpenNGS.Item.Data.Item, uint> items = new Table<Item.Data.Item, uint>((item) => { return item.Id; }, false);
        public static void Init() { }
    }
}
