using OpenNGS.Item.Data;
using OpenNGS.Statistic.Common;
using OpenNGS.Statistic.Data;
using System.Collections.Generic;


namespace OpenNGS.Systems
{
    public interface IStatisticSystem
    {
        void AddStatContainer(StatisticContainer Container);
        void Stat(STAT_EVENT @event, int category, int type, int subType, int objId, double value);
        void ResetStatsByEvent(STAT_EVENT @event);
        int GetStatInt(int id);
        double GetStat(int id);
        void ResetStat(int id);
        void RegisterEventHandler(IStatisticEvent item);
    }

    public interface IStatisticEvent
    {
        int StatID { get; }
        void OnStatValueChange(uint statId, ulong value);
    }

}