using OpenNGS.Exchange.Common;
using OpenNGS.Rank.Common;

namespace OpenNGS.Systems
{
    public interface IRankSystem
    {
        public void GetRankInfo(uint nLevelID, RANK_DIFFICULT_TYPE _typ);
    }
}