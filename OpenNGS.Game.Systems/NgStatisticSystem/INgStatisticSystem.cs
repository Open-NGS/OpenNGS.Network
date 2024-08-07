using OpenNGS.Statistic.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.Systems
{
    public interface INgStatisticSystem
    {
        void AddStatContainer(StatisticContainer Container);
        void StatByStatisticID(uint statId, double val);
        void Stat(STAT_EVENT @event, uint category, uint type, uint subType, uint objId, double value);
        void ResetStatsByEvent(STAT_EVENT @event);
        int GetStatInt(uint id);
        double GetStat(uint id);
        void ResetStat(uint id);
        void RegisterEventHandler(INgStatisticEvent item);
    }

    public interface INgStatisticEvent
    {
        uint StatID { get; }
        void OnStatValueChange(uint statId, ulong value);
    }

}