using OpenNGS.Statistic.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OpenNGS.Systems
{
    public interface IStatSystem
    {
        public void UpdateStat(STAT_EVENT _statEvt, ulong lParam1, ulong lParam2);
    }
}