using OpenNGS.Exchange.Common;
using OpenNGS.Rank.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.Systems
{
    public interface IRankSystem
    {
        public void GetRankInfo(uint nLevelID, RANK_DIFFICULT_TYPE _typ);
    }
}