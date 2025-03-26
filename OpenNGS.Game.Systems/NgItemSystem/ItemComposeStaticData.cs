namespace OpenNGS.Systems
{
    public static class ItemComposeStaticData
    {
        public static Table<OpenNGS.Item.Data.ComposeCostInfo, uint> ComposeInfo = new Table<Item.Data.ComposeCostInfo, uint>((item) => { return item.ID; }, false);
        public static void Init() { }
    }
}
