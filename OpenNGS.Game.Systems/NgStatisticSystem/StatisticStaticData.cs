
namespace OpenNGS.Systems
{
    public static class StatisticStaticData
    {
        public static Table<OpenNGS.Statistic.Data.StatData, uint> s_statDatas = new Table<Statistic.Data.StatData, uint>((item) => { return item.Id; }, false);

        public static void Init() { }
    }
}