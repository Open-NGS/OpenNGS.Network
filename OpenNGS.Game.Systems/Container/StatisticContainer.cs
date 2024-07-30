using OpenNGS.Statistic.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OpenNGS.Statistic.Common
{
    public partial class StatisticContainer
    {
        public void SetStat(uint nID, ulong uVal)
        {
            if( StatisticSaveData.ContainsKey( nID ) == true )
            {
                StatisticSaveData[nID].totalval = uVal;
            }
            else
            {
                StatValue _val = new StatValue();
                _val.id = nID;
                _val.totalval = uVal;
                StatisticSaveData[nID] = _val;
            }
        }
    }
}