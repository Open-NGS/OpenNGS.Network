using OpenNGS.Statistic.Common;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenNGS.Systems
{
    public interface IStatSystem
    {
        public void UpdateStat(STAT_EVENT _statEvt, ulong lParam1, ulong lParam2);
        public bool GetStatValueByID(uint nStatID, out ulong ulValue);
        public bool GetStatValue(STAT_EVENT eStatEvt, uint nStatID, out ulong ulValue);
        public void Subscribe(int EventID, Action handler);
        public void Unsubscribe(int EventID, Action handler);
    }
}